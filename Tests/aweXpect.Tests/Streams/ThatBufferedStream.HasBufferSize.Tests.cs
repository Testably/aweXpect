#if NET8_0_OR_GREATER
using System.IO;

// ReSharper disable AccessToDisposedClosure

namespace aweXpect.Tests;

public sealed partial class ThatBufferedStream
{
	public sealed class HasBufferSize
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldFail(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize(bufferSize);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size equal to {bufferSize},
					              but it had buffer size {actualBufferSize}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasSameBufferSize_ShouldSucceed(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldFail(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(bufferSize);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size equal to {bufferSize},
					              but it had buffer size {actualBufferSize}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasSameBufferSize_ShouldSucceed(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than or equal to {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size greater than or equal to <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size greater than <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than or equal to {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size less than or equal to <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Test]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size less than <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldSucceed(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasSameBufferSize_ShouldFail(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(bufferSize);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have buffer size equal to {bufferSize},
					              but it had buffer size {bufferSize}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have buffer size equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected buffer size must not be negative.*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenBufferSizeDiffers_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(8);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasBufferSize(9));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBufferSizeMatches_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(8);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasBufferSize(8));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have buffer size equal to 8,
					             but it had buffer size 8
					             """);
			}
		}
	}
}
#endif
