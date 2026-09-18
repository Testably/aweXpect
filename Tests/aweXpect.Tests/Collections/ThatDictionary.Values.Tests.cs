using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Values
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportChainingWithAnd()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,]);

				async Task Act()
					=> await That(subject).Values.HasCount(3).And.Contains(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which have exactly 3 items and contain 4 at least once,
					             but it did not contain it

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectationOnValuesFails_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3, 1,]);

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
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,]);

				async Task Act()
					=> await That(subject).Values.All().AreUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegated_AndExpectationOnValuesFails_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Values.All().AreUnique());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegated_AndExpectationOnValuesIsSatisfied_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Values.All().AreUnique());

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IDictionary<int, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Values.All().AreUnique());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are not unique for all items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<int, int>? subject = null;

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
				IDictionary<int, string?> subject = ToDictionary(["a", "A",]);

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

			[Fact]
			public async Task WithMemberAccessor_ShouldVerifyTheMember()
			{
				IDictionary<int, MyClass> subject = ToDictionary([1, 2, 1,], x => new MyClass(x));

				async Task Act()
					=> await That(subject).Values.All().AreUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has values which are unique for x => x.Value for all items,
					             but only 1 of 3 were
					             *
					             """).AsWildcard();
			}
		}

		public sealed class DictionaryTests
		{
			[Fact]
			public async Task WhenExpectationOnValuesIsSatisfied_ShouldSucceed()
			{
				Dictionary<int, string> subject = new()
				{
					[1] = "foo",
					[2] = "bar",
				};

				async Task Act()
					=> await That(subject).Values.Contains("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

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
