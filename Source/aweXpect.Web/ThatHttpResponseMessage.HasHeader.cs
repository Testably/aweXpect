using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> has the <paramref name="expected" /> header.
	/// </summary>
	public static HasHeaderValueResult<HttpResponseMessage, IThat<HttpResponseMessage?>> HasHeader(
		this IThat<HttpResponseMessage?> source,
		string expected)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasHeaderConstraint(expectationBuilder, it, expected)),
			source,
			a => a.Headers.TryGetValues(expected, out IEnumerable<string>? values) ? values.ToArray() : null);

	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> does not have the <paramref name="unexpected" /> header.
	/// </summary>
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> DoesNotHaveHeader(
		this IThat<HttpResponseMessage?> source,
		string unexpected)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new DoesNotHaveHeaderConstraint(expectationBuilder, it, unexpected)),
			source);

	private readonly struct HasHeaderConstraint(ExpectationBuilder expectationBuilder, string it, string expected)
		: IValueConstraint<HttpResponseMessage>
	{
		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>", FurtherProcessingStrategy.IgnoreResult);
			}

			if (actual.Headers.TryGetValues(expected, out _))
			{
				return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
				$"{it} did not contain the expected header", FurtherProcessingStrategy.IgnoreResult);
		}

		public override string ToString()
			=> $"has a `{expected}` header";
	}

	private readonly struct DoesNotHaveHeaderConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string unexpected)
		: IValueConstraint<HttpResponseMessage>
	{
		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>", FurtherProcessingStrategy.IgnoreResult);
			}

			if (!actual.Headers.TryGetValues(unexpected, out IEnumerable<string>? foundHeader))
			{
				return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
				$"{it} did contain the `{unexpected}` header: {Formatter.Format(foundHeader)}",
				FurtherProcessingStrategy.IgnoreResult);
		}

		public override string ToString()
			=> $"does not have a `{unexpected}` header";
	}
}
