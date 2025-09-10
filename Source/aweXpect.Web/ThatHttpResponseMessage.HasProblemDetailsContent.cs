using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Web.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	private static readonly JsonDocumentOptions _jsonDocumentOptions = new()
	{
		AllowTrailingCommas = true,
	};

	/// <summary>
	///     Verifies that the string content contains a problem details response with the expected <paramref name="type" />.
	///     <seealso href="https://datatracker.ietf.org/doc/html/rfc7807" />
	///     <seealso href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails" />
	/// </summary>
	/// <remarks>
	///     Type:
	///     A URI reference [<see href="https://datatracker.ietf.org/doc/html/rfc3986" />] that identifies the problem type.
	///     This specification encourages that, when dereferenced, it provide human-readable documentation for
	///     the problem type (e.g., using HTML [
	///     <see href="https://datatracker.ietf.org/doc/html/rfc7807#ref-W3C.REC-html5-20141028" />]).
	///     When this member is not present, its value is assumed to be "about:blank".
	/// </remarks>
	public static ProblemDetailsResult<HttpResponseMessage, IThat<HttpResponseMessage?>>.String
		HasProblemDetailsContent(this IThat<HttpResponseMessage?> source, string? type = null)
	{
		StringEqualityOptions typeOptions = new();
		ProblemDetailsOptions options = new();
		return new ProblemDetailsResult<HttpResponseMessage, IThat<HttpResponseMessage?>>.String(
			source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new HasProblemDetailsConstraint(expectationBuilder, it, grammars, type, options, typeOptions)),
			source,
			typeOptions,
			options);
	}

	private sealed class HasProblemDetailsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedType,
		ProblemDetailsOptions options,
		StringEqualityOptions typeOptions)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IAsyncConstraint<HttpResponseMessage>
	{
		private readonly List<string> _failures = [];

		public async Task<ConstraintResult> IsMetBy(
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
			string message = await actual.Content.ReadAsStringAsync();
#else
			string message = await actual.Content.ReadAsStringAsync(cancellationToken);
#endif
			using JsonDocument problemDetails = JsonDocument.Parse(message, _jsonDocumentOptions);
			_failures.Clear();

			string? type = GetPropertyOrDefault(problemDetails.RootElement, "type")?.GetString();
			int? status = GetPropertyOrDefault(problemDetails.RootElement, "status")?.GetInt32();
			string? title = GetPropertyOrDefault(problemDetails.RootElement, "title")?.GetString();
			string? instance = GetPropertyOrDefault(problemDetails.RootElement, "instance")?.GetString();
			string? detail = GetPropertyOrDefault(problemDetails.RootElement, "detail")?.GetString();

			if (type == null)
			{
				_failures.Add($"{It} did not match the expected format because no 'type' property existed");
			}
			else if (expectedType != null && !await typeOptions.AreConsideredEqual(type, expectedType))
			{
				_failures.Add(
					$"{It} was type {Formatter.Format(type)} which {new StringDifference(type, expectedType)}");
			}

			if (options.Status != null && status != options.Status)
			{
				_failures.Add($"{It} had status {Formatter.Format(status)}");
			}

			if (!await options.IsTitleConsideredEqualTo(title))
			{
				_failures.Add(
					$"{It} had title {Formatter.Format(title)} which {new StringDifference(title, options.Title)}");
			}

			if (!await options.IsDetailConsideredEqualTo(detail))
			{
				_failures.Add(
					$"{It} had detail {Formatter.Format(detail)} which {new StringDifference(detail, options.Detail)}");
			}

			if (!await options.IsInstanceConsideredEqualTo(instance))
			{
				_failures.Add(
					$"{It} had instance {Formatter.Format(instance)} which {new StringDifference(instance, options.Instance)}");
			}

			if (_failures.Any())
			{
				expectationBuilder.AddContext(actual);
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private static JsonElement? GetPropertyOrDefault(JsonElement jsonElement, string propertyName)
		{
			if (jsonElement.TryGetProperty(propertyName, out JsonElement element))
			{
				return element;
			}

			return null;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expectedType is null)
			{
				stringBuilder.Append("has a ProblemDetails content with any type");
			}
			else
			{
				stringBuilder.Append("has a ProblemDetails content with type ");
				Formatter.Format(stringBuilder, expectedType);
				stringBuilder.Append(typeOptions);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(string.Join($"{Environment.NewLine} and ", _failures));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expectedType is null)
			{
				stringBuilder.Append("does not have a ProblemDetails content");
			}
			else
			{
				stringBuilder.Append("does not have a ProblemDetails content with type ");
				Formatter.Format(stringBuilder, expectedType);
				stringBuilder.Append(typeOptions);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
