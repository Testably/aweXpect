using System.Threading.Tasks;
using Xunit;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.AwaitExpectationAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class AwaitExpectationAnalyzerTests
{
	[Fact]
	public async Task WhenAssignedToLocal_ThatIsAwaited_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        var expectation = Expect.That(subject).IsTrue();
			        await expectation;
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAssignedToLocal_ThatIsNeverUsed_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        var expectation = {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAssignedToLocal_ThatIsVerified_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        var expectation = Expect.That(subject).IsTrue();
			        expectation.VerifySynchronously();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaited_InConditionalExpression_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(bool condition)
			    {
			        await (condition ? Expect.That(true).IsTrue() : Expect.That(false).IsTrue());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaited_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaited_WithoutReturnValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        await {|#0:Expect.That(() => {})|}.DoesNotThrow();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaitedInAsyncLambdaReturningTask_InAsyncVoidMethod_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async void MyHandler(object sender, EventArgs e)
			    {
			        Func<bool, Task> check = async subject => await Expect.That(subject).IsTrue();
			        await check(true);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaitedInAsyncLambdaReturningTask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Func<bool, Task> check = async subject => await Expect.That(subject).IsTrue();
			        await check(true);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenAwaitedInAsyncVoidLambda_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        List<bool> subjects = new();
			        subjects.ForEach(async subject => await {|#0:Expect.That(subject)|}.IsTrue());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AsyncVoidExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAwaitedInAsyncVoidLocalFunction_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        Check();
			        await Task.Yield();

			        async void Check() => await {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AsyncVoidExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAwaitedInAsyncVoidMethod_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using aweXpect;

			public class MyClass
			{
			    public async void MyHandler(object sender, EventArgs e)
			    {
			        var subject = true;
			        await {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AsyncVoidExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAwaitedInAsyncVoidMethod_WithExpressionBody_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;

			public class MyClass
			{
			    public async void MyMethod(bool subject)
			        => await {|#0:Expect.That(subject)|}.IsTrue();
			}
			""",
			Verifier.Diagnostic(Rules.AsyncVoidExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAwaitedInAsyncVoidPartialMethod_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;

			public partial class MyClass
			{
			    partial void MyMethod(bool subject);

			    async partial void MyMethod(bool subject)
			        => await {|#0:Expect.That(subject)|}.IsTrue();
			}
			""",
			Verifier.Diagnostic(Rules.AsyncVoidExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenAwaitedInLocalFunctionReturningTask_InAsyncVoidMethod_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async void MyHandler(object sender, EventArgs e)
			    {
			        await Check(true);

			        async Task Check(bool subject) => await Expect.That(subject).IsTrue();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenChainedWithAnd_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.That(subject)|}.IsTrue().And.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenDiscarded_InConditionalExpression_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(bool condition)
			    {
			        _ = condition ? {|#0:Expect.That(true)|}.IsTrue() : {|#1:Expect.That(false)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0),
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(1)
		);

	[Fact]
	public async Task WhenDiscarded_InSwitchExpression_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(int value)
			    {
			        _ = value switch
			        {
			            1 => {|#0:Expect.That(true)|}.IsTrue(),
			            _ => {|#1:Expect.That(false)|}.IsTrue(),
			        };
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0),
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(1)
		);

	[Fact]
	public async Task WhenDiscarded_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        _ = {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenEvaluatedThroughTheAwaiter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        Expect.That(subject).IsTrue().GetAwaiter().GetResult();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenNotAwaited_InOneBranch_WithVerifyInTheOtherBranch_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        if (subject)
			            {|#0:Expect.That(subject)|}.IsTrue();
			        else
			            Synchronously.Verify(Expect.That(subject).IsTrue());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenNotAwaited_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.That(subject)|}.IsTrue();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenNotAwaited_WithoutReturnValue_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        {|#0:Expect.That(() => {})|}.DoesNotThrow();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenNotAwaited_WithoutReturnValue_WithVerifyInMethod_ShouldStillBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        aweXpect.Synchronous.Synchronously.Verify(Expect.That(true).IsTrue());
			        {|#0:Expect.That(() => {})|}.DoesNotThrow();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenOnlyGetAwaiterIsCalled_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        var subject = false;
			        {|#0:Expect.That(subject)|}.IsTrue().GetAwaiter();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedFromAsyncLambda_ToTaskRun_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Task.Run(async () => {|#0:Expect.That(subject)|}.IsTrue());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedFromAsyncLambda_WithReturnStatement_ToTaskRun_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Task.Run(async () =>
			        {
			            await Task.Yield();
			            return {|#0:Expect.That(subject)|}.IsTrue();
			        });
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedFromExpressionBodiedMethod_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public Expectation MyExpectation(bool subject)
			        => Expect.That(subject).IsTrue();
			}
			"""
		);

	[Fact]
	public async Task WhenReturnedFromLambda_AndAwaitedAgain_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await await Task.Run(() => Expect.That(subject).IsTrue());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenReturnedFromLambda_AndAwaitedAgain_WithConfigureAwait_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await await Task.Run(() => Expect.That(subject).IsTrue()).ConfigureAwait(false);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenReturnedFromLambda_AsDelegate_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Func<bool, AndOrResult<bool, IThat<bool>>> check = subject => Expect.That(subject).IsTrue();
			        await check(true);
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenReturnedFromLambda_ToTaskRun_AndTaskIsAwaitedLater_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        var task = Task.Run(() => Expect.That(subject).IsTrue());
			        await task;
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenReturnedFromLambda_ToTaskRun_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Task.Run(() => {|#0:Expect.That(subject)|}.IsTrue());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedFromLambda_ToTaskRun_WithConfigureAwait_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Task.Run(() => {|#0:Expect.That(subject)|}.IsTrue()).ConfigureAwait(false);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedFromLambda_WithReturnStatement_ToTaskRun_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Task.Run(() =>
			        {
			            return {|#0:Expect.That(subject)|}.IsTrue();
			        });
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenReturnedWithReturnStatement_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;
			using aweXpect.Core;

			public class MyClass
			{
			    public Expectation MyExpectation(bool subject)
			    {
			        return Expect.That(subject).IsTrue();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenThatAllIsAwaited_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Expect.ThatAll(Expect.That(subject).IsTrue(), Expect.That(subject).IsTrue());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenThatAllIsEvaluatedThroughTheAwaiter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        Expect.ThatAll(Expect.That(subject).IsTrue(), Expect.That(subject).IsTrue()).GetAwaiter().GetResult();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenThatAllIsNotAwaited_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.ThatAll(Expect.That(subject).IsTrue(), Expect.That(subject).IsTrue())|};
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenThatAnyIsNotAwaited_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.ThatAny(Expect.That(subject).IsTrue(), Expect.That(subject).IsTrue())|};
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Fact]
	public async Task WhenVerified_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.That(subject)|}.IsTrue().VerifySynchronously();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenVerified_WithoutReturnValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        {|#0:Expect.That(() => {})|}.DoesNotThrow().VerifySynchronously();
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenVerifiedInVoidMethod_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public void MyMethod(bool subject)
			        => Expect.That(subject).IsTrue().VerifySynchronously();
			}
			"""
		);

	[Fact]
	public async Task WhenVerifiedStatically_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        Synchronously.Verify({|#0:Expect.That(subject)|}.IsTrue());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenVerifiedStatically_WithoutReturnValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Synchronously.Verify({|#0:Expect.That(() => {})|}.DoesNotThrow());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenVerifiedWithStaticUsing_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;
			using static aweXpect.Synchronous.Synchronously;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        Verify({|#0:Expect.That(subject)|}.IsTrue());
			    }
			}
			"""
		);

	[Fact]
	public async Task WhenVerifiedWithStaticUsing_WithoutReturnValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Synchronous;
			using static aweXpect.Synchronous.Synchronously;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Verify({|#0:Expect.That(() => {})|}.DoesNotThrow());
			    }
			}
			"""
		);
}
