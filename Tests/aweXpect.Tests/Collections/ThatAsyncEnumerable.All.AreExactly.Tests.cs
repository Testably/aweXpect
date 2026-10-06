#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class AreExactly
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenItemsHaveAValueAndTypeIsNullable_ShouldSucceed()
				{
					IAsyncEnumerable<int?> subject = ToAsyncEnumerable<int?>(1, 2);

					async Task Act()
						=> await That(subject).All().AreExactly<int?>();

					await That(Act).DoesNotThrow()
						.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyBaseClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatAsyncEnumerable.All.AreExactly.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatAsyncEnumerable.All.AreExactly.MyBaseClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

					await That(Act).DoesNotThrow();
				}
			}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
			public sealed class TypeTests
			{
				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyBaseClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatAsyncEnumerable.All.AreExactly.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10));

					async Task Act()
						=> await That(subject).All().AreExactly(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The 'type' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyBaseClass));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatAsyncEnumerable.All.AreExactly.MyBaseClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).DoesNotThrow();
				}
			}
#pragma warning restore CA2263

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllItemsMatchTypeExactly_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyClass { Foo = 1, }, new MyClass { Foo = 2, });

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreExactly<MyClass>());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatAsyncEnumerable.All.AreExactly.MyClass not for all items,
						             but all 2 were

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               }
						             ]
						             """);
				}

				[Test]
				public async Task WhenOneItemIsOfDerivedType_ShouldSucceed()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyClass { Foo = 1, }, new MyBaseClass { Foo = 2, });

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreExactly<MyBaseClass>());

					await That(Act).DoesNotThrow();
				}
			}

			public class MyClass : MyBaseClass
			{
				public int Bar { get; set; }
			}

			public class MyBaseClass
			{
				public int Foo { get; set; }
			}
		}
	}
}
#endif
