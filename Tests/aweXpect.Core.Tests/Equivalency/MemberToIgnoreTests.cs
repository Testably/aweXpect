using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class MemberToIgnoreTests
{
	[Test]
	[Arguments("Name", "Name")]
	[Arguments("Name", "Child.Name")]
	[Arguments("Name", "Items[3].Name")]
	[Arguments("nAmE", "Child.Name")]
	[Arguments("Child.Name", "Root.Child.Name")]
	[Arguments("Items[3]", "Root.Items[3]")]
	[Arguments("[3]", "Items[3]")]
	[Arguments("[3]", "Items[0][3]")]
	[Arguments("[a.b]", "Root[a.b]")]
	[Arguments("[a[b]", "Root[a[b]")]
	[Arguments("[c]", "Root[a.b][c]")]
	[Arguments("[c]", "Root[a[b][c]")]
	[Arguments("Name", "Root[a.b].Name")]
	[Arguments("Name", "Root[a[b].Name")]
	[Arguments("[0]", "[0]")]
	[Arguments("[1]", "[0][1]")]
	[Arguments("[0][1]", "Items[0][1]")]
	[Arguments("Name", "[0].Name")]
	[Arguments("Name", "Items[0][1].Name")]
	[Arguments("[key]", "Root[key]")]
	[Arguments("Name", "Root[key].Name")]
	[Arguments("Name", "Root[a]b].Name")]
	public async Task ByName_WhenTheNameCoversWholePathSegments_ShouldIgnoreTheMember(
		string memberName, string memberPath)
	{
		MemberToIgnore.ByName sut = new(memberName);

		bool result = sut.IgnoreMember(memberPath, typeof(string));

		await That(result).IsTrue()
			.Because("a name that covers whole path segments matches the member at any depth");
	}

	[Test]
	[Arguments("ame", "Name")]
	[Arguments("ame", "Child.Name")]
	[Arguments("d.Name", "Child.Name")]
	[Arguments("hild.Name", "Child.Name")]
	[Arguments("3]", "Items[3]")]
	[Arguments("Name", "Items[Name]")]
	[Arguments("b]", "Root[a.b]")]
	[Arguments("[b]", "Root[a[b]")]
	[Arguments("Items[3]", "Root[x.Items[3]")]
	[Arguments(".Name", "Items[3].Name")]
	[Arguments(".Name", "[3].Name")]
	[Arguments(".Name", "Child.Name")]
	[Arguments("b]", "Root[a]b]")]
	[Arguments("Name]", "Root[a]Name]")]
	[Arguments("", "Name")]
	public async Task ByName_WhenTheNameDoesNotCoverWholePathSegments_ShouldNotIgnoreTheMember(
		string memberName, string memberPath)
	{
		MemberToIgnore.ByName sut = new(memberName);

		bool result = sut.IgnoreMember(memberPath, typeof(string));

		await That(result).IsFalse()
			.Because("neither an abbreviated or misspelled name nor a separator inside a dictionary key must silently widen the exclusion");
	}
}
