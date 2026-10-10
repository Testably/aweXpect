using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the string content is equal to <paramref name="expected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasContent(this IThat<HttpRequestMessage?> source, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasContentConstraint(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the string content satisfies the <paramref name="expectations" />
	/// </summary>
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasContent(this IThat<HttpRequestMessage?> source, Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.ForAsyncMember(MemberAccessor<HttpRequestMessage, Task<string?>>.FromFunc(
						async m => m.Content == null ? null : await m.Content.ReadAsStringAsync(),
						" the string content"),
					(_, stringBuilder) => stringBuilder.Append("has a string content which "))
				.AddExpectations(e => expectations(new ThatSubject<string?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);
	}

	private sealed class HasContentConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IAsyncConstraint<HttpRequestMessage>
	{
		private string? _message;

		public async ValueTask<ConstraintResult> IsMetBy(
			HttpRequestMessage? actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual.Content is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

#if NETSTANDARD2_0
			_message = await actual.Content.ReadAsStringAsync();
#else
			_message = await actual.Content.ReadAsStringAsync(cancellationToken);
#endif
			if (await options.AreConsideredEqual(_message, expected))
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a string content ", "have a string content "))
				.Append(options.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Content is null)
			{
				stringBuilder.Append(It).Append(" had a <null> content");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedMemberFailure(It, "string content", Grammars, _message, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a string content ", "do not have a string content "))
				.Append(options.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
