using System;
using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
		HasRequestUri(this IThat<HttpRequestMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		if (!Uri.TryCreate(expected, UriKind.Absolute, out Uri? expectedUri))
		{
			throw Tracing.WriteException(new ArgumentException(
				"The 'expected' must be a valid absolute URI.", nameof(expected)));
		}

		return new(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(expectedUri.ToString(), static (expected, it, grammars) =>
					new HasRequestUriConstraint(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> subject, Uri expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(expected.ToString(), static (expected, it, grammars) =>
					new HasRequestUriConstraint(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveRequestUri(this IThat<HttpRequestMessage?> subject, string unexpected)
		=> subject.DoesNotHaveRequestUri(ThatUri.ToAbsoluteUri(unexpected, nameof(unexpected)));

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveRequestUri(this IThat<HttpRequestMessage?> subject, Uri unexpected)
	{
		ThatUri.ThrowIfNotAbsolute(unexpected, nameof(unexpected));
		return new(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(unexpected.ToString(), static (unexpected, it, grammars) =>
					new HasRequestUriConstraint(it, grammars, unexpected).Invert()),
			subject);
	}

	private sealed class HasRequestUriConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		private readonly StringEqualityOptions _options = new(nameof(expected));
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
			if (_requestUri?.Equals(expected, StringComparison.Ordinal) == true)
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
				.Append(_options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
