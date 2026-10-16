using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aweXpect.Equivalency;

/// <summary>
///     Equivalency options for a specific type.
/// </summary>
public record EquivalencyTypeOptions
{
	/// <summary>
	///     The comparison type to use.<br />
	///     If not set (<see langword="null" />), uses the
	///     <see cref="EquivalencyOptions.DefaultComparisonTypeSelector" /> to
	///     determine the <see cref="EquivalencyComparisonType" />.
	/// </summary>
	/// <remarks>
	///     Set for a type with <see cref="EquivalencyOptions.For{TMember}" />, it applies to that type only and not to
	///     its members, because a comparison type rarely suits both a type and its members: comparing a string member by
	///     members would reduce it to its characters. Set on the top-level options, it applies to the whole graph.
	/// </remarks>
	public EquivalencyComparisonType? ComparisonType { get; init; }

	/// <summary>
	///     The members that should be ignored when checking for equivalency.
	/// </summary>
	public MemberToIgnore[] MembersToIgnore { get; init; } = [];

	/// <summary>
	///     Specifies which fields to include in the object comparison.
	/// </summary>
	public IncludeMembers Fields { get; init; } = IncludeMembers.Public;

	/// <summary>
	///     Specifies which properties to include in the object comparison.
	/// </summary>
	public IncludeMembers Properties { get; init; } = IncludeMembers.Public;

	/// <summary>
	///     Ignores the order of collections when checking for equivalency.
	/// </summary>
	public bool IgnoreCollectionOrder { get; init; }

	/// <summary>
	///     The predicates that decide whether an actual value is equivalent to an expected value of another type.
	/// </summary>
	/// <remarks>
	///     The last one that applies to a pair decides, like the last registration of
	///     <see cref="EquivalencyOptions.For{TMember}" />.
	/// </remarks>
	internal TypePairComparison[] TypePairComparisons { get; init; } = [];

	internal void AppendOptions(StringBuilder sb, string indentation = "",
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore = null)
	{
		sb.Append(indentation).Append(" - include ").Append(GetVisibilities(Fields)).Append(" fields and");

		if (Fields != Properties)
		{
			sb.Append(' ').Append(GetVisibilities(Properties));
		}

		sb.AppendLine(" properties");
		if (ComparisonType is not null)
		{
			sb.Append(indentation).Append(" - compare types ");
			sb.AppendLine(ComparisonType switch
			{
				EquivalencyComparisonType.ByMembers => "by members",
				_ => "by value",
			});
		}

		if (IgnoreCollectionOrder)
		{
			sb.Append(indentation).AppendLine(" - ignore collection order");
		}

		foreach (TypePairComparison typePairComparison in TypePairComparisons)
		{
			sb.Append(indentation).Append(" - compare ");
			typePairComparison.AppendTypes(sb);
			sb.Append(" by ").AppendLine(typePairComparison.Description);
		}

		AppendMembersToIgnore(sb, indentation, "members",
			MembersToIgnore.Where(x
				=> x is not MemberToIgnore.ByFieldPredicate and not MemberToIgnore.ByPropertyPredicate));
		AppendUnmatchedMembersToIgnore(sb, indentation, unmatchedMembersToIgnore);
		AppendMembersToIgnore(sb, indentation, "fields",
			MembersToIgnore.Where(x => x is MemberToIgnore.ByFieldPredicate));
		AppendMembersToIgnore(sb, indentation, "properties",
			MembersToIgnore.Where(x => x is MemberToIgnore.ByPropertyPredicate));
	}

	private static string GetVisibilities(IncludeMembers includeMembers)
		=> (includeMembers.HasFlag(IncludeMembers.Public), includeMembers.HasFlag(IncludeMembers.Internal)) switch
		{
			(true, true) => "public and internal",
			(true, false) => "public",
			(false, true) => "internal",
			_ => "no",
		};

	/// <remarks>
	///     The kind is part of the rule, so it has to be part of the rendering too: otherwise a failure message cannot
	///     say whether a predicate was applied to the fields, to the properties or to both.
	/// </remarks>
	private static void AppendMembersToIgnore(StringBuilder sb, string indentation, string kind,
		IEnumerable<MemberToIgnore> membersToIgnore)
	{
		MemberToIgnore[] members = membersToIgnore.ToArray();
		if (members.Length == 0)
		{
			return;
		}

		sb.Append(indentation).Append(" - ignore ").Append(kind).Append(": ");
		Formatter.Format(sb, members, FormattingOptions.SingleLine);
		sb.AppendLine();
	}

	private void AppendUnmatchedMembersToIgnore(StringBuilder sb, string indentation,
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		if (unmatchedMembersToIgnore is null)
		{
			return;
		}

		foreach (MemberToIgnore memberToIgnore in MembersToIgnore)
		{
			if (memberToIgnore is MemberToIgnore.ByName byName &&
			    unmatchedMembersToIgnore.TryGetValue(byName.MemberName, out string? suggestion))
			{
				sb.Append(indentation).Append("   ");
				Formatter.Format(sb, byName.MemberName);
				sb.Append(" matched no member");
				if (suggestion is not null)
				{
					ClosestMatch.AppendHint(sb, byName.MemberName, [suggestion,]);
				}

				sb.AppendLine();
			}
		}
	}
}
