using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Sources;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegate
{
	/// <summary>
	///     Verifies that the delegate finishes execution within the given <paramref name="duration" />
	///     without throwing an exception.
	/// </summary>
	/// <remarks>
	///     The <paramref name="duration" /> is applied as timeout (a subsequent <c>WithTimeout(…)</c> overwrites it),
	///     so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is cancelled once it
	///     elapsed. A delegate without such a parameter cannot be interrupted and is awaited to completion,
	///     however long that takes.
	/// </remarks>
	[GuaranteesNotNull]
	public static ExpectationResult<TValue> ExecutesWithin<TValue>(
		this IThat<Delegates.ThatDelegate.WithValue<TValue>> subject,
		TimeSpan duration)
	{
		ThrowHelper.ThrowIfDurationIsNegative(duration);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.WithTimeout(duration);
		return new ExpectationResult<TValue>(expectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint<TValue>(it, grammars, duration)));
	}

	/// <summary>
	///     Verifies that the delegate finishes execution within the given <paramref name="duration" />
	///     without throwing an exception.
	/// </summary>
	/// <remarks>
	///     The <paramref name="duration" /> is applied as timeout (a subsequent <c>WithTimeout(…)</c> overwrites it),
	///     so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is cancelled once it
	///     elapsed. A delegate without such a parameter cannot be interrupted and is awaited to completion,
	///     however long that takes.
	/// </remarks>
	[GuaranteesNotNull]
	public static ExpectationResult ExecutesWithin(
		this IThat<Delegates.ThatDelegate.WithoutValue> subject,
		TimeSpan duration)
	{
		ThrowHelper.ThrowIfDurationIsNegative(duration);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.WithTimeout(duration);
		return new ExpectationResult(expectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint(it, grammars, duration)));
	}

	private sealed class ExecutesWithinConstraint<T>(string it, ExpectationGrammars grammars, TimeSpan duration)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue<T>>
	{
		private DelegateValue<T>? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure ? _actual?.Exception : null;

		public ConstraintResult IsMetBy(DelegateValue<T> actual)
		{
			_actual = actual;
			if (actual.IsNull)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				Outcome = actual.Exception is null && actual.Duration <= duration
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("executes within ");
			Formatter.Format(stringBuilder, duration);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_actual.Exception is OperationCanceledException)
			{
				stringBuilder.Append(it).Append(" was canceled after ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is { } exception)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(exception.FormatForMessage(indentation));
			}
			else
			{
				stringBuilder.Append(it).Append(" took ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
		}

		public override ConstraintResult Negate()
			=> throw Tracing.WriteException(
				new NotSupportedException($"Negation of {nameof(ExecutesWithin)} is not supported."));

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
	}

	private sealed class ExecutesWithinConstraint(string it, ExpectationGrammars grammars, TimeSpan duration)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure ? _actual?.Exception : null;

		public ConstraintResult IsMetBy(DelegateValue actual)
		{
			_actual = actual;
			if (actual.IsNull)
			{
				Outcome = Outcome.Failure;
			}
			else
			{
				Outcome = actual.Exception is null && actual.Duration <= duration
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("executes within ");
			Formatter.Format(stringBuilder, duration);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_actual.Exception is OperationCanceledException)
			{
				stringBuilder.Append(it).Append(" was canceled after ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is { } exception)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(exception.FormatForMessage(indentation));
			}
			else
			{
				stringBuilder.Append(it).Append(" took ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
		}

		public override ConstraintResult Negate()
			=> throw Tracing.WriteException(
				new NotSupportedException($"Negation of {nameof(ExecutesWithin)} is not supported."));

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}
	}
}
