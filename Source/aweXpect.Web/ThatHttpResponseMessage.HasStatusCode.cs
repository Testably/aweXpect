using System.Net;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject…
	/// </summary>
	public static StatusCodeResult HasStatusCode(this IThat<HttpResponseMessage?> source)
		=> new(source, a => a.StatusCode);

	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject
	///     is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> HasStatusCode(
		this IThat<HttpResponseMessage?> source,
		HttpStatusCode? expected)
		=> new(source.ThatIs().ExpectationBuilder
			.UpdateContexts(c => c.Close())
			.AddConstraint((expectationBuilder, it, grammars) =>
				new StatusCodeResult.PropertyConstraint(
					expectationBuilder,
					it, grammars,
					expected,
					m => m.StatusCode,
					(a, e) => a.Equals(e),
					$"has status code {Formatter.Format(expected)}")), source);
}
