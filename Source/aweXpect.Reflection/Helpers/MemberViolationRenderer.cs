using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Renders the grouped failure output of the nullability member constraints: one indented line per failing
///     type, each followed by its list of violating members.
/// </summary>
/// <remarks>
///     Shared between the nullable and the non-nullable member constraints, so that the formatting (indentation,
///     comma placement, null handling) cannot drift between the two.
/// </remarks>
internal static class MemberViolationRenderer
{
	/// <summary>
	///     Appends <c>{it}{header}[</c>, one indented line per type in <paramref name="types" /> (appending
	///     <c>{memberHeader}[…]</c> when <paramref name="violations" /> has an entry for it), and a closing <c>]</c>.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> type has no violations to list; it fails because it cannot satisfy the
	///     expectation, so it is rendered without a (contradictory empty) violation list.
	/// </remarks>
	public static void AppendTypesWithViolatingMembers(
		StringBuilder stringBuilder,
		string it,
		string header,
		IReadOnlyList<Type?> types,
		IReadOnlyDictionary<Type, MemberInfo[]> violations,
		string memberHeader,
		string? indentation)
	{
		string itemIndentation = (indentation ?? string.Empty) + "  ";
		stringBuilder.Append(it).Append(header).Append('[');
		for (int index = 0; index < types.Count; index++)
		{
			Type? type = types[index];
			stringBuilder.Append(Environment.NewLine).Append(itemIndentation)
				.Append(Formatter.Format(type));

			if (type is not null && violations.TryGetValue(type, out MemberInfo[]? members))
			{
				stringBuilder.Append(memberHeader).Append(Formatter.Format(members));
			}

			if (index < types.Count - 1)
			{
				stringBuilder.Append(',');
			}
		}

		stringBuilder.Append(Environment.NewLine).Append(indentation).Append(']');
	}
}
