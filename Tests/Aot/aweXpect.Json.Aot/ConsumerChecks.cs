using System.Text.Json;
using System.Threading.Tasks;
using aweXpect;
using aweXpect.Core;
using aweXpect.Json.Options;

namespace Consumer.Json;

/// <summary>
///     Calls the option methods from outside the namespaces of aweXpect, like a consumer does, so that they stay
///     reachable without a <c>using aweXpect.Json;</c>.
/// </summary>
internal static class ConsumerChecks
{
	public static async Task OptionsWithoutPackageNamespace()
	{
		using JsonDocument document = JsonDocument.Parse("[1, 2]");
		await Expect.That(document.RootElement).MatchesExactly("[2, 1]", o => o.IgnoringArrayOrder());
		OptionsHolder<JsonLookupOptions> holder = new(JsonLookupOptions.Default);
		await Expect.That(holder.TreatingNullAsAbsent().Value)
			.IsEqualTo(JsonLookupOptions.Default.TreatingNullAsAbsent());
	}
}
