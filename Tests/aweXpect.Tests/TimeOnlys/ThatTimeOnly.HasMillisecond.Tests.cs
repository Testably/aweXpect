#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class HasMillisecond
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMillisecondOfSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 42;

				async Task Act()
					=> await That(subject).HasMillisecond(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond equal to {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSame_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has millisecond equal to <null>,
					             but it had millisecond 167
					             """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 42;

				async Task Act()
					=> await That(subject).HasMillisecond().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond equal to {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSame_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has millisecond greater than or equal to <null>,
					             but it had millisecond 167
					             """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 166;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 168;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond greater than or equal to {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has millisecond greater than <null>,
					             but it had millisecond 167
					             """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 166;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 168;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond greater than {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond greater than {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has millisecond less than or equal to <null>,
					             but it had millisecond 167
					             """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 166;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond less than or equal to {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 168;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has millisecond less than <null>,
					             but it had millisecond 167
					             """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 166;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond less than {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? expected = 168;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int expected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has millisecond less than {Formatter.Format(expected)},
					              but it had millisecond 167
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenMillisecondOfSubjectIsDifferent_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? unexpected = 42;

				async Task Act()
					=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMillisecondOfSubjectIsTheSame_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int unexpected = 167;

				async Task Act()
					=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have millisecond equal to {Formatter.Format(unexpected)},
					              but it had millisecond 167
					              """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15, 167);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasMillisecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
