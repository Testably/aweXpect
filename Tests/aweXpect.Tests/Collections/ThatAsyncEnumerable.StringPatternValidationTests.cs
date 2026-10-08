#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	/// <summary>
	///     An unusable pattern is rejected whichever items the subject has, also when no item is compared with it.
	/// </summary>
	public sealed class StringPatternValidationTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		public async Task AllAreEqualTo_AsPrefix_WhenExpectedIsNull_ShouldThrowArgumentNullException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).All().AreEqualTo(null).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task ContainsCollection_AsPrefix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).Contains(expected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task Contains_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).Contains("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task DoesNotContain_AsRegex_WhenUnexpectedIsNull_ShouldThrowArgumentNullException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).DoesNotContain((string?)null).AsRegex();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("b")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task DoesNotEndWith_AsSuffix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] unexpected = [null, "b",];

			async Task Act()
				=> await That(subject).DoesNotEndWith(unexpected).AsSuffix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' suffix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task DoesNotHaveItem_AsRegex_WhenUnexpectedIsInvalid_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).DoesNotHaveItem("[").AsRegex().AtIndex(1);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task DoesNotStartWith_AsPrefix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] unexpected = ["a", null,];

			async Task Act()
				=> await That(subject).DoesNotStartWith(unexpected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("b")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task EndsWith_AsSuffix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = [null, "b",];

			async Task Act()
				=> await That(subject).EndsWith(expected).AsSuffix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' suffix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task HasItem_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).HasItem("[").AsRegex().AtIndex(1);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		public async Task IsContainedIn_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["a", "[",];

			async Task Act()
				=> await That(subject).IsContainedIn(expected).AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		[Arguments("a,b")]
		[Arguments("a,b,c")]
		public async Task IsEqualTo_AsPrefix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		[Arguments("a,b")]
		[Arguments("a,b,c")]
		public async Task IsNotEqualTo_AsPrefix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] unexpected = ["a", null,];

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task StartsWith_AsPrefix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		/// <summary>
		///     Creates the subject from the comma-separated <paramref name="items" />, or no subject at all.
		/// </summary>
		private static IAsyncEnumerable<string?>? ToSubject(string? items)
			=> items is null
				? null
				: ToAsyncEnumerable<string?>(items.Split([',',], StringSplitOptions.RemoveEmptyEntries));
	}
}
#endif
