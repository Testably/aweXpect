using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

			[Theory]
			[InlineData('a')]
			[InlineData('X')]
			[InlineData('5')]
			[InlineData('\t')]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail(char subject)
			{
				IEnumerable<char?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'A',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectDiffersOnlyInCase_AndNotIgnoringCase_ShouldFail()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'A',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase(false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was 'a'

					             Expected values:
					             ['X', 'A']
					             """);
			}

			[Theory]
			[InlineData('a')]
			[InlineData('B', 'b', 'A')]
			public async Task WhenSubjectIsContained_ShouldSucceed(char subject,
				params char[] otherValues)
			{
				IEnumerable<char> expected = [..otherValues, subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['X', 'B',];

				async Task Act()
					=> await That(subject).IsOneOf(expected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected ignoring case,
					             but it was 'a'

					             Expected values:
					             ['X', 'B']
					             """);
			}

			[Theory]
			[InlineData('B', 'b', 'A')]
			public async Task WhenSubjectIsDifferent_ShouldFail(char subject,
				params char[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
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
