#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

/// <summary>
///     The source of a materialized collection is disposed at the end of the evaluation and between the attempts of
///     <c>Eventually()</c>, which the timeout and the cancellation of the evaluation bound.
/// </summary>
/// <remarks>
///     No test waits longer than ten seconds for the evaluation, so that a regression which awaits the dispose without
///     a bound fails the test instead of hanging the test run.
/// </remarks>
public sealed class MaterializedSourceReleaseTests
{
	[Test]
	public async Task WhenDisposeIsPending_BetweenAttemptsOfEventually_WithCancellation_ShouldBeInconclusive()
	{
		using CancellationTokenSource cts = new();
		VirtualTimeSystem timeSystem = new();
		PendingDisposeAsyncEnumerable source = new([1, 2,], () => cts.Cancel());
		Func<IAsyncEnumerable<int>> subject = () => source;
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the cancellation must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             eventually contains an item equal to 5 at least once within 0:30,
			             but it could not be verified, because the evaluation was already canceled

			             Collection:
			             []
			             """);
		await That(source.DisposeCount).IsEqualTo(2)
			.Because("one more attempt is made with the canceled token after the dispose was abandoned");

		async Task Evaluate()
			=> await That(subject).Eventually().Within(30.Seconds()).OnVirtualTime(timeSystem).Contains(5)
				.WithCancellation(cts.Token);
	}

	[Test]
	public async Task WhenDisposeIsPending_BetweenAttemptsOfEventually_WithShorterTimeout_ShouldFailWithTheTimeout()
	{
		VirtualTimeSystem timeSystem = new();
		PendingDisposeAsyncEnumerable source = new([1, 2,], () => timeSystem.Advance(10.Seconds()));
		Func<IAsyncEnumerable<int>> subject = () => source;
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the timeout must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             eventually contains an item equal to 5 at least once within 0:30,
			             but it did not finish within 0:05
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:05."));
		await That(source.DisposeCount).IsEqualTo(1);

		async Task Evaluate()
			=> await That(subject).Eventually().Within(30.Seconds()).OnVirtualTime(timeSystem).Contains(5)
				.WithTimeout(5.Seconds());
	}

	[Test]
	public async Task WhenDisposeIsPending_BetweenAttemptsOfEventually_WithTimeoutThatIsNotShorter_ShouldFail()
	{
		VirtualTimeSystem timeSystem = new();
		PendingDisposeAsyncEnumerable source = new([1, 2,], () => timeSystem.Advance(60.Seconds()));
		Func<IAsyncEnumerable<int>> subject = () => source;
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the timeout must stop waiting for the dispose, although it does not cancel the attempts");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             eventually contains an item equal to 5 at least once within 0:05,
			             but it did not contain it

			             Collection:
			             [1, 2]
			             """);
		await That(source.DisposeCount).IsEqualTo(2)
			.Because("the last attempt is made after the dispose of the first one was abandoned");

		async Task Evaluate()
			=> await That(subject).Eventually().Within(5.Seconds()).OnVirtualTime(timeSystem).Contains(5)
				.WithTimeout(30.Seconds());
	}

	[Test]
	public async Task WhenDisposeIsPending_WithCancellation_WhenExpectationFails_ShouldReportTheFailure()
	{
		using CancellationTokenSource cts = new();
		PendingDisposeAsyncEnumerable subject = new([1, 2,], () => cts.Cancel());
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the cancellation must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 5 at least once,
			             but it did not contain it

			             Collection:
			             [1, 2]
			             """);
		await That(subject.DisposeCount).IsEqualTo(1);

		async Task Evaluate()
			=> await That(subject).Contains(5).WithCancellation(cts.Token);
	}

	[Test]
	public async Task WhenDisposeIsPending_WithCancellation_WhenExpectationIsMet_ShouldStayMet()
	{
		using CancellationTokenSource cts = new();
		PendingDisposeAsyncEnumerable subject = new([1, 2,], () => cts.Cancel());
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the cancellation must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).DoesNotThrow()
			.Because("the expectation was met before the cancellation");
		await That(subject.DisposeCount).IsEqualTo(1);

		async Task Evaluate()
			=> await That(subject).Contains(1).WithCancellation(cts.Token);
	}

	[Test]
	public async Task WhenDisposeIsPending_WithoutTimeoutAndCancellation_ShouldAwaitTheDispose()
	{
		PendingDisposeAsyncEnumerable subject = new([1, 2,]);
		Task evaluation = Evaluate();

		await Task.WhenAny(evaluation, Task.Delay(100.Milliseconds()));
		bool isCompletedWhileDisposing = evaluation.IsCompleted;
		subject.CompleteDispose();

		await That(isCompletedWhileDisposing).IsFalse()
			.Because("nothing limits how long the evaluation waits for the dispose");
		await That(await IsCompleted(evaluation)).IsTrue();
		await That(() => evaluation).DoesNotThrow();

		async Task Evaluate()
			=> await That(subject).Contains(1);
	}

	[Test]
	public async Task WhenDisposeIsPending_WithTimeout_WhenExpectationFails_ShouldReportTheFailure()
	{
		VirtualTimeSystem timeSystem = new();
		PendingDisposeAsyncEnumerable subject = new([1, 2,], () => timeSystem.Advance(10.Seconds()));
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the timeout must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 5 at least once,
			             but it did not contain it

			             Collection:
			             [1, 2]
			             """);
		await That(subject.DisposeCount).IsEqualTo(1);

		async Task Evaluate()
			=> await That(subject).Contains(5).WithTimeout(5.Seconds()).WithTimeSystem(timeSystem);
	}

	[Test]
	public async Task WhenDisposeIsPending_WithTimeout_WhenExpectationIsMet_ShouldStayMet()
	{
		VirtualTimeSystem timeSystem = new();
		PendingDisposeAsyncEnumerable subject = new([1, 2,], () => timeSystem.Advance(10.Seconds()));
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the timeout must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).DoesNotThrow()
			.Because("the expectation was met before the timeout elapsed");
		await That(subject.DisposeCount).IsEqualTo(1);

		async Task Evaluate()
			=> await That(subject).Contains(1).WithTimeout(5.Seconds()).WithTimeSystem(timeSystem);
	}

	[Test]
	public async Task WhenDisposeIsPending_WithTimeoutOnTheRealClock_WhenExpectationIsMet_ShouldStayMet()
	{
		PendingDisposeAsyncEnumerable subject = new([1, 2,]);
		Task evaluation = Evaluate();

		await That(await IsCompleted(evaluation)).IsTrue()
			.Because("the timeout must stop waiting for a source that does not finish disposing");
		await That(() => evaluation).DoesNotThrow()
			.Because("the expectation was met before the timeout elapsed");

		async Task Evaluate()
			=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());
	}

	private static async Task<bool> IsCompleted(Task evaluation)
	{
		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		return evaluation.IsCompleted;
	}

	/// <summary>
	///     Returns the <paramref name="values" />, and its enumerator does not finish disposing until
	///     <see cref="CompleteDispose" /> is called.
	/// </summary>
	/// <param name="values">The items of the source.</param>
	/// <param name="onDispose">
	///     Called when the first enumerator is disposed, e.g. to state how long the dispose is pending or to cancel the
	///     evaluation meanwhile.
	/// </param>
	private sealed class PendingDisposeAsyncEnumerable(int[] values, Action? onDispose = null)
		: IAsyncEnumerable<int>
	{
		private readonly TaskCompletionSource<bool> _dispose =
			new(TaskCreationOptions.RunContinuationsAsynchronously);

		private readonly Action? _onDispose = onDispose;
		private readonly int[] _values = values;

		public int DisposeCount { get; private set; }

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> new Enumerator(this);

		public void CompleteDispose() => _dispose.TrySetResult(true);

		private sealed class Enumerator(PendingDisposeAsyncEnumerable owner) : IAsyncEnumerator<int>
		{
			private int _index = -1;

			public int Current => owner._values[_index];

			public ValueTask<bool> MoveNextAsync() => new(++_index < owner._values.Length);

			public ValueTask DisposeAsync()
			{
				if (owner.DisposeCount++ == 0)
				{
					owner._onDispose?.Invoke();
				}

				return new ValueTask(owner._dispose.Task);
			}
		}
	}
}
#endif
