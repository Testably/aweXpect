using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithoutValue
	{
		/// <summary>
		///     Verifies that the delegate executes in…
		/// </summary>
		/// <remarks>
		///     A delegate that throws an exception fails the expectation, however fast it did so,
		///     unless <c>AllowingExceptions()</c> is specified.
		///     <para />
		///     An upper bound is applied as timeout (a tighter timeout, e.g. from <c>WithTimeout(…)</c>, still applies),
		///     so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is canceled once it
		///     elapsed. The task of an asynchronous delegate is abandoned at that point, even if it ignores the
		///     cancellation, while a synchronous delegate cannot be interrupted and runs to completion.
		///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
		/// </remarks>
		[GuaranteesNotNull]
		public ExecutesInResult<AndResult<WithoutValue>> ExecutesIn()
		{
			ExecutionTimeOptions options = new();
			options.OnUpperBound(ExpectationBuilder.WithTimeout);
			return new ExecutesInResult<AndResult<WithoutValue>>(
				new AndResult<WithoutValue>(ExpectationBuilder.AddConstraint(options,
						static (executionTimeOptions, it, grammars)
							=> new ExecutesInConstraint(it, grammars, executionTimeOptions, null)),
					this),
				options);
		}

		/// <summary>
		///     Verifies that the delegate executes in approximately the <paramref name="expected" /> time…
		/// </summary>
		/// <remarks>
		///     A delegate that throws an exception fails the expectation, however fast it did so,
		///     unless <c>AllowingExceptions()</c> is specified.
		///     <para />
		///     The <paramref name="expected" /> time plus the tolerance is applied as timeout (a tighter timeout, e.g.
		///     from <c>WithTimeout(…)</c>, still applies), so that a delegate accepting a
		///     <see cref="System.Threading.CancellationToken" /> is canceled once it elapsed. The task of an
		///     asynchronous delegate is abandoned at that point, even if it ignores the cancellation, while a synchronous
		///     delegate cannot be interrupted and runs to completion.
		///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
		/// </remarks>
		[GuaranteesNotNull]
		public ExecutesInToleranceResult<AndResult<WithoutValue>> ExecutesIn(TimeSpan expected)
		{
			ThrowHelper.ThrowIfDurationIsNegative(expected, "expected duration");
			ExecutionTimeOptions options = new();
			options.OnUpperBound(ExpectationBuilder.WithTimeout);
			return new ExecutesInToleranceResult<AndResult<WithoutValue>>(
				new AndResult<WithoutValue>(ExpectationBuilder.AddConstraint(options,
						static (executionTimeOptions, it, grammars)
							=> new ExecutesInConstraint(it, grammars, executionTimeOptions, null)),
					this),
				options,
				expected);
		}
	}
}
