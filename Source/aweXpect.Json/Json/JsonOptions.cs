using System.Text.Json;
using aweXpect.Customization;

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
}
