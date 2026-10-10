using System;
using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> source, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		if (!Uri.TryCreate(expected, UriKind.Absolute, out Uri? expectedUri))
		{
			throw Tracing.WriteException(new ArgumentException(
				"The 'expected' must be a valid absolute URI.", nameof(expected)));
		}

		return new(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasRequestUriConstraint(it, grammars, expectedUri.ToString())),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> source, Uri expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.RequestContexts)
				.AddConstraint((it, grammars) =>
					new HasRequestUriConstraint(it, grammars, expected.ToString())),
			source);
	}

	private sealed class HasRequestUriConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		private readonly StringEqualityOptions _options = new StringEqualityOptions(nameof(expected)).IgnoringCase();
		private string? _requestUri;

		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_requestUri = actual.RequestUri?.ToString();
			if (_requestUri?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a request URI ", "have a request URI "))
				.Append(_options.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_options.GetExtendedMemberFailure(It, "request URI", Grammars, _requestUri, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a request URI ", "do not have a request URI "))
				.Append(_options.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
