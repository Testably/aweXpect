using System.Threading.Tasks;
using System.Xml.Linq;
using aweXpect;

namespace Consumer.Xml;

/// <summary>
///     Calls the option methods from outside the namespaces of aweXpect, like a consumer does, so that they stay
///     reachable without a <c>using aweXpect.Xml;</c>.
/// </summary>
internal static class ConsumerChecks
{
	public static async Task OptionsWithoutPackageNamespace()
		=> await Expect.That(XElement.Parse("<a><c/><b/></a>"))
			.MatchesExactly("<a><b/><c/></a>", o => o.IgnoringElementOrder());
}
