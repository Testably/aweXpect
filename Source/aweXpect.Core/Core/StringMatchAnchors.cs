using System;

namespace aweXpect.Core;

/// <summary>
///     The edges of the subject that the expected value of a <see cref="StringMatchType" /> lines up with, see
///     <see cref="StringMatchType.Anchors" />.
/// </summary>
/// <remarks>
///     The whitespace options remove the leading or trailing whitespace of the expected value only at an anchored edge,
///     so that whitespace at an inner edge, e.g. at the end of a prefix, still has to match whitespace in the subject.
/// </remarks>
[Flags]
public enum StringMatchAnchors
{
	/// <summary>
	///     The expected value may occur anywhere in the subject, e.g. a substring.
	/// </summary>
	None = 0,

	/// <summary>
	///     The expected value starts at the start of the subject, e.g. a prefix.
	/// </summary>
	Start = 1,

	/// <summary>
	///     The expected value ends at the end of the subject, e.g. a suffix.
	/// </summary>
	End = 2,

	/// <summary>
	///     The expected value covers the whole subject, e.g. an equal value.
	/// </summary>
	StartAndEnd = Start | End,
}
