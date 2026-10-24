using System.Net;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	[GenerateExpectation("Is{Not}Loopback", "is {not} a loopback address",
		Remarks = "<seealso cref=\"IPAddress.IsLoopback(IPAddress)\" />",
		NegatedRemarks = "<seealso cref=\"IPAddress.IsLoopback(IPAddress)\" />")]
	private static bool IsLoopbackAddress(IPAddress address)
		=> IPAddress.IsLoopback(address);
}
