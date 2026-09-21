using System.Threading.Tasks;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.DelegateSubjectAnalyzer,
		aweXpect.Analyzers.CodeFixers.DelegateSubjectCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class DelegateSubjectCodeFixProviderTests
{
	private const string InsertWhoseResultKey = nameof(Resources.aweXpect0004CodeFixTitle);

	[Fact]
	public async Task ShouldContinueWithTheResultOfTheDelegate() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        int Act() => 1;

		        await Expect.That(Act).[|IsEqualTo|](1);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        int Act() => 1;

		        await Expect.That(Act).DoesNotThrow().WhoseResult.IsEqualTo(1);
		    }
		}
		""",
		InsertWhoseResultKey);

	[Fact]
	public async Task ShouldKeepTheLineBreakBeforeTheExpectation() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        string Act() => "foo";

		        await Expect.That(Act)
		            .[|IsNotNull|]();
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        string Act() => "foo";

		        await Expect.That(Act).DoesNotThrow().WhoseResult
		            .IsNotNull();
		    }
		}
		""",
		InsertWhoseResultKey);

	[Fact]
	public async Task ShouldNotOfferAFixForADelegateWithoutValue() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act()
		        {
		        }

		        await Expect.That(Act).[|IsEqualTo|](1);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act()
		        {
		        }

		        await Expect.That(Act).[|IsEqualTo|](1);
		    }
		}
		""",
		InsertWhoseResultKey);
}
