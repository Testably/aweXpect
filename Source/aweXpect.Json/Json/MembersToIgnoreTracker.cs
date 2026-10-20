using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using aweXpect.Core.Extending;
using aweXpect.Equivalency;

namespace aweXpect.Json;

/// <summary>
///     Decides which JSON paths of a comparison are ignored and tracks which members to ignore by name matched one, so
///     that a failure can name those that matched none.
/// </summary>
/// <remarks>
///     A misspelled name ignores nothing without any notice, so the difference it was meant to hide is reported without
///     a reason.
/// </remarks>
internal sealed class MembersToIgnoreTracker(MemberToIgnore[] membersToIgnore)
{
	/// <summary>
	///     The number of compared paths that are kept as candidates for a member to ignore that matched none.
	/// </summary>
	private const int ComparedPathsToRemember = 1000;

	private readonly HashSet<string> _comparedPaths = new(StringComparer.Ordinal);
	private readonly HashSet<string> _matchedNames = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///     Checks if the value at the <paramref name="memberPath" /> is ignored, and tracks the result.
	/// </summary>
	public bool IsIgnored(string memberPath)
	{
		if (membersToIgnore.Length == 0)
		{
			return false;
		}

		MemberToIgnore? memberToIgnore =
			Array.Find(membersToIgnore, member => member.IgnoreMember(memberPath, typeof(JsonElement)));
		if (memberToIgnore is NamedMemberToIgnore named)
		{
			_matchedNames.Add(named.MemberName);
		}
		else if (memberToIgnore is null && _comparedPaths.Count < ComparedPathsToRemember)
		{
			_comparedPaths.Add(memberPath);
		}

		return memberToIgnore is not null;
	}

	/// <summary>
	///     Returns each member to ignore by name that matched none of the compared paths, with the compared path it most
	///     likely meant, or <see langword="null" /> when every one matched or no path was compared.
	/// </summary>
	public Dictionary<string, string?>? GetUnmatchedMembersToIgnore()
	{
		if (_comparedPaths.Count == 0 && _matchedNames.Count == 0)
		{
			return null;
		}

		Dictionary<string, string?>? unmatched = null;
		foreach (string name in membersToIgnore.OfType<NamedMemberToIgnore>().Select(named => named.MemberName))
		{
			if (_matchedNames.Contains(name))
			{
				continue;
			}

			int segments = name.Count(c => c == '.') + 1;
			(unmatched ??= new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase))[name] =
				ClosestMatch.Find(name, _comparedPaths.Select(path => GetLastSegments(path, segments)).Distinct());
		}

		return unmatched;
	}

	/// <summary>
	///     Adds the <paramref name="unmatched" /> members to ignore of another failed comparison to the
	///     <paramref name="kept" /> ones, so that a suggestion found for one comparison is not lost to a later one.
	/// </summary>
	public static Dictionary<string, string?>? Keep(Dictionary<string, string?>? kept,
		Dictionary<string, string?>? unmatched)
	{
		if (kept is null || unmatched is null)
		{
			return kept ?? unmatched;
		}

		foreach (KeyValuePair<string, string?> member in unmatched)
		{
			if (!kept.TryGetValue(member.Key, out string? suggestion) || suggestion is null)
			{
				kept[member.Key] = member.Value;
			}
		}

		return kept;
	}

	private static string GetLastSegments(string memberPath, int segments)
	{
		int start = memberPath.Length;
		for (int i = 0; i < segments && start > 0; i++)
		{
			start = memberPath.LastIndexOf('.', start - 1);
			if (start < 0)
			{
				return memberPath;
			}
		}

		return memberPath.Substring(start + 1);
	}
}
