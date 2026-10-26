using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <typeparamref name="TParameter" /> with
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, Expected: expected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <paramref name="parameterType" /> with
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, Expected: expected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
}
