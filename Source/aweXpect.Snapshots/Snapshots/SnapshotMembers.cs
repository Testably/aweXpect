using System;
using aweXpect.Core.Metadata;
using aweXpect.Formatting;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     The members of the objects in an object snapshot that a rule applies to, see <see cref="SnapshotFor{TReturn}.Members" />.
/// </summary>
/// <remarks>
///     The filters are combined, so a member has to match all of them.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
/// <typeparam name="TValue">
///     The type of the selected values, which <see cref="OfType{T}" /> and <see cref="OfExactType{T}" /> set, so that
///     <see cref="Convert{TResult}" /> gets a typed value, or <see cref="object" /> after an alternative type.
/// </typeparam>
public class SnapshotMembers<TReturn, TValue>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly ValueSelection _selection;

	internal SnapshotMembers(Func<SnapshotRule, TReturn> add, ValueSelection selection)
	{
		_add = add;
		_selection = selection;
	}

	/// <summary>
	///     Filters for the members with the <paramref name="name" />, e.g. <c>Id</c>.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is compared exactly and case-sensitive with the name of the member itself, not with
	///     its path, so <c>Id</c> matches <c>Id</c> and <c>Artist.Id</c>, but not <c>TrackId</c>. The options that follow
	///     apply to this filter only: <c>.IgnoringCase()</c>, and <c>.AsPrefix()</c>, <c>.AsSuffix()</c>,
	///     <c>.AsWildcard()</c> or <c>.AsRegex()</c>, with the timeout of
	///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c>. An invalid pattern throws an
	///     <see cref="ArgumentException" /> at the action, and a pattern that times out fails the expectation.
	/// </remarks>
	public SnapshotMembersWithName<TReturn, TValue> WithName(string name)
	{
		StringEqualityOptions options = new(nameof(name));
		return new SnapshotMembersWithName<TReturn, TValue>(_add,
			_selection.WithName(RuleArguments.ThrowIfNullOrEmpty(name, nameof(name)), options), options);
	}

	/// <summary>
	///     Filters for the members whose path ends with the <paramref name="path" />, e.g. <c>Artist.Id</c>.
	/// </summary>
	/// <remarks>
	///     The path has the format of <see cref="FormattedValue.Path" />, e.g. <c>Artist.Id</c> or <c>Tracks[0].Id</c>,
	///     and matches the end of the path on whole segments, compared ordinally, so <c>Artist.Id</c> matches the
	///     <c>Id</c> of an <c>Artist</c> at any depth, e.g. <c>Albums[0].Artist.Id</c>, but not <c>Artist.CatalogId</c>.
	///     To match the whole path from the subject, use <see cref="Which" />, e.g.
	///     <c>.Which(member =&gt; member.Path == "Id")</c>.
	/// </remarks>
	public SnapshotMembers<TReturn, TValue> WithPath(string path)
		=> new(_add, _selection.WithPath(RuleArguments.ThrowIfNullOrEmpty(path, nameof(path))));

	/// <summary>
	///     Filters for the members of the type <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     A member matches by its declared or its runtime type, including <see cref="Nullable{T}" /> and derived types,
	///     e.g. <c>OfType&lt;DateTime&gt;()</c> also matches a <c>DateTime?</c> member and a
	///     <see cref="DateTime" /> in an <see cref="object" /> member. Alternative types can follow, e.g.
	///     <c>.OfType&lt;int&gt;().OrOfType&lt;long&gt;()</c>.
	/// </remarks>
	public SnapshotMembersOfType<TReturn, T> OfType<T>()
		=> new(_add, _selection, [new TypeFilter(typeof(T), false),]);

	/// <summary>
	///     Filters for the members whose value is exactly of the type <typeparamref name="T" />, not of a derived type.
	/// </summary>
	/// <remarks>
	///     The runtime type of the value decides, or for a <see langword="null" /> value the declared type of the member,
	///     where a <c>DateTime?</c> member counts as <see cref="DateTime" />. Alternative types can follow, e.g.
	///     <c>.OfExactType&lt;int&gt;().OrOfExactType&lt;long&gt;()</c>.
	/// </remarks>
	public SnapshotMembersOfType<TReturn, T> OfExactType<T>()
		=> new(_add, _selection, [new TypeFilter(typeof(T), true),]);

	/// <summary>
	///     Filters for the members of an object of the type <typeparamref name="T" /> or of a derived type, e.g.
	///     <c>.DeclaredIn&lt;Album&gt;().WithName(nameof(Album.Price))</c> for the price of each album, but not of its
	///     tracks.
	/// </summary>
	public SnapshotMembers<TReturn, TValue> DeclaredIn<T>()
		=> new(_add, _selection.Where(value => typeof(T).IsAssignableFrom(value.DeclaringType)));

	/// <summary>
	///     Filters for the members whose value is an empty collection, e.g. an empty array, list, set or dictionary.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> value and an empty <see cref="string" /> are not selected. Only collections that
	///     know their count are selected: a sequence that does not, like a LINQ query or an iterator, is never
	///     enumerated here, as it might compute its items or only be enumerable once.
	/// </remarks>
	public SnapshotEmptyCollections<TReturn> WhichAreEmptyCollections()
		=> new(_add, _selection.Where(ValueFilters.IsEmptyCollection));

	/// <summary>
	///     Filters for the members for which the <paramref name="predicate" /> returns <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> gets each member with its <see cref="FormattedValue.Name" />,
	///     <see cref="FormattedValue.Path" />, declared <see cref="FormattedValue.Type" /> and
	///     <see cref="FormattedValue.DeclaringType" />; the value itself is only read when it calls
	///     <see cref="FormattedValue.GetValue()" />. An exception of the <paramref name="predicate" /> is thrown.
	/// </remarks>
	public SnapshotMembers<TReturn, TValue> Which(Func<FormattedValue, bool> predicate)
		=> new(_add, _selection.Where(RuleArguments.ThrowIfNull(predicate, nameof(predicate))));

	/// <summary>
	///     Filters for the members for which the <paramref name="predicate" /> returns <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> is asked like in <see cref="Which" />.
	/// </remarks>
	public SnapshotMembers<TReturn, TValue> Except(Func<FormattedValue, bool> predicate)
	{
		Func<FormattedValue, bool> filter = RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotMembers<TReturn, TValue>(_add, _selection.Where(value => !filter(value)));
	}

	/// <summary>
	///     Leaves out the selected members together with their names.
	/// </summary>
	/// <remarks>
	///     It wins over every masking and converter.
	/// </remarks>
	public TReturn Ignore()
		=> Add(ValueRule.Ignore(_selection));

	/// <summary>
	///     Replaces the value of each selected member with a numbered placeholder, where the same value keeps the same
	///     number, e.g. <c>Id = {Id_1}</c>; the shorthand for <c>Mask("{Id_{0}}")</c>.
	/// </summary>
	/// <remarks>
	///     The placeholder is named after the type of a selection that is filtered by its type, but not by its name or path, e.g.
	///     <c>{Guid_1}</c> for <c>.OfType&lt;Guid&gt;()</c>, or with alternative types after the first
	///     one that the value matches, and otherwise after the member. It shares its numbers with
	///     the other placeholders of that name, e.g. <c>{Guid_1}</c> with <see cref="SnapshotFor{TReturn}.Guids" />. A
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
	///     Replaces the value of each selected member with the value that <paramref name="convert" /> returns for it,
	///     e.g. only the year of a date, so that the meaningful part of a value that changes from run to run stays
	///     visible.
	/// </summary>
	/// <remarks>
	///     The converted value is written by the formatter like any other value, e.g. a <see cref="string" /> in quotes
	///     and an object member-wise, but the member and value rules do not apply to the values nested in it, while the
	///     text rules do; a converted <see langword="null" /> is written as <c>&lt;null&gt;</c>. A
	///     <see langword="null" /> value, or one that is no <typeparamref name="TValue" />, is not converted. An exception
	///     of <paramref name="convert" /> is thrown. The first masking or converter that matches a value wins.<br />
	///     The source generator of aweXpect registers the members of <typeparamref name="TResult" />, so that it is also
	///     written member-wise in a trimmed or Native AOT build.
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
