using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that are generic.
	/// </summary>
	public static GenericMethods WhichAreGeneric(this Filtered.Methods @this)
		=> new(@this.Which(Filter.Prefix<MethodInfo>(
			method => method.IsGenericMethod,
			"generic ")));

	/// <summary>
	///     Filters for methods that are not generic.
	/// </summary>
	public static Filtered.Methods WhichAreNotGeneric(this Filtered.Methods @this)
		=> @this.Which(Filter.Prefix<MethodInfo>(
			method => !method.IsGenericMethod,
			"non-generic "));

	/// <summary>
	///     Additional filters on generic methods.
	/// </summary>
	public class GenericMethods : Filtered.Methods
	{
		internal GenericMethods(Filtered.Methods inner) : base(inner)
		{
		}

		/// <summary>
		///     …with the <paramref name="expected" /> number of generic arguments.
		/// </summary>
		public GenericMethods WithArgumentCount(int expected)
			=> new(Which(Filter.Suffix<MethodInfo>(
				method => method.GetGenericArguments().Length == expected,
				() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")} ")));

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" />.
		/// </summary>
		public GenericMethodsWithArgument WithArgument<T>()
		{
			Type argumentType = typeof(T);
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(p, _) => argumentType == p.BaseType,
				() => $"of type {Formatter.Format(typeof(T))}");
			return new GenericMethodsWithArgument(this,
				collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions),
				CollectionIndexOptions.Default);
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			return new GenericMethodsWithNamedArgument(this,
				stringEqualityOptions =>
				{
					GenericArgumentFilterOptions genericArgumentFilterOptions = new GenericArgumentFilterOptions(
						(p, _) => argumentType == p.BaseType,
						() => $"of type {Formatter.Format(typeof(T))}")
						.WithPredicate(
							(p, _) => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
							() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
					return collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions);
				},
				CollectionIndexOptions.Default, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument(string expected)
			=> new(this,
				stringEqualityOptions =>
				{
					GenericArgumentFilterOptions genericArgumentFilterOptions = new(
						(p, _) => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
					return collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions);
				},
				CollectionIndexOptions.Default, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));

		private static IFilter<MethodInfo> WithArgumentFilter(GenericArgumentFilterOptions genericArgumentFilterOptions,
			CollectionIndexOptions collectionIndexOptions)
			=> Filter.Suffix<MethodInfo>(
				method =>
				{
					Type[] arguments = method.GetGenericArguments();
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
	///     Additional filters on methods with a generic argument of a specific type.
	/// </summary>
	public class GenericMethodsWithArgument : GenericMethods,
		IOptionsProvider<CollectionIndexOptions>,
		ICopyOnOption<CollectionIndexOptions, GenericMethodsWithArgument>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, IFilter<MethodInfo>> _createFilter;
		private readonly Filtered.Methods _source;

		internal GenericMethodsWithArgument(
			Filtered.Methods source,
			Func<CollectionIndexOptions, IFilter<MethodInfo>> createFilter,
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
		GenericMethodsWithArgument ICopyOnOption<CollectionIndexOptions, GenericMethodsWithArgument>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options);
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class GenericMethodsWithNamedArgument
		: GenericMethodsWithArgument,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, GenericMethodsWithNamedArgument>,
			ICopyOnOption<CollectionIndexOptions, GenericMethodsWithNamedArgument>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, Func<CollectionIndexOptions, IFilter<MethodInfo>>> _createFilter;
		private readonly OptionsHolder<StringEqualityOptions> _options;
		private readonly Filtered.Methods _source;

		internal GenericMethodsWithNamedArgument(
			Filtered.Methods source,
			Func<IReadOnlyOptionsHolder<StringEqualityOptions>, Func<CollectionIndexOptions, IFilter<MethodInfo>>> createFilter,
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
		GenericMethodsWithNamedArgument ICopyOnOption<CollectionIndexOptions, GenericMethodsWithNamedArgument>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, new OptionsHolder<StringEqualityOptions>(_options.Value));

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		GenericMethodsWithNamedArgument ICopyOnOption<StringEqualityOptions, GenericMethodsWithNamedArgument>.WithOptions(
			StringEqualityOptions options)
			=> new(_source, _createFilter, _collectionIndexOptions, new OptionsHolder<StringEqualityOptions>(options));
	}
}
