using aweXpect.Customization;
using aweXpect.Snapshots.PublicApi.Fixture;
using aweXpect.Snapshots.PublicApi.Options;
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
	public async Task Options_ShouldNotChangeTheOptionsTheyAreCalledOn()
	{
		string expected = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o => o);

		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o =>
		{
			_ = o.Ignoring.Attribute<ObsoleteAttribute>();
			_ = o.Ignoring.CompilerGeneratedMembers();
			_ = o.Ignoring.ForwardedTypes();
			_ = o.Ignoring.Namespace("");
			_ = o.Ignoring.Type<Album>();
			_ = o.Within.Namespace("Nowhere");
			_ = o.Within.Type<Album>();
			_ = o.Formatting.ParametersOnSeparateLines();
			return o;
		});

		await That(result).IsEqualTo(expected)
			.Because("each option returns new options instead of changing the ones it is called on");
	}

	[Test]
	public async Task StoredOptions_ShouldNotBeChangedByLaterOptions()
	{
		PublicApiOptions withinAlbum = new PublicApiOptions().Within.Type<Album>();
		string expected = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, _ => withinAlbum);

		PublicApiOptions ignoringAlbum = withinAlbum.Ignoring.Type<Album>();
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, _ => withinAlbum);

		await That(result).IsEqualTo(expected).And.Contains("class Album")
			.Because("options derived from stored options must not change them");
		await That(PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, _ => ignoringAlbum))
			.DoesNotContain("class Album");
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
