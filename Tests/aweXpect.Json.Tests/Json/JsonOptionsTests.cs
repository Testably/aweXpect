using System.Text.Json;

namespace aweXpect.Json.Tests;

public class JsonOptionsTests
{
	[Fact]
	public async Task DocumentOptions_ShouldDefaultToAllowTrailingCommas()
	{
		JsonOptions sut = new();

		await That(sut.DocumentOptions.AllowTrailingCommas).IsTrue();
	}

	[Fact]
	public async Task WithJsonOptions_WhenCallbackIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.WithJsonOptions(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("jsonDocumentOptions").And
			.WithMessage("The 'jsonDocumentOptions' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task WithJsonOptions_ShouldSetDocumentOptions()
	{
		int maxDepth = new Random().Next(1, 10);
		JsonOptions sut = new();

		JsonOptions result = sut.WithJsonOptions(o => o with
		{
			MaxDepth = maxDepth,
		});

		await That(result.DocumentOptions.MaxDepth).IsEqualTo(maxDepth);
	}

	[Fact]
	public async Task WithJsonOptions_ShouldSupportProvidingFixedOptions()
	{
		int maxDepth = new Random().Next(1, 10);
		JsonOptions sut = new();
		JsonDocumentOptions documentOptions = new()
		{
			MaxDepth = maxDepth,
		};

		JsonOptions result = sut.WithJsonOptions(_ => documentOptions);

		await That(result.DocumentOptions.MaxDepth).IsEqualTo(maxDepth);
	}

	[Fact]
	public async Task MembersToIgnore_ShouldDefaultToEmpty()
	{
		JsonOptions sut = new();

		await That(sut.MembersToIgnore).IsEmpty();
	}

	[Fact]
	public async Task IgnoringMember_WhenMemberIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.IgnoringMember(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringMember_WhenMemberIsEmpty_ShouldThrowArgumentException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.IgnoringMember("");

		await That(Act).Throws<ArgumentException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be empty.").AsPrefix();
	}

	[Fact]
	public async Task Ignoring_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.Ignoring(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringMember_ShouldAccumulateTheMembersWithoutChangingTheOriginalOptions()
	{
		JsonOptions sut = new();

		JsonOptions result = sut.IgnoringMember("id").Ignoring(path => path.EndsWith(".etag"));

		await That(result.MembersToIgnore).HasCount(2);
		await That(result.MembersToIgnore[0].IgnoreMember("$.items[0].id", typeof(JsonElement))).IsTrue();
		await That(result.MembersToIgnore[1].IgnoreMember("$.items[0].etag", typeof(JsonElement))).IsTrue();
		await That(sut.MembersToIgnore).IsEmpty();
	}

	[Fact]
	public async Task Ignoring_ShouldDescribeThePredicateWithoutCommonWhiteSpace()
	{
		JsonOptions sut = new();

		JsonOptions result = sut.Ignoring(path
			=> path.EndsWith(".etag"));

		await That(result.MembersToIgnore[0].ToString()).IsEqualTo("""
		                                                          path
		                                                          => path.EndsWith(".etag")
		                                                          """);
	}
}
