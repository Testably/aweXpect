#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreNotUnique
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 1, 2,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 1, 3,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [2, 3]

						             Collection:
						             [1, 2, 1, 3]
						             """);
				}

				[Fact]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2,], x => new MyClass(x, "a"));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.StringValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringElementTests
			{
				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "a", "b",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "cc", "dd",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!).IgnoringCase();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
