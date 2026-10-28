using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are generic.
	/// </summary>
	public static GenericTypes WhichAreGeneric(this Filtered.Types @this)
		=> new(@this.Which(Filter.Prefix<Type>(
			type => type.IsGenericType,
			"generic ")));

	/// <summary>
	///     Filters for types that are not generic.
	/// </summary>
	public static Filtered.Types WhichAreNotGeneric(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsGenericType,
			"non-generic "));

	/// <summary>
	///     Additional filters on generic types.
	/// </summary>
	public class GenericTypes : Filtered.Types
	{
		internal GenericTypes(Filtered.Types inner) : base(inner)
		{
		}

		/// <summary>
		///     …with the <paramref name="expected" /> number of generic arguments.
		/// </summary>
		public GenericTypes WithArgumentCount(int expected)
			=> new(Which(Filter.Suffix<Type>(
				type => type.GetGenericArguments().Length == expected,
				() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")} ")));

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" />.
		/// </summary>
		public GenericTypesWithArgument WithArgument<T>()
		{
			Type argumentType = typeof(T);
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => argumentType == (name is null ? type.BaseType : type),
				() => $"of type {Formatter.Format(typeof(T))}");
			return new GenericTypesWithArgument(this,
				collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions),
				CollectionIndexOptions.Default);
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			return new GenericTypesWithNamedArgument(this,
				stringEqualityOptions =>
				{
					GenericArgumentFilterOptions genericArgumentFilterOptions = new GenericArgumentFilterOptions(
						(type, name) => argumentType == (name is null ? type.BaseType : type),
						() => $"of type {Formatter.Format(typeof(T))}")
						.WithPredicate(
							(type, name) => stringEqualityOptions.Value.AreConsideredEqual(name ?? type.Name, expected, nameof(expected)),
							() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
					return collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions);
				},
				CollectionIndexOptions.Default, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument(string expected)
			=> new(this,
				stringEqualityOptions =>
				{
					GenericArgumentFilterOptions genericArgumentFilterOptions = new(
						(type, name) => stringEqualityOptions.Value.AreConsideredEqual(name ?? type.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
					return collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions);
				},
				CollectionIndexOptions.Default, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));

		private static IFilter<Type> WithArgumentFilter(GenericArgumentFilterOptions genericArgumentFilterOptions,
			CollectionIndexOptions collectionIndexOptions)
			=> Filter.Suffix<Type>(
				type =>
				{
					Type[] arguments = type.GetGenericArguments();
					return arguments.AnyAsync(async (p, i) =>
					{
						return collectionIndexOptions.MatchesIndex(i, arguments.Length) == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
	}

	/// <summary>
	///     Additional filters on types with a generic argument of a specific type.
	/// </summary>
	public class GenericTypesWithArgument : GenericTypes,
		IOptionsProvider<CollectionIndexOptions>,
		ICopyOnOption<CollectionIndexOptions, GenericTypesWithArgument>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, IFilter<Type>> _createFilter;
		private readonly Filtered.Types _source;

		internal GenericTypesWithArgument(
			Filtered.Types source,
			Func<CollectionIndexOptions, IFilter<Type>> createFilter,
			CollectionIndexOptions collectionIndexOptions)
			: base(source.Which(createFilter(collectionIndexOptions)))
		{
			_source = source;
			_createFilter = createFilter;
			_collectionIndexOptions = collectionIndexOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options
			=> new(_collectionIndexOptions);

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		GenericTypesWithArgument ICopyOnOption<CollectionIndexOptions, GenericTypesWithArgument>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options);
	}

	/// <summary>
	///     Additional filters on types with a named parameter of a specific type.
	/// </summary>
	public class GenericTypesWithNamedArgument
		: GenericTypesWithArgument,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, GenericTypesWithNamedArgument>,
			ICopyOnOption<CollectionIndexOptions, GenericTypesWithNamedArgument>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, Func<CollectionIndexOptions, IFilter<Type>>> _createFilter;
		private readonly OptionsHolder<StringEqualityOptions> _options;
		private readonly Filtered.Types _source;

		internal GenericTypesWithNamedArgument(
			Filtered.Types source,
			Func<IReadOnlyOptionsHolder<StringEqualityOptions>, Func<CollectionIndexOptions, IFilter<Type>>> createFilter,
			CollectionIndexOptions collectionIndexOptions,
			OptionsHolder<StringEqualityOptions> options)
			: base(source, createFilter(options), collectionIndexOptions)
		{
			_source = source;
			_createFilter = createFilter;
			_collectionIndexOptions = collectionIndexOptions;
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		GenericTypesWithNamedArgument ICopyOnOption<CollectionIndexOptions, GenericTypesWithNamedArgument>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, new OptionsHolder<StringEqualityOptions>(_options.Value));

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		GenericTypesWithNamedArgument ICopyOnOption<StringEqualityOptions, GenericTypesWithNamedArgument>.WithOptions(
			StringEqualityOptions options)
			=> new(_source, _createFilter, _collectionIndexOptions, new OptionsHolder<StringEqualityOptions>(options));
	}
}
