using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Equivalency;

namespace aweXpect.Json;

/// <summary>
///     Options for strings as JSON.
/// </summary>
public record JsonOptions
{
	private readonly JsonDocumentOptions? _options;

	/// <summary>
	///     The current <see cref="JsonDocumentOptions" /> to use when interpreting a <see langword="string" /> as JSON.
	/// </summary>
	public JsonDocumentOptions DocumentOptions
	{
		get => _options ?? Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get();
		init => _options = value;
	}

	/// <summary>
	///     Flag indicating, if the additional properties of objects in the subject should be ignored.
	/// </summary>
	public bool IgnoreAdditionalProperties { get; init; }

	/// <summary>
	///     Flag indicating, if the additional elements of arrays in the subject should be ignored.
	/// </summary>
	public bool IgnoreAdditionalArrayElements { get; init; }

	/// <summary>
	///     Flag indicating, if the order of the elements in arrays should be ignored.
	/// </summary>
	public bool IgnoreArrayOrder { get; init; }

	/// <summary>
	///     Flag indicating, if a property with the value <see langword="null" /> should be treated as if it was absent.
	/// </summary>
	public bool TreatNullAsAbsent { get; init; }

	/// <summary>
	///     The members that are ignored in the comparison, matched against their JSON path, e.g.
	///     <c>$.items[0].etag</c>.
	/// </summary>
	public MemberToIgnore[] MembersToIgnore { get; init; } = [];

	/// <summary>
	///     Adds the "JSON options" context, which lists the active options, to the <paramref name="contexts" /> of a
	///     failed comparison, unless no option is active.
	/// </summary>
	/// <remarks>
	///     The context comes last, like the "Equivalency options" of aweXpect.
	///     Each member to ignore by name in the <paramref name="unmatchedMembersToIgnore" /> is named below the members to
	///     ignore, with the path it most likely meant.
	/// </remarks>
	internal void AddContext(ResultContextCollector contexts,
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		StringBuilder sb = new();
		AppendOptions(sb, unmatchedMembersToIgnore);
		if (sb.Length > 0)
		{
			contexts.Add(new ResultContext.Fixed("JSON options", sb.ToString().TrimEnd(), int.MinValue));
		}
	}

	private void AppendOptions(StringBuilder sb, IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		if (IgnoreAdditionalProperties)
		{
			sb.AppendLine(" - ignore additional properties");
		}

		if (IgnoreAdditionalArrayElements)
		{
			sb.AppendLine(" - ignore additional array elements");
		}

		if (IgnoreArrayOrder)
		{
			sb.AppendLine(" - ignore array order");
		}

		if (TreatNullAsAbsent)
		{
			sb.AppendLine(" - treat null as absent");
		}

		if (MembersToIgnore.Length > 0)
		{
			sb.Append(" - ignore members: ");
			Formatter.Format(sb, MembersToIgnore, FormattingOptions.SingleLine);
			sb.AppendLine();
			AppendUnmatchedMembersToIgnore(sb, unmatchedMembersToIgnore);
		}
	}

	private void AppendUnmatchedMembersToIgnore(StringBuilder sb,
		IReadOnlyDictionary<string, string?>? unmatchedMembersToIgnore)
	{
		if (unmatchedMembersToIgnore is null)
		{
			return;
		}

		foreach (string name in MembersToIgnore.OfType<NamedMemberToIgnore>().Select(named => named.MemberName))
		{
			if (unmatchedMembersToIgnore.TryGetValue(name, out string? suggestion))
			{
				sb.Append("   ");
				Formatter.Format(sb, name);
				sb.Append(" matched no member");
				if (suggestion is not null)
				{
					sb.Append(" (did you mean ");
					Formatter.Format(sb, suggestion);
					sb.Append("?)");
				}

				sb.AppendLine();
			}
		}
	}
}
