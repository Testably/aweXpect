using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		/// <summary>
		///     Strings and primitives that are compared by their default equality, and the options and item types that
		///     compare differently.
		/// </summary>
		public sealed class InAnyOrderDefaultEqualityTests
		{
			[Fact]
			public async Task Numbers_WhenAComparerIsUsed_ShouldCompareWithIt()
			{
				int[] subject = [11, 22, 33,];
				int[] expected = [3, 1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().Using(new LastDigitComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Numbers_WhenAToleranceIsUsed_ShouldCompareWithIt()
			{
				double[] subject = [3.05, 1.05, 2.05,];
				double[] expected = [1.0, 2.0, 3.0,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Numbers_WithAdditionalItem_ShouldFail()
			{
				int[] subject = [3, 1, 2, 4,];
				int[] expected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item 4 at index 3 that was not expected

					             Collection:
					             [3, 1, 2, 4]

					             Expected:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task Numbers_WithDifferentMultiplicities_ShouldFail()
			{
				int[] subject = [1, 1, 2,];
				int[] expected = [1, 2, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item 1 at index 1 that was not expected and
					               lacked 1 of 3 expected items: 2

					             Collection:
					             [1, 1, 2]

					             Expected:
					             [1, 2, 2]
					             """);
			}

			[Fact]
			public async Task Numbers_WithMissingItem_ShouldFail()
			{
				int[] subject = [3, 1,];
				int[] expected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 3 expected items: 2

					             Collection:
					             [3, 1]

					             Expected:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task Numbers_WithSameMultiplicities_ShouldSucceed()
			{
				long[] subject = [2, 1, 2, 3, 1,];
				long[] expected = [1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Objects_WhenEqualsThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				Exception exception = new NotSupportedException("thrown by Equals");
				ThrowingEquals[] subject = [new(exception),];
				ThrowingEquals[] expected = [new(exception),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				XunitException failure = await That(Act).Throws<XunitException>()
					.WithMessage("*thrown by Equals*").AsWildcard();
				await That(failure.InnerException).IsSameAs(exception);
			}

			[Fact]
			public async Task Objects_WithDatesOfIncompatibleKinds_ShouldFail()
			{
				DateTime[] subject = [new(2026, 10, 4, 12, 0, 0, DateTimeKind.Utc),];
				DateTime[] expected = [new(2026, 10, 4, 12, 0, 0, DateTimeKind.Local),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it*lacked the one expected item*").AsWildcard();
			}

			[Fact]
			public async Task Objects_WithDifferentMultiplicities_ShouldFail()
			{
				Item[] subject = [new(1), new(1), new(2),];
				Item[] expected = [new(1), new(2), new(2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 1
					               } at index 1 that was not expected
					             and
					               lacked 1 of 3 expected items: ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 2
					               }

					             Collection:
					             [
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 1
					               },
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 1
					               },
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 2
					               }
					             ]

					             Expected:
					             [
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 1
					               },
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 2
					               },
					               ThatEnumerable.IsEqualTo.InAnyOrderDefaultEqualityTests.Item {
					                 Value = 2
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task Objects_WithEqualItemsInADifferentOrder_ShouldSucceed()
			{
				Item[] subject = [new(2), new(1), new(2),];
				Item[] expected = [new(1), new(2), new(2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Strings_WhenAComparerIsUsed_ShouldCompareWithIt()
			{
				string[] subject = ["B", "a", "C",];
				string[] expected = ["A", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Strings_WhenBothAreEmpty_ShouldSucceed()
			{
				string[] subject = [];
				string[] expected = [];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Strings_WhenCaseIsIgnored_ShouldMatchItemsThatDifferInCase()
			{
				string[] subject = ["B", "a", "C",];
				string[] expected = ["A", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Strings_WhenExpectedIsEmpty_ShouldFail()
			{
				string[] subject = ["a",];
				string[] expected = [];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item "a" at index 0 that was not expected

					             Collection:
					             [
					               "a"
					             ]

					             Expected:
					             []
					             """);
			}

			[Fact]
			public async Task Strings_WhenItemsDifferInCase_ShouldFail()
			{
				string[] subject = ["B", "a",];
				string[] expected = ["a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item "B" at index 0 that was not expected and
					               lacked 1 of 2 expected items: "b"

					             Collection:
					             [
					               "B",
					               "a"
					             ]

					             Expected:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WhenSubjectIsEmpty_ShouldFail()
			{
				string[] subject = [];
				string[] expected = ["a",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked the one expected item

					             Collection:
					             []

					             Expected:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WhenTheSetOfTheSubjectHasAComparer_ShouldCompareWithIt()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "B", "a", };
				string[] expected = ["A", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Strings_WithAdditionalNull_ShouldFail()
			{
				string?[] subject = ["a", null, null,];
				string?[] expected = [null, "a",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item <null> at index 2 that was not expected

					             Collection:
					             [
					               "a",
					               <null>,
					               <null>
					             ]

					             Expected:
					             [
					               <null>,
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WithDifferentMultiplicities_ShouldFail()
			{
				string[] subject = ["a", "a", "b",];
				string[] expected = ["a", "b", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item "a" at index 1 that was not expected and
					               lacked 1 of 3 expected items: "b"

					             Collection:
					             [
					               "a",
					               "a",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WithMissingNull_ShouldFail()
			{
				string?[] subject = ["a", "b",];
				string?[] expected = [null, "a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 3 expected items: <null>

					             Collection:
					             [
					               "a",
					               "b"
					             ]

					             Expected:
					             [
					               <null>,
					               "a",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WithMoreThan20AdditionalItems_ShouldFail()
			{
				string[] expected = ["a", "b", "c",];
				string[] subject = [..expected, ..Enumerable.Range(1, 30).Select(i => $"x{i}"),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it had more than 20 deviations:
					               contained item "x1" at index 3 that was not expected,
					               contained item "x2" at index 4 that was not expected,
					               contained item "x3" at index 5 that was not expected,
					               contained item "x4" at index 6 that was not expected,
					               contained item "x5" at index 7 that was not expected,
					               contained item "x6" at index 8 that was not expected,
					               contained item "x7" at index 9 that was not expected,
					               contained item "x8" at index 10 that was not expected,
					               contained item "x9" at index 11 that was not expected,
					               contained item "x10" at index 12 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "x1",
					               "x2",
					               "x3",
					               "x4",
					               "x5",
					               "x6",
					               "x7",
					               (… and 23 more)
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task Strings_WithSameMultiplicitiesAndNulls_ShouldSucceed()
			{
				string?[] subject = [null, "b", "a", null, "a",];
				string?[] expected = ["a", "a", "b", null, null,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			private sealed class Item(int value)
			{
				public int Value { get; } = value;

				public override bool Equals(object? obj) => obj is Item other && other.Value == Value;

				public override int GetHashCode() => Value;
			}

			private sealed class LastDigitComparer : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % 10 == y % 10;

				public int GetHashCode(int obj) => obj % 10;
			}

			private sealed class ThrowingEquals(Exception exception)
			{
				public override bool Equals(object? obj) => throw exception;

				public override int GetHashCode() => 0;
			}
		}
	}
}
