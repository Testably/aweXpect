namespace aweXpect.Core.Constraints;

public abstract partial class ConstraintResult
{
	/// <summary>
	///     A typed <see cref="ConstraintResult" /> similar to <see cref="ConstraintResult.WithValue{T}" /> which stores
	///     the actual value in the <see cref="WithValue{T}.Actual" /> property and ensures that it is not
	///     <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     The <see langword="null" /> check is applied <b>before</b> the negation, so a <see langword="null" /> subject
	///     fails the expectation and its negation alike: there is no value to inspect, and negating a question that cannot
	///     be answered does not make it true. This is the base class for every expectation that inspects its subject.
	///     <para />
	///     Set <see cref="WithValue{T}.Actual" /> in one of the <c>IsMetBy</c> overloads of <see cref="IConstraint" /> and
	///     overwrite<br />
	///     - <see cref="WithValue{T}.AppendNormalExpectation" /> / <see cref="WithValue{T}.AppendNegatedExpectation" />
	///     which add the normal and negated expectation strings<br />
	///     - <see cref="WithValue{T}.AppendNormalResult" /> / <see cref="WithValue{T}.AppendNegatedResult" />
	///     which add the normal and negated result strings.
	/// </remarks>
	public abstract class WithNotNullValue<T>(string it, ExpectationGrammars grammars)
		: WithValue<T>(it, grammars)
	{
		/// <inheritdoc />
		private protected override Outcome? GetNullSubjectOutcome() => Outcome.Failure;

		/// <inheritdoc />
		private protected override bool RendersNullSubject => true;
	}
}
