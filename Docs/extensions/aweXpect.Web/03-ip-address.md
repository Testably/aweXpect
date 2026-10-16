# IP addresses

## Kind

You can verify what kind of address the `IPAddress` is, or that it is not:

```csharp
IPAddress address = IPAddress.Parse("10.0.0.1");

await Expect.That(address).IsIPv4();
await Expect.That(address).IsNotIPv6();
await Expect.That(address).IsNotLoopback();
```

The address family decides between IPv4 and IPv6, so an IPv4-mapped address like `::ffff:10.0.0.1` is an IPv6 address:

```csharp
IPAddress address = IPAddress.Parse("::ffff:10.0.0.1");

await Expect.That(address).IsIPv4();
```

```text title="Failure message"
Expected that address
is an IPv4 address,
but it was ::ffff:10.0.0.1
```
