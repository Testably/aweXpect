using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the string content is equal to <paramref name="expected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> source, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((expected, options), static (state, it, grammars) =>
					new HasContentConstraint(it, grammars, state.expected, state.options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the string content satisfies the <paramref name="expectations" />
	/// </summary>
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> source, Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.ForAsyncMember(MemberAccessor<HttpResponseMessage, Task<string?>>.FromFunc(
						async m => await m.Content.ReadAsStringAsync(),
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
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IAsyncConstraint<HttpResponseMessage>
	{
		private string? _message;

		public async ValueTask<ConstraintResult> IsMetBy(
			HttpResponseMessage? actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
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
