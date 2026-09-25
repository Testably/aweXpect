namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class HasValue
	{
		public sealed class ContinuationTests
		{
			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, 3L)]
			public async Task ShouldSupportTheComparisonVocabulary(MyNumbers subject, long maximum)
			{
				async Task Act()
					=> await That(subject).HasValue().LessThan(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
			public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().Between(5L).And(2L);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Theory]
			[InlineData(null, 3L)]
			[InlineData(1L, null)]
			public async Task Between_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(long? minimum, long? maximum)
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().Between(minimum).And(maximum));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it had value 2
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Fact]
			public async Task Between_WhenTheRangeExceedsInt64MaxValue_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().Between((ulong)long.MaxValue).And(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a range above long.MaxValue is a legal range for a ulong-backed enum");
			}

			[Fact]
			public async Task Between_WhenTheValueIsInTheRange_ShouldSucceed()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).HasValue().Between(1L).And(3L);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Between_WhenUnsignedMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				EnumULong subject = EnumULong.UInt64LessOne;

				async Task Act()
					=> await That(subject).HasValue().Between(ulong.MaxValue).And((ulong)long.MaxValue);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Fact]
			public async Task GreaterThan_WhenExpectedIsNegative_AndTheBackingTypeIsUnsigned_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(-1);

				await That(Act).DoesNotThrow()
					.Because("a negative expected value is below every value a ulong-backed enum can have");
			}

			[Fact]
			public async Task GreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().GreaterThan(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task GreaterThan_WhenTheValueExceedsInt64MaxValue_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(0);

				await That(Act).DoesNotThrow()
					.Because("a ulong-backed member above long.MaxValue is a legal enum value and must not overflow");
			}

			[Fact]
			public async Task GreaterThan_WhenTheValueIsTheMaximumOfItsBackingType_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThan(ulong.MaxValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than 18446744073709551615,
					             but it had value 18446744073709551615
					             """);
			}

			[Fact]
			public async Task GreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().GreaterThanOrEqualTo(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have value greater than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task GreaterThanOrEqualTo_WhenTheValueEqualsTheMaximumOfItsBackingType_ShouldSucceed()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().GreaterThanOrEqualTo(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("ulong.MaxValue is compared exactly rather than approximated");
			}

			[Fact]
			public async Task GreaterThanOrEqualTo_WhenTheValueIsInt64MinValue_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().GreaterThanOrEqualTo(0L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value greater than or equal to 0,
					             but it had value -9223372036854775808
					             """);
			}

			[Fact]
			public async Task LessThan_WhenExpectedExceedsInt64MaxValue_AndTheSubjectIsSignedBacked_ShouldSucceed()
			{
				EnumLong subject = EnumLong.Int64Max;

				async Task Act()
					=> await That(subject).HasValue().LessThan(ulong.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("an unsigned expected value above long.MaxValue compares against a signed subject as well");
			}

			[Fact]
			public async Task LessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().LessThan(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task LessThan_WhenTheValueIsInt64MinValue_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().LessThan(long.MinValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value less than -9223372036854775808,
					             but it had value -9223372036854775808
					             """);
			}

			[Fact]
			public async Task LessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				MyNumbers subject = MyNumbers.Two;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue().LessThanOrEqualTo(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have value less than or equal to <null>,
					             but it had value 2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task LessThanOrEqualTo_WhenTheValueEqualsInt64MinValue_ShouldSucceed()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue().LessThanOrEqualTo(long.MinValue);

				await That(Act).DoesNotThrow()
					.Because("long.MinValue is compared exactly rather than approximated");
			}

			[Fact]
			public async Task LessThanOrEqualTo_WhenTheValueExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue().LessThanOrEqualTo(ulong.MaxValue - 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value less than or equal to 18446744073709551614,
					             but it had value 18446744073709551615
					             """);
			}

			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, -7L)]
			[InlineData(MyNumbers.Three, 0L)]
			public async Task NotEqualTo_WhenSubjectDoesNotHaveUnexpectedValue_ShouldSucceed(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(MyNumbers.One, 1L)]
			[InlineData(MyNumbers.Two, 2L)]
			[InlineData(MyNumbers.Three, 3L)]
			public async Task NotEqualTo_WhenSubjectHasUnexpectedValue_ShouldFail(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value not equal to {Formatter.Format(unexpected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Fact]
			public async Task NotEqualTo_WhenTheValueExceedsInt64MaxValue_ShouldFail()
			{
				EnumULong subject = EnumULong.Int64MaxPlusOne;

				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(9223372036854775808UL);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value not equal to 9223372036854775808,
					             but it had value 9223372036854775808
					             """);
			}

			[Fact]
			public async Task NotEqualTo_WhenUnexpectedIsNull_ShouldSucceed()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).HasValue().NotEqualTo(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldRenderLikeTheShorthand()
			{
				MyNumbers subject = MyNumbers.One;

				async Task Act()
					=> await That(subject).HasValue().EqualTo(2L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to 2,
					             but it had value 1
					             """)
					.Because("the continuation renders exactly like the HasValue(expected) shorthand");
			}
		}

		public sealed class Tests
		{
			[Theory]
			[InlineData(EnumLong.Int64Min, long.MinValue)]
			[InlineData(EnumLong.Int64LessOne, long.MaxValue - 1)]
			[InlineData(EnumLong.Int64Max, long.MaxValue)]
			public async Task WhenExpectedComesFromInlineData_ShouldSucceed(EnumLong subject, long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow()
					.Because("a long is a legal attribute argument, so the expected value can be data-driven");
			}

			[Fact]
			public async Task WhenExpectedExceedsInt64MaxValue_AndTheSubjectIsNegative_ShouldFail()
			{
				EnumLong subject = EnumLong.Int64Min;

				async Task Act()
					=> await That(subject).HasValue(ulong.MaxValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to 18446744073709551615,
					             but it had value -9223372036854775808
					             """)
					.Because("a value no member of a long-backed enum can have fails instead of overflowing");
			}

			[Fact]
			public async Task WhenExpectedIsNegative_AndTheBackingTypeIsUnsigned_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).HasValue(-1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has value equal to -1,
					             but it had value 18446744073709551615
					             """)
					.Because("a value no member of the enum can have fails instead of overflowing the conversion");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).HasValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to <null>,
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, -7)]
			[InlineData(MyNumbers.Three, 0)]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldFail(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to {Formatter.Format(expected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Theory]
			[InlineData(MyNumbers.One, 1)]
			[InlineData(MyNumbers.Two, 2)]
			[InlineData(MyNumbers.Three, 3)]
			public async Task WhenSubjectHasExpectedValue_ShouldSucceed(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Theory]
			[InlineData(EnumULong.Int64Max, (ulong)long.MaxValue)]
			[InlineData(EnumULong.Int64MaxPlusOne, 9223372036854775808UL)]
			[InlineData(EnumULong.UInt64Max, ulong.MaxValue)]
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
			[Fact]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue((long)subject + 1));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueExceedsInt64MaxValue_AndMatches_ShouldFail()
			{
				EnumULong subject = EnumULong.UInt64Max;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue(ulong.MaxValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have value equal to 18446744073709551615,
					             but it had value 18446744073709551615
					             """);
			}

			[Fact]
			public async Task WhenValueMatches_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValue((long)subject));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have value equal to {Formatter.Format((long)subject)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}
		}
	}
}
