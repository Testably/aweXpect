using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Sources;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests;

public class ExpectTests
{
	[Fact]
	public async Task Context_FromSuccess_ShouldNotBeIncludedInMessage()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Failure, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2", new DummyConstraintResult(Outcome.Success, "expectation2"));

		async Task Act()
			=> await ThatAll(
				new MyExpectation(result1, new ResultContext.Fixed("context-title1", "contest-content1")),
				new MyExpectation(result2, new ResultContext.Fixed("context-title2", "contest-content2")));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			             foo1 expectation1
			             foo2 expectation2
			             but
			              [01] result1

			             [01] context-title1:
			             contest-content1
			             """);
	}

	[Fact]
	public async Task Context_Multiple_ShouldBeIncludedInMessage()
	{
		Expectation.Result result = new(1, "foo", new DummyConstraintResult(Outcome.Failure, "expectation", "result"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result, new ResultContext.Fixed("t1", "c1"), new ResultContext.Fixed("t2", "c2"),
				new ResultContext.Fixed("t3", "c3")));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			             foo expectation
			             but
			              [01] result

			             [01] t1:
			             c1

			             [01] t2:
			             c2

			             [01] t3:
			             c3
			             """);
	}

	[Fact]
	public async Task Context_ShouldBeIncludedInMessage()
	{
		Expectation.Result result = new(1, "foo", new DummyConstraintResult(Outcome.Failure, "expectation", "result"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result, new ResultContext.Fixed("context-title", "contest-content")));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			             foo expectation
			             but
			              [01] result

			             [01] context-title:
			             contest-content
			             """);
	}

	[Fact]
	public async Task ShouldAwaitValueTaskSubject()
	{
		ValueTask<string?> sut = new(Task.FromResult<string?>(null));

		async Task Act()
			=> await That(sut).IsNotNull();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is not null,
			             but it was
			             """)
			.Because("the result of the ValueTask must become the subject, not the ValueTask itself");
	}

	[Fact]
	public async Task ShouldConsumeValueTaskSubjectOnlyOnce()
	{
		ValueTask sut = new(new SingleUseValueTaskSource(), 0);
#pragma warning disable aweXpect0001
		ExpectationResult result = That(sut).DoesNotThrow();
#pragma warning restore aweXpect0001

		async Task Act()
		{
			await result;
			await result;
		}

		await That(Act).DoesNotThrow()
			.Because("the ValueTask must be consumed when the expectation is created, not on every evaluation");
	}

	[Fact]
	public async Task ShouldFailForNullTaskSubject()
	{
		Task? sut = null;

		async Task Act()
			=> await That(sut!).DoesNotThrow();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it was <null>
			             """);
	}

	[Fact]
	public async Task ShouldObserveExceptionOfTaskSubject()
	{
		Task sut = Task.FromException(new InvalidOperationException("my exception"));

		async Task Act()
			=> await That(sut).DoesNotThrow();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it did throw an InvalidOperationException:
			               my exception
			             """);
	}

	[Fact]
	public async Task ShouldObserveExceptionOfValueTaskSubject()
	{
		ValueTask sut = new(Task.FromException(new InvalidOperationException("my exception")));

		async Task Act()
			=> await That(sut).DoesNotThrow();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it did throw an InvalidOperationException:
			               my exception
			             """);
	}

	[Fact]
	public async Task ShouldSupportCollectionExpressionsAsSubject()
	{
		async Task Act()
			=> await That([1, 2, 3,]).IsInAscendingOrder();

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ShouldSupportTaskAsSubject()
	{
		Task<int> sut = Task.FromResult(42);

		async Task Act()
			=> await That(sut).IsGreaterThan(41);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ShouldSupportValueTaskAsSubject()
	{
		ValueTask<int> sut = new(42);

		async Task Act()
			=> await That(sut).IsGreaterThan(41);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task ThatAll_WhenANestedCombinationFails_ShouldIncludeTheContextsOfItsFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAll(
					Member(1, Outcome.Failure),
					Member(2, Outcome.Success)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected all of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			             foo3 expectation3
			             but
			                [01] result1
			              [03] result3

			             [01] title1:
			             content1

			             [03] title3:
			             content3
			             """);
	}

	[Fact]
	public async Task ThatAll_WhenANestedCombinationIsUndecided_ShouldBeInconclusive()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Failure, "expectation2", "result2"));
		Expectation.Result result3 = new(3, "foo3", new DummyConstraintResult(Outcome.Success, "expectation3"));

		async Task Act()
			=> await ThatAll(
				ThatAny(new MyExpectation(result1), new MyExpectation(result2)),
				new MyExpectation(result3));

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			             foo3 expectation3
			             but
			                [01] result1
			                [02] result2
			             """)
			.Because("an undecided nested combination leaves the outer combination undecided");
	}

	[Fact]
	public async Task ThatAll_WhenANestedCombinationIsUndecided_ShouldIncludeTheContextsOfItsUndecidedAndFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Undecided),
					Member(2, Outcome.Failure)),
				Member(3, Outcome.Success));

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			             foo3 expectation3
			             but
			                [01] result1
			                [02] result2

			             [01] title1:
			             content1

			             [02] title2:
			             content2
			             """)
			.Because("the context of the succeeded expectation after the nested combination must not be included");
	}

	[Fact]
	public async Task ThatAll_WhenANestedCombinationSucceeds_ShouldExcludeTheContextsOfItsFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Failure),
					Member(2, Outcome.Success)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			             foo3 expectation3
			             but
			              [03] result3

			             [03] title3:
			             content3
			             """)
			.Because("the failure of a member of a succeeded combination is not reported, so neither is its context");
	}

	[Fact]
	public async Task ThatAll_WhenANestedCombinationSucceeds_ShouldNumberTheContextsOfLaterMembersLikeTheirResults()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Success),
					Member(2, Outcome.Failure)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			             foo3 expectation3
			             but
			              [03] result3

			             [03] title3:
			             content3
			             """);
	}

	[Fact]
	public async Task ThatAll_WhenDeeplyNested_ShouldNumberTheContextsLikeTheResults()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					ThatAll(
						Member(1, Outcome.Failure),
						Member(2, Outcome.Success)),
					Member(3, Outcome.Failure)),
				Member(4, Outcome.Failure));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			                 Expected all of the following to succeed:
			                 foo1 expectation1
			                 foo2 expectation2
			               foo3 expectation3
			             foo4 expectation4
			             but
			                  [01] result1
			                [03] result3
			              [04] result4

			             [01] title1:
			             content1

			             [03] title3:
			             content3

			             [04] title4:
			             content4
			             """);
	}

	[Fact]
	public async Task ThatAll_WhenOneExpectationFailsAndAnotherIsUndecided_ShouldFail()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Failure, "expectation2", "result2"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			             foo1 expectation1
			             foo2 expectation2
			             but
			              [01] result1
			              [02] result2
			             """)
			.Because("a failed expectation fails all of them, regardless of the undecided one");
	}

	[Fact]
	public async Task ThatAll_WhenOneExpectationIsCanceledAndTheOtherSucceeds_ShouldBeInconclusive()
	{
		Task<int> subject = PendingTask.Of<int>();
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await ThatAll(
				That(subject).IsEqualTo(1).WithCancellation(cts.Token),
				That(true).IsTrue());

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is equal to 1
			              [02] Expected that true is True
			             but
			              [01] it could not be verified, because the evaluation was already canceled
			             """)
			.Because("the canceled expectation was not verified, so not all of them succeeded");
	}

	[Fact]
	public async Task ThatAny_WhenAllExpectationsAreUndecided_ShouldBeInconclusive()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Undecided, "expectation2", "result2"));

		async Task Act()
			=> await ThatAny(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			             foo1 expectation1
			             foo2 expectation2
			             but
			              [01] result1
			              [02] result2
			             """);
	}

	[Fact]
	public async Task ThatAny_WhenOneExpectationFailsAndAnotherIsUndecided_ShouldBeInconclusive()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Failure, "expectation2", "result2"));

		async Task Act()
			=> await ThatAny(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			             foo1 expectation1
			             foo2 expectation2
			             but
			              [01] result1
			              [02] result2
			             """)
			.Because("the undecided expectation could still have succeeded");
	}

	[Fact]
	public async Task ThatAny_WhenSeveralNestedCombinationsFail_ShouldIncludeTheContextsOfTheirFailedMembers()
	{
		async Task Act()
			=> await ThatAny(
				ThatAll(
					Member(1, Outcome.Failure),
					Member(2, Outcome.Success)),
				ThatAll(
					Member(3, Outcome.Success),
					Member(4, Outcome.Failure)),
				Member(5, Outcome.Failure));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			               Expected all of the following to succeed:
			               foo1 expectation1
			               foo2 expectation2
			               Expected all of the following to succeed:
			               foo3 expectation3
			               foo4 expectation4
			             foo5 expectation5
			             but
			                [01] result1
			                [04] result4
			              [05] result5

			             [01] title1:
			             content1

			             [04] title4:
			             content4

			             [05] title5:
			             content5
			             """);
	}

	private static MyExpectation Member(int index, Outcome outcome)
		=> new(new Expectation.Result(index, $"foo{index}",
				new DummyConstraintResult(outcome, $"expectation{index}", $"result{index}")),
			new ResultContext.Fixed($"title{index}", $"content{index}"));

	/// <remarks>
	///     A <see cref="ValueTask" /> backed by this source detects a second consumption, which a
	///     <see cref="Task" />-backed one would silently allow.
	/// </remarks>
	private sealed class SingleUseValueTaskSource : IValueTaskSource
	{
		private bool _isConsumed;

		#region IValueTaskSource Members

		public ValueTaskSourceStatus GetStatus(short token) => ValueTaskSourceStatus.Succeeded;

		public void OnCompleted(Action<object?> continuation, object? state, short token,
			ValueTaskSourceOnCompletedFlags flags)
			=> continuation(state);

		public void GetResult(short token)
		{
			if (_isConsumed)
			{
				throw new InvalidOperationException("The ValueTask was consumed more than once.");
			}

			_isConsumed = true;
		}

		#endregion
	}

	private sealed class MyExpectation(Expectation.Result result, params ResultContext[] contexts) : Expectation
	{
		internal override Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes)
			=> Task.FromResult(result);

		internal override IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes)
			=> contexts;
	}
}
