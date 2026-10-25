using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Array" /> with an element at
	///     the zero-based <paramref name="index" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> HasElementAt<TNode>(this IThat<TNode?> subject,
		int index)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNegative(index, nameof(index));
		JsonValueTarget target = JsonValueTarget.ElementAt(index);
		return new AndOrResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint(target,
				static (target, it, grammars) => new JsonNodeConstraint<TNode>(target.CreateConstraint(it, grammars))),
			subject);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Array" /> with an element at
	///     the zero-based <paramref name="index" /> which satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> are only verified, when the array has an element at the
	///     <paramref name="index" />. They receive the element, which is <see langword="null" /> for the JSON value
	///     <c>null</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> HasElementAt<TNode>(this IThat<TNode?> subject,
		int index,
		Action<IThatSubject<JsonNode?>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNegative(index, nameof(index));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ElementAt(index);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static (target, it, grammars) => new JsonNodeConstraint<TNode>(target.CreateConstraint(it, grammars)));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new AndOrResult<TNode, IThat<TNode?>>(expectationBuilder, subject);
	}
}
