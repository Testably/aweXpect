using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class SortedKeysTests
{
	[Test]
	public async Task WhenEnumeratedWithoutItemType_ShouldYieldTheKeys()
	{
		IEnumerable keys = new SortedKeys<int>([2, 1,], Comparer<int>.Default);

		List<object?> items = keys.Cast<object?>().ToList();

		await That(items).IsEqualTo([2, 1,])
			.Because("the keys keep the order of the dictionary");
	}
}
