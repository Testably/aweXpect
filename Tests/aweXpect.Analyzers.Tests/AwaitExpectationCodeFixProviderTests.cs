using System.Threading.Tasks;
using Xunit;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.AwaitExpectationAnalyzer,
	aweXpect.Analyzers.CodeFixers.AwaitExpectationCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class AwaitExpectationCodeFixProviderTests
{
	[Fact]
	public async Task ShouldApplyCodeFix() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        var subject = true;
		        await Expect.That(subject).IsTrue();
		    }
		}
		""");

	[Fact]
	public async Task ShouldKeepReturnValueInContainingMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public int MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return 0;
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task<int> MyTest()
		    {
		        var subject = true;
		        await Expect.That(subject).IsTrue();
		        return 0;
		    }
		}
		""");

	[Fact]
	public async Task ShouldMakeContainingMethodAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        var subject = true;
		        await Expect.That(subject).IsTrue();
		    }
		}
		""");
}
