#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	/// <summary>
	///     A custom match type rejects an unusable expected value whichever items the subject has, also when no item is
	///     compared with it, and an option it cannot honour at the call that specifies it.
	/// </summary>
	public sealed class StringMatchTypeValidationTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		public async Task AllAreEqualTo_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

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
			IAsyncEnumerable<string?>? subject = ToSubject(items);

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
		public async Task ContainsCollection_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).Contains(expected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		public async Task Contains_AsNumber_WhenACustomComparerIsUsedAfterwards_ShouldThrowInvalidOperationException()
		{
			IAsyncEnumerable<string?>? subject = ToAsyncEnumerable<string?>([]);

			async Task Act()
				=> await That(subject).Contains("1").AsNumber().Using(StringComparer.OrdinalIgnoreCase);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for AsNumber.");
		}

		[Test]
		public async Task Contains_AsNumber_WhenACustomComparerIsUsedBefore_ShouldThrowInvalidOperationException()
		{
			IAsyncEnumerable<string?>? subject = ToAsyncEnumerable<string?>([]);

			async Task Act()
				=> await That(subject).Contains("1").Using(StringComparer.OrdinalIgnoreCase).AsNumber();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for AsNumber.");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task Contains_AsNumber_WhenCaseIsExplicitlyNotIgnored_ShouldCompareAsNumber(bool matchTypeFirst)
		{
			IAsyncEnumerable<string?>? subject = ToAsyncEnumerable<string?>(["01",]);

			async Task Act()
			{
				if (matchTypeFirst)
				{
					await That(subject).Contains("1").AsNumber().IgnoringCase(false);
				}
				else
				{
					await That(subject).Contains("1").IgnoringCase(false).AsNumber();
				}
			}

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_AsNumber_WhenCaseIsIgnoredAfterwards_ShouldThrowInvalidOperationException()
		{
			IAsyncEnumerable<string?>? subject = ToAsyncEnumerable<string?>([]);

			async Task Act()
				=> await That(subject).Contains("1").AsNumber().IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringCase cannot be combined with AsNumber.");
		}

		[Test]
		public async Task Contains_AsNumber_WhenCaseIsIgnoredBefore_ShouldThrowInvalidOperationException()
		{
			IAsyncEnumerable<string?>? subject = ToAsyncEnumerable<string?>([]);

			async Task Act()
				=> await That(subject).Contains("1").IgnoringCase().AsNumber();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringCase cannot be combined with AsNumber.");
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		public async Task DoesNotContain_AsNumber_WhenUnexpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).DoesNotContain("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task DoesNotContainCollection_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["1", "foo",];

			async Task Act()
				=> await That(subject).DoesNotContain(unexpected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task DoesNotEndWith_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["1", "foo",];

			async Task Act()
				=> await That(subject).DoesNotEndWith(unexpected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task DoesNotHaveItem_AsNumber_WhenUnexpectedIsNoNumber_ShouldThrowArgumentException(string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).DoesNotHaveItem("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task DoesNotStartWith_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["1", "foo",];

			async Task Act()
				=> await That(subject).DoesNotStartWith(unexpected).AsNumber();

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
			IAsyncEnumerable<string?>? subject = ToSubject(items);
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
			IAsyncEnumerable<string?>? subject = ToSubject(items);

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
		public async Task IsContainedIn_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).IsContainedIn(expected).AsNumber();

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
			IAsyncEnumerable<string?>? subject = ToSubject(items);
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
		public async Task IsEqualTo_InAnyOrder_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsNumber().InAnyOrder();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task IsNotContainedIn_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["1", "foo",];

			async Task Act()
				=> await That(subject).IsNotContainedIn(unexpected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		public async Task IsNotEqualTo_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(
			string? items)
		{
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["1", "foo",];

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).AsNumber();

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
			IAsyncEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["1", "foo",];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
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
