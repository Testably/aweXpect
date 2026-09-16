using System.Collections.Generic;
using System.Collections.ObjectModel;
using aweXpect.Core;

namespace aweXpect;

public static partial class ThatReadOnlyDictionary
{
	extension<TKey, TValue>(IThat<IReadOnlyDictionary<TKey, TValue>?> subject)
	{
		/// <summary>
		///     Expectations on the keys of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TKey>?> Keys
			=> subject.ForCollectionMember(dictionary => dictionary?.Keys, "keys");
	}

	extension<TKey, TValue>(IThat<ReadOnlyDictionary<TKey, TValue>?> subject)
		where TKey : notnull
	{
		/// <summary>
		///     Expectations on the keys of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TKey>?> Keys
			=> subject.ForCollectionMember(dictionary => dictionary?.Keys, "keys");
	}
}
