using System;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatJsonString
{
	/// <summary>
	///     Verifies that the subject is a valid JSON string.
	/// </summary>
	public static JsonWhichResult IsValidJson(
		this IThat<string?> source,
		Func<JsonDocumentOptions, JsonDocumentOptions>? options = null)
	{
		JsonDocumentOptions defaultOptions = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new JsonWhichResult(source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsValidJsonConstraint(it, grammar, defaultOptions)),
			source, defaultOptions);
	}

	private sealed class IsValidJsonConstraint(string it, ExpectationGrammars grammars, JsonDocumentOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		private string? _deserializationError;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(actual, options);
			}
			catch (JsonException jsonException)
			{
				_deserializationError = jsonException.Message;
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is valid JSON");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" could not be parsed: ").Append(_deserializationError);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is no valid JSON");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
