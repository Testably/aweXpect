using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
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
	private const string DefaultCurrentSubject = "it";

	private static readonly AsyncLocal<long> Evaluation = new();

	private static long _evaluationCount;

	private ResultContexts? _contexts;

	/// <summary>
	///     The current name for the subject (defaults to <see cref="DefaultCurrentSubject" />).
	/// </summary>
	private string _it = DefaultCurrentSubject;

	private Node _node = new ExpectationNode();

	private List<IBecauseReason>? _reasons;

	private ITimeSystem? _timeSystem;

	private Node? _whichNode;

	/// <summary>
	///     Initializes the <see cref="ExpectationBuilder" /> with the <paramref name="subjectExpression" />
	///     for the statement builder.
	/// </summary>
	protected ExpectationBuilder(string subjectExpression, ExpectationGrammars grammars = ExpectationGrammars.None)
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
	///     <see cref="AwexpectCustomization.SettingsCustomization.TestCancellation" />
	/// </remarks>
	public CancellationToken? CancellationToken { get; private set; }

	/// <summary>
	///     The explicit timeout to be applied to the expectation.
	/// </summary>
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
	///     "it").
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
	public MemberExpectationBuilder<TSource, TTarget> ForMember<TSource, TTarget>(
		MemberAccessor<TSource, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null,
		bool replaceIt = true) =>
		new((expectationBuilderCallback, expectationGrammar, sourceConstraintCallback, addMappingCallback, _) =>
		{
			if (sourceConstraintCallback is not null)
			{
				IValueConstraint<TSource> constraint = sourceConstraintCallback.Invoke(_it, ExpectationGrammars);
				_node.AddConstraint(constraint);
			}

			Node root = _node;
			Node mappingNode = addMappingCallback is null
				? _node.AddMapping(memberAccessor, expectationTextGenerator)
				: addMappingCallback.Invoke(_node, memberAccessor, expectationTextGenerator);
			_node = new ExpectationNode();
			if (replaceIt)
			{
				_it = memberAccessor.ToString().Trim();
			}

			Node? outerWhichNode = _whichNode;
			_whichNode = null;

			ExpectationGrammars previousGrammars = ExpectationGrammars;
			ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
			ExpectationGrammars = expectationGrammar?.Invoke(memberGrammars) ?? memberGrammars;
			expectationBuilderCallback.Invoke(this);
			ExpectationGrammars = previousGrammars;

			CompleteWhichNode();
			_whichNode = outerWhichNode;
			ThrowIfEmpty(_node, "expectations");
			mappingNode.AddNode(_node);
			_node = root;
			if (replaceIt)
			{
				_it = DefaultCurrentSubject;
			}

			return this;
		});

	/// <summary>
	///     Specifies a constraint that applies to the member selected asynchronously
	///     by the <paramref name="memberAccessor" />.
	/// </summary>
	public MemberExpectationBuilder<TSource, TTarget> ForAsyncMember<TSource, TTarget>(
		MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null,
		bool replaceIt = true) =>
		new((expectationBuilderCallback, expectationGrammar, sourceConstraintCallback, _, addAsyncMappingCallback) =>
		{
			if (sourceConstraintCallback is not null)
			{
				IValueConstraint<TSource> constraint = sourceConstraintCallback.Invoke(_it, ExpectationGrammars);
				_node.AddConstraint(constraint);
			}

			Node root = _node;
			Node mappingNode = addAsyncMappingCallback is null
				? _node.AddAsyncMapping(memberAccessor, expectationTextGenerator)
				: addAsyncMappingCallback.Invoke(_node, memberAccessor, expectationTextGenerator);
			_node = new ExpectationNode();
			if (replaceIt)
			{
				_it = memberAccessor.ToString().Trim();
			}

			Node? outerWhichNode = _whichNode;
			_whichNode = null;

			ExpectationGrammars previousGrammars = ExpectationGrammars;
			ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
			ExpectationGrammars = expectationGrammar?.Invoke(memberGrammars) ?? memberGrammars;
			expectationBuilderCallback.Invoke(this);
			ExpectationGrammars = previousGrammars;

			CompleteWhichNode();
			_whichNode = outerWhichNode;
			ThrowIfEmpty(_node, "expectations");
			mappingNode.AddNode(_node);
			_node = root;
			if (replaceIt)
			{
				_it = DefaultCurrentSubject;
			}

			return this;
		});

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
	public void WithTimeout(TimeSpan timeout)
		=> Timeout = timeout;

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
	///     Set <paramref name="negateMemberOnly" /> when the previous expectation only navigates to the member, so that
	///     a negation applies to the member expectation ("has keys which do not contain 0") instead of the whole
	///     expectation ("does not have a single item which is equal to 3").
	/// </remarks>
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, TTarget?> memberAccessor,
		string? separator = null,
		string? replaceIt = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammar = null,
		bool negateMemberOnly = false)
	{
		if (_whichNode != null)
		{
			_whichNode.AddNode(_node);
			_node = _whichNode;
			_whichNode = null;
		}

		Node? parentNode = null;
		if (_node is not ExpectationNode e || !e.IsEmpty())
		{
			parentNode = _node;
			_node = new ExpectationNode();
		}

		if (replaceIt != null)
		{
			_it = replaceIt;
		}

		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammar?.Invoke(memberGrammars) ?? memberGrammars;

		_whichNode = new WhichNode<TSource, TTarget>(parentNode, memberAccessor, separator, negateMemberOnly);
		return this;
	}

	/// <summary>
	///     Specifies a mapping to add expectations on the member from the <paramref name="asyncMemberAccessor" />.
	/// </summary>
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, Task<TTarget?>> asyncMemberAccessor,
		string? separator = null)
	{
		if (_whichNode != null)
		{
			_whichNode.AddNode(_node);
			_node = _whichNode;
			_whichNode = null;
		}

		Node? parentNode = null;
		if (_node is not ExpectationNode e || !e.IsEmpty())
		{
			parentNode = _node;
			_node = new ExpectationNode();
		}

		ExpectationGrammars &= ~ExpectationGrammars.Introduced;
		_whichNode = new WhichNode<TSource, TTarget>(parentNode, asyncMemberAccessor, separator);
		return this;
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
		if (!_contexts.Any(existing
			    => string.Equals(existing.Title, resultContext.Title, StringComparison.Ordinal)))
		{
			_contexts.Add(resultContext);
		}

		return this;
	}

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
		if (contexts is not null)
		{
			foreach (ResultContext context in contexts.OrderByDescending(x => x.Priority))
			{
				string? content = await context.GetContent(cancellationToken);
				if (content is null)
				{
					continue;
				}

				sb.AppendLine().AppendLine();
				sb.Append(context.Title).Append(':').AppendLine();
				sb.Append(content);
			}
		}

		return sb.ToString();
	}

	internal Node GetRootNode()
	{
		if (_whichNode != null)
		{
			_whichNode.AddNode(_node);
			_node = _whichNode;
			_whichNode = null;
		}

		return _node;
	}

	/// <summary>
	///     Attaches the current node to a pending <see cref="WhichNode{TSource,TMember}" />, so that it is evaluated on
	///     the projected member.
	/// </summary>
	private void CompleteWhichNode()
	{
		if (_whichNode != null)
		{
			_whichNode.AddNode(_node);
			_node = _whichNode;
			_whichNode = null;
		}
	}

	/// <summary>
	///     Identifies the evaluation that is currently running, or <c>0</c> outside of one.
	/// </summary>
	/// <remarks>
	///     All constraints of one awaited expectation share it, which is what lets a stateful subject tell a further
	///     constraint of the same expectation from a further expectation. An <c>async</c> method does not flow the
	///     value back to its caller, so it stays scoped to the one evaluation that set it.
	/// </remarks>
	internal static long CurrentEvaluation => Evaluation.Value;

	internal async Task<ConstraintResult> IsMet()
	{
		Evaluation.Value = Interlocked.Increment(ref _evaluationCount);
		EvaluationContext.EvaluationContext context = new();
		ITimeSystem timeSystem = _timeSystem ?? RealTimeSystem.Instance;
		TestCancellation? testCancellation = Customize.aweXpect.Settings().TestCancellation.Get();
		CancellationToken cancellationToken = CancellationToken ??
		                                      testCancellation?.CancellationTokenFactory?.Invoke() ??
		                                      System.Threading.CancellationToken.None;
		ConstraintResult result = await IsMet(GetRootNode(), context, timeSystem, Timeout ?? testCancellation?.Timeout,
			cancellationToken);
		return await ApplyReasons(result);
	}

	internal abstract Task<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken);

	internal void Or(string textSeparator = " or ")
	{
		if (_node is OrNode orNode)
		{
			orNode.AddNode(new ExpectationNode(), textSeparator);
			return;
		}

		OrNode newNode = new(_node);
		newNode.AddNode(new ExpectationNode(), textSeparator);
		_node = newNode;
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
				Func<Node, MemberAccessor<TSource, TMember>, Action<MemberAccessor, StringBuilder>?, Node>?,
				Func<Node, MemberAccessor<TSource, Task<TMember>>, Action<MemberAccessor, StringBuilder>?, Node>?,
				ExpectationBuilder>
			_callback;

		private Func<string, ExpectationGrammars, IValueConstraint<TSource>>? _sourceConstraintBuilder;

		internal MemberExpectationBuilder(Func<
				Action<ExpectationBuilder>,
				Func<ExpectationGrammars, ExpectationGrammars>?,
				Func<string, ExpectationGrammars, IValueConstraint<TSource>>?,
				Func<Node, MemberAccessor<TSource, TMember>, Action<MemberAccessor, StringBuilder>?, Node>?,
				Func<Node, MemberAccessor<TSource, Task<TMember>>, Action<MemberAccessor, StringBuilder>?, Node>?,
				ExpectationBuilder>
			callback)
		{
			_callback = callback;
		}

		/// <summary>
		///     Add expectations for the current <typeparamref name="TMember" />.
		/// </summary>
		public ExpectationBuilder AddExpectations(
			Action<ExpectationBuilder> expectation,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			=> _callback(expectation, expectationGrammars, _sourceConstraintBuilder, null, null);

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
			=> _callback(expectation, expectationGrammars, _sourceConstraintBuilder,
				(node, memberAccessor, expectationTextGenerator)
					=> node.AddNarrowingMapping<TSource, TMember, TNarrowed>(memberAccessor, expectationTextGenerator),
				(node, memberAccessor, expectationTextGenerator)
					=> node.AddAsyncNarrowingMapping<TSource, TMember, TNarrowed>(memberAccessor,
						expectationTextGenerator));

		/// <summary>
		///     Add a validation constraint for the current <typeparamref name="TSource" />.
		/// </summary>
		/// <remarks>
		///     The parameter passed to the <paramref name="constraintBuilder" /> is the current name for the subject (mostly
		///     "it").
		/// </remarks>
		public MemberExpectationBuilder<TSource, TMember> Validate(
			Func<string, ExpectationGrammars, IValueConstraint<TSource>> constraintBuilder)
		{
			_sourceConstraintBuilder = constraintBuilder;
			return this;
		}
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
		if (timeout != null)
		{
			using CancellationTokenSource timeoutCts = CancellationTokenSource
				.CreateLinkedTokenSource(cancellationToken);
			timeoutCts.CancelAfter(timeout.Value);
			CancellationToken token = timeoutCts.Token;
			TValue dataWithTimeout = await _subjectSource.GetValue(timeSystem, token);
			Customize.aweXpect.TraceWriter.Value?.WriteMessage(
				$"Checking expectation for {Subject} {dataWithTimeout} with timeout of {Formatter.Format(timeout)}");
			return await rootNode.IsMetBy(dataWithTimeout, context, token);
		}

		TValue data;
		try
		{
			data = await _subjectSource.GetValue(timeSystem, cancellationToken);
			Customize.aweXpect.TraceWriter.Value?.WriteMessage($"Checking expectation for {Subject} {data}");
		}
		catch (Exception exception)
		{
			ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
				EvaluationContext.ExpectationTextEvaluationContext.For(context), cancellationToken);
			Customize.aweXpect.TraceWriter.Value?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			return new ConstraintResult.FromException(expectation, exception);
		}

		return await rootNode.IsMetBy(data, context, cancellationToken);
	}
}
