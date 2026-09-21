using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AsPrefix_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsOneOf(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)} as prefix,
					              but it was <null>
					              """)
					.Because("a null has no content to inspect, just as for StartsWith");
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string?[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string> expected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar", "baz")]
			public async Task WhenValueIsDifferentToAllExpected_ShouldFail(
				string? subject, params string?[] expected)
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
			public async Task WhenValueIsEqualToAnyExpectedExceptForTheIndentation_ShouldSucceed()
			{
				string subject = "  foo\n    bar";

				async Task Act()
					=> await That(subject).IsOneOf("baz", "foo\nbar").IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar", "foo", "baz")]
			public async Task WhenValueIsEqualToAnyExpected_ShouldSucceed(
				string? subject, params string?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenValueIsNull_ShouldFail(
				params string?[] expected)
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}
		}
	}
}
