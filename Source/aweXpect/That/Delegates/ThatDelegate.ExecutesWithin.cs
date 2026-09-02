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
	public static ExpectationResult<TValue> ExecutesWithin<TValue>(
		this IThat<Delegates.ThatDelegate.WithValue<TValue>> source,
		TimeSpan duration)
		=> new(source.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint<TValue>(it, grammars, duration)));

	/// <summary>
	///     Verifies that the delegate finishes execution within the given <paramref name="duration" />
	///     without throwing an exception.
	/// </summary>
	public static ExpectationResult ExecutesWithin(
		this IThat<Delegates.ThatDelegate.WithoutValue> source,
		TimeSpan duration)
		=> new(source.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint(it, grammars, duration)));

	/// <summary>
	///     Verifies that the delegate does not finish execution within the given <paramref name="duration" />
	///     or throws an exception.
	/// </summary>
	public static ExpectationResult<TValue> DoesNotExecuteWithin<TValue>(
		this IThat<Delegates.ThatDelegate.WithValue<TValue>> source,
		TimeSpan duration)
		=> new(source.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint<TValue>(it, grammars, duration).Invert()));

	/// <summary>
	///     Verifies that the delegate does not finish execution within the given <paramref name="duration" />
	///     or throws an exception.
	/// </summary>
	public static ExpectationResult DoesNotExecuteWithin(
		this IThat<Delegates.ThatDelegate.WithoutValue> source,
		TimeSpan duration)
		=> new(source.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new ExecutesWithinConstraint(it, grammars, duration).Invert()));

	private sealed class ExecutesWithinConstraint<T>(string it, ExpectationGrammars grammars, TimeSpan duration)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue<T>>
	{
		private DelegateValue<T>? _actual;
		private bool _isNegated;

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
				Outcome = (actual.Exception is not null || actual.Duration > duration) != _isNegated
					? Outcome.Failure
					: Outcome.Success;
			}

			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_isNegated)
			{
				stringBuilder.Append("does not execute within ");
			}
			else
			{
				stringBuilder.Append("executes within ");
			}

			Formatter.Format(stringBuilder, duration);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_isNegated)
			{
				stringBuilder.Append(it).Append(" took only ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is OperationCanceledException)
			{
				stringBuilder.Append(it).Append(" was canceled within ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is { } exception)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(exception.FormatForMessage());
			}
			else
			{
				stringBuilder.Append(it).Append(" took ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			return this;
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
	}

	private sealed class ExecutesWithinConstraint(string it, ExpectationGrammars grammars, TimeSpan duration)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;
		private bool _isNegated;

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
				Outcome = (actual.Exception is not null || actual.Duration > duration) != _isNegated
					? Outcome.Failure
					: Outcome.Success;
			}

			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_isNegated)
			{
				stringBuilder.Append("does not execute within ");
			}
			else
			{
				stringBuilder.Append("executes within ");
			}

			Formatter.Format(stringBuilder, duration);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_isNegated)
			{
				stringBuilder.Append(it).Append(" took only ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is OperationCanceledException)
			{
				stringBuilder.Append(it).Append(" was canceled within ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is { } exception)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(exception.FormatForMessage());
			}
			else
			{
				stringBuilder.Append(it).Append(" took ");
				Formatter.Format(stringBuilder, _actual.Duration);
			}
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			return this;
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}
	}
}
