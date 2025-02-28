using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the string content is equal to <paramref name="expected" />
	/// </summary>
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> source, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasContentConstraint(expectationBuilder, it, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the string content satisfies the <paramref name="expectations" />
	/// </summary>
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> source, Action<IThat<string?>> expectations)
	{
		ExpectationBuilder expectationBuilder = source.ThatIs().ExpectationBuilder;
		return new AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			expectationBuilder
				.UpdateContexts(c => c.Close())
				.ForAsyncMember(MemberAccessor<HttpResponseMessage, Task<string?>>.FromFunc(
						async m =>
						{
							expectationBuilder.AddContext(m);
							return await m.Content.ReadAsStringAsync();
						},
						" the string content"),
					(member, stringBuilder) => stringBuilder.Append("has a string content which "))
				.AddExpectations(e => expectations(new ThatSubject<string?>(e)), ExpectationGrammars.Nested),
			source);
	}

	private readonly struct HasContentConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string expected,
		StringEqualityOptions options)
		: IAsyncConstraint<HttpResponseMessage>
	{
		public async Task<ConstraintResult> IsMetBy(
			HttpResponseMessage? actual,
			CancellationToken cancellationToken)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

#if NETSTANDARD2_0
			string message = await actual.Content.ReadAsStringAsync();
#else
			string message = await actual.Content.ReadAsStringAsync(cancellationToken);
#endif
			if (options.AreConsideredEqual(message, expected))
			{
				return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
			}

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
				options.GetExtendedFailure(it, message, expected));
		}

		public override string ToString()
			=> $"has a string content {options.GetExpectation(expected, ExpectationGrammars.None)}";
	}
}
