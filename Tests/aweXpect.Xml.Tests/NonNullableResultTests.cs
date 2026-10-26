using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed class NonNullableResultTests
{
	private const string Reason = "the awaited result is the verified subject, which cannot be null";

	[Test]
	public async Task StringIsValidXmlMatching_ShouldReturnNonNullableString()
	{
		string? subject = "<order id=\"1\"/>";

		string result = await That(subject).IsValidXmlMatching("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task StringIsValidXmlMatchingExactly_ShouldReturnNonNullableString()
	{
		string? subject = "<order/>";

		string result = await That(subject).IsValidXmlMatchingExactly("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentDoesNotHaveAttribute_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order/>");

		XDocument result = await That(subject).DoesNotHaveAttribute("id");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentDoesNotHaveElement_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order/>");

		XDocument result = await That(subject).DoesNotHaveElement("item");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasAttribute_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order id=\"1\"/>");

		XDocument result = await That(subject).HasAttribute("id");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasAttributeWithExpectations_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order id=\"1\"/>");

		XDocument result = await That(subject).HasAttribute("id", value => value.IsEqualTo("1"));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasAttributeWithOperator_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order id=\"1\"/>");

		XDocument result = await That(subject).HasAttribute("id").EqualTo("1");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasElement_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order><item/></order>");

		XDocument result = await That(subject).HasElement("item");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasElementWithExpectations_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order><item/></order>");

		XDocument result = await That(subject).HasElement("item", item => item.IsNotNull());

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasValue_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order>foo</order>");

		XDocument result = await That(subject).HasValue(value => value.IsEqualTo("foo"));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentHasValueWithOperator_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order>foo</order>");

		XDocument result = await That(subject).HasValue().EqualTo("foo");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentMatches_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order id=\"1\"/>");

		XDocument result = await That(subject).Matches("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XDocumentMatchesExactly_ShouldReturnNonNullableDocument()
	{
		XDocument? subject = XDocument.Parse("<order/>");

		XDocument result = await That(subject).MatchesExactly("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementDoesNotHaveAttribute_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order/>");

		XElement result = await That(subject).DoesNotHaveAttribute("id");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementDoesNotHaveElement_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order/>");

		XElement result = await That(subject).DoesNotHaveElement("item");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasAttribute_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order id=\"1\"/>");

		XElement result = await That(subject).HasAttribute("id");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasAttributeWithExpectations_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order id=\"1\"/>");

		XElement result = await That(subject).HasAttribute("id", value => value.IsEqualTo("1"));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasAttributeWithOperator_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order id=\"1\"/>");

		XElement result = await That(subject).HasAttribute("id").EqualTo("1");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasElement_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order><item/></order>");

		XElement result = await That(subject).HasElement("item");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasElementWithExpectations_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order><item/></order>");

		XElement result = await That(subject).HasElement("item", item => item.IsNotNull());

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasValue_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order>foo</order>");

		XElement result = await That(subject).HasValue(value => value.IsEqualTo("foo"));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementHasValueWithOperator_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order>foo</order>");

		XElement result = await That(subject).HasValue().EqualTo("foo");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementMatches_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order id=\"1\"/>");

		XElement result = await That(subject).Matches("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task XElementMatchesExactly_ShouldReturnNonNullableElement()
	{
		XElement? subject = XElement.Parse("<order/>");

		XElement result = await That(subject).MatchesExactly("<order/>");

		await That(result).IsSameAs(subject).Because(Reason);
	}
}
