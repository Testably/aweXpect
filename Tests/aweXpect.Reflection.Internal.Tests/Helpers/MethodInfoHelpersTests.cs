using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

// ReSharper disable UnusedMember.Local
public sealed class MethodInfoHelpersTests
{
	[Fact]
	public async Task HasAccessModifier_Internal_ShouldMatchForExpectedMethod()
	{
		MethodInfo methodInfo = typeof(AccessModifierTestClass).GetDeclaredMethods()
			.Single(c => c.Name == nameof(AccessModifierTestClass.InternalMethod));

		await That(methodInfo.HasAccessModifier(AccessModifiers.Internal)).IsTrue();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Private_ShouldMatchForExpectedMethod()
	{
		MethodInfo methodInfo = typeof(AccessModifierTestClass).GetDeclaredMethods()
			.Single(c => c.Name == "PrivateMethod");

		await That(methodInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Private)).IsTrue();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Protected_ShouldMatchForExpectedMethod()
	{
		MethodInfo methodInfo = typeof(AccessModifierTestClass).GetDeclaredMethods()
			.Single(c => c.Name == "ProtectedMethod");

		await That(methodInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Protected)).IsTrue();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Public_ShouldMatchForExpectedMethod()
	{
		MethodInfo methodInfo = typeof(AccessModifierTestClass).GetDeclaredMethods()
			.Single(c => c.Name == nameof(AccessModifierTestClass.PublicMethod));

		await That(methodInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Public)).IsTrue();
	}

	[Fact]
	public async Task HasAccessModifier_WhenNull_ShouldReturnFalse()
	{
		MethodInfo? methodInfo = null;

		await That(methodInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(methodInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Method1WithAttribute))!;

		bool result = methodInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		MethodInfo methodInfo =
			typeof(TestClass).GetMethod(nameof(TestClass.MethodWithAttributeInBaseClass))!;

		bool result = methodInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Method2WithoutAttribute))!;

		bool result = methodInfo.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Method1WithAttribute))!;

		bool result1 = methodInfo.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = methodInfo.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Method)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

	private class TestClass : TestClassBase
	{
		[Dummy(1)]
		public void Method1WithAttribute()
			=> throw new NotSupportedException();

		public void Method2WithoutAttribute()
			=> throw new NotSupportedException();

		public override void MethodWithAttributeInBaseClass()
			=> throw new NotSupportedException();
	}

	private class TestClassBase
	{
		[Dummy(1)]
		public virtual void MethodWithAttributeInBaseClass()
			=> throw new NotSupportedException();
	}

	private class AccessModifierTestClass
	{
#pragma warning disable CA1822
		internal int InternalMethod() => 1;

		private int PrivateMethod() => 2;

		protected int ProtectedMethod() => 3;
		public int PublicMethod() => 1;
#pragma warning restore CA1822
	}
}
