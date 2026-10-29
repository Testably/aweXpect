using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatConstructor.DoesNotHave.AttributeTests.TestClass(string value)
					             """);
			}

			[Test]
			public async Task WithType_WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatConstructor.DoesNotHave.AttributeTests.TestClass(string value)
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class FooAttribute : Attribute
			{
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass()
				{
				}

				[Foo]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value)
				{
				}
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
