using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
			source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammar) =>
					new HasProblemDetailsConstraint(expectationBuilder, it, type, options, typeOptions)),
			source,
			typeOptions,
			options);
	}

	private readonly struct HasProblemDetailsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string? expectedType,
		ProblemDetailsOptions options,
		StringEqualityOptions typeOptions)
		: IAsyncConstraint<HttpResponseMessage>
	{
		private static JsonElement? GetPropertyOrDefault(JsonElement jsonElement, string propertyName)
		{
			if (jsonElement.TryGetProperty(propertyName, out JsonElement element))
			{
				return element;
			}

			return null;
		}

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
			using JsonDocument problemDetails = JsonDocument.Parse(message, _jsonDocumentOptions);
			List<string> failures = new();

			string? type = GetPropertyOrDefault(problemDetails.RootElement, "type")?.GetString();
			int? status = GetPropertyOrDefault(problemDetails.RootElement, "status")?.GetInt32();
			string? title = GetPropertyOrDefault(problemDetails.RootElement, "title")?.GetString();
			string? instance = GetPropertyOrDefault(problemDetails.RootElement, "instance")?.GetString();
			string? detail = GetPropertyOrDefault(problemDetails.RootElement, "detail")?.GetString();

			if (type == null)
			{
				failures.Add($"{it} did not match the expected format because no 'type' property existed");
			}
			else if (expectedType != null && !typeOptions.AreConsideredEqual(type, expectedType))
			{
				failures.Add(
					$"{it} was type {Formatter.Format(type)} which {new StringDifference(type, expectedType)}");
			}

			if (options.Status != null && status != options.Status)
			{
				failures.Add($"{it} had status {Formatter.Format(status)}");
			}

			if (!options.IsTitleConsideredEqualTo(title))
			{
				failures.Add(
					$"{it} had title {Formatter.Format(title)} which {new StringDifference(title, options.Title)}");
			}

			if (!options.IsDetailConsideredEqualTo(detail))
			{
				failures.Add(
					$"{it} had detail {Formatter.Format(detail)} which {new StringDifference(detail, options.Detail)}");
			}

			if (!options.IsInstanceConsideredEqualTo(instance))
			{
				failures.Add(
					$"{it} had instance {Formatter.Format(instance)} which {new StringDifference(instance, options.Instance)}");
			}

			if (failures.Any())
			{
				expectationBuilder.AddContext(actual);
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					string.Join($"{Environment.NewLine} and ", failures));
			}

			return new ConstraintResult.Success<HttpResponseMessage?>(actual, ToString());
		}

		public override string ToString()
			=> expectedType switch
			{
				null => $"has a ProblemDetails content with any type{options}",
				_ => $"has a ProblemDetails content with type {Formatter.Format(expectedType)}{typeOptions}{options}",
			};
	}
}
