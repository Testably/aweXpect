using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class DoNotHave
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
					             all have no ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching constructors

					             Not matching items:
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
					             not all have no ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching constructors

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
					             all have no ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching constructors

					             Not matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoConstructorHasAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes), null,
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
