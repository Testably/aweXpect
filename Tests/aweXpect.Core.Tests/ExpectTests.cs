using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Threading.Tasks.Sources;
#endif
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
		Expectation.Result result2 = new(1, "foo2", new DummyConstraintResult(Outcome.Success, "expectation2"));

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

			             [02] context-title2:
			             contest-content2
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

#if NET8_0_OR_GREATER
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
#endif

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

#if NET8_0_OR_GREATER
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
#endif

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

#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldSupportValueTaskAsSubject()
	{
		ValueTask<int> sut = ValueTask.FromResult(42);

		async Task Act()
			=> await That(sut).IsGreaterThan(41);

		await That(Act).DoesNotThrow();
	}
#endif

#if NET8_0_OR_GREATER
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
#endif

	private sealed class MyExpectation(Expectation.Result result, params ResultContext[] contexts) : Expectation
	{
		internal override Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes)
			=> Task.FromResult(result);

		internal override IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes)
			=> contexts;
	}
}
