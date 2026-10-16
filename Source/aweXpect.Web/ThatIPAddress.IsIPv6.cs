using System.Net;
using System.Net.Sockets;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	/// <summary>
	///     Verifies that the subject is an IPv6 address.
	/// </summary>
	/// <remarks>
	///     An IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>, is an IPv6 address.
	///     <seealso cref="AddressFamily.InterNetworkV6" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsIPv6(this IThat<IPAddress?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsKindConstraint(it, grammars, IsIPv6Address, "an IPv6 address", "IPv6 addresses")),
			source);

	/// <summary>
	///     Verifies that the subject is not an IPv6 address.
	/// </summary>
	/// <remarks>
	///     An IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>, is an IPv6 address.
	///     <seealso cref="AddressFamily.InterNetworkV6" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsNotIPv6(this IThat<IPAddress?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsKindConstraint(it, grammars, IsIPv6Address, "an IPv6 address", "IPv6 addresses").Invert()),
			source);

	private static bool IsIPv6Address(IPAddress address)
		=> address.AddressFamily == AddressFamily.InterNetworkV6;
}
