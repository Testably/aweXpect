using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often a <see cref="Signaler{TParameter}" /> was signaled, which allows specifying
///     the timeout, filtering the signals by their parameter and verifying the parameters of the signals.
/// </summary>
public class SignalCountWhoseResult<TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	Quantifier quantifier,
	SignalerOptions<TParameter> options)
	: SignalCountResult<TParameter, SignalCountWhoseResult<TParameter>>(expectationBuilder, returnValue, quantifier,
		options)
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     …with parameters that…
	/// </summary>
	public IThat<IEnumerable<TParameter>> WhoseParameters
		=> new ThatSubject<IEnumerable<TParameter>>(
			_expectationBuilder.ForWhich<Signaler<TParameter>, IEnumerable<TParameter>>(
				x => x.Wait(timeout: TimeSpan.Zero).Parameters,
				" with parameters that ", null,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural |
				            ExpectationGrammars.Introduced));
}
