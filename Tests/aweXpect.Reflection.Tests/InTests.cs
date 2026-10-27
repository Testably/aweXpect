using System.Collections.Generic;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

// ReSharper disable PossibleMultipleEnumeration
public sealed class InTests
{
	[Test]
	public async Task AllLoadedAssemblies_ShouldContainMultipleAssemblies()
	{
		Filtered.Assemblies sut = In.AllLoadedAssemblies();

		await That(sut).HasCount().GreaterThanOrEqualTo(4);
		await That(sut).All().Satisfy(x => !x.FullName!.StartsWith("System."));
		await That(sut.GetDescription()).IsEqualTo("in all loaded assemblies");
	}

	[Test]
	public async Task AllLoadedAssemblies_WhenCustomizationScopeStartsAfterCreation_ShouldApplyIt()
	{
		Filtered.Assemblies sut = In.AllLoadedAssemblies();

		using (Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Set(["aweXpect.Reflection",]))
		{
			await That(sut).DoesNotContain(typeof(In).Assembly)
				.Because("the excluded prefixes are read when the source is enumerated, not when it is created");
		}

		await That(sut).Contains(typeof(In).Assembly);
	}

	[Test]
	public async Task AllLoadedAssemblies_WithPredicate_ShouldApplyPredicate()
	{
		Filtered.Assemblies sut = In.AllLoadedAssemblies()
			.Which(assembly => assembly.FullName!.StartsWith("aweXpect."));

		await That(sut).HasCount().Between(2).And(4);
		await That(sut.GetDescription())
			.IsEqualTo("in all loaded assemblies matching assembly => assembly.FullName!.StartsWith(\"aweXpect.\")");
	}

	[Test]
	public async Task Assemblies_WithArray_ShouldContainProvidedAssemblies()
	{
		Assembly[] assemblies = [typeof(In).Assembly, typeof(InTests).Assembly,];

		Filtered.Assemblies sut = In.Assemblies(assemblies);

		await That(sut).IsEqualTo(assemblies);
		await That(sut.GetDescription())
			.IsEqualTo("in the assemblies [aweXpect.Reflection*, aweXpect.Reflection.Tests*]").AsWildcard();
	}

	[Test]
	public async Task Assemblies_WithEnumerable_ShouldContainProvidedAssemblies()
	{
		IEnumerable<Assembly> assemblies = [typeof(In).Assembly, typeof(InTests).Assembly,];

		Filtered.Assemblies sut = In.Assemblies(assemblies);

		await That(sut).IsEqualTo(assemblies);
		await That(sut.GetDescription())
			.IsEqualTo("in the assemblies [aweXpect.Reflection*, aweXpect.Reflection.Tests*]").AsWildcard();
	}

	[Test]
	public async Task AssemblyContaining_WithGeneric_ShouldContainAssemblyOfProvidedType()
	{
		Filtered.Assemblies sut = In.AssemblyContaining<InTests>();

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(InTests).Assembly);
		await That(sut.GetDescription()).IsEqualTo("in assembly containing type InTests");
	}

	[Test]
	public async Task AssemblyContaining_WithType_ShouldContainAssemblyOfProvidedType()
	{
		Filtered.Assemblies sut = In.AssemblyContaining(typeof(In));

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(In).Assembly);
		await That(sut.GetDescription()).IsEqualTo("in assembly containing type In");
	}

	[Test]
	public async Task Constructors_WithEnumerable_ShouldContainProvidedConstructors()
	{
		IEnumerable<ConstructorInfo> constructors = typeof(ClassWithMembers).GetConstructors();

		Filtered.Constructors sut = In.Constructors(constructors);

		await That(sut).IsEqualTo(constructors).InAnyOrder();
		await That(sut.GetDescription()).IsEqualTo("in the constructors ").AsPrefix();
	}

	[Test]
	public async Task Constructors_WithNullConstructor_ShouldIgnoreIt()
	{
		ConstructorInfo constructor = typeof(ClassWithMembers).GetConstructors()[0];

		Filtered.Constructors sut = In.Constructors(null, constructor);

		await That(sut).HasSingle().Which.IsEqualTo(constructor);
	}

	[Test]
	public async Task Constructors_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		ConstructorInfo constructor = typeof(ClassWithMembers).GetConstructors()[0];

		Filtered.Constructors sut = In.Constructors(constructor).Which(c => c.IsPublic);

		await That(sut.GetDescription())
			.IsEqualTo("in the constructors [InTests.ClassWithMembers()] matching c => c.IsPublic");
	}

	[Test]
	public async Task Events_WithEnumerable_ShouldContainProvidedEvents()
	{
		IEnumerable<EventInfo> events = typeof(BaseClassWithMembers).GetEvents();

		Filtered.Events sut = In.Events(events);

		await That(sut).IsEqualTo(events).InAnyOrder();
		await That(sut.GetDescription()).IsEqualTo("in the events ").AsPrefix();
	}

	[Test]
	public async Task Events_WithNullEvent_ShouldIgnoreIt()
	{
		EventInfo @event = typeof(BaseClassWithMembers).GetEvents()[0];

		Filtered.Events sut = In.Events(null, @event);

		await That(sut).HasSingle().Which.IsEqualTo(@event);
	}

	[Test]
	public async Task Events_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		EventInfo @event = typeof(BaseClassWithMembers).GetEvent(nameof(BaseClassWithMembers.BaseEvent))!;

		Filtered.Events sut = In.Events(@event).Which(e => e.Name.Length > 0);

		await That(sut.GetDescription())
			.IsEqualTo(
				"in the events [event EventHandler BaseClassWithMembers.BaseEvent] matching e => e.Name.Length > 0");
	}

	[Test]
	public async Task Fields_ShouldExcludeCompilerGeneratedBackingFields()
	{
		Filtered.Fields fields = In.Type<ClassWithMembers>().Fields();

		await That(fields).All().Satisfy(field => !field.Name.EndsWith("__BackingField")).And.IsNotEmpty();
	}

	[Test]
	public async Task Fields_WithNullField_ShouldIgnoreIt()
	{
		FieldInfo field = typeof(ClassWithMembers).GetField(nameof(ClassWithMembers.Field))!;

		Filtered.Fields sut = In.Fields(null, field);

		await That(sut).HasSingle().Which.IsEqualTo(field);
	}

	[Test]
	public async Task Fields_WithSingleField_ShouldContainProvidedField()
	{
		FieldInfo field = typeof(ClassWithMembers).GetField(nameof(ClassWithMembers.Field))!;

		Filtered.Fields sut = In.Fields(field);

		await That(sut).HasSingle().Which.IsEqualTo(field);
	}

	[Test]
	public async Task Fields_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		FieldInfo field = typeof(ClassWithMembers).GetField(nameof(ClassWithMembers.Field))!;

		Filtered.Fields sut = In.Fields(field).Which(f => f.IsPublic);

		await That(sut.GetDescription())
			.IsEqualTo("in the fields [int InTests.ClassWithMembers.Field] matching f => f.IsPublic");
	}

	[Test]
	public async Task Methods_DeclaringTypes_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		MethodInfo method = typeof(ClassWithMembers).GetMethod(nameof(ClassWithMembers.Method))!;

		Filtered.Types sut = In.Methods(method).DeclaringTypes().WhichAreClasses();

		await That(sut.GetDescription())
			.IsEqualTo(
				"declaring types of in the methods [int InTests.ClassWithMembers.Method()] which are classes");
	}

	[Test]
	public async Task Methods_ShouldExcludeCompilerGeneratedAccessors()
	{
		Filtered.Methods methods = In.Type<ClassWithMembers>().Methods();

		await That(methods).All().Satisfy(method => !method.IsSpecialName).And.IsNotEmpty();
	}

	[Test]
	public async Task Methods_WithNullMethod_ShouldIgnoreIt()
	{
		MethodInfo method = typeof(ClassWithMembers).GetMethod(nameof(ClassWithMembers.Method))!;

		Filtered.Methods sut = In.Methods(null, method);

		await That(sut).HasSingle().Which.IsEqualTo(method);
	}

	[Test]
	public async Task Methods_WithSingleMethod_ShouldContainProvidedMethod()
	{
		MethodInfo method = typeof(ClassWithMembers).GetMethod(nameof(ClassWithMembers.Method))!;

		Filtered.Methods sut = In.Methods(method);

		await That(sut).HasSingle().Which.IsEqualTo(method);
	}

	[Test]
	public async Task Methods_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		MethodInfo method = typeof(ClassWithMembers).GetMethod(nameof(ClassWithMembers.Method))!;

		Filtered.Methods sut = In.Methods(method).Which(m => m.IsPublic);

		await That(sut.GetDescription())
			.IsEqualTo("in the methods [int InTests.ClassWithMembers.Method()] matching m => m.IsPublic");
	}

	[Test]
	public async Task Properties_ShouldComposeWithFilter()
	{
		IEnumerable<PropertyInfo> properties = typeof(ClassWithMembers).GetProperties();

		Filtered.Properties sut = In.Properties(properties).Which(property => property.CanWrite);

		await That(sut).All().Satisfy(property => property.CanWrite).And.IsNotEmpty();
	}

	[Test]
	public async Task Properties_WithEnumerable_ShouldContainProvidedProperties()
	{
		IEnumerable<PropertyInfo> properties = typeof(ClassWithMembers).GetProperties();

		Filtered.Properties sut = In.Properties(properties);

		await That(sut).IsEqualTo(properties).InAnyOrder();
		await That(sut.GetDescription()).IsEqualTo("in the properties ").AsPrefix();
	}

	[Test]
	public async Task Properties_WithNullProperty_ShouldIgnoreIt()
	{
		PropertyInfo property = typeof(ClassWithMembers).GetProperty(nameof(ClassWithMembers.Property))!;

		Filtered.Properties sut = In.Properties(null, property);

		await That(sut).HasSingle().Which.IsEqualTo(property);
	}

	[Test]
	public async Task Properties_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		PropertyInfo property = typeof(ClassWithMembers).GetProperty(nameof(ClassWithMembers.Property))!;

		Filtered.Properties sut = In.Properties(property).Which(p => p.CanWrite);

		await That(sut.GetDescription())
			.IsEqualTo(
				"in the properties [public int InTests.ClassWithMembers.Property { get; set; }] matching p => p.CanWrite");
	}

	[Test]
	public async Task Type_WithGenericParameter_ShouldIncludeSpecifiedType()
	{
		Filtered.Types sut = In.Type<InTests>();

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(InTests));
		await That(sut.GetDescription()).IsEqualTo($"in type {nameof(InTests)}");
	}

	[Test]
	public async Task Type_WithGenericParameter_WithPrefixFilter_ShouldIncludeItInTheFailureMessage()
	{
		async Task Act()
			=> await That(In.Type<ClassWithMembers>().WhichAreSealed()).IsEmpty();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sealed in type InTests.ClassWithMembers
			             is empty,
			             but it was [
			               InTests.ClassWithMembers,
			               (… and maybe more)
			             ]
			             """);
	}

	[Test]
	public async Task Type_WithGenericParameter_WithSuffixFilter_ShouldSeparateItFromTheSourceInTheFailureMessage()
	{
		async Task Act()
			=> await That(In.Type<ClassWithMembers>().WhichAreClasses()).IsEmpty();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that in type InTests.ClassWithMembers which are classes
			             is empty,
			             but it was [
			               InTests.ClassWithMembers,
			               (… and maybe more)
			             ]
			             """);
	}

	[Test]
	public async Task Type_WithGenericParameter_WithSuffixFilter_ShouldSeparateItFromTheSourceOfMembers()
	{
		Filtered.Methods sut = In.Type<ClassWithMembers>().WhichAreClasses().Methods();

		await That(sut.GetDescription()).IsEqualTo("methods in type InTests.ClassWithMembers which are classes");
	}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
	[Test]
	public async Task Type_WithTypeParameter_ShouldIncludeSpecifiedType()
	{
		Filtered.Types sut = In.Type(typeof(InTests));

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(InTests));
		await That(sut.GetDescription()).IsEqualTo($"in type {nameof(InTests)}");
	}

	[Test]
	public async Task Type_WithTypeParameter_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		Filtered.Types sut = In.Type(typeof(InTests)).WhichAreClasses();

		await That(sut.GetDescription()).IsEqualTo($"in type {nameof(InTests)} which are classes");
	}
#pragma warning restore CA2263

	[Test]
	public async Task Types_WhenGetTypesThrowsReflectionTypeLoadException_ShouldUseTypesThatLoaded()
	{
		Type?[] loadableTypes = [typeof(PublicClass), null, typeof(InternalClass),];
		Assembly assembly = new ThrowingAssembly(loadableTypes);

		Filtered.Types sut = In.Assemblies(assembly).Types();

		await That(sut).IsEqualTo([typeof(PublicClass), typeof(InternalClass),]).InAnyOrder();
	}

	[Test]
	public async Task Types_WithEnumerable_ShouldIncludeSpecifiedTypes()
	{
		IEnumerable<Type> types = [typeof(InTests), typeof(InternalClass),];

		Filtered.Types sut = In.Types(types);

		await That(sut).IsEqualTo([typeof(InTests), typeof(InternalClass),]).InAnyOrder();
	}

	[Test]
	public async Task Types_WithEnumerable_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		IEnumerable<Type> types = [typeof(InTests), typeof(InternalClass),];

		Filtered.Types sut = In.Types(types).WhichAreClasses();

		await That(sut.GetDescription())
			.IsEqualTo($"in types [{nameof(InTests)}, {nameof(InternalClass)}] which are classes");
	}

	[Test]
	public async Task Types_WithLazyEnumerable_ShouldEnumerateItOncePerEvaluation()
	{
		int enumerations = 0;

		IEnumerable<Type> CountedTypes()
		{
			enumerations++;
			yield return typeof(InTests);
		}

		Filtered.Types sut = In.Types(CountedTypes());

		await That(enumerations).IsEqualTo(0).Because("creating the source must not enumerate the types");

		await That(sut).AreSealed();

		await That(enumerations).IsEqualTo(1)
			.Because("a successful evaluation enumerates the types once and needs no description");
	}

	[Test]
	public async Task Types_WithNullType_ShouldIgnoreIt()
	{
		Filtered.Types sut = In.Types(null, typeof(InTests));

		await That(sut).HasSingle().Which.IsEqualTo(typeof(InTests));
	}

	[Test]
	public async Task Types_WithThreeGenericParameters_ShouldIncludeSpecifiedTypes()
	{
		Filtered.Types sut = In.Types<InTests, InternalClass, PublicClass>();

		await That(sut).IsEqualTo([typeof(InTests), typeof(InternalClass), typeof(PublicClass),]).InAnyOrder();
		await That(sut.GetDescription())
			.IsEqualTo($"in types {nameof(InTests)}, {nameof(InternalClass)} and {nameof(PublicClass)}");
	}

	[Test]
	public async Task Types_WithThreeGenericParameters_WithSuffixFilter_ShouldSeparateItFromTheSource()
	{
		Filtered.Types sut = In.Types<InTests, InternalClass, PublicClass>().WhichAreClasses();

		await That(sut.GetDescription())
			.IsEqualTo(
				$"in types {nameof(InTests)}, {nameof(InternalClass)} and {nameof(PublicClass)} which are classes");
	}

	[Test]
	public async Task Types_WithTwoGenericParameters_ShouldIncludeSpecifiedTypes()
	{
		Filtered.Types sut = In.Types<InTests, InternalClass>();

		await That(sut).IsEqualTo([typeof(InTests), typeof(InternalClass),]).InAnyOrder();
		await That(sut.GetDescription()).IsEqualTo($"in types {nameof(InTests)} and {nameof(InternalClass)}");
	}

	[Test]
	public async Task Types_WithTwoGenericParameters_WithPrefixFilter_ShouldIncludeItInTheFailureMessage()
	{
		async Task Act()
			=> await That(In.Types<ClassWithMembers, InTests>().WhichAreSealed()).IsEmpty();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sealed in types InTests.ClassWithMembers and InTests
			             is empty,
			             but it was [
			               InTests.ClassWithMembers,
			               (… and maybe more)
			             ]
			             """);
	}

	[Test]
	public async Task Types_WithTwoGenericParameters_WithSuffixFilter_ShouldSeparateItFromTheSourceInTheFailureMessage()
	{
		async Task Act()
			=> await That(In.Types<ClassWithMembers, InTests>().WhichAreClasses()).IsEmpty();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that in types InTests.ClassWithMembers and InTests which are classes
			             is empty,
			             but it was [
			               InTests.ClassWithMembers,
			               (… and maybe more)
			             ]
			             """);
	}

	[Test]
	public async Task Types_WithTypeParameters_ShouldIncludeSpecifiedTypes()
	{
		Filtered.Types sut = In.Types(typeof(InTests), typeof(InternalClass), typeof(PublicClass),
			typeof(AccessModifiers));

		await That(sut)
			.IsEqualTo([typeof(InTests), typeof(InternalClass), typeof(PublicClass), typeof(AccessModifiers),])
			.InAnyOrder();
		await That(sut.GetDescription())
			.IsEqualTo(
				$"in types [{nameof(InTests)}, {nameof(InternalClass)}, {nameof(PublicClass)}, {nameof(AccessModifiers)}]");
	}

	private sealed class ClassWithMembers
	{
		public readonly int Field = 1;
		public int Property { get; set; }

		public int Method() => Field + Property;
	}

	private sealed class ThrowingAssembly(Type?[] loadableTypes) : Assembly
	{
		public override string FullName => nameof(ThrowingAssembly);

		public override Type[] GetTypes()
			=> throw new ReflectionTypeLoadException(loadableTypes, null);
	}
}
