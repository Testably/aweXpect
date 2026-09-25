using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					MyColors? subject = MyColors.Blue;
					MyColors[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					MyColors? subject = MyColors.Blue;
					MyColors[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green)]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail(MyColors? subject)
				{
					IEnumerable<MyColors?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of [<null>],
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					MyColors? subject = MyColors.Blue;
					MyColors?[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Fact]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					MyColors? subject = MyColors.Blue;
					MyColors?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green, MyColors.Blue, MyColors.Yellow)]
				public async Task WhenSubjectIsContained_ShouldSucceed(MyColors? subject,
					params MyColors[] otherValues)
				{
					MyColors?[] expected = [..otherValues, subject,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(MyColors.Blue, MyColors.Green, MyColors.Red)]
				public async Task WhenSubjectIsDifferent_ShouldFail(MyColors? subject,
					params MyColors[] expected)
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

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;
					IEnumerable<MyColors?> expected = [MyColors.Green, MyColors.Blue,];

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
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					MyColors? subject = null;
					IEnumerable<MyColors?> expected = [MyColors.Green, null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LongTests
			{
				[Theory]
				[InlineData(EnumLong.Int64Max)]
				[InlineData(EnumLong.Int64LessOne)]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail(EnumLong? subject)
				{
					IEnumerable<EnumLong?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of [<null>],
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(EnumLong.Int64Max)]
				[InlineData(EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
				public async Task WhenSubjectIsContained_ShouldSucceed(EnumLong? subject,
					params EnumLong[] otherValues)
				{
					EnumLong?[] expected = [..otherValues, subject,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(EnumLong.Int64Max, EnumLong.Int64LessOne, EnumLong.Int64LessTwo)]
				public async Task WhenSubjectIsDifferent_ShouldFail(EnumLong? subject,
					params EnumLong[] expected)
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
			}

			public sealed class UlongTests
			{
				[Theory]
				[InlineData(EnumULong.Int64Max)]
				[InlineData(EnumULong.UInt64LessOne)]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail(EnumULong? subject)
				{
					IEnumerable<EnumULong?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of [<null>],
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(EnumULong.Int64Max)]
				[InlineData(EnumULong.UInt64LessOne, EnumULong.UInt64Max, EnumULong.Int64Max)]
				public async Task WhenSubjectIsContained_ShouldSucceed(EnumULong? subject,
					params EnumULong[] otherValues)
				{
					EnumULong?[] expected = [..otherValues, subject,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(EnumULong.UInt64Max, EnumULong.UInt64LessOne, EnumULong.Int64Max)]
				public async Task WhenSubjectIsDifferent_ShouldFail(EnumULong? subject,
					params EnumULong[] expected)
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
			}
		}
	}
}
