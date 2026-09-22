using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	private const string ContainsEntrySummary = "Verifies that the dictionary contains the <paramref name=\"expected\" /> entry.";

	private const string DoesNotContainEntrySummary =
		"Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> entry.";

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

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	internal static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		ContainsCore<TKey, TValue>(
			IThat<IDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> expected,
			bool negated)
		=> ContainsEntry(subject, expected, negated);

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary, Remarks = SharedDeclaringTypeRemarks)]
	internal static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		ContainsForReadOnlyCore<TKey, TValue>(
			IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			KeyValuePair<TKey, TValue> expected,
			bool negated)
		=> ContainsEntry(subject, expected, negated);

	private static ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>
		ContainsEntry<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, TValue> expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
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
