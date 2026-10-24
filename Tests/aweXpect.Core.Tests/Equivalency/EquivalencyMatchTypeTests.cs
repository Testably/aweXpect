using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyMatchTypeTests
{
	[Test]
	public async Task AppendContexts_AfterAFailedComparisonWithAMapping_ShouldNameItsUnmatchedMember()
	{
		ObjectMatcher sut = new EquivalencyMatchType(new EquivalencyOptions().Mapping("Value", "Valeu"))
			.ForEvaluation(CreateEnvironment());
		await sut.MatchWithExplanation(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 2,
		});

		string? result = await ContextOf(sut);

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - map "Value" to "Valeu"
		                                "Valeu" matched no member (did you mean "Value"?)
		                             """)
			.Because("a mapping names its expected member by the end of its path, like a member to ignore");
	}

	[Test]
	public async Task AppendContexts_AfterAFailedDecidingComparison_ShouldNameTheMembersToIgnore()
	{
		ObjectMatcher sut = new EquivalencyMatchType(IgnoringMember("Valeu")).ForEvaluation(CreateEnvironment());
		await sut.Match(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 2,
		});

		string? result = await ContextOf(sut);

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - ignore members: ["Valeu"]
		                                "Valeu" matched no member (did you mean "Value"?)
		                             """)
			.Because("the items of a collection are compared without an explanation");
	}

	[Test]
	public async Task AppendContexts_AfterAFailedDecidingComparisonWithAMatchedMapping_ShouldNotNameIt()
	{
		ObjectMatcher sut = new EquivalencyMatchType(new EquivalencyOptions().Mapping("Missing", "Value"))
			.ForEvaluation(CreateEnvironment());
		MatchResult matchResult = await sut.Match(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 1,
		});

		string? result = await ContextOf(sut);

		await That(matchResult.IsMatch).IsFalse()
			.Because("the mapped actual member does not exist");
		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - map "Missing" to "Value"
		                             """);
	}

	[Test]
	public async Task AppendContexts_AfterAFailedDecidingComparisonWithRegisteredOptions_ShouldNameTheMembersToIgnore()
	{
		ObjectMatcher sut = new EquivalencyMatchType(new EquivalencyOptions().For<Dummy>(o => o with
		{
			MembersToIgnore = [new MemberToIgnore.ByName("Valeu"),],
		})).ForEvaluation(CreateEnvironment());
		await sut.Match(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 2,
		});

		string? result = await ContextOf(sut);

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - for EquivalencyMatchTypeTests.Dummy:
		                                - include public fields and properties
		                                - ignore members: ["Valeu"]
		                                  "Valeu" matched no member (did you mean "Value"?)
		                             """)
			.Because("the options registered for a type are only known during the comparison");
	}

	[Test]
	public async Task AppendContexts_AfterSeveralFailedComparisons_ShouldKeepTheSuggestionOfAnEarlierOne()
	{
		ObjectMatcher sut = new EquivalencyMatchType(IgnoringMember("Titel")).ForEvaluation(CreateEnvironment());
		await sut.MatchWithExplanation(new Album
		{
			Title = "Abbey Road",
		}, new Album
		{
			Title = "Let It Be",
		});
		await sut.MatchWithExplanation(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 2,
		});

		string? result = await ContextOf(sut);

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - ignore members: ["Titel"]
		                                "Titel" matched no member (did you mean "Title"?)
		                             """)
			.Because("the items of a collection are compared with the same matcher, and a later item without a close member must not hide the suggestion for an earlier one");
	}

	[Test]
	public async Task AppendContexts_WhenEquivalent_ShouldNotNameTheMembersToIgnore()
	{
		ObjectMatcher sut = new EquivalencyMatchType(IgnoringMember("Valeu")).ForEvaluation(CreateEnvironment());
		await sut.MatchWithExplanation(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 1,
		});

		string? result = await ContextOf(sut);

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - ignore members: ["Valeu"]
		                             """);
	}

	[Test]
	public async Task Constructor_WhenOptionsAreNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = new EquivalencyMatchType(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("equivalencyOptions").And
			.WithMessage("The 'equivalencyOptions' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ForEvaluation_ShouldKeepTheMembersToIgnoreOfEachEvaluationApart()
	{
		EquivalencyMatchType sut = new(IgnoringMember("Valeu"));
		ObjectMatcher firstEvaluation = sut.ForEvaluation(CreateEnvironment());
		ObjectMatcher secondEvaluation = sut.ForEvaluation(CreateEnvironment());
		await firstEvaluation.MatchWithExplanation(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = 2,
		});

		string? firstResult = await ContextOf(firstEvaluation);
		string? secondResult = await ContextOf(secondEvaluation);

		await That(firstResult).IsEqualTo("""
		                                   - include public fields and properties
		                                   - ignore members: ["Valeu"]
		                                     "Valeu" matched no member (did you mean "Value"?)
		                                  """);
		await That(secondResult).IsEqualTo("""
		                                    - include public fields and properties
		                                    - ignore members: ["Valeu"]
		                                   """)
			.Because("each evaluation keeps the members to ignore of its own comparisons");
	}

	[Test]
	[Arguments(1, true)]
	[Arguments(2, false)]
	public async Task Match_ShouldDecideWhetherTheObjectsAreEquivalent(int expectedValue, bool expectMatch)
	{
		ObjectMatcher sut = new EquivalencyMatchType(new EquivalencyOptions()).ForEvaluation(CreateEnvironment());

		MatchResult result = await sut.Match(new Dummy
		{
			Value = 1,
		}, new Dummy
		{
			Value = expectedValue,
		});

		await That(result.IsMatch).IsEqualTo(expectMatch);
		await That(result.Explanation).IsNull()
			.Because("a deciding comparison does not collect the differences");
	}

	[Test]
	public async Task MatchWithExplanation_WhenNotEquivalent_ShouldExplainTheDifferences()
	{
		ObjectMatcher sut = new EquivalencyMatchType(new EquivalencyOptions()).ForEvaluation(CreateEnvironment());

		MatchResult result = await sut.MatchWithExplanation(new Dummy
			{
				Value = 1,
			},
			new Dummy
			{
				Value = 2,
			});

		await That(result.IsMatch).IsFalse();
		await That(sut.GetFailure("it", ExpectationGrammars.None, new Dummy(), new Dummy(), result, null))
			.IsEqualTo("""
			           it was not:
			             Property Value differed:
			                 Actual: 1
			               Expected: 2
			           """)
			.Because("the result keeps the differences of its own comparison, so that the matcher keeps none");
	}

	[Test]
	public async Task Phrase_ShouldPrependTheItemNoun()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		string result = sut.GetItemExpectation("expected", "an item", "equal to");

		await That(result).IsEqualTo("an item equivalent to expected")
			.Because("the comparison is replaced by the equivalency");
	}

	[Test]
	public async Task Phrase_WithoutItemNoun_ShouldOnlyNameTheEquivalency()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		string result = sut.GetItemExpectation("expected", comparison: "equal to");

		await That(result).IsEqualTo("equivalent to expected");
	}

	[Test]
	public async Task ToString_ShouldDescribeTheEquivalency()
	{
		ObjectEqualityOptions<int> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		string? result = options.ToString();

		await That(result).IsEqualTo(" using equivalency");
	}

	[Test]
	public async Task WhenEquivalent_ShouldSucceed()
	{
		Dummy subject = new()
		{
			Value = 1,
		};

		async Task Act()
			=> await That(subject).IsEquivalentToUsingMatchType(new
			{
				Value = 1,
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenExpectedIsNull_ShouldFailLikeIsEquivalentTo()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy? expected = null;

		async Task Act()
			=> await That(subject).IsEquivalentToUsingMatchType(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to <null>,
			             but it was EquivalencyMatchTypeTests.Dummy { Value = 1 } instead of <null>

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(await MessageOf(Act))
			.IsEqualTo(await MessageOf(async () => await That(subject).IsEquivalentTo(expected)))
			.Because("the failure is the same as the one of the built-in expectation");
	}

	[Test]
	public async Task WhenNegatedAndEquivalent_InACombination_ShouldIndentTheValueLikeTheEntry()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy unexpected = new()
		{
			Value = 1,
		};

		async Task Act()
			=> await ThatAll(That(subject).IsNotEquivalentToUsingMatchType(unexpected));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is not equivalent to unexpected
			             but
			              [01] it was EquivalencyMatchTypeTests.Dummy {
			                     Value = 1
			                   }, which is considered equivalent

			             [01] Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task WhenNegatedAndEquivalent_ShouldFailLikeIsNotEquivalentTo()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy unexpected = new()
		{
			Value = 1,
		};

		async Task Act()
			=> await That(subject).IsNotEquivalentToUsingMatchType(unexpected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not equivalent to unexpected,
			             but it was EquivalencyMatchTypeTests.Dummy {
			                 Value = 1
			               }, which is considered equivalent

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(await MessageOf(Act))
			.IsEqualTo(await MessageOf(async () => await That(subject).IsNotEquivalentTo(unexpected)))
			.Because("the failure is the same as the one of the built-in expectation");
	}

	[Test]
	public async Task WhenNotEquivalent_AndANameToIgnoreMatchesNoMember_ShouldNameItBelowTheOptions()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy expected = new()
		{
			Value = 2,
		};

		async Task Act()
			=> await That(subject).IsEquivalentToUsingMatchType(expected, IgnoringMember("Valeu"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to expected,
			             but it was not:
			               Property Value differed:
			                   Actual: 1
			                 Expected: 2

			             Equivalency options:
			              - include public fields and properties
			              - ignore members: ["Valeu"]
			                "Valeu" matched no member (did you mean "Value"?)
			             """);
	}

	[Test]
	public async Task WhenNotEquivalent_InACombination_ShouldShiftTheDifferencesUnderTheEntry()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy expected = new()
		{
			Value = 2,
		};

		async Task Act()
			=> await ThatAll(That(subject).IsEquivalentToUsingMatchType(expected));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is equivalent to expected
			             but
			              [01] it was not:
			                     Property Value differed:
			                         Actual: 1
			                       Expected: 2

			             [01] Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task WhenNotEquivalent_ShouldFailLikeIsEquivalentTo()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy expected = new()
		{
			Value = 2,
		};

		async Task Act()
			=> await That(subject).IsEquivalentToUsingMatchType(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to expected,
			             but it was not:
			               Property Value differed:
			                   Actual: 1
			                 Expected: 2

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(await MessageOf(Act))
			.IsEqualTo(await MessageOf(async () => await That(subject).IsEquivalentTo(expected)))
			.Because("the failure is the same as the one of the built-in expectation");
	}

	private static async Task<string?> ContextOf(ObjectMatcher matcher)
	{
		ResultContextCollector contexts = new();
		matcher.AppendContexts(contexts);
		return await contexts.Entries.Single().Context.GetContent();
	}

	private static MatchEnvironment CreateEnvironment()
		=> new(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None);

	private static EquivalencyOptions IgnoringMember(string memberName)
		=> new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName(memberName),],
		};

	private static async Task<string> MessageOf(Func<Task> act)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			return exception.Message;
		}

		throw new InvalidOperationException("The expectation did not fail.");
	}

	public sealed class Album
	{
		public string? Title { get; set; }
	}

	public sealed class Dummy
	{
		public int Value { get; set; }
	}
}

internal static class EquivalencyMatchTypeTestExtensions
{
	/// <summary>
	///     An equivalency expectation of a Core-only extension, which uses the <see cref="EquivalencyMatchType" />.
	/// </summary>
	public static AndOrResult<TSubject, IThat<TSubject>> IsEquivalentToUsingMatchType<TSubject, TExpected>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] TExpected expected,
		EquivalencyOptions? equivalencyOptions = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquivalentToConstraint<TSubject, TExpected>(it, grammars, expected,
					expected is null ? null : doNotPopulateThisValue, CreateOptions<TSubject>(equivalencyOptions))),
			subject);

	public static AndOrResult<TSubject, IThat<TSubject>> IsNotEquivalentToUsingMatchType<TSubject, TExpected>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] TExpected unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquivalentToConstraint<TSubject, TExpected>(it, grammars, unexpected,
					unexpected is null ? null : doNotPopulateThisValue, CreateOptions<TSubject>()).Invert()),
			subject);

	private static ObjectEqualityOptions<TSubject> CreateOptions<TSubject>(
		EquivalencyOptions? equivalencyOptions = null)
	{
		ObjectEqualityOptions<TSubject> options = new();
		options.SetMatchType(
			new EquivalencyMatchType(equivalencyOptions
			                         ?? Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get()),
			"Equivalent");
		return options;
	}

	private sealed class IsEquivalentToConstraint<TSubject, TExpected>(
		string it,
		ExpectationGrammars grammars,
		TExpected expected,
		string? expectedExpression,
		ObjectEqualityOptions<TSubject> options)
		: ConstraintResult.WithEqualToValue<TSubject>(it, grammars, expected is null),
			IAsyncContextConstraint<TSubject>
	{
		private ObjectEqualityOptions<TSubject>? _evaluationOptions;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddEqualityOptionsContexts(_evaluationOptions ?? options);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented()), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult, indentation));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented()), Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult, indentation));
	}
}
