using System.Threading;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class TaskTests
	{
		[Test]
		[Arguments(true, "CancellationToken (canceled)")]
		[Arguments(false, "CancellationToken (not canceled)")]
		public async Task CancellationToken_ShouldFormatItsState(bool isCanceled, string expectedResult)
		{
			CancellationToken value = new(isCanceled);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task InFailureMessage_WhenCollectionContainsAPendingTask_ShouldFailWithoutWaiting()
		{
			TaskCompletionSource<int> tcs = new();
			Task<int>[] subject = [tcs.Task,];

			async Task Act()
				=> await WithoutWaitingFor(tcs, async () =>
				{
					await That(subject).IsEmpty();
					return 0;
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was [
				               Task<int> (WaitingForActivation)
				             ]
				             """);
		}

		[Test]
		public async Task WhenMemberIsAPendingTask_ShouldFormatItsStatusWithoutWaiting()
		{
			TaskCompletionSource<int> tcs = new();
			object value = new
			{
				P = tcs.Task,
			};

			string result = await WithoutWaitingFor(tcs,
				() => Task.FromResult(Formatter.Format(value, FormattingOptions.SingleLine)));

			await That(result).IsEqualTo("{ P = Task<int> (WaitingForActivation) }");
		}

		[Test]
		public async Task WhenResultIsAString_ShouldQuoteTheResult()
		{
			Task<string> value = Task.FromResult("foo");

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("Task<string> (RanToCompletion, \"foo\")");
		}

		[Test]
		public async Task WhenResultIsTheTaskItself_ShouldStopAtTheRecursion()
		{
			TaskCompletionSource<object> tcs = new();
			tcs.SetResult(tcs.Task);

			string result = Formatter.Format(tcs.Task);

			await That(result).IsEqualTo("Task<object> (RanToCompletion, Task<object> (RanToCompletion, *recursive*))");
		}

		[Test]
		public async Task WhenTaskIsCanceled_ShouldFormatTheStatus()
		{
			Task value = Task.FromCanceled(new CancellationToken(true));
			Task<int> genericValue = Task.FromCanceled<int>(new CancellationToken(true));

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("Task (Canceled)");
			await That(genericResult).IsEqualTo("Task<int> (Canceled)");
		}

		[Test]
		public async Task WhenTaskIsFaulted_ShouldFormatTheStatusAndTheException()
		{
			Task value = Task.FromException(new InvalidOperationException("message"));
			Task<int> genericValue = Task.FromException<int>(new InvalidOperationException("message"));

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("Task (Faulted, InvalidOperationException: message)");
			await That(genericResult).IsEqualTo("Task<int> (Faulted, InvalidOperationException: message)");
		}

		[Test]
		public async Task WhenTaskIsPending_ShouldFormatTheStatusWithoutWaiting()
		{
			TaskCompletionSource<int> tcs = new();
			Task untypedValue = PendingUntil(tcs.Task);

			(string result, string untypedResult) = await WithoutWaitingFor(tcs,
				() => Task.FromResult((Formatter.Format(tcs.Task), Formatter.Format(untypedValue))));
			await untypedValue;

			await That(result).IsEqualTo("Task<int> (WaitingForActivation)");
			await That(untypedResult).IsEqualTo("Task (WaitingForActivation)")
				.Because("the task of an async method without a result is not named after its internal result type");
		}

		[Test]
		public async Task WhenTaskRanToCompletion_ShouldFormatTheStatusAndTheResult()
		{
			Task value = Task.CompletedTask;
			Task<int> genericValue = Task.FromResult(42);

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("Task (RanToCompletion)");
			await That(genericResult).IsEqualTo("Task<int> (RanToCompletion, 42)");
		}

		[Test]
		public async Task WhenValueTaskIsCanceled_ShouldFormatTheStatus()
		{
			ValueTask value = new(Task.FromCanceled(new CancellationToken(true)));
			ValueTask<int> genericValue = new(Task.FromCanceled<int>(new CancellationToken(true)));

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("ValueTask (Canceled)");
			await That(genericResult).IsEqualTo("ValueTask<int> (Canceled)");
		}

		[Test]
		public async Task WhenValueTaskIsFaulted_ShouldFormatTheStatus()
		{
			ValueTask value = new(Task.FromException(new InvalidOperationException("message")));
			ValueTask<int> genericValue = new(Task.FromException<int>(new InvalidOperationException("message")));

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("ValueTask (Faulted)");
			await That(genericResult).IsEqualTo("ValueTask<int> (Faulted)");
		}

		[Test]
		public async Task WhenValueTaskIsPending_ShouldFormatTheStatusWithoutWaiting()
		{
			TaskCompletionSource<int> tcs = new();
			ValueTask value = new(tcs.Task);
			ValueTask<int> genericValue = new(tcs.Task);

			string result = Formatter.Format(value);
			string genericResult = await WithoutWaitingFor(tcs, () => Task.FromResult(Formatter.Format(genericValue)));

			await That(result).IsEqualTo("ValueTask (Pending)");
			await That(genericResult).IsEqualTo("ValueTask<int> (Pending)");
		}

		[Test]
		public async Task WhenValueTaskRanToCompletion_ShouldFormatTheStatusAndTheResult()
		{
			ValueTask value = new();
			ValueTask<int> genericValue = new(42);

			string result = Formatter.Format(value);
			string genericResult = Formatter.Format(genericValue);

			await That(result).IsEqualTo("ValueTask (RanToCompletion)");
			await That(genericResult).IsEqualTo("ValueTask<int> (RanToCompletion, 42)");
		}

		private static async Task PendingUntil(Task task)
			=> await task;

		/// <summary>
		///     Fails when the <paramref name="action" /> waits for the <paramref name="pending" /> task, which is then
		///     completed, so that a blocked thread is released instead of hanging the test run.
		/// </summary>
		private static async Task<T> WithoutWaitingFor<T>(TaskCompletionSource<int> pending, Func<Task<T>> action)
		{
			Task<T> task = Task.Run(action);
			bool isCompleted = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(30))) == task;
			pending.TrySetResult(0);

			await That(isCompleted).IsTrue().Because("a pending task must not be waited for");
			return await task;
		}
	}
}
