#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class HasDay
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenDayOfSubjectIsDifferent_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasDay(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSame_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					int expected = 1;

					async Task Act()
						=> await That(subject).HasDay(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to 1,
						             but it was <null>
						             """);
				}
			}

			public sealed class EqualToTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsDifferent_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSame_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to <null>,
						             but it had day 12
						             """);
				}

				[Test]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to 1,
						             but it was <null>
						             """);
				}
			}

			public sealed class GreaterThanOrEqualToTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than or equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day greater than or equal to <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class GreaterThanTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day greater than <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class LessThanOrEqualToTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than or equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day less than or equal to <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class LessThanTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day less than <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Test]
				public async Task WhenDayOfSubjectIsDifferent_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? unexpected = 11;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfSubjectIsTheSame_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);
					int unexpected = 12;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have day equal to {Formatter.Format(unexpected)},
						              but it had day 12
						              """);
				}

				[Test]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have day equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Test]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenDayDiffers_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasDay(13));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayMatches_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasDay(12));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day equal to 12,
						             but it had day 12
						             """);
				}
			}
		}
	}
}
#endif
