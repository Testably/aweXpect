using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class FieldInfoHelpersTests
{
	[Fact]
	public async Task HasAccessModifier_Internal_ShouldMatchForExpectedField()
	{
		FieldInfo fieldInfo = typeof(AccessModifierTestClass).GetDeclaredFields()
			.Single(c => c.Name == nameof(AccessModifierTestClass.InternalField));

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsTrue();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Private_ShouldMatchForExpectedField()
	{
		FieldInfo fieldInfo = typeof(AccessModifierTestClass).GetDeclaredFields()
			.Single(c => c.Name == "PrivateField");

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsTrue();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Protected_ShouldMatchForExpectedField()
	{
		FieldInfo fieldInfo = typeof(AccessModifierTestClass).GetDeclaredFields()
			.Single(c => c.Name == "ProtectedField");

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsTrue();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAccessModifier_Public_ShouldMatchForExpectedField()
	{
		FieldInfo fieldInfo = typeof(AccessModifierTestClass).GetDeclaredFields()
			.Single(c => c.Name == nameof(AccessModifierTestClass.PublicField));

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsTrue();
	}

	[Fact]
	public async Task HasAccessModifier_WhenNull_ShouldReturnFalse()
	{
		FieldInfo? fieldInfo = null;

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		FieldInfo fieldInfo = typeof(TestClass).GetField(nameof(TestClass.Field1WithAttribute))!;

		bool result = fieldInfo.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		FieldInfo fieldInfo = typeof(TestClass).GetField(nameof(TestClass.Field2WithoutAttribute))!;

		bool result = fieldInfo.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		FieldInfo fieldInfo = typeof(TestClass).GetField(nameof(TestClass.Field1WithAttribute))!;

		bool result1 = fieldInfo.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = fieldInfo.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[AttributeUsage(AttributeTargets.Field)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

#pragma warning disable CS0649
	private class TestClass
	{
		[Dummy(1)] public int Field1WithAttribute = 1;

		public int Field2WithoutAttribute = 1;
	}

	private class AccessModifierTestClass
	{
		internal int InternalField = 4;
		public int PublicField = 1;

#pragma warning disable CS0414
		private int PrivateField = 2;

		protected int ProtectedField = 3;
#pragma warning restore CS0414
	}
#pragma warning restore CS0649
}
