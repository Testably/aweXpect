using System.Net;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	[GenerateExpectation("Is{Not}Loopback", "is {not} a loopback address",
		Remarks = """
		          An IPv4-mapped IPv6 address, e.g. <c>::ffff:127.0.0.1</c>, is mapped to its IPv4 address first,
		          so it is a loopback address if its IPv4 address is in <c>127.0.0.0/8</c>.
		          <seealso cref="IPAddress.IsLoopback(IPAddress)" />
		          """,
		NegatedRemarks = """
		                 An IPv4-mapped IPv6 address, e.g. <c>::ffff:127.0.0.1</c>, is mapped to its IPv4 address first,
		                 so it is a loopback address if its IPv4 address is in <c>127.0.0.0/8</c>.
		                 <seealso cref="IPAddress.IsLoopback(IPAddress)" />
		                 """)]
	private static bool IsLoopbackAddress(IPAddress address)
		// IPAddress.IsLoopback does not map the address itself: .NET Framework treats no mapped address as loopback
		// and .NET 8+ only ::ffff:127.0.0.1.
		=> IPAddress.IsLoopback(address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address);
}
