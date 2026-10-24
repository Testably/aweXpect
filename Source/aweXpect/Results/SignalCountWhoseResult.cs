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
		ISignalerResult<SignalCountWhoseResult<TParameter>, TParameter>
{
	private readonly SignalerOptions<TParameter> _options;
	private readonly Quantifier _quantifier;

	internal SignalCountWhoseResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>> returnValue,
		Quantifier quantifier,
		SignalerOptions<TParameter> options)
		: base(expectationBuilder, returnValue)
	{
		_quantifier = quantifier;
		_options = options;
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
				x => x.Parameters.Where(_options.Matches).ToArray(),
				" with parameters that ", null,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural |
				            ExpectationGrammars.Introduced));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions<TParameter> IOptionsProvider<SignalerOptions<TParameter>>.Options => _options;
}
