using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Core.Nodes;

/// <summary>
///     A node that maps the value to one of its members and applies the expectations on the member to it.
/// </summary>
internal abstract class MappingNode : ExpectationNode
{
	/// <summary>
	///     The accessor of the member.
	/// </summary>
	public abstract MemberAccessor MemberAccessor { get; }

	/// <summary>
	///     The name and the grammars of the subject the member is accessed on, which the result names when it is
	///     <see langword="null" />.
	/// </summary>
	public (string It, ExpectationGrammars Grammars) Source { get; set; } = ("it", ExpectationGrammars.None);

	/// <summary>
	///     The name of the member, when the result text refers to the member by it, so that its contexts are labelled
	///     with it.
	/// </summary>
	public string? ContextMember { get; set; }

	/// <summary>
	///     Combines the <paramref name="result" /> of the expectations on the member with the
	///     <paramref name="combinedResult" /> of the expectations on the value, if any.
	/// </summary>
	internal abstract ConstraintResult CombineResults(ConstraintResult? combinedResult, ConstraintResult result);
}

/// <summary>
///     A <see cref="MappingNode" /> for the member of type <typeparamref name="TTarget" />, which is accessed directly
///     or awaited, and whose expectations are typed at <typeparamref name="TNarrowed" />.
/// </summary>
/// <remarks>
///     When <typeparamref name="TNarrowed" /> is narrower than <typeparamref name="TTarget" />, the expectations are
///     not applied to a member with a different runtime type, because the expectation which narrowed the type already
///     reports the mismatch.
/// </remarks>
internal sealed class MappingNode<TSource, TTarget, TNarrowed> : MappingNode
{
	private readonly Action<StringBuilder> _appendMemberText;
	private readonly MemberAccessor<TSource, Task<TTarget>>? _asyncMemberAccessor;
	private readonly MemberAccessor<TSource, TTarget>? _memberAccessor;

	/// <summary>
	///     Maps to the member that the <paramref name="memberAccessor" /> accesses.
	/// </summary>
	public MappingNode(MemberAccessor<TSource, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_memberAccessor = memberAccessor;
		MemberAccessor = memberAccessor;
		_appendMemberText = CreateMemberText(memberAccessor, expectationTextGenerator);
	}

	/// <summary>
	///     Maps to the member that the <paramref name="asyncMemberAccessor" /> accesses and that is awaited.
	/// </summary>
	public MappingNode(MemberAccessor<TSource, Task<TTarget>> asyncMemberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
		MemberAccessor = asyncMemberAccessor;
		_appendMemberText = CreateMemberText(asyncMemberAccessor, expectationTextGenerator);
	}

	/// <inheritdoc />
	public override MemberAccessor MemberAccessor { get; }

	private static Action<StringBuilder> CreateMemberText(MemberAccessor memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator)
	{
		if (expectationTextGenerator is null)
		{
			return stringBuilder => stringBuilder.Append(memberAccessor);
		}

		return stringBuilder => expectationTextGenerator(memberAccessor, stringBuilder);
	}

	/// <inheritdoc />
	public override ValueTask<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
		=> IsMetByContinuing(value, null, context, cancellationToken);

	/// <inheritdoc />
	/// <remarks>
	///     The member is accessed on the value that a met <paramref name="source" /> stores, e.g. after a constraint
	///     that converts the value, and otherwise on the <paramref name="value" /> itself.
	/// </remarks>
	public override ValueTask<ConstraintResult> IsMetByContinuing<TValue>(
		TValue? value,
		ConstraintResult? source,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		if (source is MappingResult mappingResult)
		{
			source = mappingResult.Source;
		}

		if (source is { Outcome: Outcome.Success, } && context is not ExpectationTextEvaluationContext &&
		    source.TryGetStoredValue(out TSource? storedValue))
		{
			return IsMetByValue(storedValue, context, cancellationToken);
		}

		return IsMetByValue(value, context, cancellationToken);
	}

	private async ValueTask<ConstraintResult> IsMetByValue<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (context is ExpectationTextEvaluationContext)
		{
			return await GetExpectationResult(context, cancellationToken);
		}

		if (value is null || value is DelegateValue { IsNull: true, })
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return NullSubjectResult.Create(result, value, Source.It, Source.Grammars);
		}

		if (value is not TSource typedValue)
		{
			// The value only has another type after a failed type check (e.g. `Is<T>()`), which reports the mismatch.
			ConstraintResult expectationResult = await GetExpectationResult(context, cancellationToken);
			return new NotApplicableConstraintResult(expectationResult.AppendExpectation);
		}

		TTarget member = default!;
		Task<TTarget>? memberTask = null;
		try
		{
			if (_asyncMemberAccessor is null)
			{
				member = _memberAccessor!.AccessMember(typedValue);
			}
			else
			{
				memberTask = _asyncMemberAccessor.AccessMember(typedValue);
				if (memberTask is not null)
				{
					member = await memberTask.AbandonOnCancellation(cancellationToken);
				}
			}
		}
		catch (Exception exception) when (!MemberExceptionResult.IsCancellationOf(exception, cancellationToken))
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return MemberExceptionResult.Create(result, exception, MemberAccessor.ToString().Trim(), value,
				memberTask?.GetOtherExceptions(exception));
		}

		if (_asyncMemberAccessor is not null && memberTask is null)
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return NullSubjectResult.CreateForNullTask(result, MemberAccessor.ToString().Trim(), value);
		}

		ConstraintResult memberResult;
		using (EvaluationContext.EvaluationContext.StartNestedEvaluation(context))
		{
			memberResult = await IsMetByMember(member, context, cancellationToken);
		}

		return memberResult.UseValue(value);
	}

	/// <inheritdoc />
	/// <remarks>
	///     The expectations on the member are held in a separate node, so that a combination node created by
	///     <c>And</c>/<c>Or</c> while registering them can replace it.
	/// </remarks>
	public override void AddNode(Node node, string? separator = null) => SetInnerNode(node);

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		StringBuilder separator = new();
		_appendMemberText(separator);
		stringBuilder.AppendSeparatedExpectation(separator.ToString(),
			sb => base.AppendExpectation(sb, indentation));
	}

	/// <summary>
	///     Verifies if the <paramref name="value" /> of the member satisfies the expectations of the node.
	/// </summary>
	private ValueTask<ConstraintResult> IsMetByMember(TTarget? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (value is TNarrowed narrowedValue)
		{
			return base.IsMetBy(narrowedValue, context, cancellationToken);
		}

		if (value is null)
		{
			return base.IsMetBy<TNarrowed>(default, context, cancellationToken);
		}

		return GetNotApplicableResult(context, cancellationToken);
	}

	/// <summary>
	///     Returns the expectations on a member whose runtime type is not <typeparamref name="TNarrowed" />, without
	///     evaluating them.
	/// </summary>
	private async ValueTask<ConstraintResult> GetNotApplicableResult(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		ConstraintResult expectationResult = await base.IsMetBy<TNarrowed>(default,
			ExpectationTextEvaluationContext.For(context), cancellationToken);
		return new NotApplicableConstraintResult(expectationResult.AppendExpectation);
	}

	/// <summary>
	///     Returns the expectations on the member, without evaluating them, for when the member value is not available.
	/// </summary>
	private ValueTask<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByMember(default, ExpectationTextEvaluationContext.For(context), cancellationToken);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj)
		=> obj is MappingNode<TSource, TTarget, TNarrowed> other && MemberAccessor.Equals(other.MemberAccessor) &&
		   base.Equals(other);

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => MemberAccessor.GetHashCode();

	/// <inheritdoc />
	internal override ConstraintResult CombineResults(ConstraintResult? combinedResult, ConstraintResult result)
	{
		if (combinedResult == null)
		{
			return result.PrependExpectationText(_appendMemberText, ContextMember);
		}

		return new MappingResult(combinedResult, result, _appendMemberText, ContextMember);
	}
}
