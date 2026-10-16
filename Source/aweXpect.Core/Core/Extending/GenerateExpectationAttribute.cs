using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Generates an expectation from the marked <see langword="static" /> method, which decides whether the subject
///     meets it.
/// </summary>
/// <remarks>
///     The method returns <see langword="bool" /> or a <see cref="PredicateResult" />, which can also tell that the
///     question cannot be answered, directly or as the result of a <see cref="System.Threading.Tasks.Task{TResult}" />
///     or <see cref="System.Threading.Tasks.ValueTask{TResult}" />. Its first parameter is the subject, the other
///     parameters become the parameters of the expectation, except for a
///     <see cref="System.Threading.CancellationToken" /> marked with <see cref="EvaluationCancellationTokenAttribute" />,
///     which receives the token of the evaluation.
///     <para />
///     A <see langword="null" /> subject fails the expectation and its negation, unless the subject parameter is
///     nullable: then the method decides, and <c>[NotNullWhen]</c> on the parameter tells which of the expectations
///     guarantees a subject that is not <see langword="null" />. <c>[DisallowNull]</c> on a nullable subject parameter
///     restores the failure for a <see langword="null" /> subject.
///     <para />
///     The method has to be declared in a top-level, non-generic, <see langword="static" /> <see langword="partial" />
///     class that is not <see langword="file" />-local, which receives the generated extension methods.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateExpectationAttribute(string name, string expectationText) : Attribute
{
	/// <summary>
	///     The name of the expectation. A <c>{Not}</c> in it, or a <see cref="NegatedName" />, also generates the negated
	///     expectation, e.g. <c>"Is{Not}ActiveSince"</c> generates <c>IsActiveSince</c> and <c>IsNotActiveSince</c>.
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	///     The name of the negated expectation, e.g. <c>DoesNotHaveTitle</c> for <c>HasTitle</c>, instead of the
	///     <see cref="Name" /> with "Not" in place of <c>{Not}</c>.
	/// </summary>
	public string? NegatedName { get; set; }

	/// <summary>
	///     The text of the expectation in the failure message, e.g. <c>"is {not} active since {since}"</c>.
	/// </summary>
	/// <remarks>
	///     <c>{not}</c> is removed from the expectation and becomes "not" in its negation, so it is required unless a
	///     <see cref="NegatedExpectationText" /> is given. Any other placeholder names a parameter of the method, whose
	///     formatted value replaces it.
	/// </remarks>
	public string ExpectationText { get; } = expectationText;

	/// <summary>
	///     The text of the negated expectation in the failure message, e.g. <c>"does not have level {level}"</c>, instead
	///     of the <see cref="ExpectationText" /> with "not" in place of <c>{not}</c>.
	/// </summary>
	/// <remarks>
	///     Its placeholders name parameters of the method, like the ones of the <see cref="ExpectationText" />.
	/// </remarks>
	public string? NegatedExpectationText { get; set; }

	/// <summary>
	///     The name of a <see langword="static" /> method in the same class that describes the subject after "but", e.g.
	///     <c>"was activated on 2026-02-01"</c> in "but it was activated on 2026-02-01".
	/// </summary>
	/// <remarks>
	///     The method returns a <see langword="string" /> and takes the subject, optionally followed by the other
	///     parameters of the marked method and by a <see langword="string" /> <c>it</c> and the
	///     <see cref="ExpectationGrammars" />. With these two, the method describes the subject itself, e.g.
	///     "they were activated on 2026-02-01" instead of "it was …". Without a result method, the result shows the
	///     formatted subject, e.g. "but it was …".
	/// </remarks>
	public string? Result { get; set; }

	/// <summary>
	///     Whether an expectation on a non-nullable value type also gets an overload on its nullable type, which fails
	///     for a <see langword="null" /> subject. Defaults to <see langword="true" />.
	/// </summary>
	public bool NullableOverload { get; set; } = true;

	/// <summary>
	///     The XML documentation of the summary of the expectation, instead of one that is derived from the
	///     <see cref="ExpectationText" />.
	/// </summary>
	public string? Summary { get; set; }

	/// <summary>
	///     The XML documentation of the summary of the negated expectation, instead of one that is derived from the
	///     <see cref="ExpectationText" />.
	/// </summary>
	public string? NegatedSummary { get; set; }

	/// <summary>
	///     The XML documentation of the remarks of the expectation.
	/// </summary>
	public string? Remarks { get; set; }

	/// <summary>
	///     The XML documentation of the remarks of the negated expectation.
	/// </summary>
	public string? NegatedRemarks { get; set; }
}
