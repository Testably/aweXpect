using System;
using System.Linq;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Options;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     The immutable selection of values nested in an object snapshot: members only, or also the items of tuples,
///     collections and dictionaries, and the filters that all have to match.
/// </summary>
internal sealed class ValueSelection
{
	private readonly Func<string>[] _descriptions;
	private readonly Func<FormattedValue, bool>[] _filters;
	private readonly bool _isMembersOnly;
	private readonly bool _isNamed;
	private readonly TypeFilter[] _types;
	private readonly Action[] _validations;

	private ValueSelection(bool isMembersOnly, Func<FormattedValue, bool>[] filters, Action[] validations,
		TypeFilter[] types, bool isNamed, Func<string>[] descriptions)
	{
		_isMembersOnly = isMembersOnly;
		_filters = filters;
		_descriptions = descriptions;
		_validations = validations;
		_types = types;
		_isNamed = isNamed;
	}

	public static ValueSelection Members { get; } = new(true, [], [], [], false, []);

	public static ValueSelection Values { get; } = new(false, [], [], [], false, []);

	/// <summary>
	///     Adds the <paramref name="filter" />, which the <paramref name="description" /> describes for the failure,
	///     e.g. <c>declared in Album</c>.
	/// </summary>
	public ValueSelection Where(Func<FormattedValue, bool> filter, Func<string> description)
		=> new(_isMembersOnly, [.. _filters, filter,], _validations, _types, _isNamed,
			[.. _descriptions, description,]);

	/// <summary>
	///     Selects the values with the <paramref name="name" />, compared with the <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     The options are read from the holder whenever a value is matched, so that an option that changes the holder
	///     directly also applies.
	/// </remarks>
	public ValueSelection WithName(string name, IReadOnlyOptionsHolder<StringEqualityOptions> options)
		=> new(_isMembersOnly,
			[.. _filters, value => StringRules.IsMatch(options.Value, value.Name, name, nameof(name)),],
			[.. _validations, () => options.Value.ValidateExpected(name, nameof(name)),], _types, true,
			[.. _descriptions, () => "whose name " + options.Value.GetExpectation(name, ExpectationGrammars.Active),]);

	public ValueSelection WithPath(string path)
		=> new(_isMembersOnly, [.. _filters, value => ValueFilters.HasPath(value.Path, path),], _validations,
			_types, true, [.. _descriptions, () => "whose path ends with " + Formatter.Format(path),]);

	/// <summary>
	///     Selects the values that match any of the <paramref name="types" />.
	/// </summary>
	public ValueSelection OfType(TypeFilter[] types)
		=> new(_isMembersOnly, [.. _filters, value => types.Any(type => type.Matches(value)),], _validations, types,
			_isNamed, [.. _descriptions, () => string.Join(" or ", types.Select(type => type.Description)),]);

	/// <summary>
	///     Describes the selection for the failure, e.g. <c>the members whose name is equal to "Id"</c>.
	/// </summary>
	public string Describe()
	{
		string values = _isMembersOnly ? "the members" : "the values";
		return _descriptions.Length == 0
			? values
			: values + " " + string.Join(" and ", _descriptions.Select(describe => describe()));
	}

	/// <summary>
	///     Whether the selection contains the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     The kind is checked first, so that the filters of members never see an item.
	/// </remarks>
	public bool Matches(FormattedValue value)
		=> (!_isMembersOnly || value.Kind == FormattedValueKind.Member) && _filters.All(filter => filter(value));

	/// <summary>
	///     Throws when a filter cannot be applied, e.g. for an invalid regex pattern of a name.
	/// </summary>
	public void Validate()
		=> Array.ForEach(_validations, validate => validate());

	/// <summary>
	///     The name of a numbered placeholder for the selected <paramref name="value" />: the first of the types that it
	///     matches for a selection by type only, e.g. <c>Guid</c>, otherwise the name of the member or tuple item, or
	///     <c>Item</c> for an item of a collection or the value of a dictionary entry.
	/// </summary>
	/// <remarks>
	///     A single type is not checked again, so that the value is not read once more.
	/// </remarks>
	public string GetPlaceholderName(FormattedValue value)
	{
		if (!_isNamed)
		{
			TypeFilter? matchingType = _types.Length == 1
				? _types[0]
				: _types.FirstOrDefault(type => type.Matches(value));
			if (matchingType is not null)
			{
				return matchingType.PlaceholderName;
			}
		}

		return value.Kind == FormattedValueKind.CollectionItem ? "Item" : value.Name;
	}
}
