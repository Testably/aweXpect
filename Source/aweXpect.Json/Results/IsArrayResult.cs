using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a <see cref="JsonElement" /> or a <see cref="JsonNode" /> is an
///     <see cref="JsonValueKind.Array" /> with elements of type <typeparamref name="TItem" />.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class IsArrayResult<TType, TThat, TItem> : AndOrResult<TType, TThat>
{
	private readonly Func<TType, IEnumerable<TItem>> _elementsAccessor;

	internal IsArrayResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, IEnumerable<TItem>> elementsAccessor)
		: base(expectationBuilder, returnValue)
	{
		_elementsAccessor = elementsAccessor;
	}

	/// <summary>
	///     Further expectations on the elements of the array.
	/// </summary>
	/// <remarks>
	///     The elements of a <see cref="JsonNode" /> are the nodes themselves, e.g. for <c>Contains(node)</c>. A
	///     <see cref="JsonElement" /> has no value equality, so verify its elements with expectations like
	///     <c>ComplyWith(e => e.Matches(…))</c> or with predicates instead of comparing them to other elements.
	/// </remarks>
	public IThatSubject<IEnumerable<TItem>> WhoseElements
	{
		get
		{
			JsonFormatting.AddValueFormatters(ExpectationBuilder);
			return new ThatSubject<IEnumerable<TItem>>(ExpectationBuilder.ForWhich(
				_elementsAccessor,
				" whose elements ",
				"elements",
				grammars => grammars | ExpectationGrammars.Plural | ExpectationGrammars.Introduced));
		}
	}
}
