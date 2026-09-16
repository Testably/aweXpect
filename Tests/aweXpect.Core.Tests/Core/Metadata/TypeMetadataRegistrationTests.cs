#if NET8_0_OR_GREATER
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.Core.Metadata;

[assembly: GenerateMetadata(typeof(TypeMetadataRegistrationTests.RegisteredByTheGenerator))]

namespace aweXpect.Core.Tests.Core.Metadata;

public sealed class TypeMetadataRegistrationTests
{
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

	public sealed class RegisteredByTheGenerator
	{
		public int Number;
		public string Name { get; set; } = "";
	}
}
#endif
