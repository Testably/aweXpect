using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often a <see cref="Signaler{TParameter}" /> was signaled, which allows specifying
///     the timeout, filtering the signals by their parameter and verifying the parameters of the signals.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and <see cref="SignalerOptionsExtensions" />.
/// </remarks>
public class SignalCountWhoseResult<TParameter>
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>, SignalCountWhoseResult<TParameter>>,
		IOptionsProvider<Quantifier>,
		IOptionsProvider<SignalerOptions>,
		ISignalerResult<SignalCountWhoseResult<TParameter>, TParameter>
{
	private readonly OptionsSlot<SignalerOptions> _options;
	private readonly OptionsSlot<SignalerPredicateOptions<TParameter>> _predicates;
	private readonly OptionsSlot<Quantifier> _quantifier;

	internal SignalCountWhoseResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>> returnValue,
		OptionsSlot<Quantifier> quantifier,
		OptionsSlot<SignalerOptions> options,
		OptionsSlot<SignalerPredicateOptions<TParameter>> predicates)
		: base(expectationBuilder, returnValue)
	{
		_quantifier = quantifier;
		_options = options;
		_predicates = predicates;
	}

	/// <summary>
	///     …with parameters that…
	/// </summary>
	/// <remarks>
	///     The parameters are the ones that were counted, i.e. that match the predicates from <c>With</c>.
	/// </remarks>
	public IThatSubject<IEnumerable<TParameter>> WhoseParameters
		=> new ThatSubject<IEnumerable<TParameter>>(
			ExpectationBuilder.ForWhich<SignalerResult<TParameter>, IEnumerable<TParameter>>(
				x => x.Parameters.Where(_predicates.Value.Matches).ToArray(),
				" with parameters that ", null,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural |
				            ExpectationGrammars.Introduced));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<Quantifier> IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<SignalerPredicateOptions<TParameter>> IOptionsProvider<SignalerPredicateOptions<TParameter>>.Options
		=> _predicates;
}
