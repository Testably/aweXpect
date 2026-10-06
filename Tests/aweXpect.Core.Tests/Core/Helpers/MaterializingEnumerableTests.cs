using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class MaterializingEnumerableTests
{
	[Test]
	public async Task ReleaseSource_ShouldOnlyReplayTheItemsReadSoFar()
	{
		MaterializingEnumerable<int> materialized =
			(MaterializingEnumerable<int>)MaterializingEnumerable<int>.Wrap(ToEnumerable([1, 2, 3,]));
		_ = materialized.First();

		await materialized.ReleaseSource();
		List<int> result = materialized.ToList();

		await That(result).IsEqualTo([1,])
			.Because("the released source must not be read any further");
		await That(materialized.Count).IsNull()
			.Because("it is unknown how many items the released source has");
	}

	[Test]
	public async Task ReleaseSource_WhenCompletelyIterated_ShouldNotDisposeTheSourceAgain()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2);
		MaterializingEnumerable<int> materialized =
			(MaterializingEnumerable<int>)MaterializingEnumerable<int>.Wrap(source);
		_ = materialized.ToList();

		await materialized.ReleaseSource();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task ReleaseSource_WhenDisposingTheSourceThrows_ShouldNotThrow()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2)
		{
			DisposeException = new InvalidOperationException("dispose failed"),
		};
		MaterializingEnumerable<int> materialized =
			(MaterializingEnumerable<int>)MaterializingEnumerable<int>.Wrap(source);
		_ = materialized.First();

		async Task Act() => await materialized.ReleaseSource();

		await That(Act).DoesNotThrow()
			.Because("the outcome is already decided when the source is released");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task ReleaseSource_WhenPartiallyRead_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2);
		MaterializingEnumerable<int> materialized =
			(MaterializingEnumerable<int>)MaterializingEnumerable<int>.Wrap(source);
		_ = materialized.First();

		await materialized.ReleaseSource();
		await materialized.ReleaseSource();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task Untyped_ReleaseSource_WhenPartiallyRead_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2);
		MaterializingEnumerable materialized =
			(MaterializingEnumerable)MaterializingEnumerable.Wrap(new UntypedEnumerable(source));
		_ = materialized.Cast<object?>().First();

		await materialized.ReleaseSource();
		await materialized.ReleaseSource();
		List<object?> result = materialized.Cast<object?>().ToList();

		await That(source.DisposeCount).IsEqualTo(1);
		await That(result).IsEqualTo([1,])
			.Because("the released source must not be read any further");
	}

	[Test]
	public async Task Untyped_WhenEnumeratedWhileEnumerating_ShouldYieldAllItemsToBoth()
	{
		IEnumerable materialized = MaterializingEnumerable.Wrap(new UntypedEnumerable(ToEnumerable([1, 1, 2,])));
		List<object?> outer = [];
		List<object?> inner = [];

		foreach (object? item in materialized)
		{
			outer.Add(item);
			if (inner.Count == 0)
			{
				inner.AddRange(materialized.Cast<object?>());
			}
		}

		await That(outer).IsEqualTo([1, 1, 2,])
			.Because("the outer enumeration continues after the items that the inner one read");
		await That(inner).IsEqualTo([1, 1, 2,]);
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenEnumeratedWhileEnumerating_ShouldYieldAllItemsToBoth()
	{
		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(ToEnumerable([1, 1, 2,]));
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

	[Test]
	public async Task WhenEnumeratedWhileReplaying_ShouldYieldAllItemsToBoth()
	{
		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(ToEnumerable([1, 1, 2,]));
		_ = materialized.First();
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
			.Because("the items that the inner enumeration adds must not break the replay of the outer one");
		await That(inner).IsEqualTo([1, 1, 2,]);
	}

	[Test]
	public async Task WhenIterating_ShouldReturnAllValues()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(enumerable);

		List<int> result = materialized.ToList();

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Test]
	public async Task WhenSourceThrows_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingEnumerable source = new(new InvalidOperationException("the source is broken"), 1);

		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(source);

		void Act() => _ = materialized.ToList();

		await That(Act).Throws<Exception>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("the source is broken"));
		await That(Act).Throws<Exception>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("the source is broken"))
			.Because("a source that threw is not advanced again, but throws the same exception");
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("a source that threw is not advanced again, so it is released right away");
	}

	[Test]
	public async Task WhenSourceThrows_ShouldThrowTheSameUserCodeExceptionAgain()
	{
		InvalidOperationException exception = new("the source is broken");
		DisposeTrackingEnumerable source = new(exception, 1);
		IEnumerable<int> materialized = MaterializingEnumerable<int>.Wrap(source);
		Exception? first = null;
		Exception? second = null;

		try
		{
			_ = materialized.ToList();
		}
		catch (Exception e)
		{
			first = e;
		}

		try
		{
			_ = materialized.ToList();
		}
		catch (Exception e)
		{
			second = e;
		}

		await That(first).Is<UserCodeException>()
			.Whose(e => e.Thrower, thrower => thrower.IsNull()).And
			.Whose(e => e.InnerException, inner => inner.IsSameAs(exception))
			.Because("an exception of the source fails the expectation like one of the subject");
		await That(second).IsSameAs(first)
			.Because("every further enumeration throws the exception of the source again");
	}

	[Test]
	public async Task Wrap_ForCollection_ShouldUseCollection()
	{
		List<int> collection = new();

		IEnumerable<int> enumerable = MaterializingEnumerable<int>.Wrap(collection);

		await That(enumerable).IsSameAs(collection);
	}

	[Test]
	public async Task Wrap_Twice_ShouldUseSameInstance()
	{
		IEnumerable<int> enumerable = ToEnumerable([1, 2, 3,]);

		IEnumerable<int> materialized1 = MaterializingEnumerable<int>.Wrap(enumerable);
		IEnumerable<int> materialized2 = MaterializingEnumerable<int>.Wrap(materialized1);

		await That(enumerable).IsNotSameAs(materialized1);
		await That(materialized1).IsSameAs(materialized2);
	}

	private sealed class UntypedEnumerable(IEnumerable inner) : IEnumerable
	{
		public IEnumerator GetEnumerator() => inner.GetEnumerator();
	}

	private static IEnumerable<T> ToEnumerable<T>(T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}
}
