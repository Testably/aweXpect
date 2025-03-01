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
				=> new IsJsonSerializableConstraint<object>(it, new JsonSerializerOptions(),
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
				=> new IsJsonSerializableConstraint<object>(it, serializerOptions,
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
				=> new IsJsonSerializableConstraint<T>(it, new JsonSerializerOptions(),
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
				=> new IsJsonSerializableConstraint<T>(it, serializerOptions,
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

	private readonly struct IsJsonSerializableConstraint<T>(
		string it,
		JsonSerializerOptions serializerOptions,
		EquivalencyOptions options)
		: IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			if (actual is null)
			{
				return new ConstraintResult.Failure<T?>(default, ToString(), $"{it} was <null>");
			}

			if (actual is not T typedSubject)
			{
				return new ConstraintResult.Failure<T?>(default, ToString(),
					$"{it} was not assignable to {Formatter.Format(typeof(T))}");
			}

			object? deserializedObject;
			try
			{
				string serializedObject = JsonSerializer.Serialize(actual, serializerOptions);
				deserializedObject = JsonSerializer.Deserialize(serializedObject, actual.GetType(), serializerOptions);
			}
			catch (Exception e)
			{
				return new ConstraintResult.Failure<T?>(typedSubject, ToString(),
					$"{it} could not be deserialized: {e.Message}");
			}

			StringBuilder failureBuilder = new();
			if (EquivalencyComparison.Compare(deserializedObject, actual, options, failureBuilder))
			{
				return new ConstraintResult.Success<T?>(typedSubject, ToString());
			}

			failureBuilder.Insert(0, " was not:");
			failureBuilder.Insert(0, it);
			return new ConstraintResult.Failure<T?>(typedSubject, ToString(),
				failureBuilder.ToString());
		}

		public override string ToString()
			=> (typeof(T) == typeof(object)) switch
			{
				true => "is serializable as JSON",
				false => $"is serializable as {Formatter.Format(typeof(T))} JSON",
			};
	}
}
