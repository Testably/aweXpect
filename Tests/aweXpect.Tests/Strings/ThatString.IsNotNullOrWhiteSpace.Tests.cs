namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotNullOrWhiteSpace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was ""
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenActualIsNotEmpty_ShouldSucceed(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldFail()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was " \t "
					             """);
			}

			[Test]
			public async Task WhenActualIsWhiteSpace_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = new(' ', 101);

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not null or whitespace,
					              but it was "{new string(' ', 100)}…"
					              """);
			}
		}
	}
}
