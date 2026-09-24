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
	[InlineData("[a.b]", "Root[a.b]")]
	[InlineData("[a[b]", "Root[a[b]")]
	[InlineData("[c]", "Root[a.b][c]")]
	[InlineData("[c]", "Root[a[b][c]")]
	[InlineData("Name", "Root[a.b].Name")]
	[InlineData("Name", "Root[a[b].Name")]
	[InlineData("[0]", "[0]")]
	[InlineData("[1]", "[0][1]")]
	[InlineData("[0][1]", "Items[0][1]")]
	[InlineData("Name", "[0].Name")]
	[InlineData("Name", "Items[0][1].Name")]
	[InlineData("[key]", "Root[key]")]
	[InlineData("Name", "Root[key].Name")]
	[InlineData("Name", "Root[a]b].Name")]
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
	[InlineData("b]", "Root[a.b]")]
	[InlineData("[b]", "Root[a[b]")]
	[InlineData("Items[3]", "Root[x.Items[3]")]
	[InlineData(".Name", "Items[3].Name")]
	[InlineData(".Name", "[3].Name")]
	[InlineData(".Name", "Child.Name")]
	[InlineData("b]", "Root[a]b]")]
	[InlineData("Name]", "Root[a]Name]")]
	[InlineData("", "Name")]
	public async Task ByName_WhenTheNameDoesNotCoverWholePathSegments_ShouldNotIgnoreTheMember(
		string memberName, string memberPath)
	{
		MemberToIgnore.ByName sut = new(memberName);

		bool result = sut.IgnoreMember(memberPath, typeof(string));

		await That(result).IsFalse()
			.Because("neither an abbreviated or misspelled name nor a separator inside a dictionary key must silently widen the exclusion");
	}
}
