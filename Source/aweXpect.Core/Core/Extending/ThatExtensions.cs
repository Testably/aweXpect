using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Access to the expectation infrastructure behind an <see cref="IThat{T}" /> subject.
/// </summary>
/// <remarks>
///     This class is in its own namespace, so that its methods are only suggested to extension authors who import it.
/// </remarks>
public static class ThatExtensions
{
	/// <summary>
	///     Returns the <paramref name="subject" /> as <see cref="IExpectThat{T}" />, which gives access to the
	///     <see cref="IExpectThat{T}.ExpectationBuilder" /> to register constraints on.
	/// </summary>
	/// <exception cref="NotSupportedException">
	///     The <paramref name="subject" /> does not implement <see cref="IExpectThat{T}" />.
	/// </exception>
	public static IExpectThat<T> Get<T>(this IThat<T> subject)
	{
		if (subject is IExpectThat<T> expectThat)
		{
			return expectThat;
		}

		throw Tracing.WriteException(
			new NotSupportedException("IThat<T> must also implement IExpectThat<T>."));
	}
}
