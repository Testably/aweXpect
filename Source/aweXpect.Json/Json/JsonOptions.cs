using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Customization;

namespace aweXpect.Json;

/// <summary>
///     Options for strings as JSON.
/// </summary>
/// <remarks>
///     The options are immutable: each extension method in <see cref="JsonOptionsExtensions" /> returns a modified copy.
/// </remarks>
[ImmutableOptions]
public class JsonOptions
{
	/// <summary>
	///     The line of the "JSON options" context for <see cref="TreatNullAsAbsent" />.
	/// </summary>
	internal const string TreatNullAsAbsentOption = " - treat null as absent";

	private readonly JsonDocumentOptions? _options;

	/// <summary>
	///     Creates options without any active option.
	/// </summary>
	internal JsonOptions()
	{
	}

	/// <summary>
	///     Copies the <paramref name="options" />, so that an extension method only has to set what it changes.
	/// </summary>
	/// <remarks>
	///     Every option must be copied here.
	/// </remarks>
	internal JsonOptions(JsonOptions options)
	{
		_options = options._options;
		SerializerOptions = options.SerializerOptions;
		IgnoreAdditionalProperties = options.IgnoreAdditionalProperties;
		IgnoreAdditionalArrayElements = options.IgnoreAdditionalArrayElements;
		IgnoreArrayOrder = options.IgnoreArrayOrder;
		TreatNullAsAbsent = options.TreatNullAsAbsent;
		MembersToIgnore = options.MembersToIgnore;
		NumberTolerance = options.NumberTolerance;
	}

	/// <summary>
	///     The options without any active option, shared by every expectation that starts from them.
	/// </summary>
	internal static JsonOptions Default { get; } = new();

	/// <summary>
	///     The current <see cref="JsonDocumentOptions" /> to use when interpreting a <see langword="string" /> as JSON.
	/// </summary>
	internal JsonDocumentOptions DocumentOptions
	{
		get => _options ?? Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get();
		init => _options = value;
	}

	/// <summary>
	///     The <see cref="JsonSerializerOptions" /> to serialize the expected value with, or <see langword="null" /> to use
	///     the customizable default.
	/// </summary>
	internal JsonSerializerOptions? SerializerOptions { get; init; }

	/// <summary>
	///     The <see cref="SerializerOptions" />, else the customized default, or <see langword="null" /> when neither is set.
	/// </summary>
	internal JsonSerializerOptions? ExpectedSerializerOptions
		=> SerializerOptions ?? Customize.aweXpect.Json().GetCustomizedSerializerOptions();

	/// <summary>
	///     Flag indicating, if the additional properties of objects in the subject should be ignored.
	/// </summary>
	internal bool IgnoreAdditionalProperties { get; init; }

	/// <summary>
	///     Flag indicating, if the additional elements of arrays in the subject should be ignored.
	/// </summary>
	internal bool IgnoreAdditionalArrayElements { get; init; }

	/// <summary>
	///     Flag indicating, if the order of the elements in arrays should be ignored.
	/// </summary>
	internal bool IgnoreArrayOrder { get; init; }

	/// <summary>
	///     Flag indicating, if a property with the value <see langword="null" /> should be treated as if it was absent.
	/// </summary>
	internal bool TreatNullAsAbsent { get; init; }

	/// <summary>
	///     The members that are ignored in the comparison, matched against their JSON path, e.g.
	///     <c>$.items[0].etag</c>.
	/// </summary>
	/// <remarks>
	///     The array is shared between copies, so it is replaced instead of changed.
	/// </remarks>
	internal JsonMemberToIgnore[] MembersToIgnore { get; init; } = [];

	/// <summary>
	///     The tolerance to apply when comparing numbers, or <see langword="null" /> to compare them exactly.
	/// </summary>
	internal JsonNumberTolerance? NumberTolerance { get; init; }

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
			AddContext(contexts, sb.ToString().TrimEnd());
		}
	}

	/// <summary>
	///     Adds the "JSON options" context with the <paramref name="options" /> to the <paramref name="contexts" />.
	/// </summary>
	/// <remarks>
	///     Also used by <see cref="JsonLookupOptions" />, so that both name their options under the same heading.
	/// </remarks>
	internal static void AddContext(ResultContextCollector contexts, string options)
		=> contexts.Add(new ResultContext.Fixed("JSON options", options, int.MinValue));

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
			sb.AppendLine(TreatNullAsAbsentOption);
		}

		if (NumberTolerance is not null)
		{
			sb.Append(" - compare numbers within").Append(NumberTolerance).AppendLine();
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

		foreach (string name in MembersToIgnore.OfType<JsonMemberToIgnore.ByName>().Select(named => named.MemberName))
		{
			if (unmatchedMembersToIgnore.TryGetValue(name, out string? suggestion))
			{
				sb.Append("   ");
				Formatter.Format(sb, name);
				sb.Append(" matched no member");
				ClosestMatch.AppendHint(sb, suggestion);
				sb.AppendLine();
			}
		}
	}
}
