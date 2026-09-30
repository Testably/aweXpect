using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Sources;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     Verifies that the delegate throws exactly an exception of type <typeparamref name="TException" />.
	/// </summary>
	[GuaranteesNotNull]
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
	///     Verifies that the delegate throws exactly an exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<Exception> ThrowsExactly(Type type)
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<Exception>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateIsNotNullWithinTimeoutConstraint(it, grammars, throwOptions))
				.ForWhich<DelegateValue, Exception?>(d => d.Exception)
				.AddConstraint((it, grammars) => new ThrowsExactlyConstraint(it, grammars, type, throwOptions))
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

			bool isExpectedType = exceptionType == value?.GetType();
			// Chained expectations on a missing exception or one of another type are irrelevant.
			FurtherProcessingStrategy = isExpectedType
				? FurtherProcessingStrategy.Continue
				: FurtherProcessingStrategy.IgnoreResult;
			Outcome = isExpectedType ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendThrowsExpectation(stringBuilder, throwOptions, exceptionType, true);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (throwOptions.DoCheckThrow && _actual is null)
			{
				stringBuilder.Append(it).Append(" did not throw any exception");
			}
			else
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(_actual, indentation));
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
			throwOptions.IsNegated = !throwOptions.IsNegated;
			Outcome = Outcome == Outcome.Success ? Outcome.Failure : Outcome.Success;
			return this;
		}
	}
}
