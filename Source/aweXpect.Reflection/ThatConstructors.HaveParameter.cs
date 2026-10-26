using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
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
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(
			p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((expected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> DoNotHaveParameterOfType<TParameter>(subject, typeof(TParameter));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> DoNotHaveParameterOfType<object?>(subject, parameterType);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<TParameter>(subject, typeof(TParameter), unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, parameterType, unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, null, unexpected);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> DoNotHaveParameterOfType<TParameter>(subject, typeof(TParameter));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> DoNotHaveParameterOfType<object?>(subject, parameterType);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<TParameter>(subject, typeof(TParameter), unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, parameterType, unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, null, unexpected);
#endif

	private static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IThat<IEnumerable<ConstructorInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(predicate);
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(string Kind, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((kind, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						kind: s.Kind)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(predicate);
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(string Kind, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((kind, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						kind: s.Kind)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

	private static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameterOfType<TParameter>(
		IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameterOfType<TParameter>(
		IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

	private static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveNamedParameter<TParameter>(
		IThat<IEnumerable<ConstructorInfo?>?> subject, Type? parameterType, string unexpected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected)));
		if (parameterType is not null)
		{
			parameterFilterOptions.AddPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		}

		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type? ParameterType, string Unexpected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IEnumerable<ConstructorInfo?>>((parameterType, unexpected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, s.Unexpected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

#if NET8_0_OR_GREATER
	private static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveNamedParameter<TParameter>(
		IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type? parameterType, string unexpected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected)));
		if (parameterType is not null)
		{
			parameterFilterOptions.AddPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		}

		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type? ParameterType, string Unexpected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, ParameterFilterOptions ParameterFilterOptions), IAsyncEnumerable<ConstructorInfo?>>((parameterType, unexpected, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, s.Unexpected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
#endif

	private sealed class HaveParameterConstraint(
		string it,
		ExpectationGrammars grammars,
		Type? parameterType,
		string? expectedName,
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions,
		bool exactType = false,
		string? kind = null)
		: CollectionConstraintResult<ConstructorInfo?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, async constructor =>
			{
				if (constructor == null)
				{
					return false;
				}

				ParameterInfo[] parameters = constructor.GetParameters();
				bool hasParameter = await parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.Value.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Matches(p);
				});
				return hasParameter;
			}, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetValue(actual, context, async constructor =>
			{
				if (constructor == null)
				{
					return false;
				}

				ParameterInfo[] parameters = constructor.GetParameters();
				bool hasParameter = await parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.Value.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Matches(p);
				});
				return hasParameter;
			}, cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(kind is null ? "all have parameter" : $"all have {kind} parameter");
			if (parameterType != null)
			{
				stringBuilder.Append(exactType ? " of exact type " : " of type ").Append(Formatter.Format(parameterType));
			}

			if (expectedName != null)
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(expectedName));
			}

			stringBuilder.Append(parameterFilterOptions.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Value.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained constructors without ").Append(kind ?? "a matching").Append(" parameter");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(kind is null ? "not all have parameter" : $"not all have {kind} parameter");
			if (parameterType != null)
			{
				stringBuilder.Append(exactType ? " of exact type " : " of type ").Append(Formatter.Format(parameterType));
			}

			if (expectedName != null)
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(expectedName));
			}

			stringBuilder.Append(parameterFilterOptions.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Value.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained constructors with ").Append(kind ?? "a matching").Append(" parameter");
	}

	private sealed class DoNotHaveParameterConstraint(
		string it,
		ExpectationGrammars grammars,
		Type? parameterType,
		string? unexpectedName,
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions)
		: CollectionConstraintResult<ConstructorInfo?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, DoesNotHaveParameter, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetValue(actual, context, DoesNotHaveParameter, cancellationToken);

		private async ValueTask<bool> DoesNotHaveParameter(ConstructorInfo? constructor)
			=> constructor is not null &&
			   !await constructor.HasMatchingParameter(collectionIndexOptions.Value, parameterFilterOptions);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendParameterDescription(stringBuilder.Append("all do not have parameter"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained constructors with a matching parameter");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendParameterDescription(stringBuilder.Append("also contain a constructor with parameter"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained constructors without a matching parameter");

		private void AppendParameterDescription(StringBuilder stringBuilder)
		{
			if (parameterType != null)
			{
				stringBuilder.Append(" of type ").Append(Formatter.Format(parameterType));
			}

			if (unexpectedName != null)
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(unexpectedName));
			}

			stringBuilder.Append(parameterFilterOptions.GetModifierDescription());
			stringBuilder.Append(collectionIndexOptions.Value.Match.GetDescription());
		}
	}
}
