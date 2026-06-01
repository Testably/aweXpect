using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveAttribute_ShouldSucceed()
			{
				Type subject = typeof(BarClass);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasAttributeIndirectly_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooChildClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasAttributeIndirectly_WhenInheritIsFalse_ShouldSucceed()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(false);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class BarAttribute : Attribute
			{
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			[Bar]
			private class BarClass
			{
			}

			private class FooChildClass2 : FooClass2
			{
			}
		}
	}
}
