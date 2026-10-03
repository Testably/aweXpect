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
	///     Verifies that the delegate throws an exception.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<Exception> Throws()
		=> ThrowsOfType<Exception>(typeof(Exception), false);

	/// <summary>
	///     Verifies that the delegate throws an exception of type <typeparamref name="TException" />.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<TException> Throws<TException>()
		where TException : Exception
		=> ThrowsOfType<TException>(typeof(TException), false);

	/// <summary>
	///     Verifies that the delegate throws an exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<Exception> Throws(Type type)
	{
		type.ThrowIfNotAnExceptionType();
		return ThrowsOfType<Exception>(type, false);
	}

	/// <remarks>
	///     The type check belongs to the delegate, not to the exception, so that it stays in front of a later <c>Or</c>
	///     and the further expectations only see an exception of the expected type.
	/// </remarks>
	private ThatDelegateThrows<TException> ThrowsOfType<TException>(Type exceptionType, bool exactly)
		where TException : Exception
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<TException>(ExpectationBuilder
				.AddConstraint((ExceptionType: exceptionType, Exactly: exactly, ThrowOptions: throwOptions),
					static (state, it, grammars) => new DelegateThrowsWithinTimeoutConstraint(it, grammars,
						state.ExceptionType, state.Exactly, state.ThrowOptions))
				.ForWhich<DelegateValue, TException?>(d
					=> IsExpectedException(d.Exception, exceptionType, exactly) ? d.Exception as TException : null)
				.AddConstraint((_, _) => new DoNothingConstraint<TException>())
				.And(" "),
			throwOptions);
	}

	private static bool IsExpectedException(Exception? exception, Type exceptionType, bool exactly)
		=> exactly ? exception.IsExactlyOfType(exceptionType) : exception.IsOfType(exceptionType);

	private sealed class DoNothingConstraint<T>()
		: ConstraintResult.WithValue<T>("it", ExpectationGrammars.None), IValueConstraint<T>
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

	private sealed class DelegateThrowsWithinTimeoutConstraint(
		string it,
		ExpectationGrammars grammars,
		Type exceptionType,
		bool exactly,
		ThrowsOption options)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;
		private bool _tookTooLong;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> (Outcome is Outcome.Failure or Outcome.FailureBothWays) && !_tookTooLong ? _actual?.Exception : null;

		public ConstraintResult IsMetBy(DelegateValue value)
		{
			if (options.ExecutionTimeOptions is not null)
			{
				value = value.LateResult ?? value;
			}

			_actual = value;
			if (value.IsNull || value.ExceededTimeout is not null)
			{
				Outcome = Outcome.FailureBothWays;
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
			else if (IsExpectedException(value.Exception, exceptionType, exactly))
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendThrowsExpectation(stringBuilder, options, exceptionType, exactly);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				AppendNullResult(stringBuilder, it, _actual);
			}
			else if (_actual.ExceededTimeout is { } exceededTimeout)
			{
				stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
			}
			else if (_tookTooLong)
			{
				stringBuilder.Append(it).Append(" took ");
				options.ExecutionTimeOptions?.AppendFailureResult(stringBuilder, _actual.Duration);
			}
			else if (_actual.Exception is null)
			{
				stringBuilder.Append(it).Append(" did not throw any exception");
			}
			else
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(_actual.Exception, indentation));
			}
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(exceptionType);
		}

		/// <remarks>
		///     A negating expectation, such as <c>DoesNotComplyWith</c>, negates the result after the evaluation, so
		///     the outcome is updated as well. A delegate that is <see langword="null" /> or did not finish fails in both
		///     cases.
		/// </remarks>
		public override ConstraintResult Negate()
		{
			options.IsNegated = !options.IsNegated;
			if (_actual is { IsNull: false, ExceededTimeout: null, })
			{
				Outcome = Outcome == Outcome.Success ? Outcome.Failure : Outcome.Success;
			}

			return this;
		}
	}
}
