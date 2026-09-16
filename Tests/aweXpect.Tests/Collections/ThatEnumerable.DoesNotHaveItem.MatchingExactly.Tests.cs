using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed partial class MatchingExactly
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesExactlyAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

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
					IEnumerable<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyBaseClass>().AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

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

			public sealed class GenericPredicateTests
			{
				[Fact]
				public async Task WhenItemOfTypeMatchesAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyClass>(x => x.Value == 1).AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have item exactly of type MyClass matching x => x.Value == 1 at index 1,
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
					IEnumerable<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyBaseClass>(_ => true).AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
