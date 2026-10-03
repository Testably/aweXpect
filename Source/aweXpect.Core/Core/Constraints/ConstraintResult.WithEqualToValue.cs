namespace aweXpect.Core.Constraints;

public abstract partial class ConstraintResult
{
	/// <summary>
	///     A typed <see cref="ConstraintResult" /> for equality comparison to ensure that
	///     <see langword="null" /> is handled consistently.
	/// </summary>
	/// <param name="it">The <c>it</c> parameter.</param>
	/// <param name="grammars">The expectation grammars.</param>
	/// <param name="isExpectedNull">
	///     Flag indicating if the expected parameter for the equality comparison is
	///     <see langword="null" /> or not.
	/// </param>
	/// <remarks>
	///     A <see langword="null" /> subject fails on the side where <see langword="null" /> is not a legitimate answer,
	///     which <paramref name="isExpectedNull" /> identifies:
	///     <code>
	/// IsEqualTo("foo")     null fails    (not negated, expected not null)
	/// IsEqualTo(null)      null succeeds (not negated, expected null)
	/// IsNotEqualTo("foo")  null succeeds (negated,     expected not null)
	/// IsNotEqualTo(null)   null fails    (negated,     expected null)
	///     </code>
	///     Only equality and identity give <see langword="null" /> a meaning on both sides. An ordering or a range does
	///     not, so <c>IsGreaterThan</c> and <c>IsNotBetween</c> use <see cref="ConstraintResult.WithNotNullValue{T}" />
	///     even though they too take a value from the caller.
	///     <para />
	///     Set <see cref="WithValue{T}.Actual" /> in one of the <c>IsMetBy</c> overloads of <see cref="IConstraint" /> and
	///     overwrite<br />
	///     - <see cref="WithValue{T}.AppendNormalExpectation" /> / <see cref="WithValue{T}.AppendNegatedExpectation" />
	///     which add the normal and negated expectation strings<br />
	///     - <see cref="WithValue{T}.AppendNormalResult" /> / <see cref="WithValue{T}.AppendNegatedResult" />
	///     which add the normal and negated result strings.
	/// </remarks>
	public abstract class WithEqualToValue<T>(string it, ExpectationGrammars grammars, bool isExpectedNull)
		: WithValue<T>(it, grammars)
	{
		/// <remarks>
		///     The table does not depend on the comparison of the caller, which could consider <see langword="null" />
		///     equal to another value.
		/// </remarks>
		private protected override Outcome? GetNullSubjectOutcome()
			=> IsNegated == isExpectedNull ? Outcome.Failure : null;

		/// <inheritdoc />
		private protected override bool RendersNullSubject => true;
	}
}
