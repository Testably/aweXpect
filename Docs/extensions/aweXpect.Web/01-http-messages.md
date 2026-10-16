# HTTP requests and responses

Describes the possible expectations for `HttpRequestMessage` and `HttpResponseMessage`.

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

When the header has multiple values, `WithValues` verifies that it has exactly these values in the same order:

```csharp
HttpRequestMessage request = new(HttpMethod.Get, "https://music.example.com/tracks/1");
request.Headers.Add("Accept", ["application/json", "text/plain"]);

await Expect.That(request).HasHeader("Accept").WithValues("application/json", "text/plain");
```

`WithValues` supports the same [string options](https://docs.testably.org/aweXpect/values/string#string-options),
[match types](https://docs.testably.org/aweXpect/values/string#match-types) and
[collection options](https://docs.testably.org/aweXpect/collections/equality#equality) as comparing string collections,
e.g. `.WithValues("text/plain", "application/json").InAnyOrder().IgnoringCase()`.

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

When the header has multiple values, `WithValues` verifies that it has exactly these values in the same order:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasHeader("Vary").WithValues("Accept", "Accept-Encoding");
```

`WithValues` supports the same [string options](https://docs.testably.org/aweXpect/values/string#string-options),
[match types](https://docs.testably.org/aweXpect/values/string#match-types) and
[collection options](https://docs.testably.org/aweXpect/collections/equality#equality) as comparing string collections,
e.g. `.WithValues("text/plain", "application/json").InAnyOrder().IgnoringCase()`.

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

For example, expecting plain text from an endpoint that returns JSON

```csharp
HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

await Expect.That(response).HasContentType("text/plain");
```

fails with the difference, the request and the response:

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
