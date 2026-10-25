using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Nodes;

public class NegatedAsAlternativeTests
{
	[Test]
	public async Task WhenMemberDoesNotSatisfyTheExpectations_ShouldSucceed()
	{
		string subject = "bcd";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasFirstChar(it, c => c.IsEqualTo('a')));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenMemberIsMissing_ShouldSucceed()
	{
		string subject = "";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasFirstChar(it, c => c.IsEqualTo('a')));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenMemberSatisfiesTheExpectations_ShouldExplainTheFailureWithTheMember()
	{
		string subject = "abc";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasFirstChar(it, c => c.IsEqualTo('a')));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not have a first char or first char is not equal to 'a',
			             but first char was 'a'
			             """);
	}

	[Test]
	public async Task WhenNotNegated_ShouldKeepTheMemberText()
	{
		string subject = "bcd";

		async Task Act()
			=> await HasFirstChar(That(subject), c => c.IsEqualTo('a'));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a first char which is equal to 'a',
			             but first char was 'b'
			             """);
	}

	[Test]
	public async Task WhenSubjectFailsBothWays_ShouldExplainTheFailureWithTheSubject()
	{
		string? subject = null;

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasFirstChar(it, c => c.IsEqualTo('a')));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not have a first char or first char is not equal to 'a',
			             but it was <null>
			             """)
			.Because("the expectations on the member were not applied, but their text still follows the negation");
	}

	private static AndOrResult<string?, IThat<string?>> HasFirstChar(
		IThat<string?> subject,
		Action<IThat<char>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForMember(MemberAccessor<string?, char>.FromFuncWithName(s => s![0], "first char"),
					static (_, stringBuilder) => stringBuilder.Append(" which "))
				.Validate((it, grammars) => new HasFirstCharConstraint(it, grammars))
				.NegatedAsAlternative(static (member, stringBuilder)
					=> stringBuilder.Append(" or ").Append(member).Append(' '))
				.AddExpectations(e => expectations(new ThatSubject<char>(e))),
			subject);

	private sealed class HasFirstCharConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual is { Length: > 0, } ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a first char");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was empty");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a first char");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had a first char");
	}
}
