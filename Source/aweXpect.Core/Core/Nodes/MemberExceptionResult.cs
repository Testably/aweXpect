using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Delegates;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose member could not be accessed, because accessing it threw an exception.
/// </summary>
/// <remarks>
///     It fails the expectation and its negation alike, like <see cref="NullSubjectResult" />: the expectations on the
///     member were never evaluated, so negating them does not make them true.
/// </remarks>
internal sealed class MemberExceptionResult : ConstraintResult, IUnevaluatedMemberResult
{
	private readonly Exception _exception;
	private readonly ConstraintResult _inner;
	private readonly string _member;
	private readonly object? _value;
	private readonly Type _valueType;

	private MemberExceptionResult(ConstraintResult inner, Exception exception, string member, object? value,
		Type valueType)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_exception = exception;
		_member = member;
		_value = value;
		_valueType = valueType;
		Outcome = Outcome.Failure;
	}

	/// <inheritdoc />
	public override Exception FailureCause => _exception;

	/// <summary>
	///     Creates a <see cref="MemberExceptionResult" /> which uses the <paramref name="inner" /> result for the
	///     expectation text.
	/// </summary>
	public static MemberExceptionResult Create<T>(ConstraintResult inner, Exception exception, string member, T value)
		=> new(inner, exception, member, value, typeof(T));

	/// <summary>
	///     Checks if the <paramref name="exception" /> only signals that the evaluation of the expectation was cancelled,
	///     which must abort the evaluation instead of failing it.
	/// </summary>
	public static bool IsCancellationOf(Exception exception, CancellationToken cancellationToken)
		=> exception is OperationCanceledException && cancellationToken.IsCancellationRequested;

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_member).Append(" did throw ")
			.Append(ThatDelegate.FormatForMessage(_exception, indentation));

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
