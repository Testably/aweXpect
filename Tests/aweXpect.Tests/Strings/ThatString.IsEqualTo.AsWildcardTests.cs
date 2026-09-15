namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsWildcardTests
		{
			[Theory]
			[InlineData("some message", "*me me*", true)]
			[InlineData("some message", "*ME ME*", false)]
			[InlineData("some message", "some?message", true)]
			[InlineData("some message", "some*message", true)]
			[InlineData("some message", "some me?age", false)]
			[InlineData("some message", "some me??age", true)]
			public async Task ShouldDefaultToCaseSensitiveMatch(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)

					              Actual:
					              some message
					              
					              Expected:
					              {pattern}
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = "*ME ME*";

				async Task Act()
					=> await That(subject).IsEqualTo(pattern)
						.AsWildcard().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             matches "*ME ME*",
					             but it did not match:
					               ↓ (actual)
					               "some message"
					               "*ME ME*"
					               ↑ (wildcard pattern)

					             Actual:
					             some message
					             
					             Expected:
					             *ME ME*
					             """);
			}
		}
	}
}
