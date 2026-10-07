using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Options;

public sealed class RepeatedCheckOptionsTests
{
	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task CheckEvery_WhenIntervalIsNotPositive_ShouldThrowArgumentOutOfRangeException(int milliseconds)
	{
		RepeatedCheckOptions sut = new();

		void Act() => sut.CheckEvery(milliseconds.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("interval").And
			.WithMessage("The interval must be positive.").AsPrefix();
	}

	[Test]
	public async Task CheckRepeatedly_WhenNotRepeatedAndUserCodeThrows_ShouldFailWithTheException()
	{
		int checks = 0;
		IEnumerable<int> subject = Enumerable.Range(1, 3);

		async Task Act()
			=> await HasMatchingItem(That(subject), _ =>
			{
				checks++;
				throw new MyException("not yet");
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item,
			             but the predicate did throw a MyException:
			               not yet
			             """);
		await That(checks).IsEqualTo(1)
			.Because("without a timeout the check is not repeated");
	}

	[Test]
	public async Task CheckRepeatedly_WhenACheckTakesLongerThanTheTimeout_ShouldNotCheckAgain()
	{
		VirtualTimeSystem time = new();
		int checks = 0;
		IEnumerable<int> subject = Enumerable.Range(1, 3);

		async Task Act()
			=> await HasMatchingItem(That(subject), _ => false, () =>
				{
					checks++;
					time.Advance(600.Milliseconds());
				})
				.Within(500.Milliseconds()).CheckEvery(10.Milliseconds()).WithTimeSystem(time);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item within 0:00.500,
			             but it had none in [1, 2, 3]
			             """);
		await That(checks).IsEqualTo(1)
			.Because("the timeout is measured from before the first check");
		await That(time.Now).IsEqualTo(600.Milliseconds());
	}

	[Test]
	public async Task CheckRepeatedly_WhenRetrying_ShouldReleaseTheMaterializedSourceOfEachCheck()
	{
		VirtualTimeSystem time = new();
		int checks = 0;
		DisposeTrackingEnumerable subject = new(null, 1, 2, 3);

		async Task Act()
			=> await HasMatchingItem(That<IEnumerable<int>>(subject), _ => false, () => checks++)
				.Within(500.Milliseconds()).CheckEvery(10.Milliseconds()).WithTimeSystem(time);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item within 0:00.500,
			             but it had none in [1, 2, 3]
			             """);
		await That(checks).IsEqualTo(51)
			.Because("the checks are made at once and then every 10 ms until the timeout");
		await That(time.Now).IsEqualTo(500.Milliseconds());
		await That(subject.DisposeCount).IsEqualTo(checks)
			.Because("every check reads the subject again, and the source of each check is released exactly once");
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheContextIsOfAnotherImplementation_ShouldUseTheRealTimeSystem()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(30.Seconds());
		sut.CheckEvery(50.Milliseconds());
		int checks = 0;
		Stopwatch stopwatch = Stopwatch.StartNew();

		Outcome outcome = await sut.CheckRepeatedly(_ => new ValueTask<bool>(++checks == 2),
			new ForeignEvaluationContext());

		await That(outcome).IsEqualTo(Outcome.Success)
			.Because("the second check is met long before the budget ends, however late the first one starts");
		await That(stopwatch.Elapsed).IsGreaterThanOrEqualTo(40.Milliseconds())
			.Because("the checks wait in real time, and a timer can complete a few milliseconds before the stopwatch agrees");
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheEvaluationIsCanceled_ShouldBeUndecided()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(30.Seconds());
		int checks = 0;
		EvaluationCancellation cancellation = EvaluationCancellation.Create(null, new CancellationToken(true));

		Outcome outcome = await sut.CheckRepeatedly(_ =>
		{
			checks++;
			return new ValueTask<bool>(false);
		}, new ForeignEvaluationContext(cancellation));

		await That(outcome).IsEqualTo(Outcome.Undecided)
			.Because("a cancellation by the caller does not count as the timeout having elapsed");
		await That(checks).IsEqualTo(1);
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheFirstCheckIsMet_ShouldNotCheckAgain()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(30.Seconds());
		int checks = 0;

		Outcome outcome = await sut.CheckRepeatedly(_ =>
		{
			checks++;
			return new ValueTask<bool>(true);
		}, new ForeignEvaluationContext());

		await That(outcome).IsEqualTo(Outcome.Success);
		await That(checks).IsEqualTo(1);
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheIntervalExceedsTheTimeout_ShouldCheckAtTheTimeoutAndNotAfterIt()
	{
		VirtualTimeSystem time = new();
		List<TimeSpan> checks = [];
		IEnumerable<int> subject = Enumerable.Range(1, 3);

		async Task Act()
			=> await HasMatchingItem(That(subject), _ => checks.Count > 2, () => checks.Add(time.Now))
				.Within(100.Milliseconds()).CheckEvery(6.Seconds()).WithTimeSystem(time);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item within 0:00.100,
			             but it had none in [1, 2, 3]
			             """)
			.Because("the third check would succeed, but no check is made after the timeout");
		await That(checks).IsEqualTo([TimeSpan.Zero, 100.Milliseconds(),])
			.Because("the wait is shortened to the remaining time, so the last check is made at the timeout");
		await That(time.Now).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheSourceThrowsAtFirst_ShouldReadItAgain()
	{
		int enumerations = 0;

		IEnumerable<int> Items()
		{
			if (++enumerations == 1)
			{
				throw new MyException("not yet");
			}

			yield return 1;
		}

		IEnumerable<int> subject = Items();

		async Task Act()
			=> await HasMatchingItem(That(subject), item => item == 1)
				.Within(30.Seconds()).CheckEvery(1.Milliseconds());

		await That(Act).DoesNotThrow()
			.Because("an exception of the source must be retried instead of being replayed in every check");
		await That(enumerations).IsEqualTo(2);
	}

	[Test]
	public async Task CheckRepeatedly_WhenTheSubjectGrows_ShouldSeeTheNewItems()
	{
		ConcurrentQueue<int> received = new();
		int checks = 0;
		IEnumerable<int> subject = received.Select(x => x);

		async Task Act()
			=> await HasMatchingItem(That(subject), item => item == 1, () =>
				{
					if (++checks == 3)
					{
						received.Enqueue(1);
					}
				})
				.Within(30.Seconds()).CheckEvery(1.Milliseconds());

		await That(Act).DoesNotThrow()
			.Because("each check must read the lazy subject again instead of replaying the first snapshot");
		await That(checks).IsEqualTo(3);
	}

	[Test]
	public async Task CheckRepeatedly_WhenUserCodeAlwaysThrows_ShouldKeepCheckingAndFailWithTheLastException()
	{
		VirtualTimeSystem time = new();
		int checks = 0;
		MyException? lastThrown = null;
		IEnumerable<int> subject = Enumerable.Range(1, 3);

		async Task Act()
			=> await HasMatchingItem(That(subject), _ =>
					{
						lastThrown = new MyException($"failure {checks}");
						throw lastThrown;
					},
					() => checks++)
				.Within(500.Milliseconds()).CheckEvery(10.Milliseconds()).WithTimeSystem(time);

		Exception? exception = await Catch.ExceptionAsync(Act);

		await That(exception).IsExactly<FailException>().And
			.HasMessage("""
			            Expected that subject
			            has a matching item within 0:00.500,
			            but the predicate did throw a MyException:
			              failure *
			            """).AsWildcard();
		await That(exception?.InnerException).IsSameAs(lastThrown)
			.Because("the exception of the last check is reported");
		await That(checks).IsEqualTo(51)
			.Because("an exception of the code of the caller must not end the repeated check early");
	}

	[Test]
	public async Task CheckRepeatedly_WhenUserCodeThrowsAtFirst_ShouldKeepChecking()
	{
		int checks = 0;
		IEnumerable<int> subject = Enumerable.Range(1, 3);

		async Task Act()
			=> await HasMatchingItem(That(subject),
					item => checks > 2 ? item == 1 : throw new MyException("not yet"),
					() => checks++)
				.Within(30.Seconds()).CheckEvery(1.Milliseconds());

		await That(Act).DoesNotThrow()
			.Because("an exception of the code of the caller counts as not met and is checked again, like in Satisfies");
		await That(checks).IsEqualTo(3);
	}

	/// <summary>
	///     An extension built as the documentation recommends: each check materializes the subject from its evaluation
	///     context and calls the <paramref name="predicate" /> through <see cref="UserCode" />, after the
	///     <paramref name="onCheck" />.
	/// </summary>
	private static RepeatedCheckResult<IEnumerable<int>, IThat<IEnumerable<int>>> HasMatchingItem(
		IThat<IEnumerable<int>> subject, Func<int, bool> predicate, Action? onCheck = null)
	{
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<IEnumerable<int>, IThat<IEnumerable<int>>>(
			((IExpectThat<IEnumerable<int>>)subject).ExpectationBuilder
			.AddConstraint((it, grammars)
				=> new HasMatchingItemConstraint(it, grammars, predicate, onCheck, options)),
			subject,
			options);
	}

	private sealed class ForeignEvaluationContext(EvaluationCancellation? cancellation = null) : IEvaluationContext
	{
		public EvaluationCancellation Cancellation => cancellation ?? EvaluationCancellation.None;

		public void Store<T>(string key, T value) { }

		public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
		{
			value = default;
			return false;
		}
	}

	private sealed class HasMatchingItemConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<int, bool> predicate,
		Action? onCheck,
		RepeatedCheckOptions options)
		: ConstraintResult.WithNotNullValue<IEnumerable<int>>(it, grammars),
			IAsyncContextConstraint<IEnumerable<int>>
	{
		private List<int> _items = [];

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<int> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome outcome = await options.CheckRepeatedly(checkContext =>
			{
				onCheck?.Invoke();
				_items = checkContext.UseMaterializedEnumerable(actual).ToList();
				bool isMatch = _items.Any(item => UserCode.Invoke(predicate, item, "the predicate"));
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				return new ValueTask<bool>(isMatch != IsNegated);
			}, context);
			if (outcome == Outcome.Undecided)
			{
				Outcome = Outcome.Undecided;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a matching item").Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had none in ");
			Formatter.Format(stringBuilder, _items);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has no matching item").Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
