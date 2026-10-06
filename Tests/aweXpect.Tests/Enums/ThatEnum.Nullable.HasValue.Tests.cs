namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class HasValue
		{
			public sealed class ContinuationTests
			{
				[Test]
				public async Task GreaterThan_WhenTheValueExceedsInt64MaxValue_ShouldSucceed()
				{
					EnumULong? subject = EnumULong.UInt64Max;

					async Task Act()
						=> await That(subject).HasValue().GreaterThan(long.MaxValue);

					await That(Act).DoesNotThrow()
						.Because(
							"a ulong-backed member above long.MaxValue is a legal enum value and must not overflow");
				}

				[Test]
				public async Task NotBetween_WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotBetween(1L).And(3L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have value between 1 and 3,
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments(MyNumbers.One, 2L)]
				[Arguments(MyNumbers.Two, -7L)]
				[Arguments(MyNumbers.Three, 0L)]
				public async Task NotEqualTo_WhenSubjectDoesNotHaveUnexpectedValue_ShouldSucceed(MyNumbers? subject,
					long? unexpected)
				{
					async Task Act()
						=> await That(subject).HasValue().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(MyNumbers.One, 1L)]
				[Arguments(MyNumbers.Two, 2L)]
				[Arguments(MyNumbers.Three, 3L)]
				public async Task NotEqualTo_WhenSubjectHasUnexpectedValue_ShouldFail(MyNumbers? subject,
					long? unexpected)
				{
					async Task Act()
						=> await That(subject).HasValue().NotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have value equal to {Formatter.Format(unexpected)},
						              but it had value {Formatter.Format((long?)subject)}
						              """);
				}

				[Test]
				[Arguments(null)]
				[Arguments(0L)]
				[Arguments(2L)]
				public async Task NotEqualTo_WhenSubjectIsNull_ShouldFail(long? unexpected)
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have value equal to {Formatter.Format(unexpected)},
						              but it was <null>
						              """);
				}

				[Test]
				public async Task NotEqualTo_WhenUnexpectedIsNull_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).HasValue().NotEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task NotGreaterThan_WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotGreaterThan(2L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have value greater than 2,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task NotGreaterThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotGreaterThanOrEqualTo(2L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have value greater than or equal to 2,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task NotLessThan_WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotLessThan(2L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have value less than 2,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task NotLessThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().NotLessThanOrEqualTo(2L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have value less than or equal to 2,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldRenderLikeTheShorthand()
				{
					MyNumbers? subject = MyNumbers.One;

					async Task Act()
						=> await That(subject).HasValue().EqualTo(2L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has value equal to 2,
						             but it had value 1
						             """)
						.Because("the continuation renders exactly like the HasValue(expected) shorthand");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().GreaterThan(0L);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has value greater than 0,
						             but it was <null>
						             """);
				}
			}

			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).HasValue(null);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to <null>,
						              but it had value {Formatter.Format((long?)subject)}
						              """);
				}

				[Test]
				[Arguments(MyNumbers.One, 2L)]
				[Arguments(MyNumbers.Two, -7L)]
				[Arguments(MyNumbers.Three, 0L)]
				public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldFail(MyNumbers? subject,
					long? expected)
				{
					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to {Formatter.Format(expected)},
						              but it had value {Formatter.Format((long?)subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectExceedsInt64MaxValue_ShouldSucceed()
				{
					EnumULong? subject = EnumULong.UInt64Max;

					async Task Act()
						=> await That(subject).HasValue(ulong.MaxValue);

					await That(Act).DoesNotThrow()
						.Because("the nullable overload reads the underlying value through the same conversion");
				}

				[Test]
				[Arguments(MyNumbers.One, 1L)]
				[Arguments(MyNumbers.Two, 2L)]
				[Arguments(MyNumbers.Three, 3L)]
				public async Task WhenSubjectHasExpectedValue_ShouldSucceed(MyNumbers? subject,
					long? expected)
				{
					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(null)]
				[Arguments(0UL)]
				[Arguments(ulong.MaxValue)]
				public async Task WhenSubjectIsNull_AndExpectedIsUnsigned_ShouldFail(ulong? expected)
				{
					EnumULong? subject = null;

					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Test]
				[Arguments(null)]
				[Arguments(0L)]
				[Arguments(1L)]
				public async Task WhenSubjectIsNull_ShouldFail(long? expected)
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}
			}
		}
	}
}
