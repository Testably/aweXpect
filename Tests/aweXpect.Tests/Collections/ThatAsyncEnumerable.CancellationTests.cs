#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class CancellationTests
	{
		[Test]
		public async Task WhenAbandonedSourceFaultsLater_ShouldNotRaiseUnobservedTaskException()
		{
			MyException exception = new();
			bool isRaised = false;
			EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
			{
				if (e.Exception.InnerExceptions.Contains(exception))
				{
					isRaised = true;
				}
			};

			TaskScheduler.UnobservedTaskException += handler;
			try
			{
				await AbandonSourceThatFaultsLater(exception);
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			finally
			{
				TaskScheduler.UnobservedTaskException -= handler;
			}

			await That(isRaised).IsFalse()
				.Because("the exception of an abandoned source must be observed, as nobody else awaits it");
		}

		[Test]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortANegatedExpectation()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).DoesNotContain(item => Cancel(cts, item == 3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching item => Cancel(cts, item == 3),
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, (… and maybe more)]
				             """)
				.Because("a cancellation between two items must not be mistaken for the end of the source");
		}

		[Test]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortContains()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).Contains(item => Cancel(cts, item == 3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             contains an item matching item => Cancel(cts, item == 3) at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, (… and maybe more)]
				             """)
				.Because("a cancellation between two items must not be reported as a missing item");
		}

		[Test]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortIsEqualTo()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2, 3]).Using(new CancellingComparer(cts))
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2, 3] using ThatAsyncEnumerable.CancellationTests.CancellingComparer in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, (… and maybe more)]

				             Expected:
				             [1, 2, 3]
				             """)
				.Because("a cancellation between two items must not be reported as missing items");
		}

		[Test]
		public async Task WhenCancellationIsRequestedDuringTheReassignment_ShouldAbortContainsExpectationsInAnyOrder()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<IEnumerable<int>> subject =
				ToAsyncEnumerable<IEnumerable<int>>(CancellingItems(5, cts), Array.Empty<int>());
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected =
			[
				a => a.IsNotNull(),
				a => a.Contains(-1),
			];

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
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
				               [],
				               (… and maybe more)
				             ]

				             Expected:
				             [
				               an item that is not null,
				               an item that contains an item equal to -1 at least once
				             ]
				             """)
				.Because("the first item is only compared with the second expectation when the second item reassigns it, where its cancellation must not be reported as a missing item");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldDecideDoesNotHaveItemAtAnEarlierIndex()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 2);

			async Task Act()
				=> await That(subject).DoesNotHaveItem(1).AtIndex(0).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the item at index 0 decides the outcome, so the source must not be read any further");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldDecideHasItemAtAnEarlierIndex()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 2);

			async Task Act()
				=> await That(subject).HasItem(1).AtIndex(0).WithCancellation(cts.Token);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 1 at index 0,
				             but it had item 2 at index 0

				             Collection:
				             [2, (… and maybe more)]
				             """)
				.Because("the item at index 0 decides the outcome, so the source must not be read any further");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldDecideHasItemThatAtAnEarlierIndex()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 2);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(1)).AtIndex(0).WithCancellation(cts.Token);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to 1 at index 0,
				             but it had item 2 at index 0

				             Collection:
				             [2, (… and maybe more)]
				             """)
				.Because("the item at index 0 decides the outcome, so the source must not be read any further");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldLetAnAlternativeToHasItemDecide()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasItem(3).Or.Contains(1).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the received items already contain 1, so the undecided item search does not matter");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldLetAnAlternativeToHasItemThatDecide()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(3)).Or.Contains(1).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the received items already contain 1, so the undecided item search does not matter");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldLetAnAlternativeToIsInAscendingOrderDecide()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().Or.Contains(1).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the received items already contain 1, so the undecided order does not matter");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportACountAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasCount(3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportContainsAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportDoesNotContainAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).DoesNotContain(item => item == 3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching item => item == 3,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportEndsWithAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).EndsWith(2).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             ends with [2],
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportHasItemAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasItem(3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 3,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportHasItemFromTheEndAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasItem(2).AtIndexFromEnd(0).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 2 at index 0 from end,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportHasItemThatAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to 3,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportHasSingleAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1);

			async Task Act()
				=> await That(subject).HasSingle().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has a single item,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportIsEqualToAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2, 3]).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2, 3] in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]

				             Expected:
				             [1, 2, 3]
				             """)
				.Because("the items received before the cancellation explain where the evaluation stopped");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportIsEqualToExpectationsAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([x => x.IsEqualTo(1), x => x.IsEqualTo(2), x => x.IsEqualTo(3),])
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x.IsEqualTo(1), x => x.IsEqualTo(2), x => x.IsEqualTo(3),] in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]

				             Expected:
				             [an item that is equal to 1, an item that is equal to 2, an item that is equal to 3]
				             """)
				.Because("the items received before the cancellation explain where the evaluation stopped");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportIsEqualToPredicatesAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == 1, x => x == 2, x => x == 3,])
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == 1, x => x == 2, x => x == 3,] in order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]

				             Expected:
				             [
				               x => (x == 1),
				               x => (x == 2),
				               x => (x == 3)
				             ]
				             """)
				.Because("the items received before the cancellation explain where the evaluation stopped");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportIsInAscendingOrderAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """)
				.Because("the items received before the cancellation explain where the evaluation stopped");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportStartsWithAsNotVerified()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = HangAfter(cts.Cancel, 1, 2);

			async Task Act()
				=> await That(subject).StartsWith(1, 2, 3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             starts with [1, 2, 3],
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """)
				.Because("the items received before the cancellation explain where the evaluation stopped");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldStopWaiting()
		{
			IAsyncEnumerable<int> subject = HangAfter();
			using CancellationTokenSource cts = new();
			cts.CancelAfter(50.Milliseconds());

			async Task Act()
				=> await That(subject).Contains(1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it could not be verified, because the evaluation was already canceled

				             Collection:
				             []
				             """)
				.Because("a requested cancellation aborts the evaluation, even if the source ignores it");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWithinAnItem_ShouldAbortAllComplyWith()
		{
			using CancellationTokenSource cts = new();
			IEnumerable<int>[] items = [[], [], [], [], [], [], [], [], [], [], CancellingItems(5, cts),];
			IAsyncEnumerable<IEnumerable<int>> subject = ToAsyncEnumerable(items);

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.DoesNotContain(-1)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to -1 for all items,
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
				               (… and maybe more)
				             ]
				             """)
				.Because("a cancellation within an item must not be reported as a not matching item");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWithinAnItem_ShouldAbortContainsExpectations()
		{
			using CancellationTokenSource cts = new();
			int enumeratedCount = 0;
			IAsyncEnumerable<IEnumerable<int>> subject = ToAsyncEnumerable<IEnumerable<int>>(CancellingItems(5, cts).Select(x =>
			{
				enumeratedCount++;
				return x;
			}));
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected = [a => a.Contains(-1),];

			async Task Act()
				=> await That(subject).Contains(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
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
				               ],
				               (… and maybe more)
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

		[Test]
		public async Task WhenCancellationIsRequestedWithinAnItem_ShouldAbortHasItemThat()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<IEnumerable<int>> subject = ToAsyncEnumerable<IEnumerable<int>>(CancellingItems(1, cts), [1, 2,]);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.HasCount(3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that subject
				             has an item that has exactly 3 items,
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
				.Because("the first item was canceled before its count was known");
		}

		[Test]
		public async Task WhenCancellationIsRequestedWithinAnItem_ShouldAbortIsEqualToExpectations()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<IEnumerable<int>> subject =
				ToAsyncEnumerable(Enumerable.Repeat<IEnumerable<int>>([], 20).Append(CancellingItems(5, cts)).ToArray());
			IEnumerable<Action<IThat<IEnumerable<int>?>>> expected =
				Enumerable.Repeat<Action<IThat<IEnumerable<int>?>>>(a => a.Contains(-1), 21);

			async Task Act()
				=> await That(subject).IsEqualTo(expected).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
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
				               (… and maybe more)
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

		[Test]
		public async Task WhenChainedExpectationsAreMet_ShouldEnumerateTheSourceOnce()
		{
			int enumerations = 0;

			async IAsyncEnumerable<int> Numbers()
			{
				enumerations++;
				foreach (int item in new[] { 1, 2, 3, })
				{
					await Task.Yield();
					yield return item;
				}
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).And.Contains(3).And.HasCount(3).WithTimeout(30.Seconds());

			await That(Act).DoesNotThrow();
			await That(enumerations).IsEqualTo(1)
				.Because("the chained expectations continue the materialized source instead of enumerating it again");
		}

		[Test]
		public async Task WhenExpectedItemPrecedesAHang_ShouldSucceed()
		{
			IAsyncEnumerable<int> subject = HangAfter(1);

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(30.Seconds());

			await That(Act).DoesNotThrow()
				.Because("the source is only enumerated as far as necessary");
		}

		[Test]
		public async Task WhenGlobalTimeoutElapsesWhileTheSourceHangs_ShouldFail()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
			{
				using IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					.Set(TestCancellation.FromTimeout(50.Milliseconds()));
				await That(subject).Contains(1);
			}

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050

				             Collection:
				             []
				             """);
		}

		[Test]
		public async Task WhenSourceEndedBeforeTheCancellation_ShouldJudgeHasSingle()
		{
			using CancellationTokenSource cts = new();
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

			async Task Act()
				=> await That(subject).HasCount(1).And.HasSingle().Matching(item => Cancel(cts, item == 1))
					.WithCancellation(cts.Token);

			await That(Act).DoesNotThrow()
				.Because("the source was read to its end before the cancellation, so all items are known");
		}

		[Test]
		public async Task WhenSourceHangsAfterSomeItems_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2, 3);

			async Task Act()
				=> await That(subject).Contains(4).WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 4 at least once,
				             but it did not finish within 0:01

				             Collection:
				             [1, 2, 3, (… and maybe more)]
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:01."))
				.Because("the timeout must be long enough that the items are delivered before it elapses, even on a busy machine");
		}

		[Test]
		public async Task WhenSourceHangsBeforeTheCollectionIsFormatted_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it did not finish within 0:01

				             Collection:
				             [1, 2, (… and maybe more)]
				             """)
				.Because("the timeout must be long enough that the items are delivered before it elapses, even on a busy machine");
		}

		[Test]
		public async Task WhenSourceIgnoresTheToken_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050

				             Collection:
				             []
				             """);
		}

		[Test]
		public async Task WhenSourceIgnoresTheToken_ShouldFailANegatedExpectationAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
				=> await That(subject).DoesNotContain(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 1,
				             but it did not finish within 0:00.050

				             Collection:
				             []
				             """)
				.Because("a timeout must not be mistaken for the end of the source");
		}

		[Test]
		public async Task WhenSourceObservesTheToken_ShouldCancelTheSource()
		{
			CancellationToken sourceToken = CancellationToken.None;

			async IAsyncEnumerable<int> Numbers([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				sourceToken = cancellationToken;
				await Task.Delay(30.Seconds(), cancellationToken);
				yield return 1;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050

				             Collection:
				             []
				             """);
			await That(sourceToken.IsCancellationRequested).IsTrue()
				.Because("the token of the evaluation must reach the source");
		}

		[Test]
		public async Task WhenTimeoutElapsesBetweenItems_ShouldFailANegatedExpectation()
		{
			CancellationToken sourceToken = CancellationToken.None;

			async IAsyncEnumerable<int> Numbers([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				sourceToken = cancellationToken;
				await Task.Yield();
				yield return 1;
				await Task.Yield();
				yield return 2;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).DoesNotContain(item => BlockUntilCancelled(item == 2, sourceToken))
					.WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching item => BlockUntilCancelled(item == 2, sourceToken),
				             but it did not finish within 0:01

				             Collection:
				             [1, (… and maybe more)]
				             """)
				.Because("a timeout between two items must not be mistaken for the end of the source");
		}

		[Test]
		public async Task WhenTimeoutElapsesWhileTheSourceHangs_ShouldFailACount()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).HasCount(3).WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did not finish within 0:01

				             Collection:
				             [1, 2, (… and maybe more)]
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:01."))
				.Because("a timeout is reported the same way, whichever expectation was pending");
		}

		[Test]
		public async Task WhenTimeoutElapsesWhileTheSourceHangs_ShouldFailIsEqualTo()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2, 3]).WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2, 3] in order,
				             but it did not finish within 0:01

				             Collection:
				             [1, 2, (… and maybe more)]

				             Expected:
				             [1, 2, 3]
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:01."))
				.Because("a timeout lists the items received so far, whichever expectation was pending");
		}

		[Test]
		public async Task WhenTimeoutElapsesWhileTheSourceHangs_ShouldKeepTheFailureOfASiblingOfIsEqualTo()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).HasCount().EqualTo(0).And.IsEqualTo([1, 2, 3,]).WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 0 items and is equal to collection [1, 2, 3,] in order,
				             but it had at least 1 item

				             Collection:
				             [1, 2, (… and maybe more)]
				             """)
				.Because("the undecided comparison must not hide the decided failure of the count");
		}

		/// <remarks>
		///     The source is only reachable from within this method, so that it can be collected afterwards.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static async Task AbandonSourceThatFaultsLater(Exception exception)
		{
			TaskCompletionSource tcs = new();

			async IAsyncEnumerable<int> Numbers()
			{
				await tcs.Task;
				yield return 1;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050

				             Collection:
				             []
				             """);
			tcs.SetException(exception);
		}

		/// <remarks>
		///     Blocks the evaluation between two items until it is cancelled, bounded by half a minute, so that a
		///     regression fails the test instead of hanging the test run.
		/// </remarks>
		private static bool BlockUntilCancelled(bool result, CancellationToken cancellationToken)
		{
			cancellationToken.WaitHandle.WaitOne(30.Seconds());
			return result;
		}

		private static bool Cancel(CancellationTokenSource cts, bool result)
		{
			cts.Cancel();
			return result;
		}

		/// <remarks>
		///     The items are provided synchronously, so that they are received before a short timeout elapses, even on
		///     a busy machine. The hang ends after half a minute, so that a regression which waits for it fails the test
		///     instead of hanging the test run.
		/// </remarks>
		private static IEnumerable<int> CancellingItems(int cancelAfter, CancellationTokenSource cts)
		{
			for (int index = 0; index < 10_000; index++)
			{
				if (index == cancelAfter)
				{
					cts.Cancel();
				}

				yield return index;
			}
		}

		private static IAsyncEnumerable<int> HangAfter(params int[] items)
			=> HangAfter(() => { }, items);

		/// <remarks>
		///     The <paramref name="onHang" /> callback is invoked once all <paramref name="items" /> were delivered, so
		///     that a cancellation it requests does not depend on the timing.
		/// </remarks>
		private static async IAsyncEnumerable<int> HangAfter(Action onHang, params int[] items)
		{
			foreach (int item in items)
			{
				yield return item;
			}

			onHang();
			await Task.Delay(30.Seconds());
		}

		private sealed class CancellingComparer(CancellationTokenSource cts) : IEqualityComparer<object>
		{
			public new bool Equals(object? x, object? y) => Cancel(cts, object.Equals(x, y));

			public int GetHashCode(object obj) => obj.GetHashCode();
		}
	}
}
#endif
