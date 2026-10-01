using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

public sealed class DummyConstraintResult : ConstraintResult
{
	private readonly string? _expectationText;
	private readonly string? _failureText;
	private readonly object? _value;

	public DummyConstraintResult(Outcome outcome,
		string? expectationText = null,
		string? failureText = null,
		FurtherProcessingStrategy furtherProcessingStrategy = FurtherProcessingStrategy.Continue)
		: base(furtherProcessingStrategy)
	{
		Outcome = outcome;
		_expectationText = expectationText;
		_failureText = failureText;
	}

	public DummyConstraintResult(object value,
		Outcome outcome,
		string? expectationText = null,
		string? failureText = null,
		FurtherProcessingStrategy furtherProcessingStrategy = FurtherProcessingStrategy.Continue)
		: this(outcome, expectationText, failureText, furtherProcessingStrategy)
	{
		_value = value;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_expectationText != null)
		{
			stringBuilder.Append(_expectationText);
		}
	}

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_failureText != null)
		{
			stringBuilder.Append(_failureText);
		}
	}

	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_value is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return false;
	}

	public override ConstraintResult Negate()
	{
		Outcome = Outcome switch
		{
			Outcome.Failure => Outcome.Success,
			Outcome.Success => Outcome.Failure,
			_ => Outcome,
		};
		return this;
	}
}

public sealed class DummyConstraintResult<T> : ConstraintResult
{
	private readonly string? _expectationText;
	private readonly string? _failureText;
	private readonly T _value;

	public DummyConstraintResult(Outcome outcome,
		T value,
		string? expectationText = null,
		string? failureText = null,
		FurtherProcessingStrategy furtherProcessingStrategy = FurtherProcessingStrategy.Continue)
		: base(furtherProcessingStrategy)
	{
		Outcome = outcome;
		_value = value;
		_expectationText = expectationText;
		_failureText = failureText;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_expectationText != null)
		{
			stringBuilder.Append(_expectationText);
		}
	}

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_failureText != null)
		{
			stringBuilder.Append(_failureText);
		}
	}

	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_value is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return false;
	}

	public override ConstraintResult Negate()
	{
		Outcome = Outcome switch
		{
			Outcome.Failure => Outcome.Success,
			Outcome.Success => Outcome.Failure,
			_ => Outcome,
		};
		return this;
	}
}
