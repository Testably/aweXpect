#if NET8_0_OR_GREATER
using System.Linq;
using System.Text;
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.Core.Metadata;
using aweXpect.Equivalency;

[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.RegisteredByTheGenerator))]
[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.ImplementingExplicitly))]
[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.HidingWithNonPublicGetter))]
[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.OverridingOnlyTheSetter))]
[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.WithRefProperty))]

namespace aweXpect.Core.Tests.Core.Metadata;

public sealed class TypeMetadataRegistrationTests
{
#if DEBUG
	[Test]
	public async Task AnonymousTypeWithAGenericMemberOverAnAnonymousType_ShouldBeRegisteredByTheGenerator()
	{
		var items = new[] { 1, 2, }.Select(i => new
		{
			Id = i,
		});
		var subject = new
		{
			Items = items.ToList(),
			Map = items.ToDictionary(x => x.Id),
		};

		await That(subject).IsEquivalentTo(subject);

		await That(TypeMetadataRegistry.Instance.TryGet(subject.GetType(), out _)).IsTrue()
			.Because("the generated probe has to unify with the anonymous type of the call site");
	}

	[Test]
	public async Task GenerateMetadataAttribute_ShouldRegisterExplicitImplementations()
	{
		TypeMetadataRegistry.Instance.TryGet(typeof(ImplementingExplicitly),
			out TypeMetadataRegistry.TypeMetadata? metadata);

		await That(metadata!.Properties.Keys).IsEqualTo(["Own",]);
		await That(metadata.ExplicitProperties.Keys)
			.IsEqualTo(["aweXpect.Core.Tests.Core.Metadata.TypeMetadataRegistrationTests.IHasValue.Value",])
			.Because("the generator registers the explicit implementation under the name reflection reports for it");
	}
#endif

	[Test]
	public async Task GenerateMetadataAttribute_ShouldRegisterTheTypeBeforeTheTestsRun()
	{
		bool isRegistered = TypeMetadataRegistry.Instance.TryGet(typeof(RegisteredByTheGenerator),
			out TypeMetadataRegistry.TypeMetadata? metadata);

		await That(isRegistered).IsTrue()
			.Because("the generator emits a module initializer for the type named in the assembly attribute");
		await That(metadata!.Fields.Keys).IsEqualTo(["Number",]);
		await That(metadata.Properties.Keys).IsEqualTo(["Name",]);
	}

	[Test]
	public async Task RegisteredAccessor_ShouldReadTheMember()
	{
		TypeMetadataRegistry.Instance.TryGet(typeof(RegisteredByTheGenerator),
			out TypeMetadataRegistry.TypeMetadata? metadata);
		RegisteredByTheGenerator subject = new()
		{
			Number = 3,
			Name = "foo",
		};

		await That(metadata!.Fields["Number"].GetValue(subject)).IsEqualTo(3);
		await That(metadata.Properties["Name"].GetValue(subject)).IsEqualTo("foo");
		await That(metadata.Properties["Name"].MemberType).IsEqualTo(typeof(string))
			.Because("the declared member type feeds the type-based ignore rules");
	}

#if DEBUG
	[Test]
	public async Task RegisteredExplicitImplementation_ShouldCompareLikeReflection()
	{
		ImplementingExplicitly subject = new()
		{
			Own = 1,
		};
		var expected = new
		{
			Own = 1,
			Value = 6,
		};
		StringBuilder registered = new();
		StringBuilder reflected = new();

		bool viaRegistry = await EquivalencyComparison.Compare(subject, expected, new EquivalencyOptions(), registered);
		bool viaReflection = await EquivalencyComparison.Compare(subject, expected,
			new EquivalencyOptions
			{
				Properties = IncludeMembers.Public | IncludeMembers.Internal,
			}, reflected);

		await That(viaRegistry).IsFalse();
		await That(viaReflection).IsFalse();
		await That(registered.ToString()).IsEqualTo(reflected.ToString())
			.Because("a request for non-public members reflects, and the registry has to report the same difference");
		await That(registered.ToString()).Contains("Property Value differed");
	}

	[Test]
	public async Task RegisteredHidingPropertyWithNonPublicGetter_ShouldCompareLikeReflection()
	{
		HidingWithNonPublicGetter subject = new()
		{
			Own = 1,
		};
		((WithValue)subject).Value = 1;
		HidingWithNonPublicGetter expected = new()
		{
			Own = 1,
		};
		((WithValue)expected).Value = 2;

		(bool viaRegistry, string registered, bool viaReflection, string reflected) =
			await CompareOnBothPaths(subject, expected, new EquivalencyOptions());

		await That(viaRegistry).IsEqualTo(viaReflection);
		await That(registered).IsEqualTo(reflected);
		await That(viaRegistry).IsTrue()
			.Because("the public setter makes the hiding declaration visible, and it hides the base property, which is not compared");
	}

	[Test]
	public async Task RegisteredRefProperty_ShouldCompareLikeReflection()
	{
		WithRefProperty subject = new(1);
		WithRefProperty expected = new(2);
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByPredicate((_, type) => type == typeof(int), "int"),],
		};

		(bool viaRegistry, string registered, bool viaReflection, string reflected) =
			await CompareOnBothPaths(subject, expected, options);

		await That(viaRegistry).IsEqualTo(viaReflection);
		await That(registered).IsEqualTo(reflected);
		await That(viaRegistry).IsTrue()
			.Because("both paths declare a ref-returning property with the type it refers to, so ignoring that type ignores it");
	}

	[Test]
	public async Task RegisteredSetterOnlyOverride_ShouldCompareLikeReflection()
	{
		OverridingOnlyTheSetter subject = new()
		{
			Own = 1,
			Value = 1,
		};
		OverridingOnlyTheSetter expected = new()
		{
			Own = 1,
			Value = 2,
		};

		(bool viaRegistry, string registered, bool viaReflection, string reflected) =
			await CompareOnBothPaths(subject, expected, new EquivalencyOptions());

		await That(viaRegistry).IsEqualTo(viaReflection);
		await That(registered).IsEqualTo(reflected);
		await That(registered).Contains("Property Value differed")
			.Because("the override inherits the getter, so the property stays readable on both paths");
	}

	/// <remarks>
	///     A request for non-public members bypasses the registry, so the second comparison reflects over the type.
	/// </remarks>
	private static async Task<(bool, string, bool, string)> CompareOnBothPaths<T>(T subject, T expected,
		EquivalencyOptions options)
		where T : notnull
	{
		StringBuilder registered = new();
		StringBuilder reflected = new();
		bool viaRegistry = await EquivalencyComparison.Compare(subject, expected, options, registered);
		bool viaReflection = await EquivalencyComparison.Compare(subject, expected, options with
		{
			Properties = IncludeMembers.Public | IncludeMembers.Internal,
		}, reflected);
		return (viaRegistry, registered.ToString(), viaReflection, reflected.ToString());
	}
#endif

	public sealed class HidingWithNonPublicGetter : WithValue
	{
		public int Own { get; set; }
		public new string Value { private get; set; } = "";

		public override string ToString() => Value;
	}

	public interface IHasValue
	{
		int Value { get; }
	}

	public sealed class ImplementingExplicitly : IHasValue
	{
		public int Own { get; set; }
		int IHasValue.Value => 5;
	}

	public sealed class OverridingOnlyTheSetter : WithVirtualValue
	{
		public int Own { get; set; }

		public override int Value
		{
			set => base.Value = value;
		}
	}

	public sealed class RegisteredByTheGenerator
	{
		public int Number;
		public string Name { get; set; } = "";
	}

	public sealed class WithRefProperty(int value)
	{
		private int _value = value;
		public string Own { get; set; } = "";
		public ref int Value => ref _value;
	}

	public class WithValue
	{
		public int Value { get; set; }
	}

	public class WithVirtualValue
	{
		public virtual int Value { get; set; }
	}
}
#endif
