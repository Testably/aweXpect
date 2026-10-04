using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     A delegate with value of type <typeparamref name="T" />.
	/// </summary>
	public sealed partial class WithValue<T> : ThatDelegate, IExpectThat<WithValue<T>>
	{
		private readonly Func<CancellationToken, Task<T>>? _subject;
		private readonly Func<T>? _synchronousSubject;

		/// <summary>
		///     A delegate with value of type <typeparamref name="T" />.
		/// </summary>
		/// <param name="expectationBuilder">The builder for the expectations on the delegate.</param>
		/// <param name="subject">The delegate, which <see cref="Eventually()" /> invokes again on every attempt.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="subject" /> is <see langword="null" />.</exception>
		public WithValue(ExpectationBuilder expectationBuilder, Func<CancellationToken, Task<T>> subject)
			: base(expectationBuilder)
		{
			subject.ThrowIfNull();
			_subject = subject;
		}

		private WithValue(ExpectationBuilder expectationBuilder)
			: base(expectationBuilder)
		{
		}

		private WithValue(ExpectationBuilder expectationBuilder, Func<T> synchronousSubject)
			: base(expectationBuilder)
		{
			_synchronousSubject = synchronousSubject;
		}

		/// <summary>
		///     Creates the subject also for a <see langword="null" /> <paramref name="subject" />, which the expectations
		///     report as <c>&lt;null&gt;</c> instead of throwing.
		/// </summary>
		internal static WithValue<T> Create(ExpectationBuilder expectationBuilder,
			Func<CancellationToken, Task<T>>? subject)
			=> subject is null ? new WithValue<T>(expectationBuilder) : new WithValue<T>(expectationBuilder, subject);

		/// <inheritdoc cref="Create(ExpectationBuilder, Func{CancellationToken, Task{T}})" />
		/// <remarks>
		///     The asynchronous delegate for <see cref="Eventually()" /> is only created when it is used.
		/// </remarks>
		internal static WithValue<T> Create(ExpectationBuilder expectationBuilder, Func<T>? subject)
			=> subject is null ? new WithValue<T>(expectationBuilder) : new WithValue<T>(expectationBuilder, subject);

		/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
		ExpectationBuilder IExpectThat<WithValue<T>>.ExpectationBuilder => _expectationBuilder;

		/// <summary>
		///     Specify expectations that the delegate must eventually satisfy.
		/// </summary>
		/// <remarks>
		///     The delegate is re-evaluated in the interval configured in
		///     <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> until the expectations are met or the timeout
		///     expires.<br />
		///     The timeout defaults to <c>Customize.aweXpect.Settings().DefaultEventuallyTimeout</c> and can be
		///     overwritten per expectation with <see cref="EventuallySubject{T}.Within(TimeSpan)" />, the interval with
		///     <see cref="EventuallySubject{T}.CheckEvery(TimeSpan)" />.<br />
		///     An exception thrown by the delegate counts as an unmet expectation and is retried.<br />
		///     An asynchronous attempt that is still running when the timeout is used up is abandoned and its
		///     <see cref="CancellationToken" /> canceled, and the expectation fails with <c>did not finish within …</c>;
		///     the last attempt, made when the timeout is used up, still gets one check interval (at most the timeout) to
		///     finish.<br />
		///     When the expectation is canceled before the timeout expires, it is reported as inconclusive.
		/// </remarks>
		public EventuallySubject<T> Eventually()
			=> new(new EventuallyExpectationBuilder<T>(_subject ?? ToAsync(_synchronousSubject),
				_expectationBuilder.Subject));

		private static Func<CancellationToken, Task<T>>? ToAsync(Func<T>? subject)
			=> subject is null ? null : _ => Task.FromResult(subject());
	}
}
