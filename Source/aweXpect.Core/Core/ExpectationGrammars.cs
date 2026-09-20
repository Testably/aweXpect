using System;

namespace aweXpect.Core;

/// <summary>
///     The grammars to use in the expectation text.
/// </summary>
[Flags]
public enum ExpectationGrammars
{
	/// <summary>
	///     The default expectation text.
	/// </summary>
	None = 0,

	/// <summary>
	///     The expectation is nested.
	/// </summary>
	Nested = 1 << 1,

	/// <summary>
	///     The expectation should be in plural form.
	/// </summary>
	Plural = 1 << 2,

	/// <summary>
	///     The expectation should be in active voice.
	/// </summary>
	Active = 1 << 3,

	/// <summary>
	///     The expectation should be negated.
	/// </summary>
	Negated = 1 << 4,

	/// <summary>
	///     The subject of the expectation was already introduced by a connector that cannot be dropped, e.g. the
	///     <c>that</c> of <c>has item that</c>.
	/// </summary>
	/// <remarks>
	///     A member expectation must then not start another relative clause, because <c>has item that whose Value …</c>
	///     is not grammatical.
	/// </remarks>
	Introduced = 1 << 5,
}
