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

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<MethodInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<MethodInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<MethodInfo?>>((_, grammars)
					=> new HaveParameterConstraint(grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<MethodInfo?>>((_, grammars)
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<MethodInfo?>>((_, grammars)
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<MethodInfo?>>((_, grammars)
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<MethodInfo?>>((_, grammars)
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

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<MethodInfo?>>((_, grammars)
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
