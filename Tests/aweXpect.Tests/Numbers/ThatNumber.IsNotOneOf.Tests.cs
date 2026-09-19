using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForByte_ShouldSupportEnumerable()
			{
				byte subject = 2;
				IEnumerable<byte> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForByte_WhenUnexpectedIsNull_ShouldSucceed(
				byte subject)
			{
				byte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)2, (byte)3)]
			[InlineData((byte)1, (byte)0, (byte)3)]
			public async Task ForByte_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(byte subject,
				params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)0, (byte)1, (byte)3)]
			public async Task ForByte_WhenValueIsEqualToAnyUnexpected_ShouldFail(byte subject,
				params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForByte_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				byte subject = 2;
				IEnumerable<byte?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForByte_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				byte subject = 2;
				IEnumerable<byte?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForDecimal_ShouldSupportEnumerable()
			{
				decimal subject = 2;
				IEnumerable<decimal> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForDecimal_WhenUnexpectedIsNull_ShouldSucceed(decimal subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForDecimal_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				double subjectValue, params double[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForDecimal_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				double subjectValue, params double[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDecimal_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				decimal subject = 2;
				IEnumerable<decimal?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDecimal_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				decimal subject = 2;
				IEnumerable<decimal?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, <null>, 2.0],
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task ForDouble_ShouldSupportEnumerable()
			{
				double subject = 2;
				IEnumerable<double> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndExpectedAreNaN_ShouldFail()
			{
				double subject = double.NaN;
				double[] expected = [double.NaN,];

				async Task Act() => await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [NaN],
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(double.NaN, 0.0, 1.0)]
			[InlineData(0.0, 1.0, double.NaN)]
			public async Task ForDouble_WhenSubjectOrExpectedIsNaN_ShouldSucceed(
				double subject, params double[] expected)
			{
				async Task Act() => await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 2.0F)]
			public async Task ForDouble_WhenUnexpectedIsEqualFloat_ShouldFail(
				double subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of [{Formatter.Format(unexpected)}],
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForDouble_WhenUnexpectedIsNull_ShouldSucceed(double subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForDouble_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForDouble_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				double subject = 2;
				IEnumerable<double?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				double subject = 2;
				IEnumerable<double?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, <null>, 2.0],
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task ForFloat_ShouldSupportEnumerable()
			{
				float subject = 2;
				IEnumerable<float> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectAndExpectedAreNaN_ShouldFail()
			{
				float subject = float.NaN;
				float[] expected = [float.NaN,];

				async Task Act() => await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [NaN],
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(float.NaN, 0.0F, 1.0F)]
			[InlineData(0.0F, 1.0F, float.NaN)]
			public async Task ForFloat_WhenSubjectOrExpectedIsNaN_ShouldSucceed(
				float subject, params float[] expected)
			{
				async Task Act() => await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForFloat_WhenUnexpectedIsNull_ShouldSucceed(float subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1, (float)3.1)]
			[InlineData((float)1.1, (float)0.1, (float)3.1)]
			public async Task ForFloat_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)0.1, (float)1.1, (float)3.1)]
			public async Task ForFloat_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				float subject = 2F;
				IEnumerable<float?> unexpected = [1F, null, 3F,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				float subject = 2F;
				IEnumerable<float?> unexpected = [1F, null, 2F,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, <null>, 2.0],
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task ForInt_ShouldSupportEnumerable()
			{
				int subject = 2;
				IEnumerable<int> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForInt_WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int subject = 1;
				int[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForInt_WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int subject = 1;
				int?[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Theory]
			[AutoData]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldSucceed(
				int subject)
			{
				int? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 2, 3)]
			[InlineData(2, 1, 3)]
			public async Task ForInt_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(int subject,
				params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 0, 1, 3)]
			public async Task ForInt_WhenValueIsEqualToAnyUnexpected_ShouldFail(int subject,
				params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForInt_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				int subject = 2;
				IEnumerable<int?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				int subject = 2;
				IEnumerable<int?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForLong_ShouldSupportEnumerable()
			{
				long subject = 2;
				IEnumerable<long> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData(1L, 1)]
			public async Task ForLong_WhenUnexpectedIsEqualToInt_ShouldFail(
				long subject, int unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of [{Formatter.Format(unexpected)}],
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForLong_WhenUnexpectedIsNull_ShouldSucceed(
				long subject)
			{
				long? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)2, (long)3)]
			[InlineData((long)1, (long)0, (long)3)]
			public async Task ForLong_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(long subject,
				params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)0, (long)1, (long)3)]
			public async Task ForLong_WhenValueIsEqualToAnyUnexpected_ShouldFail(long subject,
				params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForLong_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				long subject = 2;
				IEnumerable<long?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForLong_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				long subject = 2;
				IEnumerable<long?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForNullableByte_ShouldSupportEnumerable()
			{
				byte? subject = 2;
				IEnumerable<byte?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2, (byte)3)]
			[InlineData((byte)1, (byte)0, (byte)3)]
			public async Task ForNullableByte_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)0, (byte)1, (byte)3)]
			public async Task ForNullableByte_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldSucceed(
				params byte?[] unexpected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDecimal_ShouldSupportEnumerable()
			{
				decimal? subject = 2;
				IEnumerable<decimal?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDecimal_WhenUnexpectedIsNull_ShouldSucceed(decimal? subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForNullableDecimal_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				double? subjectValue, params double?[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForNullableDecimal_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				double? subjectValue, params double?[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_ShouldSupportEnumerable()
			{
				double? subject = 2;
				IEnumerable<double?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDouble_WhenUnexpectedIsNull_ShouldSucceed(double? subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForNullableDouble_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForNullableDouble_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportEnumerable()
			{
				float? subject = 2;
				IEnumerable<float?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.0, 2.0, 3.0],
					             but it was 2.0
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableFloat_WhenUnexpectedIsNull_ShouldSucceed(float? subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1, (float)3.1)]
			[InlineData((float)1.1, (float)0.1, (float)3.1)]
			public async Task ForNullableFloat_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)0.1, (float)1.1, (float)3.1)]
			public async Task ForNullableFloat_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableInt_ShouldSupportEnumerable()
			{
				int? subject = 2;
				IEnumerable<int?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForNullableInt_WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int? subject = 1;
				int[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForNullableInt_WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int? subject = 1;
				int?[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Theory]
			[InlineData(1, 2, 3)]
			[InlineData(1, 0, 3)]
			public async Task ForNullableInt_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 0, 1, 3)]
			public async Task ForNullableInt_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldSucceed(
				params int?[] unexpected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableLong_ShouldSupportEnumerable()
			{
				long? subject = 2;
				IEnumerable<long?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((long)1, (long)2, (long)3)]
			[InlineData((long)1, (long)0, (long)3)]
			public async Task ForNullableLong_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)0, (long)1, (long)3)]
			public async Task ForNullableLong_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldSucceed(
				params long?[] unexpected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableSbyte_ShouldSupportEnumerable()
			{
				sbyte? subject = 2;
				IEnumerable<sbyte?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, (sbyte)3)]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)3)]
			public async Task ForNullableSbyte_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)1, (sbyte)3)]
			public async Task ForNullableSbyte_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldSucceed(
				params sbyte?[] unexpected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableShort_ShouldSupportEnumerable()
			{
				short? subject = 2;
				IEnumerable<short?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((short)1, (short)2, (short)3)]
			[InlineData((short)1, (short)0, (short)3)]
			public async Task ForNullableShort_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)0, (short)1, (short)3)]
			public async Task ForNullableShort_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldSucceed(
				params short?[] unexpected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableUint_ShouldSupportEnumerable()
			{
				uint? subject = 2;
				IEnumerable<uint?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2, (uint)3)]
			[InlineData((uint)1, (uint)0, (uint)3)]
			public async Task ForNullableUint_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)0, (uint)1, (uint)3)]
			public async Task ForNullableUint_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldSucceed(
				params uint?[] unexpected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableUlong_ShouldSupportEnumerable()
			{
				ulong? subject = 2;
				IEnumerable<ulong?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, (ulong)3)]
			[InlineData((ulong)1, (ulong)0, (ulong)3)]
			public async Task ForNullableUlong_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)0, (ulong)1, (ulong)3)]
			public async Task ForNullableUlong_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldSucceed(
				params ulong?[] unexpected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableUshort_ShouldSupportEnumerable()
			{
				ushort? subject = 2;
				IEnumerable<ushort?> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, (ushort)3)]
			[InlineData((ushort)1, (ushort)0, (ushort)3)]
			public async Task ForNullableUshort_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)0, (ushort)1, (ushort)3)]
			public async Task ForNullableUshort_WhenValueIsEqualToAnyUnexpected_ShouldFail(
				ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldSucceed(
				params ushort?[] unexpected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForSbyte_ShouldSupportEnumerable()
			{
				sbyte subject = 2;
				IEnumerable<sbyte> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForSbyte_WhenUnexpectedIsNull_ShouldSucceed(
				sbyte subject)
			{
				sbyte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, (sbyte)3)]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)3)]
			public async Task ForSbyte_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(sbyte subject,
				params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)1, (sbyte)3)]
			public async Task ForSbyte_WhenValueIsEqualToAnyUnexpected_ShouldFail(sbyte subject,
				params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForSbyte_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				sbyte subject = 2;
				IEnumerable<sbyte?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForSbyte_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				sbyte subject = 2;
				IEnumerable<sbyte?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForShort_ShouldSupportEnumerable()
			{
				short subject = 2;
				IEnumerable<short> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForShort_WhenUnexpectedIsNull_ShouldSucceed(
				short subject)
			{
				short? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)2, (short)3)]
			[InlineData((short)1, (short)0, (short)3)]
			public async Task ForShort_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(short subject,
				params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)0, (short)1, (short)3)]
			public async Task ForShort_WhenValueIsEqualToAnyUnexpected_ShouldFail(short subject,
				params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForShort_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				short subject = 2;
				IEnumerable<short?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForShort_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				short subject = 2;
				IEnumerable<short?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForUint_ShouldSupportEnumerable()
			{
				uint subject = 2;
				IEnumerable<uint> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForUint_WhenUnexpectedIsNull_ShouldSucceed(
				uint subject)
			{
				uint? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)2, (uint)3)]
			[InlineData((uint)1, (uint)0, (uint)3)]
			public async Task ForUint_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(uint subject,
				params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)0, (uint)1, (uint)3)]
			public async Task ForUint_WhenValueIsEqualToAnyUnexpected_ShouldFail(uint subject,
				params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForUint_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				uint subject = 2;
				IEnumerable<uint?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUint_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				uint subject = 2;
				IEnumerable<uint?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForUlong_ShouldSupportEnumerable()
			{
				ulong subject = 2;
				IEnumerable<ulong> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForUlong_WhenUnexpectedIsNull_ShouldSucceed(
				ulong subject)
			{
				ulong? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, (ulong)3)]
			[InlineData((ulong)1, (ulong)0, (ulong)3)]
			public async Task ForUlong_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(ulong subject,
				params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)0, (ulong)1, (ulong)3)]
			public async Task ForUlong_WhenValueIsEqualToAnyUnexpected_ShouldFail(ulong subject,
				params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForUlong_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				ulong subject = 2;
				IEnumerable<ulong?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUlong_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				ulong subject = 2;
				IEnumerable<ulong?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task ForUshort_ShouldSupportEnumerable()
			{
				ushort subject = 2;
				IEnumerable<ushort> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, 2, 3],
					             but it was 2
					             """);
			}

			[Theory]
			[AutoData]
			public async Task ForUshort_WhenUnexpectedIsNull_ShouldSucceed(
				ushort subject)
			{
				ushort? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, (ushort)3)]
			[InlineData((ushort)1, (ushort)0, (ushort)3)]
			public async Task ForUshort_WhenValueIsDifferentToAllUnexpected_ShouldSucceed(
				ushort subject,
				params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)0, (ushort)1, (ushort)3)]
			public async Task ForUshort_WhenValueIsEqualToAnyUnexpected_ShouldFail(ushort subject,
				params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForUshort_WithNullableEnumerable_WhenValueIsDifferentToAllUnexpected_ShouldSucceed()
			{
				ushort subject = 2;
				IEnumerable<ushort?> unexpected = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUshort_WithNullableEnumerable_WhenValueIsEqualToAnyUnexpected_ShouldFail()
			{
				ushort subject = 2;
				IEnumerable<ushort?> unexpected = [1, null, 2,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1, <null>, 2],
					             but it was 2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
			{
				int? subject = null;
				IEnumerable<int?> expected = [1, null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}
		}
	}
}
