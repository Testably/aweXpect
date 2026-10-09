using System;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose constraint could not compare the subject, because a match type answered
///     <see cref="StringMatchResult.NotComparable(string, Exception)" />.
/// </summary>
/// <remarks>
///     Like <see cref="MemberExceptionResult" />, it fails the expectation and its negation alike, with the reason of the
///     match type as the result.
/// </remarks>
internal sealed class NotComparableResult<T> : ConstraintResult
{
	private readonly ConstraintResult _inner;
	private readonly string _reason;
	private readonly T _value;

	public NotComparableResult(ConstraintResult inner, string reason, Exception? cause, T value)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_reason = reason;
		_value = value;
		FailureCause = cause;
		Outcome = Outcome.FailureBothWays;
	}

	/// <inheritdoc />
	public override Exception? FailureCause { get; }

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_reason.Indent(indentation, false));

	/// <inheritdoc />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_value is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return typeof(TValue).IsAssignableFrom(typeof(T));
	}

	/// <inheritdoc />
	/// <remarks>
	///     The constraint describes what it evaluated until the subject could not be compared, e.g. the actual value.
	/// </remarks>
	public override void AppendContexts(ResultContextCollector contexts)
		=> contexts.Visit(_inner);

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		_inner.Negate();
		return this;
	}
}
