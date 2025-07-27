using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

// ReSharper disable UnusedMember.Local
public sealed class MethodInfoHelpersTests
{
	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		MethodInfo type = typeof(TestClass).GetMethod(nameof(TestClass.Method1WithAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		MethodInfo type =
			typeof(TestClass).GetMethod(nameof(TestClass.MethodWithAttributeInBaseClass))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		MethodInfo type = typeof(TestClass).GetMethod(nameof(TestClass.Method2WithoutAttribute))!;

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		MethodInfo type = typeof(TestClass).GetMethod(nameof(TestClass.Method1WithAttribute))!;

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

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
}
