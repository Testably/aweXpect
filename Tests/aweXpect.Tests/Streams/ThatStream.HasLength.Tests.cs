using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class HasLength
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentLength_ShouldFail(long length)
			{
				long actualLength = length > 10000 ? length - 1 : length + 1;
				Stream subject = new MyStream(length: actualLength);

				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {actualLength}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameLength_ShouldSucceed(long length)
			{
				Stream subject = new MyStream(length: length);

				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasLength(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentLength_ShouldFail(long length)
			{
				long actualLength = length > 10000 ? length - 1 : length + 1;
				Stream subject = new MyStream(length: actualLength);

				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {actualLength}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameLength_ShouldSucceed(long length)
			{
				Stream subject = new MyStream(length: length);

				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasLength().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than or equal to <null>,
					             but it had length 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than or equal to {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than <null>,
					             but it had length 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than or equal to <null>,
					             but it had length 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than or equal to {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than <null>,
					             but it had length 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().LessThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Stream subject = new MyStream(length: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 2010
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentLength_ShouldSucceed(long length)
			{
				long actualLength = length > 10000 ? length - 1 : length + 1;
				Stream subject = new MyStream(length: actualLength);

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSameLength_ShouldFail(long length)
			{
				Stream subject = new MyStream(length: length);

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length not equal to {length},
					              but it had length {length}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length not equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
