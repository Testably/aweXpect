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
	///     Writes the text that introduces the member as an alternative to the negated expectation on the subject, e.g.
	///     <c> or $.a </c>, or <see langword="null" /> when a negation keeps the expectations on the member as they are.
	/// </summary>
	/// <remarks>
	///     See <see cref="ExpectationBuilder.MemberExpectationBuilder{TSource,TMember}.NegatedAsAlternative" />.
	/// </remarks>
	public Action<MemberAccessor, StringBuilder>? NegatedAlternativeTextGenerator { get; set; }

	/// <summary>
	///     Combines the <paramref name="result" /> of the expectations on the member with the
	///     <paramref name="combinedResult" /> of the expectations on the value, if any.
	/// </summary>
	internal abstract ConstraintResult CombineResults(ConstraintResult? combinedResult, ConstraintResult result);

	/// <summary>
	///     Appends the text that introduces the member to the <paramref name="stringBuilder" />.
	/// </summary>
	internal abstract void AppendMemberText(StringBuilder stringBuilder);
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
	/// <summary>
	///     The accessor of a member that returns a <see cref="Task{TResult}" /> or a
	///     <see cref="ValueTask{TResult}" /> of <typeparamref name="TTarget" />.
	/// </summary>
	private readonly MemberAccessor? _asyncMemberAccessor;

	private readonly Action<MemberAccessor, StringBuilder>? _expectationTextGenerator;
	private readonly MemberAccessor<TSource, TTarget>? _memberAccessor;

	/// <summary>
	///     Maps to the member that the <paramref name="memberAccessor" /> accesses.
	/// </summary>
	public MappingNode(MemberAccessor<TSource, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_memberAccessor = memberAccessor;
		_expectationTextGenerator = expectationTextGenerator;
	}

	/// <summary>
	///     Maps to the member that the <paramref name="asyncMemberAccessor" /> accesses and that is awaited.
	/// </summary>
	public MappingNode(MemberAccessor<TSource, Task<TTarget>> asyncMemberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
		_expectationTextGenerator = expectationTextGenerator;
	}

	/// <summary>
	///     Maps to the member that the <paramref name="asyncMemberAccessor" /> accesses and that is awaited.
	/// </summary>
	public MappingNode(MemberAccessor<TSource, ValueTask<TTarget>> asyncMemberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
		_expectationTextGenerator = expectationTextGenerator;
	}

	/// <inheritdoc />
	/// <remarks>
	///     Read from the typed accessor fields instead of a field of its own, which keeps every node smaller.
	/// </remarks>
	public override MemberAccessor MemberAccessor => (MemberAccessor?)_memberAccessor ?? _asyncMemberAccessor!;

	/// <inheritdoc />
	/// <remarks>
	///     A method instead of a delegate per node, because the text is only read for a failure.
	/// </remarks>
	internal override void AppendMemberText(StringBuilder stringBuilder)
	{
		if (_expectationTextGenerator is null)
		{
			stringBuilder.Append(MemberAccessor);
			return;
		}

		_expectationTextGenerator(MemberAccessor, stringBuilder);
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
	///     <para />
	///     A <see langword="null" /> <paramref name="value" /> for which the <paramref name="source" /> already failed is
	///     not reported again, as the source can describe it in its own way, e.g. as the JSON value <c>null</c>.
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

		if (value is null && source is { Outcome: Outcome.Failure or Outcome.FailureBothWays, } &&
		    context is not ExpectationTextEvaluationContext)
		{
			return GetNotApplicableResult(context, cancellationToken);
		}

		return IsMetByValue(value, context, cancellationToken);
	}

	/// <remarks>
	///     A member that is accessed directly and whose expectations complete synchronously is verified without a state
	///     machine, because most members are of this kind.
	/// </remarks>
	private ValueTask<ConstraintResult> IsMetByValue<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (_asyncMemberAccessor is not null || context is ExpectationTextEvaluationContext ||
		    value is not TSource typedValue || value is DelegateValue { IsNull: true, })
		{
			return IsMetByValueAsync(value, context, cancellationToken);
		}

		TTarget member;
		try
		{
			member = _memberAccessor!.AccessMember(typedValue, context, cancellationToken);
		}
		catch (Exception exception) when (!MemberExceptionResult.IsCancellationOf(exception, cancellationToken))
		{
			return FromMemberException(exception, value, context, cancellationToken);
		}

		ValueTask<ConstraintResult> isMet;
		EvaluationContext.EvaluationContext.NestedEvaluation nestedEvaluation =
			EvaluationContext.EvaluationContext.StartNestedEvaluation(context);
		try
		{
			isMet = IsMetByMember(member, context, cancellationToken);
		}
		catch
		{
			nestedEvaluation.Dispose();
			throw;
		}

		if (!isMet.IsCompletedSuccessfully)
		{
			return AwaitMember(isMet, nestedEvaluation, value!, member);
		}

		nestedEvaluation.Dispose();
		return new ValueTask<ConstraintResult>(CreateMemberResult(isMet.Result, value!, member));
	}

	/// <summary>
	///     Awaits the expectations on the member of the fast path in <see cref="IsMetByValue{TValue}" />, which did not
	///     complete synchronously, within the <paramref name="nestedEvaluation" />.
	/// </summary>
	private async ValueTask<ConstraintResult> AwaitMember<TValue>(ValueTask<ConstraintResult> isMet,
		EvaluationContext.EvaluationContext.NestedEvaluation nestedEvaluation, TValue value, TTarget member)
	{
		ConstraintResult memberResult;
		using (nestedEvaluation)
		{
			memberResult = await isMet;
		}

		return CreateMemberResult(memberResult, value, member);
	}

	private async ValueTask<ConstraintResult> FromMemberException<TValue>(Exception exception, TValue? value,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		ConstraintResult result = await GetExpectationResult(context, cancellationToken);
		return MemberExceptionResult.Create(result, exception, MemberAccessor.GetTrimmedName(), value);
	}

	private async ValueTask<ConstraintResult> IsMetByValueAsync<TValue>(
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
			return new NotApplicableConstraintResult(expectationResult);
		}

		TTarget member = default!;
		Task<TTarget>? memberTask = null;
		bool isNullTask = false;
		try
		{
			if (_asyncMemberAccessor is null)
			{
				member = _memberAccessor!.AccessMember(typedValue, context, cancellationToken);
			}
			else if (_asyncMemberAccessor is MemberAccessor<TSource, Task<TTarget>> taskMemberAccessor)
			{
				memberTask = taskMemberAccessor.AccessMember(typedValue, context, cancellationToken);
				if (memberTask is null)
				{
					isNullTask = true;
				}
				else
				{
					member = await memberTask.AbandonOnCancellation(cancellationToken);
				}
			}
			else
			{
				ValueTask<TTarget> memberValueTask =
					((MemberAccessor<TSource, ValueTask<TTarget>>)_asyncMemberAccessor).AccessMember(typedValue,
						context, cancellationToken);
				if (memberValueTask.IsCompletedSuccessfully)
				{
					member = memberValueTask.Result;
				}
				else
				{
					memberTask = memberValueTask.AsTask();
					member = await memberTask.AbandonOnCancellation(cancellationToken);
				}
			}
		}
		catch (Exception exception) when (!MemberExceptionResult.IsCancellationOf(exception, cancellationToken))
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return MemberExceptionResult.Create(result, exception, MemberAccessor.GetTrimmedName(), value,
				memberTask?.GetOtherExceptions(exception));
		}

		if (isNullTask)
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return NullSubjectResult.CreateForNullTask(result, MemberAccessor.GetTrimmedName(), value);
		}

		ConstraintResult memberResult;
		using (EvaluationContext.EvaluationContext.StartNestedEvaluation(context))
		{
			memberResult = await IsMetByMember(member, context, cancellationToken);
		}

		return CreateMemberResult(memberResult, value, member);
	}

	/// <summary>
	///     The result of the expectations on the <paramref name="member" />, which also adds the contexts of the member,
	///     when its expectations added subject contexts, see <see cref="SubjectContextsNode.IsOfMember" />.
	/// </summary>
	/// <remarks>
	///     The contexts are only added when the expectations were evaluated on the member, like the
	///     <see cref="SubjectContextsNode" /> does.
	/// </remarks>
	private ConstraintResult CreateMemberResult<TValue>(ConstraintResult memberResult, TValue value, TTarget? member)
	{
		if (InnerNode is SubjectContextsNode { IsOfMember: true, } subjectContextsNode)
		{
			if (member is TNarrowed narrowedMember)
			{
				return new MemberWithContextsResult<TValue>(memberResult, value, subjectContextsNode.SubjectContexts,
					narrowedMember);
			}

			if (member is null)
			{
				return new MemberWithContextsResult<TValue>(memberResult, value, subjectContextsNode.SubjectContexts,
					default);
			}
		}

		return new MemberResult<TValue>(memberResult, value);
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
		AppendMemberText(separator);
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
		return new NotApplicableConstraintResult(expectationResult);
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
			return result is MemberResult memberResult
				? memberResult.PrependMemberText(this)
				: result.PrependExpectationText(AppendMemberText, ContextMember, MemberAccessor.PathRoot);
		}

		return new MappingResult(combinedResult, result, this);
	}

	/// <summary>
	///     The result of the expectations on the member, which keeps the value they were evaluated on, and prepends the
	///     text of the member once it is combined, so that it is wrapped only once for every evaluation.
	/// </summary>
	/// <remarks>
	///     It behaves like a value of <see cref="ConstraintResultExtensions.UseValue{T}" />, which is wrapped by
	///     <see cref="ConstraintResultExtensions.PrependExpectationText(ConstraintResult, Action{StringBuilder})" />
	///     once combined.
	/// </remarks>
	private abstract class MemberResult : ConstraintResult
	{
		private MappingNode? _prefix;

		protected MemberResult(ConstraintResult inner) : base(inner.FurtherProcessingStrategy)
		{
			Inner = inner;
			Outcome = inner.Outcome;
		}

		protected ConstraintResult Inner { get; }

		public override Exception? FailureCause => Inner.FailureCause;

		public override string? LeadingSubject => Inner.LeadingSubject;

		public override string? TrailingSubject => Inner.TrailingSubject;

		internal override bool IsExpectationOnly => Inner.IsExpectationOnly;

		internal override bool IsNegatedAnd => Inner.IsNegatedAnd;

		public MemberResult PrependMemberText(MappingNode prefix)
		{
			_prefix = prefix;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_prefix is null)
			{
				Inner.AppendExpectation(stringBuilder, indentation);
				return;
			}

			StringBuilder prefix = new();
			_prefix.AppendMemberText(prefix);
			stringBuilder.AppendSeparatedExpectation(prefix.ToString(),
				sb => Inner.AppendExpectation(sb, indentation));
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> Inner.AppendResult(stringBuilder, indentation);

		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.VisitOptionalMember(_prefix?.ContextMember, GetContextsResult(),
				_prefix?.MemberAccessor.PathRoot);

		public override ConstraintResult Negate()
		{
			Inner.Negate();
			Outcome = Inner.Outcome;
			return this;
		}

		/// <summary>
		///     The result whose contexts are those of the member.
		/// </summary>
		protected virtual ConstraintResult GetContextsResult() => Inner;
	}

	/// <inheritdoc cref="MemberResult" />
	private class MemberResult<TValue>(ConstraintResult inner, TValue memberValue) : MemberResult(inner)
	{
		public override bool TryGetStoredValue<TStored>(out TStored? value) where TStored : default
		{
			if (memberValue is TStored typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TStored).IsAssignableFrom(typeof(TValue));
		}
	}

	/// <summary>
	///     A <see cref="MemberResult{TValue}" />, whose contexts start with the <paramref name="subjectContexts" /> of the
	///     <paramref name="member" />.
	/// </summary>
	/// <remarks>
	///     The contexts are only read for a failure, so the result that adds them is only created then, instead of for
	///     every evaluation.
	/// </remarks>
	private sealed class MemberWithContextsResult<TValue>(
		ConstraintResult inner,
		TValue memberValue,
		SubjectContext subjectContexts,
		TNarrowed? member) : MemberResult<TValue>(inner, memberValue)
	{
		protected override ConstraintResult GetContextsResult()
			=> new SubjectContextsNode.SubjectContextsResult<TNarrowed>(Inner, member, subjectContexts);
	}
}
