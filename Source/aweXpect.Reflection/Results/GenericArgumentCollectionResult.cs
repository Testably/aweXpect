using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class GenericArgumentCollectionResult<TType, TThat>
	: GenericArgumentCollectionResult<TType, TThat, GenericArgumentCollectionResult<TType, TThat>>
{
	internal GenericArgumentCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions)
	{
	}
}

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class GenericArgumentCollectionResult<TType, TThat, TSelf>
	: AndOrResult<TType, IThat<TThat>, TSelf>,
		IOptionsProvider<GenericArgumentsFilterOptions>
	where TSelf : GenericArgumentCollectionResult<TType, TThat, TSelf>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly GenericArgumentsFilterOptions _genericArgumentsFilterOptions;
	private readonly IThat<TThat> _subject;

	internal GenericArgumentCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_subject = subject;
		_genericArgumentsFilterOptions = genericArgumentsFilterOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{GenericArgumentsFilterOptions}.Options" />
	GenericArgumentsFilterOptions IOptionsProvider<GenericArgumentsFilterOptions>.Options
		=> _genericArgumentsFilterOptions;

	/// <summary>
	///     …with the <paramref name="expected" /> number of generic arguments.
	/// </summary>
	public TSelf WithArgumentCount(int expected)
	{
		_genericArgumentsFilterOptions.AddPredicate(arguments => arguments.Length == expected,
			() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")}");
		return (TSelf)this;
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
		_genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions);
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
		_genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions,
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
		_genericArgumentsFilterOptions.AddFilter(genericArgumentFilterOptions, collectionIndexOptions);
		return new GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions,
			stringEqualityOptions);
	}
}
