using System;
using System.Net.Http;
using System.Text;
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
				.AddConstraint((expectationBuilder, it, grammars) =>
					new HasContentTypeConstraint(expectationBuilder, it, grammars, expected, options)),
			source,
			options);
	}

	private sealed class HasContentTypeConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IValueConstraint<HttpResponseMessage>
	{
		private string? _contentType;

		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (!actual.Content.TryGetMediaType(out _contentType))
			{
				expectationBuilder.AddContext(actual);
				Outcome = Outcome.Failure;
				return this;
			}

			if (!options.AreConsideredEqual(_contentType, expected))
			{
				expectationBuilder.AddContext(actual);
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a `Content-Type` header ")
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_contentType is null)
			{
				stringBuilder.Append(It).Append(" had no `Content-Type` header");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It, Grammars, _contentType, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a `Content-Type` header ")
				.Append(options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
