using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class IsArrayResult<TType, TThat> : AndOrResult<TType, TThat>
{
	private readonly Func<TType, JsonElement> _arrayAccessor;

	internal IsArrayResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, JsonElement> arrayAccessor)
		: base(expectationBuilder, returnValue)
	{
		_arrayAccessor = arrayAccessor;
	}

	/// <summary>
	///     Further expectations on the elements of the array.
	/// </summary>
	/// <remarks>
	///     A <see cref="JsonElement" /> has no value equality, so verify the elements with expectations like
	///     <c>ComplyWith(e => e.Matches(…))</c> or with predicates instead of comparing them to other elements.
	/// </remarks>
	public IThatSubject<IEnumerable<JsonElement>> WhoseElements
	{
		get
		{
			JsonFormatting.AddValueFormatters(ExpectationBuilder);
			return new ThatSubject<IEnumerable<JsonElement>>(ExpectationBuilder.ForWhich<TType, IEnumerable<JsonElement>>(
				subject => _arrayAccessor(subject).EnumerateArray().ToArray(),
				" whose elements ",
				"elements",
				grammars => grammars | ExpectationGrammars.Plural | ExpectationGrammars.Introduced));
		}
	}
}
