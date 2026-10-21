using System.Reflection;
using System.Runtime.Versioning;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class AttributePredicatesTests
{
	private static readonly Type[] AttributeTypes =
	[
		typeof(InheritedAttribute), typeof(NotInheritedAttribute), typeof(MissingAttribute),
		typeof(TargetFrameworkAttribute),
	];

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfAssembly_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		Assembly assembly = typeof(AttributePredicatesTests).Assembly;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> assembly.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfConstructor_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		ConstructorInfo constructor = typeof(BaseClass).GetConstructor(Type.EmptyTypes)!;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> constructor.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfEvent_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		EventInfo baseEvent = typeof(BaseClass).GetEvent(nameof(BaseClass.Played))!;
		EventInfo overriddenEvent = typeof(DerivedClass).GetEvent(nameof(DerivedClass.Played))!;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> baseEvent.HasAttribute(attributeType, predicate, inherit));
		await ThatBothLookupsAgree((attributeType, predicate)
			=> overriddenEvent.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfField_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		FieldInfo inheritedField = typeof(DerivedClass).GetField(nameof(BaseClass.Track))!;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> inheritedField.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfMethod_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		MethodInfo baseMethod = typeof(BaseClass).GetMethod(nameof(BaseClass.Play))!;
		MethodInfo overriddenMethod = typeof(DerivedClass).GetMethod(nameof(DerivedClass.Play))!;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> baseMethod.HasAttribute(attributeType, predicate, inherit));
		await ThatBothLookupsAgree((attributeType, predicate)
			=> overriddenMethod.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfProperty_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		PropertyInfo baseProperty = typeof(BaseClass).GetProperty(nameof(BaseClass.Duration))!;
		PropertyInfo overriddenProperty = typeof(DerivedClass).GetProperty(nameof(DerivedClass.Duration))!;

		await ThatBothLookupsAgree((attributeType, predicate)
			=> baseProperty.HasAttribute(attributeType, predicate, inherit));
		await ThatBothLookupsAgree((attributeType, predicate)
			=> overriddenProperty.HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task HasAttribute_OfType_WithoutPredicate_ShouldMatchTheLookupOfTheAttributes(bool inherit)
	{
		await ThatBothLookupsAgree((attributeType, predicate)
			=> typeof(BaseClass).HasAttribute(attributeType, predicate, inherit));
		await ThatBothLookupsAgree((attributeType, predicate)
			=> typeof(DerivedClass).HasAttribute(attributeType, predicate, inherit));
	}

	[Test]
	public async Task IsOfType_ShouldOnlyMatchAttributesOfTheType()
	{
		Func<Attribute, bool> predicate = AttributePredicates.IsOfType<InheritedAttribute>();

		await That(predicate(new InheritedAttribute())).IsTrue();
		await That(predicate(new NotInheritedAttribute())).IsFalse();
	}

	[Test]
	public async Task IsOfType_ShouldReturnTheSameDelegateOnEveryCall()
	{
		Func<Attribute, bool> predicate = AttributePredicates.IsOfType<InheritedAttribute>();

		await That(AttributePredicates.IsOfType<InheritedAttribute>()).IsSameAs(predicate);
	}

	[Test]
	public async Task OnlyChecksType_WhenPredicateChecksTheType_ShouldReturnTrue()
	{
		bool result = AttributePredicates.OnlyChecksType(AttributePredicates.IsOfType<InheritedAttribute>());

		await That(result).IsTrue();
	}

	[Test]
	public async Task OnlyChecksType_WhenPredicateIsCustom_ShouldReturnFalse()
	{
		bool result = AttributePredicates.OnlyChecksType(a => a is InheritedAttribute);

		await That(result).IsFalse();
	}

	[Test]
	public async Task OnlyChecksType_WhenPredicateIsNull_ShouldReturnTrue()
	{
		bool result = AttributePredicates.OnlyChecksType(null);

		await That(result).IsTrue();
	}

	private static async Task ThatBothLookupsAgree(Func<Type, Func<Attribute, bool>?, bool> hasAttribute)
	{
		foreach (Type attributeType in AttributeTypes)
		{
			await That(hasAttribute(attributeType, null)).IsEqualTo(hasAttribute(attributeType, _ => true))
				.Because($"`IsDefined` must find {attributeType.Name} exactly when the attributes contain it");
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = true)]
	private sealed class InheritedAttribute : Attribute;

	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	private sealed class NotInheritedAttribute : Attribute;

	[AttributeUsage(AttributeTargets.All)]
	private sealed class MissingAttribute : Attribute;

	[Inherited]
	[NotInherited]
	private class BaseClass
	{
		[Inherited] [NotInherited] public string Track = "";

		[Inherited]
		[NotInherited]
		public BaseClass()
		{
		}

		[Inherited]
		[NotInherited]
		public virtual int Duration { get; set; }

		[Inherited]
		[NotInherited]
		public virtual event EventHandler? Played
		{
			add { }
			remove { }
		}

		[Inherited]
		[NotInherited]
		public virtual void Play()
		{
		}
	}

	private sealed class DerivedClass : BaseClass
	{
		public override int Duration { get; set; }

		public override event EventHandler? Played
		{
			add { }
			remove { }
		}

		public override void Play()
		{
		}
	}
}
