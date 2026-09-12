namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class HasMonth
		{
			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month equal to <null>,
						             but it had month 11
						             """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasMonth().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month equal to {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasMonth().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasMonth().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month equal to 1,
						             but it was <null>
						             """);
				}
			}

			public sealed class GreaterThanOrEqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month greater than or equal to <null>,
						             but it had month 11
						             """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 10;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month greater than or equal to {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GreaterThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month greater than <null>,
						             but it had month 11
						             """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 10;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month greater than {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasMonth().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month greater than {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}
			}

			public sealed class LessThanOrEqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month less than or equal to <null>,
						             but it had month 11
						             """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 10;

					async Task Act()
						=> await That(subject).HasMonth().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month less than or equal to {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasMonth().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasMonth().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LessThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month less than <null>,
						             but it had month 11
						             """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 10;

					async Task Act()
						=> await That(subject).HasMonth().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month less than {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasMonth().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 11;

					async Task Act()
						=> await That(subject).HasMonth().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month less than {Formatter.Format(expected)},
						              but it had month 11
						              """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenMonthOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? unexpected = 12;

					async Task Act()
						=> await That(subject).HasMonth().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMonthOfSubjectIsTheSame_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int unexpected = 11;

					async Task Act()
						=> await That(subject).HasMonth().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month not equal to {Formatter.Format(unexpected)},
						              but it had month 11
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMonth().NotEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has month not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? unexpected = 1;

					async Task Act()
						=> await That(subject).HasMonth().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has month not equal to {unexpected},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasMonth().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
