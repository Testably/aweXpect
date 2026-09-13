namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNaN
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double subject = double.NaN;

				async Task Act() => await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			[InlineData(1.0)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.NaN;

				async Task Act() => await That(subject).IsNaN()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			public async Task ForFloat_WhenSubjectIsInfinity_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				float subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			public async Task ForFloat_WhenSubjectIsNormalValue_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.NaN;

				async Task Act()
					=> await That(subject).IsNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsNaN_ShouldSucceed()
			{
				Half subject = Half.NaN;

				async Task Act() => await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsNegativeInfinity_ShouldFail()
			{
				Half subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is NaN,
					             but it was -∞
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalValue_ShouldFail(Half subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsPositiveInfinity_ShouldFail()
			{
				Half subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is NaN,
					             but it was +∞
					             """);
			}
#endif

			[Fact]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			[InlineData(null)]
			public async Task ForNullableDouble_WhenSubjectIsInfinityOrNull_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double? subject = double.NaN;

				async Task Act() => await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			[InlineData(1.0)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.NaN;

				async Task Act() => await That(subject).IsNaN()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			public async Task ForNullableFloat_WhenSubjectIsInfinity_ShouldFail(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				float? subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			public async Task ForNullableFloat_WhenSubjectIsNormalValue_ShouldFail(float? subject)
			{
				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.NaN;

				async Task Act()
					=> await That(subject).IsNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsNaN_ShouldSucceed()
			{
				Half? subject = Half.NaN;

				async Task Act() => await That(subject).IsNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsNegativeInfinity_ShouldFail()
			{
				Half? subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is NaN,
					             but it was -∞
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsNormalValue_ShouldFail(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsPositiveInfinity_ShouldFail()
			{
				Half? subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is NaN,
					             but it was +∞
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			public static TheoryData<Half> GetNormalHalfValues() =>
			[
				(Half)0.0,
				(Half)1.0,
				Half.MinValue,
				Half.MaxValue,
				Half.Epsilon,
			];
#endif
		}
		
		public sealed class NegatedTests
		{
			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNaN());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;

				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsNaN());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			[InlineData(1.0)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNaN());

				await That(Act).DoesNotThrow();
			}
			
			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsInfinity_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNaN());

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double? subject = double.NaN;

				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsNaN());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}
			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNaN());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not NaN,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			[InlineData(1.0)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNaN());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
