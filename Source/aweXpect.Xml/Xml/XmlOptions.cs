using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Xml;

/// <summary>
///     Options for comparing XML.
/// </summary>
public record XmlOptions
{
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
		if (MembersToIgnore.Count > 0)
		{
			contexts.Add(new ResultContext.Fixed("XML options",
				$" - ignore members: [{string.Join(", ", MembersToIgnore)}]", int.MinValue));
		}
	}
}
