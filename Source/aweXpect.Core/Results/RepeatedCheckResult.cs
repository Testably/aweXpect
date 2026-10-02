using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     Allows repeatedly executing the underlying check.
/// </summary>
public class RepeatedCheckResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	RepeatedCheckOptions options)
	: AndOrResult<TType, TThat>(expectationBuilder, returnValue),
		IOptionsProvider<RepeatedCheckOptions>
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	RepeatedCheckOptions IOptionsProvider<RepeatedCheckOptions>.Options => options;

	/// <summary>
	///     Allows a <paramref name="timeout" /> until the condition must be met.
	/// </summary>
	public WithRepetition Within(TimeSpan timeout)
	{
		options.Within(timeout);
		return new WithRepetition(ExpectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     Repeatedly execute the underlying check.
	/// </summary>
	public class WithRepetition(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		RepeatedCheckOptions options)
		: AndOrResult<TType, TThat>(expectationBuilder, returnValue)
	{
		/// <summary>
		///     Sets the interval in which the condition should be checked.
		/// </summary>
		/// <remarks>
		///     Defaults to <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
		/// </remarks>
		public AndOrResult<TType, TThat> CheckEvery(TimeSpan interval)
		{
			options.CheckEvery(interval);
			return this;
		}
	}
}
