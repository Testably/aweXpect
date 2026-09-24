using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Sources;
using aweXpect.Delegates;

namespace aweXpect;

/// <summary>
///     The starting point for checking expectations.
/// </summary>
public static class Expect
{
	/// <summary>
	///     Specifies expectations for the current <paramref name="subject" />.
	/// </summary>
	public static IThatSubject<T> That<T>(T subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<T>(new ExpectationBuilder<T>(
			new ValueSource<T>(subject), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current <paramref name="subject" />.
	/// </summary>
	public static IThatSubject<T[]?> That<T>(T[]? subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<T[]?>(new ExpectationBuilder<T[]?>(
			new ValueSource<T[]?>(subject), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current asynchronous <paramref name="subject" />.
	/// </summary>
	public static IThatSubject<T> That<T>(Task<T> subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<T>(new ExpectationBuilder<T>(
			new AsyncValueSource<T>(subject), doNotPopulateThisValue));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <paramref name="subject" />.
	/// </summary>
	public static IThat<SpanWrapper<T>> That<T>(ReadOnlySpan<T> subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<SpanWrapper<T>>(new ExpectationBuilder<SpanWrapper<T>>(
			new ValueSource<SpanWrapper<T>>(new SpanWrapper<T>(subject)), doNotPopulateThisValue));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <paramref name="subject" />.
	/// </summary>
	public static IThat<SpanWrapper<T>> That<T>(Span<T> subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<SpanWrapper<T>>(new ExpectationBuilder<SpanWrapper<T>>(
			new ValueSource<SpanWrapper<T>>(new SpanWrapper<T>(subject)), doNotPopulateThisValue));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current asynchronous <paramref name="subject" />.
	/// </summary>
	public static IThatSubject<T> That<T>(ValueTask<T> subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new ThatSubject<T>(new ExpectationBuilder<T>(
			new AsyncValueSource<T>(subject.AsTask()), doNotPopulateThisValue));
#endif

	/// <summary>
	///     Specifies expectations for the current <see cref="Action" /> <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithoutValue That(Action @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			new DelegateSource(@delegate), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current <see cref="Action{CancellationToken}" /> <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithoutValue That(
		Action<CancellationToken> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			new DelegateSource(@delegate), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{Task}" /> <paramref name="delegate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	[OverloadResolutionPriority(1)]
#endif
	public static ThatDelegate.WithoutValue That(Func<Task> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			new DelegateAsyncSource(@delegate is null ? null : _ => @delegate()), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{CancellationToken, Task}" /> <paramref name="delegate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	[OverloadResolutionPriority(1)]
#endif
	public static ThatDelegate.WithoutValue That(Func<CancellationToken, Task> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			new DelegateAsyncSource(@delegate), doNotPopulateThisValue));

	/// <summary>
	///     Specifies expectations for the current <see cref="Task" /> <paramref name="subject" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="subject" /> is already running, so an expectation on its execution time only measures the
	///     time that remains when the expectation is verified.
	/// </remarks>
	public static ThatDelegate.WithoutValue That(Task subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			new DelegateAsyncSource(subject is null ? null : _ => subject), doNotPopulateThisValue));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <see cref="Func{ValueTask}" /> <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithoutValue That(Func<ValueTask> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			new DelegateAsyncSource(@delegate is null ? null : _ => @delegate().AsTask()), doNotPopulateThisValue));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <see cref="Func{CancellationToken, ValueTask}" />
	///     <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithoutValue That(Func<CancellationToken, ValueTask> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue>(
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			new DelegateAsyncSource(@delegate is null ? null : token => @delegate(token).AsTask()),
			doNotPopulateThisValue));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <see cref="ValueTask" /> <paramref name="subject" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="subject" /> is already running, so an expectation on its execution time only measures the
	///     time that remains when the expectation is verified.<br />
	///     It is consumed here, because a <see cref="ValueTask" /> may only be awaited once.
	/// </remarks>
	public static ThatDelegate.WithoutValue That(ValueTask subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> That(subject.AsTask(), doNotPopulateThisValue);
#endif

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{TValue}" /> <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithValue<TValue> That<TValue>(Func<TValue> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				new DelegateValueSource<TValue>(@delegate is null ? null : _ => @delegate()), doNotPopulateThisValue),
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			@delegate is null ? null : _ => Task.FromResult(@delegate()));

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{CancellationToken, TValue}" /> <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithValue<TValue> That<TValue>(Func<CancellationToken, TValue> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				new DelegateValueSource<TValue>(@delegate), doNotPopulateThisValue),
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			@delegate is null ? null : token => Task.FromResult(@delegate(token)));

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{T}" /> of <see cref="Task{TValue}" />
	///     <paramref name="delegate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	[OverloadResolutionPriority(1)]
#endif
	public static ThatDelegate.WithValue<TValue> That<TValue>(Func<Task<TValue>> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				new DelegateAsyncValueSource<TValue>(@delegate is null ? null : _ => @delegate()),
				doNotPopulateThisValue),
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			@delegate is null ? null : _ => @delegate());

	/// <summary>
	///     Specifies expectations for the current <see cref="Func{CancellationToken, T}" /> of <see cref="Task{TValue}" />
	///     <paramref name="delegate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	[OverloadResolutionPriority(1)]
#endif
	public static ThatDelegate.WithValue<TValue> That<TValue>(
		Func<CancellationToken, Task<TValue>> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				new DelegateAsyncValueSource<TValue>(@delegate), doNotPopulateThisValue),
			@delegate);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <see cref="Func{T}" /> of <see cref="ValueTask{TValue}" />
	///     <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithValue<TValue> That<TValue>(Func<ValueTask<TValue>> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				new DelegateAsyncValueSource<TValue>(@delegate is null ? null : _ => @delegate().AsTask()),
				doNotPopulateThisValue),
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			@delegate is null ? null : _ => @delegate().AsTask());
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Specifies expectations for the current <see cref="Func{CancellationToken, T}" /> of <see cref="ValueTask{TValue}" />
	///     <paramref name="delegate" />.
	/// </summary>
	public static ThatDelegate.WithValue<TValue> That<TValue>(
		Func<CancellationToken, ValueTask<TValue>> @delegate,
		[CallerArgumentExpression("delegate")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<DelegateValue<TValue>>(
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				new DelegateAsyncValueSource<TValue>(@delegate is null ? null : token => @delegate(token).AsTask()),
				doNotPopulateThisValue),
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			@delegate is null ? null : token => @delegate(token).AsTask());
#endif

	/// <summary>
	///     Specifies expectations for the current boolean <paramref name="subject" />.
	/// </summary>
	public static ThatBoolSubject That(bool subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		=> new(new ExpectationBuilder<bool>(
			new ValueSource<bool>(subject), doNotPopulateThisValue));

	/// <summary>
	///     Verifies that all provided <paramref name="expectations" /> are met.
	/// </summary>
	public static Expectation.Combination.All ThatAll(params Expectation[] expectations)
		=> new(expectations);

	/// <summary>
	///     Verifies that any of the provided <paramref name="expectations" /> are met.
	/// </summary>
	public static Expectation.Combination.Any ThatAny(params Expectation[] expectations)
		=> new(expectations);
}
