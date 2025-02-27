using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new HasHeaderConstraint(it, expected)),
			source,
			a => a.Headers.TryGetValues(expected, out IEnumerable<string>? values) ? values.ToArray() : null);

	/// <summary>
	///     Verifies that the <see cref="HttpResponseMessage" /> does not have the <paramref name="unexpected" /> header.
	/// </summary>
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> DoesNotHaveHeader(
		this IThat<HttpResponseMessage?> source,
		string unexpected)
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new DoesNotHaveHeaderConstraint(it, unexpected)),
			source);

	private readonly struct HasHeaderConstraint(string it, string expected)
		: IAsyncConstraint<HttpResponseMessage>
	{
		public async Task<ConstraintResult> IsMetBy(
			HttpResponseMessage? actual,
			CancellationToken cancellationToken)
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

			return await new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} did not contain the expected header", FurtherProcessingStrategy.IgnoreResult)
				.AddContext(actual, cancellationToken);
		}

		public override string ToString()
			=> $"has a `{expected}` header";
	}

	private readonly struct DoesNotHaveHeaderConstraint(string it, string unexpected)
		: IAsyncConstraint<HttpResponseMessage>
	{
		public async Task<ConstraintResult> IsMetBy(
			HttpResponseMessage? actual,
			CancellationToken cancellationToken)
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

			return await new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} did contain the `{unexpected}` header: {Formatter.Format(foundHeader)}",
					FurtherProcessingStrategy.IgnoreResult)
				.AddContext(actual, cancellationToken);
		}

		public override string ToString()
			=> $"does not have a `{unexpected}` header";
	}
}
