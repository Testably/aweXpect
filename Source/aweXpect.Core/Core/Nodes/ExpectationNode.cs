using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Nodes;

internal class ExpectationNode : Node
{
	private Func<ConstraintResult?, ConstraintResult, ConstraintResult>? _combineResults;
	private IConstraint? _constraint;

	private Node? _inner;

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint)
	{
		if (_inner is not null)
		{
			_inner.AddConstraint(constraint);
		}
		else if (_constraint is null)
		{
			_constraint = constraint;
		}
		else
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					"You have to specify how to combine the expectations! Use `And()` or `Or()` in between adding expectations."));
		}
	}

	/// <inheritdoc />
	public override Node AddMapping<TValue, TTarget>(MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
	{
		MappingNode<TValue, TTarget> mappingNode =
			new(memberAccessor, expectationTextGenerator);
		_inner = mappingNode;
		_combineResults = mappingNode.CombineResults;
		return mappingNode;
	}

	/// <inheritdoc />
	public override Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
	{
		NarrowingMappingNode<TValue, TTarget, TNarrowed> mappingNode =
			new(memberAccessor, expectationTextGenerator);
		_inner = mappingNode;
		_combineResults = mappingNode.CombineResults;
		return mappingNode;
	}

	/// <inheritdoc />
	public override Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
	{
		AsyncMappingNode<TValue, TTarget> mappingNode =
			new(memberAccessor, expectationTextGenerator);
		_inner = mappingNode;
		_combineResults = mappingNode.CombineResults;
		return mappingNode;
	}

	/// <inheritdoc />
	public override Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
	{
		NarrowingAsyncMappingNode<TValue, TTarget, TNarrowed> mappingNode =
			new(memberAccessor, expectationTextGenerator);
		_inner = mappingNode;
		_combineResults = mappingNode.CombineResults;
		return mappingNode;
	}

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> throw Tracing.WriteException(
			new NotSupportedException(
				$"Don't specify the inner node for Expectation nodes directly. Use {nameof(AddMapping)}() instead!"));

	/// <summary>
	///     Indicates, if the node is empty.
	/// </summary>
	public bool IsEmpty() => _constraint is null && _inner is null;

	/// <summary>
	///     Sets the <paramref name="node" /> which contains the nested expectations.
	/// </summary>
	protected void SetInnerNode(Node node) => _inner = node;

	/// <inheritdoc />
	public override async Task<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ConstraintResult? result = null;
		try
		{
			if (context is ExpectationTextEvaluationContext)
			{
				result = _constraint switch
				{
					null => null,
					IExpectationTextConstraint expectationTextConstraint
						=> await expectationTextConstraint.GetExpectationResult(context, cancellationToken),
					ConstraintResult constraintResult => constraintResult,
					_ => new ConstraintExpectationResult(_constraint),
				};
			}
			else if (_constraint is IValueConstraint<TValue?> valueConstraint)
			{
				result = valueConstraint.IsMetBy(value);
			}
			else if (_constraint is IContextConstraint<TValue?> contextConstraint)
			{
				result = contextConstraint.IsMetBy(value, context);
			}
			else if (_constraint is IAsyncConstraint<TValue?> asyncConstraint)
			{
				result = await asyncConstraint.IsMetBy(value, cancellationToken);
			}
			else if (_constraint is IAsyncContextConstraint<TValue?> asyncContextConstraint)
			{
				result = await asyncContextConstraint.IsMetBy(value, context, cancellationToken);
			}
		}
		catch (Exception e) when (e is not ArgumentException && _constraint is not null)
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					$"Error evaluating {Formatter.Format(_constraint.GetType())} constraint with value {Formatter.Format(value)}: {e.Message}",
					e));
		}

		if (_inner != null)
		{
			ConstraintResult innerResult = await _inner.IsMetBy(value, context, cancellationToken);
			innerResult = _combineResults?.Invoke(result, innerResult) ?? innerResult;
			return innerResult;
		}

		return result ?? throw Tracing.WriteException(
			new InvalidOperationException(
				$"The expectation node does not support {Formatter.Format(typeof(TValue))} with value {Formatter.Format(value)}"));
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		_constraint?.AppendExpectation(stringBuilder, indentation);
		_inner?.AppendExpectation(stringBuilder, indentation);
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is ExpectationNode other && Equals(other);

	private bool Equals(ExpectationNode other)
	{
		if (_constraint is null && other._constraint is null)
		{
			return _inner?.Equals(other._inner) != false;
		}

		if (_constraint is null || other._constraint is null)
		{
			return false;
		}

		StringBuilder sb1 = new();
		StringBuilder sb2 = new();
		_constraint.AppendExpectation(sb1);
		other._constraint.AppendExpectation(sb2);
		return sb1.ToString() == sb2.ToString() && _inner?.Equals(other._inner) != false;
	}

	/// <inheritdoc cref="object.GetHashCode()" />
	// ReSharper disable NonReadonlyMemberInGetHashCode
#pragma warning disable S2328 // The node is built up incrementally, so the hash code can only be based on the mutable state
	public override int GetHashCode()
		=> _constraint?.GetType().GetHashCode() ?? 17
			+ _inner?.GetHashCode() ?? 0;
#pragma warning restore S2328
	// ReSharper restore NonReadonlyMemberInGetHashCode

	/// <summary>
	///     The expectation of a <paramref name="constraint" /> which is not a <see cref="ConstraintResult" /> itself.
	/// </summary>
	private sealed class ConstraintExpectationResult(IConstraint constraint)
		: ConstraintResult(ExpectationGrammars.None)
	{
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> constraint.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// The constraint was not evaluated, so there is no result.
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}
}
