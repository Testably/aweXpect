using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The expectation that a single item has one of the attributes of the <see cref="Filter" />.
/// </summary>
internal sealed class HaveAttributeExpectation<TMember>(AttributeFilterOptions<TMember?> options)
	: ItemExpectation<TMember>
	where TMember : class
{
	/// <summary>
	///     The alternative attributes and their inheritance scope, which the result of the expectation changes.
	/// </summary>
	public AttributeFilter<TMember?> Filter { get; } = new(options);

	public override ValueTask<PredicateResult> IsMatch(TMember item, CancellationToken cancellationToken)
		=> new(Filter.Matches(item));

	public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		=> Filter.AppendExpectation(stringBuilder, grammars);
}
