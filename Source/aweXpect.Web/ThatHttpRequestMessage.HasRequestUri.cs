using System;
using System.Net.Http;
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
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasRequestUriConstraint(expectationBuilder, it, new Uri(expected).ToString())),
			source);

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> source, Uri expected)
		=> new(
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasRequestUriConstraint(expectationBuilder, it, expected.ToString())),
			source);

	private readonly struct HasRequestUriConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string expected)
		: IValueConstraint<HttpRequestMessage>
	{
		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

			string? requestUri = actual.RequestUri?.ToString();
			if (requestUri?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true)
			{
				return new ConstraintResult.Success<HttpRequestMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
				$"{it} was {Formatter.Format(requestUri)} which {new StringDifference(requestUri, expected)}");
		}

		public override string ToString()
			=> $"has a request URI equal to {Formatter.Format(expected)}";
	}
}
