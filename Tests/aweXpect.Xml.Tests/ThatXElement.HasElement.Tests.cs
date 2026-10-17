using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXElement
{
	public sealed class HasElement
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenChildElementExists_ShouldSucceed()
			{
				XElement element = XElement.Parse("<order><item/></order>");

				async Task Act()
					=> await That(element).HasElement("item");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOnlyDescendantExists_ShouldFail()
			{
				XElement element = XElement.Parse("<order><items><item/></items></order>");

				async Task Act()
					=> await That(element).HasElement("item");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "item",
					             but it did not have element "item"
					             """)
					.Because("only the child elements are looked up, like by XElement.Element");
			}

			[Fact]
			public async Task WhenChildElementIsInTheSpecifiedNamespace_ShouldSucceed()
			{
				XElement element = XElement.Parse("<s:order xmlns:s=\"urn:shop\"><s:item/></s:order>");
				XNamespace shop = "urn:shop";

				async Task Act()
					=> await That(element).HasElement(shop + "item");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenChildElementIsInAnotherNamespace_ShouldFail()
			{
				XElement element = XElement.Parse("<order xmlns=\"urn:shop\"><item/></order>");

				async Task Act()
					=> await That(element).HasElement("item");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "item",
					             but it did not have element "item"
					             """)
					.Because("the child element inherits the default namespace, so it is not in no namespace");
			}

			[Fact]
			public async Task WhenChildElementSatisfiesTheExpectations_ShouldSucceed()
			{
				XElement element = XElement.Parse("<order><item sku=\"A1\" price=\"1.00\"/></order>");

				async Task Act()
					=> await That(element).HasElement("item", item => item.HasAttribute("sku", value => value.IsEqualTo("A1")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenChildElementDoesNotSatisfyTheExpectations_ShouldFail()
			{
				XElement element = XElement.Parse("<order><item sku=\"B2\"/></order>");

				async Task Act()
					=> await That(element).HasElement("item", item => item.HasAttribute("sku", value => value.IsEqualTo("A1")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "item" which has attribute "sku" whose value is equal to "A1",
					             but value of attribute "sku" was "B2", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Fact]
			public async Task WhenSeveralChildElementsExist_ShouldVerifyTheFirst()
			{
				XElement element = XElement.Parse("<order><item sku=\"A1\"/><item sku=\"B2\"/></order>");

				async Task Act()
					=> await That(element).HasElement("item", item => item.Matches("<item sku=\"B2\"/>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "item" which matches <item sku="B2"/>,
					             but element "item" differed as /item/@sku was "A1" instead of "B2"
					             """)
					.Because("the first child element with the name is verified, like by XElement.Element");
			}

			[Fact]
			public async Task WhenChildElementDoesNotExist_ShouldNotVerifyIt()
			{
				XElement element = XElement.Parse("<order/>");

				async Task Act()
					=> await That(element).HasElement("item", item => item.HasAttribute("sku"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "item" which has attribute "sku",
					             but it did not have element "item"
					             """);
			}

			[Fact]
			public async Task WhenNested_ShouldVerifyEachLevel()
			{
				XElement element = XElement.Parse("<order><customer><address city=\"Liverpool\"/></customer></order>");

				async Task Act()
					=> await That(element).HasElement("customer", customer => customer
						.HasElement("address", address => address
							.HasAttribute("city", city => city.IsEqualTo("London"))));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has element "customer" which has element "address" which has attribute "city" whose value is equal to "London",
					             but value of attribute "city" was "Liverpool", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Fact]
			public async Task WhenNegatedAndChildElementSatisfiesTheExpectations_ShouldFail()
			{
				XElement element = XElement.Parse("<order><item sku=\"A1\"/></order>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasElement("item", item => item.HasAttribute("sku")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not have element "item" which has attribute "sku",
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("order");

				async Task Act()
					=> await That(element).HasElement("item", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasElement("item");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has element "item",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("order");

				async Task Act()
					=> await That(element).HasElement(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}
		}
	}

	public sealed class DoesNotHaveElement
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenChildElementDoesNotExist_ShouldSucceed()
			{
				XElement element = XElement.Parse("<order><item/></order>");

				async Task Act()
					=> await That(element).DoesNotHaveElement("note");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenChildElementExists_ShouldFail()
			{
				XElement element = XElement.Parse("<order><note/></order>");

				async Task Act()
					=> await That(element).DoesNotHaveElement("note");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not have element "note",
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("order");

				async Task Act()
					=> await That(element).DoesNotHaveElement(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}
		}
	}
}
