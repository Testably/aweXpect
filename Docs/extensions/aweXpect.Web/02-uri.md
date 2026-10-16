# URIs

## Kind

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

## Default port

You can verify that the `Uri` uses the default port of its scheme, or that it does not:

```csharp
await Expect.That(new Uri("https://music.example.com/tracks/1")).HasDefaultPort();
await Expect.That(new Uri("https://music.example.com:8443/tracks/1")).DoesNotHaveDefaultPort();
```

## Port

You can verify the port of the `Uri`. Without an explicit port, it is the default port of the scheme:

```csharp
Uri uri = new("https://localhost:5001/health");

await Expect.That(uri).HasPort().EqualTo(5001);
await Expect.That(uri).HasPort().Between(5000).And(5999);
await Expect.That(new Uri("http://localhost:4000/health")).HasPort(4000);
```

## Scheme

You can verify the scheme of the `Uri`:

```csharp
Uri uri = new("https://music.example.com/tracks/1");

await Expect.That(uri).HasScheme().EqualTo("https");
await Expect.That(uri).HasScheme().NotEqualTo("http");
```

## Host

You can verify the host of the `Uri`:

```csharp
Uri uri = new("https://music.example.com/tracks/1");

await Expect.That(uri).HasHost().EqualTo("music.example.com");
await Expect.That(uri).HasHost().EndingWith(".example.com");
```

## Path

You can verify the URL-decoded path of the `Uri`:

```csharp
Uri uri = new("https://music.example.com/tracks/1");

await Expect.That(uri).HasPath().EqualTo("/tracks/1");
await Expect.That(uri).HasPath().StartingWith("/tracks/");
await Expect.That(new Uri("https://music.example.com/my%20song")).HasPath().EqualTo("/my song");
```

## Fragment

You can verify the URL-decoded fragment of the `Uri`, without the leading `#`:

```csharp
Uri uri = new("https://music.example.com/tracks/1#lyrics");

await Expect.That(uri).HasFragment().EqualTo("lyrics");
await Expect.That(uri).HasFragment().NotEqualTo("credits");
```

## Base URI

You can verify that the `Uri` is a base URI of another absolute URI, or that it is not:

```csharp
Uri uri = new("https://music.example.com/api/");

await Expect.That(uri).IsBaseOf("https://music.example.com/api/tracks/1");
await Expect.That(uri).IsNotBaseOf(new Uri("https://other.example.com/api/tracks/1"));
```

Like `Uri.IsBaseOf`, only the part of the `Uri` up to its last `/` is considered, so `https://music.example.com/api`
is also a base URI of `https://music.example.com/tracks/1`.

## Query parameters

You can verify that the `Uri` has a query parameter, optionally with the expected value, or that it does not have it:

```csharp
Uri uri = new("https://music.example.com/tracks?page=2&genre=Jazz");

await Expect.That(uri).HasQueryParameter("page");
await Expect.That(uri).HasQueryParameter("page").WithValue("2");
await Expect.That(uri).HasQueryParameter("genre").WithValue("jazz").IgnoringCase();
await Expect.That(uri).DoesNotHaveQueryParameter("token");
```

Names and values are URL-decoded, and names are compared case-sensitively. `WithValue` supports the same
[string options](https://docs.testably.org/aweXpect/values/string#string-options) and
[match types](https://docs.testably.org/aweXpect/values/string#match-types) as comparing strings and requires the query
parameter to occur exactly once. A query parameter that occurs several times can be verified with `WithValues`, which
compares the values in the order in which they appear in the URI, unless `InAnyOrder()` is specified. It supports the
same options as comparing [string collections](https://docs.testably.org/aweXpect/collections/equality#equality).

You can also add expectations on the value or on all values of the query parameter:

```csharp
Uri uri = new("https://music.example.com/tracks?tag=live&tag=remastered");

await Expect.That(uri).HasQueryParameter("tag").WithValues("live", "remastered");
await Expect.That(uri).HasQueryParameter("tag")
    .WhoseValues(values => values.Contains("live"));
```
