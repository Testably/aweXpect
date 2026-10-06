namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class HasOffset
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

				async Task Act()
					=> await That(subject).HasOffset(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has offset equal to <null>,
					             but it had offset 2:00:00
					             """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsDifferent_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 1.Hours();

				async Task Act()
					=> await That(subject).HasOffset(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset equal to {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenOffsetOfSubjectIsDifferent_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 1.Hours();

				async Task Act()
					=> await That(subject).HasOffset().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset equal to {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSame_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has offset greater than or equal to <null>,
					             but it had offset 2:00:00
					             """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 1.Hours(59.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 2.Hours(1.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset greater than or equal to {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).HasOffset().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has offset greater than <null>,
					             but it had offset 2:00:00
					             """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 1.Hours(59.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 2.Hours(1.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset greater than {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset greater than {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has offset less than or equal to <null>,
					             but it had offset 2:00:00
					             """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 1.Hours(59.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset less than or equal to {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 2.Hours(1.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).HasOffset().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has offset less than <null>,
					             but it had offset 2:00:00
					             """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 1.Hours(59.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset less than {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan? expected = 2.Hours(1.Minutes());

				async Task Act()
					=> await That(subject).HasOffset().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan expected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has offset less than {Formatter.Format(expected)},
					              but it had offset 2:00:00
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenOffsetOfSubjectIsDifferent_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan unexpected = 1.Hours();

				async Task Act()
					=> await That(subject).HasOffset().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetOfSubjectIsTheSame_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
				TimeSpan unexpected = 2.Hours();

				async Task Act()
					=> await That(subject).HasOffset().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have offset equal to {Formatter.Format(unexpected)},
					              but it had offset 2:00:00
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasOffset(null));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetDiffers_ShouldSucceed()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasOffset(1.Hours()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOffsetMatches_ShouldFail()
			{
				DateTimeOffset subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasOffset(2.Hours()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have offset equal to 2:00:00,
					             but it had offset 2:00:00
					             """);
			}
		}
	}
}
