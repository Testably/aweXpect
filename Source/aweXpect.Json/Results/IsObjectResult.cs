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
///     <see cref="JsonValueKind.Object" /> with properties of type <typeparamref name="TItem" />.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class IsObjectResult<TType, TThat, TItem> : AndOrResult<TType, TThat>
{
	private readonly Func<TType, IEnumerable<TItem>> _propertiesAccessor;

	internal IsObjectResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, IEnumerable<TItem>> propertiesAccessor)
		: base(expectationBuilder, returnValue)
	{
		_propertiesAccessor = propertiesAccessor;
	}

	/// <summary>
	///     Further expectations on the properties of the object, in the order of the document.
	/// </summary>
	/// <remarks>
	///     The properties are a sequence instead of a dictionary, because an object may contain the same name more
	///     than once.
	///     <para />
	///     The properties of a <see cref="JsonNode" /> are pairs of the name and the node of the value itself, e.g. for
	///     <c>Contains(p => p.Value == node)</c>. A <see cref="JsonProperty" /> has no value equality, so verify the
	///     properties of a <see cref="JsonElement" /> with predicates like
	///     <c>Satisfy(p => p.Name.StartsWith("x-"))</c> or with expectations like
	///     <c>ComplyWith(p => p.Whose(x => x.Value, v => v.IsString()))</c>.
	/// </remarks>
	public IThatSubject<IEnumerable<TItem>> WhoseProperties
	{
		get
		{
			JsonFormatting.AddValueFormatters(ExpectationBuilder);
			return new ThatSubject<IEnumerable<TItem>>(ExpectationBuilder.ForWhich(
				_propertiesAccessor,
				" whose properties ",
				"properties",
				grammars => grammars | ExpectationGrammars.Plural | ExpectationGrammars.Introduced));
		}
	}
}
