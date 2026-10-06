#if NET8_0_OR_GREATER
using System.Threading;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.EvaluationContext;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationContextExtensionsTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_ForDifferentSources_ShouldReturnDifferentInstances()
	{
		IEvaluationContext context = new Context();

		IAsyncEnumerable<int> materialized1 =
			context.UseMaterializedAsyncEnumerable(ToAsyncEnumerable(1, 2), CancellationToken.None);
		IAsyncEnumerable<int> materialized2 =
			context.UseMaterializedAsyncEnumerable(ToAsyncEnumerable(1, 2), CancellationToken.None);

		await That(materialized1).IsNotSameAs(materialized2)
			.Because("nested expectations evaluate different sources in the same context");
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_ForSameSource_ShouldReturnSameInstance()
	{
		IEvaluationContext context = new Context();
		IAsyncEnumerable<int> source = ToAsyncEnumerable(1, 2);

		IAsyncEnumerable<int> materialized1 = context.UseMaterializedAsyncEnumerable(source, CancellationToken.None);
		IAsyncEnumerable<int> materialized2 = context.UseMaterializedAsyncEnumerable(source, CancellationToken.None);

		await That(materialized1).IsSameAs(materialized2);
		await That(materialized1).IsNotSameAs(source);
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_WhenCalledAgainWithAnotherToken_ShouldKeepTheTokenOfTheFirstCall()
	{
		using CancellationTokenSource cts = new();
		IEvaluationContext context = new Context();
		IAsyncEnumerable<int> source = ToAsyncEnumerable(1, 2);
		_ = context.UseMaterializedAsyncEnumerable(source, cts.Token);
		IAsyncEnumerable<int> materialized = context.UseMaterializedAsyncEnumerable(source, CancellationToken.None);
		await cts.CancelAsync();

		async Task Act()
		{
			await foreach (int _ in materialized)
			{
			}
		}

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage(new OperationCanceledException().Message)
			.Because("the source is governed by the token of the call that materialized it");
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_WhenCompletelyEnumerated_ShouldKnowTheCount()
	{
		IEvaluationContext context = new Context();

		IAsyncEnumerable<int> materialized =
			context.UseMaterializedAsyncEnumerable(ToAsyncEnumerable(1, 2, 3), CancellationToken.None);
		await foreach (int _ in materialized)
		{
		}

		await That(materialized).Is<IMaterializedAsyncEnumerable<int>>()
			.Whose(m => m.Count, count => count.IsEqualTo(3))
			.AndWhose(m => m.MaterializedItems, items => items.IsEqualTo([1, 2, 3,]));
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_WhenNull_ShouldThrowArgumentNullException()
	{
		IEvaluationContext context = new Context();

		void Act() => context.UseMaterializedAsyncEnumerable<int>(null!, CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("collection").And
			.WithMessage("The 'collection' cannot be null.").AsPrefix();
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task UseMaterializedAsyncEnumerable_WithOneShotSource_ShouldEnumerateTheSourceOnlyOnce()
	{
		IEvaluationContext context = new Context();
		OneShotAsyncEnumerable source = new(2, 4, 6);
		List<int> items1 = [];
		List<int> items2 = [];

		await foreach (int item in context.UseMaterializedAsyncEnumerable(source, CancellationToken.None))
		{
			items1.Add(item);
			break;
		}

		await foreach (int item in context.UseMaterializedAsyncEnumerable(source, CancellationToken.None))
		{
			items2.Add(item);
		}

		await That(items1).IsEqualTo([2,]);
		await That(items2).IsEqualTo([2, 4, 6,])
			.Because("the second consumer replays the first item and continues the source where the first one stopped");
		await That(source.Enumerations).IsEqualTo(1);
	}
#endif

	[Test]
	public async Task UseMaterializedEnumerable_ForCollection_ShouldReturnTheCollection()
	{
		IEvaluationContext context = new Context();
		List<int> collection = [1, 2,];

		IEnumerable<int> materialized = context.UseMaterializedEnumerable(collection);

		await That(materialized).IsSameAs(collection)
			.Because("a collection can be enumerated repeatedly");
	}

	[Test]
	public async Task UseMaterializedEnumerable_ForDifferentSources_ShouldReturnDifferentInstances()
	{
		IEvaluationContext context = new Context();

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(ToEnumerable(1, 2));
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(ToEnumerable(1, 2));

		await That(materialized1).IsNotSameAs(materialized2)
			.Because("nested expectations evaluate different sources in the same context");
	}

	[Test]
	public async Task UseMaterializedEnumerable_ForSameSource_ShouldReturnSameInstance()
	{
		IEvaluationContext context = new Context();
		IEnumerable<int> source = ToEnumerable(1, 2);

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(source);
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(source);

		await That(materialized1).IsSameAs(materialized2);
		await That(materialized1).IsNotSameAs(source);
	}

	[Test]
	public async Task UseMaterializedEnumerable_InDifferentContexts_ShouldReturnDifferentInstances()
	{
		IEnumerable<int> source = ToEnumerable(1, 2);

		IEnumerable<int> materialized1 = new Context().UseMaterializedEnumerable(source);
		IEnumerable<int> materialized2 = new Context().UseMaterializedEnumerable(source);

		await That(materialized1).IsNotSameAs(materialized2)
			.Because("every evaluation materializes the source anew");
	}

	[Test]
	public async Task UseMaterializedEnumerable_Untyped_ForNull_ShouldReturnNull()
	{
		IEvaluationContext context = new Context();

		IEnumerable? materialized = context.UseMaterializedEnumerable(null);

		await That(materialized).IsNull();
	}

	[Test]
	public async Task UseMaterializedEnumerable_Untyped_ForSameSource_ShouldReturnSameInstance()
	{
		IEvaluationContext context = new Context();
		IEnumerable source = new UntypedEnumerable(ToEnumerable(1, 2));

		IEnumerable? materialized1 = context.UseMaterializedEnumerable(source);
		IEnumerable? materialized2 = context.UseMaterializedEnumerable(source);

		await That(materialized1).IsSameAs(materialized2);
		await That(materialized1).IsNotSameAs(source);
	}

	[Test]
	public async Task UseMaterializedEnumerable_Untyped_WhenPartiallyEnumerated_ShouldKeepTheReadItems()
	{
		IEvaluationContext context = new Context();
		IEnumerable untypedSource = new UntypedEnumerable(new OneShotEnumerable(2, 4, 6));

		IEnumerable materialized = context.UseMaterializedEnumerable(untypedSource)!;
		_ = materialized.Cast<object?>().Take(2).ToList();

		await That(((IMaterializedEnumerable)materialized).MaterializedItems).IsEqualTo([2, 4,])
			.Because("only the items that were read are kept, without reading further items");
	}

	[Test]
	public async Task UseMaterializedEnumerable_Untyped_WithOneShotSource_ShouldEnumerateTheSourceOnlyOnce()
	{
		IEvaluationContext context = new Context();
		OneShotEnumerable source = new(2, 4, 6);
		IEnumerable untypedSource = new UntypedEnumerable(source);

		List<object?> items1 = context.UseMaterializedEnumerable(untypedSource)!.Cast<object?>().Take(1).ToList();
		List<object?> items2 = context.UseMaterializedEnumerable(untypedSource)!.Cast<object?>().ToList();

		await That(items1).IsEqualTo([2,]);
		await That(items2).IsEqualTo([2, 4, 6,])
			.Because("the second consumer replays the first item and continues the source where the first one stopped");
		await That(source.Enumerations).IsEqualTo(1);
	}

	[Test]
	public async Task UseMaterializedEnumerable_WhenCompletelyEnumerated_ShouldKnowTheCount()
	{
		IEvaluationContext context = new Context();

		IEnumerable<int> materialized = context.UseMaterializedEnumerable(ToEnumerable(1, 2, 3));
		int? countBefore = (materialized as ICountable)?.Count;
		_ = materialized.ToList();

		await That(countBefore).IsNull()
			.Because("the number of items is unknown until the source is enumerated completely");
		await That(materialized).Is<ICountable>().Whose(c => c.Count, count => count.IsEqualTo(3));
	}

	[Test]
	public async Task UseMaterializedEnumerable_WhenNull_ShouldThrowArgumentNullException()
	{
		IEvaluationContext context = new Context();

		void Act() => context.UseMaterializedEnumerable<int>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("collection").And
			.WithMessage("The 'collection' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task UseMaterializedEnumerable_WhenPartiallyEnumerated_ShouldKeepTheReadItems()
	{
		IEvaluationContext context = new Context();
		OneShotEnumerable source = new(2, 4, 6);

		IEnumerable<int> materialized = context.UseMaterializedEnumerable(source);
		_ = materialized.Take(2).ToList();

		await That(((IMaterializedEnumerable<int>)materialized).MaterializedItems).IsEqualTo([2, 4,])
			.Because("only the items that were read are kept, without reading further items");
		await That(((IMaterializedEnumerable)materialized).MaterializedItems).IsEqualTo([2, 4,]);
		await That(((ICountable)materialized).Count).IsNull();
	}

	[Test]
	public async Task UseMaterializedEnumerable_WithOneShotSource_ShouldEnumerateTheSourceOnlyOnce()
	{
		IEvaluationContext context = new Context();
		OneShotEnumerable source = new(2, 4, 6);

		List<int> items1 = context.UseMaterializedEnumerable(source).Take(1).ToList();
		List<int> items2 = context.UseMaterializedEnumerable(source).ToList();

		await That(items1).IsEqualTo([2,]);
		await That(items2).IsEqualTo([2, 4, 6,])
			.Because("the second consumer replays the first item and continues the source where the first one stopped");
		await That(source.Enumerations).IsEqualTo(1);
	}

	private static IEnumerable<T> ToEnumerable<T>(params T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}

#if NET8_0_OR_GREATER
	private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(params T[] items)
	{
		foreach (T item in items)
		{
			await Task.Yield();
			yield return item;
		}
	}
#endif

	private sealed class OneShotEnumerable(params int[] items) : IEnumerable<int>
	{
		public int Enumerations { get; private set; }

		public IEnumerator<int> GetEnumerator()
		{
			Enumerations++;
			return (Enumerations == 1 ? items : []).AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

#if NET8_0_OR_GREATER
	private sealed class OneShotAsyncEnumerable(params int[] items) : IAsyncEnumerable<int>
	{
		public int Enumerations { get; private set; }

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			Enumerations++;
			return ToAsyncEnumerable(Enumerations == 1 ? items : []).GetAsyncEnumerator(cancellationToken);
		}
	}
#endif

	private sealed class UntypedEnumerable(IEnumerable inner) : IEnumerable
	{
		public IEnumerator GetEnumerator() => inner.GetEnumerator();
	}
}
