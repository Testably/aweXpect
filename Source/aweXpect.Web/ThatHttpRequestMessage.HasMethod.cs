using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" /> method.
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasMethod(this IThat<HttpRequestMessage?> source, HttpMethod expected)
		=> new(
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasMethodConstraint(expectationBuilder, it, expected)),
			source);

	private readonly struct HasMethodConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		HttpMethod expected)
		: IValueConstraint<HttpRequestMessage>
	{
		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

			if (actual.Method != expected)
			{
				expectationBuilder.AddContext(actual);
				return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
					$"{it} was {actual.Method}");
			}

			return new ConstraintResult.Success<HttpRequestMessage?>(actual, ToString());
		}

		public override string ToString()
			=> $"has a {expected} method";
	}
}
