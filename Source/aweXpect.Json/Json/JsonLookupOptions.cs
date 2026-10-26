using aweXpect.Core;

namespace aweXpect.Json;

/// <summary>
///     The options of <c>HasProperty</c>, <c>DoesNotHaveProperty</c>, <c>HasValueAt</c> and <c>DoesNotHaveValueAt</c>
///     that change how the value is looked up in the subject.
/// </summary>
/// <remarks>
///     An immutable value: the option methods return new options, and <see langword="default" /> is like
///     <see cref="Default" />.
/// </remarks>
[ImmutableOptions]
public readonly struct JsonLookupOptions
{
	/// <summary>
	///     The options that treat a property with the value <see langword="null" /> as existing.
	/// </summary>
	public static JsonLookupOptions Default => default;

	/// <summary>
	///     Flag indicating, if a property with the value <see langword="null" /> should be treated as if it was absent.
	/// </summary>
	internal bool TreatNullAsAbsent { get; private init; }

	/// <summary>
	///     Treats a property with the value <see langword="null" /> as if it was absent, also on the way to a value at a
	///     path, when <paramref name="treatNullAsAbsent" /> is <see langword="true" />.
	/// </summary>
	public JsonLookupOptions TreatingNullAsAbsent(bool treatNullAsAbsent = true)
		=> this with
		{
			TreatNullAsAbsent = treatNullAsAbsent,
		};

	/// <summary>
	///     Adds the "JSON options" context, which lists the active options, to the <paramref name="contexts" />, unless no
	///     option is active.
	/// </summary>
	internal void AddContext(ResultContextCollector contexts)
	{
		if (TreatNullAsAbsent)
		{
			JsonOptions.AddContext(contexts, JsonOptions.TreatNullAsAbsentOption);
		}
	}
}
