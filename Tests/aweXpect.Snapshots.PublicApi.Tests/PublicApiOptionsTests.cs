using aweXpect.Customization;
using aweXpect.Snapshots.PublicApi.Fixture;
using aweXpect.Snapshots.PublicApi.Tests.TestHelpers;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed class PublicApiOptionsTests
{
	[Test]
	[Arguments(null, "The 'fullName' cannot be null.*")]
	[Arguments("", "The 'fullName' cannot be empty.*")]
	public async Task IgnoringAttribute_WhenFullNameIsNullOrEmpty_ShouldThrow(string? fullName, string expectedMessage)
	{
		void Act()
			=> new PublicApiOptions().Ignoring.Attribute(fullName!);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("fullName").And
			.WithMessage(expectedMessage).AsWildcard()
			.Because("an empty name never matches an attribute, so it would be ignored silently");
	}

	[Test]
	[Arguments(null, "The 'fullName' cannot be null.*")]
	[Arguments("", "The 'fullName' cannot be empty.*")]
	public async Task IgnoringType_WhenFullNameIsNullOrEmpty_ShouldThrow(string? fullName, string expectedMessage)
	{
		void Act()
			=> new PublicApiOptions().Ignoring.Type(fullName!);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("fullName").And
			.WithMessage(expectedMessage).AsWildcard()
			.Because("an empty name never matches a type, so it would be ignored silently");
	}

	[Test]
	[Arguments(null, "The 'fullName' cannot be null.*")]
	[Arguments("", "The 'fullName' cannot be empty.*")]
	public async Task WithinType_WhenFullNameIsNullOrEmpty_ShouldThrow(string? fullName, string expectedMessage)
	{
		void Act()
			=> new PublicApiOptions().Within.Type(fullName!);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("fullName").And
			.WithMessage(expectedMessage).AsWildcard()
			.Because("an empty name never matches a type, so the public API would be empty");
	}

	[Test]
	public async Task IgnoringNamespace_WhenNamespaceIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> new PublicApiOptions().Ignoring.Namespace(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("namespace").And
			.WithMessage("The 'namespace' cannot be null.*").AsWildcard();
	}

	[Test]
	public async Task WithinNamespace_WhenNamespaceIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> new PublicApiOptions().Within.Namespace(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("namespace").And
			.WithMessage("The 'namespace' cannot be null.*").AsWildcard();
	}

	[Test]
	public async Task WithinNamespace_WhenNamespaceIsEmpty_ShouldOnlyWriteTheGlobalNamespace()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o => o.Within.Namespace(""));

		await That(result).Contains("public class GlobalType").And.DoesNotContain("namespace ")
			.Because("an empty namespace is the global namespace");
	}

	[Test]
	public async Task IgnoringNamespace_WhenNamespaceIsEmpty_ShouldIgnoreTheGlobalNamespace()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o => o.Ignoring.Namespace(""));

		await That(result).DoesNotContain("GlobalType").And.Contains("class Album")
			.Because("an empty namespace is the global namespace");
	}

	[Test]
	public async Task Ignoring_WhenArgumentIsInvalid_ShouldTraceTheException()
	{
		RecordingTraceWriter traceWriter = new();
		using CustomizationLifetime _ = Customize.aweXpect.EnableTracing(traceWriter);

		Exception? exception = Catch.Exception(() => new PublicApiOptions().Ignoring.Type(""));

		await That(exception).IsNotNull();
		await That(traceWriter.Exceptions).HasSingle().Which.IsSameAs(exception);
	}

	[Test]
	public async Task WithinType_WhenTypeIsNested_ShouldTraceTheException()
	{
		RecordingTraceWriter traceWriter = new();
		using CustomizationLifetime _ = Customize.aweXpect.EnableTracing(traceWriter);

		Exception? exception = Catch.Exception(() => new PublicApiOptions().Within.Type<Album.Track>());

		await That(exception).IsNotNull();
		await That(traceWriter.Exceptions).HasSingle().Which.IsSameAs(exception);
	}
}
