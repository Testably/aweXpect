using System;
using System.Runtime.CompilerServices;
using aweXpect.Customization;

namespace aweXpect.Equivalency;

/// <summary>
///     Extension methods for <see cref="EquivalencyOptions" />.
/// </summary>
public static class EquivalencyOptionsExtensions
{
	/// <summary>
	///     Ignores the <paramref name="memberToIgnore" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions IgnoringMember<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		string memberToIgnore)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore = [..@this.MembersToIgnore, new MemberToIgnore.ByName(memberToIgnore),],
		};

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new MemberToIgnore.ByPredicate(predicate, doNotPopulateThisValue),
			],
		};

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new MemberToIgnore.ByPredicate((memberName, _) => predicate(memberName),
					doNotPopulateThisValue),
			],
		};

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		Func<Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new MemberToIgnore.ByPredicate((_, memberType) => predicate(memberType),
					doNotPopulateThisValue),
			],
		};

	/// <summary>
	///     Ignores fields matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions IgnoringFields<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new MemberToIgnore.ByFieldPredicate(predicate, doNotPopulateThisValue),
			],
		};

	/// <summary>
	///     Ignores properties matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions IgnoringProperties<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new MemberToIgnore.ByPropertyPredicate(predicate, doNotPopulateThisValue),
			],
		};

	/// <summary>
	///     Includes fields according to the <paramref name="fieldsToInclude" /> parameter.
	/// </summary>
	/// <remarks>
	///     If <paramref name="fieldsToInclude" /> is set to <see cref="IncludeMembers.None" />, fields are excluded from the
	///     comparison.
	/// </remarks>
	public static TEquivalencyOptions IncludingFields<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		IncludeMembers fieldsToInclude = IncludeMembers.Public)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			Fields = fieldsToInclude,
		};

	/// <summary>
	///     Includes properties according to the <paramref name="propertiesToInclude" /> parameter.
	/// </summary>
	/// <remarks>
	///     If <paramref name="propertiesToInclude" /> is set to <see cref="IncludeMembers.None" />, properties are excluded
	///     from the
	///     comparison.
	/// </remarks>
	public static TEquivalencyOptions IncludingProperties<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		IncludeMembers propertiesToInclude = IncludeMembers.Public)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			Properties = propertiesToInclude,
		};

	/// <summary>
	///     Ignores the order of collections when checking for equivalency
	///     when <paramref name="ignoreCollectionOrder" /> is <see langword="true" />.
	/// </summary>
	public static TEquivalencyOptions IgnoringCollectionOrder<TEquivalencyOptions>(
		this TEquivalencyOptions @this,
		bool ignoreCollectionOrder = true)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> @this with
		{
			IgnoreCollectionOrder = ignoreCollectionOrder,
		};

	/// <summary>
	///     Creates a new <see cref="EquivalencyOptions" /> instance from the provided <paramref name="callback" />.
	/// </summary>
	/// <remarks>
	///     Uses the default instance, when no <paramref name="callback" /> is given.
	/// </remarks>
	internal static EquivalencyOptions FromCallback(Func<EquivalencyOptions, EquivalencyOptions>? callback)
		=> callback is null
			? Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get()
			: callback(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get());
}
