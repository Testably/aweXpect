using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextTests
{
	public sealed class AsyncCallbackTests
	{
		[Test]
		public async Task GetContent_ShouldForwardTheCancellationTokenToTheCallback()
		{
			using CancellationTokenSource cts = new();
			CancellationToken? receivedToken = null;
			ResultContext sut = new ResultContext.AsyncCallback("foo", token =>
			{
				receivedToken = token;
				return Task.FromResult<string?>("bar");
			});

			string? content = await sut.GetContent(cts.Token);

			await That(content).IsEqualTo("bar");
			await That(receivedToken).IsEqualTo(cts.Token);
		}

		[Test]
		public async Task ShouldUseTitleAndPriority()
		{
			ResultContext sut = new ResultContext.AsyncCallback("foo", _ => Task.FromResult<string?>("bar"), 3);

			await That(sut.Title).IsEqualTo("foo");
			await That(sut.Priority).IsEqualTo(3);
		}

		[Test]
		public async Task WhenAddedToAFailingExpectation_ShouldShowTheContentOfTheCallback()
		{
			async Task Act()
				=> await That(1).ShowsContexts((contexts, actual, _)
					=> contexts.Add(new ResultContext.AsyncCallback("Value",
						_ => Task.FromResult<string?>($"async {actual}"))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Value:
				             async 1
				             """);
		}

		[Test]
		public async Task WhenCallbackCompletesLater_WithoutTimeoutAndCancellation_ShouldAwaitTheCallback()
		{
			TaskCompletionSource<string?> content = new(TaskCreationOptions.RunContinuationsAsynchronously);
			Task evaluation = Evaluate();

			await Task.WhenAny(evaluation, Task.Delay(100.Milliseconds()));
			bool isCompletedWhilePending = evaluation.IsCompleted;
			content.SetResult("late");

			await That(isCompletedWhilePending).IsFalse()
				.Because("nothing limits how long the failure message waits for the context");
			await That(() => evaluation).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Value:
				             late
				             """);

			async Task Evaluate()
				=> await That(1).ShowsContexts((contexts, _, _)
					=> contexts.Add(new ResultContext.AsyncCallback("Value", _ => content.Task)));
		}

		[Test]
		public async Task WhenCallbackNeverCompletes_InACombination_ShouldLeaveOutTheContext()
		{
			VirtualTimeSystem timeSystem = new();
			Task evaluation = Evaluate();

			await That(await IsCompleted(evaluation)).IsTrue()
				.Because("the timeout of the expectation must stop waiting for its context in a combination, too");
			await That(() => evaluation).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that 1 shows contexts
				              [02] Expected that 2 shows contexts
				             but
				              [01] it did not
				              [02] it did not
				             """);

			async Task Evaluate()
				=> await ThatAll(
					That(1).ShowsContexts((contexts, _, _)
							=> contexts.Add(new ResultContext.AsyncCallback("Value", _ =>
							{
								timeSystem.Advance(10.Seconds());
								return PendingTask.Of<string?>();
							})))
						.WithTimeout(5.Seconds()).WithTimeSystem(timeSystem),
					That(2).ShowsContexts((_, _, _) => { }));
		}

		[Test]
		public async Task WhenCallbackNeverCompletes_WithCancellation_ShouldLeaveOutTheContext()
		{
			using CancellationTokenSource cts = new();
			Task evaluation = Evaluate();

			await That(await IsCompleted(evaluation)).IsTrue()
				.Because("the cancellation must stop waiting for a context that does not arrive");
			await That(() => evaluation).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Other:
				             completed
				             """);

			async Task Evaluate()
				=> await That(1).ShowsContexts((contexts, _, _) =>
				{
					contexts.Add(new ResultContext.AsyncCallback("Value", _ =>
					{
						// ReSharper disable once AccessToDisposedClosure
						cts.Cancel();
						return PendingTask.Of<string?>();
					}));
					contexts.Add(new ResultContext.AsyncCallback("Other", _ => Task.FromResult<string?>("completed"), 1));
				}).WithCancellation(cts.Token);
		}

		[Test]
		public async Task WhenCallbackNeverCompletes_WithTimeout_ShouldLeaveOutTheContext()
		{
			VirtualTimeSystem timeSystem = new();
			Task evaluation = Evaluate();

			await That(await IsCompleted(evaluation)).IsTrue()
				.Because("the timeout must stop waiting for a context that does not arrive");
			await That(() => evaluation).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Other:
				             completed
				             """);

			async Task Evaluate()
				=> await That(1).ShowsContexts((contexts, _, _) =>
				{
					contexts.Add(new ResultContext.AsyncCallback("Value", _ =>
					{
						timeSystem.Advance(10.Seconds());
						return PendingTask.Of<string?>();
					}));
					contexts.Add(new ResultContext.AsyncCallback("Other", _ => Task.FromResult<string?>("completed"), 1));
				}).WithTimeout(5.Seconds()).WithTimeSystem(timeSystem);
		}

		[Test]
		public async Task WhenCallbackNeverCompletes_WithTimeoutOnTheRealClock_ShouldLeaveOutTheContext()
		{
			Task evaluation = Evaluate();

			await That(await IsCompleted(evaluation)).IsTrue()
				.Because("the timeout must stop waiting for a context that does not arrive");
			await That(() => evaluation).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not
				             """);

			async Task Evaluate()
				=> await That(1).ShowsContexts((contexts, _, _)
						=> contexts.Add(new ResultContext.AsyncCallback("Value", _ => PendingTask.Of<string?>())))
					.WithTimeout(50.Milliseconds());
		}

		[Test]
		public async Task WhenCallbackReceivesTheToken_ShouldBeCanceledByTheTestCancellation()
		{
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;
			bool? canBeCanceled = null;
			Exception? exception;

			using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
				       .Set(TestCancellation.FromCancellationToken(() => token)))
			{
				async Task Act()
					=> await That(1).ShowsContexts((contexts, _, _)
						=> contexts.Add(new ResultContext.AsyncCallback("Value", callbackToken =>
						{
							canBeCanceled = callbackToken.CanBeCanceled;
							return Task.FromResult<string?>("completed");
						})));

				exception = await Catch.ExceptionAsync(Act);
			}

			await That(exception).IsExactly<FailException>();
			await That(canBeCanceled).IsEqualTo(true)
				.Because("the callback is canceled with the evaluation, also by the test cancellation");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task WhenCallbackThrowsAnOperationCanceledException_ShouldStillReportTheFailure(bool isAsync)
		{
			async Task Act()
				=> await That(1).ShowsContexts((contexts, _, _) =>
				{
					contexts.Add(new ResultContext.AsyncCallback("Value", isAsync
						? _ => Task.FromException<string?>(new OperationCanceledException())
						: _ => throw new OperationCanceledException()));
					contexts.Add(new ResultContext.AsyncCallback("Other", _ => Task.FromResult<string?>("completed")));
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Other:
				             completed
				             """)
				.Because("a context that is not available must not replace the failure");
		}

		[Test]
		public async Task WhenCallbackThrowsOnCancellation_AfterTheTimeout_ShouldStillReportTheFailure()
		{
			VirtualTimeSystem timeSystem = new();

			async Task Act()
				=> await That(1).ShowsContexts((contexts, _, _)
						=> contexts.Add(new ResultContext.AsyncCallback("Value", async token =>
						{
							timeSystem.Advance(10.Seconds());
							await Task.Yield();
							token.ThrowIfCancellationRequested();
							return "after the timeout";
						})))
					.WithTimeout(5.Seconds()).WithTimeSystem(timeSystem);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not
				             """);
		}

		[Test]
		public async Task WhenCallbackThrowsOnCancellation_WithCancellation_ShouldStillReportTheFailure()
		{
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(1).ShowsContexts((contexts, _, _)
						=> contexts.Add(new ResultContext.AsyncCallback("Value", token =>
						{
							// ReSharper disable once AccessToDisposedClosure
							cts.Cancel();
							token.ThrowIfCancellationRequested();
							return Task.FromResult<string?>("after the cancellation");
						})))
					.WithCancellation(cts.Token);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not
				             """)
				.Because("the cancellation of a context must not replace the failure");
		}

		private static async Task<bool> IsCompleted(Task evaluation)
		{
			await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
			return evaluation.IsCompleted;
		}
	}

	public sealed class SyncCallbackTests
	{
		[Test]
		public async Task WhenCodeOfTheCallerThrows_ShouldOmitTheContext()
		{
			async Task Act()
				=> await That(1).ShowsContexts((contexts, _, _)
					=> contexts.Add(new ResultContext.SyncCallback("Value",
						() => UserCode.Invoke<string?>(() => throw new MyException()))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not
				             """)
				.Because("the exception of the caller must not abort the failure message");
		}
	}
}
