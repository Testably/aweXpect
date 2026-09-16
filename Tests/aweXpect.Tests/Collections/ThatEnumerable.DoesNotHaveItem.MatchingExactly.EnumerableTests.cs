using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed partial class MatchingExactly
		{
			public sealed class EnumerableGenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesExactlyAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = new object[]
					{
						new MyBaseClass(0), new MyClass(1),
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyClass>().AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item exactly of type MyClass at index 1,
						             but it had item MyClass {
						               StringValue = "",
						               Value = 1
						             } at index 1

						             Collection:
						             [
						               MyBaseClass {
						                 Value = 0
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					IEnumerable subject = new object[]
					{
						new MyBaseClass(0), new MyClass(1),
					};

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyBaseClass>().AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyBaseClass>().AtIndex(0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item exactly of type MyBaseClass at index 0,
						             but it was <null>
						             """);
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
						=> await That(subject).DoesNotHaveItem().MatchingExactly<int>(x => x == 1).AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item exactly of type int matching x => x == 1 at index 1,
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
						=> await That(subject).DoesNotHaveItem().MatchingExactly<int>(x => x == 2).AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
