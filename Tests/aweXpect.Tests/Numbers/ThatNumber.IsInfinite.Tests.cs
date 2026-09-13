namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsInfinite
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldSucceed(double subject)
			{
				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			[InlineData(double.NaN)]
			public async Task ForDouble_WhenSubjectIsNormalOrNaNValue_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsInfinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			public async Task ForFloat_WhenSubjectIsInfinity_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			[InlineData(float.NaN)]
			public async Task ForFloat_WhenSubjectIsNormalOrNaNValue_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetInfinityHalfValues))]
			public async Task ForHalf_WhenSubjectIsInfinity_ShouldSucceed(Half subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalOrNaNHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalOrNaNValue_ShouldFail(Half subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Fact]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsInfinity_ShouldSucceed(double? subject)
			{
				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1d)]
			[InlineData(0d)]
			[InlineData(1d)]
			[InlineData(double.MinValue)]
			[InlineData(double.MaxValue)]
			[InlineData(double.Epsilon)]
			[InlineData(double.NaN)]
			[InlineData(null)]
			public async Task ForNullableDouble_WhenSubjectIsNormalOrNaNValueOrNull_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsInfinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity)]
			public async Task ForNullableFloat_WhenSubjectIsInfinity_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1f)]
			[InlineData(0f)]
			[InlineData(1f)]
			[InlineData(float.MinValue)]
			[InlineData(float.MaxValue)]
			[InlineData(float.Epsilon)]
			[InlineData(float.NaN)]
			[InlineData(null)]
			public async Task ForNullableFloat_WhenSubjectIsNormalOrNaNValueOrNull_ShouldFail(float? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetInfinityHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsInfinity_ShouldSucceed(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[MemberData(nameof(GetNormalOrNaNHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsNormalOrNaNValue_ShouldFail(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
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
					=> await That(subject).IsInfinite();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is infinite,
					             but it was <null>
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			public static TheoryData<Half> GetNormalOrNaNHalfValues() =>
			[
				(Half)0.0,
				(Half)1.0,
				Half.MinValue,
				Half.MaxValue,
				Half.Epsilon,
				Half.NaN,
			];

			public static TheoryData<Half> GetInfinityHalfValues() =>
			[
				Half.NegativeInfinity,
				Half.PositiveInfinity,
			];
#endif
		}
		
		public sealed class NegatedTests
		{
			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldFail(double subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsInfinite());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not infinite,
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
			[InlineData(double.NaN)]
			public async Task ForDouble_WhenSubjectIsNormalOrNaNValue_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsInfinite());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsInfinity_ShouldFail(double? subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsInfinite());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not infinite,
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
			[InlineData(double.NaN)]
			public async Task ForNullableDouble_WhenSubjectIsNormalOrNaNValue_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsInfinite());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInfinite());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not infinite,
					             but it was <null>
					             """);
			}
		}
	}
}
