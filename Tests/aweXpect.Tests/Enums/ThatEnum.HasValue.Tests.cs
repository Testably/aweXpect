namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class HasValue
	{
		public sealed class ContinuationTests
		{
			[Test]
			public async Task Between_WhenMaximumIsBelowMinimum_AndNegated_ShouldThrowArgumentOutOfRangeException()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().Between(5L).And(2L));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
					.Because("an inverted range would let the negated expectation succeed for every value");
			}

			[Test]
			public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().Between(5L).And(2L);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			[Arguments(null, 3L)]
			[Arguments(1L, null)]
			public async Task Between_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(long? minimum, long? maximum)
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().Between(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it had value 2
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			public async Task Between_WhenTheRangeExceedsInt64MaxValue_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().Between((ulong)long.MaxValue).And(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a range above long.MaxValue is a legal range for a ulong-backed enum");
			}

			[Test]
			public async Task Between_WhenTheValueIsInTheRange_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().Between(1L).And(3L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Between_WhenUnsignedMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().Between(ulong.MaxValue).And((ulong)long.MaxValue);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task GreaterThan_WhenExpectedIsNegative_AndTheBackingTypeIsUnsigned_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(-1);

				await That(Act).DoesNotThrow()
					.Because("a negative expected value is below every value a ulong-backed enum can have");
			}

			[Test]
			public async Task GreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().GreaterThan(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task GreaterThan_WhenTheValueExceedsInt64MaxValue_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(0);

				await That(Act).DoesNotThrow()
					.Because("a ulong-backed member above long.MaxValue is a legal enum value and must not overflow");
			}

			[Test]
			public async Task GreaterThan_WhenTheValueIsTheMaximumOfItsBackingType_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(ulong.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than 18446744073709551615,
					             but it had value 18446744073709551615
					             """);
			}

			[Test]
			public async Task GreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().GreaterThanOrEqualTo(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task GreaterThanOrEqualTo_WhenTheValueEqualsTheMaximumOfItsBackingType_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThanOrEqualTo(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("ulong.MaxValue is compared exactly rather than approximated");
			}

			[Test]
			public async Task GreaterThanOrEqualTo_WhenTheValueIsInt64MinValue_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().GreaterThanOrEqualTo(0L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than or equal to 0,
					             but it had value -9223372036854775808
					             """);
			}

			[Test]
			public async Task LessThan_WhenExpectedExceedsInt64MaxValue_AndTheSubjectIsSignedBacked_ShouldSucceed()
			{
				EnumLong subject = EnumLong.Int64Max;

				async Task Act()
					=> await That(subject).HasValue().LessThan(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("an unsigned expected value above long.MaxValue compares against a signed subject as well");
			}

			[Test]
			public async Task LessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().LessThan(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task LessThan_WhenTheValueIsInt64MinValue_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().LessThan(long.MinValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than -9223372036854775808,
					             but it had value -9223372036854775808
					             """);
			}

			[Test]
			public async Task LessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().LessThanOrEqualTo(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task LessThanOrEqualTo_WhenTheValueEqualsInt64MinValue_ShouldSucceed()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().LessThanOrEqualTo(long.MinValue);

				await That(Act).DoesNotThrow()
					.Because("long.MinValue is compared exactly rather than approximated");
			}

			[Test]
			public async Task LessThanOrEqualTo_WhenTheValueExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().LessThanOrEqualTo(ulong.MaxValue - 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than or equal to 18446744073709551614,
					             but it had value 18446744073709551615
					             """);
			}

			[Test]
			public async Task NotBetween_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotBetween(5L).And(2L);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
					.Because("an inverted range would let the negated expectation succeed for every value");
			}

			[Test]
			[Arguments(null, 3L)]
			[Arguments(1L, null)]
			public async Task NotBetween_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(long? minimum, long? maximum)
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotBetween(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it had value 2
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(null, 3L)]
			[Arguments(1L, null)]
			public async Task NotBetween_WhenMinimumOrMaximumIsNull_ShouldFail(long? minimum, long? maximum)
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it had value 2
					              """)
					.Because("nothing can be ordered against a null bound");
			}

			[Test]
			public async Task NotBetween_WhenNegated_ShouldExpectTheRange()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotBetween(3L).And(5L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value between 3 and 5,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotBetween_WhenTheRangeExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().NotBetween((ulong)long.MaxValue).And(ulong.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value between 9223372036854775807 and 18446744073709551615,
					             but it had value 18446744073709551614
					             """);
			}

			[Test]
			public async Task NotBetween_WhenTheValueIsInTheRange_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotBetween(1L).And(3L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value between 1 and 3,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotBetween_WhenTheValueIsOutsideTheRange_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotBetween(3L).And(5L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotBetween_WhenUnsignedMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().NotBetween(ulong.MaxValue).And((ulong)long.MaxValue);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task NotEqualTo_WhenNegated_ShouldExpectEquality()
			{
				MyNumbers subject = MyNumbers.One;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotEqualTo(2L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to 2,
					             but it had value 1
					             """);
			}

			[Test]
			[Arguments(MyNumbers.One, 2L)]
			[Arguments(MyNumbers.Two, -7L)]
			[Arguments(MyNumbers.Three, 0L)]
			public async Task NotEqualTo_WhenSubjectDoesNotHaveUnexpectedValue_ShouldSucceed(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(MyNumbers.One, 1L)]
			[Arguments(MyNumbers.Two, 2L)]
			[Arguments(MyNumbers.Three, 3L)]
			public async Task NotEqualTo_WhenSubjectHasUnexpectedValue_ShouldFail(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value equal to {Formatter.Format(unexpected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Test]
			public async Task NotEqualTo_WhenTheValueExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.Int64MaxPlusOne;

				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(9223372036854775808UL);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value equal to 9223372036854775808,
					             but it had value 9223372036854775808
					             """);
			}

			[Test]
			public async Task NotEqualTo_WhenUnexpectedIsNull_ShouldSucceed()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotGreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotGreaterThan(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task NotGreaterThan_WhenExpectedIsNull_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThan(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null");
			}

			[Test]
			public async Task NotGreaterThan_WhenNegated_ShouldExpectTheComparison()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotGreaterThan(2L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than 2,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotGreaterThan_WhenTheValueExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThan((ulong)long.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than 9223372036854775807,
					             but it had value 18446744073709551615
					             """);
			}

			[Test]
			public async Task NotGreaterThan_WhenTheValueIsGreater_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThan(1L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than 1,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotGreaterThan_WhenTheValueIsNotGreater_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThan(2L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotGreaterThanOrEqualTo(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThanOrEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null");
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotGreaterThanOrEqualTo(3L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than or equal to 3,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenTheValueEqualsTheMaximumOfItsBackingType_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThanOrEqualTo(ulong.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than or equal to 18446744073709551615,
					             but it had value 18446744073709551615
					             """);
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenTheValueIsEqual_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThanOrEqualTo(2L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than or equal to 2,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotGreaterThanOrEqualTo_WhenTheValueIsLess_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotGreaterThanOrEqualTo(3L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotLessThan_WhenExpectedExceedsInt64MaxValue_AndTheSubjectIsSignedBacked_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Max;

				async Task Act()
					=> await That(subject).HasValue().NotLessThan(ulong.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than 18446744073709551615,
					             but it had value 9223372036854775807
					             """);
			}

			[Test]
			public async Task NotLessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotLessThan(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task NotLessThan_WhenExpectedIsNull_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThan(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null");
			}

			[Test]
			public async Task NotLessThan_WhenNegated_ShouldExpectTheComparison()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotLessThan(2L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than 2,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotLessThan_WhenTheValueIsLess_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThan(3L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than 3,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotLessThan_WhenTheValueIsNotLess_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThan(2L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotLessThanOrEqualTo(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThanOrEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null");
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().NotLessThanOrEqualTo(1L));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value less than or equal to 1,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenTheValueEqualsInt64MinValue_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().NotLessThanOrEqualTo(long.MinValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than or equal to -9223372036854775808,
					             but it had value -9223372036854775808
					             """);
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenTheValueIsEqual_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThanOrEqualTo(2L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than or equal to 2,
					             but it had value 2
					             """);
			}

			[Test]
			public async Task NotLessThanOrEqualTo_WhenTheValueIsGreater_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().NotLessThanOrEqualTo(1L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(MyNumbers.One, 2L)]
			[Arguments(MyNumbers.Two, 3L)]
			public async Task ShouldSupportTheComparisonVocabulary(MyNumbers subject, long maximum)
			{
				async Task Act()
					=> await That(subject).HasValue().LessThan(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNested_ShouldNameTheValue()
			{
				Exception subject = new("outer", new NumberException(MyNumbers.One));

				async Task Act()
					=> await That(subject).HasInner<NumberException>(e
						=> e.Whose(x => x.Number, n => n.HasValue().EqualTo(2L)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an inner ThatEnum.HasValue.ContinuationTests.NumberException whose Number whose value is equal to 2,
					             but value was 1
					             """)
					.Because("the whose clause makes the value the subject, as for other properties");
			}

			[Test]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldRenderLikeTheShorthand()
			{
				MyNumbers subject = MyNumbers.One;

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

			private sealed class NumberException(MyNumbers number) : Exception
			{
				public MyNumbers Number { get; } = number;
			}
		}

		public sealed class Tests
		{
			[Test]
			[Arguments(EnumLong.Int64Min, long.MinValue)]
			[Arguments(EnumLong.Int64LessOne, long.MaxValue - 1)]
			[Arguments(EnumLong.Int64Max, long.MaxValue)]
			public async Task WhenExpectedComesFromInlineData_ShouldSucceed(EnumLong subject, long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow()
					.Because("a long is a legal attribute argument, so the expected value can be data-driven");
			}

			[Test]
			public async Task WhenExpectedExceedsInt64MaxValue_AndTheSubjectIsNegative_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue(ulong.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to 18446744073709551615,
					             but it had value -9223372036854775808
					             """)
					.Because("a value no member of a long-backed enum can have fails instead of overflowing");
			}

			[Test]
			public async Task WhenExpectedIsNegative_AndTheBackingTypeIsUnsigned_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue(-1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to -1,
					             but it had value 18446744073709551615
					             """)
					.Because("a value no member of the enum can have fails instead of overflowing the conversion");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).HasValue(null);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to <null>,
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Test]
			[Arguments(MyNumbers.One, 2L)]
			[Arguments(MyNumbers.Two, -7)]
			[Arguments(MyNumbers.Three, 0)]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldFail(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to {Formatter.Format(expected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Test]
			[Arguments(MyNumbers.One, 1)]
			[Arguments(MyNumbers.Two, 2)]
			[Arguments(MyNumbers.Three, 3)]
			public async Task WhenSubjectHasExpectedValue_ShouldSucceed(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasTheExtremeValueOfItsBackingType_ShouldSucceed()
			{
				async Task Act()
				{
					await That(EnumSByte.Min).HasValue(sbyte.MinValue);
					await That(EnumSByte.Max).HasValue(sbyte.MaxValue);
					await That(EnumByte.Min).HasValue(byte.MinValue);
					await That(EnumByte.Max).HasValue(byte.MaxValue);
					await That(EnumShort.Min).HasValue(short.MinValue);
					await That(EnumShort.Max).HasValue(short.MaxValue);
					await That(EnumUShort.Min).HasValue(ushort.MinValue);
					await That(EnumUShort.Max).HasValue(ushort.MaxValue);
					await That(EnumInt.Min).HasValue(int.MinValue);
					await That(EnumInt.Max).HasValue(int.MaxValue);
					await That(EnumUInt.Min).HasValue(uint.MinValue);
					await That(EnumUInt.Max).HasValue(uint.MaxValue);
					await That(EnumLong.Int64Min).HasValue(long.MinValue);
					await That(EnumLong.Int64Max).HasValue(long.MaxValue);
					await That(EnumULong.Int64MaxPlusOne).HasValue(9223372036854775808UL);
					await That(EnumULong.UInt64Max).HasValue(ulong.MaxValue);
				}

				await That(Act).DoesNotThrow()
					.Because("every backing type from sbyte to ulong is represented exactly");
			}

			[Test]
			[Arguments(EnumULong.Int64Max, (ulong)long.MaxValue)]
			[Arguments(EnumULong.Int64MaxPlusOne, 9223372036854775808UL)]
			[Arguments(EnumULong.UInt64Max, ulong.MaxValue)]
			public async Task WhenUnsignedExpectedComesFromInlineData_ShouldSucceed(EnumULong subject, ulong expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow()
					.Because(
						"a ulong is a legal attribute argument, so even a value above long.MaxValue can be data-driven");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue((long)subject + 1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueExceedsInt64MaxValue_AndMatches_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue(ulong.MaxValue));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value equal to 18446744073709551615,
					             but it had value 18446744073709551615
					             """);
			}

			[Test]
			public async Task WhenValueMatches_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue((long)subject));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value equal to {Formatter.Format((long)subject)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}
		}
	}
}
