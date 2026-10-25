using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXDocument
{
	public sealed class HasValue
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasValue(value => value.IsEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has a value which is equal to "Let It Be",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("title"));

				async Task Act()
					=> await That(document).HasValue(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasValue(value => value.IsEqualTo("Let It Be")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have a value which is equal to "Let It Be",
					             but it had no root element
					             """)
					.Because("the expectations on the value are not applied when the document has no root element");
			}

			[Test]
			public async Task WhenNegatedAndValueMatches_ShouldFail()
			{
				XDocument document = XDocument.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasValue(value => value.IsEqualTo("Let It Be")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have a value which is equal to "Let It Be",
					             but it did

					             Actual:
					             <title>Let It Be</title>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).HasValue(value => value.IsEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a value which is equal to "Let It Be",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotSatisfyTheExpectation_ShouldFail()
			{
				XDocument document = XDocument.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(document).HasValue(value => value.IsEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has a value which is equal to "Let It Be",
					             but value of the root element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenValueSatisfiesTheExpectation_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<?xml version=\"1.0\"?><track><title>Let It Be</title><artist>The Beatles</artist></track>");

				async Task Act()
					=> await That(document).HasValue(value => value.IsEqualTo("Let It BeThe Beatles"));

				await That(Act).DoesNotThrow()
					.Because("the value is taken from the root element, including the text of its descendants");
			}
		}
	}
}
