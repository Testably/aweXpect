using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class CancellationTests
	{
		[Fact]
		public async Task WhenCancellationIsRequestedDuringTheFinalCheck_ShouldAbortContainsExpectationsInAnyOrder()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<IEnumerable<int>> subject =
				ToEnumerable<IEnumerable<int>>([GetCancellingEnumerable(5, cts), Array.Empty<int>(),]);
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected =
			[
				a => a.IsNotNull(),
				a => a.Contains(-1),
			];

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in any order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [
				               [
				                 0,
				                 1,
				                 2,
				                 3,
				                 4,
				                 5,
				                 6,
				                 7,
				                 8,
				                 9,
				                 (… and maybe more)
				               ],
				               []
				             ]

				             Expected:
				             [
				               an item that is not null,
				               an item that contains an item equal to -1 at least once
				             ]
				             """)
				.Because("the first item is only compared with the second expectation when the items are reassigned at the end, where its cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortAllComplyWithWithinAnItem()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<IEnumerable<int>> subject = ToEnumerable<IEnumerable<int>>(GetCancellingEnumerable(5, cts));

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.DoesNotContain(-1)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to -1 for all items,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [
				               [
				                 0,
				                 1,
				                 2,
				                 3,
				                 4,
				                 5,
				                 6,
				                 7,
				                 8,
				                 9,
				                 (… and maybe more)
				               ],
				               (… and maybe more)
				             ]
				             """)
				.Because("a cancellation within an item must not be reported as a not matching item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortANegatedContains()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).DoesNotContain(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to -1,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContains()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to -1 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsACollection()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains([-1, -2]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [-1, -2] in order and contiguous,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]

				             Expected:
				             [-1, -2]
				             """)
				.Because("a cancellation must not be reported as missing items");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsACollectionForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains([-1, -2]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [-1, -2] in order and contiguous,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]

				             Expected:
				             [-1, -2]
				             """)
				.Because("a cancellation must not be reported as missing items");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsAPredicate()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains(x => x < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item matching x => x < 0 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsAPredicateForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains(x => x is < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item matching x => x is < 0 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsExpectations()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);
			IEnumerable<Action<IThat<int>>> expected = [a => a.IsNegative(),];

			async Task Act()
				=> await That(subject).Contains(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]

				             Expected:
				             [an item that is negative]
				             """)
				.Because("a cancellation must not be reported as missing items");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsExpectationsWithinAnItem()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IEnumerable<IEnumerable<int>> subject =
			[
				GetCancellingEnumerable(5, cts).Select(x =>
				{
					enumeratedCount++;
					return x;
				}),
			];
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected = [a => a.Contains(-1),];

			async Task Act()
				=> await That(subject).Contains(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [
				               [
				                 0,
				                 1,
				                 2,
				                 3,
				                 4,
				                 5,
				                 6,
				                 7,
				                 8,
				                 9,
				                 (… and maybe more)
				               ]
				             ]

				             Expected:
				             [
				               an item that contains an item equal to -1 at least once
				             ]
				             """)
				.Because("a cancellation within an item must not be reported as a missing item");
			await That(enumeratedCount).IsLessThan(100)
				.Because("the item expectation must stop at the cancellation instead of enumerating the whole item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to -1 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsForAnUntypedEnumerableOfNullItems()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IEnumerable subject = GetCancellingEnumerable(5, cts).Select(_ =>
			{
				enumeratedCount++;
				return (object?)null;
			});

			async Task Act()
				=> await That(subject).Contains(1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [
				               <null>,
				               <null>,
				               <null>,
				               <null>,
				               <null>,
				               <null>,
				               (… and maybe more)
				             ]
				             """)
				.Because("a cancellation must not be reported as a missing item");
			await That(enumeratedCount).IsLessThan(100)
				.Because("the collection context must not search the whole source for an item that is not null");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortContainsPredicates()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);
			IEnumerable<Expression<Func<int, bool>>> expected = [a => a < 0,];

			async Task Act()
				=> await That(subject).Contains(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]

				             Expected:
				             [
				               a => (a < 0)
				             ]
				             """)
				.Because("a cancellation must not be reported as missing items");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortDoesNotEndWith()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).DoesNotEndWith([-1]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not end with [-1],
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortDoesNotHaveItem()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).DoesNotHaveItem(x => x < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item matching x => x < 0,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortDoesNotHaveItemThat()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(it => it.IsNegative()).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that is negative,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortEndsWith()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).EndsWith([-1]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             ends with [-1],
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortEndsWithForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).EndsWith([-1]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             ends with [-1],
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItem()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItem(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to -1,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemAtAnIndexFromEnd()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts).Select(x =>
			{
				enumeratedCount++;
				return x;
			});

			async Task Act()
				=> await That(subject).HasItem(-1).AtIndex(2).FromEnd().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to -1 at index 2 from end,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
			await That(enumeratedCount).IsLessThan(100)
				.Because("counting the items must stop at the cancellation as well");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItem(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to -1,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemMatchingAPredicate()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItem(x => x < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item matching x => x < 0,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemMatchingAPredicateAtAnIndexFromEnd()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts).Select(x =>
			{
				enumeratedCount++;
				return x;
			});

			async Task Act()
				=> await That(subject).HasItem(x => x < 0).AtIndex(2).FromEnd().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item matching x => x < 0 at index 2 from end,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
			await That(enumeratedCount).IsLessThan(100)
				.Because("counting the items must stop at the cancellation as well");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemMatchingAPredicateForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItem(x => x is < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item matching x => x is < 0,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemThat()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItemThat(it => it.IsNegative()).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is negative,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemThatAtAnIndexFromEnd()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts).Select(x =>
			{
				enumeratedCount++;
				return x;
			});

			async Task Act()
				=> await That(subject).HasItemThat(it => it.IsNegative()).AtIndex(2).FromEnd().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is negative at index 2 from end,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
			await That(enumeratedCount).IsLessThan(100)
				.Because("counting the items must stop at the cancellation as well");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasItemThatForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasItemThat(it => it.IsEqualTo(-1)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to -1,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasSingle()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasSingle().Matching(x => x < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has a single item matching x => x < 0,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortHasSingleForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).HasSingle().Matching(x => x is < 0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has a single item matching x => x is < 0,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortIsEqualTo()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).IsEqualTo([0, 1, 2, 3, 4, 5, 6, 7]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [0, 1, 2, 3, 4, 5, 6, 7] in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]

				             Expected:
				             [0, 1, 2, 3, 4, 5, 6, 7]
				             """)
				.Because("a cancellation must not be reported as additional items");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortIsEqualToExpectationsWithinAnItem()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<IEnumerable<int>> subject =
				Enumerable.Repeat(Array.Empty<int>(), 20).Append(GetCancellingEnumerable(5, cts));
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected =
				Enumerable.Repeat<Action<IThat<IEnumerable<int>?>>>(a => a.Contains(-1), 21);

			async Task Act()
				=> await That(subject).IsEqualTo(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [
				               [],
				               [],
				               [],
				               [],
				               [],
				               [],
				               [],
				               [],
				               [],
				               [],
				               (… and 11 more)
				             ]

				             Expected:
				             [
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               an item that contains an item equal to -1 at least once,
				               (… and 11 more)
				             ]
				             """)
				.Because("a cancellation within an item must not be reported as a deviation");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortIsInAscendingOrder()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortIsInAscendingOrderForAnUntypedEnumerable()
		{
			using CancellationTokenSource cts = new();
			IEnumerable subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldAbortIsNotInAscendingOrder()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).IsNotInAscendingOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is not in ascending order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("a cancellation must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequested_ShouldReportTheChainedExpectationAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

			async Task Act()
				=> await That(subject).Contains(1).And.Contains(-1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once and contains an item equal to -1 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [0, 1, 2, 3, 4, 5, (… and maybe more)]
				             """)
				.Because("the met expectation cannot explain why the combination could not be verified");
		}

		[Fact]
		public async Task WhenInMemoryCollectionIsCanceled_ShouldBeJudgedCompletely()
		{
			using CancellationTokenSource cts = new();
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).All().Satisfy(x => Cancel(cts, x > 0)).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("a collection that is already complete in memory is judged like any other value");
		}

		[Fact]
		public async Task WhenInMemoryCollectionIsCanceled_ShouldStillReportAMismatchedItemExpectation()
		{
			using CancellationTokenSource cts = new();
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x.Satisfies(v => Cancel(cts, v == 1)), x => x.IsEqualTo(3),])
					.WithCancellation(cts.Token);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x.Satisfies(v => Cancel(cts, v == 1)), x => x.IsEqualTo(3),] in order,
				             but it contained item 2 at index 1 instead of an item that is equal to 3

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies v => Cancel(cts, v == 1), an item that is equal to 3]
				             """)
				.Because("only an item expectation that was not decided may be ignored, not a decided mismatch");
		}

		[Fact]
		public async Task WhenSourceEndedBeforeTheCancellation_ShouldJudgeTheCollection()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int> subject = Enumerable.Range(1, 3).Select(x => x);

			async Task Act()
				=> await That(subject).HasCount(3).And.All().Satisfy(x => Cancel(cts, x > 0))
					.WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the source was read to its end before the cancellation, so all items are known");
		}

		[Fact]
		public async Task WhenTimeoutElapsedBeforeAnInMemoryCollectionIsEvaluated_ShouldJudgeItLikeAScalar()
		{
			async Task Collection()
				=> await That(new[] { 1, 2, })
					.Satisfies(_ => OutlastTheTimeout()).And.IsEqualTo([1, 2,]).WithTimeout(50.Milliseconds());

			async Task Scalar()
				=> await That(5)
					.Satisfies(_ => OutlastTheTimeout()).And.IsEqualTo(5).WithTimeout(50.Milliseconds());

			await That(Scalar).DoesNotThrow();
			await That(Collection).DoesNotThrow()
				.Because("the array is complete when the constraint runs, like the scalar");
		}

		[Fact]
		public async Task WhenTimeoutElapsedBeforeAnInMemoryCollectionIsEvaluated_ShouldReportItsMismatch()
		{
			async Task Act()
				=> await That(new[] { 1, 2, })
					.Satisfies(_ => OutlastTheTimeout()).And.IsEqualTo([1, 3,]).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that new[] { 1, 2, }
				             satisfies _ => OutlastTheTimeout() and is equal to collection [1, 3,] in order,
				             but it contained item 2 at index 1 instead of 3

				             Collection:
				             [1, 2]

				             Expected:
				             [1, 3]
				             """)
				.Because("a complete array is judged normally instead of reporting that it did not finish");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailANegatedContains()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).DoesNotContain(-1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to -1,
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard().And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("a timeout must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailContainsACollection()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).Contains([-1]).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [-1] in order and contiguous,
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]

				             Expected:
				             [-1]
				             """).AsWildcard()
				.Because("a timeout must not be reported as missing items");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailEndsWith()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).EndsWith([-1]).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             ends with [-1],
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard()
				.Because("a timeout must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailHasItem()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).HasItem(x => x < 0).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has an item matching x => x < 0,
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard()
				.Because("a timeout must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailHasSingle()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).HasSingle().Matching(x => x < 0).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a single item matching x => x < 0,
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard()
				.Because("a timeout must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailIsInAscendingOrder()
		{
			IEnumerable<int> subject = SlowNumbers();

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard()
				.Because("a timeout must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenTimeoutElapses_ShouldFailStartsWith()
		{
			IEnumerable<int> subject = SlowNumbers();
			int[] expected = Enumerable.Range(0, 1000).ToArray();

			async Task Act()
				=> await That(subject).StartsWith(expected).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             starts with [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, (… and 990 more)],
				             but it did not finish within 0:00.050

				             Collection:
				             [*0,*(… and maybe more)*]
				             """).AsWildcard().And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("a timeout that elapsed while the items were read must not be ignored, as with EndsWith");
		}

		private static bool Cancel(CancellationTokenSource cts, bool result)
		{
			cts.Cancel();
			return result;
		}

		/// <remarks>
		///     Blocks the evaluation ten times as long as the timeout of 50 ms, so that the following expectations are
		///     evaluated after it elapsed. A delegate subject cannot be used, as a delegate that returns after the timeout
		///     did not finish within it.
		/// </remarks>
		private static bool OutlastTheTimeout()
		{
			Thread.Sleep(500.Milliseconds());
			return true;
		}

		/// <remarks>
		///     Each number takes a millisecond, so that a short timeout elapses during the enumeration. The numbers end
		///     after half a minute, so that a regression which ignores the timeout fails the test instead of hanging the
		///     test run.
		/// </remarks>
		private static IEnumerable<int> SlowNumbers()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			int number = 0;
			while (stopwatch.Elapsed < 30.Seconds())
			{
				Thread.Sleep(1);
				yield return number++;
			}
		}
	}
}
