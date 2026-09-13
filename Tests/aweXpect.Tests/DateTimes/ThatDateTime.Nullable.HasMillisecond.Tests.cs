namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class HasMillisecond
		{
			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMillisecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond equal to <null>,
						             but it had millisecond 167
						             """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 15;

					async Task Act()
						=> await That(subject).HasMillisecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond equal to {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMillisecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasMillisecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond equal to 1,
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
						=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond greater than or equal to <null>,
						             but it had millisecond 167
						             """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 166;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 168;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond greater than or equal to {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

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
						=> await That(subject).HasMillisecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond greater than <null>,
						             but it had millisecond 167
						             """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 166;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 168;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond greater than {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond greater than {Formatter.Format(expected)},
						              but it had millisecond 167
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
						=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond less than or equal to <null>,
						             but it had millisecond 167
						             """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 166;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond less than or equal to {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 168;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

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
						=> await That(subject).HasMillisecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond less than <null>,
						             but it had millisecond 167
						             """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 166;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond less than {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? expected = 168;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int expected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond less than {Formatter.Format(expected)},
						              but it had millisecond 167
						              """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenMillisecondOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? unexpected = 15;

					async Task Act()
						=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMillisecondOfSubjectIsTheSame_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int unexpected = 167;

					async Task Act()
						=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond not equal to {Formatter.Format(unexpected)},
						              but it had millisecond 167
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasMillisecond().NotEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has millisecond not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					int? unexpected = 1;

					async Task Act()
						=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has millisecond not equal to {unexpected},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167);
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
