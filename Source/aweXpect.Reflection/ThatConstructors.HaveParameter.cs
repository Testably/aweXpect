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
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, TParameter>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(
			p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>?, object?>(subject.Get()
				.ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
					=> new HaveParameterConstraint(it, grammars, null, expected,
						collectionIndexOptions,
						parameterFilterOptions)),
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
		CollectionIndexOptions collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions,
		bool exactType = false)
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
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => true, // No index constraint means all indices are valid
					};
					return isIndexInRange != false && await parameterFilterOptions.Matches(p);
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
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => true, // No index constraint means all indices are valid
					};
					return isIndexInRange != false && await parameterFilterOptions.Matches(p);
				});
				return hasParameter;
			}, cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all have parameter");
			if (parameterType != null)
			{
				stringBuilder.Append(exactType ? " of exact type " : " of type ").Append(Formatter.Format(parameterType));
			}

			if (expectedName != null)
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(expectedName));
			}

			stringBuilder.Append(parameterFilterOptions.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained constructors without a matching parameter");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all have parameter");
			if (parameterType != null)
			{
				stringBuilder.Append(exactType ? " of exact type " : " of type ").Append(Formatter.Format(parameterType));
			}

			if (expectedName != null)
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(expectedName));
			}

			stringBuilder.Append(parameterFilterOptions.GetModifierDescription());

			string indexDescription = collectionIndexOptions.Match.GetDescription();
			if (!string.IsNullOrEmpty(indexDescription))
			{
				stringBuilder.Append(indexDescription);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained constructors with a matching parameter");
	}
}
