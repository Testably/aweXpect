namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class HasSecond
		{
			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second equal to <null>,
						             but it had second 15
						             """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasSecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second equal to {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 15;

					async Task Act()
						=> await That(subject).HasSecond().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasSecond().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second equal to 1,
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
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second greater than or equal to <null>,
						             but it had second 15
						             """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 16;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second greater than or equal to {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 15;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GreaterThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second greater than <null>,
						             but it had second 15
						             """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 16;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second greater than {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 15;

					async Task Act()
						=> await That(subject).HasSecond().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second greater than {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}
			}

			public sealed class LessThanOrEqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second less than or equal to <null>,
						             but it had second 15
						             """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second less than or equal to {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 16;

					async Task Act()
						=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 15;

					async Task Act()
						=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LessThanTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second less than <null>,
						             but it had second 15
						             """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasSecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second less than {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? expected = 16;

					async Task Act()
						=> await That(subject).HasSecond().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int expected = 15;

					async Task Act()
						=> await That(subject).HasSecond().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second less than {Formatter.Format(expected)},
						              but it had second 15
						              """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenSecondOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? unexpected = 14;

					async Task Act()
						=> await That(subject).HasSecond().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSecondOfSubjectIsTheSame_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int unexpected = 15;

					async Task Act()
						=> await That(subject).HasSecond().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second not equal to {Formatter.Format(unexpected)},
						              but it had second 15
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasSecond().NotEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has second not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					int? unexpected = 1;

					async Task Act()
						=> await That(subject).HasSecond().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has second not equal to {unexpected},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasSecond().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
