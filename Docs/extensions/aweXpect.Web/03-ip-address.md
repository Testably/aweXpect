# IP addresses

Describes the possible expectations for `IPAddress`.

| Expectation              | Negated         | Summary                                                    |
|--------------------------|-----------------|------------------------------------------------------------|
| [`IsIPv4`](#kind)        | `IsNotIPv4`     | an IPv4 address                                            |
| [`IsIPv6`](#kind)        | `IsNotIPv6`     | an IPv6 address                                            |
| [`IsLoopback`](#kind)    | `IsNotLoopback` | a loopback address                                         |
| [`IsInSubnet`](#subnet)  | `IsNotInSubnet` | in the expected subnet                                     |
| [`IsEqualTo`](#equality) | `IsNotEqualTo`  | equal to the expected address, optionally ignoring mapping |

## Kind

You can verify what kind of address the `IPAddress` is, or that it is not:

```csharp
IPAddress address = IPAddress.Parse("10.0.0.1");

await Expect.That(address).IsIPv4();
await Expect.That(address).IsNotIPv6();
await Expect.That(address).IsNotLoopback();
```

The address family decides between IPv4 and IPv6. An IPv4-mapped address like `::ffff:10.0.0.1` is an IPv6 address, so

```csharp
IPAddress address = IPAddress.Parse("::ffff:10.0.0.1");

await Expect.That(address).IsIPv4();
```

fails with:

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
await Expect.That(address).IsNotInSubnet("10.0.0.0/8");
```

For example, `await Expect.That(address).IsInSubnet("10.0.0.0/8");` fails with:

```text title="Failure message"
Expected that address
is in subnet 10.0.0.0/8,
but it was 192.168.0.1
```

:::warning
The subnet must be a valid CIDR notation without host bits set beyond the prefix, so `"10.0.0.1/8"` throws an
`ArgumentException`.
:::

An IPv4 address is in an IPv6 subnet if its IPv4-mapped address is, and an IPv4-mapped address is in an IPv4 subnet if
its IPv4 address is:

```csharp
await Expect.That(IPAddress.Parse("::ffff:10.0.0.1")).IsInSubnet("10.0.0.0/8");
await Expect.That(IPAddress.Parse("10.0.0.1")).IsInSubnet("::ffff:0:0/96");
```

## Equality

You can verify that the `IPAddress` is equal to an expected address, or that it is not:

```csharp
IPAddress address = IPAddress.Parse("10.0.0.1");

await Expect.That(address).IsEqualTo(IPAddress.Parse("10.0.0.1"));
await Expect.That(address).IsNotEqualTo(IPAddress.Parse("10.0.0.2"));
```

The comparison uses `IPAddress.Equals`, so an IPv4-mapped address like `::ffff:10.0.0.1` is not equal to `10.0.0.1`.
With `IgnoringIPv4MappedToIPv6()`, IPv4-mapped addresses on either side are mapped to IPv4 before comparing:

```csharp
IPAddress address = IPAddress.Parse("10.0.0.1");

await Expect.That(address).IsEqualTo(IPAddress.Parse("::ffff:10.0.0.1")).IgnoringIPv4MappedToIPv6();
```

Different addresses are still not equal, so

```csharp
IPAddress address = IPAddress.Parse("::ffff:10.0.0.2");

await Expect.That(address).IsEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();
```

fails with:

```text title="Failure message"
Expected that address
is equal to 10.0.0.1 ignoring IPv4-to-IPv6 mapping,
but it was ::ffff:10.0.0.2
```

:::note
The other options for comparing objects, e.g. `Using(comparer)`, are also available, but only one of them can be
specified.
:::
