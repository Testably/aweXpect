using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Initialization;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Core;

/// <summary>
///     The builder for collecting all expectations.
/// </summary>
public abstract class ExpectationBuilder
{
	private protected const string DefaultCurrentSubject = "it";

	private EvaluationContext.EvaluationContext? _evaluationContext;

	/// <summary>
	///     The current name for the subject (defaults to <see cref="DefaultCurrentSubject" />).
	/// </summary>
	private string _it = DefaultCurrentSubject;

	/// <summary>
	///     The current node, or the first constraint until a node is needed.
	/// </summary>
	/// <remarks>
	///     Most expectations consist of a single constraint, which is evaluated without a node, see
	///     <see cref="TryEvaluateRightAway" />. Keeping it in the same field as the node keeps the builder small.
	/// </remarks>
	private object? _nodeOrConstraint;

	/// <summary>
	///     The settings that only few expectations use, or <see langword="null" /> when none was set.
	/// </summary>
	/// <remarks>
	///     Every expectation allocates a builder, so keeping these settings in an object of their own, which is only
	///     allocated when one is set, keeps every expectation 56 bytes smaller. An expectation without them can be
	///     evaluated right away, see <see cref="TryEvaluateRightAway" />.
	/// </remarks>
	private RareSettings? _rareSettings;

	/// <summary>
	///     The reasons of the expectation: <see langword="null" />, the text of a single reason, or the list of reasons.
	/// </summary>
	/// <remarks>
	///     Most expectations have at most one reason, which is kept as its text until the reasons are needed as a list,
	///     see <see cref="ReasonList" />.
	/// </remarks>
	private object? _reasons;

	private readonly string _subjectExpression;

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
		_subjectExpression = subjectExpression;
		ExpectationGrammars = grammars;
	}

	/// <summary>
	///     Initializes the <see cref="ExpectationBuilder" /> with an empty <see cref="Subject" />.
	/// </summary>
	private protected ExpectationBuilder()
	{
		AweXpectInitialization.EnsureInitialized();
		_subjectExpression = "";
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
	public CancellationToken? CancellationToken => _rareSettings?.CancellationToken;

	/// <summary>
	///     The explicit timeout to be applied to the expectation.
	/// </summary>
	/// <remarks>
	///     It is the tightest of all timeouts added with <see cref="WithTimeout(TimeSpan)" />. The evaluation is also
	///     limited by the timeout from <see cref="AwexpectCustomization.SettingsCustomization.TestCancellation" />, if
	///     it is tighter.<br />
	///     The limits of <c>ExecutesIn()</c> and of <c>Within(…)</c> after <c>Throws…()</c> are not included: they are
	///     kept apart, so that a later limit can replace an earlier one, and the tighter of them and this timeout
	///     applies when the evaluation starts.
	/// </remarks>
	public TimeSpan? Timeout => _rareSettings?.Timeout;

	/// <summary>
	///     The expected grammatical form of the expectation text.
	/// </summary>
	public ExpectationGrammars ExpectationGrammars { get; private set; }

	/// <remarks>
	///     The expression is only trimmed when it is read, e.g. for a failure message, because trimming a multi-line
	///     expression costs more than a met expectation. It is not kept, as it is rarely read more than once and every
	///     expectation allocates a builder.
	/// </remarks>
	internal string Subject => _subjectExpression.TrimCommonWhiteSpace();

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
	private protected IEnumerable<IBecauseReason> Reasons => ReasonList ?? [];

	/// <summary>
	///     The current node, which is created with the first constraint when it is first needed.
	/// </summary>
	private protected Node CurrentNode
	{
		get
		{
			if (_nodeOrConstraint is Node node)
			{
				return node;
			}

			ExpectationNode expectationNode = new();
			if (_nodeOrConstraint is IConstraint constraint)
			{
				expectationNode.AddConstraint(constraint, DefaultCurrentSubject);
			}

			_nodeOrConstraint = expectationNode;
			return expectationNode;
		}
		set => _nodeOrConstraint = value;
	}

	/// <summary>
	///     The reasons of the expectation as a list, or <see langword="null" /> when there are none.
	/// </summary>
	/// <remarks>
	///     A single reason that is kept as its text is turned into the list on first access.
	/// </remarks>
	private List<IBecauseReason>? ReasonList
	{
		get
		{
			if (_reasons is string reason)
			{
				List<IBecauseReason> reasons = [new BecauseReason(reason),];
				_reasons = reasons;
				return reasons;
			}

			return (List<IBecauseReason>?)_reasons;
		}
	}

	/// <summary>
	///     The which node that still waits for the expectations on its member, or <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     Kept in the <see cref="RareSettings" />, like the other state that only few expectations use, so that every
	///     expectation stays smaller.
	/// </remarks>
	private Node? PendingWhichNode
	{
		get => _rareSettings?.PendingWhichNode;
		set
		{
			if (value is not null || _rareSettings is not null)
			{
				GetRareSettings().PendingWhichNode = value;
			}
		}
	}

	/// <summary>
	///     The root node that contains the <see cref="PendingWhichNode" />.
	/// </summary>
	private Node? PendingWhichRoot
	{
		get => _rareSettings?.PendingWhichRoot;
		set
		{
			if (value is not null || _rareSettings is not null)
			{
				GetRareSettings().PendingWhichRoot = value;
			}
		}
	}

	/// <summary>
	///     The subject contexts of the current subject, or <see langword="null" /> when none were added.
	/// </summary>
	/// <remarks>
	///     Only while a which node is pending and the subject that it continues has subject contexts, they are held
	///     together with those of the current subject in <see cref="SubjectContextScopes" />, so that the common case
	///     allocates no further object.
	/// </remarks>
	private object? SubjectContexts
	{
		get => _rareSettings?.SubjectContexts;
		set
		{
			if (value is not null || _rareSettings is not null)
			{
				GetRareSettings().SubjectContexts = value;
			}
		}
	}

	/// <summary>
	///     Adds the <paramref name="constraint" /> to the current node, or keeps it without a node, when it is the first
	///     one on the subject itself.
	/// </summary>
	private void AddToNode(IConstraint constraint)
	{
		if (_nodeOrConstraint is null && _it == DefaultCurrentSubject)
		{
			_nodeOrConstraint = constraint;
			return;
		}

		CurrentNode.AddConstraint(constraint, _it);
	}

	/// <summary>
	///     Adds the constraint from the <paramref name="constraintBuilder" /> and returns it as
	///     <paramref name="constraint" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="constraint" /> can then be the source of the options of the result, so that the
	///     <see cref="OptionsHolder{TOptions}" /> is only created when an option is set, see
	///     <see cref="IOptionsHolderSource{TOptions}" />.
	/// </remarks>
	public ExpectationBuilder AddConstraint<TState, TConstraint>(
		TState state,
		Func<TState, string, ExpectationGrammars, TConstraint> constraintBuilder,
		out TConstraint constraint)
		where TConstraint : IConstraint
	{
		constraint = constraintBuilder(state, _it, ExpectationGrammars);
		AddToNode(constraint);
		return this;
	}

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
		AddToNode(constraintBuilder(_it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(_it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(_it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(_it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, this, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, _it, ExpectationGrammars));
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
		AddToNode(constraintBuilder(state, this, _it, ExpectationGrammars));
		return this;
	}

	/// <summary>
	///     Adds the contexts from <paramref name="appendContexts" /> to the failure of any expectation on the current
	///     subject, e.g. to show the whole subject whatever expectation on it fails.
	/// </summary>
	/// <remarks>
	///     The current subject is the value of the expectation or, within the expectations on a member, the member, so its
	///     contexts are labelled with it. They also cover the expectations that were added before, and precede the
	///     contexts of the expectations.
	///     <para />
	///     The <paramref name="appendContexts" /> is only called while the failure message is created and only when the
	///     value is a <typeparamref name="TValue" />. Adding the same callback again for the same subject has no effect,
	///     so pass a cached delegate, e.g. from a <see langword="static" /> field.
	/// </remarks>
	public ExpectationBuilder AddSubjectContexts<TValue>(Action<TValue, ResultContextCollector> appendContexts)
	{
		if (CurrentSubjectContexts is not { } last)
		{
			CurrentSubjectContexts = new SubjectContext<TValue>(appendContexts);
			return this;
		}

		while (!last.Callback.Equals(appendContexts))
		{
			if (last.Next is null)
			{
				last.Next = new SubjectContext<TValue>(appendContexts);
				break;
			}

			last = last.Next;
		}

		return this;
	}

	/// <summary>
	///     The subject contexts of the current subject, see <see cref="SubjectContexts" />.
	/// </summary>
	private SubjectContext? CurrentSubjectContexts
	{
		get => SubjectContexts is SubjectContextScopes scopes ? scopes.Current : (SubjectContext?)SubjectContexts;
		set
		{
			if (SubjectContexts is SubjectContextScopes scopes)
			{
				scopes.Current = value;
			}
			else
			{
				SubjectContexts = value;
			}
		}
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
		=> new(this, memberAccessor, expectationTextGenerator, replaceIt);

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
		=> new(this, memberAccessor, expectationTextGenerator, replaceIt);

	/// <inheritdoc cref="ForAsyncMember{TSource, TTarget}(MemberAccessor{TSource, Task{TTarget}}, Action{MemberAccessor, StringBuilder}?, bool)" />
	public MemberExpectationBuilder<TSource, TTarget> ForAsyncMember<TSource, TTarget>(
		MemberAccessor<TSource, ValueTask<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null,
		bool replaceIt = true)
		=> new(this, memberAccessor, expectationTextGenerator, replaceIt);

	/// <summary>
	///     Adds the <paramref name="memberNode" /> and the <paramref name="expectations" /> on the member, which receive
	///     the <paramref name="state" />, after the constraint from the <paramref name="sourceConstraint" /> on the value,
	///     if any.
	/// </summary>
	private ExpectationBuilder AddMemberExpectations<TSource, TState>(
		MappingNode memberNode,
		bool replaceIt,
		TState state,
		Action<TState, ExpectationBuilder> expectations,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars,
		Func<string, ExpectationGrammars, IValueConstraint<TSource>>? sourceConstraint)
	{
		if (sourceConstraint is not null)
		{
			IValueConstraint<TSource> constraint = sourceConstraint.Invoke(_it, ExpectationGrammars);
			CurrentNode.AddConstraint(constraint, _it);
		}

		Node root = CurrentNode;
		memberNode.Source = (_it, ExpectationGrammars);
		Node mappingNode = CurrentNode.AddMapping(memberNode);
		// The member node collects the expectations on the member itself, so that most members need no further node.
		CurrentNode = ReferenceEquals(mappingNode, memberNode) ? memberNode : new ExpectationNode();
		string previousIt = _it;
		if (replaceIt)
		{
			_it = memberNode.MemberAccessor.GetTrimmedName();
			memberNode.ContextMember = _it;
		}

		Node? outerPendingWhichNode = PendingWhichNode;
		Node? outerPendingWhichRoot = PendingWhichRoot;
		PendingWhichNode = null;
		PendingWhichRoot = null;
		object? outerSubjectContexts = SubjectContexts;
		SubjectContexts = null;

		ExpectationGrammars previousGrammars = ExpectationGrammars;
		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammars?.Invoke(memberGrammars) ?? memberGrammars;
		int outerReasonCount = ReasonList?.Count ?? 0;
		expectations.Invoke(state, this);
		ExpectationGrammars = previousGrammars;

		CompleteWhichNode();
		PendingWhichNode = outerPendingWhichNode;
		PendingWhichRoot = outerPendingWhichRoot;
		ThrowIfEmpty(CurrentNode, nameof(expectations));
		AttachMemberExpectations(mappingNode, memberNode);
		SubjectContexts = outerSubjectContexts;
		MoveReasonsTo(mappingNode, outerReasonCount);
		CurrentNode = root;
		_it = previousIt;

		return this;
	}

	/// <summary>
	///     Attaches the expectations on the member, which were collected in the current node, to the
	///     <paramref name="mappingNode" />, unless the <paramref name="memberNode" /> collected them itself and needs no
	///     node for the contexts of the member.
	/// </summary>
	private void AttachMemberExpectations(Node mappingNode, MappingNode memberNode)
	{
		if (ReferenceEquals(CurrentNode, memberNode) && CurrentSubjectContexts is null)
		{
			return;
		}

		MoveMemberExpectationsToNewNode();
		// The mapping node adds the contexts of the member to its result, unless the expectations went to another node.
		mappingNode.AddNode(WithSubjectContexts(CurrentNode, isOfMember: ReferenceEquals(mappingNode, memberNode)));
	}

	/// <summary>
	///     Moves the expectations on a member that its <see cref="MappingNode" /> collected itself to a node of their
	///     own, before the current node is combined or wrapped.
	/// </summary>
	/// <remarks>
	///     The current node is only a <see cref="MappingNode" /> while it collects the expectations on its member.
	/// </remarks>
	private void MoveMemberExpectationsToNewNode()
	{
		if (CurrentNode is MappingNode memberNode)
		{
			CurrentNode = memberNode.MoveExpectationsToNewNode();
		}
	}

	/// <summary>
	///     Moves the reasons that were added after the first <paramref name="outerReasonCount" /> ones, i.e. for the
	///     expectations on a member, to the <paramref name="mappingNode" />, so that they follow the expectation on the
	///     member instead of the whole expectation.
	/// </summary>
	private void MoveReasonsTo(Node mappingNode, int outerReasonCount)
	{
		if (ReasonList is not { } reasons || reasons.Count == outerReasonCount ||
		    mappingNode is not ExpectationNode expectationNode)
		{
			return;
		}

		int count = reasons.Count - outerReasonCount;
		expectationNode.AddReasons(reasons.GetRange(outerReasonCount, count));
		reasons.RemoveRange(outerReasonCount, count);
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
	public ExpectationBuilder WithCancellation(CancellationToken cancellationToken)
	{
		ThrowHelper.ThrowIfAlreadySpecified(CancellationToken is not null, nameof(WithCancellation));
		GetRareSettings().CancellationToken = cancellationToken;
		return this;
	}

	/// <summary>
	///     Adds a <paramref name="timeout" /> to be used by the constraints.
	/// </summary>
	/// <remarks>
	///     The tighter timeout wins, so a <paramref name="timeout" /> that is longer than one added before does not
	///     loosen it. <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public ExpectationBuilder WithTimeout(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		GetRareSettings().Timeout = TimerHelpers.Tighter(Timeout, timeout) ?? timeout;
		return this;
	}

	/// <summary>
	///     Formats the values of this expectation with the <paramref name="formatter" />, before the formatters
	///     registered with <see cref="ValueFormatter.Register(IValueFormatter)" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="formatter" /> applies while the expectation is evaluated and while its failure message is
	///     created, never to other expectations or to formatting outside of them. Use it to format a type that the
	///     extension does not own, without changing how it is formatted elsewhere.
	///     <para />
	///     The failure message of an expectation that this one is nested in, e.g. with <c>ComplyWith</c>, and of a
	///     combination like <c>Expect.ThatAll</c> uses it as a whole.
	///     <para />
	///     When several formatters can format a value, the one added last is used. Adding the same formatter again has no
	///     effect, so pass a cached instance.
	/// </remarks>
	public ExpectationBuilder AddFormatter(IValueFormatter formatter)
	{
		formatter.ThrowIfNull();
		RareSettings rareSettings = GetRareSettings();
		if (rareSettings.Formatters is not { } formatters)
		{
			rareSettings.Formatters = [formatter,];
		}
		else if (Array.IndexOf(formatters, formatter) < 0)
		{
			rareSettings.Formatters = [..formatters, formatter,];
		}

		return this;
	}

	/// <summary>
	///     The formatters added with <see cref="AddFormatter" />, or <see langword="null" /> when none was added.
	/// </summary>
	private protected IValueFormatter[]? Formatters => _rareSettings?.Formatters;

	/// <summary>
	///     Adds the formatters of this expectation and of the expectations nested in its current evaluation to the
	///     <paramref name="formatters" />.
	/// </summary>
	internal void AddFormattersTo(List<IValueFormatter> formatters)
	{
		foreach (IValueFormatter formatter in Formatters ?? [])
		{
			if (!formatters.Contains(formatter))
			{
				formatters.Add(formatter);
			}
		}

		_evaluationContext?.CollectFormatters(formatters);
	}

	/// <summary>
	///     Formats the values with the formatters of this expectation and of the expectations nested in its current
	///     evaluation, see <see cref="AddFormattersTo" />, until the returned scope is disposed.
	/// </summary>
	private ValueFormatter.FormatterScope UseAllFormatters()
	{
		List<IValueFormatter> formatters = [];
		AddFormattersTo(formatters);
		return ValueFormatter.UseScopedFormatters(formatters.Count > 0 ? formatters.ToArray() : null);
	}

	/// <summary>
	///     Writes the met <paramref name="result" /> to the trace.
	/// </summary>
	internal void TraceSuccess(ConstraintResult result)
	{
		using ValueFormatter.FormatterScope _ = UseAllFormatters();
		Tracing.WriteSuccess(Subject, result);
	}

	private RareSettings GetRareSettings() => _rareSettings ??= new RareSettings();

#if NET8_0_OR_GREATER
	/// <summary>
	///     Lets the subject measure its allocations according to the <paramref name="options" />.
	/// </summary>
	/// <returns>
	///     <see langword="false" />, when the subject is no synchronous delegate, whose allocations can be measured.
	/// </returns>
	/// <exception cref="InvalidOperationException">The allocations are already measured.</exception>
	internal virtual bool TryMeasureAllocations(Options.AllocationOptions options) => false;
#endif

	/// <summary>
	///     Adds a <paramref name="reason" /> to the expectation.
	/// </summary>
	internal void AddReason(string reason)
	{
		if (_reasons is null)
		{
			_reasons = reason;
			return;
		}

		ReasonList!.Add(new BecauseReason(reason));
	}

	/// <summary>
	///     Adds a <paramref name="reason" /> to the expectation.
	/// </summary>
	internal void AddReason(Task<string?> reason)
	{
		List<IBecauseReason>? reasons = ReasonList;
		if (reasons is null)
		{
			reasons = [];
			_reasons = reasons;
		}

		reasons.Add(new AsyncBecauseReason(reason));
	}

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
			foreach (IBecauseReason reason in ReasonList!)
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
		foreach (AsyncBecauseReason reason in ReasonList?.OfType<AsyncBecauseReason>() ?? [])
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

		foreach (IBecauseReason reason in ReasonList!)
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
		MoveMemberExpectationsToNewNode();
		if (CurrentNode is AndNode andNode)
		{
			andNode.AddNode(operand, textSeparator);
		}
		else if (CurrentNode is OrNode { Current: AndNode currentAndNode, })
		{
			// A single node evaluates the whole `And` chain, as only it skips the operands after a failed guard.
			currentAndNode.AddNode(operand, textSeparator);
		}
		else if (CurrentNode is OrNode orNode)
		{
			AndNode newNode = new(orNode.Current);
			newNode.AddNode(operand, textSeparator);
			orNode.Current = newNode;
		}
		else
		{
			AndNode newNode = new(CurrentNode);
			newNode.AddNode(operand, textSeparator);
			CurrentNode = newNode;
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
		AddWhichNode((memberAccessor, separator, negateMemberOnly, subjectName, contextMember),
			static (state, parentNode) => new WhichNode<TSource, TTarget>(parentNode, state.memberAccessor,
				state.separator, state.negateMemberOnly, state.subjectName, state.contextMember));
		return ContinueWithMember(subjectName, expectationGrammars);
	}

	/// <summary>
	///     Specifies a mapping to add expectations on the member from the <paramref name="asyncMemberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member continues only the expectation in front of it, also after <c>And</c>/<c>Or</c>, and is only
	///     accessed when that expectation is met.
	///     <para />
	///     Set <paramref name="negateMemberOnly" /> when the previous expectation only navigates to the member, so that
	///     a negation applies to the member expectation instead of the whole expectation.
	///     <para />
	///     The expectations on the member keep the current name for the subject and the
	///     <see cref="ExpectationGrammars.Plural" /> form, so for a single value, e.g. inside <c>Whose</c>, pass
	///     <c>"it"</c> as <paramref name="subjectName" /> and remove <see cref="ExpectationGrammars.Plural" /> with the
	///     <paramref name="expectationGrammars" />.
	///     <para />
	///     If accessing or awaiting the member throws, the expectations on the member fail with <c>… did throw …</c> and
	///     the exception as <see cref="ConstraintResult.FailureCause" />, which a negation does not invert. An
	///     <see cref="OperationCanceledException" /> thrown while the evaluation is canceled aborts the evaluation
	///     instead.
	///     <para />
	///     Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a timeout
	///     fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	///     <para />
	///     The contexts of the member are labelled with the <paramref name="contextMember" />, or with the
	///     <paramref name="subjectName" />, unless the member is called <c>it</c>.
	/// </remarks>
	/// <param name="asyncMemberAccessor">Accesses the member on the source value asynchronously.</param>
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
	[OverloadResolutionPriority(1)]
	public ExpectationBuilder ForAsyncWhich<TSource, TTarget>(
		Func<TSource, Task<TTarget?>> asyncMemberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false,
		string? contextMember = null)
	{
		contextMember ??= subjectName == DefaultCurrentSubject ? null : subjectName;
		AddWhichNode((asyncMemberAccessor, separator, negateMemberOnly, subjectName, contextMember),
			static (state, parentNode) => new WhichNode<TSource, TTarget>(parentNode, state.asyncMemberAccessor,
				state.separator, state.negateMemberOnly, state.subjectName, state.contextMember));
		return ContinueWithMember(subjectName, expectationGrammars);
	}

	/// <inheritdoc cref="ForAsyncWhich{TSource, TTarget}(Func{TSource, Task{TTarget}}, string?, string?, Func{ExpectationGrammars, ExpectationGrammars}?, bool, string?)" />
	public ExpectationBuilder ForAsyncWhich<TSource, TTarget>(
		Func<TSource, ValueTask<TTarget?>> asyncMemberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false,
		string? contextMember = null)
	{
		contextMember ??= subjectName == DefaultCurrentSubject ? null : subjectName;
		AddWhichNode((asyncMemberAccessor, separator, negateMemberOnly, subjectName, contextMember),
			static (state, parentNode) => new WhichNode<TSource, TTarget>(parentNode, state.asyncMemberAccessor,
				state.separator, state.negateMemberOnly, state.subjectName, state.contextMember));
		return ContinueWithMember(subjectName, expectationGrammars);
	}

	/// <inheritdoc cref="ForAsyncWhich{TSource, TTarget}(Func{TSource, Task{TTarget}}, string?, string?, Func{ExpectationGrammars, ExpectationGrammars}?, bool, string?)" />
	/// <summary>
	///     Specifies a mapping to add expectations on the member from the <paramref name="asyncMemberAccessor" />,
	///     which receives the <see cref="IEvaluationContext" /> and the <see cref="CancellationToken" /> of the
	///     evaluation. Pass the <see cref="CancellationToken" /> on to its asynchronous calls, so that a canceled
	///     evaluation or a timeout also stops them.
	/// </summary>
	[OverloadResolutionPriority(1)]
	public ExpectationBuilder ForAsyncWhich<TSource, TTarget>(
		Func<TSource, IEvaluationContext, CancellationToken, Task<TTarget?>> asyncMemberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false,
		string? contextMember = null)
	{
		contextMember ??= subjectName == DefaultCurrentSubject ? null : subjectName;
		AddWhichNode((asyncMemberAccessor, separator, negateMemberOnly, subjectName, contextMember),
			static (state, parentNode) => new WhichNode<TSource, TTarget>(parentNode, state.asyncMemberAccessor,
				state.separator, state.negateMemberOnly, state.subjectName, state.contextMember));
		return ContinueWithMember(subjectName, expectationGrammars);
	}

	/// <inheritdoc
	///     cref="ForAsyncWhich{TSource, TTarget}(Func{TSource, IEvaluationContext, CancellationToken, Task{TTarget}}, string?, string?, Func{ExpectationGrammars, ExpectationGrammars}?, bool, string?)" />
	public ExpectationBuilder ForAsyncWhich<TSource, TTarget>(
		Func<TSource, IEvaluationContext, CancellationToken, ValueTask<TTarget?>> asyncMemberAccessor,
		string? separator = null,
		string? subjectName = null,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null,
		bool negateMemberOnly = false,
		string? contextMember = null)
	{
		contextMember ??= subjectName == DefaultCurrentSubject ? null : subjectName;
		AddWhichNode((asyncMemberAccessor, separator, negateMemberOnly, subjectName, contextMember),
			static (state, parentNode) => new WhichNode<TSource, TTarget>(parentNode, state.asyncMemberAccessor,
				state.separator, state.negateMemberOnly, state.subjectName, state.contextMember));
		return ContinueWithMember(subjectName, expectationGrammars);
	}

	/// <summary>
	///     Lets the following expectations refer to the member of a which node as <paramref name="subjectName" />, if
	///     set, with the <paramref name="expectationGrammars" />.
	/// </summary>
	private ExpectationBuilder ContinueWithMember(string? subjectName,
		Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars)
	{
		if (subjectName != null)
		{
			_it = subjectName;
		}

		ExpectationGrammars memberGrammars = ExpectationGrammars & ~ExpectationGrammars.Introduced;
		ExpectationGrammars = expectationGrammars?.Invoke(memberGrammars) ?? memberGrammars;
		return this;
	}

	/// <summary>
	///     Replaces the right-most operand with the which node from <paramref name="createWhichNode" />, which continues
	///     it, and collects the following expectations for the member until the which node is completed.
	/// </summary>
	/// <remarks>
	///     The <paramref name="state" /> is passed to <paramref name="createWhichNode" />, so that it needs no closure.
	/// </remarks>
	private void AddWhichNode<TState>(TState state, Func<TState, Node?, Node> createWhichNode)
	{
		MoveMemberExpectationsToNewNode();
		CompleteWhichNode();
		if (SubjectContexts is not null)
		{
			SubjectContexts = new SubjectContextScopes
			{
				OfPendingWhichRoot = (SubjectContext)SubjectContexts,
			};
		}

		PendingWhichRoot = CurrentNode.ReplaceRightMostOperand((Builder: this, State: state, Create: createWhichNode),
			static (s, operand) =>
			{
				Node whichNode = s.Create(s.State, operand is ExpectationNode e && e.IsEmpty() ? null : operand);
				s.Builder.PendingWhichNode = whichNode;
				return whichNode;
			});
		CurrentNode = new ExpectationNode();
	}

	/// <summary>
	///     Lists the <paramref name="otherExceptions" /> of the faulted subject as context of a failure, unless they are
	///     <see langword="null" />.
	/// </summary>
	internal void AddOtherExceptions(Exception[]? otherExceptions)
	{
		if (otherExceptions is not null)
		{
			GetRareSettings().OtherExceptions = otherExceptions;
		}
	}

	/// <summary>
	///     Forgets the other exceptions of a previous attempt to meet the expectations.
	/// </summary>
	internal void ResetOtherExceptions()
	{
		if (_rareSettings is not null)
		{
			_rareSettings.OtherExceptions = null;
		}
	}

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" />.
	/// </summary>
	/// <remarks>
	///     A context is awaited until the timeout of the evaluation elapses or the evaluation is canceled.
	/// </remarks>
	internal async Task<string> FromFailure(ConstraintResult failure)
	{
		EvaluationCancellation cancellation = GetRemainingCancellation();
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		using ValueFormatter.FormatterScope __ = UseAllFormatters();
		return await FromFailure(Subject, failure, cancellation.Token);
	}

	/// <summary>
	///     Returns the cancellation for what is still awaited after the current evaluation, see
	///     <see cref="EvaluationCancellation.ForRemainingTimeout" />. It must be released.
	/// </summary>
	internal EvaluationCancellation GetRemainingCancellation()
		=> (_evaluationContext?.Cancellation ?? EvaluationCancellation.None).ForRemainingTimeout();

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

	/// <param name="wrapsMetResult">
	///     Whether a met result still shows the contexts of the subject, which only matters when it can be negated.
	/// </param>
	internal Node GetRootNode(bool wrapsMetResult = true)
	{
		CompleteWhichNode();
		return WithSubjectContexts(CurrentNode, wrapsMetResult);
	}

	/// <summary>
	///     Wraps the <paramref name="node" /> with the expectations on the current subject, so that its failure shows the
	///     contexts of the subject, if any.
	/// </summary>
	private Node WithSubjectContexts(Node node, bool wrapsMetResult = true, bool isOfMember = false)
		=> CurrentSubjectContexts is { } subjectContexts
			? new SubjectContextsNode(node, subjectContexts, wrapsMetResult, isOfMember)
			: node;

	/// <summary>
	///     Attaches the current node to a pending <see cref="WhichNode{TSource,TMember}" />, so that it is evaluated on
	///     the projected member.
	/// </summary>
	private void CompleteWhichNode()
	{
		if (PendingWhichNode is not null)
		{
			PendingWhichNode.AddNode(WithSubjectContexts(CurrentNode));
			CurrentNode = PendingWhichRoot!;
			PendingWhichNode = null;
			PendingWhichRoot = null;
			// The subject contexts that were added for the member belong to the which node.
			SubjectContexts = (SubjectContexts as SubjectContextScopes)?.OfPendingWhichRoot;
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
	/// <param name="limit">
	///     The time that the combination this expectation is a member of has left, which only tightens the timeout.
	/// </param>
	/// <param name="combinationToken">
	///     The cancellation token of the combination this expectation is a member of, which is observed together with
	///     the own one.
	/// </param>
	internal ValueTask<ConstraintResult> IsMet(bool endsWhenMet = true, TimeSpan? limit = null,
		CancellationToken? combinationToken = null)
	{
		if (_rareSettings?.Formatters is not { } formatters)
		{
			return Evaluate(endsWhenMet, limit, combinationToken);
		}

		using ValueFormatter.FormatterScope _ = ValueFormatter.UseScopedFormatters(formatters);
		return Evaluate(endsWhenMet, limit, combinationToken);
	}

	/// <summary>
	///     Evaluates the expectation, see <see cref="IsMet(bool, TimeSpan?, CancellationToken?)" />.
	/// </summary>
	/// <remarks>
	///     An evaluation that continues asynchronously keeps the formatters that were in use when it started.
	/// </remarks>
	private ValueTask<ConstraintResult> Evaluate(bool endsWhenMet, TimeSpan? limit,
		CancellationToken? combinationToken)
	{
		Task previousEvaluation = EndEvaluation();
		if (previousEvaluation.Status != TaskStatus.RanToCompletion)
		{
			return IsMetAfter(previousEvaluation, endsWhenMet, limit, combinationToken);
		}

		if (endsWhenMet && limit is null && combinationToken is null &&
		    _nodeOrConstraint is IConstraint constraint && _rareSettings is null && CanEvaluateRightAway())
		{
			EvaluationContext.EvaluationContext rightAwayContext = EvaluationContext.EvaluationContext.Rent();
			if (TryEvaluateRightAway(constraint, rightAwayContext, out ValueTask<ConstraintResult> isMetRightAway))
			{
				return CompleteRightAway(isMetRightAway, rightAwayContext);
			}

			EvaluationContext.EvaluationContext.Return(rightAwayContext);
		}

		return EvaluateRegularly(endsWhenMet, limit, combinationToken);
	}

	/// <summary>
	///     Evaluates the expectation through its nodes, see <see cref="IsMet(bool, TimeSpan?, CancellationToken?)" />.
	/// </summary>
	private ValueTask<ConstraintResult> EvaluateRegularly(bool endsWhenMet, TimeSpan? limit,
		CancellationToken? combinationToken)
	{
		ResetOtherExceptions();
		EvaluationContext.EvaluationContext context = new();
		_evaluationContext = context;
		ValueTask<ConstraintResult> isMet = StartEvaluation(context, limit, combinationToken);
		if (!isMet.IsCompletedSuccessfully)
		{
			return CompleteEvaluation(isMet, endsWhenMet);
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome != Outcome.Success || (_reasons is not null && !CanSkipReasonsOfMetResult(endsWhenMet)) ||
		    (endsWhenMet && context.HasCompletionCallbacks))
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

	/// <summary>
	///     Whether a single constraint can be evaluated right away, see <see cref="TryEvaluateRightAway" />.
	/// </summary>
	/// <remarks>
	///     Without a test cancellation and a trace writer, the regular evaluation of a single constraint on a value
	///     subject passes the result of the constraint on unchanged, and a met result needs no reasons that must be
	///     awaited.
	/// </remarks>
	private bool CanEvaluateRightAway()
	{
		(TestCancellation? testCancellation, ITraceWriter? traceWriter) = Customize.aweXpect.GetEvaluationSettings();
		if (testCancellation is not null || traceWriter is not null)
		{
			return false;
		}

		IsTracing = false;
		return _reasons is null || CanSkipReasonsOfMetResult(true);
	}

	/// <summary>
	///     Evaluates the single <paramref name="constraint" /> on the subject without a node, when the subject is a value.
	/// </summary>
	internal virtual bool TryEvaluateRightAway(IConstraint constraint, EvaluationContext.EvaluationContext context,
		out ValueTask<ConstraintResult> isMet)
	{
		isMet = default;
		return false;
	}

	/// <summary>
	///     Turns the <paramref name="exception" /> of the single constraint, which was evaluated right away, into the
	///     result, like the node with this constraint does.
	/// </summary>
	private protected virtual ValueTask<ConstraintResult> FromRightAwayException(Exception exception,
		EvaluationContext.EvaluationContext context)
		=> new(Task.FromException<ConstraintResult>(exception));

	/// <summary>
	///     Completes the evaluation of a single constraint right away, like <see cref="EvaluateRegularly" /> completes
	///     the evaluation of the node with this constraint.
	/// </summary>
	/// <remarks>
	///     A met result that kept nothing in the <paramref name="context" /> returns right away and keeps the context for
	///     the next evaluation.
	/// </remarks>
	private ValueTask<ConstraintResult> CompleteRightAway(ValueTask<ConstraintResult> isMet,
		EvaluationContext.EvaluationContext context)
	{
		if (!isMet.IsCompletedSuccessfully)
		{
			return CompleteRightAwayAsync(isMet, context);
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome == Outcome.Success && context.IsUnused)
		{
			EvaluationContext.EvaluationContext.Return(context);
			return new ValueTask<ConstraintResult>(result);
		}

		_evaluationContext = context;
		if (result.Outcome != Outcome.Success || context.HasCompletionCallbacks)
		{
			return CompleteEvaluation(new ValueTask<ConstraintResult>(result), true);
		}

		Task endEvaluation = EndEvaluation();
		return endEvaluation.Status == TaskStatus.RanToCompletion
			? new ValueTask<ConstraintResult>(result)
			: ReturnAfter(endEvaluation, result);
	}

	private async ValueTask<ConstraintResult> CompleteRightAwayAsync(ValueTask<ConstraintResult> isMet,
		EvaluationContext.EvaluationContext context)
	{
		ConstraintResult result;
		try
		{
			result = await isMet;
		}
		catch (Exception exception)
		{
			_evaluationContext = context;
			return await CompleteEvaluation(FromRightAwayException(exception, context), true);
		}

		return await CompleteRightAway(new ValueTask<ConstraintResult>(result), context);
	}

	/// <summary>
	///     Evaluates the expectation as the member of a combination, after the <paramref name="index" /> of the
	///     previous member, and traces it when it is met.
	/// </summary>
	/// <remarks>
	///     The <paramref name="limit" /> and the <paramref name="cancellationToken" /> of the combination only apply to
	///     this evaluation, so that awaiting the expectation again starts with its own settings.
	/// </remarks>
	internal ValueTask<Expectation.Result> GetResult(int index, TimeSpan? limit,
		CancellationToken? cancellationToken)
	{
		ValueTask<ConstraintResult> isMet = IsMet(false, limit, cancellationToken);
		if (!isMet.IsCompletedSuccessfully || IsTracing)
		{
			return GetResultAsync(isMet, index);
		}

		return new ValueTask<Expectation.Result>(new Expectation.Result(index + 1, Subject, isMet.Result, true));
	}

	private async ValueTask<Expectation.Result> GetResultAsync(ValueTask<ConstraintResult> isMet, int index)
	{
		ConstraintResult result = await isMet;
		if (result.Outcome == Outcome.Success && IsTracing)
		{
			TraceSuccess(result);
		}

		return new Expectation.Result(index + 1, Subject, result, true);
	}

	/// <summary>
	///     Whether the reasons need not be appended to a met result, as nothing reads its expectation text.
	/// </summary>
	/// <remarks>
	///     A combination can still fail a met member and a trace writes the expectation of a met one, and a reason that
	///     must be awaited is awaited for a met expectation as well.
	/// </remarks>
	private bool CanSkipReasonsOfMetResult(bool endsWhenMet)
	{
		if (!endsWhenMet || IsTracing)
		{
			return false;
		}

		if (_reasons is string)
		{
			return true;
		}

		List<IBecauseReason> reasons = ReasonList!;
		for (int i = 0; i < reasons.Count; i++)
		{
			if (reasons[i] is not BecauseReason)
			{
				return false;
			}
		}

		return true;
	}

	private async ValueTask<ConstraintResult> IsMetAfter(Task previousEvaluation, bool endsWhenMet,
		TimeSpan? limit, CancellationToken? combinationToken)
	{
		await previousEvaluation;
		return await Evaluate(endsWhenMet, limit, combinationToken);
	}

	private static async ValueTask<ConstraintResult> ReturnAfter(Task endEvaluation, ConstraintResult result)
	{
		await endEvaluation;
		return result;
	}

	private ValueTask<ConstraintResult> StartEvaluation(EvaluationContext.EvaluationContext context,
		TimeSpan? limit, CancellationToken? combinationToken)
	{
		try
		{
			TimeProvider timeProvider = _rareSettings?.TimeProvider ?? TimeProvider.System;
			context.TimeProvider = timeProvider;
			(TestCancellation? testCancellation, ITraceWriter? traceWriter) =
				Customize.aweXpect.GetEvaluationSettings();
			IsTracing = traceWriter is not null;
			CancellationToken cancellationToken = CombineCancellation(CancellationToken, combinationToken, context) ??
			                                      testCancellation?.CancellationTokenFactory?.Invoke() ??
			                                      System.Threading.CancellationToken.None;
			TimeSpan? timeout = TimerHelpers.Tighter(TimerHelpers.Tighter(TimerHelpers.Tighter(
				Timeout, _rareSettings?.TimeoutBound?.UpperBound), testCancellation?.Timeout), limit);
			// Nothing negates the result of the whole expectation, and a combination only reads the contexts of a
			// failed member.
			Node rootNode = GetRootNode(false);
			if (IsTrueWithoutExpectations && CurrentNode is ExpectationNode expectationNode && expectationNode.IsEmpty())
			{
				rootNode.AddConstraint(new ThatBoolSubject.IsTrueConstraint(ExpectationGrammars));
			}

			return IsMet(rootNode, context, timeProvider,
				timeout == System.Threading.Timeout.InfiniteTimeSpan ? null : timeout,
				cancellationToken);
		}
		catch (Exception exception)
		{
			return new ValueTask<ConstraintResult>(Task.FromException<ConstraintResult>(exception));
		}
	}

	/// <summary>
	///     Combines the <paramref name="own" /> cancellation token with the one of the <paramref name="combination" />
	///     this expectation is a member of, and releases a linked source together with the evaluation in the
	///     <paramref name="context" />.
	/// </summary>
	private static CancellationToken? CombineCancellation(CancellationToken? own, CancellationToken? combination,
		EvaluationContext.EvaluationContext context)
	{
		CancellationToken? token =
			Expectation.Combination.CombineCancellation(own, combination, out CancellationTokenSource? linked);
		if (linked is not null)
		{
			context.ReleaseWithEvaluation(linked.Dispose);
		}

		return token;
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

			if (endsWhenMet && _evaluationContext is not null)
			{
				await _evaluationContext.RunCompletionCallbacks(result.Outcome);
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

		return _rareSettings?.OtherExceptions is { } otherExceptions
			? new ConstraintResult.WithOtherExceptions(result, otherExceptions)
			: result;
	}

	/// <summary>
	///     Whether a callback was registered with <see cref="IEvaluationContext.OnCompleted" /> during the current
	///     evaluation.
	/// </summary>
	internal bool HasCompletionCallbacks => _evaluationContext?.HasCompletionCallbacks == true;

	/// <summary>
	///     Runs the callbacks registered with <see cref="IEvaluationContext.OnCompleted" /> during the current evaluation
	///     that are not among the <paramref name="ran" /> ones, for the combination that this expectation is a member of.
	/// </summary>
	internal Task RunCompletionCallbacks(Outcome outcome,
		List<Func<CompletedEvaluation, CancellationToken, ValueTask>> ran)
		=> _evaluationContext?.RunCompletionCallbacks(outcome, ran) ?? Task.CompletedTask;

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
		TimeProvider timeProvider,
		TimeSpan? timeout,
		CancellationToken cancellationToken);

	/// <summary>
	///     Supports chaining for alternative expectation constraints with the <paramref name="textSeparator" />.
	/// </summary>
	public ExpectationBuilder Or(string textSeparator = " or ")
	{
		MoveMemberExpectationsToNewNode();
		if (CurrentNode is OrNode orNode)
		{
			orNode.AddNode(new ExpectationNode(), textSeparator);
			return this;
		}

		OrNode newNode = new(CurrentNode);
		newNode.AddNode(new ExpectationNode(), textSeparator);
		CurrentNode = newNode;
		return this;
	}

	/// <summary>
	///     Limits the evaluation by the upper bound of the <paramref name="bound" /> options, like a timeout added with
	///     <see cref="WithTimeout(TimeSpan)" />.
	/// </summary>
	/// <remarks>
	///     The upper bound is read when the evaluation starts, so that a later option replaces the bound of an earlier
	///     one, while the tighter timeout still wins. Adding the same options again has no effect.
	/// </remarks>
	internal void AddTimeoutBound(ITimeoutBound bound)
	{
		RareSettings settings = GetRareSettings();
		if (settings.TimeoutBound is null)
		{
			settings.TimeoutBound = bound;
		}
		else if (!ReferenceEquals(settings.TimeoutBound, bound))
		{
			settings.TimeoutBound = new TighterTimeoutBound(settings.TimeoutBound, bound);
		}
	}

	/// <summary>
	///     Specifies a <see cref="TimeProvider" /> to use for the expectation.
	/// </summary>
	/// <exception cref="InvalidOperationException">A time provider is already set.</exception>
	internal void UseTimeProvider(TimeProvider timeProvider)
	{
		ThrowHelper.ThrowIfAlreadySpecified(_rareSettings?.TimeProvider is not null,
			"WithTimeProvider");
		GetRareSettings().TimeProvider = timeProvider;
	}

	/// <summary>
	///     The settings of the expectation that only few expectations use.
	/// </summary>
	private sealed class RareSettings
	{
		public CancellationToken? CancellationToken { get; set; }

		/// <summary>
		///     The other exceptions of the faulted subject in the current evaluation.
		/// </summary>
		public Exception[]? OtherExceptions { get; set; }

		/// <remarks>
		///     The ticks of the timeout, or <see cref="long.MinValue" /> for none, instead of a nullable
		///     <see cref="TimeSpan" />, so that the <see cref="TimeoutBound" /> fits into the same size.<br />
		///     <see cref="long.MinValue" /> are the ticks of <see cref="TimeSpan.MinValue" />, which cannot be confused with
		///     a timeout: <see cref="WithTimeout(TimeSpan)" /> rejects negative timeouts, and the only negative one it
		///     accepts, <see cref="System.Threading.Timeout.InfiniteTimeSpan" />, has other ticks.
		/// </remarks>
		private long _timeoutTicks = long.MinValue;

		public IValueFormatter[]? Formatters { get; set; }
		public Node? PendingWhichNode { get; set; }
		public Node? PendingWhichRoot { get; set; }
		public object? SubjectContexts { get; set; }

		public TimeSpan? Timeout
		{
			get => _timeoutTicks == long.MinValue ? null : new TimeSpan(_timeoutTicks);
			set => _timeoutTicks = value?.Ticks ?? long.MinValue;
		}

		public ITimeoutBound? TimeoutBound { get; set; }
		public TimeProvider? TimeProvider { get; set; }
	}

	/// <summary>
	///     Limits the evaluation by the tighter upper bound of two options.
	/// </summary>
	private sealed class TighterTimeoutBound(ITimeoutBound first, ITimeoutBound second) : ITimeoutBound
	{
		public TimeSpan? UpperBound => TimerHelpers.Tighter(first.UpperBound, second.UpperBound);
	}

	/// <summary>
	///     The subject contexts of the current subject and of the subject that the pending which node continues.
	/// </summary>
	/// <remarks>
	///     Only allocated while a which node is pending on a subject with subject contexts, see
	///     <see cref="SubjectContexts" />.
	/// </remarks>
	private sealed class SubjectContextScopes
	{
		public SubjectContext? Current { get; set; }
		public SubjectContext? OfPendingWhichRoot { get; set; }
	}

	/// <summary>
	///     Helper class to specify constraints on the selected <typeparamref name="TMember" />.
	/// </summary>
	public class MemberExpectationBuilder<TSource, TMember>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Action<MemberAccessor, StringBuilder>? _expectationTextGenerator;

		/// <summary>
		///     The accessor of the member, which is a <see cref="MemberAccessor{TSource,TTarget}" /> to
		///     <typeparamref name="TMember" />, or to a <see cref="Task{TResult}" /> or a <see cref="ValueTask{TResult}" />
		///     of <typeparamref name="TMember" />.
		/// </summary>
		/// <remarks>
		///     One field for all three kinds keeps every expectation on a member smaller. The kinds cannot be confused,
		///     because a <typeparamref name="TMember" /> is never its own <see cref="Task{TResult}" />.
		/// </remarks>
		private readonly MemberAccessor _memberAccessor;

		private readonly bool _replaceIt;
		private Action<MemberAccessor, StringBuilder>? _negatedAlternativeTextGenerator;

		private Func<string, ExpectationGrammars, IValueConstraint<TSource>>? _sourceConstraintBuilder;

		/// <remarks>
		///     The builder holds the <paramref name="memberAccessor" /> itself instead of a closure over it, so that adding
		///     expectations on a member allocates no delegates.
		/// </remarks>
		internal MemberExpectationBuilder(ExpectationBuilder expectationBuilder,
			MemberAccessor memberAccessor,
			Action<MemberAccessor, StringBuilder>? expectationTextGenerator,
			bool replaceIt)
		{
			_expectationBuilder = expectationBuilder;
			_memberAccessor = memberAccessor;
			_expectationTextGenerator = expectationTextGenerator;
			_replaceIt = replaceIt;
		}

		/// <summary>
		///     Add expectations for the current <typeparamref name="TMember" />.
		/// </summary>
		public ExpectationBuilder AddExpectations(
			Action<ExpectationBuilder> expectation,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			=> _expectationBuilder.AddMemberExpectations(CreateMappingNode<TMember>(), _replaceIt, expectation,
				static (expectation, expectationBuilder) => expectation(expectationBuilder),
				expectationGrammars, _sourceConstraintBuilder);

		/// <summary>
		///     Add the <paramref name="expectations" /> for the current <typeparamref name="TMember" />, which receive it
		///     as <see cref="IThatSubject{T}" />.
		/// </summary>
		/// <remarks>
		///     Pass the nested expectations of the caller directly, instead of wrapping the
		///     <see cref="ExpectationBuilder" /> in a <see cref="ThatSubject{T}" />.
		/// </remarks>
		public ExpectationBuilder AddExpectations(
			Action<IThatSubject<TMember>> expectations,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			=> _expectationBuilder.AddMemberExpectations(CreateMappingNode<TMember>(), _replaceIt, expectations,
				static (expectations, expectationBuilder) => expectations(new ThatSubject<TMember>(expectationBuilder)),
				expectationGrammars, _sourceConstraintBuilder);

		/// <summary>
		///     Add expectations for the current <typeparamref name="TMember" />, which receive the
		///     <paramref name="state" />.
		/// </summary>
		/// <remarks>
		///     Pass what the <paramref name="expectation" /> needs, e.g. the expectations of the caller, as
		///     <paramref name="state" /> instead of capturing it, so that a <see langword="static" /> lambda can be used,
		///     which allocates neither a closure nor a delegate.
		/// </remarks>
		public ExpectationBuilder AddExpectations<TState>(
			TState state,
			Action<TState, ExpectationBuilder> expectation,
			Func<ExpectationGrammars, ExpectationGrammars>? expectationGrammars = null)
			=> _expectationBuilder.AddMemberExpectations(CreateMappingNode<TMember>(), _replaceIt, state, expectation,
				expectationGrammars, _sourceConstraintBuilder);

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
			=> _expectationBuilder.AddMemberExpectations(CreateMappingNode<TNarrowed>(), _replaceIt, expectation,
				static (expectation, expectationBuilder) => expectation(expectationBuilder),
				expectationGrammars, _sourceConstraintBuilder);

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

		/// <summary>
		///     Under negation, renders the expectations on the member as an alternative to the negated expectation on
		///     the subject, introduced by the <paramref name="negatedExpectationTextGenerator" />, e.g.
		///     <c>does not have property "a" or $.a is not equal to 1</c>.
		/// </summary>
		/// <remarks>
		///     Use it when the expectation on the subject finds the member, e.g. a property that may be missing, so that
		///     its negation is met when the member is missing or does not satisfy the expectations on it. A failed
		///     negation is then explained by the result of the expectations on the member, unless the expectation on the
		///     subject fails both ways.
		/// </remarks>
		public MemberExpectationBuilder<TSource, TMember> NegatedAsAlternative(
			Action<MemberAccessor, StringBuilder> negatedExpectationTextGenerator)
		{
			_negatedAlternativeTextGenerator = negatedExpectationTextGenerator;
			return this;
		}

		/// <summary>
		///     Creates the mapping node whose expectations are typed at <typeparamref name="TNarrowed" />.
		/// </summary>
		private MappingNode<TSource, TMember, TNarrowed> CreateMappingNode<TNarrowed>()
		{
			MappingNode<TSource, TMember, TNarrowed> mappingNode;
			if (_memberAccessor is MemberAccessor<TSource, TMember> memberAccessor)
			{
				mappingNode = new MappingNode<TSource, TMember, TNarrowed>(memberAccessor, _expectationTextGenerator);
			}
			else if (_memberAccessor is MemberAccessor<TSource, Task<TMember>> taskMemberAccessor)
			{
				mappingNode = new MappingNode<TSource, TMember, TNarrowed>(taskMemberAccessor, _expectationTextGenerator);
			}
			else
			{
				mappingNode = new MappingNode<TSource, TMember, TNarrowed>(
					(MemberAccessor<TSource, ValueTask<TMember>>)_memberAccessor, _expectationTextGenerator);
			}

			mappingNode.NegatedAlternativeTextGenerator = _negatedAlternativeTextGenerator;
			return mappingNode;
		}
	}
}

/// <remarks>
///     The builder is also the subject of the expectations, so that specifying them allocates no further object.
/// </remarks>
internal class ExpectationBuilder<TValue> : ExpectationBuilder, IExpectThat<TValue>, IThatSubject<TValue>
{
	/// <summary>
	///     The source of the subject, or <see langword="null" /> for a subject that is the <see cref="_value" /> itself.
	/// </summary>
	private readonly IValueSource<TValue>? _subjectSource;

	/// <summary>
	///     The subject, when it is no <see cref="_subjectSource" />.
	/// </summary>
	private readonly TValue _value = default!;

	internal ExpectationBuilder(
		IValueSource<TValue> subjectSource,
		string subjectExpression)
		: base(subjectExpression)
	{
		_subjectSource = subjectSource;
	}

	internal ExpectationBuilder(
		TValue value,
		string subjectExpression)
		: base(subjectExpression)
	{
		_value = value;
	}

	/// <inheritdoc />
	ExpectationBuilder IExpectThat<TValue>.ExpectationBuilder => this;

	/// <inheritdoc />
	AndOrWhoseResult<TType, IThatSubject<TValue>> IThatSubject<TValue>.Is<TType>()
		=> new ThatSubject<TValue>(this).Is<TType>();

	/// <inheritdoc />
	AndOrResult<TValue, IThatSubject<TValue>> IThatSubject<TValue>.IsNot<TType>()
		=> new ThatSubject<TValue>(this).IsNot<TType>();

	/// <inheritdoc />
	AndOrWhoseResult<TType, IThatSubject<TValue>> IThatSubject<TValue>.IsExactly<TType>()
		=> new ThatSubject<TValue>(this).IsExactly<TType>();

	/// <inheritdoc />
	AndOrResult<TValue, IThatSubject<TValue>> IThatSubject<TValue>.IsNotExactly<TType>()
		=> new ThatSubject<TValue>(this).IsNotExactly<TType>();

#if NET8_0_OR_GREATER
	/// <inheritdoc />
	internal override bool TryMeasureAllocations(Options.AllocationOptions options)
	{
		if (_subjectSource is not IAllocationMeasuringSource source)
		{
			return false;
		}

		source.MeasureAllocations(options);
		return true;
	}
#endif

	/// <inheritdoc />
	internal override bool TryEvaluateRightAway(IConstraint constraint,
		EvaluationContext.EvaluationContext context, out ValueTask<ConstraintResult> isMet)
	{
		isMet = default;
		if (_subjectSource is not null || _value is DelegateValue)
		{
			return false;
		}

		try
		{
			switch (constraint)
			{
				case IValueConstraint<TValue> valueConstraint:
					isMet = new ValueTask<ConstraintResult>(valueConstraint.IsMetBy(_value));
					return true;
				case IContextConstraint<TValue> contextConstraint:
					isMet = new ValueTask<ConstraintResult>(contextConstraint.IsMetBy(_value, context));
					return true;
				case IAsyncConstraint<TValue> asyncConstraint:
					isMet = asyncConstraint.IsMetBy(_value, System.Threading.CancellationToken.None);
					return true;
				case IAsyncContextConstraint<TValue> asyncContextConstraint:
					isMet = asyncContextConstraint.IsMetBy(_value, context, System.Threading.CancellationToken.None);
					return true;
				default:
					return false;
			}
		}
		catch (Exception exception)
		{
			isMet = FromRightAwayException(exception, context);
			return true;
		}
	}

	/// <inheritdoc />
	private protected override ValueTask<ConstraintResult> FromRightAwayException(Exception exception,
		EvaluationContext.EvaluationContext context)
		=> ((ExpectationNode)CurrentNode).FromConstraintException(exception, _value, context);

	/// <inheritdoc />
	/// <remarks>
	///     A value subject without timeout and cancellation is passed to the <paramref name="rootNode" /> without starting
	///     a state machine: the shared <see cref="EvaluationCancellation.None" /> neither times out nor is canceled, so
	///     everything after the evaluation would leave its result unchanged.
	/// </remarks>
	internal override ValueTask<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		TimeProvider timeProvider,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		if (timeout is null && !cancellationToken.CanBeCanceled && _subjectSource is null &&
		    _value is not DelegateValue && !IsTracing)
		{
			context.Cancellation = EvaluationCancellation.None;
			return rootNode.IsMetBy(_value, context, cancellationToken);
		}

		return IsMetAsync(rootNode, context, timeProvider, timeout, cancellationToken);
	}

	private async ValueTask<ConstraintResult> IsMetAsync(Node rootNode,
		EvaluationContext.EvaluationContext context,
		TimeProvider timeProvider,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		EvaluationCancellation cancellation = EvaluationCancellation.Create(timeout, cancellationToken, timeProvider);
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		context.Cancellation = cancellation;
		CancellationToken token = cancellation.Token;

		if (_subjectSource is { IsNullTaskSubject: true, })
		{
			ConstraintResult expectation = await rootNode.IsMetBy(default(TValue),
				ExpectationTextEvaluationContext.For(context), token);
			return NullSubjectResult.CreateForNullTaskSubject(expectation, default(TValue));
		}

		TValue data = _value;
		try
		{
			if (_subjectSource is not null)
			{
				data = await _subjectSource.GetValue(timeProvider, token);
			}
		}
		catch (Exception exception)
		{
			AddOtherExceptions(_subjectSource!.GetOtherExceptions(exception));
			ConstraintResult result = await FromException(rootNode, context, cancellation, exception);
			if (IsTracing)
			{
				Tracing.WriteThrowingSubject(Subject);
			}

			return result;
		}

		if (IsTracing)
		{
			Tracing.WriteSubject(Subject, data, timeout);
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
