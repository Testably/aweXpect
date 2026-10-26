using System.Xml.Linq;
using aweXpect.Core;

namespace aweXpect.Xml.Tests;

public sealed class NonNullableSubjectTests
{
	private const string Reason = "the expectations only run on a found node, whose value is never null";

	[Test]
	public async Task XDocumentHasAttribute_ShouldHandOutNonNullableValue()
	{
		XDocument subject = XDocument.Parse("<order id=\"1\"/>");

		async Task Act()
			=> await That(subject).HasAttribute("id", IsOne);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	[Test]
	public async Task XDocumentHasElement_ShouldHandOutNonNullableElement()
	{
		XDocument subject = XDocument.Parse("<order><item/></order>");

		async Task Act()
			=> await That(subject).HasElement("item", IsItem);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	[Test]
	public async Task XDocumentHasValue_ShouldHandOutNonNullableValue()
	{
		XDocument subject = XDocument.Parse("<order>1</order>");

		async Task Act()
			=> await That(subject).HasValue(IsOne);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	[Test]
	public async Task XElementHasAttribute_ShouldHandOutNonNullableValue()
	{
		XElement subject = XElement.Parse("<order id=\"1\"/>");

		async Task Act()
			=> await That(subject).HasAttribute("id", IsOne);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	[Test]
	public async Task XElementHasElement_ShouldHandOutNonNullableElement()
	{
		XElement subject = XElement.Parse("<order><item/></order>");

		async Task Act()
			=> await That(subject).HasElement("item", IsItem);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	[Test]
	public async Task XElementHasValue_ShouldHandOutNonNullableValue()
	{
		XElement subject = XElement.Parse("<order>1</order>");

		async Task Act()
			=> await That(subject).HasValue(IsOne);

		await That(Act).DoesNotThrow().Because(Reason);
	}

	private static void IsItem(IThatSubject<XElement> element)
		=> element.Satisfies(e => e.Name == "item");

	private static void IsOne(IThatSubject<string> value)
		=> value.IsEqualTo("1");
}
