using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.UnorderedCollectionAnalyzer,
		aweXpect.Analyzers.CodeFixers.UnorderedCollectionCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class UnorderedCollectionCodeFixProviderTests
{
	private const string AppendInAnyOrderKey = nameof(Resources.aweXpect0006CodeFixTitle);

	[Theory]
	[InlineData("IsEqualTo")]
	[InlineData("IsNotEqualTo")]
	[InlineData("Contains")]
	[InlineData("DoesNotContain")]
	[InlineData("IsContainedIn")]
	[InlineData("IsNotContainedIn")]
	public async Task ShouldAppendInAnyOrder(string method) => await Verifier.VerifyCodeFixAsync(
		$$"""
		  using System.Collections.Generic;
		  using System.Threading.Tasks;
		  using aweXpect;

		  public class MyClass
		  {
		      public async Task MyTest(HashSet<int> subject)
		      {
		          await Expect.That(subject).{|#0:{{method}}|}(new[] { 1, 2, });
		      }
		  }
		  """,
		[Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),],
		$$"""
		  using System.Collections.Generic;
		  using System.Threading.Tasks;
		  using aweXpect;

		  public class MyClass
		  {
		      public async Task MyTest(HashSet<int> subject)
		      {
		          await Expect.That(subject).{{method}}(new[] { 1, 2, }).InAnyOrder();
		      }
		  }
		  """,
		AppendInAnyOrderKey);

	[Fact]
	public async Task ShouldAppendInAnyOrderBeforeOtherOptions() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(HashSet<int> subject)
		    {
		        await Expect.That(subject).{|#0:IsEqualTo|}(new[] { 1, 2, }).IgnoringDuplicates();
		    }
		}
		""",
		[Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),],
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(HashSet<int> subject)
		    {
		        await Expect.That(subject).IsEqualTo(new[] { 1, 2, }).InAnyOrder().IgnoringDuplicates();
		    }
		}
		""",
		AppendInAnyOrderKey);

	[Fact]
	public async Task ShouldAppendInAnyOrderForADictionary() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(Dictionary<string, int> subject, Dictionary<string, int> other)
		    {
		        await Expect.That(subject).{|#0:Contains|}(other);
		    }
		}
		""",
		[
			Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0)
				.WithArguments("Dictionary<string, int>"),
		],
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(Dictionary<string, int> subject, Dictionary<string, int> other)
		    {
		        await Expect.That(subject).Contains(other).InAnyOrder();
		    }
		}
		""",
		AppendInAnyOrderKey);

	[Fact]
	public async Task ShouldKeepTheLineBreakBeforeOtherOptions() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(HashSet<int> subject)
		    {
		        await Expect.That(subject).{|#0:IsEqualTo|}(new[] { 1, 2, })
		            .IgnoringDuplicates();
		    }
		}
		""",
		[Verifier.Diagnostic(Rules.UnorderedCollectionRule).WithLocation(0).WithArguments("HashSet<int>"),],
		"""
		using System.Collections.Generic;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(HashSet<int> subject)
		    {
		        await Expect.That(subject).IsEqualTo(new[] { 1, 2, }).InAnyOrder()
		            .IgnoringDuplicates();
		    }
		}
		""",
		AppendInAnyOrderKey);

	[Fact]
	public async Task ShouldNotOfferAFixForStartsWith()
	{
		const string source = """
		                      using System.Collections.Generic;
		                      using System.Threading.Tasks;
		                      using aweXpect;

		                      public class MyClass
		                      {
		                          public async Task MyTest(HashSet<int> subject)
		                          {
		                              await Expect.That(subject).{|#0:StartsWith|}(1);
		                          }
		                      }
		                      """;
		DiagnosticResult expected = Verifier.Diagnostic(Rules.UnorderedCollectionNoMeaningRule).WithLocation(0)
			.WithArguments("StartsWith", "HashSet<int>");
		Verifier.Test test = new()
		{
			TestCode = source,
			FixedCode = source,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
			},
		};
		test.TestState.ExpectedDiagnostics.Add(expected);
		test.FixedState.ExpectedDiagnostics.Add(expected);

		await test.RunAsync(CancellationToken.None);
	}
}