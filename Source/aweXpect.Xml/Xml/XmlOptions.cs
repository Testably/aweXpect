using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Xml;

/// <summary>
///     Options for comparing XML.
/// </summary>
public record XmlOptions
{
	internal bool IgnoreElementOrder { get; init; }

	internal IReadOnlyList<XmlMemberToIgnore> MembersToIgnore { get; init; } = [];

	internal bool IsIgnored(string path)
		=> MembersToIgnore.Any(member => member.IsIgnored(path));

	/// <summary>
	///     Adds the "XML options" context, which lists the active options, to the <paramref name="contexts" /> of a failed
	///     comparison, unless no option is active.
	/// </summary>
	/// <remarks>
	///     The context comes last, like the "Equivalency options" of aweXpect.
	/// </remarks>
	internal void AddContext(ResultContextCollector contexts)
	{
		List<string> options = new();
		if (IgnoreElementOrder)
		{
			options.Add(" - ignore element order");
		}

		if (MembersToIgnore.Count > 0)
		{
			options.Add($" - ignore members: [{string.Join(", ", MembersToIgnore)}]");
		}

		if (options.Count > 0)
		{
			contexts.Add(new ResultContext.Fixed("XML options", string.Join(Environment.NewLine, options),
				int.MinValue));
		}
	}
}
