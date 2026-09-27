using System.Threading.Tasks;
using Xunit;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.EqualsAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class EqualsAnalyzerTests
{
	[Fact]
	public async Task WhenOnlyUsingThat_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        #pragma warning disable aweXpect0001
			        IThat<string> source = Expect.That("foo");
			        #pragma warning restore aweXpect0001
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingEquals_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        string subject = "foo";
			        
			        #pragma warning disable aweXpect0001
			        {|#0:Expect.That(subject).Equals("foo")|};
			        #pragma warning restore aweXpect0001
			    }
			}
			""",
			Verifier.Diagnostic(Rules.EqualsRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenUsingEqualsOnACombination_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        #pragma warning disable aweXpect0001
			        {|#0:Expect.ThatAll(Expect.That(true).IsTrue()).Equals(null)|};
			        #pragma warning restore aweXpect0001
			    }
			}
			""",
			Verifier.Diagnostic(Rules.EqualsRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenUsingEqualsOnAnAndOrResult_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        string subject = "foo";

			        #pragma warning disable aweXpect0001
			        {|#0:Expect.That(subject).IsEqualTo("foo").Equals("foo")|};
			        #pragma warning restore aweXpect0001
			    }
			}
			""",
			Verifier.Diagnostic(Rules.EqualsRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenUsingEqualsOnAnExpectation_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        #pragma warning disable aweXpect0001
			        Expectation expectation = Expect.That(true).IsTrue();
			        #pragma warning restore aweXpect0001
			        bool result = {|#0:expectation.Equals(expectation)|};
			    }
			}
			""",
			Verifier.Diagnostic(Rules.EqualsRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenUsingEqualsOnAnotherObject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        #pragma warning disable aweXpect0001
			        Expectation expectation = Expect.That(true).IsTrue();
			        #pragma warning restore aweXpect0001
			        bool result1 = "foo".Equals(expectation);
			        bool result2 = Equals(expectation, expectation);
			    }
			}
			"""
		);
}
