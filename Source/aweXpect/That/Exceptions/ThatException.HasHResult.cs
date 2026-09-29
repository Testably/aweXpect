using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
public static partial class ThatException
{
	/// <summary>
	///     Verifies that the HResult of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Exception?, TException, IThat<TException?>> HasHResult<TException>(
		this IThat<TException?> subject)
		where TException : Exception
		=> new(subject, e => e?.HResult, "HResult");

	/// <summary>
	///     Verifies that the HResult of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException?>> HasHResult<TException>(
		this IThat<TException?> subject,
		int? expected)
		where TException : Exception
		=> subject.HasHResult().EqualTo(expected);
}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
