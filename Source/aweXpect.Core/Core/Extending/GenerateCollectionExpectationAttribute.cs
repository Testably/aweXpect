using System;

namespace aweXpect.Core.Extending;

/// <summary>
///     Generates an expectation on the elements of a collection from the marked <see langword="static" /> method, which
///     decides whether a single item meets it, e.g. <c>That(tracks).All().AreRadioFriendly()</c>.
/// </summary>
/// <remarks>
///     The method follows the rules of <see cref="GenerateExpectationAttribute" />: it returns
///     <see langword="bool" /> or a <see cref="PredicateResult" />, directly or as the result of a
///     <see cref="System.Threading.Tasks.Task{TResult}" /> or <see cref="System.Threading.Tasks.ValueTask{TResult}" />,
///     its first parameter is the item, and the other parameters become the parameters of the expectation, except for
///     a <see cref="System.Threading.CancellationToken" /> marked with
///     <see cref="EvaluationCancellationTokenAttribute" />. Both attributes can mark the same method.
///     <para />
///     An item that the method cannot answer fails the expectation and its negation with the reason of the
///     <see cref="PredicateResult" />, unless the items before already determine the outcome.
///     <para />
///     The expectation extends the elements that a quantifier like <c>All()</c> or <c>AtLeast(2)</c> selects, of every
///     kind of collection that can hold the item. A <see langword="null" /> item does not match, unless the item
///     parameter is nullable: then the method decides, and <c>[DisallowNull]</c> on the parameter restores that a
///     <see langword="null" /> item does not match.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateCollectionExpectationAttribute : Attribute
{
	/// <summary>
	///     Generates the expectation <paramref name="name" /> on the elements of a collection, with the
	///     <paramref name="expectationText" /> on a single item and its plural.
	/// </summary>
	public GenerateCollectionExpectationAttribute(string name, string expectationText, string pluralExpectationText)
	{
		Name = name;
		ExpectationText = expectationText;
		PluralExpectationText = pluralExpectationText;
	}

	/// <summary>
	///     Generates the expectation <paramref name="name" /> on the elements of a collection, whose text on a single
	///     item is the expectation text of the <see cref="GenerateExpectationAttribute" /> on the same method, without
	///     <c>{not}</c>.
	/// </summary>
	/// <remarks>
	///     The method has to be marked with <see cref="GenerateExpectationAttribute" /> as well.
	/// </remarks>
	public GenerateCollectionExpectationAttribute(string name, string pluralExpectationText)
	{
		Name = name;
		PluralExpectationText = pluralExpectationText;
	}

	/// <summary>
	///     The name of the expectation, e.g. <c>AreRadioFriendly</c>.
	/// </summary>
	public string Name { get; }

	/// <summary>
	///     The text of the expectation on a single item in the failure message, e.g. <c>"is radio friendly"</c> in
	///     "is radio friendly for all items", or <see langword="null" /> to use the expectation text of the
	///     <see cref="GenerateExpectationAttribute" /> on the same method.
	/// </summary>
	/// <remarks>
	///     The quantifier carries the negation, so the text is never negated. Its placeholders name parameters of the
	///     method, whose formatted values replace them.
	/// </remarks>
	public string? ExpectationText { get; }

	/// <summary>
	///     The plural of the <see cref="ExpectationText" />, e.g. <c>"are radio friendly"</c> in
	///     "has values of which at least 2 are radio friendly".
	/// </summary>
	public string PluralExpectationText { get; }

	/// <summary>
	///     Whether an expectation on a non-nullable value type also extends the collections of its nullable type, in
	///     which a <see langword="null" /> item does not match. Defaults to <see langword="true" />.
	/// </summary>
	public bool NullableOverload { get; set; } = true;

	/// <summary>
	///     The XML documentation of the summary of the expectation, instead of one that is derived from the
	///     <see cref="PluralExpectationText" />.
	/// </summary>
	public string? Summary { get; set; }

	/// <summary>
	///     The XML documentation of the remarks of the expectation.
	/// </summary>
	public string? Remarks { get; set; }
}
