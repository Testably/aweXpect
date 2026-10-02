using System;
using System.Text;
using aweXpect.Results;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextCollectorTests
{
	[Fact]
	public async Task And_ShouldOnlyShowTheContextOfTheFailingPart()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 1))
				.And.Whose(x => x.Second, second => second.MatchesValue("Value", 3));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose First matches Value 1 and whose Second matches Value 3,
			             but Second did not

			             Value (Second):
			             2
			             """);
	}

	[Fact]
	public async Task AppendContexts_WhenTheExpectationIsMet_ShouldNotBeCalled()
	{
		CallCounter counter = new();
		Pair subject = new(1, 2);

		await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 1, counter))
			.And.Whose(x => x.Second, second => second.MatchesValue("Value", 2, counter));

		await That(counter.Calls).IsEqualTo(0);
	}

	[Fact]
	public async Task DoesNotComplyWith_ShouldShowTheContextOfTheNegatedPartThatFails()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it
				.Whose(x => x.First, first => first.MatchesValue("Value", 3))
				.Or.Whose(x => x.Second, second => second.MatchesValue("Value", 2)));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose First does not match Value 3 and whose Second does not match Value 2,
			             but Second did

			             Value (Second):
			             2
			             """);
	}

	[Fact]
	public async Task DoublyNegated_ShouldShowTheContextOfThePartThatFailsAgain()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.DoesNotComplyWith(inner => inner
				.Whose(x => x.First, first => first.MatchesValue("Value", 1))
				.And.Whose(x => x.Second, second => second.MatchesValue("Value", 3))));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose First matches Value 1 and whose Second matches Value 3,
			             but Second did not

			             Value (Second):
			             2
			             """);
	}

	[Fact]
	public async Task ExpectThatAll_ShouldPrefixTheContextsWithTheNumberOfTheExpectation()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await ThatAll(
				That(subject.First).MatchesValue("Value", 1),
				That(subject.Second).MatchesValue("Value", 3));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task NestedMember_ShouldLabelTheContextWithTheMemberPath()
	{
		Outer subject = new(new Pair(1, 2));

		async Task Act()
			=> await That(subject).Whose(x => x.Inner, inner => inner
				.Whose(x => x.Second, second => second.MatchesValue("Value", 3)));

		await That(Act).Throws<XunitException>()
			.WithMessage("*Value (Inner.Second):*2").AsWildcard();
	}

	[Fact]
	public async Task Or_WhenBothPartsFail_ShouldShowBothContexts()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.First, first => first.MatchesValue("Value", 3))
				.Or.Whose(x => x.Second, second => second.MatchesValue("Value", 3));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task Priority_ShouldOrderTheContexts()
	{
		async Task Act()
			=> await That(1).MatchesValue("Low", 2, priority: -1).And.MatchesValue("High", 2, priority: 1);

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task SameTitleAndSubject_WithDifferentContent_ShouldNumberThem()
	{
		async Task Act()
			=> await That(1).MatchesValue("Value", 2, content: _ => "a").And.MatchesValue("Value", 3, content: _ => "b");

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task SameTitleAndSubject_WithSameContent_ShouldShowItOnce()
	{
		async Task Act()
			=> await That(1).MatchesValue("Value", 2).And.MatchesValue("Value", 3);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that 1
			             matches Value 2 and matches Value 3,
			             but it did not

			             Value:
			             1
			             """);
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
