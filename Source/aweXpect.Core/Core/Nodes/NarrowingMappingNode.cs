using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Nodes;

/// <summary>
///     A <see cref="MappingNode{TSource,TTarget}" /> whose expectations are typed at <typeparamref name="TNarrowed" />
///     instead of the <typeparamref name="TTarget" /> the member is projected as.
/// </summary>
/// <remarks>
///     The expectations are not applied when the member has a different runtime type, because the expectation which
///     narrowed the type already reports the mismatch.
/// </remarks>
internal sealed class NarrowingMappingNode<TSource, TTarget, TNarrowed>(
	MemberAccessor<TSource, TTarget> memberAccessor,
	Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	: MappingNode<TSource, TTarget>(memberAccessor, expectationTextGenerator)
{
	/// <inheritdoc cref="MappingNode{TSource,TTarget}.IsMetByMember(TTarget, IEvaluationContext, CancellationToken)" />
	protected override Task<ConstraintResult> IsMetByMember(TTarget? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (value is TNarrowed narrowedValue)
		{
			return IsMetByExpectations(narrowedValue, context, cancellationToken);
		}

		if (value is null)
		{
			return IsMetByExpectations<TNarrowed>(default, context, cancellationToken);
		}

		return Task.FromResult<ConstraintResult>(new NotApplicableConstraintResult(this));
	}
}

/// <summary>
///     An <see cref="AsyncMappingNode{TSource,TTarget}" /> whose expectations are typed at
///     <typeparamref name="TNarrowed" /> instead of the <typeparamref name="TTarget" /> the member is projected as.
/// </summary>
/// <remarks>
///     The expectations are not applied when the member has a different runtime type, because the expectation which
///     narrowed the type already reports the mismatch.
/// </remarks>
internal sealed class NarrowingAsyncMappingNode<TSource, TTarget, TNarrowed>(
	MemberAccessor<TSource, Task<TTarget>> memberAccessor,
	Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	: AsyncMappingNode<TSource, TTarget>(memberAccessor, expectationTextGenerator)
{
	/// <inheritdoc
	///     cref="AsyncMappingNode{TSource,TTarget}.IsMetByMember(TTarget, IEvaluationContext, CancellationToken)" />
	protected override Task<ConstraintResult> IsMetByMember(TTarget? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (value is TNarrowed narrowedValue)
		{
			return IsMetByExpectations(narrowedValue, context, cancellationToken);
		}

		if (value is null)
		{
			return IsMetByExpectations<TNarrowed>(default, context, cancellationToken);
		}

		return Task.FromResult<ConstraintResult>(new NotApplicableConstraintResult(this));
	}
}
