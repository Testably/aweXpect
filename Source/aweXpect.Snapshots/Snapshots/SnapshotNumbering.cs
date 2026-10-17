using System;
using System.Collections.Generic;

namespace aweXpect.Snapshots;

/// <summary>
///     The numbered placeholders of one subject value, shared by all maskings, so that the same value gets the same
///     placeholder, e.g. a <see cref="Guid" /> member masked by its type and the same <see cref="Guid" /> found in a
///     string.
/// </summary>
/// <remarks>
///     A new numbering is created for every evaluation, so that the numbers only depend on the subject itself. Each
///     placeholder name counts on its own, in the order in which the values are first seen.
/// </remarks>
internal sealed class SnapshotNumbering
{
	private readonly Dictionary<string, Dictionary<string, string>> _placeholders = new(StringComparer.Ordinal);

	/// <summary>
	///     Returns the placeholder <c>{Name_N}</c> for the <paramref name="key" />, which identifies the value.
	/// </summary>
	public string GetPlaceholder(string name, string key)
	{
		if (!_placeholders.TryGetValue(name, out Dictionary<string, string>? placeholders))
		{
			placeholders = new Dictionary<string, string>(StringComparer.Ordinal);
			_placeholders.Add(name, placeholders);
		}

		if (!placeholders.TryGetValue(key, out string? placeholder))
		{
			placeholder = $"{{{name}_{placeholders.Count + 1}}}";
			placeholders.Add(key, placeholder);
		}

		return placeholder;
	}
}
