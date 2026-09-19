using System;

namespace aweXpect.Core.Helpers;

internal static class DateTimeKindComparison
{
	/// <summary>
	///     Returns <see langword="true" /> when <paramref name="actual" /> and <paramref name="expected" /> are both
	///     a <see cref="DateTime" /> that cannot be compared: a <see cref="DateTimeKind.Local" /> and a
	///     <see cref="DateTimeKind.Utc" /> value denote different instants for the same ticks, so treating them as
	///     equal would have to guess the local offset. <see cref="DateTimeKind.Unspecified" /> stays compatible with
	///     both.
	/// </summary>
	/// <remarks>
	///     Duplicates <c>aweXpect.Helpers.EqualityHelpers.AreKindCompatible</c>, because outside of a debug build
	///     <c>aweXpect</c> compiles against the released <c>aweXpect.Core</c> package and cannot consume a new helper
	///     from it.
	/// </remarks>
	public static bool AreKindsIncompatible<TActual, TExpected>(TActual actual, TExpected expected)
		=> actual is DateTime actualDateTime && expected is DateTime expectedDateTime &&
		   actualDateTime.Kind != DateTimeKind.Unspecified &&
		   expectedDateTime.Kind != DateTimeKind.Unspecified &&
		   actualDateTime.Kind != expectedDateTime.Kind;
}
