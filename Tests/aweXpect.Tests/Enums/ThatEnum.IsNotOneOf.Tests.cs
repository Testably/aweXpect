using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
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

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(MyColors subject)
			{
				IEnumerable<MyColors?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
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

			[Test]
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

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green, MyColors.Blue, MyColors.Yellow)]
			public async Task WhenSubjectIsContained_ShouldFail(MyColors subject,
				params MyColors[] otherValues)
			{
				IEnumerable<MyColors> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of expected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			[Arguments(MyColors.Blue, MyColors.Green, MyColors.Red)]
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
			[Test]
			[Arguments(EnumLong.Int64Max)]
			[Arguments(EnumLong.Int64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(EnumLong subject)
			{
				IEnumerable<EnumLong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(EnumLong.Int64Max)]
			[Arguments(EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
			public async Task WhenSubjectIsContained_ShouldFail(EnumLong subject,
				params EnumLong[] otherValues)
			{
				EnumLong[] expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of expected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			[Arguments(EnumLong.Int64Max, EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
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
			[Test]
			[Arguments(EnumULong.Int64Max)]
			[Arguments(EnumULong.UInt64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(EnumULong subject)
			{
				IEnumerable<EnumULong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(EnumULong.Int64Max)]
			[Arguments(EnumULong.UInt64LessOne, EnumULong.UInt64Max, EnumULong.Int64Max)]
			public async Task WhenSubjectIsContained_ShouldFail(EnumULong subject,
				params EnumULong[] otherValues)
			{
				IEnumerable<EnumULong> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of expected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			[Arguments(EnumULong.UInt64Max, EnumULong.UInt64LessOne, EnumULong.Int64Max)]
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
