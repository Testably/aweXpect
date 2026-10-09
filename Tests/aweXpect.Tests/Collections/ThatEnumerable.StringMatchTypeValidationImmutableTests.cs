#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	/// <summary>
	///     A custom match type rejects an unusable expected value whichever items the subject has, also when no item is
	///     compared with it.
	/// </summary>
	public sealed class StringMatchTypeValidationImmutableTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		public async Task AllAreEqualTo_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).All().AreEqualTo("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		public async Task Contains_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).Contains("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task EndsWith_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).EndsWith(expected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task HasItem_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);

			async Task Act()
				=> await That(subject).HasItem("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task IsEqualTo_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task StartsWith_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			ImmutableArray<string> subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		/// <summary>
		///     Creates the subject from the comma-separated <paramref name="items" />, or a default array.
		/// </summary>
		private static ImmutableArray<string> ToSubject(string? items)
			=> items?.Split([',',], StringSplitOptions.RemoveEmptyEntries).ToImmutableArray() ?? default;
	}
}
#endif
