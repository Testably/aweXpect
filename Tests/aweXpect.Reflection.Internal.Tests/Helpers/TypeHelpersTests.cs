using System.Collections.Generic;
using System.IO;
using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

// ReSharper disable UnusedMember.Local
public sealed class TypeHelpersTests
{
	[Fact]
	public async Task GetDeclaredFields_ShouldFilterOutBackingFields()
	{
		Type type = typeof(TestClassWithPropertyAndField);

		FieldInfo[] allFields = type.GetFields(BindingFlags.DeclaredOnly |
		                                       BindingFlags.NonPublic |
		                                       BindingFlags.Public |
		                                       BindingFlags.Instance);
		FieldInfo[] result = type.GetDeclaredFields();

		await That(allFields).HasCount(2);
		await That(result).HasCount(1);
	}

	[Theory]
	[InlineData(typeof(TypeLoadException))]
	[InlineData(typeof(FileNotFoundException))]
	[InlineData(typeof(FileLoadException))]
	public async Task GetDeclaredMembers_WhenReflectionThrowsLoadException_ShouldReturnEmpty(Type exceptionType)
	{
		Exception exception = (Exception)Activator.CreateInstance(exceptionType)!;
		Type type = new ThrowingType(typeof(TestClassWithAttribute), exception);

		await That(type.GetDeclaredConstructors()).IsEmpty();
		await That(type.GetDeclaredEvents()).IsEmpty();
		await That(type.GetDeclaredFields()).IsEmpty();
		await That(type.GetDeclaredMethods()).IsEmpty();
		await That(type.GetDeclaredProperties()).IsEmpty();
	}

	[Fact]
	public async Task GetDeclaredMembers_WhenReflectionThrowsUnrelatedException_ShouldNotCatch()
	{
		Type type = new ThrowingType(typeof(TestClassWithAttribute), new InvalidOperationException());

		async Task Act()
		{
			await Task.Yield();
			type.GetDeclaredMethods();
		}

		await That(Act).Throws<InvalidOperationException>();
	}

	[Fact]
	public async Task HasAttribute_WithAttribute_ShouldReturnTrue()
	{
		Type type = typeof(TestClassWithAttribute);

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithInheritedAttribute_ShouldReturnTrue()
	{
		Type type = typeof(TestClassWithInheritedAttribute);

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task HasAttribute_WithoutAttribute_ShouldReturnFalse()
	{
		Type type = typeof(TestClassWithoutAttribute);

		bool result = type.HasAttribute<DummyAttribute>();

		await That(result).IsFalse();
	}

	[Fact]
	public async Task HasAttribute_WithPredicate_ShouldReturnPredicateResult()
	{
		Type type = typeof(TestClassWithAttribute);

		bool result1 = type.HasAttribute<DummyAttribute>(d => d.Value == 1);
		bool result2 = type.HasAttribute<DummyAttribute>(d => d.Value == 2);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task Implements_Object_ShouldReturnFalse(bool forceDirect)
	{
		Type sut = typeof(object);

		bool result = sut.Implements(typeof(IFooInterface), forceDirect);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task InheritsFrom_SameType_ShouldReturnFalse()
	{
		Type sut = typeof(TestClassWithAttribute);

		bool result = sut.InheritsFrom(typeof(TestClassWithAttribute));

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsEqualTo_DifferentClosedGenericType_With2Parameters_ShouldReturnFalse()
	{
		Type sut = typeof(Dictionary<int, string>);

		bool result = sut.IsEqualTo(typeof(Dictionary<int, int>));

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsEqualTo_DifferentClosedGenericTypeWith1Parameter_ShouldReturnFalse()
	{
		Type sut = typeof(List<string>);

		bool result = sut.IsEqualTo(typeof(List<int>));

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsEqualTo_DifferentNumberOfClosedGenericParameters_ShouldReturnFalse()
	{
		Type sut = typeof(Tuple<int, string>);

		bool result = sut.IsEqualTo(typeof(Tuple<int, string, int>));

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsEqualTo_DifferentNumberOfOpenGenericParameters_ShouldReturnFalse()
	{
		Type sut = typeof(Tuple<,>);

		bool result = sut.IsEqualTo(typeof(Tuple<,,>));

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsEqualTo_SameClosedGenericTypeWith1Parameter_ShouldReturnTrue()
	{
		Type sut = typeof(List<string>);

		bool result = sut.IsEqualTo(typeof(List<string>));

		await That(result).IsTrue();
	}

	[Fact]
	public async Task IsEqualTo_SameClosedGenericTypeWith2Parameters_ShouldReturnTrue()
	{
		Type sut = typeof(Dictionary<int, string>);

		bool result = sut.IsEqualTo(typeof(Dictionary<int, string>));

		await That(result).IsTrue();
	}

	[Fact]
	public async Task IsEqualTo_SameGenericType_OtherOpen_ShouldReturnTrue()
	{
		Type sut = typeof(Task<>);

		bool result = sut.IsEqualTo(typeof(Task<int>));

		await That(result).IsTrue();
	}

	[Fact]
	public async Task IsEqualTo_SameGenericType_TypeOpen_ShouldReturnTrue()
	{
		Type sut = typeof(Dictionary<int, string>);

		bool result = sut.IsEqualTo(typeof(Dictionary<,>));

		await That(result).IsTrue();
	}

	[Fact]
	public async Task IsOrInheritsFrom_SameType_ShouldReturnTrue()
	{
		Type sut = typeof(TestClassWithAttribute);

		bool result = sut.IsOrInheritsFrom(typeof(TestClassWithAttribute));

		await That(result).IsTrue();
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	private class DummyAttribute : Attribute
	{
		public DummyAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

	private interface IFooInterface
	{
	}

	private sealed class ThrowingType(Type inner, Exception exception) : TypeDelegator(inner)
	{
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw exception;
		public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw exception;
		public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw exception;
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw exception;
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => throw exception;
	}

	[Dummy(1)]
	private class TestClassWithAttribute
	{
		public void Method1WithoutAttribute()
			=> throw new NotSupportedException();

		public void Method2WithoutAttribute()
			=> throw new NotSupportedException();
	}

	private class TestClassWithInheritedAttribute : TestClassWithAttribute
	{
	}

	private class TestClassWithoutAttribute
	{
		[Dummy(1)]
		public void Method1WithAttribute()
			=> throw new NotSupportedException();

		[Dummy(2)]
		public void Method2WithAttribute()
			=> throw new NotSupportedException();
	}

	public class TestClassWithPropertyAndField
	{
		public int MyField = 1;
		public int MyProperty { get; set; } = 2;
	}
}
