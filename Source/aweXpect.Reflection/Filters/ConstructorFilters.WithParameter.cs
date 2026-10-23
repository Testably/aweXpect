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

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithParameter<T>(this Filtered.Constructors @this)
	{
		Type parameterType = typeof(T);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of type <typeparamref name="T" /> with the <paramref name="expected" />
	///     name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithParameter<T>(this Filtered.Constructors @this, string expected)
	{
		Type parameterType = typeof(T);
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithNamedParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithParameter(this Filtered.Constructors @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameter(this Filtered.Constructors @this,
		Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithNamedParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameter(this Filtered.Constructors @this,
		string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"with name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithNamedParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}


	/// <summary>
	///     Filter for constructors without a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithoutParameter<T>(this Filtered.Constructors @this)
		=> WithoutParameterOfType<T>(@this, typeof(T));

	/// <summary>
	///     Filter for constructors without a parameter of type <typeparamref name="T" /> with the
	///     <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithoutParameter<T>(this Filtered.Constructors @this,
		string unexpected)
		=> WithoutNamedParameter<T>(@this, typeof(T), unexpected);

	/// <summary>
	///     Filter for constructors without a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		Type parameterType)
		=> WithoutParameterOfType<object?>(@this, parameterType);

	/// <summary>
	///     Filter for constructors without a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		Type parameterType, string unexpected)
		=> WithoutNamedParameter<object?>(@this, parameterType, unexpected);

	/// <summary>
	///     Filter for constructors without a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		string unexpected)
		=> WithoutNamedParameter<object?>(@this, null, unexpected);

	private static ConstructorsWithParameter<T> WithoutParameterOfType<T>(Filtered.Constructors @this,
		Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<T>(
			@this.Which(WithoutParameterFilter(collectionIndexOptions, parameterFilterOptions)),
			collectionIndexOptions, parameterFilterOptions);
	}

	private static ConstructorsWithNamedParameter<T> WithoutNamedParameter<T>(Filtered.Constructors @this,
		Type? parameterType, string unexpected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(unexpected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions;
		if (parameterType is null)
		{
			parameterFilterOptions = new(p => stringEqualityOptions.AreConsideredEqual(p.Name, unexpected),
				() => $"with name {stringEqualityOptions.GetExpectation(unexpected, ExpectationGrammars.None)}");
		}
		else
		{
			parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
				() => $"of type {Formatter.Format(parameterType)}");
			parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, unexpected),
				() => $"name {stringEqualityOptions.GetExpectation(unexpected, ExpectationGrammars.None)}");
		}

		return new ConstructorsWithNamedParameter<T>(
			@this.Which(WithoutParameterFilter(collectionIndexOptions, parameterFilterOptions)),
			collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	private static IAsyncChangeableFilter<ConstructorInfo> WithoutParameterFilter(
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<ConstructorInfo>(
			async constructorInfo
				=> !await constructorInfo.HasMatchingParameter(collectionIndexOptions, parameterFilterOptions),
			()
				=> $"without parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");

	/// <summary>
	///     Additional filters on constructors with a parameter of a specific type.
	/// </summary>
	public class ConstructorsWithParameter<T>
		: Filtered.Constructors,
			IOptionsProvider<CollectionIndexOptions>,
			IOptionsProvider<ParameterFilterOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly ParameterFilterOptions _parameterFilterOptions;

		internal ConstructorsWithParameter(
			Filtered.Constructors inner,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFilterOptions)
			: base(inner)
		{
			_collectionIndexOptions = collectionIndexOptions;
			_parameterFilterOptions = parameterFilterOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
		ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => _parameterFilterOptions;

		/// <summary>
		///     …at the given <paramref name="index" />.
		/// </summary>
		public ConstructorsWithParameterAtIndex<T> AtIndex(int index)
		{
			_collectionIndexOptions.SetMatch(new AtIndexMatch(index));
			return new ConstructorsWithParameterAtIndex<T>(this, _collectionIndexOptions);
		}

		/// <summary>
		///     …without a default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithoutDefaultValue()
		{
			_parameterFilterOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
			return this;
		}

		/// <summary>
		///     …with a default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithDefaultValue()
		{
			_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
			return this;
		}

		/// <summary>
		///     …with the <paramref name="expected" /> default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithDefaultValue<TValue>(TValue expected)
			where TValue : T
		{
			_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
				() => $"with default value {Formatter.Format(expected)}");
			return this;
		}

		/// <summary>
		///     …with the <see langword="ref" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithRefModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsRefParameter(), () => "with ref modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="out" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithOutModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsOutParameter(), () => "with out modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="in" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithInModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsInParameter(), () => "with in modifier");
			return this;
		}

		/// <summary>
		///     …with the optional modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithOptionalModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsOptionalParameter(), () => "with optional modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="params" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithParamsModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsParamsParameter(), () => "with params modifier");
			return this;
		}
	}

	/// <summary>
	///     Additional filters on constructors with a parameter at a specific index.
	/// </summary>
	public class ConstructorsWithParameterAtIndex<T>
		: Filtered.Constructors,
			IOptionsProvider<CollectionIndexOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;

		internal ConstructorsWithParameterAtIndex(
			Filtered.Constructors inner,
			CollectionIndexOptions collectionIndexOptions)
			: base(inner)
		{
			_collectionIndexOptions = collectionIndexOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <summary>
		///     …from end.
		/// </summary>
		public ConstructorsWithParameterAtIndex<T> FromEnd()
		{
			if (_collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
			{
				_collectionIndexOptions.SetMatch(match.FromEnd());
			}

			return this;
		}
	}

	/// <summary>
	///     Additional filters on constructors with a named parameter of a specific type.
	/// </summary>
	public class ConstructorsWithNamedParameter<T>
		: ConstructorsWithParameter<T>,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions
	{
		private readonly StringEqualityOptions _options;

		internal ConstructorsWithNamedParameter(
			Filtered.Constructors inner,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFilterOptions,
			StringEqualityOptions options)
			: base(inner, collectionIndexOptions, parameterFilterOptions)
		{
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
	}
}
