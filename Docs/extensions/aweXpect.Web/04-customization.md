---
description: Change how aweXpect.Web formats the content of requests and responses in failure messages.
---

# Customization

Set a value under `Customize.aweXpect.Web()` for the current
[async flow](/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Web()` for
[all tests](/aweXpect/how-it-works/configuration#global-defaults).

| Value                                      | Default                                                                    | Summary                                                          |
|--------------------------------------------|----------------------------------------------------------------------------|------------------------------------------------------------------|
| [`ContentProcessors`](#content-processors) | `JsonContentProcessor`, `StringContentProcessor`, `BinaryContentProcessor` | format the content of requests and responses in failure messages |

## Content processors

The failure messages include the content of the request and the response. Each content is formatted by the first
content processor that can handle it: by default JSON is indented, text is shown as is, and audio, image, video and
PDF content is summarized with its media type and length. You can change the processors, e.g. with your own
`IContentProcessor`:

```csharp
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

using (Customize.aweXpect.Web().ContentProcessors.Set([
           new MyCsvContentProcessor(),
           new JsonContentProcessor(),
           new StringContentProcessor(),
           new BinaryContentProcessor(),
       ]))
{
    // formats CSV content with the custom processor
}
```
