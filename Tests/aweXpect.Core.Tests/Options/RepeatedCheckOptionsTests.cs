using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Options;

public sealed class RepeatedCheckOptionsTests
{
	[Fact]
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

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item,
			             but the predicate did throw a MyException:
			               not yet
			             """);
		await That(checks).IsEqualTo(1)
			.Because("without a timeout the check is not repeated");
	}

	[Fact]
	public async Task CheckRepeatedly_WhenRetrying_ShouldReleaseTheMaterializedSourceOfEachCheck()
	{
		int checks = 0;
		DisposeTrackingEnumerable subject = new(null, 1, 2, 3);

		async Task Act()
			=> await HasMatchingItem(That<IEnumerable<int>>(subject), _ => false, () => checks++)
				.Within(500.Milliseconds()).CheckEvery(10.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             has a matching item within 0:00.500,
			             but it had none in [1, 2, 3]
			             """);
		await That(checks).IsGreaterThan(1);
		await That(subject.DisposeCount).IsEqualTo(checks)
			.Because("every check reads the subject again, and the source of each check is released exactly once");
	}

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task CheckRepeatedly_WhenUserCodeAlwaysThrows_ShouldKeepCheckingAndFailWithTheLastException()
	{
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
				.Within(500.Milliseconds()).CheckEvery(10.Milliseconds());

		Exception? exception = await Record.ExceptionAsync(Act);

		await That(exception).IsExactly<XunitException>().And
			.HasMessage("""
			            Expected that subject
			            has a matching item within 0:00.500,
			            but the predicate did throw a MyException:
			              failure *
			            """).AsWildcard();
		await That(exception?.InnerException).IsSameAs(lastThrown)
			.Because("the exception of the last check is reported");
		await That(checks).IsGreaterThan(1)
			.Because("an exception of the code of the caller must not end the repeated check early");
	}

	[Fact]
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
