using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class ActionTests
		{
			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("a cancellation aborts the execution instead of timing it");
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task> @delegate = () => Task.CompletedTask;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Func<Task> @delegate = () => Task.Delay(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<Task> @delegate = () => Task.FromException(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("a cancellation aborts the execution instead of timing it");
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task<int>> @delegate = () => Task.FromResult(1);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Func<Task<int>> @delegate = () => Task.Delay(50.Milliseconds()).ContinueWith(_ => 1);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<Task<int>> @delegate = () => Task.FromException<int>(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask Delegate() => new(Task.Delay(50.Milliseconds()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask Delegate() => new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("a cancellation aborts the execution instead of timing it");
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask Delegate(CancellationToken token) => new(Task.Delay(50.Milliseconds(), token));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask<int> Delegate() => new(Task.Delay(50.Milliseconds()).ContinueWith(_ => 1));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask<int> Delegate() => new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:05,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("a cancellation aborts the execution instead of timing it");
			}

			[Fact]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromResult(1));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				ValueTask<int> Delegate(CancellationToken token)
					=> new(Task.Delay(50.Milliseconds(), token).ContinueWith(_ => 1, token));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
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
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

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
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithTimeoutTests
		{
			[Fact]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(4000.Milliseconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:04,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(4000.Milliseconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:04,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}
		}

		public sealed class WithCancellationTests
		{
			[Fact]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithCancellation(cancelledToken);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await Task.Delay(30.Seconds(), token);
					return 1;
				};
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithCancellation(cancelledToken);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it was canceled after 0:*
					             """).AsWildcard();
			}
		}
	}
}
