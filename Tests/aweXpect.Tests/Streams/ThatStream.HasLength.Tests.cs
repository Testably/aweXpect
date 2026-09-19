using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class HasLength
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to -1,
					             but it had length 3
					             """)
					.Because("a negative length is a comparison no stream can satisfy, not an invalid argument");
			}

			[Fact]
			public async Task WhenReadingTheLengthThrowsAnIOException_ShouldFail()
			{
				Stream subject = new UnreadableStream(new IOException("The device is not ready."));

				async Task Act()
					=> await That(subject).HasLength(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 3,
					             but it could not read the length, because it did throw an IOException:
					               The device is not ready.
					             """)
					.Because("a broken stream cannot answer what its length is");
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
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasLength(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 3,
					             but it could not read the length, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("the disposed stream is the production bug the test should report");
			}

			[Fact]
			public async Task WhenSubjectIsNonSeekable_ShouldFail()
			{
				Stream subject = new UnreadableStream(new NotSupportedException("Stream does not support seeking."));

				async Task Act()
					=> await That(subject).HasLength(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 3,
					             but it could not read the length, because it did throw a NotSupportedException:
					               Stream does not support seeking.
					             """)
					.Because("a non-seekable stream does not have a length of 3");
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

		public sealed class BetweenTests
		{
			[Fact]
			public async Task WhenLengthOfSubjectIsInsideTheRange_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().Between(-1).And(5);

				await That(Act).DoesNotThrow()
					.Because("a negative minimum only widens the range below the smallest possible length");
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsOutsideTheRange_ShouldFail()
			{
				Stream subject = new MyStream(length: 7);

				async Task Act()
					=> await That(subject).HasLength().Between(-1).And(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length between -1 and 5,
					             but it had length 7
					             """);
			}

			[Fact]
			public async Task WhenMaximumIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().Between(-3).And(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length between -3 and -1,
					             but it had length 3
					             """)
					.Because("an empty range fails the comparison instead of rejecting the arguments");
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().EqualTo(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to -1,
					             but it had length 3
					             """);
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
			public async Task WhenExpectedLengthIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(-1);

				await That(Act).DoesNotThrow()
					.Because("every stream length is greater than or equal to -1");
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
			public async Task WhenExpectedLengthIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(-1);

				await That(Act).DoesNotThrow()
					.Because("greater than -1 is how a caller asks for any length at all");
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

			[Fact]
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than 2,
					             but it could not read the length, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("the chained comparison must report the unreadable length, too");
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
			public async Task WhenExpectedLengthIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than or equal to -1,
					             but it had length 3
					             """);
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
			public async Task WhenExpectedLengthIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().LessThan(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than -1,
					             but it had length 3
					             """);
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
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length not equal to 3,
					             but it could not read the length, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("an unreadable length is no proof that the length differs");
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
			public async Task WhenUnexpectedLengthIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(-1);

				await That(Act).DoesNotThrow()
					.Because("no stream length can be -1, so the expectation trivially holds");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(-1));

				await That(Act).DoesNotThrow()
					.Because("the negation of an unreachable length holds instead of throwing");
			}

			[Fact]
			public async Task WhenLengthDiffers_ShouldSucceed()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(4));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthMatches_ShouldFail()
			{
				Stream subject = new MyStream(length: 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have length equal to 3,
					             but it had length 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have length equal to 3,
					             but it could not read the length, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("negating a question that cannot be answered does not make it true");
			}
		}
	}
}
