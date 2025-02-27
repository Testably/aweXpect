# [aweXpect.Web](https://github.com/aweXpect/aweXpect.Web) [![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)

Expectations for `HttpClient`.

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

You can use the same configuration options as when [comparing strings](/docs/expectations/string#equality).

Great care was taken to provide as much information as possible, when a status verification failed.  
The response could look similar to:
> ```
> Expected that response
> has success status code (2xx),
> but it was 404 NotFound
> 
> HTTP-Request:
>   HTTP/1.1 404 NotFound
>     Server: GitHub.com
>     Date: Fri, 29 Nov 2024 07:55:47 GMT
>     Cache-Control: no-cache
>     Referrer-Policy: origin-when-cross-origin, strict-origin-when-cross-origin
>     X-GitHub-Request-Id: DB30:24038B:287F716:29D98BD:67497384
>   Content is binary
>   The originating request was:
>     GET https://github.com/aweXpect/missing-repo HTTP 1.1
> ```

#### Problem Details

You can verify that the content contains a valid [ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails) object:

```csharp
HttpResponseMessage response = // a call that returns a problem details object

await Expect.That(response)
    .HasProblemDetails("https://httpstatuses.com/404")
    .WithTitle("Error: Not Found")
    .WithStatus(404)
    .WithInstance("93c8f977-7ff7-46ed-900f-7b6264624a31");
```

For all string values you can use the same configuration options as when [comparing strings](https://awexpect.com/docs/expectations/string#equality). 
