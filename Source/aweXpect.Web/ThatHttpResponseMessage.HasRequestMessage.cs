using System;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the response has a request message which satisfies the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasRequestMessage(this IThat<HttpResponseMessage?> source, Action<IThat<HttpRequestMessage>> expectations)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.ForMember(MemberAccessor<HttpResponseMessage?, HttpRequestMessage?>.FromFunc(
						response => response?.RequestMessage,
						"has a request message which "),
					null,
					false)
				.AddExpectations(expectationBuilder =>
						expectations(new ThatSubject<HttpRequestMessage>(expectationBuilder)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);
}
