namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotNull
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotNull();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenActualIsNotNull_ShouldSucceed(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNull();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_AndChainedWithAnd_ShouldReportNullOnce()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull().And.StartsWith("a");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null and starts with "a",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_AndChainedWithOr_ShouldReportNullOnce()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull().Or.IsEqualTo("a");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null or is equal to "a",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null,
					             but it was <null>
					             """);
			}
		}
	}
}
