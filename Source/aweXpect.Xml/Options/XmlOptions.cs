using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml.Options;

/// <summary>
///     Options for comparing XML.
/// </summary>
/// <remarks>
///     The options are immutable: each extension method in <see cref="XmlOptionsExtensions" /> returns a modified copy.
/// </remarks>
[ImmutableOptions]
public sealed class XmlOptions
{
	/// <summary>
	///     Creates options without any active option.
	/// </summary>
	internal XmlOptions()
	{
	}

	/// <summary>
	///     Copies the <paramref name="options" />, so that an extension method only has to set what it changes.
	/// </summary>
	/// <remarks>
	///     Every option must be copied here.
	/// </remarks>
	internal XmlOptions(XmlOptions options)
	{
		IgnoreAdditionalNodes = options.IgnoreAdditionalNodes;
		IgnoreElementOrder = options.IgnoreElementOrder;
		MembersToIgnore = options.MembersToIgnore;
	}

	/// <summary>
	///     Flag indicating, if the attributes and child nodes of the subject that are not paired with an expected one
	///     should be ignored.
	/// </summary>
	internal bool IgnoreAdditionalNodes { get; init; }

	internal bool IgnoreElementOrder { get; init; }

	internal IReadOnlyList<XmlMemberToIgnore> MembersToIgnore { get; init; } = [];

	/// <summary>
	///     Adds the "XML options" context, which lists the active options, to the <paramref name="contexts" /> of a failed
	///     comparison, unless no option is active.
	/// </summary>
	/// <remarks>
	///     The context comes last, like the "Equivalency options" of aweXpect.
	///     Each member to ignore by path in the <paramref name="unmatchedMembersToIgnore" /> is named below the members to
	///     ignore, with the path it most likely meant.
	/// </remarks>
	internal void AddContext(ResultContextCollector contexts,
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		StringBuilder sb = new();
		if (IgnoreAdditionalNodes)
		{
			sb.AppendLine(" - ignore additional nodes");
		}

		if (IgnoreElementOrder)
		{
			sb.AppendLine(" - ignore element order");
		}

		if (MembersToIgnore.Count > 0)
		{
			sb.Append(" - ignore members: ");
			Formatter.Format(sb, MembersToIgnore, FormattingOptions.SingleLine);
			sb.AppendLine();
			AppendUnmatchedMembersToIgnore(sb, unmatchedMembersToIgnore);
		}

		if (sb.Length > 0)
		{
			contexts.Add(new ResultContext.Fixed("XML options", sb.ToString().TrimEnd(), int.MinValue));
		}
	}

	private void AppendUnmatchedMembersToIgnore(StringBuilder sb,
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		if (unmatchedMembersToIgnore is null)
		{
			return;
		}

		foreach (string pattern in MembersToIgnore.OfType<XmlMemberToIgnore.ByPath>().Select(byPath => byPath.Pattern))
		{
			if (unmatchedMembersToIgnore.TryGetValue(pattern, out string? suggestion))
			{
				sb.Append("   ");
				Formatter.Format(sb, pattern);
				sb.Append(" matched no member");
				ClosestMatch.AppendHint(sb, suggestion);
				sb.AppendLine();
			}
		}
	}
}
