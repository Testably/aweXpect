using System.Net.Http;
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
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasContentTypeConstraint(expectationBuilder, it, expected, options)),
			source,
			options);
	}

	private readonly struct HasContentTypeConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string expected,
		StringEqualityOptions options)
		: IValueConstraint<HttpResponseMessage>
	{
		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

			if (!actual.Content.TryGetMediaType(out string? contentType))
			{
				expectationBuilder.AddContext(actual);
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} had no `Content-Type` header");
			}

			if (!options.AreConsideredEqual(contentType, expected))
			{
				expectationBuilder.AddContext(actual);
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					options.GetExtendedFailure(it, contentType, expected));
			}

			return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
		}

		public override string ToString()
			=> $"has a `Content-Type` header {options.GetExpectation(expected, ExpectationGrammars.None)}";
	}
}
