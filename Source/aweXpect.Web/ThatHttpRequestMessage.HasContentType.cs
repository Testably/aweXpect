using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Web;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="expected" /> value against the media type in the <c>Content-Type</c> header,
	///     so a request without content fails.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasContentType(this IThat<HttpRequestMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(expected)));
		return new StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint((expected, options), static (state, it, grammars) =>
					new HasContentTypeConstraint<HttpRequestMessage>(it, grammars, state.expected, state.options,
						static m => m.Content)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="unexpected" /> value against the media type in the <c>Content-Type</c> header,
	///     so a request without content or without a <c>Content-Type</c> header succeeds.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveContentType(this IThat<HttpRequestMessage?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(unexpected)));
		return new StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint((unexpected, options), static (state, it, grammars) =>
					new HasContentTypeConstraint<HttpRequestMessage>(it, grammars, state.unexpected, state.options,
						static m => m.Content).Invert()),
			subject,
			options);
	}
}
