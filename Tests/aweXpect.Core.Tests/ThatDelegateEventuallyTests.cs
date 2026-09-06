using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Customization;

namespace aweXpect.Core.Tests;

public sealed class ThatDelegateEventuallyTests
{
	[Fact]
	public async Task Because_ShouldBeIncludedInTheMessage()
	{
		Counter counter = new();

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsEqualTo(1).Because("of reasons")
				.WithTimeout(VeryLowTimeout);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => counter.Value
			             is equal to 1, because of reasons within 0:00.050,
			             but it was 0 which differs by -1
			             """);
	}

	[Fact]
	public async Task DefaultCheckInterval_ShouldDetermineTheNumberOfEvaluations()
	{
		Counter counter = new();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(200.Milliseconds()))
		{
			async Task Act() => await That(() => counter.Value).Eventually().IsEqualTo(1).WithTimeout(1.Seconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1 within 0:01,
				             but it was 0 which differs by -1
				             """);
		}

		await That(counter.EvaluationCount).IsBetween(2).And(6)
			.Because("the subject is evaluated at 0ms, 200ms, 400ms, 600ms, 800ms and - because the last wait " +
			         "is shortened to the remaining budget - at 1000ms, but a busy machine can drop evaluations, " +
			         "while without the check interval there would be far more of them");
	}

	[Fact]
	public async Task DefaultEventuallyTimeout_ShouldBeUsedWhenNoTimeoutIsSpecified()
	{
		Counter counter = new();
		Stopwatch stopwatch = new();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(VeryLowTimeout))
		{
			async Task Act() => await That(() => counter.Value).Eventually().IsEqualTo(1);

			stopwatch.Start();
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1 within 0:00.050,
				             but it was 0 which differs by -1
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThan(5.Seconds());
	}

	[Fact]
	public async Task ShouldForwardTheCancellationTokenToTheSubject()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		bool? isCancellationRequested = null;

		int Subject(CancellationToken token)
		{
			isCancellationRequested = token.IsCancellationRequested;
			return 0;
		}

		async Task Act()
			=> await That(Subject).Eventually().IsEqualTo(1)
				.WithTimeout(VeryLowTimeout).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that Subject
			             is equal to 1,
			             but it could not be verified, because it was already cancelled
			             """);
		await That(isCancellationRequested).IsTrue();
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldForwardTheCancellationTokenToTheValueTaskSubject()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		bool? isCancellationRequested = null;

		ValueTask<int> Subject(CancellationToken token)
		{
			isCancellationRequested = token.IsCancellationRequested;
			return new ValueTask<int>(0);
		}

		async Task Act()
			=> await That(Subject).Eventually().IsEqualTo(1)
				.WithTimeout(VeryLowTimeout).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that Subject
			             is equal to 1,
			             but it could not be verified, because it was already cancelled
			             """);
		await That(isCancellationRequested).IsTrue();
	}
#endif

	[Fact]
	public async Task ShouldReturnTheLastEvaluatedValue()
	{
		Counter counter = new(2);

		int result = await That(() => counter.Value).Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(result).IsEqualTo(4);
	}

	[Fact]
	public async Task ShouldSupportAndChain()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsGreaterThan(3).And.IsLessThan(100)
				.WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ShouldSupportAsyncSubject()
	{
		Counter counter = new(2);

		Func<Task<int>> subject = async () =>
		{
			await Task.Yield();
			return counter.Value;
		};

		async Task Act()
			=> await That(subject).Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ShouldSupportCancellationTokenSubject()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(token => token.IsCancellationRequested ? -1 : counter.Value)
				.Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ShouldSupportCancellationTokenTaskSubject()
	{
		Counter counter = new(2);

		Func<CancellationToken, Task<int>> subject = async token =>
		{
			await Task.Yield();
			return token.IsCancellationRequested ? -1 : counter.Value;
		};

		async Task Act()
			=> await That(subject).Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldSupportCancellationTokenValueTaskSubject()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(token => new ValueTask<int>(token.IsCancellationRequested ? -1 : counter.Value))
				.Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}
#endif

	[Fact]
	public async Task ShouldSupportOrChain()
	{
		Counter counter = new();

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsEqualTo(1).Or.IsEqualTo(2)
				.WithTimeout(VeryLowTimeout);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => counter.Value
			             is equal to 1 or is equal to 2 within 0:00.050,
			             but it was 0 which differs by -1 and it was 0 which differs by -2
			             """);
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldSupportValueTaskSubject()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(() => new ValueTask<int>(counter.Value)).Eventually().IsGreaterThan(3)
				.WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}
#endif

	[Fact]
	public async Task WhenAndChainFails_ShouldNotRepeatTheExpectationPerAttempt()
	{
		Counter counter = new();

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsGreaterThan(1).And.IsLessThan(0)
				.WithTimeout(LowTimeout);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => counter.Value
			             is greater than 1 and is less than 0 within 0:00.500,
			             but it was 0
			             """);
		await That(counter.EvaluationCount).IsGreaterThan(1);
	}

	[Fact(Skip="Test is brittle")]
	public async Task WhenCancelledDuringTheWaitThatConsumesTheTimeout_ShouldBeInconclusive()
	{
		using CancellationTokenSource cts = new(100.Milliseconds());
		Counter counter = new();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(30.Seconds()))
		{
			async Task Act()
				=> await That(() => counter.Value).Eventually().IsEqualTo(1)
					.WithTimeout(LowTimeout)
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1,
				             but it could not be verified, because it was already cancelled
				             """);
		}
	}

	[Fact]
	public async Task WhenCancellationTokenIsCancelled_ShouldBeInconclusiveWithoutWaitingForTheTimeout()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		Counter counter = new();
		Stopwatch stopwatch = new();

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsEqualTo(1)
				.WithTimeout(30.Seconds())
				.WithCancellation(cts.Token);

		stopwatch.Start();
		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that () => counter.Value
			             is equal to 1,
			             but it could not be verified, because it was already cancelled
			             """);
		stopwatch.Stop();

		await That(stopwatch.Elapsed).IsLessThan(5.Seconds());
	}

	[Fact]
	public async Task WhenIntervalExceedsTheMaximumDelay_ShouldStillBeCancellable()
	{
		using CancellationTokenSource cts = new(VeryLowTimeout);
		Counter counter = new();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(TimeSpan.FromDays(60)))
		{
			async Task Act()
				=> await That(() => counter.Value).Eventually().IsEqualTo(1)
					.WithTimeout(System.Threading.Timeout.InfiniteTimeSpan)
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1,
				             but it could not be verified, because it was already cancelled
				             """)
				.Because("an interval that `Task.Delay` cannot represent must not escape as an exception, " +
				         "especially when the retry budget does not limit it either");
		}
	}

	[Fact]
	public async Task WhenIntervalIsLongerThanTheTimeout_ShouldEvaluateTwice()
	{
		Stopwatch stopwatch = new();
		List<int> evaluationCounts = [];

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(30.Seconds()))
		{
			stopwatch.Start();

			for (int i = 0; i < 20; i++)
			{
				Counter counter = new();

				async Task Act()
					=> await That(() => counter.Value).Eventually().IsEqualTo(1).WithTimeout(VeryLowTimeout);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that () => counter.Value
					             is equal to 1 within 0:00.050,
					             but it was 0 which differs by -1
					             """);
				evaluationCounts.Add(counter.EvaluationCount);
			}

			stopwatch.Stop();
		}

		await That(evaluationCounts).All().AreEqualTo(2)
			.Because("the last wait is shortened to the remaining budget instead of waiting the whole interval");
		await That(stopwatch.Elapsed).IsLessThan(30.Seconds());
	}

	[Fact]
	public async Task WhenSubjectAlwaysThrows_ShouldFailWithTheExceptionAsCause()
	{
		static int AlwaysThrows() => throw new MyException("always broken");

		async Task Act()
			=> await That(() => AlwaysThrows()).Eventually().IsEqualTo(1).WithTimeout(VeryLowTimeout);

		XunitException exception = await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => AlwaysThrows()
			             is equal to 1 within 0:00.050,
			             but it did throw a ThatDelegateEventuallyTests.MyException

			             Exception:
			             *
			             """).AsWildcard();
		await That(exception.InnerException).Is<MyException>()
			.Whose(e => e.Message, m => m.IsEqualTo("always broken"));
	}

	[Fact]
	public async Task WhenSubjectBecomesValid_ShouldSucceed()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsGreaterThan(3).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
		await That(counter.EvaluationCount).IsEqualTo(4);
	}

	[Theory]
	[InlineData("Func<TValue>")]
	[InlineData("Func<CancellationToken, TValue>")]
	[InlineData("Func<Task<TValue>>")]
	[InlineData("Func<CancellationToken, Task<TValue>>")]
#if NET8_0_OR_GREATER
	[InlineData("Func<ValueTask<TValue>>")]
	[InlineData("Func<CancellationToken, ValueTask<TValue>>")]
#endif
	public async Task WhenSubjectIsNull_ShouldFail(string overload)
	{
		async Task Act()
		{
			switch (overload)
			{
				case "Func<CancellationToken, TValue>":
					await That((Func<CancellationToken, int>)null!).Eventually().IsEqualTo(1);
					break;
				case "Func<Task<TValue>>":
					await That((Func<Task<int>>)null!).Eventually().IsEqualTo(1);
					break;
				case "Func<CancellationToken, Task<TValue>>":
					await That((Func<CancellationToken, Task<int>>)null!).Eventually().IsEqualTo(1);
					break;
#if NET8_0_OR_GREATER
				case "Func<ValueTask<TValue>>":
					await That((Func<ValueTask<int>>)null!).Eventually().IsEqualTo(1);
					break;
				case "Func<CancellationToken, ValueTask<TValue>>":
					await That((Func<CancellationToken, ValueTask<int>>)null!).Eventually().IsEqualTo(1);
					break;
#endif
				case "Func<TValue>":
					await That((Func<int>)null!).Eventually().IsEqualTo(1);
					break;
				default:
					throw new ArgumentException($"Unknown overload: {overload}", nameof(overload));
			}
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("*but it was <null>").AsWildcard();
	}

	[Fact]
	public async Task WhenSubjectThrowsOnlyAtFirst_ShouldSucceed()
	{
		Counter counter = new(2);

		async Task Act()
			=> await That(() => counter.Value > 3 ? counter.Value : throw new MyException("not yet")).Eventually()
				.IsGreaterThan(3)
				.WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenTestCancellationExpiresWithTheTimeout_ShouldFailAndNotBeInconclusive()
	{
		Counter counter = new();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(VeryLowTimeout))
		using (IDisposable ___ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(VeryLowTimeout)))
		{
			async Task Act() => await That(() => counter.Value).Eventually().IsEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1 within 0:00.050,
				             but it was 0 which differs by -1
				             """);
		}
	}

	[Fact]
	public async Task WhenTestCancellationTimeoutExpires_ShouldBeInconclusive()
	{
		Counter counter = new();
		Stopwatch stopwatch = new();

		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(VeryLowTimeout)))
		{
			async Task Act() => await That(() => counter.Value).Eventually().IsEqualTo(1);

			stopwatch.Start();
			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that () => counter.Value
				             is equal to 1,
				             but it could not be verified, because it was already cancelled
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThan(5.Seconds());
	}

	[Fact]
	public async Task WhenTheConstraintAddsAContext_ShouldNotDuplicateItPerAttempt()
	{
		Counter counter = new();
		List<int> subject = [1, 2,];

		IEnumerable<int> Subject()
		{
			_ = counter.Value;
			return subject;
		}

		async Task Act()
			=> await That(Subject).Eventually().IsEqualTo([3, 4,]).WithTimeout(LowTimeout);

		XunitException exception = await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that Subject
			             matches collection [3, 4,] in order within 0:00.500,
			             but it*
			             """).AsWildcard();
		await That(counter.EvaluationCount).IsGreaterThan(1);
		await That(CountOccurrences(exception.Message, "Expected:")).IsEqualTo(1)
			.Because("the context must be added once, and not once per attempt");
	}

	[Fact]
	public async Task WhenTheSubjectContentChanges_ShouldNotReuseTheCachedEnumerable()
	{
		List<int> subject = [];

		static IEnumerable<int> Lazy(List<int> items)
		{
			foreach (int item in items)
			{
				yield return item;
			}
		}

		async Task Act()
			=> await That(() =>
			{
				subject.Add(subject.Count + 1);
				return Lazy(subject);
			}).Eventually().IsEqualTo([1, 2, 3,]).WithTimeout(SuccessTimeout);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenTimeoutExpires_ShouldFailWithTheLastObservedValue()
	{
		int observed = 0;

		async Task Act()
			=> await That(() => observed = Math.Min(observed + 1, 3)).Eventually().IsEqualTo(0)
				.WithTimeout(LowTimeout);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => observed = Math.Min(observed + 1, 3)
			             is equal to 0 within 0:00.500,
			             but it was 3 which differs by 3
			             """);
	}

	[Fact]
	public async Task WhenTimeoutIsInfinite_ShouldKeepRetrying()
	{
		Counter counter = new(2);
		using CancellationTokenSource cts = new(SuccessTimeout);

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsGreaterThan(3)
				.WithTimeout(System.Threading.Timeout.InfiniteTimeSpan)
				.WithCancellation(cts.Token);

		await That(Act).DoesNotThrow();
		await That(counter.EvaluationCount).IsEqualTo(4);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-500)]
	public async Task WhenTimeoutIsNotPositive_ShouldOnlyEvaluateOnce(int timeoutInMilliseconds)
	{
		Counter counter = new();

		async Task Act()
			=> await That(() => counter.Value).Eventually().IsEqualTo(1)
				.WithTimeout(TimeSpan.FromMilliseconds(timeoutInMilliseconds));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => counter.Value
			             is equal to 1 within 0:00,
			             but it was 0 which differs by -1
			             """);
		await That(counter.EvaluationCount).IsEqualTo(1);
	}

	/// <summary>
	///     A timeout that is long enough to allow multiple check intervals, but short enough to keep the tests fast.
	/// </summary>
	private TimeSpan LowTimeout { get; } = TimeSpan.FromMilliseconds(500);

	/// <summary>
	///     A timeout for expectations that are expected to succeed. It has enough headroom to stay reliable on a
	///     loaded machine, but still fails reasonably fast when the retry logic breaks.
	/// </summary>
	private TimeSpan SuccessTimeout { get; } = TimeSpan.FromSeconds(5);

	/// <summary>
	///     A timeout that expires after a single check interval.
	/// </summary>
	private TimeSpan VeryLowTimeout { get; } = TimeSpan.FromMilliseconds(50);

	private static int CountOccurrences(string value, string needle)
	{
		int count = 0;
		int index = value.IndexOf(needle, StringComparison.Ordinal);
		while (index >= 0)
		{
			count++;
			index = value.IndexOf(needle, index + needle.Length, StringComparison.Ordinal);
		}

		return count;
	}

	private sealed class Counter(int validAfter = int.MaxValue)
	{
		private int _evaluationCount;

		public int EvaluationCount => _evaluationCount;

		public int Value
		{
			get
			{
				int count = Interlocked.Increment(ref _evaluationCount);
				return count > validAfter ? count : 0;
			}
		}
	}

	private sealed class MyException(string message) : Exception(message);
}
