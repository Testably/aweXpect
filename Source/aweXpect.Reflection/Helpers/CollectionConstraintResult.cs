using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     A typed <see cref="ConstraintResult" /> which is used to split a collection of <typeparamref name="T" /> in a
///     matching and not matching group.
/// </summary>
/// <remarks>
///     The collection is read through the <see cref="IEvaluationContext" />, so that it is enumerated only once, also
///     when several expectations are combined. A <see langword="null" /> collection fails the expectation and its
///     negation alike.
/// </remarks>
internal abstract class CollectionConstraintResult<T>(string it, ExpectationGrammars grammars)
	: ConstraintResult(grammars)
{
	private object? _elements;
	private Outcome _outcome = Outcome.Undecided;
	private Type? _subjectType;

	/// <summary>
	///     The name of the subject.
	/// </summary>
	protected string It { get; } = it;

	/// <summary>
	///     Flag indicating if the constraint is negated.
	/// </summary>
	protected bool IsNegated { get; private set; }

	/// <inheritdoc />
	public override Outcome Outcome
	{
		get => (_outcome, _isNegated: IsNegated) switch
		{
			(Outcome.Failure, true) => Outcome.Success,
			(Outcome.Success, true) => Outcome.Failure,
			(_, _) => _outcome,
		};
		protected set => _outcome = value;
	}

	/// <inheritdoc />
	public override string? LeadingSubject => GetSubjectOfResult(It);

	/// <inheritdoc />
	public override string? TrailingSubject => GetSubjectOfResult(It);

	/// <summary>
	///     The matching elements of the last evaluation.
	/// </summary>
	protected T[] Matching { get; private set; } = [];

	/// <summary>
	///     The not matching elements of the last evaluation.
	/// </summary>
	protected T[] NotMatching { get; private set; } = [];

	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	protected ConstraintResult SetValue(IEnumerable<T>? elements, IEvaluationContext context,
		Func<T, bool> predicate)
	{
		if (!StartEvaluation(elements, typeof(IEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T> notMatching = [];
		foreach (T item in context.UseMaterializedEnumerable(elements!))
		{
			(predicate(item) ? matching : notMatching).Add(item);
		}

		return Complete(matching, notMatching);
	}

	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected async ValueTask<ConstraintResult> SetValue(IEnumerable<T>? elements, IEvaluationContext context,
		Func<T, ValueTask<bool>> predicate, CancellationToken cancellationToken)
	{
		if (!StartEvaluation(elements, typeof(IEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T> notMatching = [];
		foreach (T item in context.UseMaterializedEnumerable(elements!))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return this;
			}

			(await predicate(item) ? matching : notMatching).Add(item);
		}

		return Complete(matching, notMatching);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected ValueTask<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T>? elements, IEvaluationContext context,
		Func<T, bool> predicate, CancellationToken cancellationToken)
		=> SetAsyncValue(elements, context, item => new ValueTask<bool>(predicate(item)), cancellationToken);

	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected async ValueTask<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T>? elements,
		IEvaluationContext context, Func<T, ValueTask<bool>> predicate, CancellationToken cancellationToken)
	{
		if (!StartEvaluation(elements, typeof(IAsyncEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T> notMatching = [];
		try
		{
			await foreach (T item in context.UseMaterializedAsyncEnumerable(elements!, cancellationToken))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return this;
				}

				(await predicate(item) ? matching : notMatching).Add(item);
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			return this;
		}

		return Complete(matching, notMatching);
	}
#endif

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     not negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are not
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null);

	/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsNegated)
		{
			AppendNegatedExpectation(stringBuilder, indentation);
		}
		else
		{
			AppendNormalExpectation(stringBuilder, indentation);
		}
	}

	/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_outcome == Outcome.FailureBothWays)
		{
			stringBuilder.Append(It).Append(Grammars.IsPlural() && It != "it" ? " were <null>" : " was <null>");
		}
		else if (_outcome == Outcome.Undecided)
		{
			AppendCanceledResult(stringBuilder, It);
		}
		else if (IsNegated)
		{
			AppendNegatedResult(stringBuilder, indentation);
		}
		else
		{
			AppendNormalResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc />
	/// <remarks>
	///     Adds the not matching items, which explain the failure. A negated expectation only fails when all items
	///     match, so it adds the whole collection instead.
	/// </remarks>
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (Outcome != Outcome.Failure)
		{
			return;
		}

		if (IsNegated)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection", FormatItems(Matching), -1));
		}
		else
		{
			contexts.Add(new ResultContext.SyncCallback("Not matching items", FormatItems(NotMatching),
				int.MaxValue));
		}
	}

	/// <summary>
	///     Returns the callback that formats the <paramref name="items" /> for the context.
	/// </summary>
	/// <remarks>
	///     The constraint can be evaluated again before the callback is invoked, so the callback must only use the
	///     state that is captured when this method is called.
	/// </remarks>
	protected virtual Func<string?> FormatItems(T[] items)
		=> () => Formatter.Format(FormattableMember.FromAll(items), FormattingOptions.MultipleLines);

	/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_elements is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return _subjectType is not null && typeof(TValue).IsAssignableFrom(_subjectType);
	}

	/// <inheritdoc cref="ConstraintResult.Negate()" />
	public override ConstraintResult Negate()
	{
		IsNegated = !IsNegated;
		return this;
	}

	private bool StartEvaluation(object? elements, Type subjectType)
	{
		_elements = elements;
		_subjectType = subjectType;
		Matching = [];
		NotMatching = [];
		_outcome = elements is null ? Outcome.FailureBothWays : Outcome.Undecided;
		return elements is not null;
	}

	private CollectionConstraintResult<T> Complete(List<T> matching, List<T> notMatching)
	{
		Matching = matching.ToArray();
		NotMatching = notMatching.ToArray();
		_outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}
}
