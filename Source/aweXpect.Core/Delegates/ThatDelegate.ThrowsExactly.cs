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
	///     Verifies that the delegate throws exactly an exception of type <typeparamref name="TException" />.
	/// </summary>
	public ThatDelegateThrows<TException> ThrowsExactly<TException>()
		where TException : Exception
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<TException>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateIsNotNullWithinTimeoutConstraint(it, grammars, throwOptions))
				.ForWhich<DelegateValue, Exception?>(d => d.Exception)
				.AddConstraint((it, grammars)
					=> new ThrowsExactlyConstraint(it, grammars, typeof(TException), throwOptions))
				.And(" "),
			throwOptions);
	}

	/// <summary>
	///     Verifies that the delegate throws exactly an exception of type <paramref name="exceptionType" />.
	/// </summary>
	public ThatDelegateThrows<Exception> ThrowsExactly(Type exceptionType)
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<Exception>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateIsNotNullWithinTimeoutConstraint(it, grammars, throwOptions))
				.ForWhich<DelegateValue, Exception?>(d => d.Exception)
				.AddConstraint((it, grammars) => new ThrowsExactlyConstraint(it, grammars, exceptionType, throwOptions))
				.And(" "),
			throwOptions);
	}

	private sealed class ThrowsExactlyConstraint(
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
			else if (exceptionType == value.GetType())
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
			else
			{
				stringBuilder.Append("throws exactly ").Append(Formatter.Format(exceptionType).PrependAOrAn());
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
