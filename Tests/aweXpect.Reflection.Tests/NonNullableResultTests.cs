using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests;

public sealed class NonNullableResultTests
{
	private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
	private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";
	private const string Reason = "the awaited result is the verified subject, which cannot be null";

	[Fact]
	public async Task AssemblyHas_ShouldReturnNonNullableAssembly()
	{
		Assembly? subject = typeof(NonNullableResultTests).Assembly;

		Assembly result = await That(subject).Has<AssemblyTitleAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHas_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = typeof(TestClass).GetConstructor(Type.EmptyTypes);

		ConstructorInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasInParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasInParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasOptionalParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasOptionalParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasOutParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasOutParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasParameter<int>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasParameterExactly_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasParameterExactly<int>("d");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasParamsParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasParamsParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorHasRefParameter_ShouldReturnNonNullableConstructor()
	{
		ConstructorInfo? subject = GetParameterConstructor();

		ConstructorInfo result = await That(subject).HasRefParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task ConstructorsHaveParameter_ShouldReturnNonNullableCollection()
	{
		IEnumerable<ConstructorInfo?> subject = [GetParameterConstructor(),];

		IEnumerable<ConstructorInfo?> result = await That(subject).HaveParameter<int>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventHas_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventIsOfExactType_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).IsOfExactType<EventHandler>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task EventIsOfType_ShouldReturnNonNullableEvent()
	{
		EventInfo? subject = typeof(TestClass).GetEvent(nameof(TestClass.Event));

		EventInfo result = await That(subject).IsOfType<EventHandler>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldHas_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldIsOfExactType_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).IsOfExactType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task FieldIsOfType_ShouldReturnNonNullableField()
	{
		FieldInfo? subject = typeof(TestClass).GetField(nameof(TestClass.Field));

		FieldInfo result = await That(subject).IsOfType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberDoesNotHaveName_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).DoesNotHaveName("Foo");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberHasName_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).HasName(nameof(TestClass.Property));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberIsNotObsolete_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsNotObsolete();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MemberIsPublic_ShouldReturnNonNullableMember()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsPublic();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHas_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasInParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasInParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasOptionalParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasOptionalParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasOutParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasOutParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasParameter<int>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasParameterAtIndex_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasParameter<int>("d").IgnoringCase().AtIndex(3);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasParameterExactly_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasParameterExactly<int>("d");

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasParamsParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasParamsParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodHasRefParameter_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod));

		MethodInfo result = await That(subject).HasRefParameter();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodIsGeneric_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(Array).GetMethod(nameof(Array.Empty));

		MethodInfo result = await That(subject).IsGeneric();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturns_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).Returns<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturnsExactly_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.Method));

		MethodInfo result = await That(subject).ReturnsExactly<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodReturnsVoid_ShouldReturnNonNullableMethod()
	{
		MethodInfo? subject = typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod));

		MethodInfo result = await That(subject).ReturnsVoid();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodsAreGeneric_ShouldReturnNonNullableCollection()
	{
		IEnumerable<MethodInfo?> subject = [typeof(Array).GetMethod(nameof(Array.Empty)),];

		IEnumerable<MethodInfo?> result = await That(subject).AreGeneric();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task MethodsHaveParameter_ShouldReturnNonNullableCollection()
	{
		IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.ParameterMethod)),];

		IEnumerable<MethodInfo?> result = await That(subject).HaveParameter<int>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyHas_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyIsOfExactType_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsOfExactType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task PropertyIsOfType_ShouldReturnNonNullableProperty()
	{
		PropertyInfo? subject = typeof(TestClass).GetProperty(nameof(TestClass.Property));

		PropertyInfo result = await That(subject).IsOfType<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeContainsConstructors_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).ContainsConstructors(constructors => constructors);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeContainsEvents_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).ContainsEvents(events => events);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeContainsFields_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).ContainsFields(fields => fields);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeContainsMethods_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).ContainsMethods(methods => methods).AtLeast().Once();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeContainsProperties_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).ContainsProperties(properties => properties).Once();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOn_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).DependsOn<Uri>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOnlyOnNamespace_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DependsOnlyOn(Layer1Namespace).ExcludingOwnSubNamespaces();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOnlyOnTypeSet_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace))
			.ExcludingOwnSubNamespaces();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOnNamespace_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DependsOn(Layer1Namespace).ExcludingSubNamespaces();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDependsOnTypeSet_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DependsOn(Types.InNamespace(Layer1Namespace))
			.OrOn(Types.InNamespace(Layer2Namespace));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDoesNotDependOn_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).DoesNotDependOn<NonNullableResultTests>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDoesNotDependOnNamespace_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DoesNotDependOn(Layer2Namespace);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeDoesNotDependOnTypeSet_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(OnlyLayer1);

		Type result = await That(subject).DoesNotDependOn(Types.InNamespace(Layer2Namespace));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeHas_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(TestClass);

		Type result = await That(subject).Has<TestAttribute>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeHasDependenciesOutsideNamespace_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(Layer1AndLayer2);

		Type result = await That(subject).HasDependenciesOutside(Layer1Namespace).ExcludingOwnSubNamespaces();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeHasDependenciesOutsideTypeSet_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(Layer1AndLayer2);

		Type result = await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace))
			.ExcludingOwnSubNamespaces();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeIsGeneric_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(List<Uri>);

		Type result = await That(subject).IsGeneric();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypeIsGenericWithArgumentAtIndex_ShouldReturnNonNullableType()
	{
		Type? subject = typeof(List<Uri>);

		Type result = await That(subject).IsGeneric().WithArgument<Uri>().AtIndex(0).FromEnd();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesAreGeneric_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(List<Uri>),];

		IEnumerable<Type?> result = await That(subject).AreGeneric();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesContainProperties_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(TestClass),];

		IEnumerable<Type?> result = await That(subject).ContainProperties(properties => properties);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesDependOnlyOnNamespace_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

		IEnumerable<Type?> result = await That(subject).DependOnlyOn(Layer1Namespace);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesDependOnlyOnTypeSet_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

		IEnumerable<Type?> result = await That(subject).DependOnlyOn(Types.InNamespace(Layer1Namespace));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesDependOnNamespace_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

		IEnumerable<Type?> result = await That(subject).DependOn(Layer1Namespace);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesDependOnTypeSet_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

		IEnumerable<Type?> result = await That(subject).DependOn(Types.InNamespace(Layer1Namespace));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesHaveDependenciesOutsideNamespace_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(Layer1AndLayer2),];

		IEnumerable<Type?> result = await That(subject).HaveDependenciesOutside(Layer1Namespace);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Fact]
	public async Task TypesHaveDependenciesOutsideTypeSet_ShouldReturnNonNullableCollection()
	{
		IEnumerable<Type?> subject = [typeof(Layer1AndLayer2),];

		IEnumerable<Type?> result = await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	private static ConstructorInfo? GetParameterConstructor()
		=> typeof(TestClass).GetConstructor([
			typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int),
			typeof(int[]),
		]);

	[AttributeUsage(AttributeTargets.All)]
	private sealed class TestAttribute : Attribute;

	[Test]
	private sealed class TestClass
	{
		[Test] public Uri Field = new("https://awexpect.com");

		[Test]
		public TestClass()
		{
		}

		public TestClass(in int a, out int b, ref int c, int d = 0, params int[] e) => b = a + c + d + e.Length;

		[Test] public Uri? Property { get; set; }

		[Test] public event EventHandler? Event;

		[Test]
		public Uri? Method() => Field;

		public void ParameterMethod(in int a, out int b, ref int c, int d = 0, params int[] e)
			=> b = a + c + d + e.Length;

		public void VoidMethod() => Event?.Invoke(this, EventArgs.Empty);
	}
}
