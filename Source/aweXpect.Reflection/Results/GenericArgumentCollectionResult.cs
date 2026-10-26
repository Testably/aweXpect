using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public sealed class GenericArgumentCollectionResult<TType, TValue>
	: GenericArgumentCollectionResult<TType, TValue, GenericArgumentCollectionResult<TType, TValue>>
{
	internal GenericArgumentCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<GenericArgumentsFilterOptions> genericArgumentsFilterOptions)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions)
	{
	}
}

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class GenericArgumentCollectionResult<TType, TValue, TSelf>
	: AndOrResult<TType, IThat<TValue>, TSelf>
	where TSelf : GenericArgumentCollectionResult<TType, TValue, TSelf>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly OptionsHolder<GenericArgumentsFilterOptions> _genericArgumentsFilterOptions;
	private readonly IThat<TValue> _subject;

	internal GenericArgumentCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<GenericArgumentsFilterOptions> genericArgumentsFilterOptions)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_subject = subject;
		_genericArgumentsFilterOptions = genericArgumentsFilterOptions;
	}

	/// <summary>
	///     …with the <paramref name="expected" /> number of generic arguments.
	/// </summary>
	public TSelf WithArgumentCount(int expected)
	{
		_genericArgumentsFilterOptions.Value = _genericArgumentsFilterOptions.Value.WithPredicate(arguments => arguments.Length == expected,
			() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")}");
		return (TSelf)this;
	}

	/// <summary>
	///     …with a generic argument constrained to type <typeparamref name="T" />.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TType, TValue> WithArgument<T>()
	{
		Type argumentType = typeof(T);
		GenericArgumentFilterOptions genericArgumentFilterOptions = new(
			(type, name) => argumentType == (name is null ? type.BaseType : type),
			() => $"of type {Formatter.Format(typeof(T))}");
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		_genericArgumentsFilterOptions.Value = _genericArgumentsFilterOptions.Value.WithFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithArgumentResult<TType, TValue>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions);
	}

	/// <summary>
	///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
	/// </summary>
	public GenericArgumentCollectionWithNamedArgumentResult<TType, TValue> WithArgument<T>(string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		Type argumentType = typeof(T);
		GenericArgumentFilterOptions genericArgumentFilterOptions = new GenericArgumentFilterOptions(
			(type, name) => argumentType == (name is null ? type.BaseType : type),
			() => $"of type {Formatter.Format(typeof(T))}")
			.WithPredicate(
				(type, name) => stringEqualityOptions.Value.AreConsideredEqual(name ?? type.Name, expected, nameof(expected)),
				() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		_genericArgumentsFilterOptions.Value = _genericArgumentsFilterOptions.Value.WithFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TValue>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     …with a generic argument with the <paramref name="expected" /> name.
	/// </summary>
	public GenericArgumentCollectionWithNamedArgumentResult<TType, TValue> WithArgument(string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		GenericArgumentFilterOptions genericArgumentFilterOptions = new(
			(type, name) => stringEqualityOptions.Value.AreConsideredEqual(name ?? type.Name, expected, nameof(expected)),
			() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		_genericArgumentsFilterOptions.Value = _genericArgumentsFilterOptions.Value.WithFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TValue>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions,
			stringEqualityOptions);
	}
}
