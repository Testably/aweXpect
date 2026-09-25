#if NET8_0_OR_GREATER
using System.Text;
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.Core.Metadata;
using aweXpect.Equivalency;

[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.RegisteredByTheGenerator))]
[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.ImplementingExplicitly))]

namespace aweXpect.Core.Tests.Core.Metadata;

public sealed class TypeMetadataRegistrationTests
{
#if DEBUG
	[Fact]
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

	[Fact]
	public async Task GenerateMetadataAttribute_ShouldRegisterTheTypeBeforeTheTestsRun()
	{
		bool isRegistered = TypeMetadataRegistry.Instance.TryGet(typeof(RegisteredByTheGenerator),
			out TypeMetadataRegistry.TypeMetadata? metadata);

		await That(isRegistered).IsTrue()
			.Because("the generator emits a module initializer for the type named in the assembly attribute");
		await That(metadata!.Fields.Keys).IsEqualTo(["Number",]);
		await That(metadata.Properties.Keys).IsEqualTo(["Name",]);
	}

	[Fact]
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
	[Fact]
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
#endif

	public interface IHasValue
	{
		int Value { get; }
	}

	public sealed class ImplementingExplicitly : IHasValue
	{
		public int Own { get; set; }
		int IHasValue.Value => 5;
	}

	public sealed class RegisteredByTheGenerator
	{
		public int Number;
		public string Name { get; set; } = "";
	}
}
#endif
