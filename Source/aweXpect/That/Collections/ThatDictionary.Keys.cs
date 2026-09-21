using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using aweXpect.Core;

namespace aweXpect;

public static partial class ThatDictionary
{
	extension<TKey, TValue>(IThat<IDictionary<TKey, TValue>?> subject)
	{
		/// <summary>
		///     Expectations on the keys of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TKey>?> Keys
			=> subject.ForCollectionMember(dictionary => dictionary?.Keys, "keys");
	}

	extension<TKey, TValue>(IThat<Dictionary<TKey, TValue>?> subject)
		where TKey : notnull
	{
		/// <summary>
		///     Expectations on the keys of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TKey>?> Keys
			=> subject.ForCollectionMember(dictionary => dictionary?.Keys, "keys");
	}

	extension<TKey, TValue>(IThat<IReadOnlyDictionary<TKey, TValue>?> subject)
	{
		/// <summary>
		///     Expectations on the keys of the dictionary.
		/// </summary>
		/// <remarks>
		///     Most dictionaries implement both dictionary interfaces, so the two members must share a declaring type for the
		///     priority to decide between them.
		/// </remarks>
		[OverloadResolutionPriority(-1)]
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
