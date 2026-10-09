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
		StringEqualityOptions typeOptions = new(nameof(type));
		ProblemDetailsOptions options = new();
		return new ProblemDetailsResult<HttpResponseMessage, IThat<HttpResponseMessage?>>.String(
			source.Get().ExpectationBuilder
				.AddSubjectContexts(ThatExtensions.ResponseContexts)
				.AddConstraint((it, grammars) =>
					new HasProblemDetailsConstraint(it, grammars, type, options, typeOptions)),
			source,
			typeOptions,
			options);
	}

	private sealed class HasProblemDetailsConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expectedType,
		ProblemDetailsOptions options,
		StringEqualityOptions typeOptions)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IAsyncConstraint<HttpResponseMessage>
	{
		private readonly List<string> _failures = [];
		private string? _parseError;

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
			string message = await actual.Content.ReadAsStringAsync();
#else
			string message = await actual.Content.ReadAsStringAsync(cancellationToken);
#endif
			using JsonDocument? problemDetails = ParseObjectOrDefault(message, out _parseError);
			if (problemDetails is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

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
				_failures.Add(typeOptions.GetExtendedMemberFailure(It, "type", Grammars, type, expectedType));
			}

			if (options.Status != null && status != options.Status)
			{
				_failures.Add($"{It} had status {Formatter.Format(status)}");
			}

			if (!await options.IsTitleConsideredEqualTo(title))
			{
				_failures.Add(options.GetTitleFailure(It, Grammars, title));
			}

			if (!await options.IsDetailConsideredEqualTo(detail))
			{
				_failures.Add(options.GetDetailFailure(It, Grammars, detail));
			}

			if (!await options.IsInstanceConsideredEqualTo(instance))
			{
				_failures.Add(options.GetInstanceFailure(It, Grammars, instance));
			}

			if (_failures.Any())
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private static JsonDocument? ParseObjectOrDefault(string content, out string? error)
		{
			JsonDocument document;
			try
			{
				document = JsonDocument.Parse(content, _jsonDocumentOptions);
			}
			catch (JsonException e)
			{
				error = e.Message;
				return null;
			}

			if (document.RootElement.ValueKind != JsonValueKind.Object)
			{
				document.Dispose();
				error = "the JSON value was not an object";
				return null;
			}

			error = null;
			return document;
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
		{
			if (_parseError is not null)
			{
				AppendParseError(stringBuilder);
				return;
			}

			stringBuilder.Append(string.Join($"{Environment.NewLine} and ", _failures));
		}

		private void AppendParseError(StringBuilder stringBuilder)
			=> stringBuilder.Append(It).Append(" could not be parsed as problem details: ").Append(_parseError);

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
		{
			if (_parseError is not null)
			{
				AppendParseError(stringBuilder);
				return;
			}

			stringBuilder.Append(It).Append(" had");
		}
	}
}
