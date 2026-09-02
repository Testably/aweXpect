using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Sources;
using aweXpect.Delegates;

namespace aweXpect.Results;

/// <summary>
///     Result for a delegate with a value that does not throw.
/// </summary>
public class DelegateWithValueResult<T>(ExpectationBuilder expectationBuilder)
	: ExpectationResult<T>(expectationBuilder)
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Returns the result returned from the delegate.
	/// </summary>
	public IThat<T> AndWhoseResult
	{
		get
		{
			_expectationBuilder.And("")
				.AddConstraint((it, grammars) => new DoesNotThrowAnyExceptionConstraint(it, grammars))
				.ForWhich<DelegateValue<T>, T?>(d => d.Value, " and whose result ", "the result");
			return new ThatSubject<T?>(_expectationBuilder);
		}
	}

	private sealed class DoesNotThrowAnyExceptionConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue<T>>
	{
		private DelegateValue<T>? _actual;

		public override Exception? FailureCause
			=> Outcome == Outcome.Failure ? _actual?.Exception : null;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(DelegateValue<T> value)
		{
			_actual = value;
			if (value.IsNull)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = value.Exception is null
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do not append any expectation
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.Exception is not null)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(ThatDelegate.FormatForMessage(_actual.Exception));
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

		public override ConstraintResult Negate() => this;
	}
}
