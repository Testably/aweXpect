using System;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     A result that combines a <see cref="Left" /> and a <see cref="Right" /> part, which are both met (<c>and</c>) or
///     of which one is met (<c>or</c>).
/// </summary>
/// <remarks>
///     A negation swaps the junction (De Morgan), so that a part which stays failed under negation keeps the combination
///     failed.
/// </remarks>
internal abstract class CombinedResult : ConstraintResult
{
	private readonly bool _isAnd;

	/// <summary>
	///     Initializes the combination of the <paramref name="left" /> and the <paramref name="right" /> part.
	/// </summary>
	protected CombinedResult(ConstraintResult left, ConstraintResult right, bool isAnd,
		FurtherProcessingStrategy furtherProcessingStrategy) : base(furtherProcessingStrategy)
	{
		Left = left;
		Right = right;
		_isAnd = isAnd;
	}

	/// <summary>
	///     The left part.
	/// </summary>
	protected ConstraintResult Left { get; set; }

	/// <summary>
	///     The right part.
	/// </summary>
	protected ConstraintResult Right { get; set; }

	/// <summary>
	///     Whether the combination is negated, which swaps the junction.
	/// </summary>
	protected bool IsNegated { get; set; }

	/// <inheritdoc />
	/// <remarks>
	///     Only a part that explains the failure contributes its cause, as a met part can hold an exception that is the
	///     reason why it is met, e.g. the parse exception of <c>IsNotParsableInto</c>.
	/// </remarks>
	public override Exception? FailureCause
	{
		get
		{
			if (Outcome is not (Outcome.Failure or Outcome.FailureBothWays))
			{
				return null;
			}

			(bool explainsLeft, bool explainsRight) = GetExplainingParts();
			return (explainsLeft ? Left.FailureCause : null) ?? (explainsRight ? Right.FailureCause : null);
		}
	}

	/// <summary>
	///     Which parts explain the outcome of the combination.
	/// </summary>
	/// <remarks>
	///     The result text renders these parts, except a right part whose result text only repeats the left one.
	/// </remarks>
	protected abstract (bool Left, bool Right) GetExplainingParts();

	/// <inheritdoc />
	/// <remarks>
	///     Visits the parts that explain the outcome, also a right part whose result text is omitted as a repetition, as
	///     its contexts can differ.
	/// </remarks>
	public sealed override void AppendContexts(ResultContextCollector contexts)
	{
		(bool explainsLeft, bool explainsRight) = GetExplainingParts();
		if (explainsLeft)
		{
			contexts.Visit(Left);
		}

		if (explainsRight)
		{
			VisitRight(contexts);
		}
	}

	/// <summary>
	///     Visits the right part, which explains the outcome.
	/// </summary>
	protected virtual void VisitRight(ResultContextCollector contexts)
		=> contexts.Visit(Right);

	/// <summary>
	///     Combines the outcomes of both parts with the junction for the current negation.
	/// </summary>
	/// <remarks>
	///     A part which only contributes an expectation text does not take part in the combination.
	/// </remarks>
	protected Outcome CombineOutcomes()
	{
		if (Left.IsExpectationOnly)
		{
			return Right.Outcome;
		}

		if (Right.IsExpectationOnly)
		{
			return Left.Outcome;
		}

		return _isAnd != IsNegated ? And(Left.Outcome, Right.Outcome) : Or(Left.Outcome, Right.Outcome);
	}

	/// <inheritdoc />
	public override bool TryGetStoredValue<TValue>(out TValue? value)
		where TValue : default
	{
		if (Left.TryGetStoredValue(out TValue? leftValue))
		{
			value = leftValue;
			return true;
		}

		if (Right.TryGetStoredValue(out TValue? rightValue))
		{
			value = rightValue;
			return true;
		}

		value = default;
		return false;
	}

	/// <remarks>
	///     A part that fails both ways makes the combination fail both ways, unless the other part decides it under
	///     both negations: a failed part is met by the negation (<c>or</c>), and an undecided part leaves the negation
	///     undecided, so the combination only fails.
	/// </remarks>
	private static Outcome And(Outcome left, Outcome right)
		=> (left, right) switch
		{
			(Outcome.Success, Outcome.Success) => Outcome.Success,
			(_, Outcome.Failure) => Outcome.Failure,
			(Outcome.Failure, _) => Outcome.Failure,
			(Outcome.FailureBothWays, Outcome.Undecided) or (Outcome.Undecided, Outcome.FailureBothWays)
				=> Outcome.Failure,
			(Outcome.FailureBothWays, _) or (_, Outcome.FailureBothWays) => Outcome.FailureBothWays,
			(_, _) => Outcome.Undecided,
		};

	/// <remarks>
	///     A part that fails both ways makes a failed combination fail both ways, as the negation (<c>and</c>) fails
	///     with it.
	/// </remarks>
	private static Outcome Or(Outcome left, Outcome right)
		=> (left, right) switch
		{
			(_, Outcome.Success) => Outcome.Success,
			(Outcome.Success, _) => Outcome.Success,
			(Outcome.Failure, Outcome.Failure) => Outcome.Failure,
			(Outcome.Failure or Outcome.FailureBothWays, Outcome.Failure or Outcome.FailureBothWays)
				=> Outcome.FailureBothWays,
			(_, _) => Outcome.Undecided,
		};
}
