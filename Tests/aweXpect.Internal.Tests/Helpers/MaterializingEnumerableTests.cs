using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class MaterializingEnumerableTests
{
	[Fact]
	public async Task WhenIterating_ShouldReturnAllValues()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(enumerable);

		List<int> result = materialized.ToList();

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Fact]
	public async Task Wrap_ForCollection_ShouldUseCollection()
	{
		List<int> collection = new();

		IEnumerable<int> enumerable = MaterializingEnumerable<int>.Wrap(collection);

		await That(enumerable).IsSameAs(collection);
	}

	[Fact]
	public async Task Wrap_Twice_ShouldUseSameInstance()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized1 = MaterializingEnumerable<int>.Wrap(enumerable);
		IEnumerable<int> materialized2 = MaterializingEnumerable<int>.Wrap(materialized1);

		await That(enumerable).IsNotSameAs(materialized1);
		await That(materialized1).IsSameAs(materialized2);
	}

	[Fact]
	public async Task WrapParameter_WhenSourceThrows_ShouldThrowTheSameExceptionOnEveryEnumeration()
	{
		InvalidOperationException exception = new("the source is broken");

		IEnumerable<int> GetSource()
		{
			yield return 1;
			throw exception;
		}

		IEnumerable<int> materialized = MaterializingEnumerable<int>.WrapParameter(GetSource());

		void Act() => _ = materialized.ToList();

		await That(Act).Throws<InvalidOperationException>().Which.IsSameAs(exception);
		await That(Act).Throws<InvalidOperationException>().Which.IsSameAs(exception)
			.Because("a source that threw is not advanced again, but throws the same exception");
	}

	private static IEnumerable<T> ToEnumerable<T>(T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}
}
