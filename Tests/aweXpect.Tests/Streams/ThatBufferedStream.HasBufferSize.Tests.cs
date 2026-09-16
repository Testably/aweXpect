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
			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldFail(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize(bufferSize);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size equal to {bufferSize},
					              but it had buffer size {actualBufferSize}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameBufferSize_ShouldSucceed(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldFail(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(bufferSize);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size equal to {bufferSize},
					              but it had buffer size {actualBufferSize}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameBufferSize_ShouldSucceed(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than or equal to {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size greater than or equal to <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size greater than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size greater than <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than or equal to {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size less than or equal to <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenBufferSizeOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBufferSizeOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size less than {Formatter.Format(expected)},
					              but it had buffer size 2010
					              """);
			}

			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				using BufferedStream subject = GetBufferedStream(2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBufferSize().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size less than <null>,
					             but it had buffer size 2010
					             """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenExpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentBufferSize_ShouldSucceed(int bufferSize)
			{
				int actualBufferSize = bufferSize > 10000 ? bufferSize - 1 : bufferSize + 1;
				using BufferedStream subject = GetBufferedStream(actualBufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(bufferSize);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameBufferSize_ShouldFail(int bufferSize)
			{
				using BufferedStream subject = GetBufferedStream(bufferSize);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(bufferSize);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has buffer size not equal to {bufferSize},
					              but it had buffer size {bufferSize}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				using BufferedStream? subject = null;

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has buffer size not equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedBufferSizeIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				using BufferedStream subject = GetBufferedStream(1);

				async Task Act()
					=> await That(subject).HasBufferSize().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected buffer size must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
#endif
