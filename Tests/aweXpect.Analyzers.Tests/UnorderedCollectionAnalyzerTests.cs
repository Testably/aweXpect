using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.UnorderedCollectionAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class UnorderedCollectionAnalyzerTests
{
	[Test]
	public async Task WhenCalledAsStaticMethodWithReorderedNamedArguments_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await ThatEnumerable.{|#0:IsEqualTo|}(expected: new[] { 1, 2, }, subject: Expect.That(subject));
			        await ThatEnumerable.{|#1:StartsWith|}(expected: new[] { 1, 2, }, subject: Expect.That(subject));
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(1)
				.WithArguments("StartsWith", "HashSet<int>")
		);

	[Test]
	public async Task WhenChainedWithAndOnAStoredResult_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        var result = Expect.That(subject).IsNotEmpty();
			        await result.And.{|#0:IsEqualTo|}(new[] { 1, 2, });
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>")
		);

	[Test]
	public async Task WhenChainedWithAndOrOr_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).IsNotEmpty().And.{|#0:IsEqualTo|}(new[] { 1, 2, });
			        await Expect.That(subject).IsEmpty().Or.{|#1:StartsWith|}(1);
			        await Expect.That(subject).IsNotEmpty().And.IsEqualTo(new[] { 1, 2, }).InAnyOrder();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(1)
				.WithArguments("StartsWith", "HashSet<int>")
		);

	[Test]
	[Arguments("Dictionary<string, int>")]
	[Arguments("IDictionary<string, int>")]
	[Arguments("IReadOnlyDictionary<string, int>")]
	[Arguments("ImmutableDictionary<string, int>")]
	[Arguments("Dictionary<string, int>?")]
	public async Task WhenComparingADictionary_ShouldBeFlagged(string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  #nullable enable
			  using System.Collections.Generic;
			  using System.Collections.Immutable;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class MyClass
			  {
			      public async Task MyTest({{type}} subject, Dictionary<string, int> other)
			      {
			          await Expect.That(subject).{|#0:Contains|}(other);
			          await Expect.That(subject).{|#1:DoesNotContain|}(other);
			          await Expect.That(subject).{|#2:IsContainedIn|}(other);
			          await Expect.That(subject).{|#3:IsNotContainedIn|}(other);
			      }
			  }
			  """,
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(2).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(3).WithArguments(type.TrimEnd('?'))
		);

	[Test]
	[Arguments("IDictionary<string, int>")]
	[Arguments("IReadOnlyDictionary<string, int>")]
	public async Task WhenComparingADictionaryForEquality_ShouldNotBeFlagged(string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using System.Collections.Generic;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class MyClass
			  {
			      public async Task MyTest({{type}} subject, Dictionary<string, int> other)
			      {
			          await Expect.That(subject).IsEqualTo(other);
			          await Expect.That(subject).IsNotEqualTo(other);
			      }
			  }
			  """
		);

	[Test]
	public async Task WhenComparingAnItemOfASet_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).Contains(1);
			        await Expect.That(subject).DoesNotContain(1);
			    }
			}
			"""
		);

	[Test]
	[Arguments("List<int>")]
	[Arguments("int[]")]
	[Arguments("IEnumerable<int>")]
	[Arguments("ImmutableArray<int>")]
	[Arguments("SortedSet<int>")]
	[Arguments("ImmutableSortedSet<int>")]
	public async Task WhenComparingAnOrderedCollection_ShouldNotBeFlagged(string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using System.Collections.Generic;
			  using System.Collections.Immutable;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class MyClass
			  {
			      public async Task MyTest({{type}} subject)
			      {
			          await Expect.That(subject).IsEqualTo(new[] { 1, 2, });
			          await Expect.That(subject).Contains(new[] { 1, 2, }).IgnoringInterspersedItems();
			          await Expect.That(subject).IsContainedIn(new[] { 1, 2, });
			          await Expect.That(subject).StartsWith(1);
			          await Expect.That(subject).EndsWith(2);
			      }
			  }
			  """
		);

	[Test]
	[Arguments("HashSet<int>")]
	[Arguments("ISet<int>")]
	[Arguments("IReadOnlySet<int>")]
	[Arguments("ImmutableHashSet<int>")]
	[Arguments("HashSet<int>?")]
	public async Task WhenComparingASet_ShouldBeFlagged(string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  #nullable enable
			  using System.Collections.Generic;
			  using System.Collections.Immutable;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class MyClass
			  {
			      public async Task MyTest({{type}} subject)
			      {
			          await Expect.That(subject).{|#0:IsEqualTo|}(new[] { 1, 2, });
			          await Expect.That(subject).{|#1:IsNotEqualTo|}(new[] { 1, 2, });
			          await Expect.That(subject).{|#2:Contains|}(new[] { 1, 2, });
			          await Expect.That(subject).{|#3:DoesNotContain|}(new[] { 1, 2, });
			          await Expect.That(subject).{|#4:IsContainedIn|}(new[] { 1, 2, });
			          await Expect.That(subject).{|#5:IsNotContainedIn|}(new[] { 1, 2, });
			      }
			  }
			  """,
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(2).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(3).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(4).WithArguments(type.TrimEnd('?')),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(5).WithArguments(type.TrimEnd('?'))
		);

	[Test]
	[Arguments("SortedDictionary<string, int>")]
	[Arguments("ImmutableSortedDictionary<string, int>")]
	[Arguments("SortedList<string, int>")]
	[Arguments("OrderedDictionary<string, int>")]
	public async Task WhenComparingASortedDictionary_ShouldNotBeFlagged(string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using System.Collections.Generic;
			  using System.Collections.Immutable;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class MyClass
			  {
			      public async Task MyTest({{type}} subject, KeyValuePair<string, int>[] other)
			      {
			          await Expect.That(subject).Contains(other).IgnoringInterspersedItems();
			          await Expect.That(subject).IsContainedIn(other);
			      }
			  }
			  """
		);

	[Test]
	public async Task WhenComparingDictionaryKeysOrValues_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(Dictionary<string, int> subject)
			    {
			        await Expect.That(subject.Keys).{|#0:IsEqualTo|}(new[] { "a", });
			        await Expect.That(subject.Values).{|#1:IsEqualTo|}(new[] { 1, });
			        await Expect.That(subject.Keys).IsEqualTo(new[] { "a", }).InAnyOrder();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0)
				.WithArguments("Dictionary<string, int>.KeyCollection"),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1)
				.WithArguments("Dictionary<string, int>.ValueCollection")
		);

	[Test]
	public async Task WhenComparingSortedDictionaryKeysOrValues_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(SortedDictionary<string, int> subject)
			    {
			        await Expect.That(subject.Keys).IsEqualTo(new[] { "a", });
			        await Expect.That(subject.Values).IsEqualTo(new[] { 1, });
			    }
			}
			"""
		);

	[Test]
	public async Task WhenComparingWithAnUnorderedExpectedCollection_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(List<int> subject, HashSet<int> expected, SortedSet<int> sorted)
			    {
			        await Expect.That(subject).{|#0:IsEqualTo|}(expected);
			        await Expect.That(subject).{|#1:Contains|}(expected);
			        await Expect.That(subject).{|#2:IsContainedIn|}(expected);
			        await Expect.That(subject).IsEqualTo(expected).InAnyOrder();
			        await Expect.That(subject).IsEqualTo(sorted);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(2).WithArguments("HashSet<int>")
		);

	[Test]
	public async Task WhenFollowedByAnotherMethod_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using aweXpect;
			using aweXpect.Core;

			public static class MyOptions
			{
			    public static void Log(this Expectation result)
			    {
			    }
			}

			public class MyClass
			{
			    public void MyTest(HashSet<int> subject)
			    {
			        Expect.That(subject).{|#0:IsEqualTo|}(new[] { 1, 2, }).Log();
			        Expect.That(subject).{|#1:IsEqualTo|}(new[] { 1, 2, }).GetAwaiter().GetResult();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1).WithArguments("HashSet<int>")
		);

	[Test]
	public async Task WhenInsideDoesNotComplyWith_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).DoesNotComplyWith(it => it.{|#0:IsEqualTo|}(new[] { 1, 2, }));
			        await Expect.That(subject).DoesNotComplyWith(it => it.IsEqualTo(new[] { 1, 2, }).InAnyOrder());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>")
		);

	[Test]
	public async Task WhenTheSubjectIsATypeParameter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public async Task MyTest<TThat>(TThat subject)
			        where TThat : IThat<HashSet<int>>
			    {
			        await subject.IsEqualTo(new[] { 1, 2, });
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingConditionalAccess_ShouldBeFlaggedOnTheWholeInvocation() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject)?{|#0:.IsEqualTo(new[] { 1, 2, })|};
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>")
		);

	[Test]
	public async Task WhenUsingIgnoringInterspersedItems_AfterAnExtensionMethodOption_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public static class MyOptions
			{
			    public static TResult WithLogging<TResult>(this TResult result)
			        where TResult : Expectation
			        => result;
			}

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject, List<int> list)
			    {
			        await Expect.That(subject).Contains(new[] { 1, 2, }).InAnyOrder().WithLogging().{|#0:IgnoringInterspersedItems|}();
			        await Expect.That(list).Contains(new[] { 1, 2, }).WithLogging().IgnoringInterspersedItems();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(0)
				.WithArguments("IgnoringInterspersedItems", "HashSet<int>")
		);

	[Test]
	public async Task WhenUsingIgnoringInterspersedItems_OnAResultThatIsNotChained_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public static class MyHelpers
			{
			    public static TResult Pass<TResult>(TResult result) => result;
			}

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        var result = Expect.That(subject).Contains(new[] { 1, 2, }).InAnyOrder();
			        await result.IgnoringInterspersedItems();
			        await MyHelpers.Pass(Expect.That(subject).Contains(new[] { 1, 2, }).InAnyOrder())
			            .IgnoringInterspersedItems();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingIgnoringInterspersedItems_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).Contains(new[] { 1, 2, }).InAnyOrder().{|#0:IgnoringInterspersedItems|}();
			        await Expect.That(subject).{|#1:IsContainedIn|}(new[] { 1, 2, }).{|#2:IgnoringInterspersedItems|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(0)
				.WithArguments("IgnoringInterspersedItems", "HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(1).WithArguments("HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(2)
				.WithArguments("IgnoringInterspersedItems", "HashSet<int>")
		);

	[Test]
	public async Task WhenUsingInAnyOrder_AfterAnExtensionMethodOption_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public static class MyOptions
			{
			    public static TResult WithLogging<TResult>(this TResult result)
			        where TResult : Expectation
			        => result;
			}

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).IsEqualTo(new[] { 1, 2, }).WithLogging().InAnyOrder();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingInAnyOrder_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject, Dictionary<string, int> dictionary)
			    {
			        await Expect.That(subject).IsEqualTo(new[] { 1, 2, }).InAnyOrder();
			        await Expect.That(subject).IsNotEqualTo(new[] { 1, 2, }).IgnoringDuplicates().InAnyOrder();
			        await Expect.That(subject).Contains(new[] { 1, 2, })
			            .InAnyOrder();
			        await Expect.That(subject).IsContainedIn(new[] { 1, 2, }).InAnyOrder();
			        await Expect.That(dictionary).Contains(dictionary).InAnyOrder();
			        await Expect.That(dictionary).IsContainedIn(dictionary).InAnyOrder();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingStartsWithOrEndsWith_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(HashSet<int> subject)
			    {
			        await Expect.That(subject).{|#0:StartsWith|}(1);
			        await Expect.That(subject).{|#1:DoesNotStartWith|}(1, 2);
			        await Expect.That(subject).{|#2:EndsWith|}(1);
			        await Expect.That(subject).{|#3:DoesNotEndWith|}(1, 2);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(0)
				.WithArguments("StartsWith", "HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(1)
				.WithArguments("DoesNotStartWith", "HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(2)
				.WithArguments("EndsWith", "HashSet<int>"),
			Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(3)
				.WithArguments("DoesNotEndWith", "HashSet<int>")
		);
}
