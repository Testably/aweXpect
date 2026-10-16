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

## Subnet

You can verify that the `IPAddress` is in a subnet given in CIDR notation, or that it is not:

```csharp
IPAddress address = IPAddress.Parse("192.168.0.1");

await Expect.That(address).IsInSubnet("192.168.0.0/16");
await Expect.That(address).IsInSubnet("10.0.0.0/8");
```

```text title="Failure message"
Expected that address
is in subnet 10.0.0.0/8,
but it was 192.168.0.1
```

The subnet must be a valid CIDR notation without host bits set beyond the prefix, so `"10.0.0.1/8"` throws an
`ArgumentException`.

An IPv4 address is in an IPv6 subnet if its IPv4-mapped address is, and an IPv4-mapped address is in an IPv4 subnet if
its IPv4 address is:

```csharp
await Expect.That(IPAddress.Parse("::ffff:10.0.0.1")).IsInSubnet("10.0.0.0/8");
await Expect.That(IPAddress.Parse("10.0.0.1")).IsInSubnet("::ffff:0:0/96");
```
