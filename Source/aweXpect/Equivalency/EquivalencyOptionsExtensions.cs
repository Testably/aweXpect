using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Helpers;

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
		this TEquivalencyOptions options,
		string memberToIgnore)
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		memberToIgnore.ThrowIfNull();
		if (memberToIgnore.Length == 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				"The 'memberToIgnore' cannot be empty.", nameof(memberToIgnore)));
		}

		return options with
		{
			MembersToIgnore = [..options.MembersToIgnore, new MemberToIgnore.ByName(memberToIgnore),],
		};
	}

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new MemberToIgnore.ByPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new MemberToIgnore.ByPredicate((memberName, _) => predicate(memberName),
					doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}

	/// <summary>
	///     Ignores members matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions Ignoring<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		Func<Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new MemberToIgnore.ByPredicate((_, memberType) => predicate(memberType),
					doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}

	/// <summary>
	///     Ignores fields matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions IgnoringFields<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new MemberToIgnore.ByFieldPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}

	/// <summary>
	///     Ignores properties matching the <paramref name="predicate" /> when checking for equivalency.
	/// </summary>
	public static TEquivalencyOptions IgnoringProperties<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		Func<string, Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new MemberToIgnore.ByPropertyPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}

	/// <summary>
	///     Includes fields according to the <paramref name="fieldsToInclude" /> parameter.
	/// </summary>
	/// <remarks>
	///     If <paramref name="fieldsToInclude" /> is set to <see cref="IncludeMembers.None" />, fields are excluded from the
	///     comparison.
	/// </remarks>
	public static TEquivalencyOptions IncludingFields<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		IncludeMembers fieldsToInclude = IncludeMembers.Public)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> options with
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
		this TEquivalencyOptions options,
		IncludeMembers propertiesToInclude = IncludeMembers.Public)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> options with
		{
			Properties = propertiesToInclude,
		};

	/// <summary>
	///     Ignores the order of collections when checking for equivalency
	///     when <paramref name="ignoreCollectionOrder" /> is <see langword="true" />.
	/// </summary>
	public static TEquivalencyOptions IgnoringCollectionOrder<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		bool ignoreCollectionOrder = true)
		where TEquivalencyOptions : EquivalencyTypeOptions
		=> options with
		{
			IgnoreCollectionOrder = ignoreCollectionOrder,
		};

	/// <summary>
	///     Limits the comparison to <paramref name="maximumRecursionDepth" /> nested objects on a single path.
	/// </summary>
	/// <remarks>
	///     Defaults to 100. A graph that is deeper fails the comparison instead of overflowing the stack.
	/// </remarks>
	public static TEquivalencyOptions LimitingRecursionDepth<TEquivalencyOptions>(
		this TEquivalencyOptions options,
		int maximumRecursionDepth)
		where TEquivalencyOptions : EquivalencyOptions
	{
		ThrowHelper.ThrowIfRecursionDepthIsNotPositive(maximumRecursionDepth);
		return options with
		{
			MaxRecursionDepth = maximumRecursionDepth,
		};
	}

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
