using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             but it could not be verified, because it was already canceled

				             Collection:
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

				             Expected:
				             [an item that is negative]
				             """)
				.Because("a cancellation must not be reported as missing items");
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
				             but it could not be verified, because it was already canceled

				             Collection:
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
				             """)
				.Because("a cancellation must not be reported as a missing item");
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
				             but it could not be verified, because it was already canceled

				             Collection:
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

				             Expected:
				             [
				               a => (a < 0)
				             ]
				             """)
				.Because("a cancellation must not be reported as missing items");
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
				             but it could not be verified, because it was already canceled

				             Collection:
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

				             Expected:
				             [0, 1, 2, 3, 4, 5, 6, 7]
				             """)
				.Because("a cancellation must not be reported as additional items");
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
				             """).And
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

				             Expected:
				             [-1]
				             """)
				.Because("a timeout must not be reported as missing items");
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
