using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.Extending;

namespace aweXpect.Xml;

/// <summary>
///     Decides which paths of a comparison are ignored and tracks which members to ignore by path matched one, so that a
///     failure can name those that matched none.
/// </summary>
/// <remarks>
///     A misspelled member to ignore ignores nothing without any notice, so the difference it was meant to hide is
///     reported without a reason.
/// </remarks>
internal sealed class MembersToIgnoreTracker(XmlOptions options)
{
	/// <summary>
	///     The number of compared paths that are kept as candidates for a member to ignore that matched none.
	/// </summary>
	private const int ComparedPathsToRemember = 1000;

	private readonly HashSet<string> _comparedPaths = new(StringComparer.Ordinal);
	private readonly HashSet<string> _matchedPatterns = new(StringComparer.Ordinal);

	/// <summary>
	///     Checks if the member at the <paramref name="path" /> is ignored, and tracks the result.
	/// </summary>
	public bool IsIgnored(string path)
	{
		if (options.MembersToIgnore.Count == 0)
		{
			return false;
		}

		XmlMemberToIgnore? memberToIgnore = options.MembersToIgnore.FirstOrDefault(member => member.IsIgnored(path));
		if (memberToIgnore is XmlMemberToIgnore.ByPath byPath)
		{
			_matchedPatterns.Add(byPath.Pattern);
		}
		else if (memberToIgnore is null && _comparedPaths.Count < ComparedPathsToRemember)
		{
			_comparedPaths.Add(path);
		}

		return memberToIgnore is not null;
	}

	/// <summary>
	///     Returns each member to ignore by path that matched none of the compared paths, with the path it most likely
	///     meant, or <see langword="null" /> when every one matched or no path was compared.
	/// </summary>
	public Dictionary<string, string?>? GetUnmatchedMembersToIgnore()
	{
		if (_comparedPaths.Count == 0 && _matchedPatterns.Count == 0)
		{
			return null;
		}

		Dictionary<string, string?>? unmatched = null;
		foreach (XmlMemberToIgnore.ByPath byPath in options.MembersToIgnore.OfType<XmlMemberToIgnore.ByPath>())
		{
			if (_matchedPatterns.Contains(byPath.Pattern))
			{
				continue;
			}

			IEnumerable<string> candidates = _comparedPaths
				.Select(byPath.GetCandidate)
				.OfType<string>()
				.Distinct(StringComparer.Ordinal);
			(unmatched ??= new Dictionary<string, string?>(StringComparer.Ordinal))[byPath.Pattern] =
				ClosestMatch.Find(byPath.Pattern, candidates);
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
}
