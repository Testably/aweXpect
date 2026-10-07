using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Sources;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests;

public class ExpectTests
{
	[Test]
	public async Task Context_FromSuccess_ShouldNotBeIncludedInMessage()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Failure, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2", new DummyConstraintResult(Outcome.Success, "expectation2"));

		async Task Act()
			=> await ThatAll(
				new MyExpectation(result1, new ResultContext.Fixed("context-title1", "contest-content1")),
				new MyExpectation(result2, new ResultContext.Fixed("context-title2", "contest-content2")));

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task Context_Multiple_ShouldBeIncludedInMessage()
	{
		Expectation.Result result = new(1, "foo", new DummyConstraintResult(Outcome.Failure, "expectation", "result"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result, new ResultContext.Fixed("t1", "c1"), new ResultContext.Fixed("t2", "c2"),
				new ResultContext.Fixed("t3", "c3")));

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task Context_ShouldBeIncludedInMessage()
	{
		Expectation.Result result = new(1, "foo", new DummyConstraintResult(Outcome.Failure, "expectation", "result"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result, new ResultContext.Fixed("context-title", "contest-content")));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			             foo expectation
			             but
			              [01] result

			             [01] context-title:
			             contest-content
			             """);
	}

	[Test]
	public async Task ShouldAwaitValueTaskSubject()
	{
		ValueTask<string?> sut = new(Task.FromResult<string?>(null));

		async Task Act()
			=> await That(sut).IsNotNull();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             is not null,
			             but it was <null>
			             """)
			.Because("the result of the ValueTask must become the subject, not the ValueTask itself");
	}

	[Test]
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

	[Test]
	public async Task ShouldFailForNullGenericTaskSubject()
	{
		Task<int>? sut = null;

		async Task Act()
			=> await That(sut!).IsEqualTo(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             is equal to 1,
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("a null task is no exception thrown by the subject, but a subject without a value");
	}

	[Test]
	public async Task ShouldFailForNullGenericTaskSubject_WhenNegated()
	{
		Task<int>? sut = null;

		async Task Act()
			=> await That(sut!).DoesNotComplyWith(it => it.IsEqualTo(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             is not equal to 1,
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("a null task has no value that could meet the negated expectation");
	}

	[Test]
	public async Task ShouldFailForNullGenericTaskSubject_WhenExpectingNull()
	{
		Task<string?>? sut = null;

		async Task Act()
			=> await That(sut!).IsNull();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             is null,
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("a null task has no value, not a null value");
	}

	[Test]
	public async Task ShouldFailForNullTaskSubject()
	{
		Task? sut = null;

		async Task Act()
			=> await That(sut!).DoesNotThrow();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("a null task subject is no null delegate and no exception thrown by the subject");
	}

	[Test]
	public async Task ShouldFailForNullTaskSubject_WhenExpectingAnException()
	{
		Task? sut = null;

		async Task Act()
			=> await That(sut!).Throws<InvalidOperationException>().WithMessage("foo");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             throws an InvalidOperationException with message equal to "foo",
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("the expectation on the exception cannot be verified without a task");
	}

	[Test]
	public async Task ShouldFailForNullTaskSubject_WhenNegated()
	{
		Task? sut = null;

		async Task Act()
			=> await That(sut!).Throws().OnlyIf(false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it was a <null> task
			             """).And
			.Whose(e => e.InnerException, i => i.IsNull())
			.Because("a null task neither throws nor completes successfully");
	}

	[Test]
	public async Task ShouldObserveExceptionOfTaskSubject()
	{
		Task sut = Task.FromException(new InvalidOperationException("my exception"));

		async Task Act()
			=> await That(sut).DoesNotThrow();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it did throw an InvalidOperationException:
			               my exception
			             """);
	}

	[Test]
	public async Task ShouldObserveExceptionOfValueTaskSubject()
	{
		ValueTask sut = new(Task.FromException(new InvalidOperationException("my exception")));

		async Task Act()
			=> await That(sut).DoesNotThrow();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             does not throw any exception,
			             but it did throw an InvalidOperationException:
			               my exception
			             """);
	}

	[Test]
	public async Task ShouldSupportCollectionExpressionsAsSubject()
	{
		async Task Act()
			=> await That([1, 2, 3,]).IsInAscendingOrder();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ShouldSupportTaskAsSubject()
	{
		Task<int> sut = Task.FromResult(42);

		async Task Act()
			=> await That(sut).IsGreaterThan(41);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ShouldSupportValueTaskAsSubject()
	{
		ValueTask<int> sut = new(42);

		async Task Act()
			=> await That(sut).IsGreaterThan(41);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ThatAll_Result_ShouldNotBeNegated()
	{
		Expectation.Combination sut = ThatAll(new MyExpectation(new Expectation.Result(1, "foo",
			new DummyConstraintResult(Outcome.Failure, "expectation", "result"))));
		ConstraintResult result = (await sut.GetResult(0)).ConstraintResult;

		ConstraintResult negated = result.Negate();

		await That(negated).IsSameAs(result);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure)
			.Because("a combination is never negated");
	}

	[Test]
	public async Task ThatAll_Result_ShouldNotStoreAValue()
	{
		Expectation.Combination sut = ThatAll(new MyExpectation(new Expectation.Result(1, "foo",
			new DummyConstraintResult(42, Outcome.Success, "expectation"))));
		ConstraintResult result = (await sut.GetResult(0)).ConstraintResult;

		bool hasValue = result.TryGetStoredValue(out int value);

		await That(hasValue).IsFalse()
			.Because("a combination of several expectations has no single subject");
		await That(value).IsEqualTo(0);
	}

	[Test]
	public async Task ThatAll_WhenACombinationWithAMultiLineValueFails_ShouldIndentTheValueLikeTheEntry()
	{
		MyClass expected = new()
		{
			Value = 1,
		};
		MyClass subject = new()
		{
			Value = 1,
		};

		async Task Act()
			=> await ThatAll(That(subject).IsSameAs(expected).And.IsNotNull());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject refers to ExpectTests.MyClass {
			                     Value = 1
			                   } and is not null
			             but
			              [01] it was ExpectTests.MyClass {
			                     Value = 1
			                   }
			             """)
			.Because("the operands of a combination are indented like a single expectation");
	}

	[Test]
	public async Task ThatAll_WhenAMemberExpectationWithAMultiLineValueFails_ShouldIndentTheValueLikeTheEntry()
	{
		MyClass expected = new()
		{
			Value = 1,
		};
		MyHolder subject = new(new MyClass
		{
			Value = 1,
		});

		async Task Act()
			=> await ThatAll(That(subject).Is<MyHolder>().Whose(h => h.Inner, i => i.IsSameAs(expected)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is of type ExpectTests.MyHolder whose Inner refers to ExpectTests.MyClass {
			                     Value = 1
			                   }
			             but
			              [01] Inner was ExpectTests.MyClass {
			                     Value = 1
			                   }
			             """)
			.Because("the expectation on a member is indented like a single expectation");
	}

	[Test]
	public async Task ThatAll_WhenAMemberFails_ShouldReleaseItsMaterializedSourceAfterTheFailureMessage()
	{
		DisposeTrackingEnumerable source = new(null, Enumerable.Range(1, 20).ToArray());
		ReadsFirstItemConstraint constraint = new(source, Outcome.Failure);
		IEnumerable<int> subject = source;

		async Task Act()
			=> await ThatAll(
				new ExpectationResult(That(subject).Get().ExpectationBuilder.AddConstraint((_, _) => constraint)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject reads the first item
			             but
			              [01] it was [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and maybe more)]
			             """)
			.Because("the failure message still reads from the source");
		await That(constraint.DisposeCountWhenListed).IsEqualTo(0);
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the source is released once the failure message is created");
	}

	[Test]
	public async Task ThatAll_WhenANestedCombinationFails_ShouldIncludeTheContextsOfItsFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAll(
					Member(1, Outcome.Failure),
					Member(2, Outcome.Success)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<FailException>()
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

	[Test]
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

		await That(Act).Throws<InconclusiveTestException>()
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

	[Test]
	public async Task ThatAll_WhenANestedCombinationIsUndecided_ShouldIncludeTheContextsOfItsUndecidedAndFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Undecided),
					Member(2, Outcome.Failure)),
				Member(3, Outcome.Success));

		await That(Act).Throws<InconclusiveTestException>()
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

	[Test]
	public async Task ThatAll_WhenANestedCombinationSucceeds_ShouldExcludeTheContextsOfItsFailedMembers()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Failure),
					Member(2, Outcome.Success)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task ThatAll_WhenAWhichExpectationWithAMultiLineValueFails_ShouldIndentTheValueLikeTheEntry()
	{
		MyClass expected = new()
		{
			Value = 1,
		};
		MyClass[] subject =
		[
			new()
			{
				Value = 1,
			},
		];

		async Task Act()
			=> await ThatAll(That(subject).HasSingle().Which.IsSameAs(expected));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject has a single item that refers to ExpectTests.MyClass {
			                     Value = 1
			                   }
			             but
			              [01] it was ExpectTests.MyClass {
			                     Value = 1
			                   }
			             """)
			.Because("the expectation on the item is indented like a single expectation");
	}

	[Test]
	public async Task ThatAll_WhenANestedCombinationSucceeds_ShouldNumberTheContextsOfLaterMembersLikeTheirResults()
	{
		async Task Act()
			=> await ThatAll(
				ThatAny(
					Member(1, Outcome.Success),
					Member(2, Outcome.Failure)),
				Member(3, Outcome.Failure));

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task ThatAll_WhenAnExpectationDoesNotDecideItsOutcome_ShouldFail()
	{
		async Task Act()
			=> await ThatAll(
				new ExpectationResult(That(1).Get().ExpectationBuilder.AddConstraint((_, _)
					=> new DummyConstraint("decides nothing",
						() => new DummyConstraintResult(Outcome.Undecided, "decides nothing", "it was 1")))),
				That(true).IsTrue());

		await That(Act).ThrowsExactly<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that 1 decides nothing
			              [02] Expected that true is True
			             but
			              [01] it could not be verified, because the expectation did not decide its outcome
			             """)
			.Because("an expectation that is left undecided without a cancellation fails the combination");
	}

	[Test]
	public async Task ThatAll_WhenAnExpectationIsNull_ShouldThrowArgumentException()
	{
		async Task Act()
			=> await ThatAll(That(1).IsEqualTo(1), null!);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot contain null.").AsPrefix();
	}

	[Test]
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

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task ThatAll_WhenExpectationsIsEmpty_ShouldThrowArgumentException()
	{
		async Task Act()
			=> await ThatAll();

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expectations").And
			.WithMessage("You must provide at least one expectation.").AsPrefix();
	}

	[Test]
	public async Task ThatAll_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		async Task Act()
			=> await ThatAll(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ThatAll_WhenOneExpectationFailsAndAnotherIsUndecided_ShouldFail()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Failure, "expectation2", "result2"));

		async Task Act()
			=> await ThatAll(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task ThatAll_WhenOneExpectationIsCanceledAndTheOtherSucceeds_ShouldBeInconclusive()
	{
		Task<int> subject = PendingTask.Of<int>();
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await ThatAll(
				That(subject).IsEqualTo(1).WithCancellation(cts.Token),
				That(true).IsTrue());

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is equal to 1
			              [02] Expected that true is True
			             but
			              [01] it could not be verified, because the evaluation was already canceled
			             """)
			.Because("the canceled expectation was not verified, so not all of them succeeded");
	}

	[Test]
	public async Task ThatAny_WhenAllExpectationsAreUndecided_ShouldBeInconclusive()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Undecided, "expectation2", "result2"));

		async Task Act()
			=> await ThatAny(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			             foo1 expectation1
			             foo2 expectation2
			             but
			              [01] result1
			              [02] result2
			             """);
	}

	[Test]
	public async Task ThatAny_WhenAnExpectationIsNull_ShouldThrowArgumentException()
	{
		async Task Act()
			=> await ThatAny(That(1).IsEqualTo(1), null!);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot contain null.").AsPrefix();
	}

	[Test]
	public async Task ThatAny_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		async Task Act()
			=> await ThatAny(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ThatAny_WhenOneExpectationFailsAndAnotherIsUndecided_ShouldBeInconclusive()
	{
		Expectation.Result result1 = new(1, "foo1",
			new DummyConstraintResult(Outcome.Undecided, "expectation1", "result1"));
		Expectation.Result result2 = new(2, "foo2",
			new DummyConstraintResult(Outcome.Failure, "expectation2", "result2"));

		async Task Act()
			=> await ThatAny(new MyExpectation(result1), new MyExpectation(result2));

		await That(Act).Throws<InconclusiveTestException>()
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

	[Test]
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

		await That(Act).Throws<FailException>()
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

	private sealed class MyClass
	{
		public int Value { get; set; }
	}

	private sealed class MyHolder(MyClass inner)
	{
		public MyClass Inner { get; } = inner;
	}

	private sealed class MyExpectation(Expectation.Result result, params ResultContext[] contexts) : Expectation
	{
		internal override Task<Result> GetResult(int index)
			=> Task.FromResult(new Result(result.Index, result.SubjectLine,
				new WithContexts(result.ConstraintResult, contexts)));

		internal override Task EndEvaluation()
			=> Task.CompletedTask;

		internal override Task ResolvePendingReasons()
			=> Task.CompletedTask;

		internal override void UseTimeSystem(ITimeSystem timeSystem) { }
	}

	/// <summary>
	///     The <paramref name="inner" /> result, which adds the <paramref name="contexts" /> when it explains a failure.
	/// </summary>
	private sealed class WithContexts(ConstraintResult inner, ResultContext[] contexts)
		: ConstraintResult(inner.FurtherProcessingStrategy)
	{
		public override Outcome Outcome
		{
			get => inner.Outcome;
			protected set => _ = value;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendResult(stringBuilder, indentation);

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> inner.TryGetStoredValue(out value);

		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}

		public override void AppendContexts(ResultContextCollector collector)
		{
			foreach (ResultContext context in contexts)
			{
				collector.Add(context);
			}
		}
	}
}
