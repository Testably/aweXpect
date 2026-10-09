# aweXpect.Web

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)
[![Build](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Web/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Web)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Web&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Web)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Web%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Web/main)

Web extensions for [aweXpect](https://github.com/Testably/aweXpect).

## `HttpRequestMessage`

### Method

You can verify, the method of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "https://music.example.com/tracks/1");

await Expect.That(request).HasMethod(HttpMethod.Get);
```

### Request URI

You can verify, the request URI of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "https://music.example.com/tracks/1");

await Expect.That(request).HasRequestUri("https://music.example.com/tracks/1");
await Expect.That(request).HasRequestUri(new Uri("https://music.example.com/tracks/1"));
```

### Header

You can verify the headers of the `HttpRequestMessage`:

```csharp
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://music.example.com/tracks/1");
// Add headers

await Expect.That(request).HasHeader("X-Request-Id");
await Expect.That(request).HasHeader("Accept")
    .WithValue("application/json");

await Expect.That(request).DoesNotHaveHeader("X-My-Header");
```

You can also add additional expectations on the header value(s):

```csharp
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://music.example.com/tracks/1");
// Add headers

await Expect.That(request).HasHeader("X-Request-Id")
    .WhoseValue(value => value.IsNotEmpty());
await Expect.That(request).HasHeader("Accept-Encoding")
    .WhoseValues(values => values.Contains("gzip"));
```

### Content

You can verify, the content of the `HttpRequestMessage`:

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, "https://music.example.com/tracks")
{
	Content = new StringContent("{\"title\": \"Let It Be\"}")
};

await Expect.That(request).HasContent("*Let It Be*").AsWildcard();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

## `HttpResponseMessage`

### Status

You can verify the status code of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");
await Expect.That(response).HasStatusCode().Success();
await Expect.That(response).HasStatusCode(HttpStatusCode.OK);

response = await httpClient.PostAsync("https://music.example.com/tracks", new StringContent(""));
await Expect.That(response).HasStatusCode().ClientError().Or.HasStatusCode().ServerError().Or.HasStatusCode().Redirection();
```

### Header

You can verify the headers of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasHeader("X-Request-Id");
await Expect.That(response).HasHeader("Cache-Control")
    .WithValue("must-revalidate, max-age=0, private");

await Expect.That(response).DoesNotHaveHeader("X-My-Header");
```

You can also add additional expectations on the header value(s):

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasHeader("X-Request-Id")
    .WhoseValue(value => value.IsNotEmpty());
await Expect.That(response).HasHeader("Vary")
    .WhoseValues(values => values.Contains("Accept-Encoding"));
```

### Content

You can verify, the content of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/1");

await Expect.That(response).HasContent("*Let It Be*").AsWildcard();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

Great care was taken to provide as much information as possible, when a status verification failed.  
For example, the following expectation against a test server:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

await Expect.That(response).HasStatusCode(HttpStatusCode.NotFound);
```

fails with:
> ```
> Expected that response
> has status code 404 NotFound,
> but it had status code 200 OK
> 
> HTTP-Request:
>   GET http://localhost/tracks/1 HTTP/1.1
> 
> HTTP-Response:
>   200 OK HTTP/1.1
>     x-vendor: VENDOR
>     Content-Type: application/json; charset=utf-8
>     Content-Length: 51
>   {
>     "id": 1,
>     "title": "Let It Be",
>     "artist": "The Beatles"
>   }
> ```

#### Problem Details

You can verify that the content contains a
valid [ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails) object:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://music.example.com/tracks/42");

await Expect.That(response)
    .HasProblemDetailsContent("https://httpstatuses.com/404")
    .WithTitle("Track not found")
    .WithStatus(404)
    .WithInstance("/tracks/42");
```

For all string values you can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality). 
