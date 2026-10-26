using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Web;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> has the <paramref name="expected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="expected" /> value against the media type in the <c>Content-Type</c> header.
	///     The media type is compared ignoring case by default, as RFC 9110 defines it case-insensitively;
	///     <c>IgnoringCase(false)</c> compares it case-sensitively.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContentType(this IThat<HttpResponseMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(expected, static (expected, it, grammars) =>
						new HasContentTypeConstraint<HttpResponseMessage>(it, grammars, expected, "expected",
							StringEqualityOptions.Default.IgnoringCase(), static m => m.Content),
					out HasContentTypeConstraint<HttpResponseMessage> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> does not have the <paramref name="unexpected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="unexpected" /> value against the media type in the <c>Content-Type</c> header,
	///     so a response without a <c>Content-Type</c> header succeeds.
	///     The media type is compared ignoring case by default, as RFC 9110 defines it case-insensitively;
	///     <c>IgnoringCase(false)</c> compares it case-sensitively.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		DoesNotHaveContentType(this IThat<HttpResponseMessage?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(unexpected, static (unexpected, it, grammars) =>
						new HasContentTypeConstraint<HttpResponseMessage>(it, grammars, unexpected, "unexpected",
							StringEqualityOptions.Default.IgnoringCase(), static m => m.Content).Invert(),
					out HasContentTypeConstraint<HttpResponseMessage> constraint),
			subject,
			constraint);
	}
}
