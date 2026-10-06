namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsNullOrWhiteSpace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNullOrWhiteSpace();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenActualIsNotEmpty_ShouldFail(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is null or whitespace,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenActualIsNotEmpty_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters;

				async Task Act()
					=> await That(subject).IsNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is null or whitespace,
					              but it was "{StringWith100Characters}…"
					              """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldSucceed()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNullOrWhiteSpace();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNullOrWhiteSpace();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
