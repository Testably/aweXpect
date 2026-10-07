using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Internal;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
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
	public async Task IsMet_WhenTheExpectationTextThrows_WhenTracing_ShouldMeetTheExpectation()
	{
		ExpectationResult sut = new(new MyExpectationBuilder("my-subject", () => new ThrowingTextConstraintResult()));
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await sut;
			}
		}

		await That(Act).DoesNotThrow();
		await That(traceWriter.Messages).IsEmpty();
	}

	[Test]
	public async Task IsMet_WhenTheExpectationTextThrows_WhenTracing_ShouldReturnTheValue()
	{
		ExpectationResult<int> sut =
			new(new MyExpectationBuilder("my-subject", () => new ThrowingTextConstraintResult()));
		TestTraceWriter traceWriter = new();
		int result = 0;

		async Task Act()
		{
			using (traceWriter.Register())
			{
				result = await sut;
			}
		}

		await That(Act).DoesNotThrow();
		await That(result).IsEqualTo(4);
		await That(traceWriter.Messages).IsEmpty();
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

	[Test]
	public async Task WithCancellation_TogetherWithTimeout_ShouldNotThrow()
	{
		using CancellationTokenSource cts = new();
		ExpectationResult<int> sut = new(new MyExpectationBuilder("my-subject"));

		void Act() => sut.WithTimeout(TimeSpan.FromSeconds(1)).WithCancellation(cts.Token)
			.WithTimeout(TimeSpan.FromSeconds(2));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithCancellation_WhenSpecifiedTwice_ShouldKeepTheFirstToken()
	{
		MyExpectationBuilder myBuilder = new("my-subject");
		using CancellationTokenSource cts = new();
		CancellationToken token = cts.Token;
		ExpectationResult<int> sut = new ExpectationResult<int>(myBuilder).WithCancellation(token);

		void Act() => sut.WithCancellation(CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>();
		CancellationToken? receivedToken = await myBuilder.GetRegisteredCancellationToken();
		await That(receivedToken).IsEqualTo(token);
	}

	[Test]
	public async Task WithCancellation_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		using CancellationTokenSource cts = new();
		ExpectationResult<int> sut = new ExpectationResult<int>(new MyExpectationBuilder("my-subject"))
			.WithCancellation(cts.Token);

		void Act() => sut.WithCancellation(cts.Token);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("WithCancellation cannot be specified more than once.")
			.Because("the second token would silently replace the first one");
	}

	[Test]
	public async Task WithCancellation_WhenSpecifiedTwice_WithoutValue_ShouldThrowInvalidOperationException()
	{
		using CancellationTokenSource cts = new();
		ExpectationResult sut = new ExpectationResult(new MyExpectationBuilder("my-subject"))
			.WithCancellation(cts.Token);

		void Act() => sut.WithCancellation(cts.Token);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("WithCancellation cannot be specified more than once.");
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

	private sealed class ThrowingTextConstraintResult : ConstraintResult
	{
		public ThrowingTextConstraintResult() : base(FurtherProcessingStrategy.Continue)
		{
			Outcome = Outcome.Success;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotSupportedException("the expectation text is broken");

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// A met expectation has no result text.
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (4 is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}
}
