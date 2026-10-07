using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				char subject = 'a';
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
				char subject = 'a';
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
			public async Task WhenExpectedOnlyContainsNull_ShouldFail(char subject)
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
				char subject = 'a';
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
				char subject = 'a';
				char?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenParamsValuesDoNotContainTheSubject_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsOneOf('b', 'c');

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of ['b', 'c'],
					             but it was 'a'
					             """);
			}

			[Test]
			public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'A',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectDiffersOnlyInCase_AndNotIgnoringCase_ShouldFail()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'A',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was 'a'

					             Expected values:
					             ['X', 'A']
					             """);
			}

			[Test]
			[Arguments('a')]
			[Arguments('B', 'b', 'A')]
			public async Task WhenSubjectIsContained_ShouldSucceed(char subject,
				params char[] otherValues)
			{
				IEnumerable<char> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'B',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected ignoring case,
					             but it was 'a'

					             Expected values:
					             ['X', 'B']
					             """);
			}

			[Test]
			[Arguments('B', 'b', 'A')]
			public async Task WhenSubjectIsDifferent_ShouldFail(char subject,
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
		}
	}
}
