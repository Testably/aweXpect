using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

/// <summary>
///     Options for comparing XML.
/// </summary>
/// <remarks>
///     The options are immutable: each extension method in <see cref="XmlOptionsExtensions" /> returns a modified copy.
/// </remarks>
public sealed class XmlOptions
{
	/// <summary>
	///     Creates options without any active option.
	/// </summary>
	public XmlOptions()
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
		IgnoreElementOrder = options.IgnoreElementOrder;
		MembersToIgnore = options.MembersToIgnore;
	}

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
		if (IgnoreElementOrder)
		{
			sb.AppendLine(" - ignore element order");
		}

		if (MembersToIgnore.Count > 0)
		{
			sb.Append(" - ignore members: [").Append(string.Join(", ", MembersToIgnore)).Append(']').AppendLine();
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
				sb.Append("   ").Append(Formatter.Format(pattern)).Append(" matched no member");
				if (suggestion is not null)
				{
					sb.Append(" (did you mean ").Append(Formatter.Format(suggestion)).Append("?)");
				}

				sb.AppendLine();
			}
		}
	}
}
