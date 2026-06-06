using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class NullabilityHelpersTests
{
	[Fact]
	public async Task GetNotNullableMembers_ShouldReturnNonNullableFieldsPropertiesAndEvents()
	{
		MemberInfo[] notNullableMembers = typeof(NullabilityTestClass).GetNotNullableMembers();

		await That(notNullableMembers.Select(member => member.Name)).IsEqualTo([
			nameof(NullabilityTestClass.NonNullableValueField),
			nameof(NullabilityTestClass.NonNullableReferenceField),
			nameof(NullabilityTestClass.NonNullableGenericField),
			nameof(NullabilityTestClass.NonNullableValueProperty),
			nameof(NullabilityTestClass.NonNullableReferenceProperty),
			nameof(NullabilityTestClass.NonNullableGenericProperty),
			nameof(NullabilityTestClass.NonNullableEvent),
			nameof(NullabilityTestClass.NonNullableGenericEvent),
		]).InAnyOrder();
	}

	[Fact]
	public async Task GetNullableMembers_ShouldReturnNullableFieldsPropertiesAndEvents()
	{
		MemberInfo[] nullableMembers = typeof(NullabilityTestClass).GetNullableMembers();

		await That(nullableMembers.Select(member => member.Name)).IsEqualTo([
			nameof(NullabilityTestClass.NullableValueField),
			nameof(NullabilityTestClass.NullableReferenceField),
			nameof(NullabilityTestClass.NullableGenericField),
			nameof(NullabilityTestClass.NullableValueProperty),
			nameof(NullabilityTestClass.NullableReferenceProperty),
			nameof(NullabilityTestClass.NullableGenericProperty),
			nameof(NullabilityTestClass.NullableWriteOnlyProperty),
			nameof(NullabilityTestClass.NullableEvent),
			nameof(NullabilityTestClass.NullableGenericEvent),
		]).InAnyOrder();
	}

	[Theory]
	[InlineData(nameof(NullabilityTestClass.NullableValueField), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableValueField), false)]
	[InlineData(nameof(NullabilityTestClass.NullableReferenceField), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableReferenceField), false)]
	[InlineData(nameof(NullabilityTestClass.NullableGenericField), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableGenericField), false)]
	public async Task IsNullable_ShouldEvaluateFieldNullability(string fieldName, bool expectNullable)
	{
		FieldInfo fieldInfo = typeof(NullabilityTestClass).GetField(fieldName)!;

		await That(fieldInfo.IsNullable()).IsEqualTo(expectNullable);
	}

	[Theory]
	[InlineData(nameof(NullabilityTestClass.NullableEvent), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableEvent), false)]
	[InlineData(nameof(NullabilityTestClass.NullableGenericEvent), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableGenericEvent), false)]
	public async Task IsNullable_ShouldEvaluateEventNullability(string eventName, bool expectNullable)
	{
		EventInfo eventInfo = typeof(NullabilityTestClass).GetEvent(eventName)!;

		await That(eventInfo.IsNullable()).IsEqualTo(expectNullable);
	}

	[Theory]
	[InlineData(nameof(NullabilityTestClass.NullableValueProperty), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableValueProperty), false)]
	[InlineData(nameof(NullabilityTestClass.NullableReferenceProperty), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableReferenceProperty), false)]
	[InlineData(nameof(NullabilityTestClass.NullableGenericProperty), true)]
	[InlineData(nameof(NullabilityTestClass.NonNullableGenericProperty), false)]
	[InlineData(nameof(NullabilityTestClass.NullableWriteOnlyProperty), true)]
	public async Task IsNullable_ShouldEvaluatePropertyNullability(string propertyName, bool expectNullable)
	{
		PropertyInfo propertyInfo = typeof(NullabilityTestClass).GetProperty(propertyName)!;

		await That(propertyInfo.IsNullable()).IsEqualTo(expectNullable);
	}

	[Theory]
	[InlineData(nameof(MostlyNullableTestClass.NonNullableField), false)]
	[InlineData(nameof(MostlyNullableTestClass.NonNullableProperty), false)]
	[InlineData(nameof(MostlyNullableTestClass.FirstNullableField), true)]
	public async Task IsNullable_WhenMemberDiffersFromContextOfDeclaringType_ShouldEvaluateNullability(
		string memberName, bool expectNullable)
	{
		MemberInfo memberInfo = typeof(MostlyNullableTestClass).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsEqualTo(expectNullable);
	}

	[Fact]
	public async Task IsNullable_WhenEventIsOblivious_ShouldReturnFalse()
	{
		EventInfo eventInfo = typeof(ObliviousTestClass).GetEvent(nameof(ObliviousTestClass.ObliviousEvent))!;

		await That(eventInfo.IsNullable()).IsFalse();
	}

	[Theory]
	[InlineData(nameof(ObliviousTestClass.ObliviousField))]
	[InlineData(nameof(ObliviousTestClass.ObliviousProperty))]
	public async Task IsNullable_WhenMemberIsOblivious_ShouldReturnFalse(string memberName)
	{
		MemberInfo memberInfo = typeof(ObliviousTestClass).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsFalse();
	}

	[Theory]
	[InlineData(nameof(ObliviousTestClass.NestedObliviousTestClass.NestedObliviousField))]
	[InlineData(nameof(ObliviousTestClass.NestedObliviousTestClass.NestedObliviousProperty))]
	public async Task IsNullable_WhenMemberIsObliviousInNestedClass_ShouldReturnFalse(string memberName)
	{
		MemberInfo memberInfo = typeof(ObliviousTestClass.NestedObliviousTestClass).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsFalse();
	}

	[Theory]
	[InlineData(nameof(AllNullableTestClass.FirstNullableField))]
	[InlineData(nameof(AllNullableTestClass.SecondNullableField))]
	[InlineData(nameof(AllNullableTestClass.FirstNullableProperty))]
	public async Task IsNullable_WhenNullabilityIsStoredInContextOfDeclaringType_ShouldReturnTrue(string memberName)
	{
		MemberInfo memberInfo = typeof(AllNullableTestClass).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsTrue();
	}

	[Theory]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.NullableArrayField), true)]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.ArrayOfNullableField), false)]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.NullableArrayProperty), true)]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.ArrayOfNullableProperty), false)]
	public async Task IsNullable_WithArrayMembers_ShouldOnlyConsiderTheTopLevelAnnotation(
		string memberName, bool expectNullable)
	{
		MemberInfo memberInfo = typeof(NullabilityEdgeCaseTestClass).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsEqualTo(expectNullable);
	}

	[Theory]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedField))]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedProperty))]
	public async Task IsNullable_WithGenericParameterMembers_WhenAccessedViaConstructedType_ShouldReturnFalse(
		string memberName)
	{
		// The nullable annotation of a type argument is not part of System.Type, so it can only be
		// resolved through the base type metadata of a derived type.
		MemberInfo memberInfo = typeof(GenericTestClass<string>).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsFalse();
	}

	[Theory]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedField), false)]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedField), true)]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedProperty), false)]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedProperty), true)]
	public async Task IsNullable_WithGenericParameterMembers_WhenDerivedTypeUsesNonNullableArgument_ShouldEvaluateAnnotation(
		string memberName, bool expectNullable)
	{
		MemberInfo memberInfo =
			typeof(DerivedFromGenericTestClassWithNonNullableArgument).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsEqualTo(expectNullable);
	}

	[Theory]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedField))]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedField))]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedProperty))]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedProperty))]
	public async Task IsNullable_WithGenericParameterMembers_WhenDerivedTypeUsesNullableArgument_ShouldReturnTrue(
		string memberName)
	{
		MemberInfo memberInfo = typeof(DerivedFromGenericTestClassWithNullableArgument).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsTrue();
	}

	[Theory]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedField), false)]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedField), true)]
	[InlineData(nameof(GenericTestClass<object>.UnannotatedProperty), false)]
	[InlineData(nameof(GenericTestClass<object>.AnnotatedProperty), true)]
	public async Task IsNullable_WithGenericTypeParameterMembers_ShouldOnlyConsiderTheAnnotation(
		string memberName, bool expectNullable)
	{
		MemberInfo memberInfo = typeof(GenericTestClass<>).GetMember(memberName).Single();

		bool result = memberInfo is FieldInfo fieldInfo
			? fieldInfo.IsNullable()
			: ((PropertyInfo)memberInfo).IsNullable();

		await That(result).IsEqualTo(expectNullable);
	}

	[Fact]
	public async Task IsNullable_WithNullableIndexer_ShouldReturnTrue()
	{
		PropertyInfo propertyInfo = typeof(NullabilityEdgeCaseTestClass).GetProperty("Item")!;

		await That(propertyInfo.IsNullable()).IsTrue();
	}

	[Fact]
	public async Task IsNullable_WithNonNullableEventInConstructedGenericType_ShouldReturnFalse()
	{
		// Events cannot be declared as a bare generic type parameter, so resolving the member type
		// through the generic type definition always yields the (non generic-parameter) handler type.
		EventInfo eventInfo = typeof(GenericTestClass<string>)
			.GetEvent(nameof(GenericTestClass<string>.UnannotatedEvent))!;

		await That(eventInfo.IsNullable()).IsFalse();
	}

	[Fact]
	public async Task IsNullable_WithNullEventInfo_ShouldReturnFalse()
	{
		EventInfo? eventInfo = null;

		await That(eventInfo.IsNullable()).IsFalse();
	}

	[Fact]
	public async Task IsNullable_WithNullFieldInfo_ShouldReturnFalse()
	{
		FieldInfo? fieldInfo = null;

		await That(fieldInfo.IsNullable()).IsFalse();
	}

	[Fact]
	public async Task IsNullable_WithNullPropertyInfo_ShouldReturnFalse()
	{
		PropertyInfo? propertyInfo = null;

		await That(propertyInfo.IsNullable()).IsFalse();
	}

	[Theory]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.AllowNullProperty))]
	[InlineData(nameof(NullabilityEdgeCaseTestClass.MaybeNullProperty))]
	public async Task IsNullable_WithPostConditionAttributes_ShouldIgnoreThemAndReturnFalse(string propertyName)
	{
		PropertyInfo propertyInfo = typeof(NullabilityEdgeCaseTestClass).GetProperty(propertyName)!;

		await That(propertyInfo.IsNullable()).IsFalse();
	}

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS0067 // Event is never used
	public class NullabilityTestClass
	{
		public List<string?> NonNullableGenericField = [];
		public string NonNullableReferenceField = "";
		public int NonNullableValueField;
		public List<string>? NullableGenericField;
		public string? NullableReferenceField;
		public int? NullableValueField;
		public event EventHandler? NullableEvent;
		public event EventHandler NonNullableEvent = delegate { };
		public event EventHandler<string>? NullableGenericEvent;
		public event EventHandler<string?> NonNullableGenericEvent = delegate { };
		public int? NullableValueProperty { get; set; }
		public int NonNullableValueProperty { get; set; }
		public string? NullableReferenceProperty { get; set; }
		public string NonNullableReferenceProperty { get; set; } = "";
		public List<string>? NullableGenericProperty { get; set; }
		public List<string?> NonNullableGenericProperty { get; set; } = [];

		public static string? NullableWriteOnlyProperty
		{
			// ReSharper disable once ValueParameterNotUsed
			set { }
		}
	}

	public class AllNullableTestClass
	{
		public string? FirstNullableField;
		public string? SecondNullableField;
		public string? FirstNullableProperty { get; set; }
		public string? SecondNullableProperty { get; set; }
	}

	public class NullabilityEdgeCaseTestClass
	{
		public string?[] ArrayOfNullableField = [];
		public string[]? NullableArrayField;
		public string[]? NullableArrayProperty { get; set; }
		public string?[] ArrayOfNullableProperty { get; set; } = [];

		[AllowNull] public string AllowNullProperty { get; set; } = "";

		[MaybeNull] public string MaybeNullProperty { get; set; } = "";

		public string? this[int index] => null;
	}

	// ReSharper disable once UnusedTypeParameter
	public class GenericTestClass<T>
	{
		public T? AnnotatedField;
		public T UnannotatedField = default!;
		public event EventHandler UnannotatedEvent = delegate { };
		public T UnannotatedProperty { get; set; } = default!;
		public T? AnnotatedProperty { get; set; }
	}

	public class DerivedFromGenericTestClassWithNullableArgument : GenericTestClass<string?>;

	public class DerivedFromGenericTestClassWithNonNullableArgument : GenericTestClass<string>;

	public class MostlyNullableTestClass
	{
		public string? FirstNullableField;
		public string NonNullableField = "";
		public string? SecondNullableField;
		public string? ThirdNullableField;
		public string? FirstNullableProperty { get; set; }
		public string? SecondNullableProperty { get; set; }
		public string? ThirdNullableProperty { get; set; }
		public string NonNullableProperty { get; set; } = "";
	}

#nullable disable
	public class ObliviousTestClass
	{
		public string ObliviousField;
		public string ObliviousProperty { get; set; }
		public event EventHandler ObliviousEvent;

		public class NestedObliviousTestClass
		{
			public string NestedObliviousField;
			public string NestedObliviousProperty { get; set; }
		}
	}
#pragma warning restore CS0067
#pragma warning restore CS0649
}
