using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

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
	public class GenericMethods(Filtered.Methods inner) : Filtered.Methods(inner)
	{
		internal Filtered.Methods FilteredMethods { get; } = inner;

		/// <summary>
		///     …with the <paramref name="expected" /> number of generic arguments.
		/// </summary>
		public GenericMethods WithArgumentCount(int expected)
		{
			Which(Filter.Suffix<MethodInfo>(
				method => method.GetGenericArguments().Length == expected,
				() => $"with {expected} generic {(expected == 1 ? "argument" : "arguments")} "));
			return this;
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" />.
		/// </summary>
		public GenericMethodsWithArgument WithArgument<T>()
		{
			Type argumentType = typeof(T);
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(p, _) => argumentType == p.BaseType,
				() => $"of type {Formatter.Format(typeof(T))}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
				method =>
				{
					Type[] arguments = method.GetGenericArguments();
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
			return new GenericMethodsWithArgument(this, collectionIndexOptions);
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			StringEqualityOptions stringEqualityOptions = new();
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(p, _) => argumentType == p.BaseType,
				() => $"of type {Formatter.Format(typeof(T))}");
			genericArgumentFilterOptions.AddPredicate(
				(p, _) => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
				method =>
				{
					Type[] arguments = method.GetGenericArguments();
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
			return new GenericMethodsWithNamedArgument(this, collectionIndexOptions, stringEqualityOptions);
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument(string expected)
		{
			StringEqualityOptions stringEqualityOptions = new();
			GenericArgumentFilterOptions genericArgumentFilterOptions = new(
				(p, _) => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
				() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
				method =>
				{
					Type[] arguments = method.GetGenericArguments();
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
			return new GenericMethodsWithNamedArgument(this, collectionIndexOptions, stringEqualityOptions);
		}
	}

	/// <summary>
	///     Additional filters on methods with a generic argument of a specific type.
	/// </summary>
	public class GenericMethodsWithArgument(
		GenericMethods inner,
		CollectionIndexOptions collectionIndexOptions)
		: GenericMethods(inner.FilteredMethods),
			IOptionsProvider<CollectionIndexOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

		/// <summary>
		///     …at the given <paramref name="index" />.
		/// </summary>
		public GenericMethodsWithArgumentAtIndex AtIndex(int index)
		{
			collectionIndexOptions.SetMatch(new AtIndexMatch(index));
			return new GenericMethodsWithArgumentAtIndex(this, collectionIndexOptions);
		}
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class GenericMethodsWithNamedArgument(
		GenericMethods inner,
		CollectionIndexOptions collectionIndexOptions,
		StringEqualityOptions options)
		: GenericMethodsWithArgument(inner, collectionIndexOptions),
			IOptionsProvider<StringEqualityOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

		/// <summary>
		///     Ignores casing when comparing the generic argument name,
		///     according to the <paramref name="ignoreCase" /> parameter.
		/// </summary>
		public GenericMethodsWithNamedArgument IgnoringCase(bool ignoreCase = true)
		{
			options.IgnoringCase(ignoreCase);
			return this;
		}

		/// <summary>
		///     Uses the provided <paramref name="comparer" /> for comparing generic argument names.
		/// </summary>
		public GenericMethodsWithNamedArgument Using(IEqualityComparer<string> comparer)
		{
			options.UsingComparer(comparer);
			return this;
		}

		/// <summary>
		///     Interprets the expected generic argument name as a prefix, so that the actual value starts with it.
		/// </summary>
		public GenericMethodsWithNamedArgument AsPrefix()
		{
			options.AsPrefix();
			return this;
		}

		/// <summary>
		///     Interprets the expected generic argument name as a <see cref="Regex" /> pattern.
		/// </summary>
		public GenericMethodsWithNamedArgument AsRegex()
		{
			options.AsRegex();
			return this;
		}

		/// <summary>
		///     Interprets the expected generic argument name as a suffix, so that the actual value ends with it.
		/// </summary>
		public GenericMethodsWithNamedArgument AsSuffix()
		{
			options.AsSuffix();
			return this;
		}

		/// <summary>
		///     Interprets the expected generic argument name as wildcard pattern.<br />
		///     Supports * to match zero or more characters and ? to match exactly one character.
		/// </summary>
		public GenericMethodsWithNamedArgument AsWildcard()
		{
			options.AsWildcard();
			return this;
		}
	}

	/// <summary>
	///     Additional filters on methods with a generic argument at a specific index.
	/// </summary>
	public class GenericMethodsWithArgumentAtIndex(
		GenericMethods inner,
		CollectionIndexOptions collectionIndexOptions)
		: GenericMethods(inner.FilteredMethods),
			IOptionsProvider<CollectionIndexOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

		/// <summary>
		///     …from end.
		/// </summary>
		public GenericMethods FromEnd()
		{
			if (collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
			{
				collectionIndexOptions.SetMatch(match.FromEnd());
			}

			return this;
		}
	}
}
