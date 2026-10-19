# Attributes and child elements

## Attributes

You can verify that an [`XElement`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) has an
attribute, and add expectations on its value with all string expectations of aweXpect:

```csharp
XElement item = XElement.Parse("""<item sku="A1" price="1.00" />""");

await Expect.That(item)
    .HasAttribute("sku").And
    .HasAttribute("price", price => price.StartsWith("1.")).And
    .DoesNotHaveAttribute("discount");
```

If the price was "2.00" instead, the expectation would fail with:

```text title="Failure message"
Expected that item
has attribute "sku" and has attribute "price" whose value starts with "1." and does not have attribute "discount",
but value of attribute "price" was "2.00", which differs at index 0:
   ↓ (actual)
  "2.00"
  "1."
   ↑ (expected prefix)
```

The expectations on the value are only verified when the attribute exists, otherwise the failure states that the
element did not have the attribute.

## Child elements

You can verify that an element has a child element, and add expectations on it:

```csharp
XElement order = XElement.Parse("""<order><customer><address city="Liverpool" /></customer></order>""");

await Expect.That(order).HasElement("customer", customer => customer
    .HasElement("address", address => address
        .HasAttribute("city", city => city.IsEqualTo("Liverpool"))));
await Expect.That(order).DoesNotHaveElement("note");
```

Only the direct child elements are looked up, and the expectations are verified on the first child element with the
name, like by [`XElement.Element(name)`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.element).
To verify a whole subtree, combine it with [`Matches`](./02-matching.md):

```csharp
await Expect.That(order).HasElement("customer", customer => customer.Matches("""<customer><address /></customer>"""));
```

## Documents

All these expectations are also available for an
[`XDocument`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument) and apply to its root element:

```csharp
XDocument document = XDocument.Parse("""<order id="42"><customer /></order>""");

await Expect.That(document).HasAttribute("id");
await Expect.That(document).HasElement("customer", customer => customer.Matches("<customer />"));
await Expect.That(document).DoesNotHaveElement("note");
```

`HasElement` looks up a child element of the root element, so `HasElement("order")` fails for this document, unlike
[`XDocument.Element(name)`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.element), which
returns the root element itself. A document without a root element fails, also when the expectation is negated:

```text title="Failure message"
Expected that document
has attribute "id",
but it had no root element
```

## Names and namespaces

Attributes and child elements are looked up by their
[`XName`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xname), i.e. by their namespace and local name,
but not by their prefix. A child element inherits the default namespace of its parent, so it must be looked up in this
namespace:

```csharp
XNamespace shop = "urn:shop";
XElement order = XElement.Parse("""<order xmlns="urn:shop"><item sku="A1" /></order>""");

await Expect.That(order).HasElement(shop + "item", item => item.HasAttribute("sku"));
```

A name in a namespace is shown as `{urn:shop}item` in the failure message.
