namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class ArrayArgumentTests
	{
		[Fact]
		public async Task Contains_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [3, 2,];

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but it contained item 3 at index 2 in wrong order

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [3, 2]
				             """)
				.Because("an array converts implicitly to a span in C# 14, but must still bind to the collection overload");
		}

		[Fact]
		public async Task Contains_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] expected = ["B", "C",];

			async Task Act()
				=> await That(subject).Contains(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotContain_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] unexpected = [2, 3,];

			async Task Act()
				=> await That(subject).DoesNotContain(unexpected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain collection unexpected in order and contiguous,
				             but it did

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [2, 3]
				             """);
		}

		[Fact]
		public async Task DoesNotContain_WithStringArrays_IgnoringCase_ShouldFail()
		{
			string[] subject = ["a", "b", "c",];
			string[] unexpected = ["B", "C",];

			async Task Act()
				=> await That(subject).DoesNotContain(unexpected).IgnoringCase();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain collection unexpected ignoring case in order and contiguous,
				             but it did

				             Collection:
				             [
				               "a",
				               "b",
				               "c"
				             ]

				             Expected:
				             [
				               "B",
				               "C"
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotEndWith_WithIntArrays_ShouldSucceed()
		{
			int[] subject = [1, 2, 3,];
			int[] unexpected = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotEndWith(unexpected);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotEndWith_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] unexpected = ["A", "B",];

			async Task Act()
				=> await That(subject).DoesNotEndWith(unexpected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotStartWith_WithIntArrays_ShouldSucceed()
		{
			int[] subject = [1, 2, 3,];
			int[] unexpected = [2, 3,];

			async Task Act()
				=> await That(subject).DoesNotStartWith(unexpected);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotStartWith_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] unexpected = ["B", "C",];

			async Task Act()
				=> await That(subject).DoesNotStartWith(unexpected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task EndsWith_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [1, 2,];

			async Task Act()
				=> await That(subject).EndsWith(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             ends with [1, 2],
				             but it contained item 3 at index 2 instead of 2

				             Collection:
				             [1, 2, 3]
				             """);
		}

		[Fact]
		public async Task EndsWith_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] expected = ["B", "C",];

			async Task Act()
				=> await That(subject).EndsWith(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task HasCount_WithStringArray_ShouldFail()
		{
			string[] subject = ["a", "b", "c",];

			async Task Act()
				=> await That(subject).HasCount(2);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 2 items,
				             but it had 3 items

				             Collection:
				             [
				               "a",
				               "b",
				               "c"
				             ]
				             """);
		}

		[Fact]
		public async Task IsContainedIn_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [1, 2,];

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection expected in order and contiguous,
				             but it contained item 3 at index 2 that was not expected

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task IsContainedIn_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b",];
			string[] expected = ["A", "B", "C",];

			async Task Act()
				=> await That(subject).IsContainedIn(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsEqualTo_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [1, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it contained item 3 at index 2 that was not expected

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task IsEqualTo_WithNullableDoubleArray_Within_ShouldSucceed()
		{
			double?[] subject = [1.1, 2.1,];
			double[] expected = [1.0, 2.0,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).Within(0.2);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsEqualTo_WithNullableStringArrays_IgnoringCase_ShouldSucceed()
		{
			string?[] subject = ["a", null, "c",];
			string?[] expected = ["A", null, "C",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsEqualTo_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] expected = ["A", "B", "C",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsEquivalentTo_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [1, 2,];

			async Task Act()
				=> await That(subject).IsEquivalentTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equivalent to expected,
				             but it was not:
				               Element [2] had superfluous 3

				             Equivalency options:
				              - include public fields and properties
				             """);
		}

		[Fact]
		public async Task IsNotContainedIn_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2,];
			int[] unexpected = [1, 2, 3,];

			async Task Act()
				=> await That(subject).IsNotContainedIn(unexpected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not contained in collection unexpected in order and contiguous,
				             but it was

				             Collection:
				             [1, 2]

				             Expected:
				             [1, 2, 3]
				             """);
		}

		[Fact]
		public async Task IsNotEqualTo_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] unexpected = [1, 2, 3,];

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection unexpected in order,
				             but it was

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [1, 2, 3]
				             """);
		}

		[Fact]
		public async Task IsNotEqualTo_WithStringArrays_IgnoringCase_ShouldFail()
		{
			string[] subject = ["a", "b",];
			string[] unexpected = ["A", "B",];

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).IgnoringCase();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection unexpected ignoring case in order,
				             but it was

				             Collection:
				             [
				               "a",
				               "b"
				             ]

				             Expected:
				             [
				               "A",
				               "B"
				             ]
				             """);
		}

		[Fact]
		public async Task StartsWith_WithIntArrays_ShouldFail()
		{
			int[] subject = [1, 2, 3,];
			int[] expected = [2, 3,];

			async Task Act()
				=> await That(subject).StartsWith(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             starts with [2, 3],
				             but it contained item 1 at index 0 instead of 2

				             Collection:
				             [1, 2, 3]
				             """);
		}

		[Fact]
		public async Task StartsWith_WithStringArrays_IgnoringCase_ShouldSucceed()
		{
			string[] subject = ["a", "b", "c",];
			string[] expected = ["A", "B",];

			async Task Act()
				=> await That(subject).StartsWith(expected).IgnoringCase();

			await That(Act).DoesNotThrow();
		}
	}
}
