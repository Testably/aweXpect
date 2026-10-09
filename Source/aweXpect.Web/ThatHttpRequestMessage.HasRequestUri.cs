using System;
using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> source, string expected)
		=> new(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasRequestUriConstraint(it, grammars, new Uri(expected).ToString())),
			source);

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> source, Uri expected)
		=> new(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasRequestUriConstraint(it, grammars, expected.ToString())),
			source);

	private sealed class HasRequestUriConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		private string? _requestUri;

		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_requestUri = actual.RequestUri?.ToString();
			if (_requestUri?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has a request URI equal to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, _requestUri);
			stringBuilder.Append(" which ");
			stringBuilder.Append(new StringDifference(_requestUri, expected));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not have a request URI equal to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
