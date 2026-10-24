using System;

namespace aweXpect.Core;

/// <summary>
///     The settings of a string comparison that a <see cref="StringMatchType" /> supports, see
///     <see cref="StringMatchType.SupportedSettings" />.
/// </summary>
[Flags]
public enum StringMatchSettingKinds
{
	/// <summary>
	///     No setting.
	/// </summary>
	None = 0,

	/// <summary>
	///     <c>IgnoringCase()</c>, see <see cref="StringMatchSettings.IgnoreCase" />.
	/// </summary>
	IgnoreCase = 1,

	/// <summary>
	///     <c>Using(comparer)</c>, see <see cref="StringMatchSettings.Comparer" />.
	/// </summary>
	Comparer = 2,

	/// <summary>
	///     The options that change the whitespace or the lines of both values before they are compared, e.g.
	///     <c>IgnoringIndentation()</c> or <c>NormalizingWhiteSpace()</c>.
	/// </summary>
	WhiteSpace = 4,
}
