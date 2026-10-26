using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Extending;

/// <summary>
///     The result of a method marked with <see cref="GenerateExpectationAttribute" /> or
///     <see cref="GenerateCollectionExpectationAttribute" />: whether the subject meets the expectation, or why the
///     question cannot be answered.
/// </summary>
/// <remarks>
///     A <see langword="bool" /> converts implicitly, so the method returns <see langword="true" /> or
///     <see langword="false" /> as usual, and <see cref="FailureBothWays" /> only where it cannot decide. The
///     <see langword="default" /> value does not meet the expectation.
/// </remarks>
public readonly struct PredicateResult
{
	private readonly bool _isMet;

	private PredicateResult(bool isMet, string? reason)
	{
		_isMet = isMet;
		Reason = reason;
	}

	/// <summary>
	///     The outcome of the expectation that is not negated.
	/// </summary>
	public Outcome Outcome
	{
		get
		{
			if (Reason is not null)
			{
				return Outcome.FailureBothWays;
			}

			return _isMet ? Outcome.Success : Outcome.Failure;
		}
	}

	/// <summary>
	///     Why the question cannot be answered, when the <see cref="Outcome" /> is
	///     <see cref="Outcome.FailureBothWays" />.
	/// </summary>
	public string? Reason { get; }

	/// <summary>
	///     The question cannot be answered, so that the expectation and its negation fail alike, e.g. because a track
	///     has no duration to compare.
	/// </summary>
	/// <param name="reason">
	///     Why the question cannot be answered, which follows "it" in the failure message, e.g. "had no duration" in "but
	///     it had no duration".
	/// </param>
	public static PredicateResult FailureBothWays(string reason)
	{
		reason.ThrowIfNull();
		return new PredicateResult(false, reason);
	}

	/// <summary>
	///     Whether the subject meets the expectation.
	/// </summary>
	public static implicit operator PredicateResult(bool isMet) => new(isMet, null);

	/// <summary>
	///     The result of an item that the predicate could not answer, which a quantified collection constraint stops
	///     at.
	/// </summary>
	internal sealed class UnansweredItem(object? item, string reason) : ConstraintResult(ExpectationGrammars.None)
	{
		/// <inheritdoc />
		public override Outcome Outcome => Outcome.FailureBothWays;

		/// <inheritdoc />
		/// <remarks>
		///     The collection constraint states the expectation on the items.
		/// </remarks>
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it ").Append(reason);

		/// <inheritdoc />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (item is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return false;
		}

		/// <inheritdoc />
		protected internal override void NegateInPlace()
		{
			// An unanswered item fails both ways, also when negated.
		}
	}
}
