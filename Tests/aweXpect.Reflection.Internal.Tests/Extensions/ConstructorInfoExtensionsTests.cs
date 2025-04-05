using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Internal.Tests.Extensions;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
public sealed class ConstructorInfoExtensionsTests
{
	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		ConstructorInfo type = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 2);

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		ConstructorInfo type = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 3);

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		ConstructorInfo type = typeof(TestClass).GetDeclaredConstructors()
			.Single(c => c.GetParameters().Length == 2);

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

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
}
