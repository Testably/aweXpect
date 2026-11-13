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
PDF content is summarized with its media type and length. You can change the processors, e.g. add your own
`IContentProcessor` in front of the current ones, so that you keep the defaults, also those added in a later version:

```csharp
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

using (Customize.aweXpect.Web().ContentProcessors.Set([
           new MyCsvContentProcessor(),
           ..Customize.aweXpect.Web().ContentProcessors.Get(),
       ]))
{
    // formats CSV content with the custom processor
}
```

A processor receives every content that an earlier processor did not handle, of any media type and also content it cannot read. Return `false` for content the processor does not handle, so that the next processor is tried. Disposed content, e.g. a request content that `HttpClient` on .NET Framework disposes after sending it, is not passed to any processor. When a processor throws, the failure message shows the exception in place of the content and keeps the rest of the request or response.
