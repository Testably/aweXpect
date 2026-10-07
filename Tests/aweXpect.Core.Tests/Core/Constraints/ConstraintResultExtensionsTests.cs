using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public sealed class ConstraintResultExtensionsTests
{
	public sealed class FailTests
	{
		[Test]
		public async Task Failure_TryGetStoredValue_WithNullValue_ShouldReturnTrue()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Success, "value", "foo");
			sut = sut.Fail<string?>("bar", null);

			bool result = sut.TryGetStoredValue(out string? value);

			await That(result).IsTrue()
				.Because("the result stores a value of the requested type, even though it is null");
			await That(value).IsNull();
		}

		[Test]
		public async Task Failure_TryGetValue_WhenTypeDoesNotMatch_ShouldReturnFalse()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Success, "value", "foo");
			sut = sut.Fail("bar", 1);

			bool result = sut.TryGetValue(out string? value);

			await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task Failure_TryGetValue_WhenTypeMatches_ShouldReturnTrue()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Success, "value", "foo");
			sut = sut.Fail("bar", 1);

			bool result = sut.TryGetValue(out int value);

			await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
			await That(result).IsTrue();
			await That(value).IsEqualTo(1);
		}

		[Test]
		public async Task Failure_TryGetValue_WithNullValue_ShouldReturnFalse()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Success, "value", "foo");
			sut = sut.Fail<string?>("bar", null);

			bool result = sut.TryGetValue(out string? value);

			await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task FailureCause_ShouldBeForwardedFromInner()
		{
			Exception exception = new("foo");
			ConstraintResult inner = new ConstraintResult.FromException(
				new DummyConstraintResult(Outcome.Failure, "foo"), exception, "it");
			ConstraintResult sut = inner.Fail("bar", 1);

			await That(sut.FailureCause).IsSameAs(exception);
		}

		[Test]
		public async Task FailureCause_WhenInnerHasNoFailureCause_ShouldBeNull()
		{
			ConstraintResult inner = new DummyConstraintResult(Outcome.Failure, "foo");
			ConstraintResult sut = inner.Fail("bar", 1);

			await That(sut.FailureCause).IsNull();
		}

		[Test]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task Negate_ShouldForwardToInnerResult(Outcome innerOutcome, Outcome expectedAfterNegation)
		{
			ConstraintResult inner = new DummyConstraintResult<string>(innerOutcome, "value", "foo");
			ConstraintResult sut = inner.Fail("bar", 1);

			sut.Negate();

			await That(inner.Outcome).IsEqualTo(expectedAfterNegation);
			await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
		}

		[Test]
		[Arguments(Outcome.Failure, false)]
		[Arguments(Outcome.Failure, true)]
		[Arguments(Outcome.Success, false)]
		[Arguments(Outcome.Success, true)]
		[Arguments(Outcome.Undecided, false)]
		[Arguments(Outcome.Undecided, true)]
		public async Task Negate_ShouldReturnTheFailure(Outcome innerOutcome, bool invert)
		{
			ConstraintResult inner = new DummyConstraintResult<string>(innerOutcome, "value", "foo", "baz");
			ConstraintResult sut = inner.Fail("bar", 1);

			ConstraintResult negated = invert ? sut.Invert() : sut.Negate();

			await That(negated).IsSameAs(sut)
				.Because("callers continue with the returned result, which has to keep the forced failure");
			await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays);
			await That(negated.GetResultText()).IsEqualTo("bar");
		}
	}

	public sealed class UseValueTests
	{
		[Test]
		public async Task Failure_TryGetStoredValue_WithNullValue_ShouldReturnTrue()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Failure, "value", "foo", "bar");
			sut = sut.UseValue<string?>(null);

			bool result = sut.TryGetStoredValue(out string? value);

			await That(result).IsTrue()
				.Because("the result stores a value of the requested type, even though it is null");
			await That(value).IsNull();
		}

		[Test]
		public async Task Failure_TryGetValue_WhenTypeDoesNotMatch_ShouldReturnFalse()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Failure, "value", "foo", "bar");
			sut = sut.UseValue(1);

			bool result = sut.TryGetValue(out string? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task Failure_TryGetValue_WhenTypeMatches_ShouldReturnTrue()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Failure, "value", "foo", "bar");
			sut = sut.UseValue(1);

			bool result = sut.TryGetValue(out int value);

			await That(result).IsTrue();
			await That(value).IsEqualTo(1);
		}

		[Test]
		public async Task Failure_TryGetValue_WithNullValue_ShouldReturnFalse()
		{
			ConstraintResult sut = new DummyConstraintResult<string>(Outcome.Failure, "value", "foo", "bar");
			sut = sut.UseValue<string?>(null);

			bool result = sut.TryGetValue(out string? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task FailureCause_ShouldBeForwardedFromInner()
		{
			Exception exception = new("foo");
			ConstraintResult inner = new ConstraintResult.FromException(
				new DummyConstraintResult(Outcome.Failure, "foo"), exception, "it");
			ConstraintResult sut = inner.UseValue(1);

			await That(sut.FailureCause).IsSameAs(exception);
		}

		[Test]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task Negate_ShouldForwardToInnerResult(Outcome innerOutcome, Outcome expectedAfterNegation)
		{
			ConstraintResult inner = new DummyConstraintResult<string>(innerOutcome, "value", "foo");
			ConstraintResult sut = inner.UseValue("bar");

			sut.Negate();

			await That(inner.Outcome).IsEqualTo(expectedAfterNegation);
			await That(sut.Outcome).IsEqualTo(expectedAfterNegation);
		}
	}

	public sealed class AppendExpectationTextTests
	{
		[Test]
		public async Task FailureCause_ShouldBeForwardedFromInner()
		{
			Exception exception = new("foo");
			ConstraintResult inner = new ConstraintResult.FromException(
				new DummyConstraintResult(Outcome.Failure, "foo"), exception, "it");
			ConstraintResult sut = inner.AppendExpectationText(s => s.Append("bar"));

			await That(sut.FailureCause).IsSameAs(exception);
		}

		[Test]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task Negate_ShouldForwardToInnerResult(Outcome innerOutcome, Outcome expectedAfterNegation)
		{
			ConstraintResult inner = new DummyConstraintResult<string>(innerOutcome, "value", "foo");
			ConstraintResult sut = inner.AppendExpectationText(s => s.Append("bar"));

			sut.Negate();

			await That(inner.Outcome).IsEqualTo(expectedAfterNegation);
			await That(sut.Outcome).IsEqualTo(expectedAfterNegation);
		}

		[Test]
		public async Task ShouldAppendAfterExpectationText()
		{
			ConstraintResult sut = new DummyConstraintResult(Outcome.Success, "foo");

			ConstraintResult result = sut.AppendExpectationText(s => s.Append("\nsuffix-foo"));

			await That(result.Outcome).IsEqualTo(Outcome.Success);
			await That(result.GetExpectationText()).IsEqualTo("foo\nsuffix-foo");
		}

		[Test]
		public async Task ShouldKeepResultTextUnchanged()
		{
			ConstraintResult sut = new DummyConstraintResult(Outcome.Failure, "foo", "bar");

			ConstraintResult result = sut.AppendExpectationText(s => s.Append("\nsuffix-foo"));

			await That(result.Outcome).IsEqualTo(Outcome.Failure);
			await That(result.GetExpectationText()).IsEqualTo("foo\nsuffix-foo");
			await That(result.GetResultText()).IsEqualTo("bar");
		}
	}

	public sealed class AsExpectationOnlyTests
	{
		[Test]
		public async Task ShouldOnlyKeepTheExpectation()
		{
			ConstraintResult sut = new DummyConstraintResult(Outcome.Failure, "foo", "bar");

			ConstraintResult result = sut.AsExpectationOnly();

			await That(result.GetExpectationText()).IsEqualTo("foo");
			await That(result.GetResultText()).IsEqualTo("")
				.Because("the operand was not evaluated");
		}
	}

	public sealed class PrependExpectationTextTests
	{
		[Test]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task Negate_ShouldForwardToInnerResult(Outcome innerOutcome, Outcome expectedAfterNegation)
		{
			ConstraintResult inner = new DummyConstraintResult<string>(innerOutcome, "value", "foo");
			ConstraintResult sut = inner.PrependExpectationText(s => s.Append("bar"));

			sut.Negate();

			await That(inner.Outcome).IsEqualTo(expectedAfterNegation);
			await That(sut.Outcome).IsEqualTo(expectedAfterNegation);
		}

		[Test]
		public async Task ShouldAppendAfterExpectationText()
		{
			ConstraintResult sut = new DummyConstraintResult(Outcome.Success, "foo");

			ConstraintResult result = sut.PrependExpectationText(s => s.Append("prefix-foo\n"));

			await That(result.Outcome).IsEqualTo(Outcome.Success);
			await That(result.GetExpectationText()).IsEqualTo("prefix-foo\nfoo");
		}

		[Test]
		public async Task ShouldKeepResultTextUnchanged()
		{
			ConstraintResult sut = new DummyConstraintResult(Outcome.Failure, "foo", "bar");

			ConstraintResult result = sut.PrependExpectationText(s => s.Append("prefix-foo\n"));

			await That(result.Outcome).IsEqualTo(Outcome.Failure);
			await That(result.GetExpectationText()).IsEqualTo("prefix-foo\nfoo");
			await That(result.GetResultText()).IsEqualTo("bar");
		}
	}
}
