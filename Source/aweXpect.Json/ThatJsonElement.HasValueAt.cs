using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject has a value at the <paramref name="path" /> which satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> is a property name, followed by any number of <c>.name</c> or zero-based
	///     <c>[index]</c> segments, e.g. <c>"customer.address.city"</c> or <c>"items[0].sku"</c>, and is written without a
	///     leading <c>$</c>. Wildcards, filters and recursive descent are not supported, and property names that contain
	///     <c>.</c>, <c>[</c> or <c>]</c> can only be addressed with <c>HasProperty</c>.
	///     <para />
	///     Only the first segment that cannot be reached is reported, and the <paramref name="expectations" /> are only
	///     verified, when the value exists.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> HasValueAt(this IThat<JsonElement> subject,
		string path,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static IValueConstraint<JsonElement> (target, it, grammars) => target.CreateConstraint(it, grammars));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject, target);
	}
}
