namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLineCount
	{
		public sealed class EqualToTests
		{
			[Theory]
			[InlineData("", 0)]
			[InlineData("a", 1)]
			[InlineData("a\nb", 2)]
			[InlineData("a\nb\n", 2)]
			[InlineData("a\nb\n\n", 3)]
			[InlineData("\n", 1)]
			[InlineData("one\r\ntwo\nthree\rfour", 4)]
			public async Task WhenLineCountMatches_ShouldSucceed(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(lineCount);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedLineCountIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected line count must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[InlineData("a\nb", 3)]
			[InlineData("a\nb\n", 3)]
			public async Task WhenLineCountDiffers_ShouldFail(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLineCount().EqualTo(lineCount);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has line count equal to {lineCount},
					              but it had line count 2
					              """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenLineCountIsGreaterOrEqual_ShouldSucceed()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThanOrEqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLineCountIsSmaller_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThanOrEqualTo(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count greater than or equal to 3,
					             but it had line count 2
					             """);
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenLineCountIsGreater_ShouldSucceed()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThan(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLineCountIsNotGreater_ShouldFail()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().GreaterThan(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count greater than 2,
					             but it had line count 2
					             """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenLineCountIsGreater_ShouldFail()
			{
				string subject = "a\nb\nc";

				async Task Act()
					=> await That(subject).HasLineCount().LessThanOrEqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count less than or equal to 2,
					             but it had line count 3
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenLineCountIsNotSmaller_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLineCount().LessThan(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count less than 2,
					             but it had line count 2
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenLineCountDiffers_ShouldSucceed()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().NotEqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLineCountMatches_ShouldFail()
			{
				string subject = "a\nb\n";

				async Task Act()
					=> await That(subject).HasLineCount().NotEqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has line count not equal to 2,
					             but it had line count 2
					             """);
			}
		}
	}
}
