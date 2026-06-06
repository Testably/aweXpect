using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
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
			.IsEqualTo("could not verify, because it was already cancelled");
	}

	[Fact]
	public async Task Matching_BeforeSettingValue_ShouldBeEmpty()
	{
		TestableResult sut = new(ExpectationGrammars.None);

		await That(sut.MatchingElements).IsEmpty();
		await That(sut.NotMatchingElements).IsEmpty();
	}

	private sealed class TestableResult(ExpectationGrammars grammars)
		: CollectionConstraintResult<int>(grammars)
	{
		public int[] MatchingElements => Matching;
		public int[] NotMatchingElements => NotMatching;

		public void SetMatch(IEnumerable<int> matching, IEnumerable<int> notMatching)
		{
			List<int> all = new(matching);
			HashSet<int> notMatchingSet = new(notMatching);
			all.AddRange(notMatchingSet);
			SetValue(all, item => !notMatchingSet.Contains(item));
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("normal-expectation");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("normal-result");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("negated-expectation");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("negated-result");
	}
}
