using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class MaterializingEnumerableTests
{
	[Fact]
	public async Task WhenCompletelyIterated_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.WrapParameter(source);
		_ = materialized.Any();
		int disposeCountAfterFirstItem = source.DisposeCount;
		List<int> result = materialized.ToList();
		_ = materialized.ToList();

		await That(disposeCountAfterFirstItem).IsEqualTo(0)
			.Because("a partially read source is read further by the next enumeration");
		await That(source.DisposeCount).IsEqualTo(1);
		await That(result).IsEqualTo([1, 2,]);
	}

	[Fact]
	public async Task WhenEnumeratedWhileEnumerating_ShouldYieldAllItemsToBoth()
	{
		IEnumerable<int> materialized = MaterializingEnumerable<int>.WrapParameter(ToEnumerable([1, 1, 2,]));
		List<int> outer = [];
		List<int> inner = [];

		foreach (int item in materialized)
		{
			outer.Add(item);
			if (inner.Count == 0)
			{
				inner.AddRange(materialized);
			}
		}

		await That(outer).IsEqualTo([1, 1, 2,])
			.Because("the outer enumeration continues after the items that the inner one read");
		await That(inner).IsEqualTo([1, 1, 2,]);
	}

	[Fact]
	public async Task WhenIterating_ShouldReturnAllValues()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.WrapParameter(enumerable);

		List<int> result = materialized.ToList();

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Fact]
	public async Task WrapParameter_ForCollection_ShouldUseCollection()
	{
		List<int> collection = new();

		IEnumerable<int> enumerable = MaterializingEnumerable<int>.WrapParameter(collection);

		await That(enumerable).IsSameAs(collection);
	}

	[Fact]
	public async Task WrapParameter_Twice_ShouldUseSameInstance()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized1 = MaterializingEnumerable<int>.WrapParameter(enumerable);
		IEnumerable<int> materialized2 = MaterializingEnumerable<int>.WrapParameter(materialized1);

		await That(enumerable).IsNotSameAs(materialized1);
		await That(materialized1).IsSameAs(materialized2);
	}

	[Fact]
	public async Task WrapParameter_WhenSourceThrows_ShouldDisposeTheSourceOnce()
	{
		InvalidOperationException exception = new("the source is broken");
		DisposeTrackingEnumerable source = new(exception, 1);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.WrapParameter(source);

		void Act() => _ = materialized.ToList();

		await That(Act).Throws<InvalidOperationException>().Which.IsSameAs(exception);
		await That(Act).Throws<InvalidOperationException>().Which.IsSameAs(exception);
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("a source that threw is not advanced again, so it is released right away");
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

	private sealed class DisposeTrackingEnumerable(Exception? exception, params int[] values) : IEnumerable<int>
	{
		public int DisposeCount { get; private set; }

		public IEnumerator<int> GetEnumerator() => new Enumerator(this, exception, values);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class Enumerator(DisposeTrackingEnumerable owner, Exception? exception, int[] values)
			: IEnumerator<int>
		{
			private int _index = -1;

			public int Current => values[_index];

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				if (++_index < values.Length)
				{
					return true;
				}

				if (exception is not null)
				{
					throw exception;
				}

				return false;
			}

			public void Reset() => _index = -1;

			public void Dispose() => owner.DisposeCount++;
		}
	}

	private static IEnumerable<T> ToEnumerable<T>(T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}
}
