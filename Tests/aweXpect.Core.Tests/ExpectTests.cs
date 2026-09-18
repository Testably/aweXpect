using System.Collections.Generic;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

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
	private sealed class MyExpectation(Expectation.Result result, params ResultContext[] contexts) : Expectation
	{
		internal override Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes)
			=> Task.FromResult(result);

		internal override IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes)
			=> contexts;
	}
}
