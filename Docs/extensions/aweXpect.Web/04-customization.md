# Customization

The failure messages include the content of the request and the response. Each content is formatted by the first
content processor that can handle it: by default JSON is indented, text is shown as is, and audio, image, video and
PDF content is summarized with its media type and length. You can change the processors, e.g. with your own
`IContentProcessor`. The value is restored when the returned lifetime is disposed:

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

To change the processors for all tests, set them on `Customize.aweXpect.Global.Web()` instead.
