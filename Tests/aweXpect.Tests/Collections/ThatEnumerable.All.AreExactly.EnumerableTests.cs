using System.Collections;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreExactly
		{
			public sealed class EnumerableGenericTests
			{
				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyBaseClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

					await That(Act).Throws<FailException>()
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
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly<MyBaseClass>();

					await That(Act).Throws<FailException>()
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
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly<MyClass>();

					await That(Act).DoesNotThrow();
				}
			}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
			public sealed class EnumerableTypeTests
			{
				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyBaseClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).Throws<FailException>()
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
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable subject = Enumerable.Range(1, 10);

					async Task Act()
						=> await That(subject).All().AreExactly(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("type").And
						.WithMessage("The 'type' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenTypeMatchesBaseType_ShouldFail()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyBaseClass));

					await That(Act).Throws<FailException>()
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
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeMatchesExactly_ShouldSucceed()
				{
					IEnumerable subject = Enumerable.Range(1, 10).Select(v => new MyClass
					{
						Foo = v,
					});

					async Task Act()
						=> await That(subject).All().AreExactly(typeof(MyClass));

					await That(Act).DoesNotThrow();
				}
			}
#pragma warning restore CA2263
		}
	}
}
