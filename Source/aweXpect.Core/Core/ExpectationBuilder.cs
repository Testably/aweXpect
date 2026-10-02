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

	private ResultContexts? _contexts;

	private EvaluationContext.EvaluationContext? _evaluationContext;

	/// <summary>
	///     The current name for the subject (defaults to <see cref="DefaultCurrentSubject" />).
	/// </summary>
	private string _it = DefaultCurrentSubject;

	private Node _node = new ExpectationNode();

	private List<IBecauseReason>? _reasons;

	private ITimeSystem? _timeSystem;

	/// <summary>
	///     The which node that still waits for the expectations on its member, and the root node that contains it.
	/// </summary>
	private (Node WhichNode, Node Root)? _pendingWhich;

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
	public CancellationToken? CancellationToken { get; private set; }

	/// <summary>
	///     The explicit timeout to be applied to the expectation.
	/// </summary>
	/// <remarks>
	///     It is the tightest of all timeouts added with <see cref="WithTimeout(TimeSpan)" />. The evaluation is also
	///     limited by the timeout from <see cref="AwexpectCustomization.SettingsCustomization.TestCancellation" />, if
	///     it is tighter.
	/// </remarks>
	public TimeSpan? Timeout { get; private set; }

	/// <summary>
	///     The expected grammatical form of the expectation text.
	/// </summary>
	public ExpectationGrammars ExpectationGrammars { get; private set; }

	internal string Subject { get; }

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
		if (replaceIt)
		{
			_it = memberAccessor.ToString().Trim();
			memberNode.ContextMember = _it;
		}

		(Node WhichNode, Node Root)? outerPendingWhich = _pendingWhich;
		_pendingWhich = null;

		ExpectationGrammars previousGrammars = ExpectationGrammars;
		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammars?.Invoke(memberGrammars) ?? memberGrammars;
		int outerReasonCount = _reasons?.Count ?? 0;
		expectations.Invoke(this);
		ExpectationGrammars = previousGrammars;

		CompleteWhichNode();
		_pendingWhich = outerPendingWhich;
		ThrowIfEmpty(_node, "expectations");
		mappingNode.AddNode(_node);
		MoveReasonsTo(mappingNode, outerReasonCount);
		_node = root;
		if (replaceIt)
		{
			_it = DefaultCurrentSubject;
		}

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
	public void WithCancellation(CancellationToken cancellationToken)
		=> CancellationToken = cancellationToken;

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
		Timeout = TimerHelpers.Tighter(Timeout, timeout);
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
	/// </remarks>
	internal async Task<ConstraintResult> ApplyReasons(ConstraintResult result)
	{
		if (_reasons is not null)
		{
			foreach (IBecauseReason reason in _reasons)
			{
				result = await reason.ApplyTo(result);
			}
		}

		return result;
	}

	/// <summary>
	///     The reasons of the expectation.
	/// </summary>
	private protected IEnumerable<IBecauseReason> Reasons => _reasons ?? [];

	/// <summary>
	///     Resolves the reasons that must be awaited, so that their message is available.
	/// </summary>
	internal async Task ResolveReasons()
	{
		foreach (AsyncBecauseReason reason in _reasons?.OfType<AsyncBecauseReason>() ?? [])
		{
			await reason.Resolve();
		}
	}

	/// <summary>
	///     Supports chaining for subsequent expectation constraints with the <paramref name="textSeparator" />.
	/// </summary>
	public ExpectationBuilder And(string textSeparator = " and ")
	{
		if (_node is AndNode andNode)
		{
			andNode.AddNode(new ExpectationNode(), textSeparator);
		}
		else if (_node is OrNode orNode)
		{
			AndNode newNode = new(orNode.Current);
			newNode.AddNode(new ExpectationNode(), textSeparator);
			orNode.Current = newNode;
		}
		else
		{
			AndNode newNode = new(_node);
			newNode.AddNode(new ExpectationNode(), textSeparator);
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
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, TTarget?> memberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false)
	{
		AddWhichNode(parentNode => new WhichNode<TSource, TTarget>(parentNode, memberAccessor, separator,
			negateMemberOnly, subjectName));

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
		_pendingWhich = (whichNode!, root);
		_node = new ExpectationNode();
	}

	/// <summary>
	///     Update the list of <see cref="ResultContext" /> that is included in the failure message.
	/// </summary>
	public virtual ExpectationBuilder UpdateContexts(Action<ResultContexts> callback)
	{
		_contexts ??= new ResultContexts();
		callback(_contexts);
		return this;
	}

	/// <summary>
	///     Adds the <paramref name="resultContext" /> to the context that is included in the failure message,
	///     unless a context with the same <see cref="ResultContext.Title" /> was already added.
	/// </summary>
	/// <remarks>
	///     A constraint adds its context while it is evaluated, so an expectation that inspects the same property
	///     twice (<c>HasMessage().Containing("a").And.HasMessage().Containing("b")</c>) would otherwise repeat the
	///     identical block. <see cref="UpdateContexts(Action{ResultContexts})" /> bypasses this and can add a
	///     duplicate title deliberately.
	/// </remarks>
	public virtual ExpectationBuilder AddContext(ResultContext resultContext)
	{
		_contexts ??= new ResultContexts();
		if (!_contexts.ContainsTitle(resultContext.Title))
		{
			_contexts.Add(resultContext);
		}

		return this;
	}

	/// <summary>
	///     Adds the <paramref name="otherExceptions" /> of a faulted task to the context, unless they are
	///     <see langword="null" />.
	/// </summary>
	internal void AddOtherExceptions(Exception[]? otherExceptions)
	{
		if (otherExceptions is not null)
		{
			AddContext(CreateOtherExceptionsContext(otherExceptions));
		}
	}

	/// <remarks>
	///     A separate method, so that the closure over the <paramref name="otherExceptions" /> is only allocated when
	///     there are any, and not for every delegate subject.
	/// </remarks>
	private static ResultContext.SyncCallback CreateOtherExceptionsContext(Exception[] otherExceptions)
		=> new ResultContext.SyncCallback("Other exceptions",
			() => Formatter.Format(otherExceptions, FormattingOptions.MultipleLines));

	/// <summary>
	///     Gets the list of <see cref="ResultContext" />.
	/// </summary>
	internal IEnumerable<ResultContext> GetContexts() => _contexts ?? [];

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" />.
	/// </summary>
	internal Task<string> FromFailure(ConstraintResult failure)
		=> FromFailure(Subject, failure, _contexts, CancellationToken ?? System.Threading.CancellationToken.None);

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" />.
	/// </summary>
	private static async Task<string> FromFailure(
		string subject,
		ConstraintResult failure,
		ResultContexts? contexts,
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
		await ResultContextRenderer.AppendContexts(sb, failure, contexts ?? [], cancellationToken);
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
		if (_pendingWhich is (var whichNode, var root))
		{
			whichNode.AddNode(_node);
			_node = root;
			_pendingWhich = null;
		}
	}

	/// <remarks>
	///     A failure keeps the collections materialized during the evaluation until <see cref="EndEvaluation" />, as the
	///     failure message still reads them.
	/// </remarks>
	internal async Task<ConstraintResult> IsMet()
	{
		await EndEvaluation();
		EvaluationContext.EvaluationContext context = new(this);
		_evaluationContext = context;
		ConstraintResult result;
		try
		{
			ITimeSystem timeSystem = _timeSystem ?? RealTimeSystem.Instance;
			TestCancellation? testCancellation = Customize.aweXpect.Settings().TestCancellation.Get();
			CancellationToken cancellationToken = CancellationToken ??
			                                      testCancellation?.CancellationTokenFactory?.Invoke() ??
			                                      System.Threading.CancellationToken.None;
			TimeSpan? timeout = TimerHelpers.Tighter(Timeout, testCancellation?.Timeout);
			result = await IsMet(GetRootNode(), context, timeSystem,
				timeout == System.Threading.Timeout.InfiniteTimeSpan ? null : timeout,
				cancellationToken);
			if (_reasons is not null)
			{
				result = await ApplyReasons(result);
			}
		}
		catch
		{
			await EndEvaluation();
			throw;
		}

		if (result.Outcome == Outcome.Success)
		{
			await EndEvaluation();
		}

		return result;
	}

	/// <summary>
	///     Releases the sources of the collections materialized during the current evaluation, once the failure message
	///     no longer reads them.
	/// </summary>
	internal async Task EndEvaluation()
	{
		if (_evaluationContext is not null)
		{
			EvaluationContext.EvaluationContext context = _evaluationContext;
			_evaluationContext = null;
			await context.ReleaseMaterializations();
		}
	}

	internal abstract Task<ConstraintResult> IsMet(Node rootNode,
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

		private readonly MappingNodes<TSource, TMember> _mappingNodes;

		private Func<string, ExpectationGrammars, IValueConstraint<TSource>>? _sourceConstraintBuilder;

		internal MemberExpectationBuilder(Func<
				Action<ExpectationBuilder>,
				Func<ExpectationGrammars, ExpectationGrammars>?,
				Func<string, ExpectationGrammars, IValueConstraint<TSource>>?,
				Func<MappingNode>,
				ExpectationBuilder>
			callback,
			MappingNodes<TSource, TMember> mappingNodes)
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
	internal abstract class MappingNodes<TSource, TMember>
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
		: MappingNodes<TSource, TMember>
	{
		public override MappingNode Create<TNarrowed>()
			=> new MappingNode<TSource, TMember, TNarrowed>(memberAccessor, expectationTextGenerator);
	}

	private sealed class AsyncMappingNodes<TSource, TMember>(
		MemberAccessor<TSource, Task<TMember>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator)
		: MappingNodes<TSource, TMember>
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
	internal override async Task<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		EvaluationCancellation cancellation = EvaluationCancellation.Create(timeout, cancellationToken);
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		context.Cancellation = cancellation;
		CancellationToken token = cancellation.Token;

		if (_subjectSource is AsyncValueSource<TValue> { IsNullTask: true, })
		{
			ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
				EvaluationContext.ExpectationTextEvaluationContext.For(context), token);
			return NullSubjectResult.CreateForNullTaskSubject(expectation, default(TValue));
		}

		TValue data;
		try
		{
			data = await _subjectSource.GetValue(timeSystem, token);
			Customize.aweXpect.TraceWriter?.WriteMessage(timeout is null
				? $"Checking expectation for {Subject} {data}"
				: $"Checking expectation for {Subject} {data} with timeout of {Formatter.Format(timeout)}");
		}
		catch (Exception exception)
		{
			AddOtherExceptions((_subjectSource as AsyncValueSource<TValue>)?.GetOtherExceptions(exception));
			ConstraintResult result = await FromException(rootNode, context, cancellation, exception);
			Customize.aweXpect.TraceWriter?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			return result;
		}

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

	private static TValue WithExceededTimeout(TValue data, DelegateValue delegateValue,
		EvaluationCancellation cancellation)
	{
		if (cancellation.Timeout is { } timeout && cancellation.HasTimedOut(delegateValue.Exception))
		{
			return (TValue)(object)delegateValue.WithExceededTimeout(timeout,
				CreateTimeoutException(timeout, delegateValue.Exception!));
		}

		return data;
	}

	private static async Task<ConstraintResult> FromException(Node rootNode,
		EvaluationContext.EvaluationContext context,
		EvaluationCancellation cancellation,
		Exception exception)
	{
		ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
			EvaluationContext.ExpectationTextEvaluationContext.For(context), cancellation.Token);
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
