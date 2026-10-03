using System;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Delegates;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose member could not be accessed, because accessing it threw an exception.
/// </summary>
/// <remarks>
///     Like <see cref="NullSubjectResult" />, it fails the expectation and its negation alike, as the member was never
///     evaluated.
/// </remarks>
internal sealed class MemberExceptionResult : ConstraintResult
{
	private readonly Exception _exception;
	private readonly bool _explainsWithInner;
	private readonly ConstraintResult _inner;
	private readonly string _member;
	private readonly Exception[]? _otherExceptions;
	private readonly object? _value;
	private readonly Type _valueType;

	private MemberExceptionResult(ConstraintResult inner, Exception exception, string member, object? value,
		Type valueType, Exception[]? otherExceptions, bool explainsWithInner)
		: base(inner.FurtherProcessingStrategy)
	{
		_explainsWithInner = explainsWithInner;
		_otherExceptions = otherExceptions;
		_inner = inner;
		_exception = exception;
		_member = member;
		_value = value;
		_valueType = valueType;
		Outcome = Outcome.FailureBothWays;
	}


	/// <inheritdoc />
	public override Exception FailureCause => _exception;

	/// <summary>
	///     Creates a <see cref="MemberExceptionResult" /> which uses the <paramref name="inner" /> result for the
	///     expectation text, and lists the <paramref name="otherExceptions" /> of a faulted member as context.
	/// </summary>
	public static MemberExceptionResult Create<T>(ConstraintResult inner, Exception exception, string member, T value,
		Exception[]? otherExceptions = null)
		=> new(inner, exception, member, value, typeof(T), otherExceptions, false);

	/// <summary>
	///     Creates a <see cref="MemberExceptionResult" /> for the <paramref name="constraint" /> whose evaluation threw,
	///     which also shows the contexts of what it evaluated until then.
	/// </summary>
	public static MemberExceptionResult FromEvaluation<T>(ConstraintResult constraint, Exception exception,
		string member, T value)
		=> new(constraint, exception, member, value, typeof(T), null, true);

	/// <summary>
	///     Checks if the <paramref name="exception" /> signals the cancellation of the evaluation, which must abort it.
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
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (_explainsWithInner)
		{
			contexts.Visit(_inner);
		}

		if (_otherExceptions is not null)
		{
			contexts.Add(WithOtherExceptions.CreateContext(_otherExceptions));
		}
	}

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		_inner.Negate();
		return this;
	}
}
