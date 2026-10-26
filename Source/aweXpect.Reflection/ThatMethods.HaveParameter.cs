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

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new HaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected))));
		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type ParameterType, string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(
			p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected))));
		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(string Expected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((expected, collectionIndexOptions, parameterFilterOptions),
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
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> DoNotHaveParameterOfType<TParameter>(subject, typeof(TParameter));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> DoNotHaveParameterOfType<object?>(subject, parameterType);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<TParameter>(subject, typeof(TParameter), unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, parameterType, unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, null, unexpected);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> DoNotHaveParameterOfType<TParameter>(subject, typeof(TParameter));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> DoNotHaveParameterOfType<object?>(subject, parameterType);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<TParameter>(subject, typeof(TParameter), unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, parameterType, unexpected);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string unexpected)
		=> DoNotHaveNamedParameter<object?>(subject, null, unexpected);
#endif

	private static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IThat<IEnumerable<MethodInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(predicate));
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(string Kind, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((kind, collectionIndexOptions, parameterFilterOptions),
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
	private static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IThat<IAsyncEnumerable<MethodInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(predicate));
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<(string Kind, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((kind, collectionIndexOptions, parameterFilterOptions),
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

	private static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameterOfType<TParameter>(
		IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameterOfType<TParameter>(
		IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions =
			new(new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType)));
		return new ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<(Type ParameterType, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, collectionIndexOptions, parameterFilterOptions),
					static (s, it, grammars)
					=> new DoNotHaveParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}
#endif

	private static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveNamedParameter<TParameter>(
		IThat<IEnumerable<MethodInfo?>?> subject, Type? parameterType, string unexpected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected))));
		if (parameterType is not null)
		{
			parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		}

		return new NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type? ParameterType, string Unexpected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IEnumerable<MethodInfo?>>((parameterType, unexpected, collectionIndexOptions, parameterFilterOptions),
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
	private static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveNamedParameter<TParameter>(
		IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type? parameterType, string unexpected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		OptionsHolder<ParameterFilterOptions> parameterFilterOptions = new(new ParameterFilterOptions(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected))));
		if (parameterType is not null)
		{
			parameterFilterOptions.Value = parameterFilterOptions.Value.WithPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		}

		return new NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<(Type? ParameterType, string Unexpected, OptionsHolder<CollectionIndexOptions> CollectionIndexOptions, OptionsHolder<ParameterFilterOptions> ParameterFilterOptions), IAsyncEnumerable<MethodInfo?>>((parameterType, unexpected, collectionIndexOptions, parameterFilterOptions),
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
		IReadOnlyOptionsHolder<ParameterFilterOptions> parameterFilterOptions,
		bool exactType = false,
		string? kind = null)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, async method =>
			{
				if (method == null)
				{
					return false;
				}

				ParameterInfo[] parameters = method.GetParameters();
				return await parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.Value.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Value.Matches(p);
				});
			}, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, async method =>
			{
				if (method == null)
				{
					return false;
				}

				ParameterInfo[] parameters = method.GetParameters();
				return await parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.Value.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Value.Matches(p);
				});
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

			stringBuilder.Append(parameterFilterOptions.Value.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Value.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained methods without ").Append(kind ?? "a matching").Append(" parameter");

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

			stringBuilder.Append(parameterFilterOptions.Value.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Value.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained methods with ").Append(kind ?? "a matching").Append(" parameter");
	}

	private sealed class DoNotHaveParameterConstraint(
		string it,
		ExpectationGrammars grammars,
		Type? parameterType,
		string? unexpectedName,
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions,
		IReadOnlyOptionsHolder<ParameterFilterOptions> parameterFilterOptions)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, DoesNotHaveParameter, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, DoesNotHaveParameter, cancellationToken);

		private async ValueTask<bool> DoesNotHaveParameter(MethodInfo? method)
			=> method is not null &&
			   !await method.HasMatchingParameter(collectionIndexOptions.Value, parameterFilterOptions.Value);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendParameterDescription(stringBuilder.Append("all do not have parameter"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained methods with a matching parameter");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendParameterDescription(stringBuilder.Append("also contain a method with parameter"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained methods without a matching parameter");

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

			stringBuilder.Append(parameterFilterOptions.Value.GetModifierDescription());
			stringBuilder.Append(collectionIndexOptions.Value.Match.GetDescription());
		}
	}
}
