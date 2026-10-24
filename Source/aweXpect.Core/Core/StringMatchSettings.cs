using System;
using System.Collections.Generic;

namespace aweXpect.Core;

/// <summary>
///     The settings of a string comparison, frozen when the matcher of an evaluation is created.
/// </summary>
/// <remarks>
///     The whitespace and newline options are applied to both values before a matcher sees them, so only the settings
///     that change the comparison itself are passed on.
/// </remarks>
public readonly struct StringMatchSettings
{
	/// <summary>
	///     Ignores the casing when <paramref name="ignoreCase" />, or compares with the <paramref name="comparer" />.
	/// </summary>
	public StringMatchSettings(bool ignoreCase, IEqualityComparer<string>? comparer)
	{
		IgnoreCase = ignoreCase;
		Comparer = comparer;
	}

	/// <summary>
	///     Whether the casing is ignored.
	/// </summary>
	public bool IgnoreCase { get; }

	/// <summary>
	///     The comparer of the caller, or <see langword="null" /> when none is specified.
	/// </summary>
	public IEqualityComparer<string>? Comparer { get; }

	/// <summary>
	///     The <see cref="Comparer" /> of the caller, or else the ordinal comparer that honours <see cref="IgnoreCase" />.
	/// </summary>
	public IEqualityComparer<string> EffectiveComparer
		=> Comparer ?? (IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
}
