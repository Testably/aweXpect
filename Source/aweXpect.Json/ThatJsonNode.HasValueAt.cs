using System;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> has a value at the <paramref name="path" /> which satisfies
	///     the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> is a property name, followed by any number of <c>.name</c> or zero-based
	///     <c>[index]</c> segments, e.g. <c>"customer.address.city"</c> or <c>"items[0].sku"</c>, and is written without a
	///     leading <c>$</c>. Wildcards, filters and recursive descent are not supported, and property names that contain
	///     <c>.</c>, <c>[</c> or <c>]</c> can only be addressed with <c>HasProperty</c>.
	///     <para />
	///     Only the first segment that cannot be reached is reported, and the <paramref name="expectations" /> are only
	///     verified, when the value exists. They receive the value, which is <see langword="null" /> for the JSON value
	///     <c>null</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static JsonPropertyResult<TNode, IThat<TNode?>> HasValueAt<TNode>(this IThat<TNode?> subject,
		string path,
		Action<IThatSubject<JsonNode?>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static (target, it, grammars) => new JsonNodeConstraint<TNode>(target.CreateConstraint(it, grammars)));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<TNode, IThat<TNode?>>(expectationBuilder, subject, target);
	}
}
