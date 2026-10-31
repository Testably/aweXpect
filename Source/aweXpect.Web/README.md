# aweXpect.Web

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)

[aweXpect.Web](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Web) contains expectations for `HttpRequestMessage`,
`HttpResponseMessage`, `Uri` and `IPAddress`, e.g. to verify the responses of an `HttpClient`.

## Installation

```shell
dotnet add package aweXpect.Web --prerelease
```

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well. The expectations live in the namespace `aweXpect`, so
`using aweXpect;` is all you need; only the
[customization](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/customization) lives in `aweXpect.Web`.

## At a glance

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

// verify the status code, either the exact value or its category
await Expect.That(response).HasStatusCode().Success();

// verify the media type and the headers of the response
await Expect.That(response).HasContentType("application/json");
await Expect.That(response).HasHeader("Cache-Control").WithValue("must-revalidate, max-age=0, private");

// verify the content of the response
await Expect.That(response).HasContent("*Let It Be*").AsWildcard();
```

A failing expectation on a request or response includes the formatted HTTP request and response. If the test server
returned the track, `HasStatusCode(HttpStatusCode.NotFound)` would fail with:

```text
Expected that response
has status code equal to 404 NotFound,
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

## Overview

Expectations for [`HttpRequestMessage`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#httprequestmessage):

| Expectation                                                                                                   | Negated                  | Summary                               |
|---------------------------------------------------------------------------------------------------------------|--------------------------|---------------------------------------|
| [`HasMethod`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#method)                | `DoesNotHaveMethod`      | has the expected HTTP method          |
| [`HasRequestUri`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#request-uri)       | `DoesNotHaveRequestUri`  | has the expected request URI          |
| [`HasHeader`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#request-headers)       | `DoesNotHaveHeader`      | has a header, optionally with a value |
| [`HasContentType`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#request-content-type) | `DoesNotHaveContentType` | has the expected media type       |
| [`HasContent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#request-content)      | `DoesNotHaveContent`     | has the expected string content       |

Expectations for [`HttpResponseMessage`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#httpresponsemessage):

| Expectation                                                                                                        | Negated                      | Summary                                         |
|--------------------------------------------------------------------------------------------------------------------|------------------------------|-------------------------------------------------|
| [`HasStatusCode`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#status-code)            | `DoesNotHaveStatusCode`      | has the expected status code or status category |
| [`HasHeader`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#response-headers)           | `DoesNotHaveHeader`          | has a header, optionally with a value           |
| [`HasContentType`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#response-content-type) | `DoesNotHaveContentType`     | has the expected media type                     |
| [`HasContent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#response-content)          | `DoesNotHaveContent`         | has the expected string content                 |
| [`HasProblemDetailsContent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#problem-details) | use `DoesNotHaveContentType` | has a `ProblemDetails` content              |
| [`HasRequestMessage`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages#request-message)    |                              | the request message meets nested expectations   |

Expectations for [`Uri`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri):

| Expectation                                                                                        | Negated                           | Summary                                        |
|----------------------------------------------------------------------------------------------------|-----------------------------------|------------------------------------------------|
| [`IsAbsolute`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#kind)                | `IsNotAbsolute`                   | an absolute URI                                |
| [`IsFile`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#kind)                    | `IsNotFile`                       | a file URI                                     |
| [`IsLoopback`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#kind)                | `IsNotLoopback`                   | references the local host                      |
| [`IsUnc`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#kind)                     | `IsNotUnc`                        | a UNC path                                     |
| [`HasDefaultPort`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#default-port)    | `DoesNotHaveDefaultPort`          | uses the default port of its scheme            |
| [`HasPort`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#port)                   | use `HasPort().NotEqualTo(…)`     | has the expected port                          |
| [`HasScheme`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#scheme)               | use `HasScheme().NotEqualTo(…)`   | has the expected scheme                        |
| [`HasHost`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#host)                   | use `HasHost().NotEqualTo(…)`     | has the expected host                          |
| [`HasPath`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#path)                   | use `HasPath().NotEqualTo(…)`     | has the expected path                          |
| [`HasFragment`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#fragment)           | use `HasFragment().NotEqualTo(…)` | has the expected fragment                      |
| [`IsBaseOf`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#base-uri)              | `IsNotBaseOf`                     | a base URI of the expected URI                 |
| [`HasQueryParameter`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri#query-parameters) | `DoesNotHaveQueryParameter`   | has a query parameter, optionally with a value |

Expectations for [`IPAddress`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address):

| Expectation                                                                               | Negated         | Summary                                                    |
|-------------------------------------------------------------------------------------------|-----------------|------------------------------------------------------------|
| [`IsIPv4`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address#kind)        | `IsNotIPv4`     | an IPv4 address                                            |
| [`IsIPv6`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address#kind)        | `IsNotIPv6`     | an IPv6 address                                            |
| [`IsLoopback`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address#kind)    | `IsNotLoopback` | a loopback address                                         |
| [`IsInSubnet`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address#subnet)  | `IsNotInSubnet` | in the expected subnet                                     |
| [`IsEqualTo`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address#equality) | `IsNotEqualTo`  | equal to the expected address, optionally ignoring mapping |

## Customization

Set a value under `Customize.aweXpect.Web()` for the current
[async flow](https://docs.testably.org/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Web()` for
[all tests](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                                                                          | Default                                                                    | Summary                                                          |
|----------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------|------------------------------------------------------------------|
| [`ContentProcessors`](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/customization#content-processors) | `JsonContentProcessor`, `StringContentProcessor`, `BinaryContentProcessor` | format the content of requests and responses in failure messages |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/):

- [HTTP requests and responses](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/http-messages): method,
  request URI, headers, content type and content of an `HttpRequestMessage`, and status code, headers, content type,
  content, problem details and the request message of an `HttpResponseMessage`
- [URIs](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/uri): kind, port, scheme, host, path, fragment,
  base URI and query parameters of a `Uri`
- [IP addresses](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/ip-address): IPv4, IPv6, loopback, subnet
  membership and equality of an `IPAddress`
- [Customization](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/customization): formatting the request
  and response content in failure messages
