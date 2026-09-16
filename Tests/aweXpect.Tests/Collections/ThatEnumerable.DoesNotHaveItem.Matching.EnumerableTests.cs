using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed partial class Matching
		{
			public sealed class EnumerablePredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = new[]
					{
						0, 1, 2,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have item matching _ => true at index 2,
						              but it had item 2 at index 2

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
				{
					IEnumerable subject = new[]
					{
						0, 1, 2,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item matching _ => true at index 0,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableGenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = new object[]
					{
						"foo", 1,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<int>().AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item of type int at index 1,
						             but it had item 1 at index 1

						             Collection:
						             [
						               "foo",
						               1
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatchAtGivenIndex_ShouldSucceed()
				{
					IEnumerable subject = new object[]
					{
						"foo", 1,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<int>().AtIndex(0);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerableGenericPredicateTests
			{
				[Fact]
				public async Task WhenItemOfTypeMatchesAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = new object[]
					{
						"foo", 1,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<int>(x => x == 1).AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item of type int matching x => x == 1 at index 1,
						             but it had item 1 at index 1

						             Collection:
						             [
						               "foo",
						               1
						             ]
						             """);
				}

				[Fact]
				public async Task WhenPredicateDoesNotMatch_ShouldSucceed()
				{
					IEnumerable subject = new object[]
					{
						"foo", 1,
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<int>(x => x == 2).AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
