using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class Are
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = Enumerable.Range(1, 10).Select(v => new MyBaseClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().Are<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatEnumerable.All.Are.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
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
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
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
					IEnumerable<MyBaseClass> subject = Enumerable.Range(1, 10).Select(v => new MyBaseClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().Are(typeof(MyClass));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatEnumerable.All.Are.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 1
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 2
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 3
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 4
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 5
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 6
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 7
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 8
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 9
						               },
						               ThatEnumerable.All.Are.MyBaseClass {
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<int> subject = Enumerable.Range(1, 10);

					async Task Act()
						=> await That(subject).All().Are(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The type cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
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
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
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
					IEnumerable<MyBaseClass> subject = [new MyClass { Foo = 1, }, new MyClass { Foo = 2, },];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().Are<MyClass>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type ThatEnumerable.All.Are.MyClass for not all items,
						             but all 2 were

						             Collection:
						             [
						               ThatEnumerable.All.Are.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatEnumerable.All.Are.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenOneItemDoesNotMatchType_ShouldSucceed()
				{
					IEnumerable<MyBaseClass> subject = [new MyClass { Foo = 1, }, new MyBaseClass { Foo = 2, },];

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
