using System.Net;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject…
	/// </summary>
	/// <remarks>
	///     Besides the equality operators, the result offers the status code classes, e.g. <c>Success()</c> or
	///     <c>ClientError()</c>, which are extension methods in <see cref="HttpStatusCodePropertyResultExtensions" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.Enum<HttpResponseMessage?, HttpResponseMessage, IThat<HttpResponseMessage?>,
		HttpStatusCode> HasStatusCode(this IThat<HttpResponseMessage?> subject)
	{
		subject.Get().ExpectationBuilder.AddSubjectContexts(ResultContextExtensions.ResponseContexts);
		return new(subject, response => response?.StatusCode, "status code");
	}

	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject
	///     is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> HasStatusCode(
		this IThat<HttpResponseMessage?> subject,
		HttpStatusCode expected)
		=> subject.HasStatusCode().EqualTo(expected);

	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject
	///     is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> DoesNotHaveStatusCode(
		this IThat<HttpResponseMessage?> subject,
		HttpStatusCode unexpected)
		=> subject.HasStatusCode().NotEqualTo(unexpected);
}
