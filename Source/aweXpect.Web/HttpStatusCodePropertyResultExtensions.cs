using System.Net;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The status code classes of an <see cref="IEnumPropertyResult{TResult, TEnum}" /> for a
///     <see cref="HttpStatusCode" /> property, e.g. of <c>HasStatusCode()</c>.
/// </summary>
public static class HttpStatusCodePropertyResultExtensions
{
	private static readonly PropertyComparison<HttpStatusCode> SuccessComparison
		= PropertyComparison<HttpStatusCode>.Satisfying(
			static code => (int?)code is >= 200 and < 300, "in the success range (2xx)");

	private static readonly PropertyComparison<HttpStatusCode> RedirectionComparison
		= PropertyComparison<HttpStatusCode>.Satisfying(
			static code => (int?)code is >= 300 and < 400, "in the redirection range (3xx)");

	private static readonly PropertyComparison<HttpStatusCode> ClientErrorComparison
		= PropertyComparison<HttpStatusCode>.Satisfying(
			static code => (int?)code is >= 400 and < 500, "in the client error range (4xx)");

	private static readonly PropertyComparison<HttpStatusCode> ServerErrorComparison
		= PropertyComparison<HttpStatusCode>.Satisfying(
			static code => (int?)code is >= 500 and < 600, "in the server error range (5xx)");

	private static readonly PropertyComparison<HttpStatusCode> ErrorComparison
		= PropertyComparison<HttpStatusCode>.Satisfying(
			static code => (int?)code is >= 400 and < 600, "in the error range (4xx or 5xx)");

	/// <summary>
	///     …is a success status code (2xx).
	/// </summary>
	public static TResult Success<TResult>(this IEnumPropertyResult<TResult, HttpStatusCode> result)
		=> result.Compare(SuccessComparison);

	/// <summary>
	///     …is a redirection status code (3xx).
	/// </summary>
	public static TResult Redirection<TResult>(this IEnumPropertyResult<TResult, HttpStatusCode> result)
		=> result.Compare(RedirectionComparison);

	/// <summary>
	///     …is a client error status code (4xx).
	/// </summary>
	public static TResult ClientError<TResult>(this IEnumPropertyResult<TResult, HttpStatusCode> result)
		=> result.Compare(ClientErrorComparison);

	/// <summary>
	///     …is a server error status code (5xx).
	/// </summary>
	public static TResult ServerError<TResult>(this IEnumPropertyResult<TResult, HttpStatusCode> result)
		=> result.Compare(ServerErrorComparison);

	/// <summary>
	///     …is a client or server error status code (4xx or 5xx).
	/// </summary>
	public static TResult Error<TResult>(this IEnumPropertyResult<TResult, HttpStatusCode> result)
		=> result.Compare(ErrorComparison);
}
