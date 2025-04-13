using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenTypesHaveAttribute_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types().With<FooAttribute>();

				async Task Act()
					=> await That(subject).Have<FooAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesHaveAttributeIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesHaveAttributeIndirectly_WhenInheritIsFalse_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have ThatTypes.Have.AttributeTests.FooAttribute,
					             but it contained not matching types [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesHaveMatchingAttribute_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(foo => foo.Value == 2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesHaveMatchingAttributeIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(foo => foo.Value == 2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesHaveMatchingAttributeIndirectly_WhenInheritIsFalse_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(foo => foo.Value == 2, false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it contained not matching types [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesHaveNotMatchingAttribute_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(foo => foo.Value == 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have direct ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but it contained not matching types [
					               ThatTypes.Have.AttributeTests.FooClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesHaveNotMatchingAttributeIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
					=> await That(subject).Have<FooAttribute>(foo => foo.Value == 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have direct ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but it contained not matching types [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			private class FooChildClass2 : FooClass2
			{
			}
		}
	}
}
