using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public class ManualExpectationBuilderTests
{
	[Test]
	public async Task AddAsyncContextValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new();
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyAsyncContextConstraint<int>(_
				=> Task.FromResult<ConstraintResult>(new DummyConstraint<int>(_ => true)));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Test]
	public async Task AddAsyncValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new();
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyAsyncConstraint<int>(_
				=> Task.FromResult<ConstraintResult>(new DummyConstraint<int>(_ => true)));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Test]
	public async Task AddContextValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new();
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyContextConstraint<int>(_ => new DummyConstraint<int>(_ => true));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Test]
	public async Task AddValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new();
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyConstraint("");
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Test]
	public async Task AppendExpectation_ShouldNotAppendReasons()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason("of a");
		StringBuilder sb = new();

		sut.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("is foo")
			.Because("the reasons follow the whole expectation that the expectations are nested in");
	}

	[Test]
	public async Task AppendReasons_ShouldAppendAllReasons()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason("of a");
		sut.AddReason("because of b");
		StringBuilder sb = new();

		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEqualTo(", because of a, because of b");
	}

	[Test]
	public async Task AppendReasons_ShouldOmitReasonsThatMustBeAwaited()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder sb = new();

		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEmpty()
			.Because("an asynchronous reason is only included once it is resolved");
	}

	[Test]
	public async Task Equals_BothNull_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null, null);

		await That(result).IsTrue();
	}

	[Test]
	[Arguments("is 1", "is 1", true)]
	[Arguments("is 1", "is 2", false)]
	public async Task Equals_ExpectationsOnTheSameMember_ShouldCompareTheExpectations(
		string expectation1, string expectation2, bool expectedResult)
	{
		ManualExpectationBuilder<string> sut1 = new();
		sut1.ForMember(MemberAccessor<string, int>.FromFunc(s => s.Length, "length "))
			.AddExpectations(e => e.AddConstraint((_, _, _) => new DummyConstraint(expectation1)));
		ManualExpectationBuilder<string> sut2 = new();
		sut2.ForMember(MemberAccessor<string, int>.FromFunc(s => s.Length, "length "))
			.AddExpectations(e => e.AddConstraint((_, _, _) => new DummyConstraint(expectation2)));

		await That(sut1.Equals(sut2)).IsEqualTo(expectedResult);
		await That(sut2.Equals(sut1)).IsEqualTo(expectedResult);
	}

	[Test]
	public async Task Equals_FirstNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null, sut);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_ObjectNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_SecondNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(sut, null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_WithSelf_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(sut, sut);

		await That(result).IsTrue();
	}

	[Test]
	public async Task GetHashCode_DifferentConstraint_ShouldNotBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
		sut2.ForWhich<int, int>(x => x)
			.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsNotEqualTo(sut2.GetHashCode());
	}

	[Test]
	public async Task GetHashCode_SameConstraint_ShouldBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
		sut2.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode());
	}

	[Test]
	public async Task GetHashCode_WithParameter_ShouldUseHashCodeFromParameter()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
		sut2.ForWhich<int, int>(x => x)
			.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode(sut1));
	}

	[Test]
	[Arguments("is equal to 1", "were")]
	[Arguments("are equal to 1", "were")]
	[Arguments("was equal to 1", "were")]
	[Arguments("were equal to 1", "were")]
	[Arguments("starts with \"a\"", "did")]
	[Arguments("has length 3", "did")]
	[Arguments("", "were")]
	public async Task GetResultVerb_ShouldUseDoSupportForEveryVerbButBe(string expectationText, string expected)
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint(expectationText));

		await That(sut.GetResultVerb()).IsEqualTo(expected)
			.Because("the result refers back to the expectation with a pro-verb that has to match its head verb");
	}

	[Test]
	public async Task IsMet_ShouldThrowNotSupportedException()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		async Task Act() => await sut.IsMet(
			new ExpectationNode(), null!, new TimeSystemMock(), null, CancellationToken.None);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Use IsMetBy for ManualExpectationBuilder.");
	}

	[Test]
	public async Task IsMetBy_FailingConstraint_ShouldReturnFailure()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
	}

	[Test]
	public async Task IsMetBy_SucceedingConstraint_ShouldReturnSuccess()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
	}

	[Test]
	public async Task IsMetBy_WhenConstraintFails_ShouldNotApplyReasonButResolveIt()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false, "is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder expectation = new();
		StringBuilder reasons = new();

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);
		result.AppendExpectation(expectation);
		sut.AppendReasons(reasons);

		await That(expectation.ToString()).IsEqualTo("is foo")
			.Because("the reasons follow the whole expectation that the expectations are nested in");
		await That(reasons.ToString()).IsEqualTo(", because of a");
	}

	[Test]
	public async Task IsMetBy_WhenConstraintFails_WhenCancellationIsRequested_ShouldAbandonAPendingReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false, "is foo"));
		sut.AddReason(becauseSource.Task);
		StringBuilder reasons = new();

		Task evaluation = sut.IsMetBy(1, null!, new CancellationToken(true)).AsTask();
		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = evaluation.IsCompleted;
		becauseSource.SetResult("of a");
		await evaluation;
		sut.AppendReasons(reasons);

		await That(isCompleted).IsTrue()
			.Because("the cancellation must stop waiting for a reason that does not arrive");
		await That(reasons.ToString()).IsEqualTo(", because the reason was not available in time");
	}

	[Test]
	public async Task IsMetBy_WhenConstraintSucceeds_ShouldResolveReasonOnlyWhenTheEvaluationFails()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true, "is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		aweXpect.Core.EvaluationContext.EvaluationContext context = new();
		StringBuilder reasonsWhenMet = new();
		StringBuilder reasonsWhenFailed = new();

		await sut.IsMetBy(1, context, CancellationToken.None);
		sut.AppendReasons(reasonsWhenMet);
		await context.ResolvePendingReasons(CancellationToken.None);
		sut.AppendReasons(reasonsWhenFailed);

		await That(reasonsWhenMet.ToString()).IsEmpty()
			.Because("a met expectation does not wait for the reason");
		await That(reasonsWhenFailed.ToString()).IsEqualTo(", because of a")
			.Because("an outer negation can still fail the evaluation, whose failure message then shows the reason");
	}

	[Test]
	public async Task IsMetBy_WhenReasonIsResolvedAndConstraintSucceeds_ShouldNotApplyReason()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true, "is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		await sut.PrepareExpectation(null!, CancellationToken.None);
		StringBuilder sb = new();

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);
		result.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("is foo");
	}

	[Test]
	public async Task IsMetBy_WithoutContext_ShouldPassTheCancellationToken()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		CancellationToken receivedToken = CancellationToken.None;
		EvaluationCancellation? receivedCancellation = null;
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new ContextConstraint<int>((_, context, cancellationToken) =>
		{
			receivedToken = cancellationToken;
			receivedCancellation = context.Cancellation;
			return Outcome.Success;
		}));

		await sut.IsMetBy(1, cts.Token);

		await That(receivedToken).IsEqualTo(cts.Token);
		await That(receivedCancellation?.Token).IsEqualTo(cts.Token);
		await That(receivedCancellation?.Reason).IsEqualTo(CancellationReason.Caller)
			.Because("the evaluation context must report the cancellation by the caller");
		await That(receivedCancellation?.Timeout).IsNull()
			.Because("no expectation timeout applies to a manual evaluation");
	}

	[Test]
	public async Task IsMetBy_WithoutContext_ShouldReleaseMaterializedSourcesAfterTheEvaluation()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2, 3);
		int? disposeCountDuringEvaluation = null;
		ManualExpectationBuilder<IEnumerable<int>> sut = new();
		sut.AddConstraint((_, _) => new ContextConstraint<IEnumerable<int>>((actual, context, cancellationToken) =>
		{
			_ = context.UseMaterializedEnumerable(actual).First();
			disposeCountDuringEvaluation = source.DisposeCount;
			return Outcome.Failure;
		}));

		ConstraintResult result = await sut.IsMetBy(source, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(disposeCountDuringEvaluation).IsEqualTo(0);
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the evaluation context of its own is released once the evaluation is completed");
	}

	[Test]
	public async Task IsMetBy_WithoutContext_WhenConstraintThrows_ShouldReleaseMaterializedSources()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2, 3);
		ManualExpectationBuilder<IEnumerable<int>> sut = new();
		sut.AddConstraint((_, _) => new ContextConstraint<IEnumerable<int>>((actual, context, cancellationToken) =>
		{
			_ = context.UseMaterializedEnumerable(actual).First();
			throw new InvalidOperationException("foo");
		}));

		async Task Act() => await sut.IsMetBy(source, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>().WithMessage("foo");
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the evaluation context of its own is also released when the evaluation throws");
	}

	[Test]
	public async Task IsMetBy_WithoutContext_WhenCancellationIsRequested_ShouldAbandonAPendingReasonOfAMetMember()
	{
		TaskCompletionSource<string?> becauseSource = new();
		ManualExpectationBuilder<string> sut = new();
		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(length =>
			{
				length.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"));
				length.AddReason(becauseSource.Task);
			});
		sut.And();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(_ => false, "is bar"));

		Task<ConstraintResult> evaluation = sut.IsMetBy("foo", new CancellationToken(true)).AsTask();
		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = evaluation.IsCompleted;
		becauseSource.SetResult("of a");
		ConstraintResult result = await evaluation;

		await That(isCompleted).IsTrue()
			.Because("the cancellation must stop waiting for a reason that does not arrive");
		await That(result.GetExpectationText())
			.IsEqualTo("length equal to 3, because the reason was not available in time and is bar");
	}

	[Test]
	public async Task IsMetBy_WithoutContext_WhenMetMemberHasAsyncReason_AndEvaluationFails_ShouldAppendTheReason()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(length =>
			{
				length.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"));
				length.AddReason(Task.FromResult<string?>("of a"));
			});
		sut.And();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(_ => false, "is bar"));

		ConstraintResult result = await sut.IsMetBy("foo", CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("length equal to 3, because of a and is bar")
			.Because("the reason of the met member is shown like a string reason in the failure message");
	}

	[Test]
	public async Task PrepareExpectation_ShouldNotEvaluateTheConstraints()
	{
		bool isEvaluated = false;
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ =>
		{
			isEvaluated = true;
			return true;
		}));

		await sut.PrepareExpectation(null!, CancellationToken.None);

		await That(isEvaluated).IsFalse();
	}

	[Test]
	public async Task PrepareExpectation_ShouldResolveReasonsThatMustBeAwaited()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder sb = new();

		await sut.PrepareExpectation(null!, CancellationToken.None);
		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEqualTo(", because of a");
	}

	[Test]
	public async Task PrepareExpectation_WhenCancellationIsRequested_ShouldAbandonAPendingReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason(becauseSource.Task);
		StringBuilder sb = new();

		Task preparation = sut.PrepareExpectation(null!, new CancellationToken(true));
		await Task.WhenAny(preparation, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = preparation.IsCompleted;
		becauseSource.SetResult("of a");
		await preparation;
		sut.AppendReasons(sb);

		await That(isCompleted).IsTrue()
			.Because("the cancellation must stop waiting for a reason that does not arrive");
		await That(sb.ToString()).IsEqualTo(", because the reason was not available in time");
	}

	[Test]
	public async Task Subject_ShouldBeEmpty()
	{
		ManualExpectationBuilder<int> sut = new();

		await That(sut.Subject).IsEmpty();
	}

	private sealed class ContextConstraint<T>(Func<T, IEvaluationContext, CancellationToken, Outcome> callback)
		: IAsyncContextConstraint<T>
	{
		public ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> new(new DummyConstraintResult(callback(actual, context, cancellationToken)));

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
