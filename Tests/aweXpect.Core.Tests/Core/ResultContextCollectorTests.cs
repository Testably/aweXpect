using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextCollectorTests
{
	[Test]
	public async Task And_ShouldOnlyShowTheContextOfTheFailingPart()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 1))
				.And.Whose(x => x.Second, second => second.MatchesValue("Value", 3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose First matches Value 1 and whose Second matches Value 3,
			             but Second did not

			             Value (Second):
			             2
			             """);
	}

	[Test]
	public async Task AppendContexts_WhenTheExpectationIsMet_ShouldNotBeCalled()
	{
		CallCounter counter = new();
		Pair subject = new(1, 2);

		await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 1, counter))
			.And.Whose(x => x.Second, second => second.MatchesValue("Value", 2, counter));

		await That(counter.Calls).IsEqualTo(0);
	}

	[Test]
	public async Task DoesNotComplyWith_ShouldShowTheContextOfTheNegatedPartThatFails()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it
				.Whose(x => x.First, first => first.MatchesValue("Value", 3))
				.Or.Whose(x => x.Second, second => second.MatchesValue("Value", 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose First does not match Value 3 and whose Second does not match Value 2,
			             but Second did

			             Value (Second):
			             2
			             """);
	}

	[Test]
	public async Task DoublyNegated_ShouldShowTheContextOfThePartThatFailsAgain()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.DoesNotComplyWith(inner => inner
				.Whose(x => x.First, first => first.MatchesValue("Value", 1))
				.And.Whose(x => x.Second, second => second.MatchesValue("Value", 3))));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose First matches Value 1 and whose Second matches Value 3,
			             but Second did not

			             Value (Second):
			             2
			             """);
	}

	[Test]
	public async Task ExpectThatAll_ShouldPrefixTheContextsWithTheNumberOfTheExpectation()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await ThatAll(
				That(subject.First).MatchesValue("Value", 1),
				That(subject.Second).MatchesValue("Value", 3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject.First matches Value 1
			              [02] Expected that subject.Second matches Value 3
			             but
			              [02] it did not

			             [02] Value:
			             2
			             """);
	}

	[Test]
	public async Task ItemExpectations_ForAll_ShouldShowTheContextsOfTheFirstNotMatchingItem()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).All().ComplyWith(item => item.MatchesValue("Value", 1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches Value 1 for all items,
			             but only 1 of 3 did

			             Not matching items:
			             [2, 3]

			             Collection:
			             [1, 2, 3]

			             Value (item [1]):
			             2
			             """);
	}

	[Test]
	public async Task ItemExpectations_ForNone_ShouldShowTheContextsOfTheFirstMatchingItem()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).None().ComplyWith(item => item.MatchesValue("Value", 3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches Value 3 for no items,
			             but 1 of 3 did

			             Matching items:
			             [3]

			             Collection:
			             [1, 2, 3]

			             Value (item [2]):
			             3
			             """);
	}

	[Test]
	public async Task NestedMember_ShouldLabelTheContextWithTheMemberPath()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).Whose(x => x.Inner, inner => inner
				.Whose(x => x.Second, second => second.MatchesValue("Value", 3)));

		await That(Act).Throws<FailException>()
			.WithMessage("*Value (Inner.Second):*2").AsWildcard();
	}

	[Test]
	[Arguments("Inner.Second", "Inner.Inner.Second")]
	[Arguments("Inner[1]", "Inner.Inner[1]")]
	public async Task NestedMemberWithNameThatStartsWithTheParent_ShouldNotReplaceIt(string name,
		string expectedLabel)
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).WhoseNamed("Inner", x => x.Inner, inner => inner
				.WhoseNamed(name, x => x.Second, second => second.MatchesValue("Value", 3)));

		await That(Act).Throws<FailException>()
			.WithMessage($"*Value ({expectedLabel}):*2").AsWildcard()
			.Because("a member of a self-referential type, e.g. Parent.Value within Parent, is no path continuation");
	}

	[Test]
	public async Task Or_WhenBothPartsFail_ShouldShowBothContexts()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 3))
				.Or.Whose(x => x.Second, second => second.MatchesValue("Value", 3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose First matches Value 3 or whose Second matches Value 3,
			             but First did not and Second did not

			             Value (First):
			             1

			             Value (Second):
			             2
			             """);
	}

	[Test]
	public async Task PathMember_ShouldLabelTheContextWithThePath()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).WhoseAtPath("$.inner.second", x => x.Inner.Second,
				second => second.MatchesValue("Value", 3));

		await That(Act).Throws<FailException>()
			.WithMessage("*Value ($.inner.second):*2").AsWildcard();
	}

	[Test]
	public async Task PathMember_WithinMember_ShouldFollowItsLabelWithoutTheRoot()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).Whose(x => x.Inner, inner => inner
				.WhoseAtPath("$.second", x => x!.Second, second => second.MatchesValue("Value", 3)));

		await That(Act).Throws<FailException>()
			.WithMessage("*Value (Inner.second):*2").AsWildcard();
	}

	[Test]
	public async Task PathMember_WithinPathMember_ShouldReplaceItsLabel()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).WhoseAtPath("$.inner", x => x.Inner, inner => inner
				.WhoseAtPath("$.inner.second", x => x.Second, second => second.MatchesValue("Value", 3)));

		await That(Act).Throws<FailException>()
			.WithMessage("*Value ($.inner.second):*2").AsWildcard()
			.Because("the nested path continues the path of the same root");
	}

	[Test]
	public async Task PathMember_WithinPathMemberWithinMember_ShouldOnlyReplaceThePathPart()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).WhoseNamed("Outer", x => x, outer => outer
				.WhoseAtPath("$.inner", x => x.Inner, inner => inner
					.WhoseAtPath("$.inner.second", x => x.Second, second => second.MatchesValue("Value", 3))));

		await That(Act).Throws<FailException>()
			.WithMessage("*Value (Outer.inner.second):*2").AsWildcard();
	}

	[Test]
	public async Task Priority_ShouldOrderTheContexts()
	{
		async Task Act()
			=> await That(1).MatchesValue("Low", 2, priority: -1).And.MatchesValue("High", 2, priority: 1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             matches Low 2 and matches High 2,
			             but it did not

			             High:
			             1

			             Low:
			             1
			             """);
	}

	[Test]
	public async Task SameTitleAndSubject_WithDifferentContent_ShouldNumberThem()
	{
		async Task Act()
			=> await That(1).MatchesValue("Value", 2, content: _ => "a").And.MatchesValue("Value", 3, content: _ => "b");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             matches Value 2 and matches Value 3,
			             but it did not

			             Value #1:
			             a

			             Value #2:
			             b
			             """);
	}

	[Test]
	public async Task SameTitleAndSubject_WithSameContent_ShouldShowItOnce()
	{
		async Task Act()
			=> await That(1).MatchesValue("Value", 2).And.MatchesValue("Value", 3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             matches Value 2 and matches Value 3,
			             but it did not

			             Value:
			             1
			             """);
	}

	[Test]
	public async Task VisitItem_ShouldLabelTheContextsWithTheItemAfterThoseOfTheCollection()
	{
		int[] subject = [1, 2,];

		async Task Act()
			=> await That(subject).HasItemThat(x => x.MatchesValue("Value", 3, priority: 1)).AtIndex(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has an item that matches Value 3 at index 1,
			             but it had item 2 at index 1

			             Collection:
			             [1, 2]

			             Value (item [1]):
			             2
			             """)
			.Because("the contexts of an item follow those of the collection, whatever their priority");
	}

	private sealed record Pair(int First, int Second);

	private sealed record Outer(Pair Inner);
}

internal sealed class CallCounter
{
	public int Calls { get; set; }
}

internal static class ResultContextCollectorTestExtensions
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member that the <paramref name="accessor" /> selects, which
	///     is called by the <paramref name="path" /> with the root <c>$</c>.
	/// </summary>
	public static AndOrResult<TSource, IThat<TSource>> WhoseAtPath<TSource, TMember>(this IThat<TSource> subject,
		string path, Func<TSource, TMember> accessor, Action<IThat<TMember>> expectations)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(MemberAccessor<TSource, TMember>.FromFuncWithPath(accessor, path, "$"),
				(member, stringBuilder) => stringBuilder.Append("whose ").Append(member).Append(' '))
			.AddExpectations(e => expectations(new ThatSubject<TMember>(e)));
		return new AndOrResult<TSource, IThat<TSource>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member that the <paramref name="accessor" /> selects, which
	///     is called <paramref name="name" />.
	/// </summary>
	public static AndOrResult<TSource, IThat<TSource>> WhoseNamed<TSource, TMember>(this IThat<TSource> subject,
		string name, Func<TSource, TMember> accessor, Action<IThat<TMember>> expectations)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(MemberAccessor<TSource, TMember>.FromFuncWithName(accessor, name),
				(member, stringBuilder) => stringBuilder.Append("whose ").Append(member).Append(' '))
			.AddExpectations(e => expectations(new ThatSubject<TMember>(e)));
		return new AndOrResult<TSource, IThat<TSource>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value and adds the subject as context
	///     with the <paramref name="title" />.
	/// </summary>
	public static AndOrResult<int, IThat<int>> MatchesValue(this IThat<int> subject, string title, int expected,
		CallCounter? counter = null, Func<int, string>? content = null, int priority = 0)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new MatchesValueConstraint(it, grammars, title, expected, counter, content, priority)),
			subject);

	private sealed class MatchesValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string title,
		int expected,
		CallCounter? counter,
		Func<int, string>? content,
		int priority)
		: ConstraintResult.WithNotNullValue<int>(it, grammars),
			IValueConstraint<int>
	{
		public ConstraintResult IsMetBy(int actual)
		{
			Actual = actual;
			Outcome = actual == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (counter is not null)
			{
				counter.Calls++;
			}

			int actual = Actual;
			contexts.Add(new ResultContext.SyncCallback(title,
				() => content?.Invoke(actual) ?? actual.ToString(), priority));
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("matches ").Append(title).Append(' ').Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not match ").Append(title).Append(' ').Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
