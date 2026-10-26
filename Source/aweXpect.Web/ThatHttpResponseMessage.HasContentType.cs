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
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContentType(this IThat<HttpResponseMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((expected, options), static (state, it, grammars) =>
					new HasContentTypeConstraint<HttpResponseMessage>(it, grammars, state.expected, "expected", state.options,
						static m => m.Content)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> does not have the <paramref name="unexpected" /> content type.
	/// </summary>
	/// <remarks>
	///     This compares the <paramref name="unexpected" /> value against the media type in the <c>Content-Type</c> header,
	///     so a response without a <c>Content-Type</c> header succeeds.
	///     <br />
	///     <seealso href="https://www.iana.org/assignments/media-types/media-types.xhtml" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		DoesNotHaveContentType(this IThat<HttpResponseMessage?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((unexpected, options), static (state, it, grammars) =>
					new HasContentTypeConstraint<HttpResponseMessage>(it, grammars, state.unexpected, "unexpected", state.options,
						static m => m.Content).Invert()),
			subject,
			options);
	}
}
