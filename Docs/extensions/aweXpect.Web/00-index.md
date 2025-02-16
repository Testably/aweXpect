# [aweXpect.Web](https://github.com/aweXpect/aweXpect.Web) [![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)

Expectations for `HttpClient`.


## `HttpResponseMessage`

### Content

You can verify, the content of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect");

await Expect.That(response).HasContent("*aweXpect*").AsWildcard();
```

You can use the same configuration options as when [comparing strings](/docs/expectations/string#equality).

### Status code

You can verify, that the status code of the `HttpResponseMessage`:

```csharp
HttpResponseMessage response = await httpClient.GetAsync("https://github.com/aweXpect/aweXpect");
await Expect.That(response).HasStatusCode().Success();
await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.OK);

response = await httpClient.PostAsync("https://github.com/aweXpect/aweXpect", new StringContent(""));
await Expect.That(response).HasStatusCode().ClientError().Or.HasStatusCode().ServerError().Or.HasStatusCode().Redirection();
```

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

