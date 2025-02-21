using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> has the <paramref name="expected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="expected" /> value against the media type in the <c>Content-Type</c> header.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContentType(this IThat<HttpResponseMessage?> source, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new HasContentTypeConstraint(it, expected, options)),
			source,
			options);
	}

	private readonly struct HasContentTypeConstraint(string it, string expected, StringEqualityOptions options)
		: IAsyncConstraint<HttpResponseMessage>
	{
		public async Task<ConstraintResult> IsMetBy(HttpResponseMessage? actual, CancellationToken cancellationToken)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

			if (!actual.Content.TryGetMediaType(out string? contentType))
			{
				string formattedResponse =
					await HttpResponseMessageFormatter.Format(actual, "  ", cancellationToken);
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} had no `Content-Type` header")
					.WithContext("HTTP-Request", formattedResponse);
			}

			if (!options.AreConsideredEqual(contentType, expected))
			{
				string formattedResponse =
					await HttpResponseMessageFormatter.Format(actual, "  ", cancellationToken);
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
						options.GetExtendedFailure(it, contentType, expected))
					.WithContext("HTTP-Request", formattedResponse);
			}

			return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
		}

		public override string ToString()
			=> $"has a `Content-Type` header {options.GetExpectation(expected, ExpectationGrammars.None)}";
	}
}
