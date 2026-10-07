using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	/// <summary>
	///     An unusable pattern is rejected whichever subject it is matched against, also when the subject is
	///     <see langword="null" />.
	/// </summary>
	public sealed class PatternValidationTests
	{
		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task Contains_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? subject)
		{
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
		public async Task Contains_IgnoringIndentation_WhenExpectedIsOnlyWhiteSpace_ShouldThrowArgumentException(
			string? subject)
		{
			async Task Act()
				=> await That(subject).Contains("  ").IgnoringIndentation();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' string cannot be empty.").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task DoesNotContain_AsRegex_WhenUnexpectedIsInvalid_ShouldThrowArgumentException(string? subject)
		{
			async Task Act()
				=> await That(subject).DoesNotContain("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task IsEqualTo_AsRegex_WhenExpectedIsInvalid_ShouldThrowArgumentException(string? subject)
		{
			async Task Act()
				=> await That(subject).IsEqualTo("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("x")]
		public async Task IsNotEqualTo_AsRegex_WhenUnexpectedIsInvalid_ShouldThrowArgumentException(string? subject)
		{
			async Task Act()
				=> await That(subject).IsNotEqualTo("[").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix();
		}

		[Test]
		public async Task IsNotOneOf_AsBlock_WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
		{
			string? subject = null;
			IEnumerable<string?> unexpected = ["foo", null,];

			async Task Act()
				=> await That(subject).IsNotOneOf(unexpected).AsBlock();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not one of unexpected as block,
				             but it was <null>

				             Unexpected values:
				             ["foo", <null>]
				             """)
				.Because("a null has no content to inspect, in both directions");
		}

		[Test]
		public async Task IsOneOf_AsBlock_WhenSubjectIsNullAndExpectedContainsNull_ShouldFail()
		{
			string? subject = null;
			IEnumerable<string?> expected = ["foo", null,];

			async Task Act()
				=> await That(subject).IsOneOf(expected).AsBlock();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is one of expected as block,
				             but it was <null>

				             Expected values:
				             ["foo", <null>]
				             """)
				.Because("a null has no content to inspect, in both directions");
		}
	}
}
