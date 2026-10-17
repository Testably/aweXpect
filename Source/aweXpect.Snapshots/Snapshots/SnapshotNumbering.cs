using System;
using System.Collections.Generic;
using System.Globalization;

namespace aweXpect.Snapshots;

/// <summary>
///     The numbers of the masked values of one subject, shared by all rules, so that the same value gets the same
///     number, e.g. a <see cref="Guid" /> member masked by its type and the same <see cref="Guid" /> found in a string.
/// </summary>
/// <remarks>
///     A new numbering is created for every evaluation, so that the numbers only depend on the subject itself. Each
///     template counts on its own, in the order in which the values are first seen.
/// </remarks>
internal sealed class SnapshotNumbering
{
	/// <summary>
	///     The text in a mask that is replaced with the number of the value.
	/// </summary>
	public const string Number = "{0}";

	private readonly Dictionary<string, Dictionary<string, string>> _numbers = new(StringComparer.Ordinal);

	/// <summary>
	///     Whether the <paramref name="template" /> contains <see cref="Number" />, so that it needs the key of the value.
	/// </summary>
	public static bool IsNumbered(string template)
		=> template.Contains(Number);

	/// <summary>
	///     Returns the name of a numbered placeholder as a template, e.g. <c>{Guid_{0}}</c>.
	/// </summary>
	public static string GetTemplate(string name)
		=> "{" + name + "_" + Number + "}";

	/// <summary>
	///     Replaces <see cref="Number" /> in the <paramref name="template" /> with the number of the value that the
	///     <paramref name="key" /> identifies, or returns a template without it as it is.
	/// </summary>
	/// <remarks>
	///     The replacement is plain text, not a composite format, so braces need no escaping. Every value masked with the
	///     same template shares one counter.
	/// </remarks>
	public string Apply(string template, string key)
	{
		if (!IsNumbered(template))
		{
			return template;
		}

		if (!_numbers.TryGetValue(template, out Dictionary<string, string>? numbers))
		{
			numbers = new Dictionary<string, string>(StringComparer.Ordinal);
			_numbers.Add(template, numbers);
		}

		if (!numbers.TryGetValue(key, out string? number))
		{
			number = (numbers.Count + 1).ToString(CultureInfo.InvariantCulture);
			numbers.Add(key, number);
		}

		return template.Replace(Number, number);
	}
}
