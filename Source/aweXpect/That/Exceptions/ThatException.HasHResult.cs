using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the HResult of the actual <see cref="Exception" />…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<TException> HasHResult<TException>(this IThat<TException> subject)
		where TException : Exception?
		=> new(subject, e => e?.HResult, "HResult");

	/// <summary>
	///     Verifies that the HResult of the actual <see cref="Exception" /> is equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException>> HasHResult<TException>(
		this IThat<TException> subject,
		int expected)
		where TException : Exception?
		=> subject.HasHResult().EqualTo(expected);
}
