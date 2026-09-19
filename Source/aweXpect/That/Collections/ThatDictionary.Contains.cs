using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expectedKey" /> with the
	///     <paramref name="expectedValue" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		Contains<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			TKey expectedKey,
			TValue expectedValue)
		=> subject.Contains(new KeyValuePair<TKey, TValue>(expectedKey, expectedValue));

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> entry.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		Contains<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> expected)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expectedKey" /> with the
	///     <paramref name="expectedValue" />.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		Contains<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TKey expectedKey,
			TValue expectedValue)
		=> subject.Contains(new KeyValuePair<TKey, TValue>(expectedKey, expectedValue));

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> entry.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		Contains<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> expected)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpectedKey" /> with the
	///     <paramref name="unexpectedValue" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		DoesNotContain<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			TKey unexpectedKey,
			TValue unexpectedValue)
		=> subject.DoesNotContain(new KeyValuePair<TKey, TValue>(unexpectedKey, unexpectedValue));

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> entry.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		DoesNotContain<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> unexpected)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpectedKey" /> with the
	///     <paramref name="unexpectedValue" />.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		DoesNotContain<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TKey unexpectedKey,
			TValue unexpectedValue)
		=> subject.DoesNotContain(new KeyValuePair<TKey, TValue>(unexpectedKey, unexpectedValue));

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> entry.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		DoesNotContain<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> unexpected)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class ContainsConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		KeyValuePair<TKey, TValue> expected,
		ObjectEqualityOptions<TValue> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private TValue? _actualValue;
		private bool _hasKey;

		public async Task<ConstraintResult> IsMetBy(TDictionary? actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_hasKey = GetLookup(actual)(expected.Key, out _actualValue);
			Outcome = _hasKey && await options.AreConsideredEqual(_actualValue!, expected.Value)
				? Outcome.Success
				: Outcome.Failure;
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains ", "contain "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_hasKey)
			{
				stringBuilder.Append(It).Append(" did not contain the key ");
				Formatter.Format(stringBuilder, expected.Key);
				return;
			}

			stringBuilder.Append(It).Append(" contained the key ");
			Formatter.Format(stringBuilder, expected.Key);
			stringBuilder.Append(" with value ");
			Formatter.Format(stringBuilder, _actualValue);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain ", "do not contain "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
