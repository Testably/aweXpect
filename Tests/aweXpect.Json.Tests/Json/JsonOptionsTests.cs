using System.Text.Json;
using aweXpect.Json.Options;

namespace aweXpect.Json.Tests;

public class JsonOptionsTests
{
	[Test]
	public async Task ComparingNumbersWithin_ShouldNotChangeTheOriginalOptions()
	{
		JsonElement subject = JsonDocument.Parse("1.5").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o =>
			{
				_ = o.ComparingNumbersWithin(1);
				return o;
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches 1 exactly,
			             but it differed as $ was 1.5 instead of 1
			             """)
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
	public async Task ComparingNumbersWithin_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		JsonElement subject = JsonDocument.Parse("1").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.ComparingNumbersWithin(double.NaN));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix();
	}

	[Test]
	public async Task ComparingNumbersWithin_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		JsonElement subject = JsonDocument.Parse("1").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.ComparingNumbersWithin(-0.1));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task DocumentOptions_ShouldDefaultToAllowTrailingCommas()
	{
		string subject = "[1, 2,]";

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,]);

		await That(Act).DoesNotThrow();
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

		async Task ActWithResult()
			=> await That(subject).MatchesExactly(expected,
				o => o.IgnoringMember("id").IgnoringMembers(path => path.EndsWith(".etag")));

		async Task ActWithOriginal()
			=> await That(subject).MatchesExactly(expected, o =>
			{
				_ = o.IgnoringMember("id").IgnoringMembers(path => path.EndsWith(".etag"));
				return o;
			});

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
			.Because("IgnoringMember and IgnoringMembers return new options instead of changing the original ones");
	}

	[Test]
	public async Task IgnoringMember_WhenMemberIsEmpty_ShouldThrowArgumentException()
	{
		JsonElement subject = JsonDocument.Parse("1").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.IgnoringMember(""));

		await That(Act).Throws<ArgumentException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be empty.").AsPrefix();
	}

	[Test]
	public async Task IgnoringMember_WhenMemberIsNull_ShouldThrowArgumentNullException()
	{
		JsonElement subject = JsonDocument.Parse("1").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.IgnoringMember(null!));

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task IgnoringMembers_ShouldDescribeThePredicateWithoutCommonWhiteSpace()
	{
		JsonElement subject = JsonDocument.Parse("{\"name\": \"x\"}").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				name = "y",
			}, o => o.IgnoringMembers(path
				=> path.EndsWith(".etag")));

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
	public async Task IgnoringMembers_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		JsonElement subject = JsonDocument.Parse("1").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.IgnoringMembers(null!));

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Percent_ShouldNotAffectCopiesMadeBefore()
	{
		JsonElement subject = JsonDocument.Parse("3").RootElement;

		async Task ActWithCopyBeforePercent()
			=> await That(subject).MatchesExactly(1, o =>
			{
				JsonNumbersWithinOptions withinOptions = o.ComparingNumbersWithin(5);
				JsonOptions copyBeforePercent = withinOptions.IgnoringArrayOrder();
				_ = withinOptions.Percent();
				return copyBeforePercent;
			});

		async Task ActWithPercent()
			=> await That(subject).MatchesExactly(1, o =>
			{
				JsonNumbersWithinOptions withinOptions = o.ComparingNumbersWithin(5);
				_ = withinOptions.IgnoringArrayOrder();
				return withinOptions.Percent();
			});

		await That(ActWithCopyBeforePercent).DoesNotThrow()
			.Because("the copy keeps the absolute tolerance, as Percent() replaces the tolerance instead of changing it");
		await That(ActWithPercent).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches 1 exactly,
			             but it differed as $ was 3 instead of 1 ± 5%

			             JSON options:
			              - compare numbers within ± 5%
			             """)
			.Because("5% of 1 is less than the difference of 2");
	}

	[Test]
	public async Task Percent_ShouldNotAffectOtherOptionsFromTheSameOrigin()
	{
		JsonElement subject = JsonDocument.Parse("0.3").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(0.1, o =>
			{
				JsonOptions absoluteOptions = o.ComparingNumbersWithin(5);
				_ = o.ComparingNumbersWithin(5).Percent();
				return absoluteOptions;
			});

		await That(Act).DoesNotThrow()
			.Because("each call of ComparingNumbersWithin creates its own tolerance");
	}

	[Test]
	public async Task Percent_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		JsonElement subject = JsonDocument.Parse("3").RootElement;

		async Task Act()
			=> await That(subject).MatchesExactly(1, o =>
			{
				JsonNumbersWithinOptions withinOptions = o.ComparingNumbersWithin(5);
				_ = withinOptions.Percent();
				return withinOptions;
			});

		await That(Act).DoesNotThrow()
			.Because("Percent() returns new options, so the options it is called on keep the absolute tolerance");
	}

	[Test]
	public async Task WithDocumentOptions_ShouldSetDocumentOptions()
	{
		string subject = "[1, /* two */ 2]";

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], o => o.WithDocumentOptions(d => d with
			{
				CommentHandling = JsonCommentHandling.Skip,
			}));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithDocumentOptions_ShouldSupportProvidingFixedOptions()
	{
		string subject = "[1, /* two */ 2]";
		JsonDocumentOptions documentOptions = new()
		{
			CommentHandling = JsonCommentHandling.Skip,
		};

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], o => o.WithDocumentOptions(_ => documentOptions));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithDocumentOptions_WhenCallbackIsNull_ShouldThrowArgumentNullException()
	{
		string subject = "[1, 2]";

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly([1, 2,], o => o.WithDocumentOptions(null!));

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("documentOptions").And
			.WithMessage("The 'documentOptions' cannot be null.").AsPrefix();
	}
}
