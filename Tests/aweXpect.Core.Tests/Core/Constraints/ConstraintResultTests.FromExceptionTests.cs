using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class FromExceptionTests
	{
		[Fact]
		public async Task AppendExpectation_ShouldUseInnerExpectation()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "it");
			StringBuilder sb = new();

			sut.AppendExpectation(sb);

			await That(sb.ToString()).IsEqualTo("foo");
		}

		[Fact]
		public async Task AppendResult_ExceededTimeout_ShouldNameTheSubject()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "the subject",
				TimeSpan.FromSeconds(2));
			StringBuilder sb = new();

			sut.AppendResult(sb);

			await That(sb.ToString()).IsEqualTo("the subject did not finish within 0:02");
		}

		[Fact]
		public async Task AppendResult_Exception_ShouldAppendExpectedValue()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "it");
			StringBuilder sb = new();

			sut.AppendResult(sb);

			await That(sb.ToString()).IsEqualTo($"it did throw an Exception:{Environment.NewLine}  bar");
		}

		[Fact]
		public async Task AppendResult_Exception_ShouldNameTheSubject()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "the subject");
			StringBuilder sb = new();

			sut.AppendResult(sb);

			await That(sb.ToString()).IsEqualTo($"the subject did throw an Exception:{Environment.NewLine}  bar");
		}

		[Fact]
		public async Task AppendResult_SpecificException_ShouldAppendExpectedValue()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			ArgumentException exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "it");
			StringBuilder sb = new();

			sut.AppendResult(sb);

			await That(sb.ToString()).IsEqualTo($"it did throw an ArgumentException:{Environment.NewLine}  bar");
		}

		[Fact]
		public async Task FailureCause_ShouldBeTheException()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "it");

			await That(sut.FailureCause).IsSameAs(exception);
		}

		[Theory]
		[InlineData(Outcome.Failure, Outcome.Success)]
		[InlineData(Outcome.Success, Outcome.Failure)]
		[InlineData(Outcome.Undecided, Outcome.Undecided)]
		public async Task Negate_ShouldNegateInnerOutcome(Outcome innerOutcome, Outcome expectedAfterNegation)
		{
			DummyConstraintResult inner = new(innerOutcome, "foo");
			Exception exception = new("bar");
			MyFromExceptionConstraintResult sut = new(inner, exception);

			sut.Negate();

			await That(sut.Outcome).IsEqualTo(Outcome.Failure);
			await That(inner.Outcome).IsEqualTo(expectedAfterNegation);
		}

		[Fact]
		public async Task Outcome_ShouldBeFailure()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			ConstraintResult sut = new ConstraintResult.FromException(inner, exception, "it");

			await That(sut.Outcome).IsEqualTo(Outcome.Failure);
		}

		[Fact]
		public async Task SetOutcome_ShouldBeForwardedToInner()
		{
			DummyConstraintResult inner = new(Outcome.Success, "foo");
			Exception exception = new("bar");
			MyFromExceptionConstraintResult sut = new(inner, exception);
			sut.SetOutcome(Outcome.Undecided);

			await That(sut.Outcome).IsEqualTo(Outcome.Failure);
			await That(inner.Outcome).IsEqualTo(Outcome.Undecided);
		}

		private class MyFromExceptionConstraintResult(
			ConstraintResult inner,
			Exception exception)
			: ConstraintResult.FromException(inner, exception, "it")
		{
			public void SetOutcome(Outcome outcome) => Outcome = outcome;
		}
	}
}
