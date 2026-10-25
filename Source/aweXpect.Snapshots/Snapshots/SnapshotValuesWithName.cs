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
///     <c>.AsWildcard()</c> or <c>.AsRegex()</c>, apply to the name only.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
/// <typeparam name="TValue">The type of the selected values.</typeparam>
public sealed class SnapshotValuesWithName<TResult, TValue> : SnapshotValues<TResult, TValue>,
	IOptionsProvider<StringEqualityOptions>,
	IStringMatchTypeOptions
{
	private readonly StringEqualityOptions _options;

	internal SnapshotValuesWithName(Func<SnapshotRule, TResult> add, ValueSelection selection,
		StringEqualityOptions options)
		: base(add, selection)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
}
