namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotNullOrEmpty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotNullOrEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or empty,
					             but it was ""
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenActualIsNotEmpty_ShouldSucceed(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNullOrEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNullOrEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or empty,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotNullOrEmpty();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
