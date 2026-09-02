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
	///     Appends the expectation to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result text to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Tries to extract the <paramref name="value" /> that is stored in the constraint result.
	/// </summary>
	public abstract bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value);

	/// <summary>
	///     Negate the current <see cref="ConstraintResult" />.
	/// </summary>
	public abstract ConstraintResult Negate();
}
