using System;
using System.Linq;
using aweXpect.Formatting;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     The immutable selection of values nested in an object snapshot: members only, or also the items of tuples,
///     collections and dictionaries, and the filters that all have to match.
/// </summary>
internal sealed class ValueSelection
{
	private readonly Func<FormattedValue, bool>[] _filters;
	private readonly bool _isMembersOnly;
	private readonly bool _isNamed;
	private readonly string? _typeName;
	private readonly Action[] _validations;

	private ValueSelection(bool isMembersOnly, Func<FormattedValue, bool>[] filters, Action[] validations,
		string? typeName, bool isNamed)
	{
		_isMembersOnly = isMembersOnly;
		_filters = filters;
		_validations = validations;
		_typeName = typeName;
		_isNamed = isNamed;
	}

	public static ValueSelection Members { get; } = new(true, [], [], null, false);

	public static ValueSelection Values { get; } = new(false, [], [], null, false);

	public ValueSelection Where(Func<FormattedValue, bool> filter)
		=> new(_isMembersOnly, [.. _filters, filter,], _validations, _typeName, _isNamed);

	/// <summary>
	///     Selects the values with the <paramref name="name" />, compared with the <paramref name="options" />, which can
	///     still change until the rule is created.
	/// </summary>
	public ValueSelection WithName(string name, StringEqualityOptions options)
		=> new(_isMembersOnly, [.. _filters, value => StringRules.IsMatch(options, value.Name, name),],
			[.. _validations, () => options.ValidateExpected(name),], _typeName, true);

	public ValueSelection WithPath(string path)
		=> new(_isMembersOnly, [.. _filters, value => ValueFilters.HasPath(value.Path, path),], _validations,
			_typeName, true);

	public ValueSelection OfType(Type type, Func<FormattedValue, bool> filter)
		=> new(_isMembersOnly, [.. _filters, filter,], _validations, ValueFilters.GetPlaceholderName(type),
			_isNamed);

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
	///     The name of a numbered placeholder for the selected <paramref name="value" />: the type of a selection by type
	///     only, e.g. <c>Guid</c>, otherwise the name of the member or tuple item, or <c>Item</c> for an item of a
	///     collection or the value of a dictionary entry.
	/// </summary>
	public string GetPlaceholderName(FormattedValue value)
	{
		if (_typeName is not null && !_isNamed)
		{
			return _typeName;
		}

		return value.Kind == FormattedValueKind.CollectionItem ? "Item" : value.Name;
	}
}
