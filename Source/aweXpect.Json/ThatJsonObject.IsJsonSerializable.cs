using System;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="object" /> values.
/// </summary>
public static class ThatJsonObject
{
	/// <summary>
	///     Verifies that the subject can be serialized as JSON.
	/// </summary>
	public static AndOrResult<object?, IThat<object?>> IsJsonSerializable(
		this IThat<object?> source,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar)
				=> new IsJsonSerializableConstraint<object>(it, grammar, new JsonSerializerOptions(),
					FromCallback(equivalencyOptions))),
			source);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON.
	/// </summary>
	public static AndOrResult<object?, IThat<object?>> IsJsonSerializable(
		this IThat<object?> source,
		JsonSerializerOptions serializerOptions,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar)
				=> new IsJsonSerializableConstraint<object>(it, grammar, serializerOptions,
					FromCallback(equivalencyOptions))),
			source);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON of type <typeparamref name="T" />.
	/// </summary>
	public static AndOrResult<object?, IThat<object?>> IsJsonSerializable<T>(
		this IThat<object?> source,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar)
				=> new IsJsonSerializableConstraint<T>(it, grammar, new JsonSerializerOptions(),
					FromCallback(equivalencyOptions))),
			source);

	/// <summary>
	///     Verifies that the subject can be serialized as JSON of type <typeparamref name="T" />.
	/// </summary>
	public static AndOrResult<object?, IThat<object?>> IsJsonSerializable<T>(
		this IThat<object?> source,
		JsonSerializerOptions serializerOptions,
		Func<EquivalencyOptions, EquivalencyOptions>? equivalencyOptions = null)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar)
				=> new IsJsonSerializableConstraint<T>(it, grammar, serializerOptions,
					FromCallback(equivalencyOptions))),
			source);

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

	private sealed class IsJsonSerializableConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		JsonSerializerOptions serializerOptions,
		EquivalencyOptions options)
		: ConstraintResult.WithValue<T?>(grammars),
			IValueConstraint<object?>
	{
		private object? _actual;
		private string? _deserializationError;
		private StringBuilder? _failureBuilder;

		public ConstraintResult IsMetBy(object? actual)
		{
			_actual = actual;
			if (actual is not T typedSubject)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Actual = typedSubject;

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

			_failureBuilder = new StringBuilder();
			if (EquivalencyComparison.Compare(deserializedObject, actual, options, _failureBuilder))
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is serializable as ");
			if (typeof(T) != typeof(object))
			{
				Formatter.Format(stringBuilder, typeof(T));
				stringBuilder.Append(' ');
			}

			stringBuilder.Append("JSON");
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else if (_actual is not T)
			{
				stringBuilder.Append(it).Append(" was not assignable to ");
				Formatter.Format(stringBuilder, typeof(T));
			}
			else if (_deserializationError is not null)
			{
				stringBuilder.Append(it).Append(" could not be deserialized: ").Append(_deserializationError);
			}
			else if (_failureBuilder is not null)
			{
				stringBuilder.Append(it).Append(" was not:").Append(_failureBuilder);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotImplementedException();
	}
}
