using System;
using System.Collections;
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
///     <para />
///     A leading "it" of the reason is replaced with the compared string as the constraint calls it, e.g. the member
///     name inside <c>Whose</c>, or with "an item" (a dictionary: "a value") when the constraint verifies a collection.
/// </remarks>
internal sealed class NotComparableResult<T> : ConstraintResult
{
	private const string It = "it";
	private readonly ConstraintResult _inner;
	private readonly string _reason;
	private readonly T _value;

	public NotComparableResult(ConstraintResult inner, string reason, string it, Exception? cause, T value)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_reason = reason.StartsWith(It + " ", StringComparison.Ordinal)
			? GetSubject(it, value) + reason.Substring(It.Length)
			: reason;
		_value = value;
		FailureCause = cause;
		Outcome = Outcome.FailureBothWays;
	}

	private static string GetSubject(string it, T value)
		=> value switch
		{
			IDictionary => it == It ? "a value" : $"a value of {it}",
			IEnumerable and not string => it == It ? "an item" : $"an item of {it}",
			_ => it,
		};

	/// <inheritdoc />
	public override Exception? FailureCause { get; }

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_reason.Indent(indentation, false));

	/// <inheritdoc />
	/// <remarks>
	///     A constraint that could not compare the subject still describes it, like when it fails.
	/// </remarks>
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_inner.TryGetDescribableSubject(out value))
		{
			return true;
		}

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
