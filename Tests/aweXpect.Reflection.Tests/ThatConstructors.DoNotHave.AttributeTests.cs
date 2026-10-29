using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAConstructorHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}
#endif

			[Test]
			public async Task Negated_WhenNoConstructorHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass()
					             ]
					             """);
			}

			[Test]
			public async Task WhenAConstructorHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoConstructorHasAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
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
