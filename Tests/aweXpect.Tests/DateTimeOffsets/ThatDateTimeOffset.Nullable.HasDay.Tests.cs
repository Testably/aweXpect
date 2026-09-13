namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class HasDay
		{
			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to <null>,
						             but it had day 12
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasDay().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day equal to 1,
						             but it was <null>
						             """);
				}
			}

			public sealed class GreaterThanOrEqualToTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than or equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day greater than or equal to <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class GreaterThanTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day greater than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day greater than <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class LessThanOrEqualToTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than or equal to {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day less than or equal to <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class LessThanTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 11;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenDayOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 13;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 12;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day less than {Formatter.Format(expected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day less than <null>,
						             but it had day 12
						             """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenDayOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? unexpected = 11;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDayOfSubjectIsTheSame_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int unexpected = 12;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day not equal to {Formatter.Format(unexpected)},
						              but it had day 12
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has day not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? unexpected = 1;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has day not equal to {unexpected},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasDay().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
