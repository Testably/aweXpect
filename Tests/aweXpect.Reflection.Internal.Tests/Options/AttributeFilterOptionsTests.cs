using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Internal.Tests.Options;

public sealed class AttributeFilterOptionsTests
{
	[Test]
	public async Task Matches_WhenTheScopeIsDirect_ShouldIgnoreInheritedAttributes()
	{
		AttributeFilterOptions<Type?> sut = CreateSut().WithAttribute<MyAttribute>();

		bool inherited = sut.Matches(typeof(DerivedClass), MemberScopes.None);
		bool direct = sut.Matches(typeof(DerivedClass), MemberScopes.DeclaredOnly);

		await That(inherited).IsTrue();
		await That(direct).IsFalse()
			.Because("the scope is passed when the options are evaluated, not stored in them");
	}

	[Test]
	public async Task WithAttribute_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		AttributeFilterOptions<Type?> sut = CreateSut().WithAttribute<ObsoleteAttribute>();

		AttributeFilterOptions<Type?> result = sut.WithAttribute<SerializableAttribute>();

		await That(Describe(sut)).IsEqualTo("has ObsoleteAttribute");
		await That(Describe(result)).IsEqualTo("has ObsoleteAttribute or SerializableAttribute");
	}

	private static AttributeFilterOptions<Type?> CreateSut()
		=> new((type, attributeType, predicate, inherit) => type.HasAttribute(attributeType, predicate, inherit));

	private static string Describe(AttributeFilterOptions<Type?> options)
	{
		StringBuilder sb = new();
		options.AppendExpectation(sb, ExpectationGrammars.None, MemberScopes.None);
		return sb.ToString();
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	private sealed class MyAttribute : Attribute;

	[My]
	private class BaseClass;

	private sealed class DerivedClass : BaseClass;
}
