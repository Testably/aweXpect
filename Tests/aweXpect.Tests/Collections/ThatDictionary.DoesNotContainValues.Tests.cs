using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContainValues
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllValuesDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(0, 2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllValuesDoNotExist_WithNull_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAtLeastOneValueExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(42, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [42, 2],
					             but it contained [
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenAtLeastOneValueExists_WithNull_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [null, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [2, <null>],
					             but it contained [
					               <null>
					             ]

					             Dictionary:
					             {[1] = <null>, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenAtLeastOneValueIsANumberOfADifferentType_ShouldFail()
			{
				Dictionary<string, object> subject = new() { ["a"] = 1, };

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, 1L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [2, 1],
					             but it contained [
					               1
					             ]

					             Dictionary:
					             {
					               ["a"] = 1
					             }
					             """);
			}

			[Fact]
			public async Task WhenOneStringValueDiffersOnlyInCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).DoesNotContainValues("BAZ", "BAR").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values ["BAZ", "BAR"] ignoring case,
					             but it contained [
					               "BAR"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar"
					             }
					             """);
			}

			[Fact]
			public async Task WhenOneValueOfAnEnumerableExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> unexpected = new List<int> { 42, 2, };

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values unexpected,
					             but it contained [
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotContainValues("foo", "bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenValuesOfAnEnumerableDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> unexpected = new List<int> { 0, 2, };

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).DoesNotContainValues(new List<int> { 2, });

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).DoesNotContainValues(2);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
