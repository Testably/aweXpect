using System.IO;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	public sealed class HasPosition
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().GreaterThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().GreaterThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().LessThanOrEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().LessThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
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
			[Fact]
			public async Task WhenExpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}

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
			public async Task WhenUnexpectedPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Stream subject = new MyStream();

				async Task Act()
					=> await That(subject).HasPosition().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected position must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
