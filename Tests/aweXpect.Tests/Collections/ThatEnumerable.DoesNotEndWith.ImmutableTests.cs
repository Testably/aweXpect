#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotEndWith
	{
		public sealed class ImmutableTests
		{
			[Fact]
			public async Task ShouldSupportCaseInsensitiveComparison()
			{
				ImmutableArray<string?> subject = ["FOO", "BAR",];

				async Task Act()
					=> await That(subject).DoesNotEndWith("bar").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["bar"] ignoring case,
					             but it did end with [
					               "BAR"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(x => new MyClass(x), 6),];

				async Task Act()
					=> await That(subject).DoesNotEndWith(
						new MyClass(3),
						new MyClass(5),
						new MyClass(8)
					).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [MyClass { StringValue = "", Value = 3 }, MyClass { StringValue = "", Value = 5 }, MyClass { StringValue = "", Value = 8 }] equivalent,
					             but it did end with [
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 5
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 8
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [1, 2, 3],
					             but it did end with [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 0, 1, 2, 3,];
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectEndsWithUnexpectedValues_ShouldFail()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];
				IEnumerable<string> unexpected = ["bar", "baz",];

				async Task Act()
					=> await That(subject)!.DoesNotEndWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["bar", "baz"],
					             but it did end with [
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(0, 0, 1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotEndWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}
		}
	}
}
#endif
