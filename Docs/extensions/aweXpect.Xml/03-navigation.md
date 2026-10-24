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
XElement track = XElement.Parse("""<track title="Let It Be" artist="The Beatles" />""");

await Expect.That(track)
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

The expectations on the value are only verified when the attribute exists, otherwise the failure states that the
element did not have the attribute.

## Child elements

You can verify that an element has a child element, and add expectations on it:

```csharp
XElement album = XElement.Parse("""<album><artist><origin city="Liverpool" /></artist></album>""");

await Expect.That(album).HasElement("artist", artist => artist
    .HasElement("origin", origin => origin
        .HasAttribute("city", city => city.IsEqualTo("Liverpool"))));
await Expect.That(album).DoesNotHaveElement("bonusTrack");
```

Only the direct child elements are looked up, and the expectations are verified on the first child element with the
name, like by [`XElement.Element(name)`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.element).
To verify a whole subtree, combine it with [`Matches`](./02-matching.md):

```csharp
await Expect.That(album).HasElement("artist", artist => artist.Matches("""<artist><origin /></artist>"""));
```

To verify all child elements with a name, select them with
[`Whose`](https://docs.testably.org/aweXpect/how-it-works/combining#on-members-of-the-subject) and use the
[collection expectations](https://docs.testably.org/aweXpect/collections/items) of aweXpect, e.g. the
[quantifiers](https://docs.testably.org/aweXpect/collections/items#quantifiers) with `ComplyWith(…)`:

```csharp
XElement album = XElement.Parse(
    """
    <album title="Abbey Road">
      <track title="Come Together" />
      <track title="Something" />
      <artist name="The Beatles" />
    </album>
    """);

await Expect.That(album).Whose(a => a.Elements("track"), tracks => tracks.HasCount(2));
await Expect.That(album).Whose(a => a.Elements("track"), tracks => tracks
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
XDocument document = XDocument.Parse("""<album id="42"><artist /></album>""");

await Expect.That(document).HasAttribute("id");
await Expect.That(document).HasElement("artist", artist => artist.Matches("<artist />"));
await Expect.That(document).DoesNotHaveElement("bonusTrack");
```

`HasElement` looks up a child element of the root element, so `HasElement("album")` fails for this document, unlike
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
XNamespace music = "urn:music";
XElement album = XElement.Parse("""<album xmlns="urn:music"><track title="Come Together" /></album>""");

await Expect.That(album).HasElement(music + "track", track => track.HasAttribute("title"));
```

A name in a namespace is shown as `{urn:music}track` in the failure message.
