using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class MemberToIgnoreTests
{
	[Theory]
	[InlineData("Name", "Name")]
	[InlineData("Name", "Child.Name")]
	[InlineData("Name", "Items[3].Name")]
	[InlineData("nAmE", "Child.Name")]
	[InlineData("Child.Name", "Root.Child.Name")]
	[InlineData("Items[3]", "Root.Items[3]")]
	[InlineData("[3]", "Items[3]")]
	[InlineData("[3]", "Items[0][3]")]
	public async Task ByName_WhenTheNameCoversWholePathSegments_ShouldIgnoreTheMember(
		string memberName, string memberPath)
	{
		MemberToIgnore.ByName sut = new(memberName);

		bool result = sut.IgnoreMember(memberPath, typeof(string));

		await That(result).IsTrue()
			.Because("a name that covers whole path segments matches the member at any depth");
	}

	[Theory]
	[InlineData("ame", "Name")]
	[InlineData("ame", "Child.Name")]
	[InlineData("d.Name", "Child.Name")]
	[InlineData("hild.Name", "Child.Name")]
	[InlineData("3]", "Items[3]")]
	[InlineData("Name", "Items[Name]")]
	[InlineData("", "Name")]
	public async Task ByName_WhenTheNameDoesNotCoverWholePathSegments_ShouldNotIgnoreTheMember(
		string memberName, string memberPath)
	{
		MemberToIgnore.ByName sut = new(memberName);

		bool result = sut.IgnoreMember(memberPath, typeof(string));

		await That(result).IsFalse()
			.Because("an abbreviated or misspelled name must not silently widen the exclusion");
	}
}
