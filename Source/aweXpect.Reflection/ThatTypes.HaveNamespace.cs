using System;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("HaveNamespace", "has namespace {expected}", "have namespace {expected}",
		CollectionOverload = true, NegatedName = "DoNotHaveNamespace",
		Remarks = """
		          This compares the whole namespace as a string with the string options (e.g. <c>AsPrefix()</c> or
		          <c>IgnoringCase()</c>), unlike <c>AreWithinNamespace</c>, which includes the sub-namespaces.
		          """)]
	private static async ValueTask<PredicateResult> HasNamespace(Type? type,
		[NegatedParameterName("unexpected")] string expected, [ExpectationOptions] StringEqualityOptions options)
		=> type is null ? ItemResults.Null : await options.AreConsideredEqual(type.Namespace, expected, nameof(expected));
}
