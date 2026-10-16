using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyTypeOptionsExtensionsTests
{
	[Test]
	public async Task Comparing_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		EquivalencyOptions options = new();

		_ = options.Comparing<int, string>((number, text) => number.ToString() == text);

		await That(options.ToString()).IsEqualTo(" - include public fields and properties")
			.Because("the customized default is shared by every expectation");
	}

	[Test]
	public async Task Comparing_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> new EquivalencyOptions().Comparing<int, string>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Mapping_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		EquivalencyOptions options = new();

		_ = options.Mapping("Id", "Key");

		await That(options.ToString()).IsEqualTo(" - include public fields and properties")
			.Because("the customized default is shared by every expectation");
	}

	[Test]
	public async Task Mapping_WhenExpressionDoesNotSelectAMember_ShouldThrowArgumentException()
	{
		void Act()
			=> new EquivalencyOptions().Mapping<string, Version>(text => text.Trim(), version => version.Major);

		await That(Act).Throws<ArgumentException>()
			.WithParamName("actualMember").And
			.WithMessage(
				"The 'actualMember' must select a member of its parameter, e.g. x => x.Name or x => x.Artist.Name, but was text => text.Trim().")
			.AsPrefix();
	}

	[Test]
	[Arguments("", "Key", "actualMember", "")]
	[Arguments("Id", "", "expectedMember", "")]
	[Arguments("Artist..Name", "Key", "actualMember", "Artist..Name")]
	[Arguments("Id", "Tracks[0].Title", "expectedMember", "Tracks[0].Title")]
	public async Task Mapping_WhenMemberIsInvalid_ShouldThrowArgumentException(string actualMember,
		string expectedMember, string paramName, string member)
	{
		void Act()
			=> new EquivalencyOptions().Mapping(actualMember, expectedMember);

		await That(Act).Throws<ArgumentException>()
			.WithParamName(paramName).And
			.WithMessage(
				$"The '{paramName}' must be a member name or a dot-separated path of member names, e.g. \"Name\" or \"Artist.Name\", but was \"{member}\".")
			.AsPrefix();
	}

	[Test]
	public async Task Mapping_WhenMemberIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> new EquivalencyOptions().Mapping("Id", null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectedMember").And
			.WithMessage("The 'expectedMember' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ToString_WhenComparing_ShouldListTheTypesAndThePredicate()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.Comparing<int, string>((number, text) => number.ToString() == text);

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - compare int with string by (number, text) => number.ToString() == text
		                             """);
	}

	[Test]
	public async Task ToString_WhenComparingForAType_ShouldListItBelowTheType()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.For<Version>(o => o.Comparing<int, string>((number, text) => number.ToString() == text));

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - for Version:
		                                - include public fields and properties
		                                - compare int with string by (number, text) => number.ToString() == text
		                             """);
	}

	[Test]
	public async Task ToString_WhenMapping_ShouldListTheMembers()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.Mapping<Version, Version>(version => version.Major, version => version.Minor)
			.Mapping("Build", "Revision");

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - map Version.Major to Version.Minor
		                              - map "Build" to "Revision"
		                             """);
	}

	[Test]
	public async Task ToString_WhenMappingForAType_ShouldListItBelowTheType()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.For<Version>(o => o
				.Mapping<Version, Version>(version => version.Major, version => version.Minor)
				.Mapping("Build", "Revision"));

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - for Version:
		                                - include public fields and properties
		                                - map Version.Major to Version.Minor
		                                - map "Build" to "Revision"
		                             """);
	}
}
