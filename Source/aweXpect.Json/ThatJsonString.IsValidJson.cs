#if NET8_0_OR_GREATER
using System;
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
public static class ThatJsonString
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
				new IsValidJsonConstraint(it, defaultOptions)),
			source, defaultOptions);
	}

	private readonly struct IsValidJsonConstraint(string it, JsonDocumentOptions options) : IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			if (actual is null)
			{
				return new ConstraintResult.Failure<string?>(null, ToString(), $"{it} was <null>");
			}

			try
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(actual, options);
			}
			catch (JsonException jsonException)
			{
				return new ConstraintResult.Failure<string?>(actual, ToString(),
					$"{it} could not be parsed: {jsonException.Message}");
			}

			return new ConstraintResult.Success<string?>(actual, ToString());
		}

		public override string ToString()
			=> "is valid JSON";
	}
}
#endif
