using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Extending;

/// <summary>
///     Finds the value that was most likely meant, to suggest it in a failure message.
/// </summary>
public static class ClosestMatch
{
	/// <summary>
	///     The number of candidates that are compared at most, so that a failure on a large collection stays fast.
	/// </summary>
	private const int MaximumCandidates = 1000;

	/// <summary>
	///     Returns the candidate that the <paramref name="value" /> most likely meant, or <see langword="null" /> when
	///     no candidate is close enough.
	/// </summary>
	/// <remarks>
	///     A candidate that differs only in casing is the closest. Otherwise the candidate with the fewest edits wins,
	///     counting a swap of two adjacent characters as one edit, when it needs at most one edit per three characters
	///     of the <paramref name="value" />. Several equally close candidates, or one equal to the
	///     <paramref name="value" />, suggest nothing, as a wrong suggestion misleads more than none.
	/// </remarks>
	public static string? Find(string value, IEnumerable<string> candidates)
	{
		value.ThrowIfNull();
		candidates.ThrowIfNull();

		int maximumDistance = Math.Max(1, value.Length / 3);
		string? closest = null;
		int closestDistance = int.MaxValue;
		bool isAmbiguous = false;
		int count = 0;
		foreach (string? candidate in candidates)
		{
			if (count++ == MaximumCandidates)
			{
				break;
			}

			if (candidate is null || string.Equals(candidate, closest, StringComparison.Ordinal))
			{
				continue;
			}

			if (string.Equals(candidate, value, StringComparison.Ordinal))
			{
				return null;
			}

			int distance = GetDistance(value, candidate, maximumDistance);
			if (distance < closestDistance)
			{
				closest = candidate;
				closestDistance = distance;
				isAmbiguous = false;
			}
			else if (distance == closestDistance)
			{
				isAmbiguous = true;
			}
		}

		return isAmbiguous || closestDistance > maximumDistance ? null : closest;
	}

	/// <summary>
	///     Appends <c> (did you mean "…"?)</c> when one of the <paramref name="candidates" /> is close to the
	///     <paramref name="value" />, as found by <see cref="Find(string, IEnumerable{string})" />.
	/// </summary>
	public static void AppendHint(StringBuilder stringBuilder, string value, IEnumerable<string> candidates)
	{
		stringBuilder.ThrowIfNull();
		AppendHint(stringBuilder, Find(value, candidates));
	}

	/// <summary>
	///     Appends <c> (did you mean "…"?)</c> for a <paramref name="suggestion" /> that is already known, e.g. from
	///     <see cref="Find(string, IEnumerable{string})" />, or nothing when it is <see langword="null" />.
	/// </summary>
	public static void AppendHint(StringBuilder stringBuilder, string? suggestion)
	{
		stringBuilder.ThrowIfNull();
		if (suggestion is not null)
		{
			stringBuilder.Append(" (did you mean ");
			Formatter.Format(stringBuilder, suggestion);
			stringBuilder.Append("?)");
		}
	}

	/// <summary>
	///     The optimal string alignment distance, ignoring the casing, or a value above the
	///     <paramref name="maximumDistance" /> as soon as it is certain to exceed it.
	/// </summary>
	private static int GetDistance(string value, string candidate, int maximumDistance)
	{
		if (Math.Abs(value.Length - candidate.Length) > maximumDistance)
		{
			return maximumDistance + 1;
		}

		int[] beforePrevious = new int[candidate.Length + 1];
		int[] previous = new int[candidate.Length + 1];
		int[] current = new int[candidate.Length + 1];
		for (int j = 0; j <= candidate.Length; j++)
		{
			previous[j] = j;
		}

		for (int i = 1; i <= value.Length; i++)
		{
			current[0] = i;
			int rowMinimum = i;
			for (int j = 1; j <= candidate.Length; j++)
			{
				int cost = IsSame(value[i - 1], candidate[j - 1]) ? 0 : 1;
				int distance = Math.Min(Math.Min(previous[j] + 1, current[j - 1] + 1), previous[j - 1] + cost);
				if (i > 1 && j > 1 &&
				    IsSame(value[i - 1], candidate[j - 2]) &&
				    IsSame(value[i - 2], candidate[j - 1]))
				{
					distance = Math.Min(distance, beforePrevious[j - 2] + 1);
				}

				current[j] = distance;
				rowMinimum = Math.Min(rowMinimum, distance);
			}

			if (rowMinimum > maximumDistance)
			{
				return maximumDistance + 1;
			}

			(beforePrevious, previous, current) = (previous, current, beforePrevious);
		}

		return previous[candidate.Length];
	}

	private static bool IsSame(char left, char right)
		=> char.ToUpperInvariant(left) == char.ToUpperInvariant(right);
}
