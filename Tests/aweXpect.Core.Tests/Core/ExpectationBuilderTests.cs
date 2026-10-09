using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public class ExpectationBuilderTests
{
	[Test]
	public async Task AddConstraint_WithState_AsyncConstraint_ShouldPassStateSubjectNameAndGrammars()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;

		sut.AddConstraint(3, (state, it, grammars) =>
		{
			received = (state, it, grammars);
			return new DummyAsyncConstraint<int>(v => Task.FromResult(new DummyConstraint<int>(x => x == state).IsMetBy(v)));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_AsyncConstraintWithBuilder_ShouldAlsoPassTheBuilder()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;
		ExpectationBuilder? receivedBuilder = null;

		sut.AddConstraint(3, (state, builder, it, grammars) =>
		{
			received = (state, it, grammars);
			receivedBuilder = builder;
			return new DummyAsyncConstraint<int>(v => Task.FromResult(new DummyConstraint<int>(x => x == state).IsMetBy(v)));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(receivedBuilder).IsSameAs(sut);
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_AsyncContextConstraint_ShouldPassStateSubjectNameAndGrammars()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;

		sut.AddConstraint(3, (state, it, grammars) =>
		{
			received = (state, it, grammars);
			return new DummyAsyncContextConstraint<int>(v => Task.FromResult(new DummyConstraint<int>(x => x == state).IsMetBy(v)));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_AsyncContextConstraintWithBuilder_ShouldAlsoPassTheBuilder()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;
		ExpectationBuilder? receivedBuilder = null;

		sut.AddConstraint(3, (state, builder, it, grammars) =>
		{
			received = (state, it, grammars);
			receivedBuilder = builder;
			return new DummyAsyncContextConstraint<int>(v => Task.FromResult(new DummyConstraint<int>(x => x == state).IsMetBy(v)));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(receivedBuilder).IsSameAs(sut);
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_ContextConstraint_ShouldPassStateSubjectNameAndGrammars()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;

		sut.AddConstraint(3, (state, it, grammars) =>
		{
			received = (state, it, grammars);
			return new DummyContextConstraint<int>(v => new DummyConstraint<int>(x => x == state).IsMetBy(v));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_ContextConstraintWithBuilder_ShouldAlsoPassTheBuilder()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;
		ExpectationBuilder? receivedBuilder = null;

		sut.AddConstraint(3, (state, builder, it, grammars) =>
		{
			received = (state, it, grammars);
			receivedBuilder = builder;
			return new DummyContextConstraint<int>(v => new DummyConstraint<int>(x => x == state).IsMetBy(v));
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(receivedBuilder).IsSameAs(sut);
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_ValueConstraint_ShouldPassStateSubjectNameAndGrammars()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;

		sut.AddConstraint(3, (state, it, grammars) =>
		{
			received = (state, it, grammars);
			return new DummyConstraint<int>(v => v == state);
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddConstraint_WithState_ValueConstraintWithBuilder_ShouldAlsoPassTheBuilder()
	{
		ManualExpectationBuilder<int> sut = new(ExpectationGrammars.Plural);
		(int, string, ExpectationGrammars)? received = null;
		ExpectationBuilder? receivedBuilder = null;

		sut.AddConstraint(3, (state, builder, it, grammars) =>
		{
			received = (state, it, grammars);
			receivedBuilder = builder;
			return new DummyConstraint<int>(v => v == state);
		});
		ConstraintResult result = await sut.IsMetBy(3, null!, CancellationToken.None);

		await That(received).IsEqualTo((3, "it", ExpectationGrammars.Plural));
		await That(receivedBuilder).IsSameAs(sut);
		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the constraint compares with the state");
	}

	[Test]
	public async Task AddSubjectContexts_ForAMember_ShouldLabelTheContextWithTheMember()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.Second, second => second.WithSubjectContext().IsEqualTo(3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Second is equal to 3,
			             but Second was 2, which differs by -1

			             Subject (Second):
			             2
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_ForAnItem_ShouldLabelTheContextWithTheItem()
	{
		int[] subject = [1, 2,];

		async Task Act()
			=> await That(subject).All().ComplyWith(item => item.WithSubjectContext().IsEqualTo(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1 for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [2]

			             Collection:
			             [1, 2]

			             Subject (item [1]):
			             2
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_ForTheMemberOfWhich_ShouldAddTheContextOfTheMember()
	{
		int[] subject = [1,];

		async Task Act()
			=> await That(subject).HasSingle().Which.WithSubjectContext().IsEqualTo(2);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a single item that is equal to 2,
			             but it was 1, which differs by -1

			             Subject:
			             1
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_InAWhichScope_WhenWhichFollowsAnOr_ShouldOnlyContinueTheRightOperand()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddSubjectContexts<int>((_, _) => { });
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 4, "is 4"));
		sut.ForWhich<int, bool>(i => i % 2 == 0, " whose evenness ");
		sut.AddConstraint((_, _) => new DummyConstraint<bool>(b => b, "is true"));

		ConstraintResult result = await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the evenness only continues the right operand \"is 4\", while the length 3 meets the left one");
	}

	[Test]
	public async Task AddSubjectContexts_ShouldPrecedeTheContextsOfTheExpectations()
	{
		async Task Act()
			=> await That(1).MatchesValue("Value", 2).And.WithSubjectContext().IsNotEqualTo(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             matches Value 2 and is not equal to 3,
			             but it did not

			             Subject:
			             1

			             Value:
			             1
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_WhenAddedTwice_ShouldOnlyCallTheCallbackOnce()
	{
		int calls = 0;
		Action<int, ResultContextCollector> appendContexts = (_, _) => calls++;

		async Task Act()
			=> await That(1).WithSubjectContext(appendContexts).IsEqualTo(2)
				.And.WithSubjectContext(appendContexts).IsNotEqualTo(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             is equal to 2 and is not equal to 3,
			             but it was 1, which differs by -1
			             """);
		await That(calls).IsEqualTo(1)
			.Because("the same callback is only added once for the same subject");
	}

	[Test]
	public async Task AddSubjectContexts_WhenAnExpectationAddedAfterFails_ShouldAddTheContext()
	{
		async Task Act()
			=> await That(1).WithSubjectContext().IsNotEqualTo(3).And.IsEqualTo(2);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             is not equal to 3 and is equal to 2,
			             but it was 1, which differs by -1

			             Subject:
			             1
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_WhenAnExpectationAddedBeforeFails_ShouldAddTheContext()
	{
		async Task Act()
			=> await That(1).IsEqualTo(2).And.WithSubjectContext().IsNotEqualTo(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             is equal to 2 and is not equal to 3,
			             but it was 1, which differs by -1

			             Subject:
			             1
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_WhenNegated_ShouldAddTheContext()
	{
		async Task Act()
			=> await That(1).DoesNotComplyWith(it => it.WithSubjectContext().IsEqualTo(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             is not equal to 1,
			             but it was 1

			             Subject:
			             1
			             """);
	}

	[Test]
	public async Task AddSubjectContexts_WhenTheExpectationIsMet_ShouldNotCallTheCallback()
	{
		int calls = 0;

		await That(1).WithSubjectContext((_, _) => calls++).IsEqualTo(1);

		await That(calls).IsEqualTo(0);
	}

	[Test]
	public async Task AddSubjectContexts_WhenTheValueHasAnotherType_ShouldNotCallTheCallback()
	{
		int calls = 0;

		async Task Act()
		{
			IThat<int> that = That(1);
			that.Get().ExpectationBuilder.AddSubjectContexts<string>((_, _) => calls++);
			await that.IsEqualTo(2);
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             is equal to 2,
			             but it was 1, which differs by -1
			             """);
		await That(calls).IsEqualTo(0);
	}

	[Test]
	public async Task AddSubjectContexts_WithWhich_ShouldKeepTheContextOfTheOuterSubject()
	{
		int[] subject = [1,];

		async Task Act()
			=> await That(subject).WithSubjectContext().HasSingle().Which.IsEqualTo(2);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has a single item that is equal to 2,
			             but it was 1, which differs by -1

			             Subject:
			             [1]
			             """)
			.Because("the expectations after Which are on the item, but the context was added for the collection");
	}

	[Test]
	public async Task ForAsyncMember_ShouldUseAndResetExpectationGrammars()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForAsyncMember_WhenTheSourceOfANestedMemberIsNull_ShouldNameTheOuterMember()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, string?>.FromFunc(_ => null, "inner "))
			.AddExpectations(inner => inner
				.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
				.AddExpectations(length => length.AddConstraint((_, _)
					=> new DummyConstraint<int>(v => v == 3, "equal to 3"))));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(constraintResult.GetResultText()).IsEqualTo("inner was <null>")
			.Because("the subject itself was not null, only the member the nested member is read from");
	}

	[Test]
	public async Task ForAsyncMember_WithAndCombinedExpectations_ShouldApplyAllExpectations()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"))
				.And()
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3 and equal to 2");
	}

	[Test]
	public async Task ForAsyncMember_WithFailingExpectation_ShouldReturnFailureConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2");
	}

	[Test]
	public async Task ForAsyncMember_WithOrCombinedExpectations_ShouldApplyEitherExpectation()
	{
		ManualExpectationBuilder<string> sut = new();

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

	[Test]
	public async Task ForAsyncMember_WithSucceedingExpectation_ShouldReturnSuccessConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3");
	}

	[Test]
	public async Task ForAsyncMember_WithValidation_ShouldIncludeValidation()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForAsyncMember(MemberAccessor<string, Task<int>>.FromFunc(x => Task.FromResult(x.Length), "length "))
			.Validate((_, _) => new DummyConstraint<string>(_ => false, "validated and "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("validated and length equal to 3");
	}

	[Test]
	public async Task ForMember_ShouldUseAndResetExpectationGrammars()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForMember_WhenTheSourceOfANestedMemberIsNull_ShouldNameTheOuterMember()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, string?>.FromFunc(_ => null, "inner "))
			.AddExpectations(inner => inner
				.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
				.AddExpectations(length => length.AddConstraint((_, _)
					=> new DummyConstraint<int>(v => v == 3, "equal to 3"))));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(constraintResult.GetResultText()).IsEqualTo("inner was <null>")
			.Because("the subject itself was not null, only the member the nested member is read from");
	}

	[Test]
	public async Task ForMember_WhenTheSourceOfANestedPluralMemberIsNull_ShouldUseThePluralVerb()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, string?>.FromFunc(_ => null, "items "))
			.AddExpectations(inner => inner
					.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
					.AddExpectations(length => length.AddConstraint((_, _)
						=> new DummyConstraint<int>(v => v == 3, "equal to 3"))),
				g => g | ExpectationGrammars.Plural);

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.GetResultText()).IsEqualTo("items were <null>");
	}

	[Test]
	public async Task ForMember_WhenTheSubjectIsNull_ShouldReferToTheSubjectAsIt()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(length => length.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy(null!, null!, CancellationToken.None);

		await That(constraintResult.GetResultText()).IsEqualTo("it was <null>");
	}

	[Test]
	public async Task ForMember_WithAndCombinedExpectations_ShouldApplyAllExpectations()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 3, "equal to 3"))
				.And()
				.AddConstraint((_, _) => new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3 and equal to 2");
	}

	[Test]
	public async Task ForMember_WithFailingExpectation_ShouldReturnFailureConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 2");
	}

	[Test]
	public async Task ForMember_WithOrCombinedExpectations_ShouldApplyEitherExpectation()
	{
		ManualExpectationBuilder<string> sut = new();

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

	[Test]
	public async Task ForMember_WithSucceedingExpectation_ShouldReturnSuccessConstraintResult()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Success);
		await That(constraintResult.GetExpectationText()).IsEqualTo("length equal to 3");
	}

	[Test]
	public async Task ForMember_WithValidation_ShouldIncludeValidation()
	{
		ManualExpectationBuilder<string> sut = new();

		sut.ForMember(MemberAccessor<string, int>.FromFunc(x => x.Length, "length "))
			.Validate((_, _) => new DummyConstraint<string>(_ => false, "validated and "))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 3, "equal to 3")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("validated and length equal to 3");
	}

	[Test]
	public async Task ForWhich_AfterAnd_WhenTheLeftOperandFails_ShouldStillEvaluateTheMember()
	{
		bool isMemberEvaluated = false;
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.And();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 3, "has length 3"));
		sut.ForWhich<string, char>(s => s[0], " whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c =>
		{
			isMemberEvaluated = true;
			return c == 'b';
		}, "is 'b'"));

		ConstraintResult result = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(isMemberEvaluated).IsTrue()
			.Because("the member only continues the right operand, which was met");
		await That(result.GetExpectationText()).IsEqualTo("is foo and has length 3 whose first char is 'b'");
	}

	[Test]
	public async Task ForWhich_AfterOr_WhenCalledTwice_ShouldOnlyContinueTheRightOperand()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));
		sut.ForWhich<string, char>(s => s[0], " and whose first char ");
		sut.AddConstraint((_, _) => new DummyConstraint<char>(c => c == 'f', "is 'f'"));

		ConstraintResult result = await sut.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is empty or is foo whose length is 3 and whose first char is 'f'");
	}

	[Test]
	public async Task ForWhich_AfterOr_WhenTheLeftOperandIsMet_ShouldSucceed()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));

		ConstraintResult result = await sut.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText()).IsEqualTo("is empty or is foo whose length is 3");
	}

	[Test]
	[Arguments("foo", Outcome.Success)]
	[Arguments("bar", Outcome.Failure)]
	[Arguments("fooo", Outcome.Failure)]
	public async Task ForWhich_AfterOr_WhenTheLeftOperandIsNotMet_ShouldDependOnTheRightOperand(
		string subject, Outcome expectedOutcome)
	{
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.StartsWith("foo"), "starts with foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));

		ConstraintResult result = await sut.IsMetBy(subject, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	public async Task ForWhich_AfterOr_WithAMemberExpectationOnTheMember_ShouldOnlyContinueTheRightOperand()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.ForMember(MemberAccessor<int, int>.FromFunc(i => 2 * i, "doubled "))
			.AddExpectations(e => e.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 6, "is 6")));

		ConstraintResult result = await sut.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText()).IsEqualTo("is empty or is foo whose length doubled is 6");
	}

	[Test]
	public async Task ForWhich_AfterOrAndAnd_ShouldOnlyContinueTheRightMostOperand()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s is ['f', ..,], "starts with f"));
		sut.And();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));

		ConstraintResult result = await sut.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText())
			.IsEqualTo("is empty or starts with f and is foo whose length is 3");
	}

	[Test]
	public async Task ForWhich_Async_AfterOr_WhenTheLeftOperandIsMet_ShouldSucceed()
	{
		Func<string, Task<int>> lengthAccessor = s => Task.FromResult(s.Length);
		ManualExpectationBuilder<string> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s.Length == 0, "is empty"));
		sut.Or();
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich(lengthAccessor, " whose length ");
		sut.AddConstraint((_, _) => new DummyConstraint<int>(i => i == 3, "is 3"));

		ConstraintResult result = await sut.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.GetExpectationText()).IsEqualTo("is empty or is foo whose length is 3");
	}

	[Test]
	public async Task ForWhich_Async_CalledTwice_ShouldHonorConstraintsFromAllLevels()
	{
		Func<string, Task<string?>> upperAccessor = s => Task.FromResult<string?>(s.ToUpperInvariant());
		Func<string, Task<string?>> doubledAccessor = s => Task.FromResult<string?>(s + s);
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForWhich_Async_CalledTwice_WhereSecondProjectsFromFirstResult_ShouldChainProjections()
	{
		Func<int, Task<string?>> stringify = i =>
			Task.FromResult<string?>(i.ToString(CultureInfo.InvariantCulture));
		Func<string, Task<string?>> doubled = s => Task.FromResult<string?>(s + s);
		ManualExpectationBuilder<int> sut = new();
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

	[Test]
	public async Task ForWhich_Async_InsideAPluralMember_ShouldReferToTheSingularValueAsIt()
	{
		Func<string, Task<char>> firstChar = s => Task.FromResult(s[0]);
		ManualExpectationBuilder<string> sut = new();
		string? usedIt = null;
		ExpectationGrammars usedExpectationGrammars = ExpectationGrammars.None;

		sut.ForMember(MemberAccessor<string, string>.FromFunc(x => x, "chars "))
			.AddExpectations(expectationBuilder => expectationBuilder
				.ForWhich(firstChar, " whose first ")
				.AddConstraint((it, g) =>
				{
					usedIt = it;
					usedExpectationGrammars = g;
					return new DummyConstraint<char>(c => c == 'b', "is 'b'");
				}), _ => ExpectationGrammars.Plural);

		await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(usedIt).IsEqualTo("it")
			.Because("the value is no longer the member that the enclosing expectation named");
		await That(usedExpectationGrammars).IsEqualTo(ExpectationGrammars.None);
	}

	[Test]
	public async Task ForWhich_CalledThreeTimes_EachProjectionChainsFromPrevious_ShouldEvaluateDeeply()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForWhich_CalledTwice_OuterConstraintFails_ShouldStillEvaluateOuterConstraint()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForWhich_CalledTwice_SecondProjectionFails_ShouldIncludeAllProjectionsInOrder()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForWhich_CalledTwice_ShouldHonorConstraintsFromAllLevels()
	{
		ManualExpectationBuilder<string> sut = new();
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

	[Test]
	public async Task ForWhich_CalledTwice_WhereSecondProjectsFromFirstResult_ShouldChainProjections()
	{
		ManualExpectationBuilder<int> sut = new();
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

	[Test]
	public async Task ForWhich_WithSubjectNameAndExpectationGrammars_ShouldApplyThemToTheMember()
	{
		ManualExpectationBuilder<string> sut = new();
		string? usedIt = null;
		ExpectationGrammars usedExpectationGrammars = ExpectationGrammars.None;
		sut.AddConstraint((_, _) => new DummyConstraint<string>(s => s == "foo", "is foo"));
		sut.ForWhich<string, int>(s => s.Length, " whose length ",
			"the length",
			g => g | ExpectationGrammars.Nested);
		sut.AddConstraint((it, g) =>
		{
			usedIt = it;
			usedExpectationGrammars = g;
			return new DummyConstraint<int>(i => i == 3, "is 3");
		});

		await sut.IsMetBy("foo", null!, CancellationToken.None);

		await That(usedIt).IsEqualTo("the length");
		await That(usedExpectationGrammars).IsEqualTo(ExpectationGrammars.Nested);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task IsMet_WhenEvaluatedAgain_ShouldAwaitTheReleaseOfThePreviousEvaluation()
	{
		SlowlyReleasedAsyncEnumerable source = new(1, 2, 3);
		List<int> disposeCountsAtEvaluation = [];
		ExpectationResult expectation = ThatReadsFirstAsyncItem(source,
			() => disposeCountsAtEvaluation.Add(source.DisposeCount));

		await Expect.ThatAll(expectation, expectation);

		await That(disposeCountsAtEvaluation).IsEqualTo(new[] { 0, 1, })
			.Because("the source of the previous evaluation is released before the next evaluation starts");
	}
#endif

	[Test]
	public async Task IsMet_WhenFailing_ShouldReleaseTheMaterializedSourceAfterTheFailureMessage()
	{
		DisposeTrackingEnumerable source = new(null, Enumerable.Range(1, 20).ToArray());
		ReadsFirstItemConstraint constraint = new(source, Outcome.Failure);

		async Task Act()
			=> await ThatReadsFirstItem(source, constraint);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             reads the first item,
			             but it was [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and maybe more)]
			             """)
			.Because("the failure message still reads from the source");
		await That(constraint.DisposeCountWhenListed).IsEqualTo(0);
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the source is released once the failure message is created");
	}

	[Test]
	public async Task IsMet_WhenSucceeding_ShouldReleaseTheMaterializedSource()
	{
		DisposeTrackingEnumerable source = new(null, 1, 2, 3);
		ReadsFirstItemConstraint constraint = new(source, Outcome.Success);

		await ThatReadsFirstItem(source, constraint);

		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the source that was only read partially is released after the evaluation");
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task IsMet_WhenSucceeding_WhenTheSourceIsReleasedAsynchronously_ShouldAwaitTheRelease()
	{
		SlowlyReleasedAsyncEnumerable source = new(1, 2, 3);

		await ThatReadsFirstAsyncItem(source);

		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the evaluation only ends once the source is released");
	}
#endif

	[Test]
	public async Task WhenAConstraintDoesNotDecideItsOutcome_ShouldFail()
	{
		async Task Act()
			=> await ThatUndecided(1);

		await That(Act).ThrowsExactly<FailException>()
			.WithMessage("""
			             Expected that subject
			             decides nothing,
			             but it could not be verified, because the expectation did not decide its outcome
			             """)
			.Because("an outcome that is left undecided without a cancellation is a mistake of the constraint");
	}

	[Test]
	public async Task WhenAConstraintDoesNotDecideItsOutcome_WithACancellationThatIsNotRequested_ShouldFail()
	{
		using CancellationTokenSource cts = new();

		async Task Act()
			=> await ThatUndecided(1).WithCancellation(cts.Token);

		await That(Act).ThrowsExactly<FailException>()
			.WithMessage("""
			             Expected that subject
			             decides nothing,
			             but it could not be verified, because the expectation did not decide its outcome
			             """)
			.Because("only a requested cancellation leaves the expectation inconclusive");
	}

	[Test]
	public async Task WhenAConstraintStopsAtTheCancellationWithoutDecidingItsOutcome_ShouldBeInconclusive()
	{
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await new ExpectationResult(That(1).Get().ExpectationBuilder
				.AddConstraint((_, _) => new StopsAtTheCancellationConstraint())).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that 1
			             stops at the cancellation,
			             but it could not be verified, because the evaluation was already canceled
			             """)
			.Because("the undecided outcome is explained by the cancellation of the caller");
	}

	[Test]
	public async Task WhenAMemberCannotBeCompared_ShouldNameTheMemberInTheReason()
	{
		Pair subject = new(1, 2);

		async Task Act()
			=> await That(subject).Whose(x => x.Second, second => second.Get().ExpectationBuilder
				.AddConstraint((_, _) => new DescribingConstraint<int>("the second",
					new UserCodeException(new NotComparableException("it was no number", null)))));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Second is described,
			             but Second was no number
			             """)
			.Because("the leading \"it\" of the reason stands for the member that could not be compared");
	}

	[Test]
	public async Task WhenCancellationIsRequestedWhileAConstraintAwaits_ShouldBeInconclusive()
	{
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await ThatAwaiting(1).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it could not be verified, because the evaluation was already canceled
			             """)
			.Because("a requested cancellation leaves the expectation unverified instead of failing it");
	}

	[Test]
	[Arguments("fails", "it was not described")]
	[Arguments("not answered", "for item \"foo\", it was broken")]
	[Arguments("not comparable", "it was no number")]
	[Arguments("throws", "the predicate did throw an InvalidOperationException:\n  boom")]
	[Arguments("throws for an item",
		"for the item at index 2, the predicate did throw an InvalidOperationException:\n  boom")]
	public async Task WhenConstraintDescribesTheSubject_ShouldUseTheDescriptionHoweverTheConstraintFails(
		string failure, string expectedResult)
	{
		Exception? exception = failure switch
		{
			"not answered" => new UnansweredItemException(
				new DummyConstraintResult(Outcome.FailureBothWays, "is valid", "it was broken"), "foo"),
			"not comparable" => new UserCodeException(new NotComparableException("it was no number", null)),
			"throws" => new UserCodeException(new InvalidOperationException("boom"), "the predicate"),
			"throws for an item" => new UserCodeException(new InvalidOperationException("boom"), "the predicate", 2),
			_ => null,
		};

		async Task Act()
			=> await new ExpectationResult(That(1).Get().ExpectationBuilder
				.AddConstraint((_, _) => new DescribingConstraint<int>("the described subject", exception)));

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that the described subject
			              is described,
			              but {expectedResult}
			              """).IgnoringNewlineStyle()
			.Because("the subject is named the same, whether the constraint fails or its evaluation did not complete");
	}

	[Test]
	public async Task WhenSubjectHasMultipleLines_ShouldTrimCommonWhiteSpace()
	{
		async Task Act() => await That(new[]
		{
			1, 2, 3,
		}).IsEmpty();

		await That(Act).Throws<FailException>()
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

	[Test]
	public async Task WhenTimeoutElapsesWhileAConstraintAwaits_ShouldFailWithTheTimeout()
	{
		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("a timeout during the evaluation is reported like a subject that did not finish in time");
	}

	[Test]
	public async Task WhenTypeImplementsIDescribableSubject_AndSubjectIsNull_ShouldUseTheSubjectExpression()
	{
		MyDescribableSubject? subject = null;

		async Task Act() => await That(subject).IsNotNull();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not null,
			             but it was <null>
			             """)
			.Because("a null subject cannot describe itself, so the subject expression is used instead");
	}

	[Test]
	public async Task WhenTypeImplementsIDescribableSubject_ShouldUseToStringFromIt()
	{
		MyDescribableSubject subject = new("this long description for the subject");

		async Task Act() => await That(subject).IsNull();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that this long description for the subject
			             is null,
			             but it was ExpectationBuilderTests.MyDescribableSubject { }
			             """);
	}

	[Test]
	public async Task WithCancellation_TogetherWithTimeout_ShouldApplyBoth()
	{
		using CancellationTokenSource cts = new();

		async Task Act()
			=> await ThatAwaiting(1).WithCancellation(cts.Token).WithTimeout(50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00.050
			             """)
			.Because("a timeout is a different option than the cancellation token");
	}

	[Test]
	public async Task WithCancellation_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		using CancellationTokenSource cts = new();
		ExpectationResult sut = ThatAwaiting(1).WithCancellation(cts.Token);

		void Act() => sut.WithCancellation(System.Threading.CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("WithCancellation cannot be specified more than once.")
			.Because("the second token would silently replace the first one");
	}

	[Test]
	public async Task WithCancellation_WhenSpecifiedTwice_WithTimeoutInBetween_ShouldThrowInvalidOperationException()
	{
		using CancellationTokenSource cts = new();
		ExpectationResult sut = ThatAwaiting(1).WithCancellation(cts.Token).WithTimeout(50.Milliseconds());

		void Act() => sut.WithCancellation(cts.Token);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("WithCancellation cannot be specified more than once.");
	}

	[Test]
	public async Task WithTimeout_WhenALongerTimeoutFollows_ShouldKeepTheShorterTimeout()
	{
		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(50.Milliseconds()).WithTimeout(20.Seconds());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the tighter limit wins, so a later timeout must not loosen an earlier one");
	}

	[Test]
	public async Task WithTimeout_WhenAShorterTimeoutFollows_ShouldUseTheShorterTimeout()
	{
		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(20.Seconds()).WithTimeout(50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
	}

	[Test]
	public async Task WithTimeout_WhenInfinite_AndAShorterTimeoutWasSet_ShouldKeepTheShorterTimeout()
	{
		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(50.Milliseconds()).WithTimeout(Timeout.InfiniteTimeSpan);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("an infinite timeout imposes no limit, so the shorter one still applies");
	}

	[Test]
	public async Task WithTimeout_WhenInfinite_ShouldNotLimitTheEvaluation()
	{
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it could not be verified, because the evaluation was already canceled
			             """)
			.Because("an infinite timeout imposes no limit, so only the cancellation ends the evaluation");
	}

	[Test]
	public async Task WithTimeout_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => ThatAwaiting(1).WithTimeout(-5.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix()
			.Because("the timeout is validated when the expectation is built");
	}

	[Test]
	public async Task WithTimeout_WhenZero_ShouldFailWithTheTimeout()
	{
		async Task Act()
			=> await ThatAwaiting(1).WithTimeout(TimeSpan.Zero);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             awaits,
			             but it did not finish within 0:00
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00."));
	}

	private static ExpectationResult ThatAwaiting(int subject)
		=> new(That(subject).Get().ExpectationBuilder.AddConstraint((_, _) => new AwaitingConstraint()));

	private static ExpectationResult ThatReadsFirstItem(IEnumerable<int> subject,
		ReadsFirstItemConstraint constraint)
		=> new(That(subject).Get().ExpectationBuilder.AddConstraint((_, _) => constraint));

#if NET8_0_OR_GREATER
	private static ExpectationResult ThatReadsFirstAsyncItem(IAsyncEnumerable<int> subject,
		Action? onEvaluation = null)
		=> new(That(subject).Get().ExpectationBuilder.AddConstraint((_, _)
			=> new ReadsFirstAsyncItemConstraint(onEvaluation)));
#endif

	private static ExpectationResult ThatUndecided(int subject)
		=> new(That(subject).Get().ExpectationBuilder.AddConstraint((_, _)
			=> new DummyConstraint("decides nothing",
				() => new DummyConstraintResult(Outcome.Undecided, "decides nothing", "it was 1"))));

	/// <remarks>
	///     It awaits until the evaluation is cancelled, or fails after half a minute, so that a regression fails the
	///     test instead of hanging the test run.
	/// </remarks>
	private sealed class AwaitingConstraint : IAsyncConstraint<int>
	{
		public async ValueTask<ConstraintResult> IsMetBy(int actual, CancellationToken cancellationToken)
		{
			await Task.Delay(30.Seconds(), cancellationToken);
			return new DummyConstraint<int>(_ => false, "awaits").IsMetBy(actual);
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("awaits");
	}

	private sealed class MyDescribableSubject(string subject) : IDescribableSubject
	{
		public string GetDescription()
			=> subject;
	}

	private sealed record Pair(int First, int Second);

#if NET8_0_OR_GREATER
	/// <remarks>
	///     It reads synchronously, so that the evaluation completes synchronously as well.
	/// </remarks>
	private sealed class ReadsFirstAsyncItemConstraint(Action? onEvaluation)
		: IContextConstraint<IAsyncEnumerable<int>>
	{
		public ConstraintResult IsMetBy(IAsyncEnumerable<int> actual, IEvaluationContext context)
		{
			onEvaluation?.Invoke();
			IAsyncEnumerator<int> enumerator = context
				.UseMaterializedAsyncEnumerable(actual, CancellationToken.None)
				.GetAsyncEnumerator();
			_ = enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult();
			return new DummyConstraintResult(Outcome.Success, "reads the first item");
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("reads the first item");
	}

	/// <summary>
	///     Returns the <paramref name="values" /> synchronously, but releases its enumerator asynchronously.
	/// </summary>
	private sealed class SlowlyReleasedAsyncEnumerable(params int[] values) : IAsyncEnumerable<int>
	{
		public int DisposeCount { get; private set; }

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> new Enumerator(this, values);

		private sealed class Enumerator(SlowlyReleasedAsyncEnumerable owner, int[] values) : IAsyncEnumerator<int>
		{
			private int _index = -1;

			public int Current => values[_index];

			public ValueTask<bool> MoveNextAsync()
				=> new(++_index < values.Length);

			public async ValueTask DisposeAsync()
			{
				await Task.Yield();
				owner.DisposeCount++;
			}
		}
	}
#endif

	/// <remarks>
	///     It awaits until the evaluation is cancelled, or fails after half a minute, so that a regression fails the
	///     test instead of hanging the test run.
	/// </remarks>
	private sealed class StopsAtTheCancellationConstraint : IAsyncConstraint<int>
	{
		public async ValueTask<ConstraintResult> IsMetBy(int actual, CancellationToken cancellationToken)
		{
			try
			{
				await Task.Delay(30.Seconds(), cancellationToken);
				return new DummyConstraintResult(Outcome.Failure, "stops at the cancellation", "it was not canceled");
			}
			catch (OperationCanceledException)
			{
				return new DummyConstraintResult(Outcome.Undecided, "stops at the cancellation",
					"it could not be verified, because the evaluation was already canceled");
			}
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("stops at the cancellation");
	}
}

internal static class SubjectContextTestExtensions
{
	/// <summary>
	///     Adds the subject as context with the title <c>Subject</c> to the failure of any expectation on it.
	/// </summary>
	public static IThat<T> WithSubjectContext<T>(this IThat<T> subject)
		=> subject.WithSubjectContext(SubjectCallback<T>.Instance);

	public static IThat<T> WithSubjectContext<T>(this IThat<T> subject,
		Action<T, ResultContextCollector> appendContexts)
	{
		subject.Get().ExpectationBuilder.AddSubjectContexts(appendContexts);
		return subject;
	}

	private static class SubjectCallback<T>
	{
		public static readonly Action<T, ResultContextCollector> Instance = static (value, contexts)
			=> contexts.Add(new ResultContext.SyncCallback("Subject", () => Formatter.Format(value)));
	}
}
