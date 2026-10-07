using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
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
			IEnumerable<string?>? subject = ToSubject(items);

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
			IEnumerable<string?>? subject = ToSubject(items);
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
			IEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).Contains("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task DoesNotContainCollection_AsPrefix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string?[] unexpected = ["a", null,];

			async Task Act()
				=> await That(subject).DoesNotContain(unexpected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task DoesNotContain_AsRegex_WhenUnexpectedIsNull_ShouldThrowArgumentNullException(string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);

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
			IEnumerable<string?>? subject = ToSubject(items);
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
			IEnumerable<string?>? subject = ToSubject(items);

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
			IEnumerable<string?>? subject = ToSubject(items);
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
			IEnumerable<string?>? subject = ToSubject(items);
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
			IEnumerable<string?>? subject = ToSubject(items);

			async Task Act()
				=> await That(subject).HasItem("[").AsRegex().AtIndex(1);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null, 0)]
		[Arguments("", 0)]
		[Arguments("a", 0)]
		[Arguments("x", 0)]
		[Arguments("", 1)]
		[Arguments("a", 1)]
		[Arguments("x", 1)]
		[Arguments("", 2)]
		[Arguments("a", 2)]
		[Arguments("x", 2)]
		public async Task IsContainedIn_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
			string? items, int order)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["a", "[",];

			async Task Act()
			{
				switch (order)
				{
					case 1:
						await That(subject).IsContainedIn(expected).AsRegex().IgnoringInterspersedItems();
						break;
					case 2:
						await That(subject).IsContainedIn(expected).AsRegex().InAnyOrder();
						break;
					default:
						await That(subject).IsContainedIn(expected).AsRegex();
						break;
				}
			}

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
			IEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsPrefix();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
		}

		[Test]
		[Arguments("")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task IsEqualTo_AsRegex_WhenExpectedIsALazySequenceWithAnInvalidPattern_ShouldThrowArgumentException(
			string items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			IEnumerable<string> expected = new[] { "a", "[", }.Select(pattern => pattern);

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix()
				.Because("a sequence that is not a collection is validated when it is enumerated");
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		[Arguments("a,b")]
		public async Task IsEqualTo_InAnyOrder_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
			string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["a", "[",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsRegex().InAnyOrder();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("x")]
		public async Task IsNotContainedIn_AsRegex_WhenUnexpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
			string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string[] unexpected = ["a", "[",];

			async Task Act()
				=> await That(subject).IsNotContainedIn(unexpected).AsRegex();

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
		[Arguments("a,b,c")]
		public async Task IsNotEqualTo_AsPrefix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
			string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
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
			IEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsPrefix();

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
		public async Task StartsWith_AsRegex_WhenExpectedContainsAnEmptyPattern_ShouldThrowArgumentException(
			string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string[] expected = ["a", "",];

			async Task Act()
				=> await That(subject).StartsWith(expected).AsRegex();

			await That(Act).ThrowsExactly<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("a")]
		[Arguments("a,b")]
		public async Task WhenExpectedContainsNullWithoutAPattern_ShouldNotThrowAnArgumentException(string? items)
		{
			IEnumerable<string?>? subject = ToSubject(items);
			string?[] expected = ["a", null,];

			async Task Act()
				=> await That(subject).StartsWith(expected).IgnoringCase().And.IsNotEqualTo(expected).IgnoringCase();

			await That(Act).Throws<FailException>()
				.Because("null is a legal expected value as long as it is compared as a value");
		}

		/// <summary>
		///     Creates the subject from the comma-separated <paramref name="items" />, or no subject at all.
		/// </summary>
		private static string[]? ToSubject(string? items)
			=> items?.Split([',',], StringSplitOptions.RemoveEmptyEntries);
	}
}
