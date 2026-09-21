using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ForByte_ShouldSupportEnumerable()
			{
				byte subject = 1;
				IEnumerable<byte> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2, (byte)3)]
			[InlineData((byte)1, (byte)0, (byte)3)]
			public async Task ForByte_WhenValueIsDifferentToAllExpected_ShouldFail(byte subject,
				params byte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)0, (byte)1, (byte)3)]
			public async Task ForByte_WhenValueIsEqualToAnyExpected_ShouldSucceed(byte subject,
				params byte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForByte_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				byte subject = 1;
				IEnumerable<byte?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForByte_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				byte subject = 1;
				IEnumerable<byte?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDecimal_ShouldSupportEnumerable()
			{
				decimal subject = 1;
				IEnumerable<decimal> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.0, 3.0],
					             but it was 1.0
					             """);
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForDecimal_WhenValueIsDifferentToAllExpected_ShouldFail(
				double subjectValue, params double[] expectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] expected = expectedValues
					.Select(expectedValue => new decimal(expectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForDecimal_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				double subjectValue, params double[] expectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] expected = expectedValues
					.Select(expectedValue => new decimal(expectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDecimal_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				decimal subject = 1;
				IEnumerable<decimal?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.0, <null>, 3.0],
					             but it was 1.0
					             """);
			}

			[Fact]
			public async Task ForDecimal_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				decimal subject = 1;
				IEnumerable<decimal?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_ShouldSupportEnumerable()
			{
				double subject = 1.1;
				IEnumerable<double> expected = [2.1, 3.1,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.1, 3.1],
					             but it was 1.1
					             """);
			}

			[Theory]
			[InlineData(2.0, 2.0F)]
			public async Task ForDouble_WhenExpectedIsEqualFloat_ShouldSucceed(
				double subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double expected = double.NaN;

				async Task Act() => await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.NaN, 0.0, 1.0)]
			[InlineData(0.0, 1.0, double.NaN)]
			public async Task ForDouble_WhenSubjectOrExpectedIsNaN_ShouldFail(double subject,
				params double[] expected)
			{
				async Task Act() => await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForDouble_WhenValueIsDifferentToAllExpected_ShouldFail(
				double subject, params double[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForDouble_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				double subject, params double?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				double subject = 1.1;
				IEnumerable<double?> expected = [2.1, null, 3.1,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.1, <null>, 3.1],
					             but it was 1.1
					             """);
			}

			[Fact]
			public async Task ForDouble_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				double subject = 1.1;
				IEnumerable<double?> expected = [null, 1.1, 3.1,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_ShouldSupportEnumerable()
			{
				float subject = 1.1F;
				IEnumerable<float> expected = [2.1F, 3.1F,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.1, 3.1],
					             but it was 1.1
					             """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float expected = float.NaN;

				async Task Act() => await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.NaN, 0.0F, 1.0F)]
			[InlineData(0.0F, 1.0F, float.NaN)]
			public async Task ForFloat_WhenSubjectOrExpectedIsNaN_ShouldFail(float subject,
				params float[] expected)
			{
				async Task Act() => await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1, (float)3.1)]
			[InlineData((float)1.1, (float)0.1, (float)3.1)]
			public async Task ForFloat_WhenValueIsDifferentToAllExpected_ShouldFail(
				float subject, params float[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)0.1, (float)1.1, (float)3.1)]
			public async Task ForFloat_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				float subject, params float?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				float subject = 1.1F;
				IEnumerable<float?> expected = [2.1F, null, 3.1F,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.1, <null>, 3.1],
					             but it was 1.1
					             """);
			}

			[Fact]
			public async Task ForFloat_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				float subject = 1.1F;
				IEnumerable<float?> expected = [null, 1.1F, 3.1F,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_ShouldSupportEnumerable()
			{
				int subject = 1;
				IEnumerable<int> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForInt_WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int subject = 1;
				int[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForInt_WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				int subject = 1;
				int[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task ForInt_WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int subject = 1;
				int?[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForInt_WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				int subject = 1;
				int?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData(1, 2, 3)]
			[InlineData(2, 1, 3)]
			public async Task ForInt_WhenValueIsDifferentToAllExpected_ShouldFail(int subject,
				params int?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1, 0, 1, 3)]
			public async Task ForInt_WhenValueIsEqualToAnyExpected_ShouldSucceed(int subject,
				params int?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				int subject = 1;
				IEnumerable<int?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForInt_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				int subject = 1;
				IEnumerable<int?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForLong_ShouldSupportEnumerable()
			{
				long subject = 1;
				IEnumerable<long> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData(1L, 1)]
			public async Task ForLong_WhenExpectedIsEqualInt_ShouldSucceed(long subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)2, (long)3)]
			[InlineData((long)1, (long)0, (long)3)]
			public async Task ForLong_WhenValueIsDifferentToAllExpected_ShouldFail(long subject,
				params long?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)0, (long)1, (long)3)]
			public async Task ForLong_WhenValueIsEqualToAnyExpected_ShouldSucceed(long subject,
				params long?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForLong_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				long subject = 1;
				IEnumerable<long?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForLong_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				long subject = 1;
				IEnumerable<long?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableByte_ShouldSupportEnumerable()
			{
				byte? subject = 1;
				IEnumerable<byte?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2, (byte)3)]
			[InlineData((byte)1, (byte)0, (byte)3)]
			public async Task ForNullableByte_WhenValueIsDifferentToAllExpected_ShouldFail(
				byte? subject, params byte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)0, (byte)1, (byte)3)]
			public async Task ForNullableByte_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				byte? subject, params byte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				params byte?[] expected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableDecimal_ShouldSupportEnumerable()
			{
				decimal? subject = 1;
				IEnumerable<decimal?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.0, 3.0],
					             but it was 1.0
					             """);
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForNullableDecimal_WhenValueIsDifferentToAllExpected_ShouldFail(
				double? subjectValue, params double?[] expectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] expected = expectedValues
					.Select(expectedValue => expectedValue == null
						? (decimal?)null
						: new decimal(expectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForNullableDecimal_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				double? subjectValue, params double?[] expectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] expected = expectedValues
					.Select(expectedValue => expectedValue == null
						? (decimal?)null
						: new decimal(expectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_ShouldSupportEnumerable()
			{
				double? subject = 1;
				IEnumerable<double?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.0, 3.0],
					             but it was 1.0
					             """);
			}

			[Theory]
			[InlineData(1.1, 2.1, 3.1)]
			[InlineData(1.1, 0.1, 3.1)]
			public async Task ForNullableDouble_WhenValueIsDifferentToAllExpected_ShouldFail(
				double? subject, params double?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 0.1, 1.1, 3.1)]
			public async Task ForNullableDouble_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				double? subject, params double?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_ShouldSupportEnumerable()
			{
				float? subject = 1;
				IEnumerable<float?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2.0, 3.0],
					             but it was 1.0
					             """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1, (float)3.1)]
			[InlineData((float)1.1, (float)0.1, (float)3.1)]
			public async Task ForNullableFloat_WhenValueIsDifferentToAllExpected_ShouldFail(
				float? subject, params float?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)0.1, (float)1.1, (float)3.1)]
			public async Task ForNullableFloat_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				float? subject, params float?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_ShouldSupportEnumerable()
			{
				int? subject = 1;
				IEnumerable<int?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForNullableInt_WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int? subject = 1;
				int[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForNullableInt_WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				int? subject = 1;
				int[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task ForNullableInt_WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				int? subject = 1;
				int?[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task ForNullableInt_WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				int? subject = 1;
				int?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData(1, 2, 3)]
			[InlineData(1, 0, 3)]
			public async Task ForNullableInt_WhenValueIsDifferentToAllExpected_ShouldFail(
				int? subject, params int?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1, 0, 1, 3)]
			public async Task ForNullableInt_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				int? subject, params int?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				params int?[] expected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableLong_ShouldSupportEnumerable()
			{
				long? subject = 1;
				IEnumerable<long?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((long)1, (long)2, (long)3)]
			[InlineData((long)1, (long)0, (long)3)]
			public async Task ForNullableLong_WhenValueIsDifferentToAllExpected_ShouldFail(
				long? subject, params long?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)0, (long)1, (long)3)]
			public async Task ForNullableLong_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				long? subject, params long?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				params long?[] expected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableSbyte_ShouldSupportEnumerable()
			{
				sbyte? subject = 1;
				IEnumerable<sbyte?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, (sbyte)3)]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)3)]
			public async Task ForNullableSbyte_WhenValueIsDifferentToAllExpected_ShouldFail(
				sbyte? subject, params sbyte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)1, (sbyte)3)]
			public async Task ForNullableSbyte_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				sbyte? subject, params sbyte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				params sbyte?[] expected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableShort_ShouldSupportEnumerable()
			{
				short? subject = 1;
				IEnumerable<short?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((short)1, (short)2, (short)3)]
			[InlineData((short)1, (short)0, (short)3)]
			public async Task ForNullableShort_WhenValueIsDifferentToAllExpected_ShouldFail(
				short? subject, params short?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)0, (short)1, (short)3)]
			public async Task ForNullableShort_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				short? subject, params short?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				params short?[] expected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUint_ShouldSupportEnumerable()
			{
				uint? subject = 1;
				IEnumerable<uint?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2, (uint)3)]
			[InlineData((uint)1, (uint)0, (uint)3)]
			public async Task ForNullableUint_WhenValueIsDifferentToAllExpected_ShouldFail(
				uint? subject, params uint?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)0, (uint)1, (uint)3)]
			public async Task ForNullableUint_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				uint? subject, params uint?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				params uint?[] expected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUlong_ShouldSupportEnumerable()
			{
				ulong? subject = 1;
				IEnumerable<ulong?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, (ulong)3)]
			[InlineData((ulong)1, (ulong)0, (ulong)3)]
			public async Task ForNullableUlong_WhenValueIsDifferentToAllExpected_ShouldFail(
				ulong? subject, params ulong?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)0, (ulong)1, (ulong)3)]
			public async Task ForNullableUlong_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				ulong? subject, params ulong?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				params ulong?[] expected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUshort_ShouldSupportEnumerable()
			{
				ushort? subject = 1;
				IEnumerable<ushort?> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, (ushort)3)]
			[InlineData((ushort)1, (ushort)0, (ushort)3)]
			public async Task ForNullableUshort_WhenValueIsDifferentToAllExpected_ShouldFail(
				ushort? subject, params ushort?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)0, (ushort)1, (ushort)3)]
			public async Task ForNullableUshort_WhenValueIsEqualToAnyExpected_ShouldSucceed(
				ushort? subject, params ushort?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				params ushort?[] expected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForSbyte_ShouldSupportEnumerable()
			{
				sbyte subject = 1;
				IEnumerable<sbyte> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, (sbyte)3)]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)3)]
			public async Task ForSbyte_WhenValueIsDifferentToAllExpected_ShouldFail(sbyte subject,
				params sbyte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)0, (sbyte)1, (sbyte)3)]
			public async Task ForSbyte_WhenValueIsEqualToAnyExpected_ShouldSucceed(sbyte subject,
				params sbyte?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForSbyte_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				sbyte subject = 1;
				IEnumerable<sbyte?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForSbyte_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				sbyte subject = 1;
				IEnumerable<sbyte?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForShort_ShouldSupportEnumerable()
			{
				short subject = 1;
				IEnumerable<short> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((short)1, (short)2, (short)3)]
			[InlineData((short)1, (short)0, (short)3)]
			public async Task ForShort_WhenValueIsDifferentToAllExpected_ShouldFail(short subject,
				params short?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)0, (short)1, (short)3)]
			public async Task ForShort_WhenValueIsEqualToAnyExpected_ShouldSucceed(short subject,
				params short?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForShort_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				short subject = 1;
				IEnumerable<short?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForShort_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				short subject = 1;
				IEnumerable<short?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUint_ShouldSupportEnumerable()
			{
				uint subject = 1;
				IEnumerable<uint> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2, (uint)3)]
			[InlineData((uint)1, (uint)0, (uint)3)]
			public async Task ForUint_WhenValueIsDifferentToAllExpected_ShouldFail(uint subject,
				params uint?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)0, (uint)1, (uint)3)]
			public async Task ForUint_WhenValueIsEqualToAnyExpected_ShouldSucceed(uint subject,
				params uint?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUint_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				uint subject = 1;
				IEnumerable<uint?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForUint_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				uint subject = 1;
				IEnumerable<uint?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUlong_ShouldSupportEnumerable()
			{
				ulong subject = 1;
				IEnumerable<ulong> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, (ulong)3)]
			[InlineData((ulong)1, (ulong)0, (ulong)3)]
			public async Task ForUlong_WhenValueIsDifferentToAllExpected_ShouldFail(ulong subject,
				params ulong?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)0, (ulong)1, (ulong)3)]
			public async Task ForUlong_WhenValueIsEqualToAnyExpected_ShouldSucceed(ulong subject,
				params ulong?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUlong_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				ulong subject = 1;
				IEnumerable<ulong?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForUlong_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				ulong subject = 1;
				IEnumerable<ulong?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUshort_ShouldSupportEnumerable()
			{
				ushort subject = 1;
				IEnumerable<ushort> expected = [2, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, 3],
					             but it was 1
					             """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, (ushort)3)]
			[InlineData((ushort)1, (ushort)0, (ushort)3)]
			public async Task ForUshort_WhenValueIsDifferentToAllExpected_ShouldFail(ushort subject,
				params ushort?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)0, (ushort)1, (ushort)3)]
			public async Task ForUshort_WhenValueIsEqualToAnyExpected_ShouldSucceed(ushort subject,
				params ushort?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForUshort_WithNullableEnumerable_WhenValueIsDifferentToAllExpected_ShouldFail()
			{
				ushort subject = 1;
				IEnumerable<ushort?> expected = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [2, <null>, 3],
					             but it was 1
					             """);
			}

			[Fact]
			public async Task ForUshort_WithNullableEnumerable_WhenValueIsEqualToAnyExpected_ShouldSucceed()
			{
				ushort subject = 1;
				IEnumerable<ushort?> expected = [null, 1, 3,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
			{
				int? subject = null;
				IEnumerable<int?> expected = [1, null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
