using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Core.Nodes;

internal class AsyncMappingNode<TSource, TTarget> : ExpectationNode
{
	/// <summary>
	///     Appends the text for the member, which precedes the expectations on it.
	/// </summary>
	private readonly Action<StringBuilder> _appendMemberText;

	private readonly Action<MemberAccessor<TSource, Task<TTarget>>, StringBuilder>
		_expectationTextGenerator;

	private readonly MemberAccessor<TSource, Task<TTarget>> _memberAccessor;

	public AsyncMappingNode(MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_memberAccessor = memberAccessor;
		if (expectationTextGenerator == null)
		{
			_expectationTextGenerator = DefaultExpectationTextGenerator;
		}
		else
		{
			_expectationTextGenerator = expectationTextGenerator;
		}

		_appendMemberText = stringBuilder => _expectationTextGenerator(_memberAccessor, stringBuilder);
	}

	/// <inheritdoc cref="MappingNode{TSource,TTarget}.Source" />
	public (string It, ExpectationGrammars Grammars) Source { get; set; } = ("it", ExpectationGrammars.None);

	/// <inheritdoc />
	public override async Task<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
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

		if (value is TSource typedValue)
		{
			TTarget matchingValue = default!;
			Task<TTarget>? member = null;
			try
			{
				member = _memberAccessor.AccessMember(typedValue);
				if (member is not null)
				{
					matchingValue = await member.AbandonOnCancellation(cancellationToken);
				}
			}
			catch (Exception exception) when (!MemberExceptionResult.IsCancellationOf(exception, cancellationToken))
			{
				(context as EvaluationContext.EvaluationContext)?.AddOtherExceptions(
					member?.GetOtherExceptions(exception));
				ConstraintResult result = await GetExpectationResult(context, cancellationToken);
				return MemberExceptionResult.Create(result, exception, _memberAccessor.ToString().Trim(), value);
			}

			if (member is null)
			{
				ConstraintResult result = await GetExpectationResult(context, cancellationToken);
				return NullSubjectResult.CreateForNullTask(result, _memberAccessor.ToString().Trim(), value);
			}

			ConstraintResult memberResult = await IsMetByMember(matchingValue, context, cancellationToken);
			return memberResult.UseValue(value);
		}

		// The value only has another type after a failed type check (e.g. `Is<T>()`), which reports the mismatch.
		ConstraintResult expectationResult = await GetExpectationResult(context, cancellationToken);
		return new NotApplicableConstraintResult(expectationResult.AppendExpectation);
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
		_expectationTextGenerator(_memberAccessor, separator);
		stringBuilder.AppendSeparatedExpectation(separator.ToString(), sb => AppendMemberExpectation(sb, indentation));
	}

	/// <summary>
	///     Appends the expectations on the member, without the text for the member itself.
	/// </summary>
	/// <remarks>
	///     Results flowing through <see cref="CombineResults" /> already get the member text prepended there.
	/// </remarks>
	protected void AppendMemberExpectation(StringBuilder stringBuilder, string? indentation)
		=> base.AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Verifies if the <paramref name="value" /> of the member satisfies the expectations of the node.
	/// </summary>
	protected virtual Task<ConstraintResult> IsMetByMember(TTarget? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByExpectations(value, context, cancellationToken);

	/// <summary>
	///     Returns the expectations on the member, without evaluating them, for when the member value is not available.
	/// </summary>
	private Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByMember(default, ExpectationTextEvaluationContext.For(context), cancellationToken);

	/// <summary>
	///     Verifies if the <paramref name="value" /> satisfies the expectations of the node, without accessing the member.
	/// </summary>
	protected Task<ConstraintResult> IsMetByExpectations<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		=> base.IsMetBy(value, context, cancellationToken);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is AsyncMappingNode<TSource, TTarget> other && Equals(other);

	private bool Equals(AsyncMappingNode<TSource, TTarget> other) => _memberAccessor.Equals(other._memberAccessor);

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => _memberAccessor.GetHashCode();

	internal ConstraintResult CombineResults(
		ConstraintResult? combinedResult,
		ConstraintResult result)
	{
		if (combinedResult == null)
		{
			return result.PrependExpectationText(_appendMemberText);
		}

		return new MappingResult(combinedResult, result, _appendMemberText);
	}

	private static void DefaultExpectationTextGenerator(
		MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		StringBuilder expectation)
		=> expectation.Append(memberAccessor);
}
