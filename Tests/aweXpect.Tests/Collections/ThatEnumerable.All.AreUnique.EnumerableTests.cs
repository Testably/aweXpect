using System.Collections;
using System.Linq;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreUnique
		{
			public sealed class EnumerableTests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().AreUnique().WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but could not verify, because it was already cancelled
						             *
						             """).AsWildcard();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [1, 1]

						             Collection:
						             [1, 2, 3, 1]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1, 2, -1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [1, 2, 1, 2]

						             Collection:
						             [1, 2, 3, 1, 2, -1]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleNullItems_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<object?>([null, null,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               <null>,
						               <null>
						             ]

						             Collection:
						             [
						               <null>,
						               <null>
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedTests
			{
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but all 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerableMemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => (x as MyClass)?.Value for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable subject =
						ToEnumerable([1, 2, 3, 1, 2, -1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => (x as MyClass)?.Value for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = -1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => (x as MyClass)?.Value for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedMemberTests
			{
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => (x as MyClass)?.Value));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for x => (x as MyClass)?.Value for all items,
						             but all 3 were

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => (x as MyClass)?.Value));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
