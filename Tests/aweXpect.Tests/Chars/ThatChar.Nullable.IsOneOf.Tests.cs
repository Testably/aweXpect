using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					char? subject = 'a';
					char[] expected = [];

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
					char? subject = 'a';
					char[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				[Arguments('a')]
				[Arguments('X')]
				[Arguments('5')]
				[Arguments('\t')]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail(char? subject)
				{
					IEnumerable<char?> expected = [null,];

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
					char? subject = 'a';
					char?[] expected = [];

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
					char? subject = 'a';
					char?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsOneOf('X', 'A').IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments('a')]
				[Arguments('B', 'b', 'A')]
				public async Task WhenSubjectIsContained_ShouldSucceed(char? subject,
					params char[] otherValues)
				{
					char?[] expected = [..otherValues, subject,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsOneOf('X', 'B').IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is one of ['X', 'B'] ignoring case,
						             but it was 'a'
						             """);
				}

				[Test]
				[Arguments('B', 'b', 'A')]
				public async Task WhenSubjectIsDifferent_ShouldFail(char? subject,
					params char[] expected)
				{
					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was {Formatter.Format(subject)}

						              Expected values:
						              ['b', 'A']
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					char? subject = null;
					IEnumerable<char?> expected = ['a', null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
