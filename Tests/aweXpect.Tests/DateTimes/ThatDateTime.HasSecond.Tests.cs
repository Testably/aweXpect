namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class HasSecond
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 14;

				async Task Act()
					=> await That(subject).HasSecond(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has second equal to <null>,
					             but it had second 15
					             """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has second greater than or equal to <null>,
					             but it had second 15
					             """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than or equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has second greater than <null>,
					             but it had second 15
					             """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has second less than or equal to <null>,
					             but it had second 15
					             """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than or equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has second less than <null>,
					             but it had second 15
					             """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? unexpected = 14;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int unexpected = 15;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have second equal to {Formatter.Format(unexpected)},
					              but it had second 15
					              """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
