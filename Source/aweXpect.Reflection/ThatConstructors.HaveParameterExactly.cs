using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif
}
