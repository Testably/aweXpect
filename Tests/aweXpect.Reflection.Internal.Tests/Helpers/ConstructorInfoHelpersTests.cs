using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
public sealed class ConstructorInfoHelpersTests
{
	[Fact]
	public async Task HasAccessModifier_Internal_ShouldMatchForExpectedConstructor()
	{
		ConstructorInfo constructorInfo = typeof(AccessModifierTestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Single().Name == "internal");

		await That(constructorInfo.HasAccessModifier(AccessModifiers.Internal)).IsTrue();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Private_ShouldMatchForExpectedConstructor()
	{
		ConstructorInfo constructorInfo = typeof(AccessModifierTestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Single().Name == "private");

		await That(constructorInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Private)).IsTrue();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Protected_ShouldMatchForExpectedConstructor()
	{
		ConstructorInfo constructorInfo = typeof(AccessModifierTestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Single().Name == "protected");

		await That(constructorInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Protected)).IsTrue();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Public_ShouldMatchForExpectedConstructor()
	{
		ConstructorInfo constructorInfo = typeof(AccessModifierTestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Single().Name == "public");

		await That(constructorInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(constructorInfo.HasAccessModifier(AccessModifiers.Public)).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		ConstructorInfo constructorInfo = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 2);

		bool result = constructorInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		ConstructorInfo constructorInfo = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 3);

		bool result = constructorInfo.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		ConstructorInfo constructorInfo = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 2);

		bool result1 = constructorInfo.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = constructorInfo.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Constructor)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

	private class TestClass
	{
		[Dummy(1)]
		public TestClass(int value1, int value2)
		{
		}

		public TestClass(int value1, int value2, int value3)
		{
		}
	}

	private class AccessModifierTestClass
	{
		public AccessModifierTestClass(int @public)
		{
		}

		private AccessModifierTestClass(string @private)
		{
		}

		internal AccessModifierTestClass(DateTime @internal)
		{
		}

		protected AccessModifierTestClass(TimeSpan @protected)
		{
		}
	}
}
