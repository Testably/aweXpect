import PackageInstall from '@site/src/components/PackageInstall';

# aweXpect.Web

[aweXpect.Web](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Web) contains expectations for `HttpRequestMessage`,
`HttpResponseMessage`, `Uri` and `IPAddress`, e.g. to verify the responses of an `HttpClient`.

## Installation

<PackageInstall package="aweXpect.Web" prerelease />

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well. The expectations live in the namespace `aweXpect`, so
`using aweXpect;` is all you need; only the [customization](./04-customization.md) lives in `aweXpect.Web`.

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

```text title="Failure message"
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

Expectations for [`HttpRequestMessage`](./01-http-messages.md#httprequestmessage):

| Expectation                                                    | Negated                  | Summary                               |
|----------------------------------------------------------------|--------------------------|---------------------------------------|
| [`HasMethod`](./01-http-messages.md#method)                    | `DoesNotHaveMethod`      | has the expected HTTP method          |
| [`HasRequestUri`](./01-http-messages.md#request-uri)           | `DoesNotHaveRequestUri`  | has the expected request URI          |
| [`HasHeader`](./01-http-messages.md#request-headers)           | `DoesNotHaveHeader`      | has a header, optionally with a value |
| [`HasContentType`](./01-http-messages.md#request-content-type) | `DoesNotHaveContentType` | has the expected media type           |
| [`HasContent`](./01-http-messages.md#request-content)          | `DoesNotHaveContent`     | has the expected string content       |

Expectations for [`HttpResponseMessage`](./01-http-messages.md#httpresponsemessage):

| Expectation                                                         | Negated                      | Summary                                         |
|---------------------------------------------------------------------|------------------------------|-------------------------------------------------|
| [`HasStatusCode`](./01-http-messages.md#status-code)                | `DoesNotHaveStatusCode`      | has the expected status code or status category |
| [`HasHeader`](./01-http-messages.md#response-headers)               | `DoesNotHaveHeader`          | has a header, optionally with a value           |
| [`HasContentType`](./01-http-messages.md#response-content-type)     | `DoesNotHaveContentType`     | has the expected media type                     |
| [`HasContent`](./01-http-messages.md#response-content)              | `DoesNotHaveContent`         | has the expected string content                 |
| [`HasProblemDetailsContent`](./01-http-messages.md#problem-details) | use `DoesNotHaveContentType` | has a `ProblemDetails` content                  |
| [`HasRequestMessage`](./01-http-messages.md#request-message)        |                              | the request message meets nested expectations   |

Expectations for [`Uri`](./02-uri.md):

| Expectation                                         | Negated                           | Summary                                        |
|-----------------------------------------------------|-----------------------------------|------------------------------------------------|
| [`IsAbsolute`](./02-uri.md#kind)                    | `IsNotAbsolute`                   | an absolute URI                                |
| [`IsFile`](./02-uri.md#kind)                        | `IsNotFile`                       | a file URI                                     |
| [`IsLoopback`](./02-uri.md#kind)                    | `IsNotLoopback`                   | references the local host                      |
| [`IsUnc`](./02-uri.md#kind)                         | `IsNotUnc`                        | a UNC path                                     |
| [`HasDefaultPort`](./02-uri.md#default-port)        | `DoesNotHaveDefaultPort`          | uses the default port of its scheme            |
| [`HasPort`](./02-uri.md#port)                       | use `HasPort().NotEqualTo(…)`     | has the expected port                          |
| [`HasScheme`](./02-uri.md#scheme)                   | use `HasScheme().NotEqualTo(…)`   | has the expected scheme                        |
| [`HasHost`](./02-uri.md#host)                       | use `HasHost().NotEqualTo(…)`     | has the expected host                          |
| [`HasPath`](./02-uri.md#path)                       | use `HasPath().NotEqualTo(…)`     | has the expected path                          |
| [`HasFragment`](./02-uri.md#fragment)               | use `HasFragment().NotEqualTo(…)` | has the expected fragment                      |
| [`IsBaseOf`](./02-uri.md#base-uri)                  | `IsNotBaseOf`                     | a base URI of the expected URI                 |
| [`HasQueryParameter`](./02-uri.md#query-parameters) | `DoesNotHaveQueryParameter`       | has a query parameter, optionally with a value |

Expectations for [`IPAddress`](./03-ip-address.md):

| Expectation                                | Negated         | Summary                                                    |
|--------------------------------------------|-----------------|------------------------------------------------------------|
| [`IsIPv4`](./03-ip-address.md#kind)        | `IsNotIPv4`     | an IPv4 address                                            |
| [`IsIPv6`](./03-ip-address.md#kind)        | `IsNotIPv6`     | an IPv6 address                                            |
| [`IsLoopback`](./03-ip-address.md#kind)    | `IsNotLoopback` | a loopback address                                         |
| [`IsInSubnet`](./03-ip-address.md#subnet)  | `IsNotInSubnet` | in the expected subnet                                     |
| [`IsEqualTo`](./03-ip-address.md#equality) | `IsNotEqualTo`  | equal to the expected address, optionally ignoring mapping |

## Customization

Set a value under `Customize.aweXpect.Web()` for the current
[async flow](https://docs.testably.org/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Web()` for
[all tests](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                           | Default                                                                    | Summary                                                          |
|-----------------------------------------------------------------|----------------------------------------------------------------------------|------------------------------------------------------------------|
| [`ContentProcessors`](./04-customization.md#content-processors) | `JsonContentProcessor`, `StringContentProcessor`, `BinaryContentProcessor` | format the content of requests and responses in failure messages |

## Documentation

- [HTTP requests and responses](./01-http-messages.md): method, request URI, headers, content type and content of an
  `HttpRequestMessage`, and status code, headers, content type, content, problem details and the request message of
  an `HttpResponseMessage`
- [URIs](./02-uri.md): kind, port, scheme, host, path, fragment, base URI and query parameters of a `Uri`
- [IP addresses](./03-ip-address.md): IPv4, IPv6, loopback, subnet membership and equality of an `IPAddress`
- [Customization](./04-customization.md): formatting the request and response content in failure messages
