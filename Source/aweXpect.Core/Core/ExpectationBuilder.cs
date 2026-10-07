using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Initialization;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.TimeSystem;
using aweXpect.Customization;

namespace aweXpect.Core;

/// <summary>
///     The builder for collecting all expectations.
/// </summary>
public abstract class ExpectationBuilder
{
	private protected const string DefaultCurrentSubject = "it";

	private CancellationToken _cancellationToken;

	private EvaluationContext.EvaluationContext? _evaluationContext;
	private bool _hasCancellationToken;
	private bool _hasTimeout;

	/// <summary>
	///     The current name for the subject (defaults to <see cref="DefaultCurrentSubject" />).
	/// </summary>
	private string _it = DefaultCurrentSubject;

	private Node _node = new ExpectationNode();

	/// <summary>
	///     The other exceptions of the faulted subject in the current evaluation.
	/// </summary>
	private Exception[]? _otherExceptions;

	/// <summary>
	///     The which node that still waits for the expectations on its member, or <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     Two fields instead of a nullable tuple, and the cancellation token and the timeout as values with a flag
	///     instead of nullable values, keep every expectation 24 bytes smaller.
	/// </remarks>
	private Node? _pendingWhichNode;

	/// <summary>
	///     The root node that contains the <see cref="_pendingWhichNode" />.
	/// </summary>
	private Node? _pendingWhichRoot;

	private List<IBecauseReason>? _reasons;
	private TimeSpan _timeout;

	private ITimeSystem? _timeSystem;

	/// <summary>
	///     Initializes the <see cref="ExpectationBuilder" /> with the <paramref name="subjectExpression" />
	///     for the statement builder.
	/// </summary>
	/// <remarks>
	///     Only aweXpect.Core can derive, because the evaluation relies on members that are not public.
	/// </remarks>
	private protected ExpectationBuilder(string subjectExpression,
		ExpectationGrammars grammars = ExpectationGrammars.None)
	{
		AweXpectInitialization.EnsureInitialized();
		Subject = subjectExpression.TrimCommonWhiteSpace();
		ExpectationGrammars = grammars;
	}

	/// <summary>
	///     Initializes the <see cref="ExpectationBuilder" /> with an empty <see cref="Subject" />.
	/// </summary>
	private protected ExpectationBuilder()
	{
		AweXpectInitialization.EnsureInitialized();
		Subject = "";
		_it = "";
		ExpectationGrammars = ExpectationGrammars.None;
	}

	/// <summary>
	///     The explicit cancellation token to be used for the expectation.
	/// </summary>
	/// <remarks>
	///     When not set, the expectation will still use the cancellation token from
	///     <see cref="AwexpectCustomization.SettingsCustomization.TestCancellation" />.
	/// </remarks>
	public CancellationToken? CancellationToken => _hasCancellationToken ? _cancellationToken : null;

	/// <summary>
	///     The explicit timeout to be applied to the expectation.
	/// </summary>
	/// <remarks>
	///     It is the tightest of all timeouts added with <see cref="WithTimeout(TimeSpan)" />. The evaluation is also
	///     limited by the timeout from <see cref="AwexpectCustomization.SettingsCustomization.TestCancellation" />, if
	///     it is tighter.
	/// </remarks>
	public TimeSpan? Timeout => _hasTimeout ? _timeout : null;

	/// <summary>
	///     The expected grammatical form of the expectation text.
	/// </summary>
	public ExpectationGrammars ExpectationGrammars { get; private set; }

	internal string Subject { get; }

	/// <summary>
	///     Verifies that the subject is <see langword="true" />, when no expectation was added.
	/// </summary>
	internal bool IsTrueWithoutExpectations { private get; set; }

	/// <summary>
	///     Whether a trace writer was set when the current evaluation started.
	/// </summary>
	/// <remarks>
	///     Read once per evaluation together with the test cancellation, because each read of a customization costs
	///     about as much as a simple constraint.
	/// </remarks>
	internal bool IsTracing { get; private set; }

	/// <summary>
	///     The reasons of the expectation.
	/// </summary>
	private protected IEnumerable<IBecauseReason> Reasons => _reasons ?? [];

	/// <summary>
	///     Adds the <see cref="IValueConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies the
	///     underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the current name for the subject (mostly "it") and the
	///     current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<string, ExpectationGrammars, IValueConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(_it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IValueConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies the
	///     underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives this <see cref="ExpectationBuilder" />, the current name for
	///     the subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<ExpectationBuilder, string, ExpectationGrammars, IValueConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the current name for the subject (mostly "it") and the
	///     current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<string, ExpectationGrammars, IContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(_it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives this <see cref="ExpectationBuilder" />, the current name for
	///     the subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<ExpectationBuilder, string, ExpectationGrammars, IContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies the
	///     underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the current name for the subject (mostly "it") and the
	///     current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<string, ExpectationGrammars, IAsyncConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(_it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies the
	///     underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives this <see cref="ExpectationBuilder" />, the current name for
	///     the subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<ExpectationBuilder, string, ExpectationGrammars, IAsyncConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which
	///     verifies the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the current name for the subject (mostly "it") and the
	///     current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<string, ExpectationGrammars, IAsyncContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(_it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which
	///     verifies the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives this <see cref="ExpectationBuilder" />, the current name for
	///     the subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TValue>(
		Func<ExpectationBuilder, string, ExpectationGrammars, IAsyncContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IValueConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, the current name for the
	///     subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, string, ExpectationGrammars, IValueConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IValueConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, this
	///     <see cref="ExpectationBuilder" />, the current name for the subject (mostly "it") and the current
	///     <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, ExpectationBuilder, string, ExpectationGrammars, IValueConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, the current name for the
	///     subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, string, ExpectationGrammars, IContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, this
	///     <see cref="ExpectationBuilder" />, the current name for the subject (mostly "it") and the current
	///     <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, ExpectationBuilder, string, ExpectationGrammars, IContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, the current name for the
	///     subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, string, ExpectationGrammars, IAsyncConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, this
	///     <see cref="ExpectationBuilder" />, the current name for the subject (mostly "it") and the current
	///     <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, ExpectationBuilder, string, ExpectationGrammars, IAsyncConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, the current name for the
	///     subject (mostly "it") and the current <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, string, ExpectationGrammars, IAsyncContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the <see cref="IAsyncContextConstraint{TValue}" /> from the <paramref name="constraintBuilder" /> which verifies
	///     the underlying value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraintBuilder" /> receives the <paramref name="state" />, this
	///     <see cref="ExpectationBuilder" />, the current name for the subject (mostly "it") and the current
	///     <see cref="ExpectationGrammars" />.
	///     <para />
	///     Passing the values of the constraint as <paramref name="state" /> to a <see langword="static" /> lambda avoids
	///     allocating a closure and a delegate for every expectation.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TValue>(TState state,
		Func<TState, ExpectationBuilder, string, ExpectationGrammars, IAsyncContextConstraint<TValue>> constraintBuilder)
	{
		_node.AddConstraint(constraintBuilder(state, this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Specifies a constraint that applies to the member selected
	///     by the <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectations on the member fail with <c>… did throw …</c> and the
	///     exception as <see cref="ConstraintResult.FailureCause" />, which a negation does not invert. An
	///     <see cref="OperationCanceledException" /> thrown while the evaluation is canceled aborts the evaluation
	///     instead.
	/// </remarks>
	public MemberExpectationBuilder<TSource, TTarget> ForMember<TSource, TTarget>(
		MemberAccessor<TSource, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null,
		bool replaceIt = true)
		=> new((expectations, expectationGrammars, sourceConstraint, createMappingNode)
				=> AddMemberExpectations(memberAccessor, replaceIt, expectations, expectationGrammars,
					sourceConstraint, createMappingNode),
			new SyncMappingNodes<TSource, TTarget>(memberAccessor, expectationTextGenerator));

	/// <summary>
	///     Specifies a constraint that applies to the member selected asynchronously
	///     by the <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the expectations on it are applied. If accessing or awaiting the member throws,
	///     they fail with <c>… did throw …</c> and the exception as <see cref="ConstraintResult.FailureCause" />, which
	///     a negation does not invert. Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a
	///     timeout fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	public MemberExpectationBuilder<TSource, TTarget> ForAsyncMember<TSource, TTarget>(
		MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null,
		bool replaceIt = true)
		=> new((expectations, expectationGrammars, sourceConstraint, createMappingNode)
				=> AddMemberExpectations(memberAccessor, replaceIt, expectations, expectationGrammars,
					sourceConstraint, createMappingNode),
			new AsyncMappingNodes<TSource, TTarget>(memberAccessor, expectationTextGenerator));

	/// <summary>
	///     Adds the mapping node from <paramref name="createMappingNode" /> and the <paramref name="expectations" /> on
	///     the member, after the constraint from the <paramref name="sourceConstraint" /> on the value, if any.
	/// </summary>
	private ExpectationBuilder AddMemberExpectations<TSource>(
		MemberAccessor memberAccessor,
		bool replaceIt,
		Action<ExpectationBuilder> expectations,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars,
		Func<string, ExpectationGrammars, IValueConstraint<TSource>>? sourceConstraint,
		Func<MappingNode> createMappingNode)
	{
		if (sourceConstraint is not null)
		{
			IValueConstraint<TSource> constraint = sourceConstraint.Invoke(_it, ExpectationGrammars);
			_node.AddConstraint(constraint);
		}

		Node root = _node;
		MappingNode memberNode = createMappingNode();
		memberNode.Source = (_it, ExpectationGrammars);
		Node mappingNode = _node.AddMapping(memberNode);
		_node = new ExpectationNode();
		string previousIt = _it;
		if (replaceIt)
		{
			_it = memberAccessor.ToString().Trim();
			memberNode.ContextMember = _it;
		}

		Node? outerPendingWhichNode = _pendingWhichNode;
		Node? outerPendingWhichRoot = _pendingWhichRoot;
		_pendingWhichNode = null;
		_pendingWhichRoot = null;

		ExpectationGrammars previousGrammars = ExpectationGrammars;
		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammars?.Invoke(memberGrammars) ?? memberGrammars;
		int outerReasonCount = _reasons?.Count ?? 0;
		expectations.Invoke(this);
		ExpectationGrammars = previousGrammars;

		CompleteWhichNode();
		_pendingWhichNode = outerPendingWhichNode;
		_pendingWhichRoot = outerPendingWhichRoot;
		ThrowIfEmpty(_node, nameof(expectations));
		mappingNode.AddNode(_node);
		MoveReasonsTo(mappingNode, outerReasonCount);
		_node = root;
		_it = previousIt;

		return this;
	}

	/// <summary>
	///     Moves the reasons that were added after the first <paramref name="outerReasonCount" /> ones, i.e. for the
	///     expectations on a member, to the <paramref name="mappingNode" />, so that they follow the expectation on the
	///     member instead of the whole expectation.
	/// </summary>
	private void MoveReasonsTo(Node mappingNode, int outerReasonCount)
	{
		if (_reasons is null || _reasons.Count == outerReasonCount ||
		    mappingNode is not ExpectationNode expectationNode)
		{
			return;
		}

		int count = _reasons.Count - outerReasonCount;
		expectationNode.AddReasons(_reasons.GetRange(outerReasonCount, count));
		_reasons.RemoveRange(outerReasonCount, count);
	}

	// An empty member node cannot be evaluated, and silently skipping it would hide a forgotten expectation.
	// The parameter name refers to the expectations callback of the public expectation, not to a parameter here.
	private static void ThrowIfEmpty(Node memberNode, string paramName)
	{
		if (memberNode is ExpectationNode expectationNode && expectationNode.IsEmpty())
		{
			throw Tracing.WriteException(
				new ArgumentException("You must add at least one expectation in the expectations callback.",
					paramName));
		}
	}

	/// <summary>
	///     Adds a <paramref name="cancellationToken" /> to be used by the constraints.
	/// </summary>
	/// <exception cref="InvalidOperationException">A cancellation token is already set.</exception>
	public void WithCancellation(CancellationToken cancellationToken)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_hasCancellationToken, nameof(WithCancellation));
		_cancellationToken = cancellationToken;
		_hasCancellationToken = true;
	}

	/// <summary>
	///     Adds a <paramref name="timeout" /> to be used by the constraints.
	/// </summary>
	/// <remarks>
	///     The tighter timeout wins, so a <paramref name="timeout" /> that is longer than one added before does not
	///     loosen it. <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public void WithTimeout(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		_timeout = TimerHelpers.Tighter(Timeout, timeout) ?? timeout;
		_hasTimeout = true;
	}

	/// <summary>
	///     Adds a <paramref name="reason" /> to the expectation.
	/// </summary>
	internal void AddReason(string reason)
		=> (_reasons ??= []).Add(new BecauseReason(reason));

	/// <summary>
	///     Adds a <paramref name="reason" /> to the expectation.
	/// </summary>
	internal void AddReason(Task<string?> reason)
		=> (_reasons ??= []).Add(new AsyncBecauseReason(reason));

	/// <summary>
	///     Appends the reasons to the expectation of the <paramref name="result" />.
	/// </summary>
	/// <remarks>
	///     The reasons are applied to the whole expectation instead of to the constraint they were given for, so that
	///     they follow every suffix, e.g. constraints combined with <c>And</c> or <c>Or</c> and the timeout of
	///     <c>Eventually</c>.
	///     <para />
	///     With an <paramref name="evaluation" />, a reason that must be awaited is appended without awaiting it and
	///     resolved by <see cref="ResolvePendingReasons()" />, which limits the wait. It is passed for a
	///     <paramref name="result" /> that is not met and for the member of a combination, as the combination can still
	///     fail a met expectation.
	/// </remarks>
	internal async ValueTask<ConstraintResult> ApplyReasons(ConstraintResult result,
		EvaluationContext.EvaluationContext? evaluation = null)
	{
		if (_reasons is not null)
		{
			foreach (IBecauseReason reason in _reasons)
			{
				result = evaluation is not null && reason is AsyncBecauseReason asyncReason
					? asyncReason.ApplyPending(result, evaluation)
					: await reason.ApplyTo(result);
			}
		}

		return result;
	}

	/// <summary>
	///     Resolves the reasons that must be awaited of the current evaluation, when it fails or when a combination
	///     fails the met expectation.
	/// </summary>
	/// <remarks>
	///     A reason is awaited until the timeout of the evaluation elapses or the evaluation is canceled.
	/// </remarks>
	internal Task ResolvePendingReasons()
		=> _evaluationContext is { HasPendingReasons: true, } context
			? ResolvePendingReasons(context)
			: Task.CompletedTask;

	private static async Task ResolvePendingReasons(EvaluationContext.EvaluationContext context)
	{
		EvaluationCancellation cancellation = context.Cancellation.ForRemainingTimeout();
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		await context.ResolvePendingReasons(cancellation.Token);
	}

	/// <summary>
	///     Resolves the reasons that must be awaited, so that their message is available.
	/// </summary>
	/// <remarks>
	///     A reason that is still pending when the <paramref name="cancellationToken" /> is canceled is abandoned.
	/// </remarks>
	internal async Task ResolveReasons(CancellationToken cancellationToken)
	{
		foreach (AsyncBecauseReason reason in _reasons?.OfType<AsyncBecauseReason>() ?? [])
		{
			await reason.Resolve(cancellationToken);
		}
	}

	/// <summary>
	///     Registers the reasons that must be awaited, to be resolved when the evaluation in the
	///     <paramref name="context" /> fails, e.g. because an outer negation inverts the met expectations.
	/// </summary>
	internal void ResolveReasonsOnFailureOf(IEvaluationContext context)
	{
		if (_reasons is null)
		{
			return;
		}

		foreach (IBecauseReason reason in _reasons)
		{
			if (reason is AsyncBecauseReason asyncReason)
			{
				asyncReason.ResolveOnFailureOf(context);
			}
		}
	}

	/// <summary>
	///     Supports chaining for subsequent expectation constraints with the <paramref name="textSeparator" />.
	/// </summary>
	public ExpectationBuilder And(string textSeparator = " and ")
		=> AddAndOperand(new ExpectationNode(), textSeparator);

	/// <summary>
	///     Supports chaining expectations on further members of the value that the preceding expectation passes on, e.g.
	///     for <c>AndWhose</c>, with the <paramref name="textSeparator" />.
	/// </summary>
	internal ExpectationBuilder AndContinuing(string textSeparator)
		=> AddAndOperand(new ExpectationNode
		{
			ContinuesPrecedingOperand = true,
		}, textSeparator);

	private ExpectationBuilder AddAndOperand(ExpectationNode operand, string textSeparator)
	{
		if (_node is AndNode andNode)
		{
			andNode.AddNode(operand, textSeparator);
		}
		else if (_node is OrNode { Current: AndNode currentAndNode, })
		{
			// A single node evaluates the whole `And` chain, as only it skips the operands after a failed guard.
			currentAndNode.AddNode(operand, textSeparator);
		}
		else if (_node is OrNode orNode)
		{
			AndNode newNode = new(orNode.Current);
			newNode.AddNode(operand, textSeparator);
			orNode.Current = newNode;
		}
		else
		{
			AndNode newNode = new(_node);
			newNode.AddNode(operand, textSeparator);
			_node = newNode;
		}

		return this;
	}

	/// <summary>
	///     Specifies a mapping to add expectations on the member from the <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member continues only the expectation in front of it, also after <c>And</c>/<c>Or</c>, and is only
	///     accessed when that expectation is met.
	///     <para />
	///     Set <paramref name="negateMemberOnly" /> when the previous expectation only navigates to the member, so that
	///     a negation applies to the member expectation ("has keys that do not contain 0") instead of the whole
	///     expectation ("does not have a single item that is equal to 3").
	///     <para />
	///     If accessing the member throws, the expectations on the member fail with <c>… did throw …</c> and the
	///     exception as <see cref="ConstraintResult.FailureCause" />, which a negation does not invert. An
	///     <see cref="OperationCanceledException" /> thrown while the evaluation is canceled aborts the evaluation
	///     instead.
	///     <para />
	///     The contexts of the member are labelled with the <paramref name="contextMember" />, e.g. <c>keys</c>, or with
	///     the <paramref name="subjectName" />, unless the member is called <c>it</c>.
	/// </remarks>
	/// <param name="memberAccessor">Accesses the member on the source value.</param>
	/// <param name="separator">The text between the previous expectation and the expectations on the member.</param>
	/// <param name="subjectName">
	///     The name that the expectations on the member use for it, or <see langword="null" /> to keep the current one.
	/// </param>
	/// <param name="expectationGrammars">
	///     Changes the <see cref="ExpectationGrammars" /> of the expectations on the member.
	/// </param>
	/// <param name="negateMemberOnly">
	///     Whether a negation only applies to the expectations on the member instead of the whole expectation.
	/// </param>
	/// <param name="contextMember">
	///     The member that labels the contexts of the expectations on the member, or <see langword="null" /> for the
	///     <paramref name="subjectName" />.
	/// </param>
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, TTarget?> memberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false,
		string? contextMember = null)
	{
		contextMember ??= subjectName == DefaultCurrentSubject ? null : subjectName;
		AddWhichNode(parentNode => new WhichNode<TSource, TTarget>(parentNode, memberAccessor, separator,
			negateMemberOnly, subjectName, contextMember));

		if (subjectName != null)
		{
			_it = subjectName;
		}

		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammars?.Invoke(memberGrammars) ?? memberGrammars;
		return this;
	}

	/// <summary>
	///     Specifies a mapping to add expectations on the member from the <paramref name="asyncMemberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member continues only the expectation in front of it, also after <c>And</c>/<c>Or</c>, and is only
	///     accessed when that expectation is met.
	///     <para />
	///     The member is a single value, so its expectations are in singular form and refer to it as <c>it</c>, even
	///     when the enclosing expectation named its subject (e.g. inside <c>Whose</c>).
	///     <para />
	///     If accessing or awaiting the member throws, the expectations on the member fail with <c>… did throw …</c> and
	///     the exception as <see cref="ConstraintResult.FailureCause" />, which a negation does not invert. An
	///     <see cref="OperationCanceledException" /> thrown while the evaluation is canceled aborts the evaluation
	///     instead.
	///     <para />
	///     Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a timeout
	///     fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	/// <param name="asyncMemberAccessor">Accesses the member on the source value asynchronously.</param>
	/// <param name="separator">The text between the previous expectation and the expectations on the member.</param>
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, Task<TTarget?>> asyncMemberAccessor,
		string? separator = null)
	{
		AddWhichNode(parentNode => new WhichNode<TSource, TTarget>(parentNode, asyncMemberAccessor, separator));
		_it = DefaultCurrentSubject;
		ExpectationGrammars &= ~(ExpectationGrammars.Introduced | ExpectationGrammars.Plural);
		return this;
	}

	/// <summary>
	///     Replaces the right-most operand with the which node from <paramref name="createWhichNode" />, which continues
	///     it, and collects the following expectations for the member until the which node is completed.
	/// </summary>
	private void AddWhichNode(Func<Node?, Node> createWhichNode)
	{
		CompleteWhichNode();
		Node? whichNode = null;
		Node root = _node.ReplaceRightMostOperand(operand =>
		{
			whichNode = createWhichNode(operand is ExpectationNode e && e.IsEmpty() ? null : operand);
			return whichNode;
		});
		_pendingWhichNode = whichNode!;
		_pendingWhichRoot = root;
		_node = new ExpectationNode();
	}

	/// <summary>
	///     Lists the <paramref name="otherExceptions" /> of the faulted subject as context of a failure, unless they are
	///     <see langword="null" />.
	/// </summary>
	internal void AddOtherExceptions(Exception[]? otherExceptions)
	{
		if (otherExceptions is not null)
		{
			_otherExceptions = otherExceptions;
		}
	}

	/// <summary>
	///     Forgets the other exceptions of a previous attempt to meet the expectations.
	/// </summary>
	internal void ResetOtherExceptions() => _otherExceptions = null;

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" />.
	/// </summary>
	internal Task<string> FromFailure(ConstraintResult failure)
		=> FromFailure(Subject, failure, CancellationToken ?? System.Threading.CancellationToken.None);

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" />.
	/// </summary>
	private static async Task<string> FromFailure(
		string subject,
		ConstraintResult failure,
		CancellationToken cancellationToken)
	{
		StringBuilder sb = new();
		sb.Append("Expected that ");
		sb.Append(failure.TryGetValue(out IDescribableSubject? describableSubject)
			? describableSubject.GetDescription()
			: subject);
		sb.AppendLine();
		failure.AppendExpectation(sb);
		sb.AppendLine(",");
		sb.Append("but ");
		failure.AppendResult(sb);
		await ResultContextRenderer.AppendContexts(sb, failure, cancellationToken);
		return sb.ToString();
	}

	internal Node GetRootNode()
	{
		CompleteWhichNode();
		return _node;
	}

	/// <summary>
	///     Attaches the current node to a pending <see cref="WhichNode{TSource,TMember}" />, so that it is evaluated on
	///     the projected member.
	/// </summary>
	private void CompleteWhichNode()
	{
		if (_pendingWhichNode is not null)
		{
			_pendingWhichNode.AddNode(_node);
			_node = _pendingWhichRoot!;
			_pendingWhichNode = null;
			_pendingWhichRoot = null;
		}
	}

	/// <remarks>
	///     A failure keeps the collections materialized during the evaluation until <see cref="EndEvaluation" />, as the
	///     failure message still reads them.
	///     <para />
	///     A met expectation that was evaluated synchronously and has nothing to release returns without starting a state
	///     machine, because most expectations are of this kind.
	/// </remarks>
	/// <param name="endsWhenMet">
	///     Whether a met expectation ends its evaluation. The member of a combination leaves that to the combination,
	///     so that the resources it shares with the other members, like an event recording, stay usable for them.
	/// </param>
	internal ValueTask<ConstraintResult> IsMet(bool endsWhenMet = true)
	{
		Task previousEvaluation = EndEvaluation();
		if (previousEvaluation.Status != TaskStatus.RanToCompletion)
		{
			return IsMetAfter(previousEvaluation, endsWhenMet);
		}

		ResetOtherExceptions();
		EvaluationContext.EvaluationContext context = new();
		_evaluationContext = context;
		ValueTask<ConstraintResult> isMet = StartEvaluation(context);
		if (_reasons is not null || !isMet.IsCompletedSuccessfully)
		{
			return CompleteEvaluation(isMet, endsWhenMet);
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome != Outcome.Success)
		{
			return CompleteEvaluation(new ValueTask<ConstraintResult>(result), endsWhenMet);
		}

		if (!endsWhenMet)
		{
			return new ValueTask<ConstraintResult>(result);
		}

		Task endEvaluation = EndEvaluation();
		return endEvaluation.Status == TaskStatus.RanToCompletion
			? new ValueTask<ConstraintResult>(result)
			: ReturnAfter(endEvaluation, result);
	}

	private async ValueTask<ConstraintResult> IsMetAfter(Task previousEvaluation, bool endsWhenMet)
	{
		await previousEvaluation;
		return await IsMet(endsWhenMet);
	}

	private static async ValueTask<ConstraintResult> ReturnAfter(Task endEvaluation, ConstraintResult result)
	{
		await endEvaluation;
		return result;
	}

	private ValueTask<ConstraintResult> StartEvaluation(EvaluationContext.EvaluationContext context)
	{
		try
		{
			ITimeSystem timeSystem = _timeSystem ?? RealTimeSystem.Instance;
			context.TimeSystem = timeSystem;
			(TestCancellation? testCancellation, ITraceWriter? traceWriter) =
				Customize.aweXpect.GetEvaluationSettings();
			IsTracing = traceWriter is not null;
			CancellationToken cancellationToken = CancellationToken ??
			                                      testCancellation?.CancellationTokenFactory?.Invoke() ??
			                                      System.Threading.CancellationToken.None;
			TimeSpan? timeout = TimerHelpers.Tighter(Timeout, testCancellation?.Timeout);
			Node rootNode = GetRootNode();
			if (IsTrueWithoutExpectations && rootNode is ExpectationNode expectationNode && expectationNode.IsEmpty())
			{
				rootNode.AddConstraint(new ThatBoolSubject.IsTrueConstraint(ExpectationGrammars));
			}

			return IsMet(rootNode, context, timeSystem,
				timeout == System.Threading.Timeout.InfiniteTimeSpan ? null : timeout,
				cancellationToken);
		}
		catch (Exception exception)
		{
			return new ValueTask<ConstraintResult>(Task.FromException<ConstraintResult>(exception));
		}
	}

	private async ValueTask<ConstraintResult> CompleteEvaluation(ValueTask<ConstraintResult> isMet,
		bool endsWhenMet)
	{
		ConstraintResult result;
		try
		{
			result = await isMet;
			if (_reasons is not null)
			{
				result = await ApplyReasons(result,
					endsWhenMet && result.Outcome == Outcome.Success ? null : _evaluationContext);
			}
		}
		catch
		{
			await EndEvaluation();
			throw;
		}

		if (result.Outcome == Outcome.Success)
		{
			if (endsWhenMet)
			{
				await EndEvaluation();
			}

			return result;
		}

		if (_evaluationContext is not null)
		{
			if (result.Outcome == Outcome.Undecided &&
			    _evaluationContext.Cancellation.Reason == CancellationReason.None)
			{
				result = new ConstraintResult.WithoutDecision(result);
			}

			await ResolvePendingReasons();
		}

		return _otherExceptions is null ? result : new ConstraintResult.WithOtherExceptions(result, _otherExceptions);
	}

	/// <summary>
	///     Releases the sources of the collections materialized during the current evaluation, once the failure message
	///     no longer reads them.
	/// </summary>
	internal Task EndEvaluation()
	{
		if (_evaluationContext is null)
		{
			return Task.CompletedTask;
		}

		EvaluationContext.EvaluationContext context = _evaluationContext;
		_evaluationContext = null;
		return context.ReleaseMaterializations();
	}

	internal abstract ValueTask<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken);

	/// <summary>
	///     Supports chaining for alternative expectation constraints with the <paramref name="textSeparator" />.
	/// </summary>
	public ExpectationBuilder Or(string textSeparator = " or ")
	{
		if (_node is OrNode orNode)
		{
			orNode.AddNode(new ExpectationNode(), textSeparator);
			return this;
		}

		OrNode newNode = new(_node);
		newNode.AddNode(new ExpectationNode(), textSeparator);
		_node = newNode;
		return this;
	}

	/// <summary>
	///     Specifies a <see cref="ITimeSystem" /> to use for the expectation.
	/// </summary>
	internal void UseTimeSystem(ITimeSystem timeSystem)
		=> _timeSystem = timeSystem;

	/// <summary>
	///     Helper class to specify constraints on the selected <typeparamref name="TMember" />.
	/// </summary>
	public class MemberExpectationBuilder<TSource, TMember>
	{
		private readonly Func<
				Action<ExpectationBuilder>,
				Func<ExpectationGrammars, ExpectationGrammars>?,
				Func<string, ExpectationGrammars, IValueConstraint<TSource>>?,
				Func<MappingNode>,
				ExpectationBuilder>
			_callback;

		private readonly MappingNodes<TMember> _mappingNodes;

		private Func<string, ExpectationGrammars, IValueConstraint<TSource>>? _sourceConstraintBuilder;

		internal MemberExpectationBuilder(Func<
					Action<ExpectationBuilder>,
					Func<ExpectationGrammars, ExpectationGrammars>?,
					Func<string, ExpectationGrammars, IValueConstraint<TSource>>?,
					Func<MappingNode>,
					ExpectationBuilder>
				callback,
			MappingNodes<TMember> mappingNodes)
		{
			_callback = callback;
			_mappingNodes = mappingNodes;
		}

		/// <summary>
		///     Add expectations for the current <typeparamref name="TMember" />.
		/// </summary>
		public ExpectationBuilder AddExpectations(
			Action<ExpectationBuilder> expectation,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			=> _callback(expectation, expectationGrammars, _sourceConstraintBuilder, _mappingNodes.Create);

		/// <summary>
		///     Add expectations for the current <typeparamref name="TMember" /> that are typed at the narrower
		///     <typeparamref name="TNarrowed" />.
		/// </summary>
		/// <remarks>
		///     Use this overload when the <paramref name="expectation" /> is handed an
		///     <see cref="IThatSubject{TNarrowed}" /> for a member that is projected as the wider
		///     <typeparamref name="TMember" />. The expectations are skipped when the member has a different runtime type,
		///     because the expectation which narrowed the type reports the mismatch on its own.
		/// </remarks>
		public ExpectationBuilder AddExpectations<TNarrowed>(
			Action<ExpectationBuilder> expectation,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			where TNarrowed : TMember
			=> _callback(expectation, expectationGrammars, _sourceConstraintBuilder, _mappingNodes.Create<TNarrowed>);

		/// <summary>
		///     Add a validation constraint for the current <typeparamref name="TSource" />.
		/// </summary>
		/// <remarks>
		///     The <paramref name="constraintBuilder" /> receives the current name for the subject (mostly "it") and the
		///     current <see cref="ExpectationBuilder.ExpectationGrammars" /> of the <typeparamref name="TSource" />, as they
		///     are when the expectations are added and before they change for the <typeparamref name="TMember" />.
		/// </remarks>
		public MemberExpectationBuilder<TSource, TMember> Validate(
			Func<string, ExpectationGrammars, IValueConstraint<TSource>> constraintBuilder)
		{
			_sourceConstraintBuilder = constraintBuilder;
			return this;
		}
	}

	/// <summary>
	///     Creates the mapping nodes for the member of type <typeparamref name="TMember" />.
	/// </summary>
	internal abstract class MappingNodes<TMember>
	{
		/// <summary>
		///     Creates the mapping node whose expectations are typed at <typeparamref name="TMember" />.
		/// </summary>
		public MappingNode Create() => Create<TMember>();

		/// <summary>
		///     Creates the mapping node whose expectations are typed at <typeparamref name="TNarrowed" />.
		/// </summary>
		public abstract MappingNode Create<TNarrowed>();
	}

	private sealed class SyncMappingNodes<TSource, TMember>(
		MemberAccessor<TSource, TMember> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator)
		: MappingNodes<TMember>
	{
		public override MappingNode Create<TNarrowed>()
			=> new MappingNode<TSource, TMember, TNarrowed>(memberAccessor, expectationTextGenerator);
	}

	private sealed class AsyncMappingNodes<TSource, TMember>(
		MemberAccessor<TSource, Task<TMember>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator)
		: MappingNodes<TMember>
	{
		public override MappingNode Create<TNarrowed>()
			=> new MappingNode<TSource, TMember, TNarrowed>(memberAccessor, expectationTextGenerator);
	}
}

internal class ExpectationBuilder<TValue> : ExpectationBuilder
{
	/// <summary>
	///     The subject.
	/// </summary>
	private readonly IValueSource<TValue> _subjectSource;

	internal ExpectationBuilder(
		IValueSource<TValue> subjectSource,
		string subjectExpression)
		: base(subjectExpression)
	{
		_subjectSource = subjectSource;
	}

	/// <inheritdoc />
	/// <remarks>
	///     A value subject without timeout and cancellation is passed to the <paramref name="rootNode" /> without starting
	///     a state machine: the shared <see cref="EvaluationCancellation.None" /> neither times out nor is canceled, so
	///     everything after the evaluation would leave its result unchanged.
	/// </remarks>
	internal override ValueTask<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		if (timeout is null && !cancellationToken.CanBeCanceled &&
		    _subjectSource is ValueSource<TValue> { Value: not DelegateValue, } valueSource &&
		    !IsTracing)
		{
			context.Cancellation = EvaluationCancellation.None;
			return rootNode.IsMetBy(valueSource.Value, context, cancellationToken);
		}

		return IsMetAsync(rootNode, context, timeSystem, timeout, cancellationToken);
	}

	private async ValueTask<ConstraintResult> IsMetAsync(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		EvaluationCancellation cancellation = EvaluationCancellation.Create(timeout, cancellationToken, timeSystem);
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		context.Cancellation = cancellation;
		CancellationToken token = cancellation.Token;

		if (_subjectSource.IsNullTaskSubject)
		{
			ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
				ExpectationTextEvaluationContext.For(context), token);
			return NullSubjectResult.CreateForNullTaskSubject(expectation, default(TValue));
		}

		TValue data;
		try
		{
			data = await _subjectSource.GetValue(timeSystem, token);
		}
		catch (Exception exception)
		{
			AddOtherExceptions(_subjectSource.GetOtherExceptions(exception));
			ConstraintResult result = await FromException(rootNode, context, cancellation, exception);
			Tracing.WriteThrowingSubject(Subject);
			return result;
		}

		Tracing.WriteSubject(Subject, data, timeout);
		if (data is DelegateValue delegateValue)
		{
			AddOtherExceptions(delegateValue.OtherExceptions);
			if (cancellation.IsCanceledBy(delegateValue.Exception))
			{
				return await FromException(rootNode, context, cancellation, delegateValue.Exception!);
			}

			data = WithExceededTimeout(data, delegateValue, cancellation);
		}

		ConstraintResult constraintResult;
		try
		{
			constraintResult = await rootNode.IsMetBy(data, context, token);
		}
		catch (Exception exception) when (cancellation.HasTimedOut(exception) || cancellation.IsCanceledBy(exception))
		{
			return await FromException(rootNode, context, cancellation, exception);
		}

		return DecideByTimeout(constraintResult, cancellation);
	}

	/// <summary>
	///     Decides the undecided outcome of a constraint that stopped at the cancellation, when it was caused by the
	///     timeout.
	/// </summary>
	private static ConstraintResult DecideByTimeout(ConstraintResult result, EvaluationCancellation cancellation)
	{
		if (result.Outcome == Outcome.Undecided && cancellation.Timeout is { } timeout &&
		    cancellation.Reason == CancellationReason.Timeout)
		{
			return new ConstraintResult.FromException(result,
				CreateTimeoutException(timeout, new OperationCanceledException(cancellation.Token)),
				DefaultCurrentSubject, timeout);
		}

		return result;
	}

	/// <remarks>
	///     A synchronous delegate cannot be interrupted, so it also exceeded the timeout when it returns after the timeout
	///     elapsed without a cancellation. Its measured duration decides, as the timer of the timeout can fire late when
	///     the thread pool is busy.
	/// </remarks>
	private static TValue WithExceededTimeout(TValue data, DelegateValue delegateValue,
		EvaluationCancellation cancellation)
	{
		if (cancellation.Timeout is not { } timeout || delegateValue.IsNull)
		{
			return data;
		}

		bool isCanceled = cancellation.HasTimedOut(delegateValue.Exception);
		if (!isCanceled && delegateValue.Duration < timeout)
		{
			return data;
		}

		return (TValue)(object)delegateValue.WithExceededTimeout(timeout,
			CreateTimeoutException(timeout,
				delegateValue.Exception ?? new OperationCanceledException(cancellation.Token)),
			!isCanceled);
	}

	private static async Task<ConstraintResult> FromException(Node rootNode,
		EvaluationContext.EvaluationContext context,
		EvaluationCancellation cancellation,
		Exception exception)
	{
		ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
			ExpectationTextEvaluationContext.For(context), cancellation.Token);
		if (cancellation.Timeout is { } timeout && cancellation.HasTimedOut(exception))
		{
			return new ConstraintResult.FromException(expectation,
				CreateTimeoutException(timeout, exception), DefaultCurrentSubject, timeout);
		}

		return cancellation.IsCanceledBy(exception)
			? new ConstraintResult.FromCancellation(expectation)
			: new ConstraintResult.FromException(expectation, exception, DefaultCurrentSubject);
	}

	/// <summary>
	///     Replaces the cancellation caused by the <paramref name="timeout" />, so that it is reported the same way,
	///     whether the awaited task reacted to the cancellation itself or was abandoned.
	/// </summary>
	internal static TimeoutException CreateTimeoutException(TimeSpan timeout, Exception cancellation)
		=> new($"The operation did not finish within {Formatter.Format(timeout)}.", cancellation);
}
