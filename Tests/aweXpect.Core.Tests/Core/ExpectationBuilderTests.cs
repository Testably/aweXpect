using System.Globalization;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public class ExpectationBuilderTests
{
	[Fact]
	public async Task ForAsyncMember_ShouldUseAndResetExpectationGrammars()
	{
		ManualExpectationBuilder<string> sut = new(null);
		ExpectationGrammars usedExpectationGrammars = ExpectationGrammars.None;

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, g)
				=>
			{
				usedExpectationGrammars = g;
				return new DummyConstraint<int>(v => v == 2, "equal to 2");
			}), _ => ExpectationGrammars.Nested);

		await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(usedExpectationGrammars).IsEqualTo(ExpectationGrammars.Nested);
		await That(sut.ExpectationGrammars).IsEqualTo(ExpectationGrammars.None);
	}

	[Fact]
	public async Task ForAsyncMember_WithAndCombinedExpectations_ShouldApplyAllExpectations()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"))
				.And()
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3 and equal to 2");
	}

	[Fact]
	public async Task ForAsyncMember_WithFailingExpectation_ShouldReturnFailureConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2");
	}

	[Fact]
	public async Task ForAsyncMember_WithOrCombinedExpectations_ShouldApplyEitherExpectation()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder =>
			{
				expectationBuilder.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2"));
				expectationBuilder.Or();
				expectationBuilder.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"));
			});

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2 or equal to 3");
	}

	[Fact]
	public async Task ForAsyncMember_WithSucceedingExpectation_ShouldReturnSuccessConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3");
	}

	[Fact]
	public async Task ForAsyncMember_WithValidation_ShouldIncludeValidation()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.Validate((_, _) => new DummyConstraint<string>(_ => false, "validated and "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("validated and length equal to 3");
	}

	[Fact]
	public async Task ForMember_ShouldUseAndResetExpectationGrammars()
	{
		ManualExpectationBuilder<string> sut = new(null);
		ExpectationGrammars usedExpectationGrammars = ExpectationGrammars.None;

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, g)
				=>
			{
				usedExpectationGrammars = g;
				return new DummyConstraint<int>(v => v == 2, "equal to 2");
			}), _ => ExpectationGrammars.Nested);

		await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(usedExpectationGrammars).IsEqualTo(ExpectationGrammars.Nested);
		await That(sut.ExpectationGrammars).IsEqualTo(ExpectationGrammars.None);
	}

	[Fact]
	public async Task ForMember_WithAndCombinedExpectations_ShouldApplyAllExpectations()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"))
				.And()
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3 and equal to 2");
	}

	[Fact]
	public async Task ForMember_WithFailingExpectation_ShouldReturnFailureConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2");
	}

	[Fact]
	public async Task ForMember_WithOrCombinedExpectations_ShouldApplyEitherExpectation()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder =>
			{
				expectationBuilder.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2"));
				expectationBuilder.Or();
				expectationBuilder.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"));
			});

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2 or equal to 3");
	}

	[Fact]
	public async Task ForMember_WithSucceedingExpectation_ShouldReturnSuccessConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3");
	}

	[Fact]
	public async Task ForMember_WithValidation_ShouldIncludeValidation()
	{
		ManualExpectationBuilder<string> sut = new(null);

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.Validate((_, _) => new DummyConstraint<string>(_ => false, "validated and "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("validated and length equal to 3");
	}

	[Fact]
	public async Task ForWhich_Async_CalledTwice_ShouldHonorConstraintsFromAllLevels()
	{
		Func<string, Task<string?>> upperAccessor = s => Task.FromResult<string?>(s.ToUpperInvariant());
		Func<string, Task<string?>> doubledAccessor = s => Task.FromResult<string?>(s + s);
		ManualExpectationBuilder<string> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich(upperAccessor, " whose upper ");
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "FOO", "is FOO"));
		sut.ForWhich(doubledAccessor, " and whose doubled ");
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "FOOFOO", "is FOOFOO"));

		ConstraintResult result = await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is foo whose upper is FOO and whose doubled is FOOFOO");
	}

	[Fact]
	public async Task ForWhich_Async_CalledTwice_WhereSecondProjectsFromFirstResult_ShouldChainProjections()
	{
		Func<int, Task<string?>> stringify = i =>
			Task.FromResult<string?>(i.ToString(CultureInfo.InvariantCulture));
		Func<string, Task<string?>> doubled = s => Task.FromResult<string?>(s + s);
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 12, "is 12"));
		sut.ForWhich(stringify, " whose string ");
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "12", "is \"12\""));
		sut.ForWhich(doubled, " and whose doubled ");
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "1212", "is \"1212\""));

		ConstraintResult result = await sut.IsMetBy(12, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is 12 whose string is \"12\" and whose doubled is \"1212\"");
	}

	[Fact]
	public async Task ForWhich_CalledThreeTimes_EachProjectionChainsFromPrevious_ShouldEvaluateDeeply()
	{
		ManualExpectationBuilder<string> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, char>(s => s[0], " whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c => c == 'f', "is 'f'"));
		sut.ForWhich<char, int>(c => c, " whose code point ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 'f', "is 102"));
		sut.ForWhich<int, bool>(i => i % 2 == 0, " whose is-even ");
		sut.AddConstraint((_, _) => new DummyConstraint<bool>(b => b, "is true"));

		ConstraintResult result = await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is foo whose first char is 'f' whose code point is 102 whose is-even is true");
	}

	[Fact]
	public async Task ForWhich_CalledTwice_OuterConstraintFails_ShouldStillEvaluateOuterConstraint()
	{
		ManualExpectationBuilder<string> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));
		sut.ForWhich<string, char>(s => s[0], " and whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c => c == 'B', "is 'B'"));

		ConstraintResult result = await sut.IsMetBy("BAR", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText())
			.IsEqualTo("is foo whose length is 3 and whose first char is 'B'");
	}

	[Fact]
	public async Task ForWhich_CalledTwice_SecondProjectionFails_ShouldIncludeAllProjectionsInOrder()
	{
		ManualExpectationBuilder<string> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));
		sut.ForWhich<string, char>(s => s[0], " and whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c => c == 'x', "is 'x'"));

		ConstraintResult result = await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText())
			.IsEqualTo("is foo whose length is 3 and whose first char is 'x'");
	}

	[Fact]
	public async Task ForWhich_CalledTwice_ShouldHonorConstraintsFromAllLevels()
	{
		ManualExpectationBuilder<string> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));
		sut.ForWhich<string, char>(s => s[0], " and whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c => c == 'f', "is 'f'"));

		ConstraintResult result = await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is foo whose length is 3 and whose first char is 'f'");
	}

	[Fact]
	public async Task ForWhich_CalledTwice_WhereSecondProjectsFromFirstResult_ShouldChainProjections()
	{
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 123, "is 123"));
		sut.ForWhich<int, string>(i => i.ToString(CultureInfo.InvariantCulture),
			" whose string ");
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "123", "is \"123\""));
		sut.ForWhich<string, int>(s => s.Length, " and whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));

		ConstraintResult result = await sut.IsMetBy(123, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is 123 whose string is \"123\" and whose length is 3");
	}

	[Fact]
	public async Task WhenSubjectHasMultipleLines_ShouldTrimCommonWhiteSpace()
	{
		async Task Act() => await That(new[]
		{
			1, 2, 3,
		}).IsEmpty();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that new[]
			             {
			             	1, 2, 3,
			             }
			             is empty,
			             but it was [
			               1,
			               2,
			               3
			             ]
			             """);
	}

	[Fact]
	public async Task WhenTypeImplementsIDescribableSubject_ShouldUseToStringFromIt()
	{
		MyDescribableSubject subject = new("this long description for the subject");

		async Task Act() => await That(subject).IsNull();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that this long description for the subject
			             is null,
			             but it was ExpectationBuilderTests.MyDescribableSubject { }
			             """);
	}

	private sealed class MyDescribableSubject(string subject) : IDescribableSubject
	{
		public string GetDescription()
			=> subject;
	}
}
