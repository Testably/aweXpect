namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLineCount
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLineCount(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedLineCountIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLineCount(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected line count must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[Arguments("a\nb", 3)]
			[Arguments("a\nb\n", 3)]
			public async Task WhenLineCountDiffers_ShouldFail(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount(lineCount);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has line count equal to {lineCount},
					              but it had line count 2
					              """);
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("a", 1)]
			[Arguments("a\nb", 2)]
			[Arguments("a\nb\n", 2)]
			[Arguments("a\nb\n\n", 3)]
			[Arguments("\n", 1)]
			[Arguments("a\r\n", 1)]
			[Arguments("a\r\nb\r\n", 2)]
			[Arguments("\r\n", 1)]
			[Arguments("\r", 1)]
			[Arguments("one\r\ntwo\nthree\rfour", 4)]
			public async Task WhenLineCountMatches_ShouldSucceed(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount(lineCount);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedLineCountIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected line count must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[Arguments("a\nb", 3)]
			[Arguments("a\nb\n", 3)]
			public async Task WhenLineCountDiffers_ShouldFail(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(lineCount);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has line count equal to {lineCount},
					              but it had line count 2
					              """);
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("a", 1)]
			[Arguments("a\nb", 2)]
			[Arguments("a\nb\n", 2)]
			[Arguments("a\nb\n\n", 3)]
			[Arguments("\n", 1)]
			[Arguments("a\r\n", 1)]
			[Arguments("a\r\nb\r\n", 2)]
			[Arguments("\r\n", 1)]
			[Arguments("\r", 1)]
			[Arguments("one\r\ntwo\nthree\rfour", 4)]
			public async Task WhenLineCountMatches_ShouldSucceed(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(lineCount);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenLineCountIsGreaterOrEqual_ShouldSucceed()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThanOrEqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLineCountIsSmaller_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThanOrEqualTo(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count greater than or equal to 3,
					             but it had line count 2
					             """);
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenLineCountIsGreater_ShouldSucceed()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThan(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLineCountIsNotGreater_ShouldFail()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count greater than 2,
					             but it had line count 2
					             """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenLineCountIsGreater_ShouldFail()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().LessThanOrEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count less than or equal to 2,
					             but it had line count 3
					             """);
			}

			[Test]
			public async Task WhenLineCountIsSmallerOrEqual_ShouldSucceed()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLineCount().LessThanOrEqualTo(2);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenLineCountIsNotSmaller_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLineCount().LessThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has line count less than 2,
					             but it had line count 2
					             """);
			}

			[Test]
			public async Task WhenLineCountIsSmaller_ShouldSucceed()
			{
				string subject = "a";

				async Task Act()
					=> await That(subject).HasLineCount().LessThan(2);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenLineCountDiffers_ShouldSucceed()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().NotEqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLineCountMatches_ShouldFail()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().NotEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have line count equal to 2,
					             but it had line count 2
					             """);
			}
		}
	}
}
