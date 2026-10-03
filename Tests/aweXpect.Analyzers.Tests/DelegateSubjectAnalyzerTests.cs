using System.Threading.Tasks;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.DelegateSubjectAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class DelegateSubjectAnalyzerTests
{
	[Fact]
	public async Task WhenUsingACustomHelperOnADelegateSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect.Core;
			using aweXpect.Delegates;

			internal static class ExpectHelpers
			{
			    public static IExpectThat<T> Get<T>(this IThat<T> subject) => (IExpectThat<T>)subject;
			}

			public static class MyExtensions
			{
			    public static void Inspect(this ThatDelegate.WithValue<int> subject)
			    {
			        _ = subject.Get();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingAHelperReturningAnUnconstrainedTypeParameter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using aweXpect;
			using aweXpect.Core;

			public static class MyExtensions
			{
			    public static TResult Get<TResult>(this IThat<object> subject, TResult result) => result;
			}

			public class MyClass
			{
			    public void MyTest<TResult>(TResult result)
			    {
			        int Act() => 1;

			        _ = Expect.That(Act).Get(result);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenUsingAnExpectationReturningAConstrainedTypeParameter_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public static class MyExpectations
			{
			    public static TResult Satisfies<TResult>(this IThat<object> subject, TResult result)
			        where TResult : Expectation
			        => result;
			}

			public class MyClass
			{
			    public void MyTest<TResult>(TResult result, ExpectationResult concreteResult)
			        where TResult : ExpectationResult
			    {
			        int Act() => 1;

			        _ = Expect.That(Act).{|#0:Satisfies|}(result);
			        _ = Expect.That(Act).{|#1:Satisfies|}(concreteResult);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(0)
				.WithArguments("Satisfies"),
			Verifier.Diagnostic(Rules.DelegateSubjectRule)
				.WithLocation(1)
				.WithArguments("Satisfies")
		);

	[Fact]
	public async Task WhenUsingAnExtensionDeclaredForADelegateSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Delegates;

			public static class MyExtensions
			{
			    public static void Inspect(this IThat<ThatDelegate.WithValue<int>> subject)
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
