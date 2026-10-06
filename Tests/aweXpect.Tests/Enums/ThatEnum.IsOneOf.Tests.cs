using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyColors subject = MyColors.Blue;
				MyColors[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyColors subject = MyColors.Blue;
				MyColors[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green)]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail(MyColors subject)
			{
				IEnumerable<MyColors?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyColors subject = MyColors.Blue;
				MyColors?[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyColors subject = MyColors.Blue;
				MyColors?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green, MyColors.Blue, MyColors.Yellow)]
			public async Task WhenSubjectIsContained_ShouldSucceed(MyColors subject,
				params MyColors[] otherValues)
			{
				MyColors[] expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(MyColors.Blue, MyColors.Green, MyColors.Red)]
			public async Task WhenSubjectIsDifferent_ShouldFail(MyColors subject,
				params MyColors[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [Green, Red]
					              """);
			}
		}

		public sealed class LongTests
		{
			[Test]
			[Arguments(EnumLong.Int64Max)]
			[Arguments(EnumLong.Int64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail(EnumLong subject)
			{
				IEnumerable<EnumLong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Test]
			[Arguments(EnumLong.Int64Max)]
			[Arguments(EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
			public async Task WhenSubjectIsContained_ShouldSucceed(EnumLong subject,
				params EnumLong[] otherValues)
			{
				IEnumerable<EnumLong> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(EnumLong.Int64Max, EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
			public async Task WhenSubjectIsDifferent_ShouldFail(EnumLong subject,
				params EnumLong[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [Int64LessOne, Int64LessTwo]
					              """);
			}
		}

		public sealed class UlongTests
		{
			[Test]
			[Arguments(EnumULong.Int64Max)]
			[Arguments(EnumULong.UInt64LessOne)]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail(EnumULong subject)
			{
				IEnumerable<EnumULong?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Test]
			[Arguments(EnumULong.Int64Max)]
			[Arguments(EnumULong.UInt64LessOne, EnumULong.UInt64Max, EnumULong.Int64Max)]
			public async Task WhenSubjectIsContained_ShouldSucceed(EnumULong subject,
				params EnumULong[] otherValues)
			{
				IEnumerable<EnumULong> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(EnumULong.UInt64Max, EnumULong.UInt64LessOne, EnumULong.Int64Max)]
			public async Task WhenSubjectIsDifferent_ShouldFail(EnumULong subject,
				params EnumULong[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [UInt64LessOne, Int64Max]
					              """);
			}
		}
	}
}
