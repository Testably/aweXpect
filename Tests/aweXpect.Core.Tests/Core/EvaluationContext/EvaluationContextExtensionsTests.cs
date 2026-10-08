using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core.TimeSystem;
using aweXpect.Signaling;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationContextExtensionsTests
{
	[Test]
	public async Task GetElapsedTime_ShouldMeasureOnTheTimeSystemOfTheEvaluation()
	{
		VirtualTimeSystem timeSystem = new();
		IEvaluationContext context = new Context
		{
			TimeSystem = timeSystem,
		};
		timeSystem.Advance(10.Seconds());

		long timestamp = context.GetTimestamp();
		timeSystem.Advance(30.Seconds());
		TimeSpan result = context.GetElapsedTime(timestamp);

		await That(result).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task GetTimestamp_ForAContextOfAnotherImplementation_ShouldBeOfTheRealTimeSystem()
	{
		IEvaluationContext context = new ForeignEvaluationContext();
		long before = RealTimeSystem.Instance.GetTimestamp();

		long timestamp = context.GetTimestamp();
		TimeSpan elapsed = context.GetElapsedTime(timestamp);

		await That(timestamp).IsGreaterThanOrEqualTo(before).And
			.IsLessThanOrEqualTo(RealTimeSystem.Instance.GetTimestamp())
			.Because("only an evaluation of this library has a time system of its own");
		await That(elapsed).IsGreaterThanOrEqualTo(TimeSpan.Zero);
	}

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
	public async Task UseMaterializedEnumerable_ForDifferentSourcesThatAreEqual_ShouldReturnDifferentInstances()
	{
		IEvaluationContext context = new Context();

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(new AlwaysEqualEnumerable(1, 2));
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(new AlwaysEqualEnumerable(3, 4));

		await That(materialized1).IsNotSameAs(materialized2)
			.Because("a source is identified by its reference, not by its equality");
		await That(materialized2.ToArray()).IsEqualTo([3, 4,]);
	}

	[Test]
	public async Task UseMaterializedEnumerable_ForEqualValueTypeSources_ShouldReturnSameInstance()
	{
		IEvaluationContext context = new Context();
		IEnumerable<int> items = ToEnumerable(1, 2);

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(new ValueTypeEnumerable(items));
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(new ValueTypeEnumerable(items));

		await That(materialized1).IsSameAs(materialized2)
			.Because("a value type is boxed anew for every call, so it has no reference that identifies it");
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
	public async Task UseMaterializedEnumerable_WhenEqualsOfADifferentSourceThrows_ShouldNotCallIt()
	{
		IEvaluationContext context = new Context();
		ThrowingEqualsEnumerable source1 = new(1, 2);
		ThrowingEqualsEnumerable source2 = new(3, 4);

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(source1);
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(source2);
		IEnumerable<int> materializedAgain = context.UseMaterializedEnumerable(source1);

		await That(materialized1).IsNotSameAs(materialized2);
		await That(materializedAgain).IsSameAs(materialized1);
	}

	[Test]
	public async Task UseMaterializedEnumerable_WhenEqualsOfAValueTypeSourceThrows_ShouldMaterializeItAgain()
	{
		IEvaluationContext context = new Context();
		IEnumerable<int> items = ToEnumerable(1, 2);

		IEnumerable<int> materialized1 = context.UseMaterializedEnumerable(new ThrowingEqualsValueTypeEnumerable(items));
		IEnumerable<int> materialized2 = context.UseMaterializedEnumerable(new ThrowingEqualsValueTypeEnumerable(items));

		await That(materialized1).IsNotSameAs(materialized2)
			.Because("a source whose equality cannot be asked is not known to be the same");
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

	[Test]
	public async Task WaitForSignalsAsync_NegativeTimeout_ShouldThrowArgumentOutOfRangeException()
	{
		Signaler signaler = new();

		void Act()
			=> _ = new Context().WaitForSignalsAsync(signaler, 2.Times(), -2.Milliseconds(), CancellationToken.None);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WaitForSignalsAsync_ShouldCompleteAsSoonAsEnoughSignalsWereRecorded()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};
		Signaler signaler = new();
		signaler.Signal();

		SignalerResult result =
			await context.WaitForSignalsAsync(signaler, 1.Times(), 30.Seconds(), CancellationToken.None);

		await That(result.IsSuccess).IsTrue();
		await That(timeSystem.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForSignalsAsync_ShouldLetTheTimeoutExpireOnTheTimeSystemOfTheEvaluation()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};
		Signaler signaler = new();

		SignalerResult result =
			await context.WaitForSignalsAsync(signaler, 1.Times(), 30.Seconds(), CancellationToken.None);

		await That(result.IsSuccess).IsFalse();
		await That(timeSystem.Now).IsEqualTo(30.Seconds())
			.Because("the wait ends when the virtual clock of the evaluation reaches the timeout");
	}

	[Test]
	public async Task WaitForSignalsAsync_WhenSignalerIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = new Context().WaitForSignalsAsync(null!, 1.Times(), 30.Seconds(), CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("signaler").And
			.WithMessage("The 'signaler' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WaitForSignalsAsync_WithParameter_NegativeTimeout_ShouldThrowArgumentOutOfRangeException()
	{
		Signaler<int> signaler = new();

		void Act()
			=> _ = new Context().WaitForSignalsAsync(signaler, 2.Times(), null, -2.Milliseconds(),
				CancellationToken.None);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WaitForSignalsAsync_WithParameter_ShouldLetTheTimeoutExpireOnTheTimeSystemOfTheEvaluation()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};
		Signaler<int> signaler = new();
		signaler.Signal(1);

		SignalerResult<int> result = await context.WaitForSignalsAsync(signaler, 1.Times(), p => p > 1,
			30.Seconds(), CancellationToken.None);

		await That(result.IsSuccess).IsFalse();
		await That(result.Parameters).IsEqualTo([1,]);
		await That(timeSystem.Now).IsEqualTo(30.Seconds())
			.Because("the wait ends when the virtual clock of the evaluation reaches the timeout");
	}

	[Test]
	public async Task WaitForSignalsAsync_WithParameter_WhenSignalerIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = new Context().WaitForSignalsAsync<int>(null!, 1.Times(), null, 30.Seconds(),
				CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("signaler").And
			.WithMessage("The 'signaler' cannot be null.").AsPrefix();
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task WaitForSignalsAsync_WithParameter_ZeroOrNegativeAmount_ShouldThrowArgumentOutOfRangeException(
		int amount)
	{
		Signaler<int> signaler = new();

		void Act()
			=> _ = new Context().WaitForSignalsAsync(signaler, amount, null, 30.Seconds(), CancellationToken.None);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The amount must be greater than zero*").AsWildcard().And
			.WithParamName("amount");
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task WaitForSignalsAsync_ZeroOrNegativeAmount_ShouldThrowArgumentOutOfRangeException(int amount)
	{
		Signaler signaler = new();

		void Act()
			=> _ = new Context().WaitForSignalsAsync(signaler, amount, 30.Seconds(), CancellationToken.None);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The amount must be greater than zero*").AsWildcard().And
			.WithParamName("amount");
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

	private sealed class AlwaysEqualEnumerable(params int[] items) : IEnumerable<int>
	{
		public IEnumerator<int> GetEnumerator() => items.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object? obj) => obj is AlwaysEqualEnumerable;

		public override int GetHashCode() => 0;
	}

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

	private sealed class ThrowingEqualsEnumerable(params int[] items) : IEnumerable<int>
	{
		public IEnumerator<int> GetEnumerator() => items.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object? obj) => throw new InvalidOperationException("Equals failed");

		public override int GetHashCode() => 0;
	}

	private readonly struct ThrowingEqualsValueTypeEnumerable(IEnumerable<int> items) : IEnumerable<int>
	{
		public IEnumerator<int> GetEnumerator() => items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object? obj) => throw new InvalidOperationException("Equals failed");

		public override int GetHashCode() => 0;
	}

	private sealed class UntypedEnumerable(IEnumerable inner) : IEnumerable
	{
		public IEnumerator GetEnumerator() => inner.GetEnumerator();
	}

	private sealed class ForeignEvaluationContext : IEvaluationContext
	{
		public EvaluationCancellation Cancellation => EvaluationCancellation.None;

		public void Store<T>(string key, T value) { }

		public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
		{
			value = default;
			return false;
		}
	}

	private readonly struct ValueTypeEnumerable(IEnumerable<int> items) : IEnumerable<int>
	{
		public IEnumerator<int> GetEnumerator() => items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
