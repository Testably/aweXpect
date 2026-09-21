using System.Threading.Tasks;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.DelegateSubjectAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class DelegateSubjectAnalyzerTests
{
	[Fact]
	public async Task WhenUsingAnExtensionDeclaredForADelegateSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using aweXpect;
			using aweXpect.Core;

			public static class MyExtensions
			{
			    public static void Inspect(this IThat<aweXpect.Delegates.ThatDelegate.WithValue<int>> subject)
			    {
			    }
			}

			public class MyClass
			{
			    public void MyTest()
			    {
			        int Act() => 1;

			        Expect.That(Act).Inspect();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingDoesNotThrowWhoseResult_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
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
			"""
		);

	[Fact]
	public async Task WhenUsingEventually_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        int Act() => 1;

			        await Expect.That(Act).Eventually().IsEqualTo(1);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingIsEqualToOnADelegateWithoutValue_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
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

			        await Expect.That(Act).{|#0:IsEqualTo|}(1);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("IsEqualTo")
		);

	[Fact]
	public async Task WhenUsingIsEqualToOnAGenericSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public async Task MyTest<T>(IThat<T> subject)
			        where T : class
			    {
			        await subject.IsEqualTo(1);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingIsEqualToOnAnObjectVariableHoldingADelegate_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        object subject = (Func<int>)(() => 1);

			        await Expect.That(subject).IsEqualTo(1);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingIsEqualToOnAPlainSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        int subject = 1;

			        await Expect.That(subject).IsEqualTo(1);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingIsEqualToOnAReturningDelegate_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        int Act() => 1;

			        await Expect.That(Act).{|#0:IsEqualTo|}(1);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("IsEqualTo")
		);

	[Fact]
	public async Task WhenUsingIsNotNullOnAnExplicitlyTypedTask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(Task subject)
			    {
			        await Expect.That<Task>(subject).IsNotNull();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingIsNotNullOnAReturningDelegate_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        string Act() => "foo";

			        await Expect.That(Act).{|#0:IsNotNull|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("IsNotNull")
		);

	[Fact]
	public async Task WhenUsingIsNotNullOnATaskSubject_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(Task subject)
			    {
			        await Expect.That(subject).{|#0:IsNotNull|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("IsNotNull")
		);

	[Fact]
	public async Task WhenUsingIsNotOneOfOnAReturningDelegate_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        int Act() => 1;

			        await Expect.That(Act).{|#0:IsNotOneOf|}(1, 2);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("IsNotOneOf")
		);

	[Fact]
	public async Task WhenUsingThrowsOnADelegate_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        int Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().WithMessage("foo");
			    }
			}
			"""
		);
}
