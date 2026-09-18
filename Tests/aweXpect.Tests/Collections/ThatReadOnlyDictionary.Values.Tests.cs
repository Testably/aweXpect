using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class Values
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationOnValuesFails_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).Values.All().AreUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are unique for all items,
					             but only 2 of 4 were

					             Not matching items:
					             [1, 1]

					             Collection:
					             [1, 2, 3, 1]
					             """);
			}

			[Fact]
			public async Task WhenExpectationOnValuesIsSatisfied_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,]);

				async Task Act()
					=> await That(subject).Values.All().AreUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IReadOnlyDictionary<int, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Values.All().AreUnique());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are unique for not all items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<int, int>? subject = null;

				async Task Act()
					=> await That(subject).Values.All().AreUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are unique for all items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValuesAreStrings_ShouldSupportStringOptions()
			{
				IReadOnlyDictionary<int, string?> subject = ToDictionary(["a", "A",]);

				async Task Act()
					=> await That(subject).Values.All().AreUnique().IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are unique ignoring case for all items,
					             but none of 2 were

					             Not matching items:
					             [
					               "a",
					               "A"
					             ]

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}
		}

		public sealed class ReadOnlyDictionaryTests
		{
			[Fact]
			public async Task WhenExpectationOnValuesIsSatisfied_ShouldSucceed()
			{
				ReadOnlyDictionary<int, string> subject = new(new Dictionary<int, string>
				{
					[1] = "foo",
					[2] = "bar",
				});

				async Task Act()
					=> await That(subject).Values.Contains("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Values.Contains("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which contain "bar" at least once,
					             but it was <null>
					             """);
			}
		}
	}
}
