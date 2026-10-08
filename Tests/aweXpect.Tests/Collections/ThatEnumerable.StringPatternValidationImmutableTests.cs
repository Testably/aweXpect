#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	/// <summary>
	///     An unusable pattern is rejected whichever items the subject has, also when no item is compared with it.
	/// </summary>
	public sealed class StringPatternValidationImmutableTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		public async Task AllAreEqualTo_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).All().AreEqualTo("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task Contains_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).Contains("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("b")]
		[Arguments("a,b")]
		public async Task EndsWith_AsSuffix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
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
		public async Task HasItem_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).HasItem("[").AsRegex().AtIndex(1);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task IsEqualTo_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
			string[] expected = ["a", "[",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task StartsWith_AsPrefix_WhenExpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		/// <summary>
		///     Creates the subject from the comma-separated <paramref name="items" />, or a default array.
		/// </summary>
		private static ImmutableArray<string> ToSubject(string? items)
			=> items?.Split([',',], StringSplitOptions.RemoveEmptyEntries).ToImmutableArray() ?? default;
	}
}
#endif
