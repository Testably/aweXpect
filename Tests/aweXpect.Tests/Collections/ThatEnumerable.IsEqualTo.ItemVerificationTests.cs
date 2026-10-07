using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		/// <summary>
		///     The items are verified in the same way, whether the first item is answered right away or awaited.
		/// </summary>
		public sealed class ItemVerificationTests
		{
			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenAListIsModifiedDuringTheEnumeration_ShouldThrow(bool isFirstItemAwaited)
			{
				List<int> subject = [1, 2, 3,];
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.Satisfies(_ => AddTo(subject)),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<InvalidOperationException>();
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenAllItemsMatch_ShouldSucceedAndDisposeTheEnumeratorOnce(bool isFirstItemAwaited)
			{
				TrackingCollection subject = new([1, 2, 3,]);
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.IsEqualTo(2),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
				await That(subject.Enumerators[0].ReadItems).IsEqualTo(3);
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenAnItemDiffers_ShouldFailAndDisposeTheEnumeratorOnce(bool isFirstItemAwaited)
			{
				TrackingCollection subject = new([1, 2, 3,]);
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.IsEqualTo(2),
					x => x.IsEqualTo(4),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("*but it contained item 3 at index 2 instead of an item that is equal to 4*")
					.AsWildcard();
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenAnItemExpectationThrows_ShouldFailWithTheExceptionAndDisposeTheEnumeratorOnce(
				bool isFirstItemAwaited)
			{
				Exception exception = new NotSupportedException("thrown by the predicate");
				TrackingCollection subject = new([1, 2, 3,]);
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.Satisfies(_ => Throw(exception)),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				FailException failure = await That(Act).Throws<FailException>()
					.WithMessage("*thrown by the predicate*").AsWildcard();
				await That(failure.InnerException).IsSameAs(exception);
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			[Test]
			public async Task WhenAnItemThrowsInEquals_ShouldFailWithTheExceptionAsInnerException()
			{
				Exception exception = new NotSupportedException("thrown by Equals");
				ThrowingEquals[] subject = [new(exception),];
				ThrowingEquals[] expected = [new(exception),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				FailException failure = await That(Act).Throws<FailException>()
					.WithMessage("*thrown by Equals*").AsWildcard();
				await That(failure.InnerException).IsSameAs(exception);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenCanceledBeforeTheEnd_ShouldBeInconclusive(bool isFirstItemAwaited)
			{
				using CancellationTokenSource cts = new();
				IEnumerable<int> subject = GetCancellingEnumerable(5, cts);
				Action<IThat<int>>[] expected = Enumerable.Range(0, 8)
					.Select(value => EqualTo(value, isFirstItemAwaited && value == 0))
					.ToArray();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             *
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [0, 1, 2, 3, 4, 5, (… and maybe more)]
					             *
					             """).AsWildcard();
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenContainedItemsAreFound_ShouldStopAndDisposeTheEnumeratorOnce(bool isFirstItemAwaited)
			{
				TrackingCollection subject = new(Enumerable.Range(1, 100).ToArray());
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.IsEqualTo(2),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
				await That(subject.Enumerators[0].ReadItems).IsEqualTo(2);
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAndDisposeTheEnumeratorOnce(
				bool isFirstItemAwaited)
			{
				Exception exception = new NotSupportedException("thrown by the enumeration");
				TrackingCollection subject = new([1, 2, 3,], 2, exception);
				Action<IThat<int>>[] expected =
				[
					EqualTo(1, isFirstItemAwaited),
					x => x.IsEqualTo(2),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				FailException failure = await That(Act).Throws<FailException>()
					.WithMessage("*thrown by the enumeration*").AsWildcard();
				await That(failure.InnerException).IsSameAs(exception);
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenThereAreTooManyDeviations_ShouldStopAndDisposeTheEnumeratorOnce(
				bool isFirstItemAwaited)
			{
				TrackingCollection subject = new(Enumerable.Range(1, 100).ToArray());
				Action<IThat<int>>[] expected = [EqualTo(1, isFirstItemAwaited),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("*but it had more than 20 deviations:*").AsWildcard();
				await That(subject.Enumerators[0].ReadItems).IsLessThan(100);
				await That(subject.Enumerators[0].DisposeCount).IsEqualTo(1);
			}

			private static Action<IThat<int>> EqualTo(int expected, bool isAwaited)
			{
				if (!isAwaited)
				{
					return x => x.IsEqualTo(expected);
				}

				return x => x.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new AwaitedEqualToConstraint(it, grammars, expected));
			}

			private static bool AddTo(List<int> list)
			{
				list.Add(0);
				return true;
			}

			private static bool Throw(Exception exception)
				=> throw exception;

			private sealed class ThrowingEquals(Exception exception)
			{
				public override bool Equals(object? obj) => throw exception;

				public override int GetHashCode() => 0;
			}

			/// <summary>
			///     Waits before it compares the value, so that it does not complete synchronously.
			/// </summary>
			private sealed class AwaitedEqualToConstraint(string it, ExpectationGrammars grammars, int expected)
				: ConstraintResult.WithValue<int>(it, grammars), IAsyncConstraint<int>
			{
				public async ValueTask<ConstraintResult> IsMetBy(int actual, CancellationToken cancellationToken)
				{
					await Task.Delay(10.Milliseconds(), cancellationToken);
					Actual = actual;
					Outcome = actual == expected ? Outcome.Success : Outcome.Failure;
					return this;
				}

				protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
					=> stringBuilder.Append("is equal to ").Append(expected);

				protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
					=> stringBuilder.Append(It).Append(" was ").Append(Actual);

				protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
					=> stringBuilder.Append("is not equal to ").Append(expected);

				protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
					=> stringBuilder.Append(It).Append(" was");
			}

			/// <summary>
			///     A collection that is enumerated as it is, and that tracks what happens to its enumerators.
			/// </summary>
			private sealed class TrackingCollection(int[] items, int throwAfter = int.MaxValue, Exception? exception = null)
				: ICollection<int>, ICollection
			{
				public List<TrackingEnumerator> Enumerators { get; } = [];

				public bool IsSynchronized => false;

				public object SyncRoot => this;

				public void CopyTo(Array array, int index) => items.CopyTo(array, index);

				public int Count => items.Length;

				public bool IsReadOnly => true;

				public IEnumerator<int> GetEnumerator()
				{
					TrackingEnumerator enumerator = new(items, throwAfter, exception);
					Enumerators.Add(enumerator);
					return enumerator;
				}

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				public bool Contains(int item) => items.Contains(item);

				public void CopyTo(int[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

				public void Add(int item) => throw new NotSupportedException();

				public void Clear() => throw new NotSupportedException();

				public bool Remove(int item) => throw new NotSupportedException();
			}

			private sealed class TrackingEnumerator(int[] items, int throwAfter, Exception? exception) : IEnumerator<int>
			{
				public int DisposeCount { get; private set; }

				public int ReadItems { get; private set; }

				public int Current => items[ReadItems - 1];

				object IEnumerator.Current => Current;

				public bool MoveNext()
				{
					if (ReadItems == throwAfter)
					{
						throw exception!;
					}

					if (ReadItems == items.Length)
					{
						return false;
					}

					ReadItems++;
					return true;
				}

				public void Reset() => throw new NotSupportedException();

				public void Dispose() => DisposeCount++;
			}
		}
	}
}
