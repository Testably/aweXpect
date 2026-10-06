namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class TaskTests
		{
			[Test]
			public async Task WhenTasksAreDifferentInstances_ShouldFail()
			{
				var subject = new
				{
					P = Task.FromResult("foo"),
				};
				var expected = new
				{
					P = Task.FromResult("foo"),
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property P differed:
					                   Actual: Task<string> (RanToCompletion, "foo")
					                 Expected: Task<string> (RanToCompletion, "foo")
					                 (tasks are compared by reference)

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenTheSameTaskIsPending_ShouldSucceedWithoutWaiting()
			{
				TaskCompletionSource<int> tcs = new();
				var subject = new
				{
					P = tcs.Task,
				};
				var expected = new
				{
					P = tcs.Task,
				};

				Task expectation = Task.Run(async () => await That(subject).IsEquivalentTo(expected));
				bool isCompleted =
					await Task.WhenAny(expectation, Task.Delay(TimeSpan.FromSeconds(30))) == expectation;
				// Releases an expectation that waits for the task, so that it does not hang the test run.
				tcs.SetResult(1);

				await That(isCompleted).IsTrue().Because("a pending task must not be waited for");
				await expectation;
			}
		}
	}
}
