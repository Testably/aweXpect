using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed class ExecutesWithin
	{
		public sealed class ActionTests
		{
			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesWithin(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("duration").And
					.WithMessage("The duration must not be negative.").AsPrefix()
					.Because("an execution can never take less than no time at all");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncTaskTests
		{
			[Fact]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);
				Func<Task> @delegate = () => Task.FromCanceled(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task> @delegate = () => Task.CompletedTask;

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Func<Task> @delegate = () => Task.Delay(30.Seconds());

				async Task Act()
					=> await That(@delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the duration is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				Func<Task> @delegate = () => Task.FromException(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncTaskValueTests
		{
			[Fact]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);
				Func<Task<int>> @delegate = () => Task.FromCanceled<int>(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task<int>> @delegate = () => Task.FromResult(1);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Func<Task<int>> @delegate = () => Task.Delay(30.Seconds()).ContinueWith(_ => 1);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the duration is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				Func<Task<int>> @delegate = () => Task.FromException<int>(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class FuncValueTaskTests
		{
			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask Delegate() => new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask Delegate() => new(Task.Delay(30.Seconds()));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the duration is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				ValueTask Delegate() => new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncCancellationTokenValueTaskTests
		{
			[Fact]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);

				ValueTask Delegate(CancellationToken _)
					=> new(Task.FromCanceled(canceledToken));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask Delegate(CancellationToken token) => new(Task.Delay(6.Seconds(), token));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncValueTaskValueTests
		{
			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask<int> Delegate() => new(Task.FromResult(1));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask<int> Delegate() => new(Task.Delay(30.Seconds()).ContinueWith(_ => 1));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the duration is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				ValueTask<int> Delegate() => new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncCancellationTokenValueTaskValueTests
		{
			[Fact]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);

				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromCanceled<int>(canceledToken));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromResult(1));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask<int> Delegate(CancellationToken token)
					=> new(Task.Delay(6.Seconds(), token).ContinueWith(_ => 1, token));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

		public sealed class FuncValueTests
		{
			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<int> @delegate = () => 0;

				async Task Act()
					=> await That(@delegate).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Func<int> @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					return 0;
				};

				async Task Act()
					=> await That(@delegate).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes within 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFailWithDescriptiveMessage)}
					              """);
			}

			[Fact]
			public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).ExecutesWithin(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("duration").And
					.WithMessage("The duration must not be negative.").AsPrefix()
					.Because("an execution can never take less than no time at all");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class TaskTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Task? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesWithin(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.500,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTaskIsFastEnough_ShouldSucceed()
			{
				Task subject = Task.CompletedTask;

				async Task Act()
					=> await That(subject).ExecutesWithin(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTaskTakesLonger_ShouldFail()
			{
				Task subject = Task.Delay(30.Seconds());

				async Task Act()
					=> await That(subject).ExecutesWithin(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes within 0:00.010,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the duration is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Fact]
			public async Task WhenTaskWasAlreadyCompleted_ShouldOnlyMeasureTheRemainingDuration()
			{
				Task subject = Task.Delay(200.Milliseconds());
				await subject;

				async Task Act()
					=> await That(subject).ExecutesWithin(100.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("the task is already running, so only the duration that remains is measured");
			}
		}

		public sealed class CancellationTokenTests
		{
			[Fact]
			public async Task WithoutReturnValue_WhenDelegateExceedsTheDuration_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.050,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the elapsed duration must cancel the token instead of awaiting the delegate");
			}

			[Fact]
			public async Task WithReturnValue_WhenDelegateExceedsTheDuration_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesWithin(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:00.050,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("the elapsed duration must cancel the token instead of awaiting the delegate");
			}
		}

		public sealed class WithTimeoutTests
		{
			[Fact]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(60.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:30,
					             but it was canceled after 0:0*
					             """).AsWildcard()
					.Because("the 50 ms timeout must cancel the delegate within seconds, long before the 30 s duration would");
			}

			[Fact]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await Task.Delay(60.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesWithin(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:30,
					             but it was canceled after 0:0*
					             """).AsWildcard()
					.Because("the 50 ms timeout must cancel the delegate within seconds, long before the 30 s duration would");
			}
		}

		public sealed class WithCancellationTests
		{
			[Fact]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(60.Seconds(), token);
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(30.Seconds()).WithCancellation(cancelledToken);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:30,
					             but it was canceled after 0:0*
					             """).AsWildcard()
					.Because("the already canceled token must cancel the delegate within seconds, long before the 30 s duration would");
			}

			[Fact]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await Task.Delay(60.Seconds(), token);
					return 1;
				};
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesWithin(30.Seconds()).WithCancellation(cancelledToken);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes within 0:30,
					             but it was canceled after 0:0*
					             """).AsWildcard()
					.Because("the already canceled token must cancel the delegate within seconds, long before the 30 s duration would");
			}
		}
	}
}
