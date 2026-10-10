# aweXpect.Web

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)
[![Build](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Web)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Web)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Web%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Web/main)

Expectations for `HttpRequestMessage`, `HttpResponseMessage` and `Uri` for
[aweXpect](https://github.com/Testably/aweXpect).

## Overview

Expectations for [`HttpRequestMessage`](#httprequestmessage):

| Expectation                      | Negated             | Summary                               |
|----------------------------------|---------------------|---------------------------------------|
| [`HasMethod`](#method)           |                     | has the expected HTTP method          |
| [`HasRequestUri`](#request-uri)  |                     | has the expected request URI          |
| [`HasHeader`](#request-headers)  | `DoesNotHaveHeader` | has a header, optionally with a value |
| [`HasContent`](#request-content) |                     | has the expected string content       |

Expectations for [`HttpResponseMessage`](#httpresponsemessage):

| Expectation                                    | Negated                       | Summary                                         |
|------------------------------------------------|-------------------------------|-------------------------------------------------|
| [`HasStatusCode`](#status-code)                | `HasStatusCode().DifferentTo` | has the expected status code or status category |
| [`HasHeader`](#response-headers)               | `DoesNotHaveHeader`           | has a header, optionally with a value           |
| [`HasContentType`](#content-type)              |                               | has the expected media type                     |
| [`HasContent`](#response-content)              |                               | has the expected string content                 |
| [`HasProblemDetailsContent`](#problem-details) |                               | has a `ProblemDetails` content                  |
| [`HasRequestMessage`](#request-message)        |                               | the request message meets nested expectations   |

Expectations for [`Uri`](#uri):

| Expectation                       | Negated                  | Summary                             |
|-----------------------------------|--------------------------|-------------------------------------|
| [`IsAbsolute`](#kind)             | `IsNotAbsolute`          | an absolute URI                     |
| [`IsFile`](#kind)                 | `IsNotFile`              | a file URI                          |
| [`IsLoopback`](#kind)             | `IsNotLoopback`          | references the local host           |
| [`IsUnc`](#kind)                  | `IsNotUnc`               | a UNC path                          |
| [`HasDefaultPort`](#default-port) | `DoesNotHaveDefaultPort` | uses the default port of its scheme |
| [`HasScheme`](#scheme)            | negated comparison       | has the expected scheme             |

When an expectation on a request or response fails, the failure message includes the formatted HTTP request and
response.

## `HttpRequestMessage`

### Method

You can verify that the `HttpRequestMessage` has the expected method:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");

await Expect.That(request).HasMethod(HttpMethod.Get);
```

### Request URI

You can verify that the `HttpRequestMessage` has the expected request URI:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");

await Expect.That(request).HasRequestUri("https://music.example.com/tracks/1");
await Expect.That(request).HasRequestUri(new Uri("https://music.example.com/tracks/1"));
```

### Request headers

You can verify that the `HttpRequestMessage` has a header, optionally with the expected value, or that it does not have
it:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");
request.Headers.Add("Accept", "application/json");

await Expect.That(request).HasHeader("Accept");
await Expect.That(request).HasHeader("Accept").WithValue("application/json");
await Expect.That(request).DoesNotHaveHeader("Authorization");
```

`WithValue` supports the same [string options](https://docs.testably.org/aweXpect/values/string#string-options) and
[match types](https://docs.testably.org/aweXpect/values/string#match-types) as comparing strings.

You can also add expectations on the header value or on all of its values:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");
request.Headers.Add("Accept-Encoding", ["gzip", "deflate"]);

await Expect.That(request).HasHeader("Accept-Encoding")
    .WhoseValues(values => values.Contains("gzip"));
```

The failure message shows the difference and the request:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");
request.Headers.Add("Cache-Control", "no-cache");

await Expect.That(request).HasHeader("Cache-Control").WithValue("max-age=0");
```

```text title="Failure message"
Expected that request
has a "Cache-Control" header whose value is equal to "max-age=0",
but it had header value "no-cache", which differs at index 0:
   ↓ (actual)
  "no-cache"
  "max-age=0"
   ↑ (expected)

HTTP-Request:
  GET https://music.example.com/tracks/1 HTTP/1.1
    Cache-Control: no-cache
```

### Request content

You can verify that the `HttpRequestMessage` has the expected string content:

```csharp
HttpRequestMessage request = new(HttpMethod.Post, "https://music.example.com/tracks")
{
    Content = new StringContent("{\"title\": \"Let It Be\"}"),
};

await Expect.That(request).HasContent("*Let It Be*").AsWildcard();
await Expect.That(request).HasContent(content => content.Contains("Let It Be"));
```

The content supports the same [string options](https://docs.testably.org/aweXpect/values/string#string-options) and
[match types](https://docs.testably.org/aweXpect/values/string#match-types) as comparing strings.

## `HttpResponseMessage`

### Status code

You can verify the status code of the `HttpResponseMessage`, either the exact value or its category:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasStatusCode(HttpStatusCode.OK);
await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.OK);
await Expect.That(response).HasStatusCode().DifferentTo(HttpStatusCode.NotFound);
await Expect.That(response).HasStatusCode().Success();
```

The categories are `Success()` (2xx), `Redirection()` (3xx), `ClientError()` (4xx), `ServerError()` (5xx) and
`Error()` (4xx or 5xx). Combine them with `.Or` to allow several:

```csharp
HttpResponseMessage response = await httpClient.PostAsync("https://music.example.com/tracks", new StringContent(""));

await Expect.That(response).HasStatusCode().ClientError().Or.HasStatusCode().ServerError();
```

A failing expectation against a test server

```csharp
HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

await Expect.That(response).HasStatusCode(HttpStatusCode.NotFound);
```

fails with the request and the response:

```text title="Failure message"
Expected that response
has status code 404 NotFound,
but it had status code 200 OK

HTTP-Request:
  GET http://localhost/tracks/1 HTTP/1.1

HTTP-Response:
  200 OK HTTP/1.1
    x-vendor: VENDOR
    Content-Type: application/json; charset=utf-8
    Content-Length: 51
  {
    "id": 1,
    "title": "Let It Be",
    "artist": "The Beatles"
  }
```

### Response headers

You can verify that the `HttpResponseMessage` has a header, optionally with the expected value, or that it does not
have it:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasHeader("Cache-Control");
await Expect.That(response).HasHeader("Cache-Control").WithValue("must-revalidate, max-age=0, private");
await Expect.That(response).DoesNotHaveHeader("Set-Cookie");
```

You can also add expectations on the header value or on all of its values:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasHeader("ETag")
    .WhoseValue(value => value.IsNotEmpty());
await Expect.That(response).HasHeader("Vary")
    .WhoseValues(values => values.Contains("Accept-Encoding"));
```

### Content type

You can verify the media type in the `Content-Type` header of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

await Expect.That(response).HasContentType("application/json");
await Expect.That(response).HasContentType("application/*").AsWildcard();
```

```csharp
HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

await Expect.That(response).HasContentType("text/plain");
```

```text title="Failure message"
Expected that response
has a `Content-Type` header equal to "text/plain",
but it had content type "application/json", which differs at index 0:
   ↓ (actual)
  "application/json"
  "text/plain"
   ↑ (expected)

HTTP-Request:
  GET http://localhost/tracks/1 HTTP/1.1

HTTP-Response:
  200 OK HTTP/1.1
    x-vendor: VENDOR
    Content-Type: application/json; charset=utf-8
    Content-Length: 51
  {
    "id": 1,
    "title": "Let It Be",
    "artist": "The Beatles"
  }
```

### Response content

You can verify that the `HttpResponseMessage` has the expected string content:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasContent("*Let It Be*").AsWildcard();
await Expect.That(response).HasContent(content => content.Contains("The Beatles"));
```

The content supports the same [string options](https://docs.testably.org/aweXpect/values/string#string-options) and
[match types](https://docs.testably.org/aweXpect/values/string#match-types) as comparing strings. With
[aweXpect.Json](https://github.com/Testably/aweXpect.Json), the nested expectations can also verify JSON content:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasContent(content => content
    .IsValidJsonMatching(new { title = "Let It Be", artist = "The Beatles" }));
```

### Problem details

You can verify that the content of the `HttpResponseMessage` is a valid
[`ProblemDetails`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails) object,
optionally with the expected type, title, status, detail and instance:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/42");

await Expect.That(response)
    .HasProblemDetailsContent("https://httpstatuses.com/404")
    .WithTitle("Track not found")
    .WithStatus(404)
    .WithInstance("/tracks/42");
```

The type, title, detail and instance support the same
[string options](https://docs.testably.org/aweXpect/values/string#string-options) and
[match types](https://docs.testably.org/aweXpect/values/string#match-types) as comparing strings.

### Request message

You can add expectations on the `HttpRequestMessage` that led to the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasRequestMessage(request => request
    .HasMethod(HttpMethod.Get).And
    .HasRequestUri("https://music.example.com/tracks/1"));
```

## `Uri`

### Kind

You can verify what kind of URI the `Uri` is, or that it is not:

```csharp
Uri uri = new("https://music.example.com/tracks/1");

await Expect.That(uri).IsAbsolute();
await Expect.That(uri).IsNotFile();
await Expect.That(uri).IsNotLoopback();
await Expect.That(uri).IsNotUnc();
```

```csharp
Uri uri = new("/tracks/1", UriKind.Relative);

await Expect.That(uri).IsAbsolute();
```

```text title="Failure message"
Expected that uri
is an absolute URI,
but it was /tracks/1
```

### Default port

You can verify that the `Uri` uses the default port of its scheme, or that it does not:

```csharp
await Expect.That(new Uri("https://music.example.com/tracks/1")).HasDefaultPort();
await Expect.That(new Uri("https://music.example.com:8443/tracks/1")).DoesNotHaveDefaultPort();
```

### Scheme

You can verify the scheme of the `Uri`:

```csharp
Uri uri = new("https://music.example.com/tracks/1");

await Expect.That(uri).HasScheme().EqualTo("https");
await Expect.That(uri).HasScheme().NotEqualTo("http");
```

## Customization

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
