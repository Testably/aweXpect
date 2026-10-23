using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class IsObjectResult<TType, TThat> : AndOrResult<TType, TThat>
{
	private readonly Func<TType, JsonElement> _objectAccessor;

	internal IsObjectResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, JsonElement> objectAccessor)
		: base(expectationBuilder, returnValue)
	{
		_objectAccessor = objectAccessor;
	}

	/// <summary>
	///     Further expectations on the properties of the object, in the order of the document.
	/// </summary>
	/// <remarks>
	///     The properties are a sequence instead of a dictionary, because an object may contain the same name more
	///     than once.
	///     <para />
	///     A <see cref="JsonProperty" /> has no value equality, so verify the properties with predicates like
	///     <c>Satisfy(p => p.Name.StartsWith("x-"))</c> or with expectations like
	///     <c>ComplyWith(p => p.Whose(x => x.Value, v => v.IsString()))</c>.
	/// </remarks>
	public IThat<IEnumerable<JsonProperty>> WhoseProperties
	{
		get
		{
			JsonFormatting.RegisterValueFormatter();
			return new ThatSubject<IEnumerable<JsonProperty>>(ExpectationBuilder.ForWhich<TType, IEnumerable<JsonProperty>>(
				subject => _objectAccessor(subject).EnumerateObject().ToArray(),
				" whose properties ",
				"properties",
				grammars => grammars | ExpectationGrammars.Plural | ExpectationGrammars.Introduced));
		}
	}
}
