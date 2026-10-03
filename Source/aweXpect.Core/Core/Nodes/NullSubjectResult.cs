using System;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose member could not be accessed, because the subject was <see langword="null" />, or
///     whose value is missing, because the subject or the member was a <see langword="null" /> task.
/// </summary>
/// <remarks>
///     It fails the expectation and its negation alike, like <see cref="ConstraintResult.WithNotNullValue{T}" />: the
///     expectations on the member were never evaluated, so negating them does not make them true. The combined
///     results of the nodes negate it like their other operands, so that their negation keeps the failure as well.
/// </remarks>
internal sealed class NullSubjectResult : ConstraintResult
{
	private readonly ConstraintResult _inner;
	private readonly string _result;
	private readonly object? _value;
	private readonly Type _valueType;

	private NullSubjectResult(ConstraintResult inner, object? value, Type valueType, string result)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_value = value;
		_valueType = valueType;
		_result = result;
		Outcome = Outcome.FailureBothWays;
	}


	/// <inheritdoc />
	public override Exception? FailureCause => _inner.FailureCause;

	/// <summary>
	///     Creates a <see cref="NullSubjectResult" /> which uses the <paramref name="inner" /> result for the expectation
	///     text.
	/// </summary>
	/// <remarks>
	///     The <paramref name="it" /> names the subject that was <see langword="null" />, e.g. the member a nested
	///     member is accessed on, in the number of the <paramref name="grammars" />.
	/// </remarks>
	public static NullSubjectResult Create<T>(ConstraintResult inner, T value, string it = "it",
		ExpectationGrammars grammars = ExpectationGrammars.None)
	{
		StringBuilder result = new();
		result.ItWasNull(it, grammars);
		return new NullSubjectResult(inner, value, typeof(T), result.ToString());
	}

	/// <summary>
	///     Creates a <see cref="NullSubjectResult" /> for the <paramref name="member" /> that returned a
	///     <see langword="null" /> task, which uses the <paramref name="inner" /> result for the expectation text.
	/// </summary>
	public static NullSubjectResult CreateForNullTask<T>(ConstraintResult inner, string member, T value)
		=> new(inner, value, typeof(T), $"{member} returned <null> instead of a task");

	/// <summary>
	///     Creates a <see cref="NullSubjectResult" /> for a subject that is a <see langword="null" /> task, which uses the
	///     <paramref name="inner" /> result for the expectation text.
	/// </summary>
	public static NullSubjectResult CreateForNullTaskSubject<T>(ConstraintResult inner, T value)
		=> new(inner, value, typeof(T), "it was a <null> task");

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_result);

	/// <inheritdoc />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
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
