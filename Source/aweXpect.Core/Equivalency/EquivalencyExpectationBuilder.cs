using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Internal;
using aweXpect.Core.Nodes;

namespace aweXpect.Equivalency;

internal class EquivalencyExpectationBuilder<T> : EquivalencyExpectationBuilder
{
	private ConstraintResult? _result;

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append("is ");
		Formatter.Format(sb, typeof(T));
		int typeEnd = sb.Length;
		_result?.AppendExpectation(sb);
		// Without expectations there is at most a reason, which follows the type directly.
		if (sb.Length > typeEnd && sb[typeEnd] != ',')
		{
			sb.Insert(typeEnd, " that ");
		}

		return sb.ToString();
	}

	private protected override async ValueTask<ConstraintResult> IsMetByCore<TValue>(
		TValue value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (value is T typedValue)
		{
			_result = await ApplyReasons(await GetRootNode().IsMetBy(typedValue, context, cancellationToken),
				cancellationToken);
		}
		else if (value is null)
		{
			T? typedDefault = default;
			if (typedDefault is not null)
			{
				_result = await ApplyReasons(new NotMatchingTypesResult(this, typedDefault, null), cancellationToken);
			}
			else
			{
				// ReSharper disable ExpressionIsAlwaysNull
				// typedDefault is used to have the correct generic overload in `IsMetBy`.
				_result = new NotMatchingTypesResult(this, typedDefault,
					await ApplyReasons(await GetRootNode().IsMetBy(typedDefault, context, cancellationToken),
						cancellationToken));
				// ReSharper restore ExpressionIsAlwaysNull
			}
		}
		else
		{
			_result = await ApplyReasons(new NotMatchingTypesResult(this, value, null), cancellationToken);
		}

		return _result;
	}

	internal override bool IsOfExpectedType(object value) => value is T;

	/// <summary>
	///     Appends the reasons to the expectation of the <paramref name="result" />.
	/// </summary>
	/// <remarks>
	///     The difference is described right after the comparison, so a reason that must be awaited is awaited here,
	///     until the <paramref name="cancellationToken" /> of the evaluation is canceled.
	/// </remarks>
	private async ValueTask<ConstraintResult> ApplyReasons(ConstraintResult result,
		CancellationToken cancellationToken)
	{
		if (result.Outcome != Outcome.Success)
		{
			await ResolveReasons(cancellationToken);
		}

		return await ApplyReasons(result);
	}

	private sealed class NotMatchingTypesResult : ConstraintResult
	{
		private readonly EquivalencyExpectationBuilder<T> _builder;
		private readonly ConstraintResult? _inner;
		private readonly object? _value;

		public NotMatchingTypesResult(EquivalencyExpectationBuilder<T> builder, object? value, ConstraintResult? inner)
			: base(FurtherProcessingStrategy.Continue)
		{
			_builder = builder;
			_value = value;
			_inner = inner;
			Outcome = inner?.Outcome ?? Outcome.Failure;
		}

		/// <remarks>
		///     Without an <c>inner</c> result the expectations were not evaluated, so they are described by their nodes.
		/// </remarks>
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_inner is null)
			{
				_builder.AppendExpectation(stringBuilder, indentation);
				return;
			}

			_inner.AppendExpectation(stringBuilder, indentation);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(" was ");
			Formatter.Format(stringBuilder, _value?.GetType());
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

		public override ConstraintResult Negate() => this;
	}
}

/// <summary>
///     An <see cref="ExpectationBuilder" /> used in equivalency comparison checks.
/// </summary>
public abstract class EquivalencyExpectationBuilder : ExpectationBuilder
{
	/// <remarks>
	///     Only aweXpect.Core can derive, because the evaluation relies on members that are not public.
	/// </remarks>
	private protected EquivalencyExpectationBuilder()
	{
	}

	/// <summary>
	///     Appends the expectation of the root node to the <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> GetRootNode().AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Evaluate if the expectations are met by the <paramref name="value" /> as part of the evaluation in the
	///     <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     The expectations are evaluated in a context of their own, which is canceled and measures the time like the
	///     evaluation in the <paramref name="context" />, so its timeout and its cancellation also end them. The
	///     collections they materialize, the reasons they leave pending and the values they store are their own, so
	///     they never reach the <paramref name="context" />: the reasons are resolved when the expectations are not
	///     met, and the collections are released before the returned task completes, also when the evaluation throws.
	/// </remarks>
	public ValueTask<ConstraintResult> IsMetBy<TValue>(
		TValue value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByInOwnContext(value, context, cancellationToken);

	/// <summary>
	///     Evaluates the expectations like <see cref="IsMetBy{TValue}(TValue, IEvaluationContext, CancellationToken)" />,
	///     or on their own, without a timeout, when there is no <paramref name="evaluation" />.
	/// </summary>
	internal async ValueTask<ConstraintResult> IsMetByInOwnContext<TValue>(
		TValue value,
		IEvaluationContext? evaluation,
		CancellationToken cancellationToken)
	{
		EvaluationContext context = EvaluationContext.ForNestedExpectation(evaluation);
		try
		{
			ConstraintResult result = await IsMetByCore(value, context, cancellationToken);
			if (result.Outcome != Outcome.Success)
			{
				await context.ResolvePendingReasons(cancellationToken);
			}

			return result;
		}
		finally
		{
			await context.ReleaseMaterializations();
		}
	}

	private protected abstract ValueTask<ConstraintResult> IsMetByCore<TValue>(
		TValue value,
		IEvaluationContext context,
		CancellationToken cancellationToken);

	internal abstract bool IsOfExpectedType(object value);

	/// <inheritdoc cref="ExpectationBuilder.IsMet(Node, EvaluationContext, ITimeSystem, TimeSpan?, CancellationToken)" />
	internal override ValueTask<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
		=> throw Tracing.WriteException(
			new NotSupportedException($"Use {nameof(IsMetBy)} for EquivalencyExpectationBuilder."));
}
