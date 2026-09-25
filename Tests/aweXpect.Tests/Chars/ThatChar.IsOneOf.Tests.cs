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
					.WithMessage("The expected cannot be null.").AsPrefix();
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
					              is one of [<null>],
					              but it was {Formatter.Format(subject)}
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
					.WithMessage("The expected cannot be null.").AsPrefix();
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
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
