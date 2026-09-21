#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class Are
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyBaseClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatAsyncEnumerable.All.Are.MyClass for all items,
						             but none of 10 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are<MyBaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are<MyClass>();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyBaseClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are(typeof(MyClass));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatAsyncEnumerable.All.Are.MyClass for all items,
						             but none of 10 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatAsyncEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10));

					async Task Act()
						=> await That(subject).All().Are(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The type cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are(typeof(MyBaseClass));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(
						Enumerable.Range(1, 10), v => new MyClass
						{
							Foo = v,
						});

					async Task Act()
						=> await That(subject).All().Are(typeof(MyClass));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenAllItemsMatchType_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyClass { Foo = 1, }, new MyClass { Foo = 2, });

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().Are<MyClass>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatAsyncEnumerable.All.Are.MyClass not for all items,
						             but all 2 were

						             Collection:
						             [
						               ThatAsyncEnumerable.All.Are.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatAsyncEnumerable.All.Are.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenOneItemDoesNotMatchType_ShouldSucceed()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyClass { Foo = 1, }, new MyBaseClass { Foo = 2, });

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().Are<MyClass>());

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
