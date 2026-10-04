using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core;

/// <summary>
///     A manual expectation builder can be used for manually evaluating inner expectations.
/// </summary>
public sealed class ManualExpectationBuilder<TValue>(
	ExpectationGrammars grammars = ExpectationGrammars.None)
	: ExpectationBuilder("", grammars),
		IEqualityComparer<ManualExpectationBuilder<TValue>>
{
	/// <summary>
	///     Appends the expectation of the root node to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     The reasons are not included, so that they can follow the whole expectation they are nested in, see
	///     <see cref="AppendReasons" />.
	/// </remarks>
	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> GetRootNode().AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Appends the reasons of the expectations to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     Append them after the whole expectation that the expectations are nested in (e.g. after a quantifier like
	///     <c>for all items</c>).<br />
	///     Reasons that must be awaited are omitted until they are resolved by <see cref="PrepareExpectation" /> or by a
	///     failed evaluation.
	/// </remarks>
	public void AppendReasons(StringBuilder stringBuilder)
	{
		foreach (IBecauseReason reason in Reasons)
		{
			stringBuilder.Append(reason);
		}
	}

	/// <summary>
	///     Prepares the expectation text without evaluating the expectations, so that
	///     <see cref="AppendExpectation" /> also describes expectations whose text depends on an evaluation (e.g. a nested
	///     <c>DoesNotComplyWith</c>) or on a reason that must be awaited, when no value is evaluated.
	/// </summary>
	public async Task PrepareExpectation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		await GetRootNode().IsMetBy<TValue>(default, ExpectationTextEvaluationContext.For(context), cancellationToken);
		await ResolveReasons();
	}

	/// <summary>
	///     Returns the pro-verb which stands in for the expectations when a result refers back to them, e.g. <c>did</c> in
	///     <c>starts with "a" for all items, but only 1 of 3 did</c>.
	/// </summary>
	/// <remarks>
	///     English requires do-support for every verb but <c>be</c>, so the pro-verb is <c>were</c> exactly when the
	///     expectation text is headed by a form of <c>be</c> (<c>is equal to 1</c>) and <c>did</c> otherwise. Deriving it
	///     from the whole expectation text rather than from the innermost constraint keeps a mapped expectation
	///     (<c>has length that is equal to 3</c>) tied to the verb the reader actually sees. An expectation without a
	///     text of its own falls back to <c>were</c>.
	/// </remarks>
	public string GetResultVerb()
	{
		StringBuilder stringBuilder = new();
		AppendExpectation(stringBuilder);
		int headLength = 0;
		while (headLength < stringBuilder.Length && stringBuilder[headLength] != ' ')
		{
			headLength++;
		}

		return stringBuilder.ToString(0, headLength) switch
		{
			"" or "is" or "are" or "was" or "were" => "were",
			_ => "did",
		};
	}

	/// <summary>
	///     Evaluate if the expectations are met by the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     The reasons are not applied to the result, see <see cref="AppendReasons" />.
	/// </remarks>
	public async Task<ConstraintResult> IsMetBy(
		TValue value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		ConstraintResult result = await GetRootNode().IsMetBy(value, context, cancellationToken);
		if (result.Outcome != Outcome.Success)
		{
			await ResolveReasons();
		}

		return result;
	}

	/// <summary>
	///     Evaluate if the expectations are met by the <paramref name="value" /> in an evaluation context of their own.
	/// </summary>
	/// <remarks>
	///     Use this overload when no <see cref="IEvaluationContext" /> of a surrounding evaluation is available;
	///     otherwise prefer <see cref="IsMetBy(TValue, IEvaluationContext, CancellationToken)" />, so that the
	///     expectations share the materialized collections of the surrounding evaluation.<br />
	///     The evaluation context is released before the returned task completes, also when the evaluation throws: the
	///     sources of the collections materialized during the evaluation (e.g. with
	///     <see cref="EvaluationContextExtensions.UseMaterializedEnumerable{TItem}(IEvaluationContext, IEnumerable{TItem})" />)
	///     are disposed, so a failure message created from the result afterwards only shows the items that the
	///     evaluation read. Only the <paramref name="cancellationToken" /> cancels the evaluation; no expectation
	///     timeout applies.<br />
	///     The reasons are not applied to the result, see <see cref="AppendReasons" />.
	/// </remarks>
	public async Task<ConstraintResult> IsMetBy(TValue value, CancellationToken cancellationToken)
	{
		EvaluationContext.EvaluationContext context = new()
		{
			Cancellation = EvaluationCancellation.Create(null, cancellationToken),
		};
		try
		{
			return await IsMetBy(value, context, cancellationToken);
		}
		finally
		{
			await context.ReleaseMaterializations();
		}
	}

	/// <inheritdoc />
	internal override ValueTask<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
		=> throw Tracing.WriteException(
			new NotSupportedException($"Use {nameof(IsMetBy)} for ManualExpectationBuilder."));

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append("it ");
		AppendExpectation(sb);
		AppendReasons(sb);
		return sb.ToString();
	}

	#region Equals methods

	/// <inheritdoc cref="IEqualityComparer{T}.Equals(T, T)" />
	public bool Equals(ManualExpectationBuilder<TValue>? x, ManualExpectationBuilder<TValue>? y)
	{
		if (x is null && y is null)
		{
			return true;
		}

		if (x is null || y is null)
		{
			return false;
		}

		return x.Equals(y);
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is ManualExpectationBuilder<TValue> other && Equals(other);

	/// <summary>
	///     Determines whether the <paramref name="other" /> object is equal to the current object.
	/// </summary>
	private bool Equals(ManualExpectationBuilder<TValue> other) => GetRootNode().Equals(other.GetRootNode());

	#endregion

	#region GetHashCode methods

	/// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)" />
	public int GetHashCode(ManualExpectationBuilder<TValue> obj)
		=> obj.GetHashCode();

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => GetRootNode().GetHashCode();

	#endregion
}
