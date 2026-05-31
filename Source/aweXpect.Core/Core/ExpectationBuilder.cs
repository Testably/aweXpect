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

	private ResultContexts? _contexts;

	/// <summary>
	///     The current name for the subject (defaults to <see cref="DefaultCurrentSubject" />).
	/// </summary>
	private string _it = DefaultCurrentSubject;

	private Node _node = new ExpectationNode();

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
		new((expectationBuilderCallback, expectationGrammar, sourceConstraintCallback) =>
		{
			if (sourceConstraintCallback is not null)
			{
				IValueConstraint<TSource> constraint = sourceConstraintCallback.Invoke(_it, ExpectationGrammars);
				_node.AddConstraint(constraint);
			}

			Node root = _node;
			_node = _node.AddMapping(memberAccessor, expectationTextGenerator);
			if (replaceIt)
			{
				_it = memberAccessor.ToString().Trim();
			}

			if (expectationGrammar != null)
			{
				ExpectationGrammars previousGrammars = ExpectationGrammars;
				ExpectationGrammars = expectationGrammar(ExpectationGrammars);
				expectationBuilderCallback.Invoke(this);
				ExpectationGrammars = previousGrammars;
			}
			else
			{
				expectationBuilderCallback.Invoke(this);
			}

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
		new((expectationBuilderCallback, expectationGrammar, sourceConstraintCallback) =>
		{
			if (sourceConstraintCallback is not null)
			{
				IValueConstraint<TSource> constraint = sourceConstraintCallback.Invoke(_it, ExpectationGrammars);
				_node.AddConstraint(constraint);
			}

			Node root = _node;
			_node = _node.AddAsyncMapping(memberAccessor, expectationTextGenerator);
			if (replaceIt)
			{
				_it = memberAccessor.ToString().Trim();
			}

			if (expectationGrammar != null)
			{
				ExpectationGrammars previousGrammars = ExpectationGrammars;
				ExpectationGrammars = expectationGrammar(ExpectationGrammars);
				expectationBuilderCallback.Invoke(this);
				ExpectationGrammars = previousGrammars;
			}
			else
			{
				expectationBuilderCallback.Invoke(this);
			}

			_node = root;
			if (replaceIt)
			{
				_it = DefaultCurrentSubject;
			}

			return this;
		});

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
	///     Adds a <paramref name="reason" /> to the current expectation constraint.
	/// </summary>
	internal void AddReason(string reason)
	{
		BecauseReason becauseReason = new(reason);
		_node.SetReason(becauseReason);
	}

	/// <summary>
	///     Adds a <paramref name="reason" /> to the current expectation constraint.
	/// </summary>
	internal void AddReason(Task<string?> reason)
	{
		AsyncBecauseReason becauseReason = new(reason);
		_node.SetReason(becauseReason);
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
	public ExpectationBuilder ForWhich<TSource, TTarget>(
		Func<TSource, TTarget?> memberAccessor,
		string? separator = null,
		string? replaceIt = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammar = null)
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

		if (expectationGrammar != null)
		{
			ExpectationGrammars = expectationGrammar(ExpectationGrammars);
		}

		_whichNode = new WhichNode<TSource, TTarget>(parentNode, memberAccessor, separator);
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
	///     Adds the <paramref name="resultContext" /> to the context that is included in the failure message.
	/// </summary>
	public virtual ExpectationBuilder AddContext(ResultContext resultContext)
	{
		_contexts ??= new ResultContexts();
		_contexts.Add(resultContext);
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

	internal Task<ConstraintResult> IsMet()
	{
		EvaluationContext.EvaluationContext context = new();
		ITimeSystem timeSystem = _timeSystem ?? RealTimeSystem.Instance;
		TestCancellation? testCancellation = Customize.aweXpect.Settings().TestCancellation.Get();
		CancellationToken cancellationToken = CancellationToken ??
		                                      testCancellation?.CancellationTokenFactory?.Invoke() ??
		                                      System.Threading.CancellationToken.None;
		return IsMet(GetRootNode(), context, timeSystem, Timeout ?? testCancellation?.Timeout,
			cancellationToken);
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
				ExpectationBuilder>
			_callback;

		private Func<string, ExpectationGrammars, IValueConstraint<TSource>>? _sourceConstraintBuilder;

		internal MemberExpectationBuilder(Func<
				Action<ExpectationBuilder>,
				Func<ExpectationGrammars, ExpectationGrammars>?,
				Func<string, ExpectationGrammars, IValueConstraint<TSource>>?,
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
			=> _callback(expectation, expectationGrammars, _sourceConstraintBuilder);

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
			ConstraintResult expectation = await rootNode.IsMetBy(default(TValue), context, cancellationToken);
			Customize.aweXpect.TraceWriter.Value?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			return new ConstraintResult.FromException(expectation, exception, this);
		}

		return await rootNode.IsMetBy(data, context, cancellationToken);
	}
}
