using System;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Options;
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
	/// <remarks>
	///     Only the document options of the <paramref name="options" /> apply, set with
	///     <see cref="JsonOptionsExtensions.WithDocumentOptions" />. The comparison options are ignored, as there is no
	///     expected JSON to compare with.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJson(
		this IThat<string?> subject,
		Func<JsonOptions, JsonOptions>? options = null)
		=> new(AddIsValidJsonConstraint(subject, GetDocumentOptions(options)), subject);

	/// <summary>
	///     Verifies that the subject is a valid JSON string whose root <see cref="JsonElement" /> satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     A subject that is no valid JSON fails also when negated, as the <paramref name="expectations" /> cannot be
	///     verified.
	///     <para />
	///     Only the document options of the <paramref name="options" /> apply, set with
	///     <see cref="JsonOptionsExtensions.WithDocumentOptions" />. The comparison options are ignored, as there is no
	///     expected JSON to compare with.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJson(
		this IThat<string?> subject,
		Action<IThatSubject<JsonElement>> expectations,
		Func<JsonOptions, JsonOptions>? options = null)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<string, IThat<string?>>(subject.Get().ExpectationBuilder.AddConstraint(
					GetDocumentOptions(options),
					static (o, it, grammar) => new IsValidJsonConstraint(it, grammar, o, true))
				.ForMember(MemberAccessor<JsonElement, JsonElement>.FromFuncWithName(rootElement => rootElement, "it"),
					(_, stringBuilder) => stringBuilder.Append(" which "))
				.AddExpectations(expectations),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not a valid JSON string.
	/// </summary>
	/// <remarks>
	///     Only the document options of the <paramref name="options" /> apply, set with
	///     <see cref="JsonOptionsExtensions.WithDocumentOptions" />. The comparison options are ignored, as there is no
	///     expected JSON to compare with.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsNotValidJson(
		this IThat<string?> subject,
		Func<JsonOptions, JsonOptions>? options = null)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(GetDocumentOptions(options),
				static (o, it, grammar) => new IsValidJsonConstraint(it, grammar, o).Invert()),
			subject);

	private static JsonDocumentOptions GetDocumentOptions(Func<JsonOptions, JsonOptions>? options)
		=> (options?.Invoke(JsonOptions.Default) ?? JsonOptions.Default).DocumentOptions;

	private static ExpectationBuilder AddIsValidJsonConstraint(IThat<string?> subject, JsonDocumentOptions options)
		=> subject.Get().ExpectationBuilder.AddConstraint(options,
			static (o, it, grammar) => new IsValidJsonConstraint(it, grammar, o));

	private sealed class IsValidJsonConstraint(
		string it,
		ExpectationGrammars grammars,
		JsonDocumentOptions options,
		bool hasExpectations = false)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		private string? _deserializationError;
		private JsonElement? _rootElement;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			_deserializationError = null;
			_rootElement = null;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(actual, options);
				if (hasExpectations)
				{
					_rootElement = jsonDocument.RootElement.Clone();
				}
			}
			catch (JsonException jsonException)
			{
				_deserializationError = jsonException.Message;
				// The expectations on the root element cannot be verified, so their negation cannot be met either.
				Outcome = hasExpectations ? Outcome.FailureBothWays : Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		/// <remarks>
		///     Stores the root element of the parsed subject, so that the expectations on it do not parse the subject again.
		/// </remarks>
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_rootElement is TValue rootElement)
			{
				value = rootElement;
				return true;
			}

			return base.TryGetStoredValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is valid JSON", "are valid JSON"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" could not be parsed: ").Append(_deserializationError);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not valid JSON", "are not valid JSON"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_deserializationError is not null)
			{
				AppendNormalResult(stringBuilder, indentation);
				return;
			}

			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
