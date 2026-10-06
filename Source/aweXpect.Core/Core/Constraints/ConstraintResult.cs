using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace aweXpect.Core.Constraints;

/// <summary>
///     The result of the check if an expectation is met.
/// </summary>
public abstract partial class ConstraintResult
{
	/// <summary>
	///     Initializes a new instance of <see cref="ConstraintResult" />.
	/// </summary>
	protected ConstraintResult(FurtherProcessingStrategy furtherProcessingStrategy)
	{
		Grammars = ExpectationGrammars.None;
		FurtherProcessingStrategy = furtherProcessingStrategy;
	}

	/// <summary>
	///     Initializes a new instance of <see cref="ConstraintResult" />.
	/// </summary>
	protected ConstraintResult(ExpectationGrammars grammars)
	{
		Grammars = grammars;
		FurtherProcessingStrategy = FurtherProcessingStrategy.Continue;
	}

	/// <summary>
	///     The <see cref="ExpectationGrammars" /> of the constraint result.
	/// </summary>
	public ExpectationGrammars Grammars { get; protected set; }

	/// <summary>
	///     The outcome of the <see cref="ConstraintResult" />.
	/// </summary>
	public virtual Outcome Outcome { get; protected set; } = Outcome.Undecided;

	/// <summary>
	///     Specifies if further processing of chained constraints should be ignored.
	/// </summary>
	public FurtherProcessingStrategy FurtherProcessingStrategy { get; protected set; }

	/// <summary>
	///     The <see cref="Exception" /> that caused the failure, or <see langword="null" /> when the failure was not caused
	///     by an exception.
	/// </summary>
	/// <remarks>
	///     It is forwarded as inner exception of the framework-specific assertion exception, so that the original stack trace
	///     remains available.
	/// </remarks>
	public virtual Exception? FailureCause => null;

	/// <summary>
	///     The subject that the result text starts with, or <see langword="null" /> when it is unknown.
	/// </summary>
	/// <remarks>
	///     When two results are combined with <c>and</c> on the same line, the result on the right omits its subject if
	///     it equals the <see cref="TrailingSubject" /> of the result on the left, e.g. "it was 2 and was not even".
	///     Override both with <see cref="GetSubjectOfResult" />, unless a helper class like
	///     <see cref="WithNotNullValue{T}" /> already does.
	/// </remarks>
	public virtual string? LeadingSubject => null;

	/// <summary>
	///     The subject that the last part of the result text starts with, or <see langword="null" /> when it is unknown.
	/// </summary>
	public virtual string? TrailingSubject => null;

	/// <summary>
	///     Indicates that the result only contributes an expectation text, so that combinations ignore its outcome.
	/// </summary>
	internal virtual bool IsExpectationOnly => false;

	/// <summary>
	///     Indicates that the expectation text combines its operands with the "or" of a negated <c>And</c>, so that a
	///     surrounding "and" has to group it.
	/// </summary>
	internal virtual bool IsNegatedAnd => false;

	/// <summary>
	///     Appends the result text for an expectation that could not be verified, because its evaluation was canceled,
	///     to the <paramref name="stringBuilder" />, starting with <paramref name="it" />.
	/// </summary>
	protected static void AppendCanceledResult(StringBuilder stringBuilder, string it)
		=> stringBuilder.Append(it).Append(" could not be verified, because the evaluation was already canceled");

	/// <summary>
	///     Returns <paramref name="it" />, when the result text starts with it, otherwise <see langword="null" />.
	/// </summary>
	protected string? GetSubjectOfResult(string it)
	{
		StringBuilder sb = new();
		AppendResult(sb);
		return sb.ToString().StartsWith(it + " ", StringComparison.Ordinal) ? it : null;
	}

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result text to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Tries to extract the <paramref name="value" /> of type <typeparamref name="TValue" /> that is stored in the
	///     constraint result, which can also be <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     Returns <see langword="true" /> with a <see langword="null" /> <paramref name="value" />, when the result stores
	///     a value of a type assignable to <typeparamref name="TValue" />, which is <see langword="null" />.
	/// </remarks>
	public abstract bool TryGetStoredValue<TValue>(out TValue? value);

	/// <summary>
	///     Tries to extract the non-<see langword="null" /> <paramref name="value" /> of type
	///     <typeparamref name="TValue" /> that is stored in the constraint result.
	/// </summary>
	public bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value)
		=> TryGetStoredValue(out value) && value is not null;

	/// <summary>
	///     Negate the current <see cref="ConstraintResult" />.
	/// </summary>
	public abstract ConstraintResult Negate();

	/// <summary>
	///     Adds the contexts that explain the failure of this result, e.g. the items of a collection, to the
	///     <paramref name="contexts" />.
	/// </summary>
	/// <remarks>
	///     It is only called while the failure message is created, and only for the parts of the result that explain the
	///     failure, so a context must not be created during the evaluation. The negation is already applied, so the
	///     contexts can depend on it.
	///     <para />
	///     A result that combines other results visits those that it renders with
	///     <see cref="ResultContextCollector.Visit(ConstraintResult)" />.
	///     <para />
	///     The contexts capture the state they render when they are added, as the result can be evaluated again (e.g. for
	///     the next item of a collection) before their content is created.
	/// </remarks>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
		// A result without contexts adds nothing.
	}
}
