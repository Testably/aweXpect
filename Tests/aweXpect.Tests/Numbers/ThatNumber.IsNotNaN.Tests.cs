namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotNaN
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.Epsilon;

				async Task Act()
					=> await That(subject).IsNotNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;

				async Task Act() => await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			[InlineData(double.NegativeInfinity)]
			[InlineData(double.PositiveInfinity)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.Epsilon;

				async Task Act() => await That(subject).IsNotNaN()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldFail()
			{
				float subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			[InlineData(float.NegativeInfinity)]
			[InlineData(float.PositiveInfinity)]
			public async Task ForFloat_WhenSubjectIsNormalValue_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.Epsilon;

				async Task Act()
					=> await That(subject).IsNotNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsNaN_ShouldFail()
			{
				Half subject = Half.NaN;

				async Task Act() => await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsNegativeInfinity_ShouldSucceed()
			{
				Half subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalValue_ShouldSucceed(Half subject)
			{
				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsPositiveInfinity_ShouldSucceed()
			{
				Half subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.Epsilon;

				async Task Act()
					=> await That(subject).IsNotNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double? subject = double.NaN;

				async Task Act() => await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not NaN,
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			[InlineData(double.NegativeInfinity)]
			[InlineData(double.PositiveInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not NaN,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.Epsilon;

				async Task Act() => await That(subject).IsNotNaN()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_WhenSubjectIsNaN_ShouldFail()
			{
				float? subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not NaN,
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			[InlineData(float.NegativeInfinity)]
			[InlineData(float.PositiveInfinity)]
			public async Task ForNullableFloat_WhenSubjectIsNormalValue_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.Epsilon;

				async Task Act()
					=> await That(subject).IsNotNaN()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsNaN_ShouldFail()
			{
				Half? subject = Half.NaN;

				async Task Act() => await That(subject).IsNotNaN();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not NaN,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsNegativeInfinity_ShouldSucceed()
			{
				Half? subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsNormalValue_ShouldSucceed(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsPositiveInfinity_ShouldSucceed()
			{
				Half? subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNaN();

				await That(Act).DoesNotThrow();
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
	}
}
