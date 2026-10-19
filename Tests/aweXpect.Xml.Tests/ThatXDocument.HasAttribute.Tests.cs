using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXDocument
{
	public sealed class HasAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldFail()
			{
				XDocument document = XDocument.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(document).HasAttribute("artist");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has attribute "artist",
					             but it did not have attribute "artist"
					             """);
			}

			[Test]
			public async Task WhenAttributeExists_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<?xml version=\"1.0\"?><track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(document).HasAttribute("title");

				await That(Act).DoesNotThrow()
					.Because("the attribute is looked up on the root element");
			}

			[Test]
			public async Task WhenCombinedAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasAttribute("id").And.HasElement("item");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has attribute "id" and has element "item",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasAttribute("id");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has attribute "id",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenDocumentHasNoRootWithExpectations_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasAttribute("id", value => value.IsEqualTo("42"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has attribute "id" whose value is equal to "42",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("track"));

				async Task Act()
					=> await That(document).HasAttribute("title", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("track"));

				async Task Act()
					=> await That(document).HasAttribute(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenNegatedAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasAttribute("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have attribute "id",
					             but it had no root element
					             """)
					.Because("a document without a root element fails, also when the expectation is negated");
			}

			[Test]
			public async Task WhenNegatedAndValueMatches_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order id=\"42\"/>");

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasAttribute("id", value => value.IsEqualTo("42")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have attribute "id" whose value is equal to "42",
					             but it did
					             """);
			}

			[Test]
			public async Task WhenNegatedWithExpectationsAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasAttribute("id", value => value.IsEqualTo("42")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have attribute "id" whose value is equal to "42",
					             but it could not be verified, because the expectation did not decide its outcome
					             """)
					.Because("aweXpect.Core leaves a negated member expectation undecided when the subject fails both ways");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).HasAttribute("id");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has attribute "id",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotSatisfyTheExpectation_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order id=\"41\"/>");

				async Task Act()
					=> await That(document).HasAttribute("id", value => value.IsEqualTo("42"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has attribute "id" whose value is equal to "42",
					             but value of attribute "id" was "41", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Test]
			public async Task WhenValueSatisfiesTheExpectation_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<order id=\"42\"/>");

				async Task Act()
					=> await That(document).HasAttribute("id", value => value.IsEqualTo("42"));

				await That(Act).DoesNotThrow();
			}
		}
	}

	public sealed class DoesNotHaveAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(document).DoesNotHaveAttribute("artist");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAttributeExists_ShouldFail()
			{
				XDocument document = XDocument.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(document).DoesNotHaveAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have attribute "title",
					             but it did
					             """);
			}

			[Test]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotHaveAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have attribute "title",
					             but it had no root element
					             """)
					.Because("a document without a root element fails, also when the expectation is negated");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("track"));

				async Task Act()
					=> await That(document).DoesNotHaveAttribute(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have attribute "title",
					             but it was <null>
					             """);
			}
		}
	}
}
