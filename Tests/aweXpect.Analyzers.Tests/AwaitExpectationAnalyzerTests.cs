using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.AwaitExpectationAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class AwaitExpectationAnalyzerTests
{
	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenCallingAnotherGetResult_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public static class MyExtensions
			{
			    public static Expectation GetResult(this Expectation expectation) => expectation;
			}

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        {|#0:Expect.That(subject)|}.IsTrue().GetResult();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule)
				.WithLocation(0)
		);

	[Test]
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

	[Test]
	public async Task WhenChainedWithAnExtensionMethodThatContinuesTheExpectation_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public class Quantity<T>(IThat<T> subject)
			{
			    public IThat<T> Items() => subject;
			}

			public static class MyExtensions
			{
			    public static TResult WithLogging<TResult>(this TResult result)
			        => result;

			    public static AndOrResult<T, IThat<T>> Logged<T>(this AndOrResult<T, IThat<T>> result)
			        => result;

			    public static IThat<T> AndAlso<T>(this AndOrResult<T, IThat<T>> result)
			        => result.And;

			    public static Quantity<T> Some<T>(this IThat<T> subject)
			        => new Quantity<T>(subject);
			}

			public class MyClass
			{
			    public void MyTest(string subject, List<int> values)
			    {
			        {|#0:Expect.That(subject)|}.IsNotEmpty().WithLogging();
			        {|#1:Expect.That(subject)|}.IsNotEmpty().Logged();
			        {|#2:Expect.That(subject)|}.IsNotEmpty().AndAlso();
			        {|#3:Expect.That(subject)|}.IsNotEmpty().AndAlso().IsNotEmpty();
			        {|#4:Expect.That(values)|}.HasSingle().Which.IsGreaterThan(1);
			        {|#5:Expect.That(values)|}.IsEqualTo(new[] { 1, 2, }).InAnyOrder();
			        {|#6:Expect.That(values)|}.All().Satisfy(x => x > 0);
			        {|#7:Expect.That(values)|}.All();
			        {|#8:Expect.That(subject)|}.Some().Items().IsNotEmpty();
			        var unused = {|#9:Expect.That(subject)|}.IsNotEmpty().WithLogging();
			        _ = {|#10:Expect.That(subject)|}.IsNotEmpty().WithLogging();
			        MyExtensions.WithLogging({|#11:Expect.That(subject)|}.IsNotEmpty());
			    }
			}
			""",
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(0),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(1),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(2),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(3),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(4),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(5),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(6),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(7),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(8),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(9),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(10),
			Verifier.Diagnostic(Rules.AwaitExpectationRule).WithLocation(11)
		);

	[Test]
	public async Task WhenConsumedByAnExtensionMethodReturningAValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;
			using aweXpect.Results;
			using aweXpect.Synchronous;

			public static class MyExtensions
			{
			    public static T Verified<T, TSelf>(this ExpectationResult<T, TSelf> result)
			        where TSelf : ExpectationResult<T, TSelf>
			        => result.VerifySynchronously();
			}

			public class MyClass
			{
			    public void MyTest(string subject)
			    {
			        Expect.That(subject).IsNotEmpty().Verified();
			        Expect.That(subject).IsNotEmpty().And.IsNotEmpty().Verified();
			        Expect.That(subject).IsNotEmpty().Verified().ToString();
			        _ = Expect.That(subject).IsNotEmpty().Verified();
			        string unused = Expect.That(subject).IsNotEmpty().Verified();
			        MyExtensions.Verified(Expect.That(subject).IsNotEmpty());
			    }
			}
			"""
		);

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenPassedAsArgument_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class Holder
			{
			    public Holder(Expectation expectation)
			    {
			    }

			    public static void Consume(int value, Expectation expectation)
			    {
			    }
			}

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        _ = new Holder(Expect.That(subject).IsTrue());
			        Holder.Consume(1, Expect.That(subject).IsTrue());
			    }
			}
			"""
		);

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenReturnedFromLambda_ToMethodReturningANonGenericTask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public static class Deferred
			{
			    public static Task Run<T>(Func<T> func) => Task.CompletedTask;
			}

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Deferred.Run(() => Expect.That(subject).IsTrue());
			    }
			}
			"""
		);

	[Test]
	public async Task WhenReturnedFromLambda_ToMethodReturningTheTypeParameter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public static class Deferred
			{
			    public static T Pass<T>(Func<T> func) => func();
			}

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        await Deferred.Pass(() => Expect.That(subject).IsTrue());
			    }
			}
			"""
		);

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenReturnedFromLambda_ToTaskRun_WithConfigureAwait_AndStoredInALocal_ShouldNotBeFlagged()
		=> await Verifier
			.VerifyAnalyzerAsync(
				"""
				using System.Threading.Tasks;
				using aweXpect;

				public class MyClass
				{
				    public async Task MyTest()
				    {
				        var subject = true;
				        var awaitable = Task.Run(() => Expect.That(subject).IsTrue()).ConfigureAwait(false);
				        await await awaitable;
				    }
				}
				"""
			);

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenUsedAsConditionOfAConditionalExpression_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        _ = (bool)(object)Expect.That(subject).IsTrue() ? 1 : 2;
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsedAsSwitchExpressionGuard_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(int value)
			    {
			        var subject = true;
			        _ = value switch
			        {
			            1 when (bool)(object)Expect.That(subject).IsTrue() => 1,
			            _ => 2,
			        };
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsedAsSwitchExpressionValue_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        var subject = true;
			        _ = Expect.That(subject).IsTrue() switch
			        {
			            _ => 1,
			        };
			    }
			}
			"""
		);

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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
