using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class ContainsValue
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNullValueDoesNotExist_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value <null>,
					             but it did not contain <null>

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenNullValueExists_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, null, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringValueDiffersInMoreThanCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).ContainsValue("BAZ").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value "BAZ" ignoring case,
					             but it did not contain "BAZ"

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar"
					             }
					             """);
			}

			[Fact]
			public async Task WhenStringValueDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).ContainsValue("BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.ContainsValue("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValueExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(42);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueIsANumberOfADifferentType_ShouldSucceed()
			{
				Dictionary<string, object> subject = new() { ["a"] = 1, };

				async Task Act()
					=> await That(subject).ContainsValue(1L);

				await That(Act).DoesNotThrow()
					.Because("values are compared with the same object equality as the items of a collection");
			}

			[Fact]
			public async Task WhenValueIsEquivalent_WithEquivalent_ShouldSucceed()
			{
				Dictionary<int, MyClass> subject = new() { [1] = new MyClass(1), [2] = new MyClass(2), };

				async Task Act()
					=> await That(subject).ContainsValue(new MyClass(2)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueIsMissing_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value 2,
					             but it did not contain 2

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenValueIsMissing_UsingAComparer_ShouldIncludeTheComparerAndFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(2).Using(new NeverEqualComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value 2 using ThatDictionary.ContainsValue.Tests.NeverEqualComparer,
					             but it did not contain 2

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenValueMatchesTheComparer_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(2).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}

			private sealed class NeverEqualComparer : IEqualityComparer<object>
			{
				public new bool Equals(object? x, object? y) => false;

				public int GetHashCode(object obj) => 0;
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).ContainsValue(1);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForADictionaryOfStrings_ShouldKeepTheSubjectType()
			{
				Dictionary<string, string?> subject = new() { { "a", "foo" }, };

				Dictionary<string, string?> result = await That(subject).ContainsValue("FOO").IgnoringCase();

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).ContainsValue(1);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
