using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

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
	public class GenericTypes(Filtered.Types inner) : Filtered.Types(inner)
	{
		internal Filtered.Types FilteredTypes { get; } = inner;

		/// <summary>
		///     …with the <paramref name="expected" /> number of generic arguments.
		/// </summary>
		public GenericTypes WithArgumentCount(int expected)
		{
			Which(Filter.Suffix<Type>(
				type => type.GetGenericArguments().Length == expected,
				() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")} "));
			return this;
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" />.
		/// </summary>
		public GenericTypesWithArgument WithArgument<T>()
		{
			Type argumentType = typeof(T);
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => argumentType == (name is null ? type.BaseType : type),
				() => $"of type {Formatter.Format(typeof(T))}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<Type> filter = Filter.Suffix<Type>(
				type =>
				{
					Type[] arguments = type.GetGenericArguments();
					return arguments.AnyAsync(async (p, i) =>
					{
						bool? isIndexInRange = collectionIndexOptions.Match switch
						{
							CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
							CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, arguments.Length),
							_ => false,
						};
						return isIndexInRange == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
			Which(filter);
			return new GenericTypesWithArgument(this, collectionIndexOptions);
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			StringEqualityOptions stringEqualityOptions = new();
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => argumentType == (name is null ? type.BaseType : type),
				() => $"of type {Formatter.Format(typeof(T))}");
			genericArgumentFilterOptions.AddPredicate(
				(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<Type> filter = Filter.Suffix<Type>(
				type =>
				{
					Type[] arguments = type.GetGenericArguments();
					return arguments.AnyAsync(async (p, i) =>
					{
						bool? isIndexInRange = collectionIndexOptions.Match switch
						{
							CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
							CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, arguments.Length),
							_ => false,
						};
						return isIndexInRange == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
			Which(filter);
			return new GenericTypesWithNamedArgument(this, collectionIndexOptions, stringEqualityOptions);
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericTypesWithNamedArgument WithArgument(string expected)
		{
			StringEqualityOptions stringEqualityOptions = new();
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(type, name) => stringEqualityOptions.AreConsideredEqual(name ?? type.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<Type> filter = Filter.Suffix<Type>(
				type =>
				{
					Type[] arguments = type.GetGenericArguments();
					return arguments.AnyAsync(async (p, i) =>
					{
						bool? isIndexInRange = collectionIndexOptions.Match switch
						{
							CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
							CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, arguments.Length),
							_ => false,
						};
						return isIndexInRange == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
			Which(filter);
			return new GenericTypesWithNamedArgument(this, collectionIndexOptions, stringEqualityOptions);
		}

		/// <summary>
		///     Additional filters on types with a generic argument of a specific type.
		/// </summary>
		public class GenericTypesWithArgument(
			GenericTypes inner,
			CollectionIndexOptions collectionIndexOptions)
			: GenericTypes(inner.FilteredTypes),
				IOptionsProvider<CollectionIndexOptions>
		{
			/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
			CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

			/// <summary>
			///     …at the given <paramref name="index" />.
			/// </summary>
			public GenericTypesWithArgumentAtIndex AtIndex(int index)
			{
				collectionIndexOptions.SetMatch(new AtIndexMatch(index));
				return new GenericTypesWithArgumentAtIndex(this, collectionIndexOptions);
			}
		}

		/// <summary>
		///     Additional filters on types with a named parameter of a specific type.
		/// </summary>
		public class GenericTypesWithNamedArgument(
			GenericTypes inner,
			CollectionIndexOptions collectionIndexOptions,
			StringEqualityOptions options)
			: GenericTypesWithArgument(inner, collectionIndexOptions),
				IOptionsProvider<StringEqualityOptions>
		{
			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

			/// <summary>
			///     Ignores casing when comparing the parameter name,
			///     according to the <paramref name="ignoreCase" /> parameter.
			/// </summary>
			public GenericTypesWithNamedArgument IgnoringCase(bool ignoreCase = true)
			{
				options.IgnoringCase(ignoreCase);
				return this;
			}

			/// <summary>
			///     Uses the provided <paramref name="comparer" /> for comparing parameter names.
			/// </summary>
			public GenericTypesWithNamedArgument Using(IEqualityComparer<string> comparer)
			{
				options.UsingComparer(comparer);
				return this;
			}

			/// <summary>
			///     Interprets the expected parameter name as a prefix, so that the actual value starts with it.
			/// </summary>
			public GenericTypesWithNamedArgument AsPrefix()
			{
				options.AsPrefix();
				return this;
			}

			/// <summary>
			///     Interprets the expected parameter name as a <see cref="Regex" /> pattern.
			/// </summary>
			public GenericTypesWithNamedArgument AsRegex()
			{
				options.AsRegex();
				return this;
			}

			/// <summary>
			///     Interprets the expected parameter name as a suffix, so that the actual value ends with it.
			/// </summary>
			public GenericTypesWithNamedArgument AsSuffix()
			{
				options.AsSuffix();
				return this;
			}

			/// <summary>
			///     Interprets the expected parameter name as wildcard pattern.<br />
			///     Supports * to match zero or more characters and ? to match exactly one character.
			/// </summary>
			public GenericTypesWithNamedArgument AsWildcard()
			{
				options.AsWildcard();
				return this;
			}
		}

		/// <summary>
		///     Additional filters on types with a generic argument at a specific index.
		/// </summary>
		public class GenericTypesWithArgumentAtIndex(
			GenericTypes inner,
			CollectionIndexOptions collectionIndexOptions)
			: GenericTypes(inner.FilteredTypes),
				IOptionsProvider<CollectionIndexOptions>
		{
			/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
			CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

			/// <summary>
			///     …from end.
			/// </summary>
			public GenericTypes FromEnd()
			{
				if (collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
				{
					collectionIndexOptions.SetMatch(match.FromEnd());
				}

				return this;
			}
		}
	}
}
