using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Helpers;
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
	[GuaranteesNotNull]
	public static AndOrResult<object, IThat<object?>> IsJsonSerializable(
		this IThat<object?> subject,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(equivalencyOptions, static (callback, it, grammar)
				=> new IsJsonSerializableConstraint<object>(it, grammar, new JsonSerializerOptions(),
					FromCallback(callback))),
			subject);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<object, IThat<object?>> IsJsonSerializable(
		this IThat<object?> subject,
		JsonSerializerOptions serializerOptions,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(
				(SerializerOptions: serializerOptions, EquivalencyOptions: equivalencyOptions),
				static (s, it, grammar) => new IsJsonSerializableConstraint<object>(it, grammar, s.SerializerOptions,
					FromCallback(s.EquivalencyOptions))),
			subject);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON of type <typeparamref name="T" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<object, IThat<object?>> IsJsonSerializable<T>(
		this IThat<object?> subject,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(equivalencyOptions, static (callback, it, grammar)
				=> new IsJsonSerializableConstraint<T>(it, grammar, new JsonSerializerOptions(),
					FromCallback(callback))),
			subject);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON of type <typeparamref name="T" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<object, IThat<object?>> IsJsonSerializable<T>(
		this IThat<object?> subject,
		JsonSerializerOptions serializerOptions,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(
				(SerializerOptions: serializerOptions, EquivalencyOptions: equivalencyOptions),
				static (s, it, grammar) => new IsJsonSerializableConstraint<T>(it, grammar, s.SerializerOptions,
					FromCallback(s.EquivalencyOptions))),
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
	///     Compares through <see cref="ObjectEqualityOptions{TSubject}" /> with an <see cref="EquivalencyMatchType" />
	///     like <c>IsEquivalentTo</c>, so that the comparison is part of the evaluation and the failure lists the
	///     equivalency options with a hint for every member to ignore that matched no member.
	/// </remarks>
	private sealed class IsJsonSerializableConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		JsonSerializerOptions serializerOptions,
		EquivalencyOptions options)
		: ConstraintResult.WithNotNullValue<object?>(it, grammars),
			IAsyncContextConstraint<object?>
	{
		private readonly ObjectEqualityOptions<object?> _equalityOptions = CreateEqualityOptions(options);
		private string? _deserializationError;
		private object? _deserializedObject;
		private ObjectEqualityOptions<object?>? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(object? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is not T)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (!ReflectionFallback.IsSupported)
			{
				throw Tracing.WriteException(ReflectionFallbackHelpers.SerializationNotSupported(
					$"The subject of type {Formatter.Format(actual.GetType())}"));
			}

			object? deserializedObject;
			try
			{
				string serializedObject = JsonSerializer.Serialize(actual, serializerOptions);
				deserializedObject = JsonSerializer.Deserialize(serializedObject, actual.GetType(), serializerOptions);
			}
			catch (Exception e)
			{
				_deserializationError = e.Message;
				Outcome = Outcome.Failure;
				return this;
			}

			_deserializedObject = deserializedObject;
			_evaluationOptions = _equalityOptions.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(deserializedObject, actual);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		private static ObjectEqualityOptions<object?> CreateEqualityOptions(EquivalencyOptions options)
			=> new ObjectEqualityOptions<object?>().SetMatchType(new EquivalencyMatchType(options), "Equivalent");

		public override void AppendContexts(ResultContextCollector contexts)
			=> _evaluationOptions?.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is serializable as ", "are serializable as "));
			if (typeof(T) != typeof(object))
			{
				Formatter.Format(stringBuilder, typeof(T));
				stringBuilder.Append(' ');
			}

			stringBuilder.Append("JSON");
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is not T)
			{
				stringBuilder.Append(It)
					.Append(Grammars.SubjectVerb(It, " was not assignable to ", " were not assignable to "));
				Formatter.Format(stringBuilder, typeof(T));
			}
			else if (_deserializationError is not null)
			{
				stringBuilder.Append(It).Append(" could not be deserialized: ").Append(_deserializationError);
			}
			else if (_evaluationOptions is not null)
			{
				stringBuilder.Append(_evaluationOptions.GetExtendedFailure(It, Grammars, _deserializedObject, Actual,
					_matchResult, indentation));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not serializable as ", "are not serializable as "));
			if (typeof(T) != typeof(object))
			{
				Formatter.Format(stringBuilder, typeof(T));
				stringBuilder.Append(' ');
			}

			stringBuilder.Append("JSON");
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_evaluationOptions is not null)
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (Actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(T));
		}
	}
}
