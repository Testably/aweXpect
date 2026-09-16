using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the param name of the actual <see cref="ArgumentException" />…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.String<Exception?, TException, IThat<TException>> HasParamName<TException>(
		this IThat<TException> subject)
		where TException : ArgumentException?
		=> new(subject, e => (e as ArgumentException)?.ParamName, "ParamName");

	/// <summary>
	///     Verifies that the actual <see cref="ArgumentException" /> has an <paramref name="expected" /> param name.
	/// </summary>
	/// <remarks>
	///     Shorthand for <c>HasParamName().EqualTo(expected)</c>, so a <see langword="null" />
	///     <paramref name="expected" /> requires the param name to be <see langword="null" /> as well.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TException, IThat<TException>> HasParamName<TException>(
		this IThat<TException> subject,
		string? expected)
		where TException : ArgumentException?
		=> subject.HasParamName().EqualTo(expected);
}
