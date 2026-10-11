namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		/// <summary>
		///     Strings and primitives that are compared by their default equality while duplicates are ignored, and the
		///     options that compare differently.
		/// </summary>
		public sealed class InAnyOrderIgnoringDuplicatesDefaultEqualityTests
		{
			[Test]
			public async Task Numbers_WithMissingItems_ShouldListThemInTheExpectedOrder()
			{
				int[] subject = [3, 1, 3,];
				int[] expected = [5, 1, 4, 2, 3,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 3 of 5 expected items:
					               5,
					               4,
					               2

					             Collection:
					             [3, 1, 3]

					             Expected:
					             [5, 1, 4, 2, 3]
					             """);
			}

			[Test]
			public async Task Strings_WhenCaseIsIgnored_ShouldMatchItemsThatDifferInCase()
			{
				string[] subject = ["B", "a", "b", "A",];
				string[] expected = ["a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Strings_WithAdditionalItems_ShouldReportEachDistinctItemOnce()
			{
				string[] subject = ["a", "x", "a", "y", "x",];
				string[] expected = ["a",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it
					               contained item "x" at index 1 that was not expected and
					               contained item "y" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "x",
					               "a",
					               "y",
					               "x"
					             ]

					             Expected:
					             [
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task Strings_WithDuplicatesAndNulls_ShouldSucceed()
			{
				string?[] subject = [null, "b", "a", null, "a",];
				string?[] expected = ["a", "b", null, "a",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Strings_WithMissingNull_ShouldFail()
			{
				string?[] subject = ["a", "b", "a",];
				string?[] expected = [null, "a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 1 of 3 expected items: <null>

					             Collection:
					             [
					               "a",
					               "b",
					               "a"
					             ]

					             Expected:
					             [
					               <null>,
					               "a",
					               "b"
					             ]
					             """);
			}
		}
	}
}
