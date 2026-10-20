using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXDocument
{
	public sealed class HasElement
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenChildElementDoesNotExist_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order><item/></order>");

				async Task Act()
					=> await That(document).HasElement("customer");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has element "customer",
					             but it did not have element "customer"
					             """);
			}

			[Test]
			public async Task WhenChildElementDoesNotSatisfyTheExpectations_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order><item sku=\"B2\"/></order>");

				async Task Act()
					=> await That(document).HasElement("item", item => item.HasAttribute("sku", value => value.IsEqualTo("A1")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has element "item" which has attribute "sku" whose value is equal to "A1",
					             but value of attribute "sku" was "B2", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Test]
			public async Task WhenChildElementExists_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<?xml version=\"1.0\"?><order><item/></order>");

				async Task Act()
					=> await That(document).HasElement("item");

				await That(Act).DoesNotThrow()
					.Because("the child element is looked up in the root element");
			}

			[Test]
			public async Task WhenChildElementSatisfiesTheExpectations_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<order><customer/></order>");

				async Task Act()
					=> await That(document).HasElement("customer", customer => customer.Matches("<customer />"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasElement("item");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has element "item",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenDocumentHasNoRootWithExpectations_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).HasElement("item", item => item.HasAttribute("sku"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has element "item" which has attribute "sku",
					             but it had no root element
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("order"));

				async Task Act()
					=> await That(document).HasElement("item", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("order"));

				async Task Act()
					=> await That(document).HasElement(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenNameIsTheRootName_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order/>");

				async Task Act()
					=> await That(document).HasElement("order");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             has element "order",
					             but it did not have element "order"
					             """)
					.Because("a child of the root element is looked up, not the root element itself like by XDocument.Element");
			}

			[Test]
			public async Task WhenNegatedAndChildElementSatisfiesTheExpectations_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order><item sku=\"A1\"/></order>");

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasElement("item", item => item.HasAttribute("sku")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have element "item" which has attribute "sku",
					             but it did
					             """);
			}

			[Test]
			public async Task WhenNegatedAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasElement("item"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have element "item",
					             but it had no root element
					             """)
					.Because("a document without a root element fails, also when the expectation is negated");
			}

			[Test]
			public async Task WhenNegatedWithExpectationsAndDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.HasElement("item", item => item.HasAttribute("sku")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have element "item" which has attribute "sku",
					             but it could not be verified, because the expectation did not decide its outcome
					             """)
					.Because("aweXpect.Core leaves a negated member expectation undecided when the subject fails both ways");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).HasElement("item");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element "item",
					             but it was <null>
					             """);
			}
		}
	}

	public sealed class DoesNotHaveElement
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenChildElementDoesNotExist_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<order><item/></order>");

				async Task Act()
					=> await That(document).DoesNotHaveElement("note");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenChildElementExists_ShouldFail()
			{
				XDocument document = XDocument.Parse("<order><note/></order>");

				async Task Act()
					=> await That(document).DoesNotHaveElement("note");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have element "note",
					             but it did
					             """);
			}

			[Test]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotHaveElement("note");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that document
					             does not have element "note",
					             but it had no root element
					             """)
					.Because("a document without a root element fails, also when the expectation is negated");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XDocument document = new(new XElement("order"));

				async Task Act()
					=> await That(document).DoesNotHaveElement(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenNameIsTheRootName_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<order/>");

				async Task Act()
					=> await That(document).DoesNotHaveElement("order");

				await That(Act).DoesNotThrow()
					.Because("a child of the root element is looked up, not the root element itself like by XDocument.Element");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveElement("note");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have element "note",
					             but it was <null>
					             """);
			}
		}
	}
}
