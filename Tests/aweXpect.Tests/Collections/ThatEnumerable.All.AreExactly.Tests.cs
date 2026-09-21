using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreExactly
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = Enumerable.Range(1, 10).Select(_ => new MyBaseClass());

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatEnumerable.All.AreExactly.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 0
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly<MyBaseClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatEnumerable.All.AreExactly.MyBaseClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 3
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 4
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 5
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 6
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 7
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 8
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 9
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

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
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatEnumerable.All.AreExactly.MyClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 1
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 2
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 3
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 4
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 5
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 6
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 7
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 8
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
						                 Foo = 9
						               },
						               ThatEnumerable.All.AreExactly.MyBaseClass {
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
						=> await That(subject).All().AreExactly(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The type cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyBaseClass));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatEnumerable.All.AreExactly.MyBaseClass for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 3
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 4
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 5
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 6
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 7
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 8
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 9
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 10
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenAllItemsMatchTypeExactly_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = [new MyClass { Foo = 1, }, new MyClass { Foo = 2, },];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreExactly<MyClass>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ThatEnumerable.All.AreExactly.MyClass not for all items,
						             but all 2 were

						             Collection:
						             [
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 1
						               },
						               ThatEnumerable.All.AreExactly.MyClass {
						                 Bar = 0,
						                 Foo = 2
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenOneItemIsOfDerivedType_ShouldSucceed()
				{
					IEnumerable<MyBaseClass> subject = [new MyClass { Foo = 1, }, new MyBaseClass { Foo = 2, },];

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
