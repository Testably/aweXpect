using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParameter<TParameter>(
		this IThat<MethodInfo?> subject)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of type <typeparamref name="TParameter" /> with
	///     the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParameter<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, Expected: expected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameter(
		this IThat<MethodInfo?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of type <paramref name="parameterType" /> with
	///     the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameter(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, Expected: expected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameter(
		this IThat<MethodInfo?> subject,
		string expected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)));
		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((Expected: expected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, null, s.Expected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> DoesNotHaveParameter<TParameter>(
		this IThat<MethodInfo?> subject)
		=> DoesNotHaveParameterOfType<TParameter>(subject, typeof(TParameter));

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameter of type <typeparamref name="TParameter" /> with
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> DoesNotHaveParameter<TParameter>(
		this IThat<MethodInfo?> subject, string unexpected)
		=> DoesNotHaveNamedParameter<TParameter>(subject, typeof(TParameter), unexpected);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> DoesNotHaveParameter(
		this IThat<MethodInfo?> subject, Type parameterType)
		=> DoesNotHaveParameterOfType<object?>(subject, parameterType);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameter of type <paramref name="parameterType" /> with
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> DoesNotHaveParameter(
		this IThat<MethodInfo?> subject, Type parameterType, string unexpected)
		=> DoesNotHaveNamedParameter<object?>(subject, parameterType, unexpected);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> DoesNotHaveParameter(
		this IThat<MethodInfo?> subject, string unexpected)
		=> DoesNotHaveNamedParameter<object?>(subject, null, unexpected);

	private static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParameterOfKind(
		IThat<MethodInfo?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(predicate);
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((Kind: kind, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, null, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions,
						kind: s.Kind)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	private static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> DoesNotHaveParameterOfType<TParameter>(
		IThat<MethodInfo?> subject, Type parameterType)
	{
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		return new ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, null,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions).Invert()),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	private static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> DoesNotHaveNamedParameter<TParameter>(
		IThat<MethodInfo?> subject, Type? parameterType, string unexpected)
	{
		OptionsHolder<StringEqualityOptions> stringEqualityOptions = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions = new(CollectionIndexOptions.Default);
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected)));
		if (parameterType is not null)
		{
			parameterFilterOptions.AddPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType));
		}

		return new NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((ParameterType: parameterType, Unexpected: unexpected, CollectionIndexOptions: collectionIndexOptions, ParameterFilterOptions: parameterFilterOptions),
					static (s, it, grammars)
					=> new HasParameterConstraint(it, grammars, s.ParameterType, s.Unexpected,
						s.CollectionIndexOptions,
						s.ParameterFilterOptions).Invert()),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	private sealed class HasParameterConstraint(
		string it,
		ExpectationGrammars grammars,
		Type? parameterType,
		string? expectedName,
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions,
		bool exactType = false,
		string? kind = null)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IAsyncConstraint<MethodInfo?>
	{
		public async ValueTask<ConstraintResult> IsMetBy(MethodInfo? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ParameterInfo[] parameters = actual.GetParameters();
			bool hasParameter = await parameters.AnyAsync(async (p, i) =>
			{
				return collectionIndexOptions.Value.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Matches(p);
			});

			Outcome = hasParameter ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(kind is null ? "has parameter" : $"has {kind} parameter");
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
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(kind is null ? "does not have parameter" : $"does not have {kind} parameter");
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
			=> stringBuilder.Append(It).Append(" did");
	}
}
