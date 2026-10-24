using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a <see cref="Signaler{TParameter}" /> was signaled a given number of times, which
///     allows specifying the timeout, filtering the signals by their parameter and verifying the parameters of the
///     signals.
/// </summary>
/// <remarks>
///     The number of times is already given, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountWhoseResult{TParameter}" /> to specify how often a callback must be signaled. The options
///     are specified via <see cref="SignalerOptionsExtensions" />.
/// </remarks>
public class SignaledWhoseResult<TParameter>
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>, SignaledWhoseResult<TParameter>>,
		ISignalerResult<SignaledWhoseResult<TParameter>, TParameter>
{
	private readonly SignalerOptions<TParameter> _options;

	internal SignaledWhoseResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>> returnValue,
		SignalerOptions<TParameter> options)
		: base(expectationBuilder, returnValue)
	{
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
	SignalerOptions<TParameter> IOptionsProvider<SignalerOptions<TParameter>>.Options => _options;
}
