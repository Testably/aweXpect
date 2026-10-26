using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="object" /> values.
/// </summary>
public static partial class ThatJsonObject
{
	/// <summary>
	///     Verifies that the subject can be serialized as JSON.
	/// </summary>
	/// <remarks>
	///     The subject is serialized as its runtime type with the customizable default <see cref="JsonSerializerOptions" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsJsonSerializable<[RequiresMemberMetadata] T>(
		this IThat<T?> subject,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> AddIsJsonSerializable(subject, null, null, equivalencyOptions);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON with the <paramref name="serializerOptions" />.
	/// </summary>
	/// <remarks>
	///     The subject is serialized as its runtime type.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsJsonSerializable<[RequiresMemberMetadata] T>(
		this IThat<T?> subject,
		JsonSerializerOptions serializerOptions,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> AddIsJsonSerializable(subject, serializerOptions, null, equivalencyOptions);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON with the <paramref name="typeInfo" />.
	/// </summary>
	/// <remarks>
	///     Unlike the other overloads, it requires no reflection to serialize the subject, so that it can be used with
	///     trimming and Native AOT.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsJsonSerializable<[RequiresMemberMetadata] T>(
		this IThat<T?> subject,
		JsonTypeInfo<T> typeInfo,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> AddIsJsonSerializable(subject, null, typeInfo, equivalencyOptions);

	private static AndOrResult<T, IThat<T?>> AddIsJsonSerializable<T>(
		IThat<T?> subject,
		JsonSerializerOptions? serializerOptions,
		JsonTypeInfo<T>? typeInfo,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(
				(SerializerOptions: serializerOptions, TypeInfo: typeInfo, EquivalencyOptions: equivalencyOptions),
				static (s, it, grammar) => new IsJsonSerializableConstraint<T>(it, grammar, s.SerializerOptions,
					s.TypeInfo, FromCallback(s.EquivalencyOptions))),
			subject);

	/// <summary>
	///     Creates a new <see cref="EquivalencyOptions" /> instance from the provided <paramref name="callback" />.
	/// </summary>
	/// <remarks>
	///     Uses the default instance, when no <paramref name="callback" /> is given.
	/// </remarks>
	private static EquivalencyOptions FromCallback(Func<EquivalencyOptions, EquivalencyOptions>? callback)
		=> callback is null
			? Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get()
			: callback(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get());

	/// <remarks>
	///     Compares with the matcher of an <see cref="EquivalencyMatchType{TElement}" />
	///     like <c>IsEquivalentTo</c>, so that the comparison is part of the evaluation and the failure lists the
	///     equivalency options with a hint for every member to ignore that matched no member.
	/// </remarks>
	private sealed class IsJsonSerializableConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		JsonSerializerOptions? serializerOptions,
		JsonTypeInfo<T>? typeInfo,
		EquivalencyOptions options)
		: ConstraintResult.WithNotNullValue<T>(it, grammars),
			IAsyncContextConstraint<T>
	{
		private readonly EquivalencyMatchType<object?> _matchType = new(options);
		private string? _deserializationError;
		private object? _deserializedObject;
		private ObjectMatcher<object?>? _matcher;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			object? deserializedObject;
			try
			{
				deserializedObject = typeInfo is null
					? RoundTripByReflection(actual)
					: JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(actual, typeInfo), typeInfo);
			}
			catch (Exception e) when (typeInfo is not null || ReflectionFallback.IsSupported)
			{
				_deserializationError = e.Message;
				Outcome = Outcome.Failure;
				return this;
			}

			_deserializedObject = deserializedObject;
			_matcher = _matchType.ForEvaluation(new MatchEnvironment(context, cancellationToken));
			_matchResult = await _matcher.CompareWithExplanation(deserializedObject, actual);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     Uses the runtime type instead of <typeparamref name="T" />, so that the members of a derived type are
		///     serialized and deserialized as well.
		/// </remarks>
		private object? RoundTripByReflection(object actual)
		{
			Type type = actual.GetType();
			if (!ReflectionFallback.IsSupported)
			{
				throw Tracing.WriteException(ReflectionFallback.NotSupported(
					$"The members to serialize the subject of type {Formatter.Format(type)} as JSON"));
			}

			JsonSerializerOptions jsonSerializerOptions =
				serializerOptions ?? Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
			string serializedObject = JsonSerializer.Serialize(actual, type, jsonSerializerOptions);
			return JsonSerializer.Deserialize(serializedObject, type, jsonSerializerOptions);
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> _matcher?.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is serializable as JSON", "are serializable as JSON"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_deserializationError is not null)
			{
				stringBuilder.Append(It).Append(" could not be deserialized: ").Append(_deserializationError);
			}
			else if (_matcher is not null)
			{
				stringBuilder.Append(_matcher.GetFailure(It, Grammars, _deserializedObject, Actual,
					_matchResult, indentation));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not serializable as JSON", "are not serializable as JSON"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_matcher is not null)
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
			}
		}
	}
}
