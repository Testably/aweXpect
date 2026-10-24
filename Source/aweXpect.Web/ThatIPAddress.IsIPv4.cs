using System.Net;
using System.Net.Sockets;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	[GenerateExpectation("Is{Not}IPv4", "is {not} an IPv4 address",
		Remarks = """
		          An IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>, is an IPv6 address.
		          <seealso cref="AddressFamily.InterNetwork" />
		          """,
		NegatedRemarks = """
		                 An IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>, is an IPv6 address.
		                 <seealso cref="AddressFamily.InterNetwork" />
		                 """)]
	private static bool IsIPv4Address(IPAddress address)
		=> address.AddressFamily == AddressFamily.InterNetwork;
}
