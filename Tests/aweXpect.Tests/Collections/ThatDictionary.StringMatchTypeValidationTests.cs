using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	/// <summary>
	///     A custom match type rejects an unusable expected value whichever entries the subject has, also when no value
	///     is compared with it.
	/// </summary>
	public sealed class StringMatchTypeValidationTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task Contains_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).Contains(2, "foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task ContainsValue_AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).ContainsValue("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task ContainsValues_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).ContainsValues("1", "foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContain_AsNumber_WhenUnexpectedIsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContain(2, "foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContainValue_AsNumber_WhenUnexpectedIsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContainValue("foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task DoesNotContainValues_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);

			async Task Act()
				=> await That(subject).DoesNotContainValues("1", "foo").AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("1")]
		[Arguments("1,2")]
		[Arguments("1,2,3")]
		public async Task IsEqualTo_AsNumber_WhenExpectedContainsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);
			Dictionary<int, string?> expected = new()
			{
				[1] = "1",
				[2] = "foo",
			};

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
		[Arguments("1,2,3")]
		public async Task IsNotEqualTo_AsNumber_WhenUnexpectedContainsNoNumber_ShouldThrowArgumentException(string? values)
		{
			IDictionary<int, string?>? subject = ToSubject(values);
			Dictionary<int, string?> unexpected = new()
			{
				[1] = "1",
				[2] = "foo",
			};

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).AsNumber();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The value \"foo\" is no number.").AsPrefix();
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
