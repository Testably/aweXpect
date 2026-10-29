using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The expectation that the type of a single item, e.g. the type of a field, is one of the types of the
///     <see cref="Options" />.
/// </summary>
internal sealed class OfTypeExpectation<TMember>(Func<TMember, Type?> getType, Type type, bool exactly)
	: ItemExpectation<TMember>
	where TMember : class
{
	/// <summary>
	///     The alternative types, which the result of the expectation changes.
	/// </summary>
	public OptionsHolder<TypeFilterOptions> Options { get; } = new(TypeFilterOptions.Default.WithType(type, exactly));

	public override string Verb => "were";

	public override ValueTask<PredicateResult> IsMatch(TMember item, CancellationToken cancellationToken)
		=> new(Options.Value.Matches(getType(item)));

	public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
	{
		stringBuilder.Append(grammars.IsPlural() ? "are " : "is ");
		Options.Value.AppendOfTypeDescription(stringBuilder);
	}
}
