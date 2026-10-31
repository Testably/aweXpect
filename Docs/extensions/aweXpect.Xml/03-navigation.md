---
description: Verify the attributes, child elements and values of an XElement or XDocument.
---

# Attributes and child elements

Describes the possible expectations for the attributes, child elements and values of an `XElement` or an
[`XDocument`](#documents).

| Expectation                                    | Negated                | Summary                                                                                       |
|------------------------------------------------|------------------------|-----------------------------------------------------------------------------------------------|
| [`HasAttribute`](#attributes)                  | `DoesNotHaveAttribute` | has the attribute, optionally with expectations on its value                                  |
| [`HasAttribute(…).EqualTo(…)`, …](#attributes) |                        | has the attribute, whose value is compared with a string operator, e.g. `StartingWith`        |
| [`HasElement`](#child-elements)                | `DoesNotHaveElement`   | has the child element, optionally with expectations on it                                     |
| [`HasValue`](#values)                          |                        | the value satisfies the expectations                                                          |
| [`HasValue().EqualTo(…)`, …](#values)          |                        | the value is compared with a string operator, e.g. `EndingWith` or `Containing(…).AtLeast(2)` |

## Attributes

You can verify that an [`XElement`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) has an
attribute, and add expectations on its value with all string expectations of aweXpect:

```csharp
XElement track = XElement.Parse("""<track title="Let It Be" artist="The Beatles" />""");

await That(track)
    .HasAttribute("title").And
    .HasAttribute("artist", artist => artist.StartsWith("The")).And
    .DoesNotHaveAttribute("explicit");
```

If the artist was "Paul McCartney" instead, the expectation would fail with:

```text title="Failure message"
Expected that track
has attribute "title" and has attribute "artist" whose value starts with "The" and does not have attribute "explicit",
but value of attribute "artist" was "Paul McCartney", which differs at index 0:
   ↓ (actual)
  "Paul McCartney"
  "The"
   ↑ (expected prefix)
```

The value can also be compared directly on the result of `HasAttribute(name)` with the operators of a string
property, `EqualTo`, `StartingWith`, `EndingWith` and `Containing(…)` (each also with `Not…`), and the string options,
e.g. `IgnoringCase()` or `AsWildcard()`:

```csharp
await That(track)
    .HasAttribute("title").EqualTo("let it be").IgnoringCase().And
    .HasAttribute("artist").StartingWith("The");
```

If the title was "Yesterday" instead, the expectation would fail with:

```text title="Failure message"
Expected that track
has attribute "title" equal to "let it be" ignoring case and has attribute "artist" starting with "The",
but value of attribute "title" was "Yesterday", which differs at index 0:
   ↓ (actual)
  "Yesterday"
  "let it be"
   ↑ (expected)

Actual:
<track title="Yesterday" artist="The Beatles" />
```

The expectations on the value are only verified when the attribute exists, otherwise the failure states that the
element did not have the attribute, and a negated expectation, e.g. with `DoesNotComplyWith`, is met. A failed lookup, also a negated one, shows the complete element it ran on as
`Actual` context. Without a title, `HasAttribute("title")` would fail with:

```text title="Failure message"
Expected that track
has attribute "title",
but it did not have attribute "title"

Actual:
<track artist="The Beatles" />
```

## Child elements

You can verify that an element has a child element, and add expectations on it:

```csharp
XElement album = XElement.Parse("""<album><artist><origin city="Liverpool" /></artist></album>""");

await That(album).HasElement("artist", artist => artist
    .HasElement("origin", origin => origin
        .HasAttribute("city", city => city.IsEqualTo("Liverpool"))));
await That(album).DoesNotHaveElement("bonusTrack");
```

Only the direct child elements are looked up, and the expectations are verified on the first child element with the name, like by [`XElement.Element(name)`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.element). A failed lookup within the expectations on a child element shows this child element, labeled with it, e.g. `Actual (element "artist"):`, instead of the whole subject. To verify a whole subtree, combine it with [`Matches`](./02-matching.md):

```csharp
await That(album).HasElement("artist", artist => artist.Matches("""<artist><origin /></artist>"""));
```

To verify all child elements with a name, select them with
[`Whose`](/aweXpect/how-it-works/combining#on-members-of-the-subject) and use the
[collection expectations](/aweXpect/collections/items) of aweXpect, e.g. the
[quantifiers](/aweXpect/collections/items#quantifiers) with `ComplyWith(…)`:

```csharp
XElement album = XElement.Parse(
    """
    <album title="Abbey Road">
      <track title="Come Together" />
      <track title="Something" />
      <artist name="The Beatles" />
    </album>
    """);

await That(album).Whose(a => a.Elements("track"), tracks => tracks.HasCount(2));
await That(album).Whose(a => a.Elements("track"), tracks => tracks
    .All().ComplyWith(track => track.HasAttribute("title")));
```

With a third `<track title="Here Comes the Sun" />`, the first expectation would fail with the following message. As
`Elements(…)` is only enumerated as far as needed, it states a lower bound:

```text title="Failure message"
Expected that album
whose Elements("track") have exactly 2 items,
but Elements("track") had at least 3 items

Collection (Elements("track")):
[
  <track title="Come Together" />,
  <track title="Something" />,
  <track title="Here Comes the Sun" />,
  (… and maybe more)
]
```

## Values

You can add expectations on the value of an element with all string expectations of aweXpect:

```csharp
XElement track = XElement.Parse("""<track><title>Let It Be</title></track>""");

await That(track).HasElement("title", title => title.HasValue(value => value.StartsWith("Let")));
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

Like for attributes, the value can also be compared directly on the result of `HasValue()`:

```csharp
await That(track).HasElement("title", title => title.HasValue().Containing("e").AtLeast(3));
```

With the title "Let It Be", the expectation would fail with:

```text title="Failure message"
Expected that track
has element "title" which has a value containing "e" at least 3 times,
but value of the element contained "e" twice in "Let It Be"

Actual (element "title"):
<title>Let It Be</title>
```

The value is the concatenated text of all descendant text nodes, including CDATA sections, like
[`XElement.Value`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement.value). So the value of an
element with mixed content, e.g. `<p>a<b>x</b>c</p>`, is `axc`, and the value of an element without text is empty.

## Documents

All these expectations are also available for an
[`XDocument`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument) and apply to its root element:

```csharp
XDocument document = XDocument.Parse("""<album id="42"><artist /></album>""");

await That(document).HasAttribute("id");
await That(document).HasElement("artist", artist => artist.Matches("<artist />"));
await That(document).DoesNotHaveElement("bonusTrack");
```

`HasElement` looks up a child element of the root element, so `HasElement("album")` fails for this document, unlike
[`XDocument.Element(name)`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.element), which
returns the root element itself.

<details>
<summary>Documents without a root element</summary>

A document without a root element fails, also when the expectation is negated:

```text title="Failure message"
Expected that document
has attribute "id",
but it had no root element
```

</details>

## Names and namespaces

Attributes and child elements are looked up by their
[`XName`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xname), i.e. by their namespace and local name,
but not by their prefix. A child element inherits the default namespace of its parent, so it must be looked up in this
namespace:

```csharp
XNamespace music = "urn:music";
XElement album = XElement.Parse("""<album xmlns="urn:music"><track title="Come Together" /></album>""");

await That(album).HasElement(music + "track", track => track.HasAttribute("title"));
```

A name in a namespace is shown as `{urn:music}track` in the failure message.
