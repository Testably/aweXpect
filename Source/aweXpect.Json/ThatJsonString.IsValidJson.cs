using System;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJson(
		this IThat<string?> subject,
		Func<JsonDocumentOptions, JsonDocumentOptions>? options = null)
		=> new(AddIsValidJsonConstraint(subject, GetDocumentOptions(options)), subject);

	/// <summary>
	///     Verifies that the subject is a valid JSON string whose root <see cref="JsonElement" /> satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJson(
		this IThat<string?> subject,
		Action<IThatSubject<JsonElement>> expectations,
		Func<JsonDocumentOptions, JsonDocumentOptions>? options = null)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonDocumentOptions documentOptions = GetDocumentOptions(options);
		return new AndOrResult<string, IThat<string?>>(AddIsValidJsonConstraint(subject, documentOptions)
				.ForMember(MemberAccessor<string, JsonElement>.FromFunc(jsonString =>
					{
						using JsonDocument jsonDocument = JsonDocument.Parse(jsonString, documentOptions);
						return jsonDocument.RootElement.Clone();
					}, "it"),
					(_, stringBuilder) => stringBuilder.Append(" which "))
				.AddExpectations(expectations,
					static (expectations, e) => expectations(new ThatSubject<JsonElement>(e))),
			subject);
	}

	private static JsonDocumentOptions GetDocumentOptions(Func<JsonDocumentOptions, JsonDocumentOptions>? options)
	{
		JsonDocumentOptions defaultOptions = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return defaultOptions;
	}

	private static ExpectationBuilder AddIsValidJsonConstraint(IThat<string?> subject, JsonDocumentOptions options)
		=> subject.Get().ExpectationBuilder.AddConstraint(options,
			static (o, it, grammar) => new IsValidJsonConstraint(it, grammar, o));

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
			=> stringBuilder.Append(Grammars.Verb("is valid JSON", "are valid JSON"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" could not be parsed: ").Append(_deserializationError);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not valid JSON", "are not valid JSON"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
