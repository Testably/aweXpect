using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeysResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			params TKey[] expected)
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject,
			expected,
			dictionary => new KeyedValues<TKey, TValue?>(expected
				.Where(key => key is not null && dictionary.ContainsKey(key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key, dictionary[key])))
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeysResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			params TKey[] expected)
		where TKey : notnull
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject,
			expected,
			dictionary => new KeyedValues<TKey, TValue?>(expected
				.Where(key => key is not null && dictionary.ContainsKey(key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key, dictionary[key])))
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ContainsKeysResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TKey, TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			params TKey[] expected)
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject,
			expected,
			dictionary => new KeyedValues<TKey, TValue?>(expected
				.Where(key => key is not null && dictionary.ContainsKey(key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key, dictionary[key])))
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeysResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey,
			TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			params TKey[] expected)
		where TKey : notnull
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey
			, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject,
			expected,
			dictionary => new KeyedValues<TKey, TValue?>(expected
				.Where(dictionary.ContainsKey)
				.Select(key => new KeyValuePair<TKey, TValue?>(key, dictionary[key])))
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			params TKey[] unexpected)
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			params TKey[] unexpected)
		where TKey : notnull
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			params TKey[] unexpected)
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			params TKey[] unexpected)
		where TKey : notnull
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	private sealed class ContainKeysConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TKey[] expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private List<TKey>? _existingKeys;
		private List<TKey>? _missingKeys;

		public ConstraintResult IsMetBy(TDictionary? actual)
		{
			Actual = actual;
			if (actual is not null)
			{
				_missingKeys = [];
				_existingKeys = [];
				foreach (TKey item in expected)
				{
					if (ContainsKey(actual, item))
					{
						_existingKeys.Add(item);
					}
					else
					{
						_missingKeys.Add(item);
					}
				}
			}

			Outcome = (IsNegated, _missingKeys, _existingKeys) switch
			{
				(true, _, []) => Outcome.Failure,
				(true, _, _) => Outcome.Success,
				(false, [], _) => Outcome.Success,
				(false, _, _) => Outcome.Failure,
			};
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains keys ", "contain keys "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingKeys, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain keys ", "do not contain keys "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain ");
			Formatter.Format(stringBuilder, _existingKeys, FormattingOptions.MultipleLines);
		}
	}
}
