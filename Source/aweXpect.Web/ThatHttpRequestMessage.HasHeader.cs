using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
	public static HasHeaderValueResult<HttpRequestMessage, IThat<HttpRequestMessage?>> HasHeader(
		this IThat<HttpRequestMessage?> source,
		string expected)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasHeaderConstraint(expectationBuilder, it, expected)),
			source,
			a => a.Headers.TryGetValues(expected, out IEnumerable<string>? values) ? values.ToArray() : null);

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" /> header.
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>> DoesNotHaveHeader(
		this IThat<HttpRequestMessage?> source,
		string unexpected)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new DoesNotHaveHeaderConstraint(expectationBuilder, it, unexpected)),
			source);

	private readonly struct HasHeaderConstraint(ExpectationBuilder expectationBuilder, string it, string expected)
		: IValueConstraint<HttpRequestMessage>
	{
		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
					$"{it} was <null>", FurtherProcessingStrategy.IgnoreResult);
			}

			if (actual.Headers.TryGetValues(expected, out _))
			{
				return new ConstraintResult.Success<HttpRequestMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
				$"{it} did not contain the expected header", FurtherProcessingStrategy.IgnoreResult);
		}

		public override string ToString()
			=> $"has a `{expected}` header";
	}

	private readonly struct DoesNotHaveHeaderConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string unexpected)
		: IValueConstraint<HttpRequestMessage>
	{
		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
					$"{it} was <null>", FurtherProcessingStrategy.IgnoreResult);
			}

			if (!actual.Headers.TryGetValues(unexpected, out IEnumerable<string>? foundHeader))
			{
				return new ConstraintResult.Success<HttpRequestMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpRequestMessage?>(actual, ToString(),
				$"{it} did contain the `{unexpected}` header: {Formatter.Format(foundHeader)}",
				FurtherProcessingStrategy.IgnoreResult);
		}

		public override string ToString()
			=> $"does not have a `{unexpected}` header";
	}
}
