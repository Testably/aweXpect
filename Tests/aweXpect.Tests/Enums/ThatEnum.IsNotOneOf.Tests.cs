using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyColors subject = MyColors.Blue;
				MyColors[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyColors subject = MyColors.Blue;
				MyColors[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData(MyColors.Blue)]
			[InlineData(MyColors.Green)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(MyColors subject)
			{
				IEnumerable<MyColors?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyColors subject = MyColors.Blue;
				MyColors?[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyColors subject = MyColors.Blue;
				MyColors?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData(MyColors.Blue)]
			[InlineData(MyColors.Green, MyColors.Blue, MyColors.Yellow)]
			public async Task WhenSubjectIsContained_ShouldFail(MyColors subject,
				params MyColors[] otherValues)
			{
				IEnumerable<MyColors> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(MyColors.Blue, MyColors.Green, MyColors.Red)]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(MyColors subject,
				params MyColors[] expected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LongTests
		{
			[Theory]
			[InlineData(EnumLong.Int64Max)]
			[InlineData(EnumLong.Int64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(EnumLong subject)
			{
				IEnumerable<EnumLong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(EnumLong.Int64Max)]
			[InlineData(EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
			public async Task WhenSubjectIsContained_ShouldFail(EnumLong subject,
				params EnumLong[] otherValues)
			{
				EnumLong[] expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(EnumLong.Int64Max, EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(EnumLong subject,
				params EnumLong[] expected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UlongTests
		{
			[Theory]
			[InlineData(EnumULong.Int64Max)]
			[InlineData(EnumULong.UInt64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(EnumULong subject)
			{
				IEnumerable<EnumULong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(EnumULong.Int64Max)]
			[InlineData(EnumULong.UInt64LessOne, EnumULong.UInt64Max, EnumULong.Int64Max)]
			public async Task WhenSubjectIsContained_ShouldFail(EnumULong subject,
				params EnumULong[] otherValues)
			{
				IEnumerable<EnumULong> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(EnumULong.UInt64Max, EnumULong.UInt64LessOne, EnumULong.Int64Max)]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(EnumULong subject,
				params EnumULong[] expected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
