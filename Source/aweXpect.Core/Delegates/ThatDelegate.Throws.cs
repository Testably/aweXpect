using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     Verifies that the delegate throws an exception of type <typeparamref name="TException" />.
	/// </summary>
	public ThatDelegateThrows<TException> Throws<TException>()
		where TException : Exception
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<TException>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateThrowsWithinTimeoutConstraint<TException>(it, grammars, throwOptions))
				.ForWhich<DelegateValue, TException?>(d => d.Exception as TException)
				.AddConstraint((_, _) => new DoNothingConstraint<TException>())
				.And(" "),
			throwOptions);
	}

	/// <summary>
	///     Verifies that the delegate throws an exception of type <paramref name="exceptionType" />.
	/// </summary>
	public ThatDelegateThrows<Exception> Throws(Type exceptionType)
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<Exception>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateIsNotNullWithinTimeoutConstraint(it, grammars, throwOptions))
				.ForWhich<DelegateValue, Exception?>(d => d.Exception)
				.AddConstraint((it, grammars) => new ThrowsConstraint(it, grammars, exceptionType, throwOptions))
				.And(" "),
			throwOptions);
	}

	private sealed class DoNothingConstraint<T>()
		: ConstraintResult.WithValue<T>(ExpectationGrammars.None), IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual)
		{
			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do nothing
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do nothing
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do nothing
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do nothing
		}
	}

	private sealed class DelegateThrowsWithinTimeoutConstraint<TException>(
		string it,
		ExpectationGrammars grammars,
		ThrowsOption options)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
		where TException : Exception
	{
		private DelegateValue? _actual;
		private bool _tookTooLong;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure && !_tookTooLong ? _actual?.Exception : null;

		public ConstraintResult IsMetBy(DelegateValue value)
		{
			_actual = value;
			if (value.IsNull)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (options.ExecutionTimeOptions is not null &&
			    !options.ExecutionTimeOptions.IsWithinLimit(value.Duration))
			{
				_tookTooLong = true;
				Outcome = Outcome.Failure;
				return this;
			}

			if (!options.DoCheckThrow)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreCompletely;
				Outcome = value.Exception is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (value.Exception is null)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}
			else if (typeof(TException).IsAssignableFrom(value.Exception.GetType()))
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!options.DoCheckThrow)
			{
				stringBuilder.Append("does not throw any exception");
			}
			else if (typeof(TException) == typeof(Exception))
			{
				stringBuilder.Append("throws an exception");
			}
			else
			{
				stringBuilder.Append("throws ").Append(Formatter.Format(typeof(TException)).PrependAOrAn());
			}

			if (options.ExecutionTimeOptions is not null)
			{
				stringBuilder.Append(' ');
				options.ExecutionTimeOptions.AppendTo(stringBuilder, "in ");
			}
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_tookTooLong)
			{
				stringBuilder.Append(it).Append(" took ");
				options.ExecutionTimeOptions?.AppendFailureResult(stringBuilder, _actual.Duration);
			}
			else if (options.DoCheckThrow && _actual.Exception is null)
			{
				stringBuilder.Append(it).Append(" did not throw any exception");
			}
			else
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(_actual.Exception));
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(TException));
		}

		public override ConstraintResult Negate()
		{
			options.DoCheckThrow = !options.DoCheckThrow;
			return this;
		}
	}

	private sealed class ThrowsConstraint(
		string it,
		ExpectationGrammars grammars,
		Type exceptionType,
		ThrowsOption throwOptions)
		: ConstraintResult(grammars),
			IValueConstraint<Exception?>
	{
		private Exception? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => Outcome == Outcome.Failure ? _actual : null;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(Exception? value)
		{
			_actual = value;

			if (!throwOptions.DoCheckThrow)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreCompletely;
				Outcome = value is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (value is null)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}
			else if (exceptionType.IsAssignableFrom(value.GetType()))
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!throwOptions.DoCheckThrow)
			{
				stringBuilder.Append("does not throw any exception");
			}
			else if (exceptionType == typeof(Exception))
			{
				stringBuilder.Append("throws an exception");
			}
			else
			{
				stringBuilder.Append("throws ").Append(Formatter.Format(exceptionType).PrependAOrAn());
			}

			if (throwOptions.ExecutionTimeOptions is not null)
			{
				stringBuilder.Append(' ');
				throwOptions.ExecutionTimeOptions.AppendTo(stringBuilder, "in ");
			}
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (throwOptions.DoCheckThrow && _actual is null)
			{
				stringBuilder.Append(it).Append(" did not throw any exception");
			}
			else
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(_actual));
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(exceptionType);
		}

		public override ConstraintResult Negate()
		{
			throwOptions.DoCheckThrow = !throwOptions.DoCheckThrow;
			return this;
		}
	}
}
