using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose member could not be accessed, because the subject was <see langword="null" />.
/// </summary>
/// <remarks>
///     It fails the expectation and its negation alike, like <see cref="ConstraintResult.WithNotNullValue{T}" />: the
///     expectations on the member were never evaluated, so negating them does not make them true. The combined
///     results of the nodes check for it, so that their negation does not flip the failure either.
/// </remarks>
internal sealed class NullSubjectResult : ConstraintResult, IUnevaluatedMemberResult
{
	private readonly ConstraintResult _inner;
	private readonly object? _value;
	private readonly Type _valueType;

	private NullSubjectResult(ConstraintResult inner, object? value, Type valueType)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_value = value;
		_valueType = valueType;
		Outcome = Outcome.Failure;
	}

	/// <inheritdoc />
	public override Exception? FailureCause => _inner.FailureCause;

	/// <summary>
	///     Creates a <see cref="NullSubjectResult" /> which uses the <paramref name="inner" /> result for the expectation
	///     text.
	/// </summary>
	public static NullSubjectResult Create<T>(ConstraintResult inner, T value)
		=> new(inner, value, typeof(T));

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("it was <null>");

	/// <inheritdoc />
	public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
	{
		if (_value is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return typeof(TValue).IsAssignableFrom(_valueType);
	}

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		_inner.Negate();
		return this;
	}
}
