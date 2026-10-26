using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Delegates;

namespace aweXpect.Results;

/// <summary>
///     Result for a delegate with a value that does not throw.
/// </summary>
public class DelegateWithValueResult<TValue>(ExpectationBuilder expectationBuilder)
	: ExpectationResult<TValue, DelegateWithValueResult<TValue>>(expectationBuilder)
{
	/// <summary>
	///     Returns the result returned from the delegate.
	/// </summary>
	/// <remarks>
	///     The result only exists when the delegate did not throw, so a negation only applies to the expectations on the
	///     result: "throws an exception or its result is not …".
	/// </remarks>
	public IThatSubject<TValue> WhoseResult => ContinueWithResult(ExpectationBuilder);

	/// <summary>
	///     Continues the <paramref name="expectationBuilder" /> with the result returned from the delegate.
	/// </summary>
	internal static IThatSubject<TValue> ContinueWithResult(ExpectationBuilder expectationBuilder)
	{
		expectationBuilder.And(" and its result ")
			.AddConstraint((it, grammars) => new DoesNotThrowAnyExceptionConstraint(it, grammars))
			.ForWhich<DelegateValue<TValue>, TValue?>(d => d.Value, "", "it", negateMemberOnly: true);
		return new ThatSubject<TValue>(expectationBuilder);
	}

	private sealed class DoesNotThrowAnyExceptionConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue<TValue>>
	{
		private DelegateValue<TValue>? _actual;

		public override Exception? FailureCause
			=> Outcome == Outcome.FailureBothWays ? _actual?.Exception : null;

		/// <inheritdoc />
		/// <remarks>
		///     The negation does not apply to this guard, so a delegate without a result fails it both ways.
		/// </remarks>
		public ConstraintResult IsMetBy(DelegateValue<TValue> value)
		{
			_actual = value;
			Outcome = value.IsNull || value.Exception is not null
				? Outcome.FailureBothWays
				: Outcome.Success;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do not append any expectation
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// Repeats the result of the preceding delegate expectation, so that the combination renders it only once.
			if (_actual?.IsNull == true)
			{
				ThatDelegate.AppendNullResult(stringBuilder, it, _actual);
			}
			else if (_actual?.ExceededTimeout is { } exceededTimeout)
			{
				stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
			}
			else if (_actual?.Exception is not null)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(ThatDelegate.FormatForMessage(_actual.Exception, indentation));
			}
		}

		public override bool TryGetStoredValue<T>(out T? value) where T : default
		{
			if (_actual is { Value: T typedValue, })
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(T).IsAssignableFrom(typeof(TValue));
		}

		protected internal override void NegateInPlace()
		{
			// The negation does not apply to this guard.
		}
	}
}
