using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.SourceGenerators;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IDictionary{TKey,TValue}" /> and <see cref="IReadOnlyDictionary{TKey,TValue}" />.
/// </summary>
[CollectionSubjects(
	"System.Collections.Generic.IDictionary<TKey, TValue>",
	"System.Collections.Generic.Dictionary<TKey, TValue>",
	"System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>")]
[CollectionSubjects("System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>",
	Priority = -1, Remarks = SharedDeclaringTypeRemarks)]
public static partial class ThatDictionary
{
	private const string ExpectedDictionaryWasNull = "the expected dictionary was <null>";

	private const string SharedDeclaringTypeRemarks =
		"Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the\n" +
		"priority to decide between them.";

	private const string NegatedKeyReturnType =
		"global::aweXpect.Results.AndOrResult<TCollection, global::aweXpect.Core.IThat<TCollection?>>";

	/// <summary>
	///     Compares the values with the equality <paramref name="options" /> of the expectation, as values, unlike keys,
	///     have no comparer of the dictionary to honour.
	/// </summary>
	/// <remarks>
	///     Only the enumeration of the dictionary is called as code of the caller, so that its exception fails the
	///     expectation, while the comparison of the values reports its own.
	/// </remarks>
	private static async Task<bool> ContainsValue<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
		TValue value, IOptionsEquality<TValue> options)
	{
		using IEnumerator<KeyValuePair<TKey, TValue>> entries =
			UserCode.Invoke(static subject => subject.GetEnumerator(), dictionary);
		while (UserCode.Invoke(static enumerator => enumerator.MoveNext(), entries))
		{
			if (await options.AreConsideredEqual(entries.Current.Value, value))
			{
				return true;
			}
		}

		return false;
	}

	private static ValueLookup<TKey, TValue> GetLookup<TKey, TValue>(
		IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		=> dictionary is IDictionary<TKey, TValue> mutableDictionary
			? mutableDictionary.TryGetValue
			: ((IReadOnlyDictionary<TKey, TValue>)dictionary).TryGetValue;

	/// <summary>
	///     Looks the <paramref name="key" /> up with the <paramref name="lookup" /> of the dictionary as code of the
	///     caller, so that an exception of the dictionary or of its key comparer fails the expectation.
	/// </summary>
	private static bool TryLookUp<TKey, TValue>(ValueLookup<TKey, TValue> lookup, TKey key, out TValue? value)
	{
		(bool hasKey, value) = UserCode.Invoke(
			static values => (values.Lookup(values.Key, out TValue? found), found), (Lookup: lookup, Key: key));
		return hasKey;
	}

	/// <summary>
	///     Asks the dictionary itself for the <paramref name="key" />, so that its key comparer decides.
	/// </summary>
	/// <remarks>
	///     The dictionary is asked as code of the caller, so that an exception of it or of its key comparer fails the
	///     expectation.
	/// </remarks>
	private static bool ContainsKey<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, TKey key)
		=> UserCode.Invoke(
			static values => values.Dictionary is IDictionary<TKey, TValue> mutableDictionary
				? mutableDictionary.ContainsKey(values.Key)
				: ((IReadOnlyDictionary<TKey, TValue>)values.Dictionary).ContainsKey(values.Key),
			(Dictionary: dictionary, Key: key));

	/// <summary>
	///     Adds the "Dictionary" context for the <paramref name="dictionary" />.
	/// </summary>
	private static void AddDictionaryContext<TKey, TValue>(ResultContextCollector contexts,
		IEnumerable<KeyValuePair<TKey, TValue>>? dictionary)
	{
		CollectionContext context = default;
		context.SetDictionary(dictionary);
		context.AppendTo(contexts);
	}

	/// <summary>
	///     Looks the <paramref name="key" /> up through the dictionary itself, so that its key comparer decides.
	/// </summary>
	private delegate bool ValueLookup<in TKey, TValue>(TKey key, out TValue? value);
}
