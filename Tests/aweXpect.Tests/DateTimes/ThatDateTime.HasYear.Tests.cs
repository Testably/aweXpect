namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class HasYear
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);

				async Task Act()
					=> await That(subject).HasYear(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year equal to <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsDifferent_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2011;

				async Task Act()
					=> await That(subject).HasYear(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year equal to {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear(expected);

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
					=> await That(subject).HasYear().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year equal to <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsDifferent_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasYear().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year equal to {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear().EqualTo(expected);

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
					=> await That(subject).HasYear().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year greater than or equal to <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasYear().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasYear().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year greater than or equal to {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear().GreaterThanOrEqualTo(expected);

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
					=> await That(subject).HasYear().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year greater than <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasYear().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasYear().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year greater than {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year greater than {Formatter.Format(expected)},
					              but it had year 2010
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
					=> await That(subject).HasYear().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year less than or equal to <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasYear().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year less than or equal to {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasYear().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear().LessThanOrEqualTo(expected);

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
					=> await That(subject).HasYear().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has year less than <null>,
					             but it had year 2010
					             """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasYear().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year less than {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}

			[Test]
			public async Task WhenYearOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasYear().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasYear().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has year less than {Formatter.Format(expected)},
					              but it had year 2010
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasYear().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsDifferent_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int? unexpected = 2011;

				async Task Act()
					=> await That(subject).HasYear().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearOfSubjectIsTheSame_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12, 13, 14, 15, 167);
				int unexpected = 2010;

				async Task Act()
					=> await That(subject).HasYear().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have year equal to {Formatter.Format(unexpected)},
					              but it had year 2010
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasYear(null));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearDiffers_ShouldSucceed()
			{
				DateTime subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasYear(2011));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenYearMatches_ShouldFail()
			{
				DateTime subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasYear(2010));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have year equal to 2010,
					             but it had year 2010
					             """);
			}
		}
	}
}
