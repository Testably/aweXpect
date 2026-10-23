using System;
using System.Linq;
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
				new CollectionIndexOptions());
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			StringEqualityOptions stringEqualityOptions = new(nameof(expected));
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => argumentType == (name is null ? type.BaseType : type),
				() => $"of type {Formatter.Format(typeof(T))}");
			genericArgumentFilterOptions.AddPredicate(
				(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			return new GenericTypesWithNamedArgument(this,
				collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions),
				new CollectionIndexOptions(), stringEqualityOptions);
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument(string expected)
		{
			StringEqualityOptions stringEqualityOptions = new(nameof(expected));
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			return new GenericTypesWithNamedArgument(this,
				collectionIndexOptions => WithArgumentFilter(genericArgumentFilterOptions, collectionIndexOptions),
				new CollectionIndexOptions(), stringEqualityOptions);
		}

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

		/// <summary>
		///     Additional filters on types with a generic argument of a specific type.
		/// </summary>
		public class GenericTypesWithArgument : GenericTypes
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

			/// <summary>
			///     …at the given zero-based <paramref name="index" />.
			/// </summary>
			/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
			/// <exception cref="InvalidOperationException">An index is already specified.</exception>
			public GenericTypesWithArgument AtIndex(int index)
				=> WithIndex(o => o.AtIndex(index));

			/// <summary>
			///     …at the given zero-based <paramref name="index" />, counted from the end of the generic arguments.
			/// </summary>
			/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
			/// <exception cref="InvalidOperationException">An index is already specified.</exception>
			public GenericTypesWithArgument AtIndexFromEnd(int index)
				=> WithIndex(o => o.AtIndexFromEnd(index));

			private GenericTypesWithArgument WithIndex(Action<CollectionIndexOptions> setIndex)
				=> new(_source, _createFilter, FilterIndexOptions.With(_collectionIndexOptions, setIndex));
		}

		/// <summary>
		///     Additional filters on types with a named parameter of a specific type.
		/// </summary>
		public class GenericTypesWithNamedArgument
			: GenericTypesWithArgument,
				IOptionsProvider<StringEqualityOptions>,
				IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal GenericTypesWithNamedArgument(
				Filtered.Types source,
				Func<CollectionIndexOptions, IFilter<Type>> createFilter,
				CollectionIndexOptions collectionIndexOptions,
				StringEqualityOptions options)
				: base(source, createFilter, collectionIndexOptions)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
