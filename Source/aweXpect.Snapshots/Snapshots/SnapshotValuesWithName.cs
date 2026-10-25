using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Snapshots;

/// <summary>
///     The values with a name that a rule applies to, see <see cref="SnapshotMembers{TResult, TValue}.WithName" />,
///     which also allows setting how the name is compared.
/// </summary>
/// <remarks>
///     The options of aweXpect's string comparison, e.g. <c>.IgnoringCase()</c>, <c>.AsPrefix()</c>, <c>.AsSuffix()</c>,
///     <c>.AsWildcard()</c> or <c>.AsRegex()</c>, apply to the name only. Each option returns a new selection, so a
///     stored selection can be continued more than once and never changes a rule created from it.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
/// <typeparam name="TValue">The type of the selected values.</typeparam>
public sealed class SnapshotValuesWithName<TResult, TValue> : SnapshotValues<TResult, TValue>,
	IOptionsProvider<StringEqualityOptions>,
	IStringMatchTypeOptions,
	ICopyOnOption<StringEqualityOptions, SnapshotValuesWithName<TResult, TValue>>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly string _name;
	private readonly StringEqualityOptions _options;
	private readonly ValueSelection _source;

	internal SnapshotValuesWithName(Func<SnapshotRule, TResult> add, ValueSelection source, string name,
		StringEqualityOptions options)
		: base(add, source.WithName(name, options))
	{
		_add = add;
		_source = source;
		_name = name;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;

	/// <inheritdoc cref="ICopyOnOption{TOptions, TResult}.WithOptions(TOptions)" />
	SnapshotValuesWithName<TResult, TValue>
		ICopyOnOption<StringEqualityOptions, SnapshotValuesWithName<TResult, TValue>>.WithOptions(
			StringEqualityOptions options)
		=> new(_add, _source, _name, options);
}
