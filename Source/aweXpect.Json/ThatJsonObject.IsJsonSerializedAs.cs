using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonObject
{
	/// <summary>
	///     Verifies that the subject is serialized as the <paramref name="expected" /> JSON.
	/// </summary>
	/// <remarks>
	///     The subject is serialized with the customizable default <see cref="JsonSerializerOptions" />, and the result is
	///     compared exactly, unless the <paramref name="options" /> relax the comparison.
	/// </remarks>
	public static AndOrResult<T, IThat<T>> IsJsonSerializedAs<T>(
		this IThat<T> subject,
		string expected,
		Func<JsonOptions, JsonOptions>? options = null)
		=> IsJsonSerializedAsJson(subject, expected, options, null, null);

	/// <summary>
	///     Verifies that the subject is serialized with the <paramref name="serializerOptions" /> as the
	///     <paramref name="expected" /> JSON.
	/// </summary>
	/// <remarks>
	///     The result is compared exactly, unless the <paramref name="options" /> relax the comparison.
	/// </remarks>
	public static AndOrResult<T, IThat<T>> IsJsonSerializedAs<T>(
		this IThat<T> subject,
		string expected,
		JsonSerializerOptions serializerOptions,
		Func<JsonOptions, JsonOptions>? options = null)
		=> IsJsonSerializedAsJson(subject, expected, options, serializerOptions, null);

	/// <summary>
	///     Verifies that the subject is serialized with the <paramref name="typeInfo" /> as the
	///     <paramref name="expected" /> JSON.
	/// </summary>
	/// <remarks>
	///     Unlike the other overloads, it requires no reflection, so that it can be used with trimming and Native AOT.
	///     The result is compared exactly, unless the <paramref name="options" /> relax the comparison.
	/// </remarks>
	public static AndOrResult<T?, IThat<T?>> IsJsonSerializedAs<T>(
		this IThat<T?> subject,
		string expected,
		JsonTypeInfo<T> typeInfo,
		Func<JsonOptions, JsonOptions>? options = null)
		=> IsJsonSerializedAsJson(subject, expected, options, null, typeInfo);

	/// <summary>
	///     Verifies that the subject is serialized as JSON that matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The subject is serialized with the customizable default <see cref="JsonSerializerOptions" />, and the result is
	///     compared exactly, unless the <paramref name="options" /> relax the comparison.
	///     <para />
	///     The <paramref name="expected" /> value is serialized with the default options, not the customized ones, unless
	///     <c>o => o.WithSerializerOptions(…)</c> in the <paramref name="options" /> specifies others.
	/// </remarks>
	public static AndOrResult<T, IThat<T>> IsJsonSerializedAs<T>(
		this IThat<T> subject,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> IsJsonSerializedAsObject(subject, expected, doNotPopulateThisValue, options, null);

	/// <summary>
	///     Verifies that the subject is serialized with the <paramref name="serializerOptions" /> as JSON that matches the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The result is compared exactly, unless the <paramref name="options" /> relax the comparison.
	///     <para />
	///     The <paramref name="serializerOptions" /> only serialize the subject. The <paramref name="expected" /> value is
	///     serialized with the default options, unless <c>o => o.WithSerializerOptions(…)</c> in the
	///     <paramref name="options" /> specifies others.
	/// </remarks>
	public static AndOrResult<T, IThat<T>> IsJsonSerializedAs<T>(
		this IThat<T> subject,
		object? expected,
		JsonSerializerOptions serializerOptions,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> IsJsonSerializedAsObject(subject, expected, doNotPopulateThisValue, options, serializerOptions);

	/// <remarks>
	///     The expected JSON is shown as it is, like in <c>IsEqualTo(expected).AsJson()</c>, because it tells more than
	///     the expression of a variable.
	/// </remarks>
	private static AndOrResult<T, IThat<T>> IsJsonSerializedAsJson<T>(
		IThat<T> source,
		string expected,
		Func<JsonOptions, JsonOptions>? options,
		JsonSerializerOptions? serializerOptions,
		JsonTypeInfo? typeInfo)
	{
		JsonOptions jsonOptions = options?.Invoke(JsonOptions.Default) ?? JsonOptions.Default;
		return AddIsJsonSerializedAs(source,
			_ => JsonMatchType.ParseExpected(expected, nameof(expected), jsonOptions.DocumentOptions),
			expected, jsonOptions, serializerOptions, typeInfo);
	}

	private static AndOrResult<T, IThat<T>> IsJsonSerializedAsObject<T>(
		IThat<T> source,
		object? expected,
		string expectedExpression,
		Func<JsonOptions, JsonOptions>? options,
		JsonSerializerOptions? serializerOptions)
	{
		JsonOptions jsonOptions = options?.Invoke(JsonOptions.Default) ?? JsonOptions.Default;
		return AddIsJsonSerializedAs(source,
			converter => converter.ParseExpected(expected, jsonOptions.DocumentOptions, jsonOptions.SerializerOptions),
			expectedExpression.TrimCommonWhiteSpace(), jsonOptions, serializerOptions, null);
	}

	private static AndOrResult<T, IThat<T>> AddIsJsonSerializedAs<T>(
		IThat<T> source,
		Func<ExpectationJsonConverter, JsonDocument> parseExpected,
		string expectedText,
		JsonOptions options,
		JsonSerializerOptions? serializerOptions,
		JsonTypeInfo? typeInfo)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(
				(ParseExpected: parseExpected, ExpectedText: expectedText, Options: options,
					SerializerOptions: serializerOptions, TypeInfo: typeInfo),
				static (s, it, grammar) => new IsJsonSerializedAsConstraint<T>(it, grammar, s.ParseExpected,
					s.ExpectedText, s.Options, s.SerializerOptions, s.TypeInfo)),
			source);

	private sealed class IsJsonSerializedAsConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		Func<ExpectationJsonConverter, JsonDocument> parseExpected,
		string expectedText,
		JsonOptions options,
		JsonSerializerOptions? serializerOptions,
		JsonTypeInfo? typeInfo)
		: ConstraintResult.WithValue<T>(it, grammars),
			IAsyncContextConstraint<T>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;
		private string? _serializationError;
		/// <remarks>
		///     The subject is serialized to UTF-8, which the comparison parses directly, and only converted to a
		///     <see langword="string" /> for the context of a failure.
		/// </remarks>
		private byte[]? _serialized;

		public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_comparisonResult = null;
			_serializationError = null;
			_serialized = null;
			ExpectationJsonConverter converter = new(context, cancellationToken);
			using JsonDocument expectedDocument = parseExpected(converter);
			if (!TrySerialize(actual, out byte[] serialized))
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_serialized = serialized;
			using JsonDocument actualDocument = JsonDocument.Parse(serialized, options.DocumentOptions);
			_comparisonResult = await JsonElementValidator.Compare(
				actualDocument.RootElement,
				expectedDocument.RootElement,
				options,
				converter);

			Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
			return this;
		}

		/// <remarks>
		///     Any exception of the serializer, e.g. of a converter or a property getter, fails the expectation, except
		///     the error that reflection is switched off, which names the fix and must not be swallowed.
		/// </remarks>
		private bool TrySerialize(T actual, out byte[] serialized)
		{
			try
			{
				serialized = typeInfo is null
					? SerializeByReflection(actual)
					: JsonSerializer.SerializeToUtf8Bytes(actual, typeInfo);
				return true;
			}
			catch (Exception exception) when (typeInfo is not null || ReflectionFallback.IsSupported)
			{
				_serializationError = exception.Message;
				serialized = [];
				return false;
			}
		}

		private byte[] SerializeByReflection(T actual)
		{
			if (!ReflectionFallback.IsSupported)
			{
				throw Tracing.WriteException(ReflectionFallback.NotSupported(
					$"The members to serialize the subject of type {Formatter.Format(typeof(T))} as JSON"));
			}

			return JsonSerializer.SerializeToUtf8Bytes(actual,
				serializerOptions ?? Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get());
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_serialized is not null)
			{
				contexts.Add(new ResultContext.Fixed("Serialized", Encoding.UTF8.GetString(_serialized)));
			}

			options.AddContext(contexts, _comparisonResult?.GetUnmatchedMembersToIgnore());
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is serialized as JSON ", "are serialized as JSON ")).Append(expectedText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!TryAppendSerializationError(stringBuilder))
			{
				stringBuilder.Append(It).Append(" differed as")
					.Append(_comparisonResult?.ToString().Indent(indentation, false));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not serialized as JSON ", "are not serialized as JSON "))
				.Append(expectedText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!TryAppendSerializationError(stringBuilder))
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
			}
		}

		private bool TryAppendSerializationError(StringBuilder stringBuilder)
		{
			if (_serializationError is null)
			{
				return false;
			}

			stringBuilder.Append(It).Append(" could not be serialized: ").Append(_serializationError);
			return true;
		}
	}
}
