using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Renders the violating members of the member constraints, for a single type as a context and for several types
///     as one indented line per type, each followed by its list of violating members.
/// </summary>
/// <remarks>
///     Shared between the nullable and the non-nullable member constraints, so that the formatting (indentation,
///     comma placement, null handling) cannot drift between the two.
/// </remarks>
internal static class MemberViolationRenderer
{
	/// <summary>
	///     Adds the context with the <paramref name="title" />, which lists the <paramref name="members" />, unless there
	///     are none.
	/// </summary>
	public static void AddMembersContext(this ResultContextCollector contexts, string title, MemberInfo[] members)
	{
		if (members.Length > 0)
		{
			contexts.Add(new ResultContext.SyncCallback(title,
				() => Formatter.Format(FormattableMember.FromAll(members), FormattingOptions.MultipleLines),
				int.MaxValue));
		}
	}

	/// <summary>
	///     Formats the <paramref name="types" /> as a list with one line per type (appending
	///     <c>{memberHeader}[…]</c> when <paramref name="violations" /> has an entry for it).
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> type has no violations to list; it fails because it cannot satisfy the
	///     expectation, so it is rendered without a (contradictory empty) violation list.
	/// </remarks>
	public static string FormatTypesWithViolatingMembers(
		IReadOnlyList<Type?> types,
		IReadOnlyDictionary<Type, MemberInfo[]> violations,
		string memberHeader)
	{
		StringBuilder stringBuilder = new();
		stringBuilder.Append('[');
		for (int index = 0; index < types.Count; index++)
		{
			Type? type = types[index];
			stringBuilder.Append(Environment.NewLine).Append("  ")
				.Append(Formatter.Format(type));

			if (type is not null && violations.TryGetValue(type, out MemberInfo[]? members))
			{
				stringBuilder.Append(memberHeader).Append(Formatter.Format(FormattableMember.FromAll(members)));
			}

			if (index < types.Count - 1)
			{
				stringBuilder.Append(',');
			}
		}

		return stringBuilder.Append(Environment.NewLine).Append(']').ToString();
	}
}
