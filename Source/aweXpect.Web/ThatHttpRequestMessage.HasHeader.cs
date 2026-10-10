using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" /> header.
	/// </summary>
	[GuaranteesNotNull]
	public static HasHeaderValueResult<HttpRequestMessage, IThat<HttpRequestMessage?>> HasHeader(
		this IThat<HttpRequestMessage?> source,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasHeaderConstraint(it, grammars, expected)),
			source,
			a => a.Headers.TryGetValues(expected, out IEnumerable<string>? values) ? values.ToArray() : null);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" /> header.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>> DoesNotHaveHeader(
		this IThat<HttpRequestMessage?> source,
		string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		return new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasHeaderConstraint(it, grammars, unexpected).Invert()),
			source);
	}

	private sealed class HasHeaderConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		private IEnumerable<string>? _foundHeader;

		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual.Headers.TryGetValues(expected, out _foundHeader))
			{
				Outcome = Outcome.Success;
				return this;
			}

			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(expected.IndefiniteArticleForHeader()).Append(' ');
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header");
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not contain the expected header");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "))
				.Append(expected.IndefiniteArticleForHeader()).Append(' ');
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header");
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain the ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header: ");
			Formatter.Format(stringBuilder, _foundHeader);
		}
	}
}
