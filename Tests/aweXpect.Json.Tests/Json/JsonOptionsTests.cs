using System.Text.Json;

namespace aweXpect.Json.Tests;

public class JsonOptionsTests
{
	[Test]
	public async Task DocumentOptions_ShouldDefaultToAllowTrailingCommas()
	{
		JsonOptions sut = new();

		await That(sut.DocumentOptions.AllowTrailingCommas).IsTrue();
	}

	[Test]
	public async Task WithJsonOptions_WhenCallbackIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.WithJsonOptions(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("jsonDocumentOptions").And
			.WithMessage("The 'jsonDocumentOptions' cannot be null.").AsPrefix();
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task MembersToIgnore_ShouldDefaultToEmpty()
	{
		JsonOptions sut = new();

		await That(sut.MembersToIgnore).IsEmpty();
	}

	[Test]
	public async Task IgnoringMember_WhenMemberIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.IgnoringMember(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task IgnoringMember_WhenMemberIsEmpty_ShouldThrowArgumentException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.IgnoringMember("");

		await That(Act).Throws<ArgumentException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be empty.").AsPrefix();
	}

	[Test]
	public async Task Ignoring_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.Ignoring(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task IgnoringMember_ShouldAccumulateTheMembersWithoutChangingTheOriginalOptions()
	{
		JsonOptions sut = new();

		JsonOptions result = sut.IgnoringMember("id").Ignoring(path => path.EndsWith(".etag"));

		await That(result.MembersToIgnore).HasCount(2);
		await That(result.MembersToIgnore[0].IgnoreMember("$.items[0].id", typeof(JsonElement))).IsTrue();
		await That(result.MembersToIgnore[1].IgnoreMember("$.items[0].etag", typeof(JsonElement))).IsTrue();
		await That(sut.MembersToIgnore).IsEmpty();
	}

	[Test]
	public async Task NumbersWithin_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.NumbersWithin(-0.1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task NumbersWithin_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.NumbersWithin(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix();
	}

	[Test]
	public async Task NumbersWithin_ShouldNotChangeTheOriginalOptions()
	{
		JsonElement subject = JsonDocument.Parse("1.5").RootElement;
		JsonOptions sut = new();

		_ = sut.NumbersWithin(1);

		async Task Act()
			=> await That(subject).MatchesExactly(1, _ => sut);

		await That(Act).Throws<FailException>()
			.Because("NumbersWithin returns new options instead of changing the original ones");
	}

	[Test]
	public async Task NumbersWithin_WhenCalledAgain_ShouldReplaceTheTolerance()
	{
		JsonElement subject = JsonDocument.Parse("1.5").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.NumbersWithin(1).IgnoringArrayOrder().NumbersWithin(0.1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches 1 exactly,
			             but it differed as $ was 1.5 instead of 1 ± 0.1

			             JSON options:
			              - ignore array order
			              - compare numbers within ± 0.1
			             """);
	}

	[Test]
	public async Task Percent_ShouldNotAffectCopiesMadeBefore()
	{
		JsonElement subject = JsonDocument.Parse("0.3").RootElement;
		JsonNumbersWithinOptions withinOptions = new JsonOptions().NumbersWithin(5);
		JsonOptions copyBeforePercent = withinOptions.IgnoringArrayOrder();

		JsonOptions percentOptions = withinOptions.Percent();

		async Task ActWithCopyBeforePercent()
			=> await That(subject).MatchesExactly(0.1, _ => copyBeforePercent);

		async Task ActWithPercent()
			=> await That(subject).MatchesExactly(0.1, _ => percentOptions);

		await That(ActWithCopyBeforePercent).DoesNotThrow()
			.Because("the copy keeps the absolute tolerance, as Percent() replaces the tolerance instead of changing it");
		await That(ActWithPercent).Throws<FailException>()
			.Because("5% of 0.1 is less than the difference of 0.2");
	}

	[Test]
	public async Task Percent_ShouldNotAffectOtherOptionsFromTheSameOrigin()
	{
		JsonElement subject = JsonDocument.Parse("0.3").RootElement;
		JsonOptions sut = new();
		JsonOptions absoluteOptions = sut.NumbersWithin(5);

		_ = sut.NumbersWithin(5).Percent();

		async Task Act()
			=> await That(subject).MatchesExactly(0.1, _ => absoluteOptions);

		await That(Act).DoesNotThrow()
			.Because("each call of NumbersWithin creates its own tolerance");
	}

	[Test]
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
