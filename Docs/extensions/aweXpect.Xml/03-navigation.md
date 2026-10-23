# Attributes and child elements

Describes the possible expectations for the attributes, child elements and values of an `XElement` or an
[`XDocument`](#documents).

| Expectation                     | Negated                | Summary                                                      |
|---------------------------------|------------------------|--------------------------------------------------------------|
| [`HasAttribute`](#attributes)   | `DoesNotHaveAttribute` | has the attribute, optionally with expectations on its value |
| [`HasElement`](#child-elements) | `DoesNotHaveElement`   | has the child element, optionally with expectations on it    |
| [`HasValue`](#values)           |                        | the value satisfies the expectations                         |

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

To verify all child elements with a name, select them with
[`Whose`](https://docs.testably.org/aweXpect/how-it-works/combining#on-members-of-the-subject) and use the
[collection expectations](https://docs.testably.org/aweXpect/collections/items) of aweXpect, e.g. the
[quantifiers](https://docs.testably.org/aweXpect/collections/items#quantifiers) with `ComplyWith(…)`:

```csharp
XElement order = XElement.Parse("""<order><item sku="A1" /><item sku="B2" /><note /></order>""");

await Expect.That(order).Whose(o => o.Elements("item"), items => items.HasCount(2));
await Expect.That(order).Whose(o => o.Elements("item"), items => items
    .All().ComplyWith(item => item.HasAttribute("sku")));
```

With a third `<item sku="C3" />`, the first expectation would fail with the following message. As `Elements(…)` is
only enumerated as far as needed, it states a lower bound:

```text title="Failure message"
Expected that order
whose Elements("item") have exactly 2 items,
but Elements("item") had at least 3 items

Collection (Elements("item")):
[
  <item sku="A1" />,
  <item sku="B2" />,
  <item sku="C3" />,
  (… and maybe more)
]
```

## Values

You can add expectations on the value of an element with all string expectations of aweXpect:

```csharp
XElement track = XElement.Parse("""<track><title>Let It Be</title></track>""");

await Expect.That(track).HasElement("title", title => title.HasValue(value => value.StartsWith("Let")));
```

If the title was "Yesterday" instead, the expectation would fail with:

```text title="Failure message"
Expected that track
has element "title" which has a value which starts with "Let",
but value of the element was "Yesterday", which differs at index 0:
   ↓ (actual)
  "Yesterday"
  "Let"
   ↑ (expected prefix)
```

The value is the concatenated text of all descendant text nodes, including CDATA sections, like
[`XElement.Value`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement.value). So the value of an
element with mixed content, e.g. `<p>a<b>x</b>c</p>`, is `axc`, and the value of an element without text is empty.

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
