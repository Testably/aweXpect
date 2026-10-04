using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyMatchTypeTests
{
	[Fact]
	public async Task AreConsideredEqualWithExplanation_ShouldReturnTheMatchTypeItself()
	{
		EquivalencyMatchType sut = new(new EquivalencyOptions());

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(new Dummy { Value = 1, },
			new Dummy { Value = 2, });

		await That(result).IsSameAs(sut)
			.Because("the match type keeps the differences itself, so that a comparison allocates no result");
		await That(result.IsMatch).IsFalse();
		await That(result.GetExtendedFailure("it", ExpectationGrammars.None, new Dummy(), new Dummy()))
			.IsEqualTo("""
			           it was not:
			             Property Value differed:
			                 Actual: 1
			               Expected: 2
			           """);
	}

	[Theory]
	[InlineData(1, true)]
	[InlineData(2, false)]
	public async Task AreConsideredEqual_ShouldDecideWhetherTheObjectsAreEquivalent(int expectedValue,
		bool expectMatch)
	{
		EquivalencyMatchType sut = new(new EquivalencyOptions());

		bool result = await sut.AreConsideredEqual(new Dummy { Value = 1, }, new Dummy { Value = expectedValue, });

		await That(result).IsEqualTo(expectMatch);
	}

	[Fact]
	public async Task Constructor_WhenOptionsAreNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = new EquivalencyMatchType(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("equivalencyOptions").And
			.WithMessage("The 'equivalencyOptions' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task PrependItemAndComparison_ShouldPrependTheItemNoun()
	{
		EquivalencyMatchType sut = new(new EquivalencyOptions());

		string result = sut.PrependItemAndComparison("expected", "an item", "equal to");

		await That(result).IsEqualTo("an item equivalent to expected")
			.Because("the comparison is replaced by the equivalency");
	}

	[Fact]
	public async Task PrependItemAndComparison_WithoutItemNoun_ShouldOnlyNameTheEquivalency()
	{
		EquivalencyMatchType sut = new(new EquivalencyOptions());

		string result = sut.PrependItemAndComparison("expected", comparison: "equal to");

		await That(result).IsEqualTo("equivalent to expected");
	}

	[Fact]
	public async Task ToString_ShouldDescribeTheEquivalency()
	{
		ObjectEqualityOptions<int> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		string? result = options.ToString();

		await That(result).IsEqualTo(" using equivalency");
	}

	[Fact]
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

	[Fact]
	public async Task WhenExpectedIsNull_ShouldFailLikeIsEquivalentTo()
	{
		Dummy subject = new()
		{
			Value = 1,
		};
		Dummy? expected = null;

		async Task Act()
			=> await That(subject).IsEquivalentToUsingMatchType(expected);

		await That(Act).Throws<XunitException>()
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

	[Fact]
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

		await That(Act).Throws<XunitException>()
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

	[Fact]
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

		await That(Act).Throws<XunitException>()
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

	private static async Task<string> MessageOf(Func<Task> act)
	{
		try
		{
			await act();
		}
		catch (XunitException exception)
		{
			return exception.Message;
		}

		throw new InvalidOperationException("The expectation did not fail.");
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
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquivalentToConstraint<TSubject, TExpected>(it, grammars, expected,
					expected is null ? null : doNotPopulateThisValue, CreateOptions<TSubject>())),
			subject);

	public static AndOrResult<TSubject, IThat<TSubject>> IsNotEquivalentToUsingMatchType<TSubject, TExpected>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] TExpected unexpected,
		[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquivalentToConstraint<TSubject, TExpected>(it, grammars, unexpected,
					unexpected is null ? null : doNotPopulateThisValue, CreateOptions<TSubject>()).Invert()),
			subject);

	private static ObjectEqualityOptions<TSubject> CreateOptions<TSubject>()
	{
		ObjectEqualityOptions<TSubject> options = new();
		options.SetMatchType(
			new EquivalencyMatchType(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get()),
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
			IAsyncConstraint<TSubject>
	{
		private IObjectMatchResult? _matchResult;

		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddEqualityOptionsContexts(options);

		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_matchResult = await options.AreConsideredEqualWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented()), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented()), Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));
	}
}
