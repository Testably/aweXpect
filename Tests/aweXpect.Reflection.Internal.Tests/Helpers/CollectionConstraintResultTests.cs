using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class CollectionConstraintResultTests
{
	[Fact]
	public async Task AppendResult_WhenOutcomeIsDecided_ShouldNotAppendCancelledMessage()
	{
		TestableResult sut = new(ExpectationGrammars.None);
		sut.SetMatch([1,], []);
		StringBuilder stringBuilder = new();

		sut.AppendResult(stringBuilder);

		await That(stringBuilder.ToString()).IsEqualTo("normal-result");
	}

	[Fact]
	public async Task AppendResult_WhenOutcomeIsUndecided_ShouldAppendCancelledMessage()
	{
		TestableResult sut = new(ExpectationGrammars.None);
		StringBuilder stringBuilder = new();

		sut.AppendResult(stringBuilder);

		await That(stringBuilder.ToString())
			.IsEqualTo("it could not be verified, because the evaluation was already canceled");
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task AppendResult_WhenSubjectIsNull_ShouldFailBothWays(bool isNegated)
	{
		TestableResult sut = new(ExpectationGrammars.None);
		if (isNegated)
		{
			sut.Negate();
		}

		sut.SetNull();
		StringBuilder stringBuilder = new();

		sut.AppendResult(stringBuilder);

		await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(stringBuilder.ToString()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task Matching_BeforeSettingValue_ShouldBeEmpty()
	{
		TestableResult sut = new(ExpectationGrammars.None);

		await That(sut.MatchingElements).IsEmpty();
		await That(sut.NotMatchingElements).IsEmpty();
	}

	[Fact]
	public async Task TryGetStoredValue_ForTheElementType_ShouldReturnFalse()
	{
		TestableResult sut = new(ExpectationGrammars.None);
		sut.SetNull();

		bool result = sut.TryGetStoredValue(out int _);

		await That(result).IsFalse().Because("the stored value is the collection, not an element of it");
	}

	[Fact]
	public async Task TryGetStoredValue_ForTheSubjectType_ShouldReturnTrue()
	{
		TestableResult sut = new(ExpectationGrammars.None);
		sut.SetNull();

		bool result = sut.TryGetStoredValue(out IEnumerable<int>? value);

		await That(result).IsTrue();
		await That(value).IsNull();
	}

	private sealed class TestableResult(ExpectationGrammars grammars)
		: CollectionConstraintResult<int>("it", grammars)
	{
		public int[] MatchingElements => Matching;
		public int[] NotMatchingElements => NotMatching;

		public void SetMatch(IEnumerable<int> matching, IEnumerable<int> notMatching)
		{
			List<int> all = new(matching);
			HashSet<int> notMatchingSet = new(notMatching);
			all.AddRange(notMatchingSet);
			SetValue(all, new EvaluationContextStub(), item => !notMatchingSet.Contains(item));
		}

		public void SetNull()
			=> SetValue(null, new EvaluationContextStub(), _ => true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("normal-expectation");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("normal-result");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("negated-expectation");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("negated-result");
	}

	private sealed class EvaluationContextStub : IEvaluationContext
	{
		public EvaluationCancellation Cancellation => EvaluationCancellation.None;

		public void Store<T>(string key, T value)
		{
		}

		public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
		{
			value = default;
			return false;
		}
	}
}
