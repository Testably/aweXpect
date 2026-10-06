using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core.TimeSystem;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public class ExpectationResultTests
{
	[Test]
	public async Task GetResult_ShouldIncrementIndexAndHaveCorrectSubjectLine()
	{
		ExpectationResult sut = new(new MyExpectationBuilder("my-subject"));

		Expectation.Result result = await sut.GetResult(3);

		await That(result.Index).IsEqualTo(4);
		await That(result.SubjectLine).IsEqualTo(" [04] Expected that my-subject");
	}

	[Test]
	public async Task IsMet_InvalidType_ShouldReturnDefault()
	{
		MyExpectationBuilder myBuilder =
			new("my-subject", () => new DummyConstraintResult<string>(Outcome.Success, "foo", "SUCCESS"));
		ExpectationResult<int> sut = new(myBuilder);

		int result = await sut;

		await That(result).IsEqualTo(0)
			.Because("a met expectation never throws, also without a value of the expected type");
	}

	[Test]
	public async Task IsMet_InvalidType_WhenTracing_ShouldReturnDefault()
	{
		MyExpectationBuilder myBuilder =
			new("my-subject", () => new DummyConstraintResult<string>(Outcome.Success, "foo", "SUCCESS"));
		ExpectationResult<int> sut = new(myBuilder);
		TestTraceWriter traceWriter = new();
		int result;

		using (traceWriter.Register())
		{
			result = await sut;
		}

		await That(result).IsEqualTo(0);
		await That(traceWriter.Messages).IsEqualTo(["  Successfully verified that my-subject SUCCESS",]);
	}

	[Test]
	public async Task IsMet_WhenTheStoredValueIsNull_ShouldReturnNull()
	{
		MyExpectationBuilder myBuilder =
			new("my-subject", () => new DummyConstraintResult(Outcome.Success, "SUCCESS").UseValue<string?>(null));
		ExpectationResult<string?> sut = new(myBuilder);

		string? result = await sut;

		await That(result).IsNull()
			.Because("a stored null value of the expected type is a valid result, not a type mismatch");
	}

	[Test]
	public async Task WithCancellation_ShouldForwardTokenToExpectationBuilder()
	{
		MyExpectationBuilder myBuilder = new("my-subject");
		CancellationTokenSource cts = new();
		CancellationToken token = cts.Token;
		ExpectationResult<int> sut = new(myBuilder);

		_ = sut.WithCancellation(token);

		CancellationToken? receivedToken = await myBuilder.GetRegisteredCancellationToken();
		await That(receivedToken).IsEqualTo(token);
	}

	private sealed class MyExpectationBuilder(string subject, Func<ConstraintResult>? resultBuilder = null)
		: ExpectationBuilder(subject)
	{
		private readonly Func<ConstraintResult> _resultBuilder = resultBuilder ?? DefaultResultBuilder;

		private CancellationToken? _cancellationToken;

		private static ConstraintResult DefaultResultBuilder() => new DummyConstraintResult(Outcome.Success, "SUCCESS");

		public async Task<CancellationToken?> GetRegisteredCancellationToken()
		{
			await IsMet();
			return _cancellationToken;
		}

		internal override ValueTask<ConstraintResult> IsMet(Node rootNode,
			EvaluationContext.EvaluationContext context,
			ITimeSystem timeSystem,
			TimeSpan? timeout,
			CancellationToken cancellationToken)
		{
			_cancellationToken = cancellationToken;
			ConstraintResult result = _resultBuilder();
			return new ValueTask<ConstraintResult>(result);
		}
	}
}
