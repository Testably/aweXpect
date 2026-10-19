using System;
using aweXpect.Core.Metadata;
using aweXpect.Formatting;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     The values in an object snapshot that a rule applies to, see <see cref="SnapshotFor{TReturn}.Values" />: the
///     members, and the items of tuples, collections and dictionaries.
/// </summary>
/// <remarks>
///     The filters are combined, so a value has to match all of them.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
/// <typeparam name="TValue">
///     The type of the selected values, which <see cref="OfType{T}" /> and <see cref="OfExactType{T}" /> set, so that
///     <see cref="Convert{TResult}" /> gets a typed value, or <see cref="object" /> after an alternative type.
/// </typeparam>
public class SnapshotValues<TReturn, TValue>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly ValueSelection _selection;

	internal SnapshotValues(Func<SnapshotRule, TReturn> add, ValueSelection selection)
	{
		_add = add;
		_selection = selection;
	}

	/// <summary>
	///     Filters for the values with the <paramref name="name" />, e.g. <c>Id</c>.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is compared exactly and case-sensitive with the <see cref="FormattedValue.Name" />
	///     of the value itself, not with its path: the name of a member or tuple item (<c>Item1</c>), or the index of a
	///     collection item (<c>[0]</c>) or the key of a dictionary entry (<c>[Abbey Road]</c>) in brackets. The options
	///     that follow apply to this filter only, like in <see cref="SnapshotMembers{TReturn, TValue}.WithName" />.
	/// </remarks>
	public SnapshotValuesWithName<TReturn, TValue> WithName(string name)
	{
		StringEqualityOptions options = new(nameof(name));
		return new SnapshotValuesWithName<TReturn, TValue>(_add,
			_selection.WithName(RuleArguments.ThrowIfNullOrEmpty(name, nameof(name)), options), options);
	}

	/// <summary>
	///     Filters for the values whose path ends with the <paramref name="path" />, e.g. <c>Artist.Id</c> or
	///     <c>Tracks[0]</c>.
	/// </summary>
	/// <remarks>
	///     The path matches like in <see cref="SnapshotMembers{TReturn, TValue}.WithPath" />.
	/// </remarks>
	public SnapshotValues<TReturn, TValue> WithPath(string path)
		=> new(_add, _selection.WithPath(RuleArguments.ThrowIfNullOrEmpty(path, nameof(path))));

	/// <summary>
	///     Filters for the values of the type <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     A value matches by its declared or its runtime type, including <see cref="Nullable{T}" /> and derived types,
	///     e.g. <c>OfType&lt;Guid&gt;()</c> also matches a <c>Guid?</c> member, a <see cref="Guid" /> in an
	///     <see cref="object" /> member and the items of a <c>List&lt;Guid&gt;</c>. Alternative types can follow, e.g.
	///     <c>.OfType&lt;int&gt;().OrOfType&lt;long&gt;()</c>.
	/// </remarks>
	public SnapshotValuesOfType<TReturn, T> OfType<T>()
		=> new(_add, _selection, [new TypeFilter(typeof(T), false),]);

	/// <summary>
	///     Filters for the values that are exactly of the type <typeparamref name="T" />, not of a derived type.
	/// </summary>
	/// <remarks>
	///     The runtime type of the value decides, or for a <see langword="null" /> value its declared type, where a
	///     <c>Guid?</c> member counts as <see cref="Guid" />. Alternative types can follow, e.g.
	///     <c>.OfExactType&lt;int&gt;().OrOfExactType&lt;long&gt;()</c>.
	/// </remarks>
	public SnapshotValuesOfType<TReturn, T> OfExactType<T>()
		=> new(_add, _selection, [new TypeFilter(typeof(T), true),]);

	/// <summary>
	///     Filters for the values in an object, tuple or collection of the type <typeparamref name="T" /> or of a derived
	///     type, e.g. <c>.DeclaredIn&lt;Album&gt;()</c> for the members of each album.
	/// </summary>
	public SnapshotValues<TReturn, TValue> DeclaredIn<T>()
		=> new(_add, _selection.Where(value => typeof(T).IsAssignableFrom(value.DeclaringType)));

	/// <summary>
	///     Filters for the values for which the <paramref name="predicate" /> returns <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> gets each value with its <see cref="FormattedValue.Kind" />,
	///     <see cref="FormattedValue.Name" />, <see cref="FormattedValue.Path" />, declared
	///     <see cref="FormattedValue.Type" /> and <see cref="FormattedValue.DeclaringType" />; the value itself is only read
	///     when it calls <see cref="FormattedValue.GetValue()" />. An exception of the <paramref name="predicate" /> is
	///     thrown.
	/// </remarks>
	public SnapshotValues<TReturn, TValue> Which(Func<FormattedValue, bool> predicate)
		=> new(_add, _selection.Where(RuleArguments.ThrowIfNull(predicate, nameof(predicate))));

	/// <summary>
	///     Filters for the values for which the <paramref name="predicate" /> returns <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> is asked like in <see cref="Which" />.
	/// </remarks>
	public SnapshotValues<TReturn, TValue> Except(Func<FormattedValue, bool> predicate)
	{
		Func<FormattedValue, bool> filter = RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotValues<TReturn, TValue>(_add, _selection.Where(value => !filter(value)));
	}

	/// <summary>
	///     Leaves out the selected values: a member together with its name, an item of a tuple, collection or dictionary
	///     together with its place.
	/// </summary>
	/// <remarks>
	///     It wins over every masking and converter. As a left out item changes the collection, e.g. a
	///     <c>List&lt;DateTime&gt;</c> is written as an empty list by <c>.For.Values().OfType&lt;DateTime&gt;().Ignore()</c>,
	///     prefer <see cref="SnapshotFor{TReturn}.Members" /> to only leave out members, or mask the items instead.
	/// </remarks>
	public TReturn Ignore()
		=> Add(ValueRule.Ignore(_selection));

	/// <summary>
	///     Replaces each selected value with a numbered placeholder, where the same value keeps the same number, e.g.
	///     <c>{Guid_1}</c>.
	/// </summary>
	/// <remarks>
	///     The placeholder is named after the type of a selection that is filtered by its type, but not by its name or path, e.g.
	///     <c>{Guid_1}</c> for <c>.OfType&lt;Guid&gt;()</c>, or with alternative types after the first
	///     one that the value matches, and otherwise after the member or tuple item, or <c>Item</c>
	///     for an item of a collection or the value of a dictionary entry. It shares its numbers with the other
	///     placeholders of that name, e.g. <c>{Guid_1}</c> with <see cref="SnapshotFor{TReturn}.Guids" />. A
	///     <see langword="null" /> value is not masked. The first masking or converter that matches a value wins.
	/// </remarks>
	public TReturn Mask()
		=> Add(ValueRule.Mask(_selection));

	/// <summary>
	///     Replaces each selected value with the <paramref name="template" />, which is written as it is, without
	///     quotes, e.g. <c>{token}</c>, or with <c>{0}</c> replaced by the number of the value, e.g. <c>{Customer_{0}}</c>.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, not as a composite format, so braces need no escaping:
	///     <c>{{Customer_{0}}}</c> is written as <c>{{Customer_1}}</c>. Every value masked with the same template shares
	///     one counter across all rules, also the text rules, and the same value keeps the same number; <see cref="Mask()" />
	///     is the template named after the selection. A <see langword="null" /> value is not masked. The first masking or
	///     converter that matches a value wins.
	/// </remarks>
	public TReturn Mask(string template)
	{
		RuleArguments.ThrowIfNull(template, nameof(template));
		return Add(ValueRule.Mask(_selection, _ => template));
	}

	/// <summary>
	///     Replaces each selected value with the mask that the <paramref name="template" /> returns for it, e.g. a
	///     numbered placeholder named after the type that declares it.
	/// </summary>
	/// <remarks>
	///     The mask is a template like in <see cref="Mask(string)" />, so values with the same template share one
	///     counter, e.g. <c>value =&gt; "{" + value.DeclaringType.Name + "_{0}}"</c>. A value for which the
	///     <paramref name="template" /> returns <see langword="null" /> is kept, and an exception of the
	///     <paramref name="template" /> is thrown. A <see langword="null" /> value is not masked. The first masking or
	///     converter that matches a value wins.
	/// </remarks>
	public TReturn Mask(Func<FormattedValue, string?> template)
	{
		RuleArguments.ThrowIfNull(template, nameof(template));
		return Add(ValueRule.Mask(_selection, template));
	}

	/// <summary>
	///     Replaces each selected value with the value that <paramref name="convert" /> returns for it, e.g. only the year
	///     of a date, so that the meaningful part of a value that changes from run to run stays visible.
	/// </summary>
	/// <remarks>
	///     The converted value is written like in <see cref="SnapshotMembers{TReturn, TValue}.Convert{TResult}" />.
	/// </remarks>
	public TReturn Convert<[RequiresMemberMetadata] TResult>(Func<TValue, TResult> convert)
	{
		RuleArguments.ThrowIfNull(convert, nameof(convert));
		return Add(ValueRule.Convert(_selection,
			value => value is TValue typedValue ? ValueFormatting.Replace(convert(typedValue)) : null));
	}

	private TReturn Add(ValueRule rule)
	{
		_selection.Validate();
		return _add(new SnapshotRule(rule));
	}
}
