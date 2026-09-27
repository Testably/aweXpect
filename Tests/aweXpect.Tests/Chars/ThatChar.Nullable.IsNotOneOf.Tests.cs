using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					char? subject = 'a';
					char[] expected = [];

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
					char? subject = 'a';
					char[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Theory]
				[InlineData('a')]
				[InlineData('X')]
				[InlineData('5')]
				[InlineData('\t')]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed(char subject)
				{
					IEnumerable<char?> expected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					char? subject = 'a';
					char?[] expected = [];

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
					char? subject = 'a';
					char?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldFail()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsNotOneOf('X', 'A').IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not one of ['X', 'A'] ignoring case,
						             but it was 'a'
						             """);
				}

				[Theory]
				[InlineData('a')]
				[InlineData('B', 'b', 'A')]
				public async Task WhenSubjectIsContained_ShouldFail(char subject,
					params char[] otherValues)
				{
					IEnumerable<char> expected = [..otherValues, subject,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of expected,
						              but it was {Formatter.Format(subject)}

						              Unexpected values:
						              {Formatter.Format(expected)}
						              """);
				}

				[Theory]
				[InlineData('B', 'b', 'A')]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(char subject,
					params char[] expected)
				{
					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndIgnoringCase_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotOneOf('X', 'A').IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					char? subject = null;
					IEnumerable<char?> expected = ['a', null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not one of expected,
						             but it was <null>

						             Unexpected values:
						             ['a', <null>]
						             """);
				}
			}
		}
	}
}
