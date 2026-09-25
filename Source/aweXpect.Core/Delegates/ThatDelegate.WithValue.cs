using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     A delegate with value of type <typeparamref name="T" />.
	/// </summary>
	public sealed partial class WithValue<T> : ThatDelegate, IExpectThat<WithValue<T>>
	{
		private readonly Func<CancellationToken, Task<T>>? _subject;

		/// <summary>
		///     A delegate with value of type <typeparamref name="T" />.
		/// </summary>
		public WithValue(ExpectationBuilder expectationBuilder)
			: base(expectationBuilder)
		{
		}

		internal WithValue(ExpectationBuilder expectationBuilder, Func<CancellationToken, Task<T>>? subject)
			: base(expectationBuilder)
			=> _subject = subject;

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
			=> new(new EventuallyExpectationBuilder<T>(_subject, ExpectationBuilder.Subject));
	}
}
