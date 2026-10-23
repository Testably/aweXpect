using System;
using System.Linq;
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
			FilteredMethods = inner;
		}

		internal Filtered.Methods FilteredMethods { get; }

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
			CollectionIndexOptions collectionIndexOptions = new();
			IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
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
			Filtered.Methods filtered = Which(filter);
			return new GenericMethodsWithArgument(new GenericMethods(filtered), collectionIndexOptions);
		}

		/// <summary>
		///     …with a generic argument constrained to type <typeparamref name="T" /> and the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument<T>(string expected)
		{
			Type argumentType = typeof(T);
			StringEqualityOptions stringEqualityOptions = new(nameof(expected));
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
						return collectionIndexOptions.MatchesIndex(i, arguments.Length) == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
			Filtered.Methods filtered = Which(filter);
			return new GenericMethodsWithNamedArgument(new GenericMethods(filtered), collectionIndexOptions, stringEqualityOptions);
		}

		/// <summary>
		///     …with a generic argument with the <paramref name="expected" /> name.
		/// </summary>
		public GenericMethodsWithNamedArgument WithArgument(string expected)
		{
			StringEqualityOptions stringEqualityOptions = new(nameof(expected));
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
						return collectionIndexOptions.MatchesIndex(i, arguments.Length) == true &&
						       await genericArgumentFilterOptions.Matches(p);
					});
				},
				()
					=> $"with argument {genericArgumentFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
			Filtered.Methods filtered = Which(filter);
			return new GenericMethodsWithNamedArgument(new GenericMethods(filtered), collectionIndexOptions, stringEqualityOptions);
		}
	}

	/// <summary>
	///     Additional filters on methods with a generic argument of a specific type.
	/// </summary>
	public class GenericMethodsWithArgument
		: GenericMethods,
			IOptionsProvider<CollectionIndexOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;

		internal GenericMethodsWithArgument(
			GenericMethods inner,
			CollectionIndexOptions collectionIndexOptions)
			: base(inner.FilteredMethods)
		{
			_collectionIndexOptions = collectionIndexOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <summary>
		///     …at the given <paramref name="index" />.
		/// </summary>
		public GenericMethodsWithArgumentAtIndex AtIndex(int index)
		{
			_collectionIndexOptions.SetMatch(new AtIndexMatch(index));
			return new GenericMethodsWithArgumentAtIndex(this, _collectionIndexOptions);
		}
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class GenericMethodsWithNamedArgument
		: GenericMethodsWithArgument,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions
	{
		private readonly StringEqualityOptions _options;

		internal GenericMethodsWithNamedArgument(
			GenericMethods inner,
			CollectionIndexOptions collectionIndexOptions,
			StringEqualityOptions options)
			: base(inner, collectionIndexOptions)
		{
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
	}

	/// <summary>
	///     Additional filters on methods with a generic argument at a specific index.
	/// </summary>
	public class GenericMethodsWithArgumentAtIndex
		: GenericMethods,
			IOptionsProvider<CollectionIndexOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;

		internal GenericMethodsWithArgumentAtIndex(
			GenericMethods inner,
			CollectionIndexOptions collectionIndexOptions)
			: base(inner.FilteredMethods)
		{
			_collectionIndexOptions = collectionIndexOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <summary>
		///     …from end.
		/// </summary>
		public GenericMethods FromEnd()
		{
			if (_collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
			{
				_collectionIndexOptions.SetMatch(match.FromEnd());
			}

			return this;
		}
	}
}
