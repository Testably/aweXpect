using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Core.Nodes;

internal class ExpectationNode : Node
{
	private Func<ConstraintResult?, ConstraintResult, ConstraintResult>? _combineResults;
	private IConstraint? _constraint;

	private Node? _inner;

	private List<IBecauseReason>? _reasons;

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
					"You have to specify how to combine the expectations. Use `And()` or `Or()` in between adding expectations."));
		}
	}

	/// <inheritdoc />
	public override Node AddMapping(MappingNode mappingNode)
	{
		_inner = mappingNode;
		_combineResults = mappingNode.CombineResults;
		return mappingNode;
	}

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> throw Tracing.WriteException(
			new NotSupportedException(
				$"Don't specify the inner node for Expectation nodes directly. Use {nameof(AddMapping)}() instead."));

	/// <summary>
	///     Indicates, if the node is empty.
	/// </summary>
	public bool IsEmpty() => _constraint is null && _inner is null;

	/// <summary>
	///     Sets the <paramref name="node" /> which contains the nested expectations.
	/// </summary>
	protected void SetInnerNode(Node node) => _inner = node;

	/// <inheritdoc />
	/// <remarks>
	///     A node with a single constraint returns its result without starting a state machine, when the constraint
	///     completes synchronously, because most expectations are of this kind.
	/// </remarks>
	public override ValueTask<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		if (_inner is null && _reasons is null && context is not ExpectationTextEvaluationContext)
		{
			try
			{
				if (_constraint is IValueConstraint<TValue?> valueConstraint)
				{
					return new ValueTask<ConstraintResult>(valueConstraint.IsMetBy(value));
				}

				if (_constraint is IContextConstraint<TValue?> contextConstraint)
				{
					return new ValueTask<ConstraintResult>(contextConstraint.IsMetBy(value, context));
				}

				if (_constraint is IAsyncConstraint<TValue?> asyncConstraint)
				{
					ValueTask<ConstraintResult> isMet = asyncConstraint.IsMetBy(value, cancellationToken);
					return isMet.IsCompletedSuccessfully
						? isMet
						: AwaitConstraint(isMet, value, context, cancellationToken);
				}

				if (_constraint is IAsyncContextConstraint<TValue?> asyncContextConstraint)
				{
					ValueTask<ConstraintResult> isMet = asyncContextConstraint.IsMetBy(value, context, cancellationToken);
					return isMet.IsCompletedSuccessfully
						? isMet
						: AwaitConstraint(isMet, value, context, cancellationToken);
				}
			}
			catch (UserCodeException e) when (!MemberExceptionResult.IsCancellationOf(e.Exception, cancellationToken))
			{
				return new ValueTask<ConstraintResult>(FromUserCodeException(e, value, context, cancellationToken));
			}
			catch (UserCodeException e)
			{
				ExceptionDispatchInfo.Capture(e.Exception).Throw();
			}
			catch (UnansweredItemException e)
			{
				return new ValueTask<ConstraintResult>(
					FromUnansweredItemException(e, value, context, cancellationToken));
			}
		}

		return IsMetByAsync(value, context, cancellationToken);
	}

	/// <summary>
	///     Awaits the constraint of the fast path in <see cref="IsMetBy{TValue}" /> that did not complete synchronously,
	///     and handles the exceptions of the caller's code like <see cref="IsMetByAsync{TValue}" />.
	/// </summary>
	private async ValueTask<ConstraintResult> AwaitConstraint<TValue>(ValueTask<ConstraintResult> isMet,
		TValue? value, IEvaluationContext context, CancellationToken cancellationToken)
	{
		try
		{
			return await isMet;
		}
		catch (UserCodeException e) when (!MemberExceptionResult.IsCancellationOf(e.Exception, cancellationToken))
		{
			return await FromUserCodeException(e, value, context, cancellationToken);
		}
		catch (UserCodeException e)
		{
			ExceptionDispatchInfo.Capture(e.Exception).Throw();
			throw;
		}
		catch (UnansweredItemException e)
		{
			return await FromUnansweredItemException(e, value, context, cancellationToken);
		}
	}

	private async ValueTask<ConstraintResult> IsMetByAsync<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
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
			else if (_constraint is not null && value is DelegateValue { Exception: { } exception, })
			{
				result = MemberExceptionResult.Create(await GetExpectationResult(_constraint, context, cancellationToken),
					exception, "it", value);
			}
		}
		catch (UserCodeException e) when (!MemberExceptionResult.IsCancellationOf(e.Exception, cancellationToken))
		{
			result = await FromUserCodeException(e, value, context, cancellationToken);
		}
		catch (UserCodeException e)
		{
			ExceptionDispatchInfo.Capture(e.Exception).Throw();
		}
		catch (UnansweredItemException e)
		{
			result = await FromUnansweredItemException(e, value, context, cancellationToken);
		}

		if (_inner != null)
		{
			ConstraintResult innerResult = await _inner.IsMetBy(value, context, cancellationToken);
			innerResult = _combineResults?.Invoke(result, innerResult) ?? innerResult;
			return await ApplyReasons(innerResult, context);
		}

		if (result is null)
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					$"The expectation node does not support {Formatter.Format(typeof(TValue))} with value {Formatter.Format(value)}."));
		}

		return await ApplyReasons(result, context);
	}

	/// <summary>
	///     Adds the <paramref name="reasons" /> which were given for the expectations of this node.
	/// </summary>
	/// <remarks>
	///     They follow the expectation of this node, e.g. of a member, instead of the whole expectation.
	/// </remarks>
	internal void AddReasons(IEnumerable<IBecauseReason> reasons)
		=> (_reasons ??= []).AddRange(reasons);

	/// <remarks>
	///     When only the expectation text is evaluated, the reasons that must be awaited are resolved, so that
	///     <see cref="AppendExpectation" /> includes them.
	///     <para />
	///     A <see cref="ValueTask{TResult}" />, because most nodes have no reasons, and then neither a state machine is
	///     started nor anything is allocated.
	/// </remarks>
	private ValueTask<ConstraintResult> ApplyReasons(ConstraintResult result, IEvaluationContext context)
		=> _reasons is null ? new ValueTask<ConstraintResult>(result) : ApplyReasonsAsync(_reasons, result, context);

	private static async ValueTask<ConstraintResult> ApplyReasonsAsync(List<IBecauseReason> reasons,
		ConstraintResult result, IEvaluationContext context)
	{
		foreach (IBecauseReason reason in reasons)
		{
			if (reason is AsyncBecauseReason asyncReason && context is ExpectationTextEvaluationContext)
			{
				await asyncReason.Resolve();
			}

			result = await reason.ApplyTo(result);
		}

		return result;
	}

	/// <summary>
	///     The result of a constraint whose evaluation code of the caller ended, naming the item when the code was
	///     evaluated for one item of a collection.
	/// </summary>
	private async Task<ConstraintResult> FromUserCodeException<TValue>(UserCodeException exception, TValue? value,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		ConstraintResult expectation = await GetExpectationResult(_constraint!, context, cancellationToken);
		ConstraintResult result =
			MemberExceptionResult.FromEvaluation(expectation, exception.Exception, exception.Thrower ?? "it",
				value);
		return exception.ItemIndex is null
			? result
			: UnansweredItemResult.Create(expectation, result, null, exception.ItemIndex, value);
	}

	/// <summary>
	///     The result of a constraint that could not answer an item of a collection.
	/// </summary>
	private async Task<ConstraintResult> FromUnansweredItemException<TValue>(UnansweredItemException exception,
		TValue? value, IEvaluationContext context, CancellationToken cancellationToken)
		=> UnansweredItemResult.Create(
			await GetExpectationResult(_constraint!, context, cancellationToken), exception.ItemResult, exception.Item,
			exception.Index, value);

	/// <summary>
	///     The expectation of the <paramref name="constraint" />, for when it could not be evaluated, because code of the
	///     caller threw.
	/// </summary>
	private static async Task<ConstraintResult> GetExpectationResult(IConstraint constraint,
		IEvaluationContext context, CancellationToken cancellationToken)
		=> constraint switch
		{
			IExpectationTextConstraint expectationTextConstraint
				=> await expectationTextConstraint.GetExpectationResult(
					ExpectationTextEvaluationContext.For(context), cancellationToken),
			ConstraintResult constraintResult => constraintResult,
			_ => new ConstraintExpectationResult(constraint),
		};

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		_constraint?.AppendExpectation(stringBuilder, indentation);
		_inner?.AppendExpectation(stringBuilder, indentation);
		foreach (IBecauseReason reason in _reasons ?? [])
		{
			stringBuilder.Append(reason);
		}
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

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}
}
