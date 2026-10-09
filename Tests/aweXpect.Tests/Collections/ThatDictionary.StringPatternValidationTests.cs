using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	/// <summary>
	///     An unusable pattern is rejected whichever entries the subject has, also when no value is compared with it.
	/// </summary>
	/// <remarks>
	///     The results offer no <c>AsRegex()</c>, so the match type is switched through the options, as an extension
	///     would do.
	/// </remarks>
	public sealed class StringPatternValidationTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task Contains_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).Contains(2, "[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task ContainsValue_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).ContainsValue("[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task ContainsValues_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).ContainsValues("a", "[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContain_AsRegex_WhenUnexpectedIsInvalid_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContain(2, "[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContainValue_AsRegex_WhenUnexpectedIsInvalid_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContainValue("[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContainValues_AsRegex_WhenUnexpectedContainsAnInvalidPattern_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContainValues("a", "[").AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task IsEqualTo_AsRegex_WhenExpectedContainsAnInvalidPattern_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);
			Dictionary<int, string?> expected = new()
			{
				[1] = "a",
				[2] = "[",
			};

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task IsNotEqualTo_AsRegex_WhenUnexpectedContainsAnInvalidPattern_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);
			Dictionary<int, string?> unexpected = new()
			{
				[1] = "a",
				[2] = "[",
			};

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).AsRegexThroughOptions();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		/// <summary>
		///     Creates the subject from the comma-separated <paramref name="values" /> with their position as key, or
		///     no subject at all.
		/// </summary>
		private static IDictionary<int, string?>? ToSubject(string? values)
			=> values is null
				? null
				: ToDictionary<string?>(values.Split([',',], StringSplitOptions.RemoveEmptyEntries));
	}
}
