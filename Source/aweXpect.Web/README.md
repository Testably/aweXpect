# aweXpect.Web

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)
[![Build](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Web)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Web)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Web%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Web/main)

Expectations for `HttpRequestMessage`, `HttpResponseMessage`, `Uri` and `IPAddress` for
[aweXpect](https://github.com/Testably/aweXpect).

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

## Overview

Expectations for [`HttpRequestMessage`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#httprequestmessage):

| Expectation                                                    | Negated                 | Summary                               |
|----------------------------------------------------------------|-------------------------|---------------------------------------|
| [`HasMethod`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#method)           | `DoesNotHaveMethod`     | has the expected HTTP method          |
| [`HasRequestUri`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#request-uri)  | `DoesNotHaveRequestUri` | has the expected request URI          |
| [`HasHeader`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#request-headers)  | `DoesNotHaveHeader`     | has a header, optionally with a value |
| [`HasContent`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#request-content) | `DoesNotHaveContent`    | has the expected string content       |

Expectations for [`HttpResponseMessage`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#httpresponsemessage):

| Expectation                                                                  | Negated                       | Summary                                         |
|------------------------------------------------------------------------------|-------------------------------|-------------------------------------------------|
| [`HasStatusCode`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#status-code)                | `HasStatusCode().DifferentTo` | has the expected status code or status category |
| [`HasHeader`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#response-headers)               | `DoesNotHaveHeader`           | has a header, optionally with a value           |
| [`HasContentType`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#content-type)              | `DoesNotHaveContentType`      | has the expected media type                     |
| [`HasContent`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#response-content)              | `DoesNotHaveContent`          | has the expected string content                 |
| [`HasProblemDetailsContent`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#problem-details) | `DoesNotComplyWith`           | has a `ProblemDetails` content                  |
| [`HasRequestMessage`](../../Docs/extensions/aweXpect.Web/01-http-messages.md#request-message)        |                               | the request message meets nested expectations   |

Expectations for [`Uri`](../../Docs/extensions/aweXpect.Web/02-uri.md):

| Expectation                                                  | Negated                     | Summary                                        |
|--------------------------------------------------------------|-----------------------------|------------------------------------------------|
| [`IsAbsolute`](../../Docs/extensions/aweXpect.Web/02-uri.md#kind)                    | `IsNotAbsolute`             | an absolute URI                                |
| [`IsFile`](../../Docs/extensions/aweXpect.Web/02-uri.md#kind)                        | `IsNotFile`                 | a file URI                                     |
| [`IsLoopback`](../../Docs/extensions/aweXpect.Web/02-uri.md#kind)                    | `IsNotLoopback`             | references the local host                      |
| [`IsUnc`](../../Docs/extensions/aweXpect.Web/02-uri.md#kind)                         | `IsNotUnc`                  | a UNC path                                     |
| [`HasDefaultPort`](../../Docs/extensions/aweXpect.Web/02-uri.md#default-port)        | `DoesNotHaveDefaultPort`    | uses the default port of its scheme            |
| [`HasPort`](../../Docs/extensions/aweXpect.Web/02-uri.md#port)                       | negated comparison          | has the expected port                          |
| [`HasScheme`](../../Docs/extensions/aweXpect.Web/02-uri.md#scheme)                   | negated comparison          | has the expected scheme                        |
| [`HasHost`](../../Docs/extensions/aweXpect.Web/02-uri.md#host)                       | negated comparison          | has the expected host                          |
| [`HasPath`](../../Docs/extensions/aweXpect.Web/02-uri.md#path)                       | negated comparison          | has the expected path                          |
| [`HasFragment`](../../Docs/extensions/aweXpect.Web/02-uri.md#fragment)               | negated comparison          | has the expected fragment                      |
| [`IsBaseOf`](../../Docs/extensions/aweXpect.Web/02-uri.md#base-uri)                  | `IsNotBaseOf`               | a base URI of the expected URI                 |
| [`HasQueryParameter`](../../Docs/extensions/aweXpect.Web/02-uri.md#query-parameters) | `DoesNotHaveQueryParameter` | has a query parameter, optionally with a value |

Expectations for [`IPAddress`](../../Docs/extensions/aweXpect.Web/03-ip-address.md):

| Expectation                                         | Negated         | Summary                                                    |
|-----------------------------------------------------|-----------------|------------------------------------------------------------|
| [`IsIPv4`](../../Docs/extensions/aweXpect.Web/03-ip-address.md#kind)        | `IsNotIPv4`     | an IPv4 address                                            |
| [`IsIPv6`](../../Docs/extensions/aweXpect.Web/03-ip-address.md#kind)        | `IsNotIPv6`     | an IPv6 address                                            |
| [`IsLoopback`](../../Docs/extensions/aweXpect.Web/03-ip-address.md#kind)    | `IsNotLoopback` | a loopback address                                         |
| [`IsInSubnet`](../../Docs/extensions/aweXpect.Web/03-ip-address.md#subnet)  | `IsNotInSubnet` | in the expected subnet                                     |
| [`IsEqualTo`](../../Docs/extensions/aweXpect.Web/03-ip-address.md#equality) | `IsNotEqualTo`  | equal to the expected address, optionally ignoring mapping |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Web/):

- [HTTP requests and responses](../../Docs/extensions/aweXpect.Web/01-http-messages.md): method, request URI, headers and content of an
  `HttpRequestMessage`, and status code, headers, content type, content, problem details and the request message of
  an `HttpResponseMessage`
- [URIs](../../Docs/extensions/aweXpect.Web/02-uri.md): kind, port, scheme, host, path, fragment, base URI and query parameters of a `Uri`
- [IP addresses](../../Docs/extensions/aweXpect.Web/03-ip-address.md): IPv4, IPv6, loopback, subnet membership and equality of an `IPAddress`
- [Customization](../../Docs/extensions/aweXpect.Web/04-customization.md): formatting the request and response content in failure messages
