namespace aweXpect.Tests;

public class ExpectTests
{
	public class ThatAllTests
	{
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async Task ShouldEvaluateAndDisplayAllExpectations(bool subjectA, bool subjectB)
		{
			async Task Act()
				=> await ThatAll(
					That(subjectA).IsTrue(),
					That(subjectB).IsTrue());

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected all of the following to succeed:
				               [01] Expected that subjectA is True
				               [02] Expected that subjectB is True
				              but
				               [{(subjectB ? "01" : "02")}] it was False
				              """);
		}

		[Fact]
		public async Task ShouldIncludeProperIndentationForMultilineResults()
		{
			string subjectA = "subject A";
			string subjectB = "subject C";
			string subjectC = "subject B";

			async Task Act()
				=> await ThatAll(
					That(subjectA).IsEqualTo("subject A"),
					That(subjectB).IsEqualTo("subject B"),
					That(subjectC).IsEqualTo("subject C"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subjectA is equal to "subject A"
				              [02] Expected that subjectB is equal to "subject B"
				              [03] Expected that subjectC is equal to "subject C"
				             but
				              [02] it was "subject C" which differs at index 8:
				                              ↓ (actual)
				                     "subject C"
				                     "subject B"
				                              ↑ (expected)
				              [03] it was "subject B" which differs at index 8:
				                              ↓ (actual)
				                     "subject B"
				                     "subject C"
				                              ↑ (expected)
				             """);
		}

		[Fact]
		public async Task WhenAllConditionIsMet_ShouldSucceed()
		{
			bool subjectA = true;
			bool subjectB = true;

			async Task Act()
				=> await ThatAll(
					That(subjectA).IsTrue(),
					That(subjectB).IsTrue());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenBothConditionsAreNotMet_ShouldFail()
		{
			bool subjectA = false;
			bool subjectB = false;

			async Task Act()
				=> await ThatAll(
					That(subjectA).IsTrue(),
					That(subjectB).IsTrue());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subjectA is True
				              [02] Expected that subjectB is True
				             but
				              [01] it was False
				              [02] it was False
				             """);
		}

		[Fact]
		public async Task WhenNested_ShouldIncludeProperIndentationForMultilineResults()
		{
			string subjectA = "subject A";
			string subjectB = "some unexpected value";
			string subjectC = "subject B";

			async Task Act()
				=> await ThatAll(
					That(true).IsTrue(),
					ThatAll(
						That(subjectA).IsEqualTo("subject A"),
						That(subjectB).IsEqualTo("subject B"),
						That(subjectC).IsEqualTo("subject C")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that true is True
				               Expected all of the following to succeed:
				                [02] Expected that subjectA is equal to "subject A"
				                [03] Expected that subjectB is equal to "subject B"
				                [04] Expected that subjectC is equal to "subject C"
				             but
				                [03] it was "some unexpected value" which differs at index 1:
				                         ↓ (actual)
				                       "some unexpected value"
				                       "subject B"
				                         ↑ (expected)
				                [04] it was "subject B" which differs at index 8:
				                                ↓ (actual)
				                       "subject B"
				                       "subject C"
				                                ↑ (expected)
				             """);
		}

		[Fact]
		public async Task WhenNested_ShouldUseIncrementingNumber()
		{
			bool subjectA = false;
			bool subjectB = false;
			bool subjectC = true;
			bool subjectD = false;

			async Task Act()
				=> await ThatAll(
					ThatAny(
						That(subjectA).IsTrue(),
						That(subjectB).IsTrue()),
					That(subjectC).IsTrue(),
					That(subjectD).IsTrue());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				               Expected any of the following to succeed:
				                [01] Expected that subjectA is True
				                [02] Expected that subjectB is True
				              [03] Expected that subjectC is True
				              [04] Expected that subjectD is True
				             but
				                [01] it was False
				                [02] it was False
				              [04] it was False
				             """);
		}

		[Fact]
		public async Task WhenNoExpectationWasProvided_ShouldThrowArgumentException()
		{
			async Task Act()
				=> await ThatAll();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must provide at least one expectation*").AsWildcard().And
				.WithParamName("expectations");
		}
	}

	public class ThatAnyTests
	{
		[Fact]
		public async Task ShouldEvaluateAndDisplayAllExpectations()
		{
			bool subjectA = false;
			bool subjectB = false;

			async Task Act()
				=> await ThatAny(
					That(subjectA).IsTrue(),
					That(subjectB).IsTrue());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that subjectA is True
				              [02] Expected that subjectB is True
				             but
				              [01] it was False
				              [02] it was False
				             """);
		}

		[Fact]
		public async Task ShouldIncludeProperIndentationForMultilineResults()
		{
			string subjectA = "subject X";
			string subjectB = "subject Y";
			string subjectC = "subject Z";

			async Task Act()
				=> await ThatAny(
					That(subjectA).IsEqualTo("subject A"),
					That(subjectB).IsEqualTo("subject B"),
					That(subjectC).IsEqualTo("subject C"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that subjectA is equal to "subject A"
				              [02] Expected that subjectB is equal to "subject B"
				              [03] Expected that subjectC is equal to "subject C"
				             but
				              [01] it was "subject X" which differs at index 8:
				                              ↓ (actual)
				                     "subject X"
				                     "subject A"
				                              ↑ (expected)
				              [02] it was "subject Y" which differs at index 8:
				                              ↓ (actual)
				                     "subject Y"
				                     "subject B"
				                              ↑ (expected)
				              [03] it was "subject Z" which differs at index 8:
				                              ↓ (actual)
				                     "subject Z"
				                     "subject C"
				                              ↑ (expected)
				             """);
		}

		[Theory]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async Task WhenAnyConditionIsMet_ShouldSucceed(bool subjectA, bool subjectB)
		{
			async Task Act()
				=> await ThatAny(
					That(subjectA).IsTrue(),
					That(subjectB).IsTrue());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenNested_ShouldIncludeProperIndentationForMultilineResults()
		{
			string subjectA = "subject X";
			string subjectB = "some unexpected value";
			string subjectC = "subject Z";

			async Task Act()
				=> await ThatAny(
					That(false).IsTrue(),
					ThatAny(
						That(subjectA).IsEqualTo("subject A"),
						That(subjectB).IsEqualTo("subject B"),
						That(subjectC).IsEqualTo("subject C")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that false is True
				               Expected any of the following to succeed:
				                [02] Expected that subjectA is equal to "subject A"
				                [03] Expected that subjectB is equal to "subject B"
				                [04] Expected that subjectC is equal to "subject C"
				             but
				              [01] it was False
				                [02] it was "subject X" which differs at index 8:
				                                ↓ (actual)
				                       "subject X"
				                       "subject A"
				                                ↑ (expected)
				                [03] it was "some unexpected value" which differs at index 1:
				                         ↓ (actual)
				                       "some unexpected value"
				                       "subject B"
				                         ↑ (expected)
				                [04] it was "subject Z" which differs at index 8:
				                                ↓ (actual)
				                       "subject Z"
				                       "subject C"
				                                ↑ (expected)
				             """);
		}

		[Fact]
		public async Task WhenNested_ShouldIndentMultiLineResults()
		{
			async Task Act()
				=> await ThatAll(That(true).IsMyConstraint(
					"""
					expectation
					over
					multiple
					lines
					""",
					_ => false,
					"""
					result
					also
					on multiple
					lines
					"""
				));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that true expectation
				                   over
				                   multiple
				                   lines
				             but
				              [01] result
				                   also
				                   on multiple
				                   lines
				             """);
		}

		[Fact]
		public async Task WhenNested_ShouldUseIncrementingNumber()
		{
			bool subjectA = false;
			bool subjectB = true;
			bool subjectC = false;
			bool subjectD = false;

			async Task Act()
				=> await ThatAny(
					ThatAll(
						That(subjectA).IsTrue(),
						That(subjectB).IsTrue()),
					That(subjectC).IsTrue(),
					That(subjectD).IsTrue());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				               Expected all of the following to succeed:
				                [01] Expected that subjectA is True
				                [02] Expected that subjectB is True
				              [03] Expected that subjectC is True
				              [04] Expected that subjectD is True
				             but
				                [01] it was False
				              [03] it was False
				              [04] it was False
				             """);
		}

		[Fact]
		public async Task WhenNoExpectationWasProvided_ShouldThrowArgumentException()
		{
			async Task Act()
				=> await ThatAny();

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must provide at least one expectation*").AsWildcard().And
				.WithParamName("expectations");
		}
	}
}
