using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The expectation on a single item of a collection, which <see cref="ItemExpectationConstraints" /> checks for
///     the items that a quantifier selects.
/// </summary>
/// <remarks>
///     The options of a result, e.g. <c>Directly()</c>, are set after the expectation is added, so they are only read
///     while the items are checked or the expectation is described.
/// </remarks>
internal abstract class ItemExpectation<TItem>
	where TItem : class
{
	/// <summary>
	///     The verb in the past tense in the result, e.g. "did" in "but only 1 of 3 did".
	/// </summary>
	public virtual string Verb => "did";

	/// <summary>
	///     Whether the <paramref name="item" /> matches the expectation.
	/// </summary>
	public abstract ValueTask<PredicateResult> IsMatch(TItem item, CancellationToken cancellationToken);

	/// <summary>
	///     Appends the expectation on a single item for the <paramref name="grammars" />, e.g. "inherits from Base", or
	///     "inherit from Base" when the grammars are plural.
	/// </summary>
	public abstract void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars);

	/// <summary>
	///     Called when an evaluation starts, before the items are checked, e.g. to get the options of the evaluation
	///     from the <paramref name="context" /> or to clear the state that a previous evaluation kept for the failure
	///     message.
	/// </summary>
	public virtual void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
	{
	}

	/// <summary>
	///     Adds the contexts of the expectation to the failure message, e.g. those of its options.
	/// </summary>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
	}

	/// <summary>
	///     Formats the <paramref name="items" /> that the failure message lists.
	/// </summary>
	public virtual string FormatItems(IEnumerable<TItem?> items, FormattingOptions options)
		=> ValueFormatters.Format(Formatter, items.Select(item => FormattableMember.From(item)), options);
}
