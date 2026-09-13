namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotFinite
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			[InlineData(double.NaN)]
			public async Task ForDouble_WhenSubjectIsInfinityOrNaN_ShouldSucceed(double subject)
			{
				async Task Act() => await That(subject).IsNotFinite();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsNotFinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			[InlineData(float.NaN)]
			public async Task ForFloat_WhenSubjectIsInfinityOrNaN_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

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
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNaNOrInfinityHalfValues))]
			public async Task ForHalf_WhenSubjectIsInfinityOrNaN_ShouldSucceed(Half subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalValue_ShouldFail(Half subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Fact]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			[InlineData(double.NaN)]
			public async Task ForNullableDouble_WhenSubjectIsInfinityOrNaN_ShouldSucceed(
				double? subject)
			{
				async Task Act() => await That(subject).IsNotFinite();

				await That(Act).DoesNotThrow();
			}


			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldFail(double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not finite,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsNotFinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			[InlineData(float.NaN)]
			public async Task ForNullableFloat_WhenSubjectIsInfinityOrNaN_ShouldSucceed(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsNotFinite();

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
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
			[Fact]
			public async Task ForNullableFloat_WhenSubjectIsNull_ShouldFail()
			{
				float? subject = null;

				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not finite,
					             but it was <null>
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNaNOrInfinityHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsInfinityOrNaN_ShouldSucceed(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act() => await That(subject).IsNotFinite();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenSubjectIsNull_ShouldFail()
			{
				Half? subject = null;

				async Task Act()
					=> await That(subject).IsNotFinite();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not finite,
					             but it was <null>
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

			public static TheoryData<Half> GetNaNOrInfinityHalfValues() =>
			[
				Half.NaN,
				Half.NegativeInfinity,
				Half.PositiveInfinity,
			];
#endif
		}
	}
}
