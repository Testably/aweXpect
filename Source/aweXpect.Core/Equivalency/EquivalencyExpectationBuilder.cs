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

	public override async ValueTask<ConstraintResult> IsMetBy<TValue>(
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
	///     Evaluate if the expectations are met by the <paramref name="value" />.
	/// </summary>
	public abstract ValueTask<ConstraintResult> IsMetBy<TValue>(
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
