using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class MaterializingEnumerableTests
{
	[Fact]
	public async Task Untyped_WhenSourceThrows_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(new InvalidOperationException("the source is broken"), 1);

		IEnumerable materialized = MaterializingEnumerable.Wrap(source);

		void Act() => _ = materialized.Cast<object?>().ToList();

		await That(Act).Throws<Exception>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("the source is broken"));
		await That(Act).Throws<Exception>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("the source is broken"));
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("a source that threw is not advanced again, so it is released right away");
	}

	[Fact]
	public async Task WhenCompletelyIterated_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(source);
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
