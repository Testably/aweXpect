using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class GenericArgumentCollectionResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	GenericArgumentsFilterOptions genericArgumentsFilterOptions)
	: AndOrResult<TType, IThat<TThat>>(expectationBuilder, subject),
		IOptionsProvider<GenericArgumentsFilterOptions>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
	private readonly IThat<TThat> _subject = subject;

	/// <inheritdoc cref="IOptionsProvider{GenericArgumentsFilterOptions}.Options" />
	GenericArgumentsFilterOptions IOptionsProvider<GenericArgumentsFilterOptions>.Options
		=> genericArgumentsFilterOptions;

	/// <summary>
	///     …with the <paramref name="expected" /> number of generic arguments.
	/// </summary>
	public GenericArgumentCollectionResult<TType, TThat> WithArgumentCount(int expected)
	{
		genericArgumentsFilterOptions.AddPredicate(arguments => arguments.Length == expected,
			() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")}");
		return this;
	}

	/// <summary>
	///     …with a generic argument constrained to type <typeparamref name="T" />.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TType, TThat> WithArgument<T>()
	{
		Type argumentType = typeof(T);
		GenericArgumentFilterOptions genericArgumentFilterOptions = new(
			(type, name) => argumentType == (name is null ? type.BaseType : type),
			() => $"of type {Formatter.Format(typeof(T))}");
		CollectionIndexOptions collectionIndexOptions = new();
		genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, genericArgumentsFilterOptions, collectionIndexOptions);
	}

	/// <summary>
	///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
	/// </summary>
	public GenericArgumentCollectionWithNamedArgumentResult<TType, TThat> WithArgument<T>(string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		Type argumentType = typeof(T);
		GenericArgumentFilterOptions genericArgumentFilterOptions = new(
			(type, name) => argumentType == (name is null ? type.BaseType : type),
			() => $"of type {Formatter.Format(typeof(T))}");
		genericArgumentFilterOptions.AddPredicate(
			(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, genericArgumentsFilterOptions, collectionIndexOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     …with a generic argument with the <paramref name="expected" /> name.
	/// </summary>
	public GenericArgumentCollectionWithNamedArgumentResult<TType, TThat> WithArgument(string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		GenericArgumentFilterOptions genericArgumentFilterOptions = new(
			(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, genericArgumentsFilterOptions, collectionIndexOptions,
			stringEqualityOptions);
	}
}
