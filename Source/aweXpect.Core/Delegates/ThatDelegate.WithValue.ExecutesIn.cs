using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithValue<T>
	{
		/// <summary>
		///     Verifies that the delegate executes in…
		/// </summary>
		/// <remarks>
		///     A delegate that throws an exception fails the expectation, however fast it did so,
		///     unless <c>AllowingExceptions()</c> is specified.
		///     <para />
		///     An upper bound is applied as timeout (a subsequent <c>WithTimeout(…)</c> overwrites it),
		///     so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is canceled once it
		///     elapsed. The task of an asynchronous delegate is abandoned at that point, even if it ignores the
		///     cancellation, while a synchronous delegate cannot be interrupted and runs to completion.
		///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
		/// </remarks>
		[GuaranteesNotNull]
		public ExecutesInResult<AndResult<WithValue<T>>> ExecutesIn()
		{
			ExecutionTimeOptions options = new();
			options.OnUpperBound(ExpectationBuilder.WithTimeout);
			return new ExecutesInResult<AndResult<WithValue<T>>>(
				new AndResult<WithValue<T>>(ExpectationBuilder.AddConstraint((it, grammars)
						=> new ExecutesInConstraint(it, grammars, options)),
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
		///     The <paramref name="expected" /> time plus the tolerance is applied as timeout (a subsequent
		///     <c>WithTimeout(…)</c> overwrites it), so that a delegate accepting a
		///     <see cref="System.Threading.CancellationToken" /> is canceled once it elapsed. The task of an
		///     asynchronous delegate is abandoned at that point, even if it ignores the cancellation, while a synchronous
		///     delegate cannot be interrupted and runs to completion.
		///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
		/// </remarks>
		[GuaranteesNotNull]
		public ExecutesInToleranceResult<AndResult<WithValue<T>>> ExecutesIn(TimeSpan expected)
		{
			ThrowHelper.ThrowIfDurationIsNegative(expected, "expected duration");
			ExecutionTimeOptions options = new();
			options.OnUpperBound(ExpectationBuilder.WithTimeout);
			return new ExecutesInToleranceResult<AndResult<WithValue<T>>>(
				new AndResult<WithValue<T>>(ExpectationBuilder.AddConstraint((it, grammars)
						=> new ExecutesInConstraint(it, grammars, options)),
					this),
				options,
				expected);
		}

		private sealed class ExecutesInConstraint(
			string it,
			ExpectationGrammars grammars,
			ExecutionTimeOptions options)
			: ConstraintResult(grammars),
				IValueConstraint<DelegateValue<T>>
		{
			private DelegateValue<T>? _actual;

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause
				=> Outcome == Outcome.Failure &&
				   (_actual?.ExceededTimeout is not null || !options.AllowsException(_actual?.Exception))
					? _actual?.Exception
					: null;

			/// <inheritdoc />
			public ConstraintResult IsMetBy(DelegateValue<T> value)
			{
				_actual = value;
				if (value.IsNull || value.ExceededTimeout is not null)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				Outcome = options.AllowsException(value.Exception) && options.IsWithinLimit(value.Duration)
					? Outcome.Success
					: Outcome.Failure;
				return this;
			}

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append("executes ");
				options.AppendTo(stringBuilder, "in ");
				if (options.AreExceptionsAllowed)
				{
					stringBuilder.Append(" allowing exceptions");
				}
			}

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_actual?.IsNull != false)
				{
					stringBuilder.ItWasNull(it);
				}
				else if (_actual.ExceededTimeout is { } exceededTimeout)
				{
					stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
				}
				else if (_actual.Exception is { } exception && !options.AreExceptionsAllowed)
				{
					stringBuilder.Append(it).Append(" did throw ");
					stringBuilder.Append(FormatForMessage(exception, indentation));
				}
				else
				{
					stringBuilder.Append(it).Append(" took ");
					options.AppendFailureResult(stringBuilder, _actual.Duration);
					if (_actual.Exception is { } allowedException)
					{
						stringBuilder.Append(" and did throw ");
						stringBuilder.Append(FormatForMessage(allowedException, indentation));
					}
				}
			}

			public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
			{
				if (_actual is { Value: TValue typedValue, })
				{
					value = typedValue;
					return true;
				}

				value = default;
				return typeof(TValue).IsAssignableFrom(typeof(T));
			}

			public override ConstraintResult Negate()
				=> throw Tracing.WriteException(new NotSupportedException($"Negation of {nameof(ExecutesIn)} is not supported."));
		}
	}
}
