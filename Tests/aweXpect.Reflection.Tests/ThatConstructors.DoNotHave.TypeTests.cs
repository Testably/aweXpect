using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed partial class DoNotHave
	{
		public sealed class TypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoItemHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(long),]), typeof(TestClass).GetConstructor([typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

#endif
			[Test]
			public async Task WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor([typeof(long),]), typeof(TestClass).GetConstructor(Type.EmptyTypes),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.TypeTests.FooAttribute,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.TypeTests.TestClass()
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.TypeTests.TestClass(long value),
					               ThatConstructors.DoNotHave.TypeTests.TestClass()
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class BarAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo(Value = 1)]
				public TestClass() { }

				[Foo(Value = 2)]
				public TestClass(int value) { }

				[Bar]
				public TestClass(string value) { }

				public TestClass(long value) { }
			}
		}
	}
}
