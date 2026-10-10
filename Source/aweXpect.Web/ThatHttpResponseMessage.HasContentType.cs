using System;
using System.Net.Http;
using System.Text;
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
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContentType(this IThat<HttpResponseMessage?> source, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.ResponseContexts)
				.AddConstraint((it, grammars) =>
					new HasContentTypeConstraint(it, grammars, expected, options)),
			source,
			options);
	}

	private sealed class HasContentTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IAsyncConstraint<HttpResponseMessage>
	{
		private string? _contentType;

		public async ValueTask<ConstraintResult> IsMetBy(HttpResponseMessage? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (!actual.Content.TryGetMediaType(out _contentType))
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (!await options.AreConsideredEqual(_contentType, expected))
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a `Content-Type` header ", "have a `Content-Type` header "))
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_contentType is null)
			{
				stringBuilder.Append(It).Append(" had no `Content-Type` header");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedMemberFailure(It, "content type", Grammars, _contentType, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a `Content-Type` header ", "do not have a `Content-Type` header "))
				.Append(options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
