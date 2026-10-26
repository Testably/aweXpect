using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
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
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
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
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif
}
