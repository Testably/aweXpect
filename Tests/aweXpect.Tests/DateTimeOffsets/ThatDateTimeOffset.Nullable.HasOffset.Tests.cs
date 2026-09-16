namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class HasOffset
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenOffsetOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 1.Hours();

					async Task Act()
						=> await That(subject).HasOffset(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset equal to {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset equal to 2:00:00,
						             but it was <null>
						             """);
				}
			}

			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenOffsetOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 1.Hours();

					async Task Act()
						=> await That(subject).HasOffset().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset equal to {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset equal to 2:00:00,
						             but it was <null>
						             """);
				}
			}

			public sealed class GreaterThanOrEqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = null;

					async Task Act()
						=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset greater than or equal to <null>,
						             but it had offset 2:00:00
						             """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 1.Hours(59.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 2.Hours(1.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset greater than or equal to {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GreaterThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = null;

					async Task Act()
						=> await That(subject).HasOffset().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset greater than <null>,
						             but it had offset 2:00:00
						             """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 1.Hours(59.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 2.Hours(1.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset greater than {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset greater than {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}
			}

			public sealed class LessThanOrEqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = null;

					async Task Act()
						=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset less than or equal to <null>,
						             but it had offset 2:00:00
						             """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 1.Hours(59.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset less than or equal to {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 2.Hours(1.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LessThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = null;

					async Task Act()
						=> await That(subject).HasOffset().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has offset less than <null>,
						             but it had offset 2:00:00
						             """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 1.Hours(59.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset less than {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan? expected = 2.Hours(1.Minutes());

					async Task Act()
						=> await That(subject).HasOffset().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan expected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset less than {Formatter.Format(expected)},
						              but it had offset 2:00:00
						              """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenOffsetOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan unexpected = 1.Hours();

					async Task Act()
						=> await That(subject).HasOffset().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOffsetOfSubjectIsTheSame_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					TimeSpan unexpected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset not equal to {Formatter.Format(unexpected)},
						              but it had offset 2:00:00
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					TimeSpan unexpected = 2.Hours();

					async Task Act()
						=> await That(subject).HasOffset().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has offset not equal to {Formatter.Format(unexpected)},
						              but it was <null>
						              """);
				}
			}
		}
	}
}
