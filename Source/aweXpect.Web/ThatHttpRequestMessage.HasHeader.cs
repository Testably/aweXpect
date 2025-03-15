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
	public static HasHeaderValueResult<HttpRequestMessage, IThat<HttpRequestMessage?>> HasHeader(
		this IThat<HttpRequestMessage?> source,
		string expected)
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new HasHeaderConstraint(expectationBuilder, it, grammars, expected)),
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
				.AddConstraint((expectationBuilder, it, grammars) =>
					new HasHeaderConstraint(expectationBuilder, it, grammars, unexpected).Invert()),
			source);

	private sealed class HasHeaderConstraint(
		ExpectationBuilder expectationBuilder,
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

			expectationBuilder.AddContext(actual);
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
			=> stringBuilder.Append("has a `").Append(expected).Append("` header");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not contain the expected header");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a `").Append(expected).Append("` header");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain the `").Append(expected).Append("` header: ");
			Formatter.Format(stringBuilder, _foundHeader);
		}
	}
}
