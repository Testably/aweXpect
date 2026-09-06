using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     A trigger result that also allows specifying the timeout.
/// </summary>
public class SignalCountWhoseResult<TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	SignalerOptions<TParameter> options)
	: SignalCountResult<TParameter, SignalCountWhoseResult<TParameter>>(expectationBuilder, returnValue, options)
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     …and whose parameters…
	/// </summary>
	public IThat<IEnumerable<TParameter>> WhoseParameters
		=> new ThatSubject<IEnumerable<TParameter>>(
			_expectationBuilder.ForWhich<Signaler<TParameter>, IEnumerable<TParameter>>(
				x => x.Wait(timeout: TimeSpan.Zero).Parameters,
				" and whose parameters ", null,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural));
}
