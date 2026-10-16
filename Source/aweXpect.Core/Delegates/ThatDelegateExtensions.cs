using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Expectations on delegates that only apply to some of their return types.
/// </summary>
public static class ThatDelegateExtensions
{
	/// <summary>
	///     Verifies that the object returned by the delegate is garbage collected after the <paramref name="cleanup" />
	///     ran on it.
	/// </summary>
	/// <remarks>
	///     The delegate and the <paramref name="cleanup" /> run in a separate method that keeps only a weak reference to
	///     the object, before full blocking garbage collections are forced. The expectation fails when anything else
	///     still references the object afterward, e.g. an event handler or a static collection that the
	///     <paramref name="cleanup" /> did not remove.
	///     <para />
	///     An exception thrown by the delegate or by the <paramref name="cleanup" /> fails the expectation, as does a
	///     delegate that returns <see langword="null" />.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="cleanup" /> is <see langword="null" />.</exception>
	[GuaranteesNotNull]
	public static ExpectationResult IsCollectedAfter<T>(this ThatDelegate.WithValue<T> source, Action<T> cleanup,
		[CallerArgumentExpression("cleanup")] string doNotPopulateThisValue = "")
		where T : class
		=> source.IsCollectedAfterCore(cleanup, doNotPopulateThisValue);
}
