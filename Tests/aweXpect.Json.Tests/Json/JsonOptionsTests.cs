using System.Text.Json;

namespace aweXpect.Json.Tests;

public class JsonOptionsTests
{
	[Test]
	public async Task DocumentOptions_ShouldDefaultToAllowTrailingCommas()
	{
		string subject = "[1, 2,]";
		JsonOptions sut = new();

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], _ => sut);

		await That(Act).DoesNotThrow();
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
		JsonElement subject = JsonDocument.Parse("{\"id\": 1, \"etag\": \"a\", \"name\": \"x\"}").RootElement;
		object expected = new
		{
			id = 2,
			etag = "b",
			name = "x",
		};
		JsonOptions sut = new();

		JsonOptions result = sut.IgnoringMember("id").Ignoring(path => path.EndsWith(".etag"));

		async Task ActWithResult()
			=> await That(subject).MatchesExactly(expected, _ => result);

		async Task ActWithOriginal()
			=> await That(subject).MatchesExactly(expected, _ => sut);

		await That(ActWithResult).DoesNotThrow()
			.Because("both members to ignore are kept");
		await That(ActWithOriginal).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches expected exactly,
			             but it differed as
			               $.id was 1 instead of 2 and
			               $.etag was "a" instead of "b"
			             """)
			.Because("IgnoringMember and Ignoring return new options instead of changing the original ones");
	}

	[Test]
	public async Task ComparingNumbersWithin_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.ComparingNumbersWithin(-0.1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task ComparingNumbersWithin_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.ComparingNumbersWithin(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix();
	}

	[Test]
	public async Task ComparingNumbersWithin_ShouldNotChangeTheOriginalOptions()
	{
		JsonElement subject = JsonDocument.Parse("1.5").RootElement;
		JsonOptions sut = new();

		_ = sut.ComparingNumbersWithin(1);

		async Task Act()
			=> await That(subject).MatchesExactly(1, _ => sut);

		await That(Act).Throws<FailException>()
			.Because("ComparingNumbersWithin returns new options instead of changing the original ones");
	}

	[Test]
	public async Task ComparingNumbersWithin_WhenCalledAgain_ShouldReplaceTheTolerance()
	{
		JsonElement subject = JsonDocument.Parse("1.5").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.ComparingNumbersWithin(1).IgnoringArrayOrder().ComparingNumbersWithin(0.1));

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
		JsonNumbersWithinOptions withinOptions = new JsonOptions().ComparingNumbersWithin(5);
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
		JsonOptions absoluteOptions = sut.ComparingNumbersWithin(5);

		_ = sut.ComparingNumbersWithin(5).Percent();

		async Task Act()
			=> await That(subject).MatchesExactly(0.1, _ => absoluteOptions);

		await That(Act).DoesNotThrow()
			.Because("each call of ComparingNumbersWithin creates its own tolerance");
	}

	[Test]
	public async Task Ignoring_ShouldDescribeThePredicateWithoutCommonWhiteSpace()
	{
		JsonElement subject = JsonDocument.Parse("{\"name\": \"x\"}").RootElement;
		JsonOptions sut = new();

		JsonOptions result = sut.Ignoring(path
			=> path.EndsWith(".etag"));

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				name = "y",
			}, _ => result);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	name = "y",
			             } exactly,
			             but it differed as $.name was "x" instead of "y"

			             JSON options:
			              - ignore members: [path*\n=> path.EndsWith(".etag")]
			             """).AsWildcard()
			.Because("the line break of the predicate is escaped and depends on the line endings of the source");
	}

	[Test]
	public async Task WithDocumentOptions_ShouldSetDocumentOptions()
	{
		string subject = "[1, /* two */ 2]";
		JsonOptions sut = new();

		JsonOptions result = sut.WithDocumentOptions(o => o with
		{
			CommentHandling = JsonCommentHandling.Skip,
		});

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], _ => result);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithDocumentOptions_ShouldSupportProvidingFixedOptions()
	{
		string subject = "[1, /* two */ 2]";
		JsonOptions sut = new();
		JsonDocumentOptions documentOptions = new()
		{
			CommentHandling = JsonCommentHandling.Skip,
		};

		JsonOptions result = sut.WithDocumentOptions(_ => documentOptions);

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], _ => result);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithDocumentOptions_WhenCallbackIsNull_ShouldThrowArgumentNullException()
	{
		JsonOptions sut = new();

		void Act()
			=> sut.WithDocumentOptions(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("documentOptions").And
			.WithMessage("The 'documentOptions' cannot be null.").AsPrefix();
	}
}
