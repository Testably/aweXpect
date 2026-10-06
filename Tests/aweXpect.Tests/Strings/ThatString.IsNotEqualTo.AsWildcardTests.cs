namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsWildcardTests
		{
			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = "*ME ME*";

				async Task Act()
					=> await That(subject).IsNotEqualTo(pattern)
						.AsWildcard().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not match "*ME ME*" ignoring case,
					             but it was "some message"
					             """);
			}

			[Test]
			[Arguments("", false)]
			[Arguments("a", true)]
			[Arguments("\n", true)]
			public async Task WhenPatternIsEmpty_ShouldOnlyFailForTheEmptySubject(
				string subject, bool expectSuccess)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo("").AsWildcard();

				await That(Act).Throws().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not match "",
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("an empty wildcard pattern has the well-defined meaning of the empty string");
			}

			[Test]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsWildcard();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' wildcard pattern cannot be null.").AsPrefix()
					.Because("a missing pattern matches no subject, so the negated expectation could never fail");
			}

			[Test]
			public async Task WhenPatternIsNullAndSubjectIsNull_ShouldThrowArgumentNullException()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsWildcard();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' wildcard pattern cannot be null.").AsPrefix()
					.Because("the missing pattern is a setup error that outranks the null subject");
			}

			[Test]
			public async Task WhenPatternMatchesOnlyOneLineOfTheSubject_ShouldSucceed()
			{
				string subject = "xyz\nabc";

				async Task Act()
					=> await That(subject).IsNotEqualTo("abc").AsWildcard();

				await That(Act).DoesNotThrow()
					.Because("the pattern has to cover the complete subject, not one of its lines");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo("p").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match "p",
					             but it was <null>
					             """)
					.Because("a null has no content to inspect, just as for the dedicated negated expectations");
			}
		}
	}
}
