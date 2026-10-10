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
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" /> method.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasMethod(this IThat<HttpRequestMessage?> source, HttpMethod expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasMethodConstraint(it, grammars, expected)),
			source);
	}

	private sealed class HasMethodConstraint(
		string it,
		ExpectationGrammars grammars,
		HttpMethod expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual.Method != expected)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private string Article => char.ToUpperInvariant(expected.Method[0]) is 'A' or 'E' or 'I' or 'O' or 'U' ? "an" : "a";

		public override string ToString()
			=> $"has {Article} {expected} method";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(Article).Append(' ').Append(expected).Append(" method");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual?.Method);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(Article).Append(' ').Append(expected).Append(" method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
