using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class HasPosition
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to -1,
					             but it had position 3
					             """)
					.Because("a negative position is a comparison no stream can satisfy, not an invalid argument");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentPosition_ShouldFail(long position)
			{
				long actualPosition = position > 10000 ? position - 1 : position + 1;
				Stream subject = new MyStream(position: actualPosition);

				async Task Act()
					=> await That(subject).HasPosition(position);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position equal to {position},
					              but it had position {actualPosition}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSamePosition_ShouldSucceed(long position)
			{
				Stream subject = new MyStream(position: position);

				async Task Act()
					=> await That(subject).HasPosition(position);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasPosition(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to 0,
					             but it could not read the position, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("the disposed stream is the production bug the test should report");
			}

			[Fact]
			public async Task WhenSubjectIsNonSeekable_ShouldFail()
			{
				Stream subject = new UnreadableStream(new NotSupportedException("Stream does not support seeking."));

				async Task Act()
					=> await That(subject).HasPosition(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to 0,
					             but it could not read the position, because it did throw a NotSupportedException:
					               Stream does not support seeking.
					             """)
					.Because("a non-seekable stream does not have a position of 0");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasPosition(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class BetweenTests
		{
			[Fact]
			public async Task WhenMaximumIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().Between(-3).And(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position between -3 and -1,
					             but it had position 3
					             """)
					.Because("an empty range fails the comparison instead of rejecting the arguments");
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsInsideTheRange_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().Between(-1).And(5);

				await That(Act).DoesNotThrow()
					.Because("a negative minimum only widens the range below the smallest possible position");
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsOutsideTheRange_ShouldFail()
			{
				Stream subject = new MyStream(position: 7);

				async Task Act()
					=> await That(subject).HasPosition().Between(-1).And(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position between -1 and 5,
					             but it had position 7
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().EqualTo(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to -1,
					             but it had position 3
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentPosition_ShouldFail(long position)
			{
				long actualPosition = position > 10000 ? position - 1 : position + 1;
				Stream subject = new MyStream(position: actualPosition);

				async Task Act()
					=> await That(subject).HasPosition().EqualTo(position);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position equal to {position},
					              but it had position {actualPosition}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSamePosition_ShouldSucceed(long position)
			{
				Stream subject = new MyStream(position: position);

				async Task Act()
					=> await That(subject).HasPosition().EqualTo(position);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasPosition().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position equal to 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position greater than or equal to <null>,
					             but it had position 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(-1);

				await That(Act).DoesNotThrow()
					.Because("every stream position is greater than or equal to -1");
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsLessThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position greater than or equal to {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position greater than <null>,
					             but it had position 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(-1);

				await That(Act).DoesNotThrow()
					.Because("greater than -1 is how a caller asks for any position at all");
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsLessThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position greater than {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position greater than {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position greater than 2,
					             but it could not read the position, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("the chained comparison must report the unreadable position, too");
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position less than or equal to <null>,
					             but it had position 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position less than or equal to -1,
					             but it had position 3
					             """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position less than or equal to {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasPosition().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position less than <null>,
					             but it had position 2010
					             """);
			}

			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldFail()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().LessThan(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position less than -1,
					             but it had position 3
					             """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasPosition().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position less than {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 2010);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasPosition().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPositionOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Stream subject = new MyStream(position: 2010);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasPosition().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position less than {Formatter.Format(expected)},
					              but it had position 2010
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Theory]
			[AutoData]
			public async Task WhenSubjectHasDifferentPosition_ShouldSucceed(long position)
			{
				long actualPosition = position > 10000 ? position - 1 : position + 1;
				Stream subject = new MyStream(position: actualPosition);

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(position);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasSamePosition_ShouldFail(long position)
			{
				Stream subject = new MyStream(position: position);

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(position);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has position not equal to {position},
					              but it had position {position}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsDisposed_ShouldFail()
			{
				Stream subject = new MemoryStream(new byte[3]);
				subject.Dispose();

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position not equal to 0,
					             but it could not read the position, because it did throw an ObjectDisposedException:
					               *
					             """).AsWildcard()
					.Because("an unreadable position is no proof that the position differs");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Stream? subject = null;

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has position not equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedPositionIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(-1);

				await That(Act).DoesNotThrow()
					.Because("no stream position can be -1, so the expectation trivially holds");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldSucceed()
			{
				Stream subject = new MyStream(position: 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPosition(-1));

				await That(Act).DoesNotThrow()
					.Because("the negation of an unreachable position holds instead of throwing");
			}
		}
	}
}
