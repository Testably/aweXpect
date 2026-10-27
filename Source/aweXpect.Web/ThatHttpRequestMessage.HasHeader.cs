using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the header with the given <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The headers of the content are included, e.g. <c>Content-Type</c> or <c>Content-Length</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasHeaderValueResult<HttpRequestMessage, IThat<HttpRequestMessage?>> HasHeader(
		this IThat<HttpRequestMessage?> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new(subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(name, static (name, it, grammars) =>
					new HasHeaderConstraint(it, grammars, name)),
			subject,
			a => a.Headers.GetHeaderValues(a.Content, name));
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the header with the given
	///     <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The headers of the content are included, e.g. <c>Content-Type</c> or <c>Content-Length</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>> DoesNotHaveHeader(
		this IThat<HttpRequestMessage?> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new(subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(name, static (name, it, grammars) =>
					new HasHeaderConstraint(it, grammars, name).Invert()),
			subject);
	}

	private sealed class HasHeaderConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IValueConstraint<HttpRequestMessage>
	{
		private string[]? _foundHeader;

		public ConstraintResult IsMetBy(HttpRequestMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
				Outcome = Outcome.Failure;
				return this;
			}

			_foundHeader = actual.Headers.GetHeaderValues(actual.Content, expected);
			if (_foundHeader != null)
			{
				Outcome = Outcome.Success;
				return this;
			}

			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(expected.IndefiniteArticleForHeader()).Append(' ');
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header");
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not contain the expected header");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "))
				.Append(expected.IndefiniteArticleForHeader()).Append(' ');
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header");
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain the ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" header: ");
			Formatter.Format(stringBuilder, _foundHeader);
		}
	}
}
