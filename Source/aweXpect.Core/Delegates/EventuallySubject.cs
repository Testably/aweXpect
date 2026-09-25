using System;
using System.Diagnostics;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Delegates;

/// <summary>
///     The subject of <see cref="ThatDelegate.WithValue{T}.Eventually()" />, which allows configuring the retries.
/// </summary>
[DebuggerDisplay("EventuallySubject<{typeof(T)}>: {ExpectationBuilder}")]
public readonly struct EventuallySubject<T> : IExpectThat<T>, IThatSubject<T>
{
	private readonly EventuallyExpectationBuilder<T> _expectationBuilder;

	internal EventuallySubject(EventuallyExpectationBuilder<T> expectationBuilder)
	{
		_expectationBuilder = expectationBuilder;
	}

	/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
	public ExpectationBuilder ExpectationBuilder => _expectationBuilder;

	/// <summary>
	///     Sets the <paramref name="interval" /> in which the delegate is re-evaluated.
	/// </summary>
	/// <remarks>
	///     Defaults to <c>Customize.aweXpect.Settings().DefaultCheckInterval</c>.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="interval" /> is not positive.</exception>
	/// <exception cref="InvalidOperationException">An interval is already set.</exception>
	public EventuallySubject<T> CheckEvery(TimeSpan interval)
	{
		_expectationBuilder.CheckEvery(interval);
		return this;
	}

	/// <summary>
	///     Sets the <paramref name="timeout" /> until the expectations must be met.
	/// </summary>
	/// <remarks>
	///     Defaults to <c>Customize.aweXpect.Settings().DefaultEventuallyTimeout</c>;
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> retries until the expectations are met.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public EventuallySubject<T> Within(TimeSpan timeout)
	{
		_expectationBuilder.Within(timeout);
		return this;
	}

	/// <inheritdoc cref="IThatSubject{T}.Is{TType}" />
	[GuaranteesNotNull]
	public AndOrWhoseResult<TType, IThatSubject<T>> Is<TType>()
		=> new ThatSubject<T>(_expectationBuilder).Is<TType>();

	/// <inheritdoc cref="IThatSubject{T}.IsNot{TType}" />
	[GuaranteesNotNull]
	public AndOrResult<T, IThatSubject<T>> IsNot<TType>()
		=> new ThatSubject<T>(_expectationBuilder).IsNot<TType>();

	/// <inheritdoc cref="IThatSubject{T}.IsExactly{TType}" />
	[GuaranteesNotNull]
	public AndOrWhoseResult<TType, IThatSubject<T>> IsExactly<TType>()
		=> new ThatSubject<T>(_expectationBuilder).IsExactly<TType>();

	/// <inheritdoc cref="IThatSubject{T}.IsNotExactly{TType}" />
	[GuaranteesNotNull]
	public AndOrResult<T, IThatSubject<T>> IsNotExactly<TType>()
		=> new ThatSubject<T>(_expectationBuilder).IsNotExactly<TType>();
}
