using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect;

public static partial class ThatDictionary
{
	extension<TKey, TValue>(IThat<IDictionary<TKey, TValue>?> subject)
	{
		/// <summary>
		///     Expectations on the values of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TValue>?> Values
			=> subject.ForCollectionMember(dictionary => dictionary?.Values, "values");
	}

	extension<TKey, TValue>(IThat<Dictionary<TKey, TValue>?> subject)
		where TKey : notnull
	{
		/// <summary>
		///     Expectations on the values of the dictionary.
		/// </summary>
		public IThat<IEnumerable<TValue>?> Values
			=> subject.ForCollectionMember(dictionary => dictionary?.Values, "values");
	}
}
