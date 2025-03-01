# aweXpect.Web

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)
[![Build](https://github.com/aweXpect/aweXpect.Web/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Web/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Web)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Web&metric=coverage)](https://sonarcloud.io/summary/overall?id=aweXpect_aweXpect.Web)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FaweXpect%2FaweXpect.Web%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/aweXpect/aweXpect.Web/main)

Web extensions for [aweXpect](https://github.com/aweXpect/aweXpect).

## `HttpRequestMessage`

### Method

You can verify, the method of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/aweXpect/aweXpect.Web");

await Expect.That(request).HasMethod(HttpMethod.Get);
```

### Request URI

You can verify, the request URI of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/aweXpect/aweXpect.Web");

await Expect.That(request).HasRequestUri("https://github.com/aweXpect/aweXpect.Web");
await Expect.That(request).HasRequestUri(new Uri("https://github.com/aweXpect/aweXpect.Web"));
```

### Header

You can verify the headers of the `HttpRequestMessage`:

```csharp
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/aweXpect/aweXpect.Web");
// Add headers

await Expect.That(request).HasHeader("X-GitHub-Request-Id");
await Expect.That(request).HasHeader("Cache-Control")
    .WithValue("must-revalidate, max-age=0, private");

await Expect.That(request).DoesNotHaveHeader("X-My-Header");
```

You can also add additional expectations on the header value(s):

```csharp
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/aweXpect/aweXpect.Web");
// Add headers

await Expect.That(request).HasHeader("X-GitHub-Request-Id")
    .WhoseValue(value => value.IsNotEmpty());
await Expect.That(request).HasHeader("Vary")
    .WhoseValues(values => values.Contains("Turbo-Frame"));
```

### Content

You can verify, the content of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/aweXpect/aweXpect.Web")
{
	Content = new StringContent("my aweXpect content")
};

await Expect.That(request).HasContent("*aweXpect*").AsWildcard();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

## `HttpResponseMessage`

### Status

You can verify the status code of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect.Web");
await Expect.That(response).HasStatusCode().Success();
await Expect.That(response).HasStatusCode(HttpStatusCode.OK);

response = await httpClient.PostAsync("https://github.com/aweXpect/aweXpect.Web", new StringContent(""));
await Expect.That(response).HasStatusCode().ClientError().Or.HasStatusCode().ServerError().Or.HasStatusCode().Redirection();
```

### Header

You can verify the headers of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect.Web");

await Expect.That(response).HasHeader("X-GitHub-Request-Id");
await Expect.That(response).HasHeader("Cache-Control")
    .WithValue("must-revalidate, max-age=0, private");

await Expect.That(response).DoesNotHaveHeader("X-My-Header");
```

You can also add additional expectations on the header value(s):

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect.Web");

await Expect.That(response).HasHeader("X-GitHub-Request-Id")
    .WhoseValue(value => value.IsNotEmpty());
await Expect.That(response).HasHeader("Vary")
    .WhoseValues(values => values.Contains("Turbo-Frame"));
```

### Content

You can verify, the content of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect");

await Expect.That(response).HasContent("*aweXpect*").AsWildcard();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

Great care was taken to provide as much information as possible, when a status verification failed.  
The response could look similar to:
> ```
> Expected that response
> has success status code (2xx),
> but it was 404 NotFound
> 
> HTTP-Request:
>   GET https://github.com/aweXpect/missing-repo HTTP/1.1
> 
> HTTP-Response:
>   404 NotFound HTTP/1.1
>     Server: GitHub.com
>     Date: Fri, 29 Nov 2024 07:55:47 GMT
>     Cache-Control: no-cache
>     Referrer-Policy: origin-when-cross-origin, strict-origin-when-cross-origin
>     X-GitHub-Request-Id: DB30:24038B:287F716:29D98BD:67497384
>   Content is binary
> ```

#### Problem Details

You can verify that the content contains a
valid [ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails) object:

```csharp
HttpResponseMessage response = // a call that returns a problem details object

await Expect.That(response)
    .HasProblemDetailsContent("https://httpstatuses.com/404")
    .WithTitle("Error: Not Found")
    .WithStatus(404)
    .WithInstance("93c8f977-7ff7-46ed-900f-7b6264624a31");
```

For all string values you can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality). 
