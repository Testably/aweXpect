using System.Net;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static StatusCodeResult HasStatusCode(this IThat<HttpResponseMessage?> subject)
		=> new(subject, a => a.StatusCode);

	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject
	///     is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> HasStatusCode(
		this IThat<HttpResponseMessage?> subject,
		HttpStatusCode expected)
		=> new(subject.Get().ExpectationBuilder
			.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
			.AddConstraint(expected, static (expected, it, grammars) =>
				new StatusCodeResult.PropertyConstraint(
					it, grammars,
					expected,
					m => m.StatusCode,
					(a, e) => a == e,
					"status code ",
					true)), subject);

	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject
	///     is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> DoesNotHaveStatusCode(
		this IThat<HttpResponseMessage?> subject,
		HttpStatusCode unexpected)
		=> new(subject.Get().ExpectationBuilder
			.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
			.AddConstraint(unexpected, static (unexpected, it, grammars) =>
				new StatusCodeResult.PropertyConstraint(
					it, grammars,
					unexpected,
					m => m.StatusCode,
					(a, e) => a == e,
					"status code ",
					true).Invert()), subject);
}
