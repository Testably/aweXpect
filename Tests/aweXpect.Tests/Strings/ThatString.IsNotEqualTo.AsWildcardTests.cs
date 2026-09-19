namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsWildcardTests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
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
					             does not match "*ME ME*",
					             but it was "some message"
					             """);
			}

			[Fact]
			public async Task WhenPatternMatchesOnlyOneLineOfTheSubject_ShouldSucceed()
			{
				string subject = "xyz\nabc";

				async Task Act()
					=> await That(subject).IsNotEqualTo("abc").AsWildcard();

				await That(Act).DoesNotThrow()
					.Because("the pattern has to cover the complete subject, not one of its lines");
			}
		}
	}
}
