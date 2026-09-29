using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.AwaitExpectationAnalyzer,
	aweXpect.Analyzers.CodeFixers.AwaitExpectationCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class AwaitExpectationCodeFixProviderTests
{
	[Fact]
	public async Task ShouldAddAsyncBeforeThePartialModifier() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public partial class MyClass
		{
		    public partial Task MyTest();

		    public partial Task MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return Task.CompletedTask;
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public partial class MyClass
		{
		    public partial Task MyTest();

		    public async partial Task MyTest()
		    {
		        var subject = true;
		        await Expect.That(subject).IsTrue();
		    }
		}
		""");

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
	public async Task ShouldAwaitAGenericTaskReturnedFromTheMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public ValueTask<int> MyTest(bool flag)
		    {
		        {|aweXpect0001:Expect.That(flag)|}.IsTrue();
		        return flag ? GetValue() : default;
		    }

		    private ValueTask<int> GetValue() => new(1);
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async ValueTask<int> MyTest(bool flag)
		    {
		        await Expect.That(flag).IsTrue();
		        return await (flag ? GetValue() : default);
		    }

		    private ValueTask<int> GetValue() => new(1);
		}
		""");

	[Fact]
	public async Task ShouldAwaitTheDiscardedExpectation() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        var subject = true;
		        _ = {|aweXpect0001:Expect.That(subject)|}.IsTrue();
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
	public async Task ShouldAwaitTheExpectationInsteadOfAssigningItToAnUnusedLocal() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        var subject = true;
		        var result = {|aweXpect0001:Expect.That(subject)|}.IsTrue();
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
	public async Task ShouldAwaitTheExpectationInTopLevelStatements()
	{
		Verifier.Test test = new()
		{
			TestCode = """
			           using aweXpect;

			           var subject = true;
			           {|aweXpect0001:Expect.That(subject)|}.IsTrue();
			           """,
			FixedCode = """
			            using aweXpect;

			            var subject = true;
			            await Expect.That(subject).IsTrue();
			            """,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				OutputKind = OutputKind.ConsoleApplication,
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
			},
		};

		await test.RunAsync(CancellationToken.None);
	}

	[Fact]
	public async Task ShouldFixAllExpectationsInTheSameMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        {|aweXpect0001:Expect.That(subject)|}.IsNotEqualTo(false);
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
		        await Expect.That(subject).IsNotEqualTo(false);
		    }
		}
		""");

	[Fact]
	public async Task ShouldFixAllExpectationsInTheSameTaskMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest()
		    {
		        var subject = true;
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        {|aweXpect0001:Expect.That(subject)|}.IsNotEqualTo(false);
		        return Task.CompletedTask;
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
		        await Expect.That(subject).IsNotEqualTo(false);
		    }
		}
		""");

	[Fact]
	public async Task ShouldKeepCommentsWhenChangingTheReturnType() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    /// <summary>
		    ///     Some documentation.
		    /// </summary>
		    // Some comment
		    int MyTest()
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
		    /// <summary>
		    ///     Some documentation.
		    /// </summary>
		    // Some comment
		    async Task<int> MyTest()
		    {
		        var subject = true;
		        await Expect.That(subject).IsTrue();
		        return 0;
		    }
		}
		""");

	[Fact]
	public async Task ShouldKeepCommentsWhenRemovingTheFinalReturn() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        // Some comment
		        return Task.CompletedTask;
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(bool subject)
		    {
		        await Expect.That(subject).IsTrue();
		        // Some comment
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

	[Fact]
	public async Task ShouldMakeExpressionBodiedMethodAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool subject) => {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(bool subject) => await Expect.That(subject).IsTrue();
		}
		""");

	[Fact]
	public async Task ShouldMakeTheLocalFunctionAsyncInsteadOfTheContainingMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        static void Verify(bool value)
		        {
		            {|aweXpect0001:Expect.That(value)|}.IsTrue();
		        }
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        static async Task Verify(bool value)
		        {
		            await Expect.That(value).IsTrue();
		        }
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotChangeTheContainingMethodOfAnAsyncLambda() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        Func<Task> act = async () =>
		        {
		            {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        };
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        var subject = true;
		        Func<Task> act = async () =>
		        {
		            await Expect.That(subject).IsTrue();
		        };
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixForANonAsyncLambda() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Collections.Generic;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(List<int> list, bool subject)
		    {
		        list.ForEach(i => { {|aweXpect0001:Expect.That(i)|}.IsPositive(); });
		        Action act = () => {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        Action anonymous = delegate { {|aweXpect0001:Expect.That(subject)|}.IsTrue(); };
		    }
		}
		""",
		"""
		using System;
		using System.Collections.Generic;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(List<int> list, bool subject)
		    {
		        list.ForEach(i => { {|aweXpect0001:Expect.That(i)|}.IsPositive(); });
		        Action act = () => {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        Action anonymous = delegate { {|aweXpect0001:Expect.That(subject)|}.IsTrue(); };
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixForAPartialVoidMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public partial class MyClass
		{
		    partial void MyTest(bool subject);

		    partial void MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""",
		"""
		using aweXpect;

		public partial class MyClass
		{
		    partial void MyTest(bool subject);

		    partial void MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixForATaskMethodReturningAnotherTask() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return Task.Delay(1);
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return Task.Delay(1);
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixInALockStatement() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public class MyClass
		{
		    private readonly object _lock = new();

		    public void MyTest(bool subject)
		    {
		        lock (_lock)
		        {
		            {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        }
		    }
		}
		""",
		"""
		using aweXpect;

		public class MyClass
		{
		    private readonly object _lock = new();

		    public void MyTest(bool subject)
		    {
		        lock (_lock)
		        {
		            {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        }
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixInMembersThatCannotBeAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Collections.Generic;
		using aweXpect;

		public class MyClass
		{
		    public MyClass(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    ~MyClass()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }

		    public bool Property
		    {
		        get
		        {
		            {|aweXpect0001:Expect.That(true)|}.IsTrue();
		            return true;
		        }
		    }

		    public static MyClass operator +(MyClass left, MyClass right)
		    {
		        {|aweXpect0001:Expect.That(left)|}.IsNotNull();
		        return left;
		    }

		    public void WithRefParameter(ref bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public void WithSpanParameter(Span<int> span)
		    {
		        {|aweXpect0001:Expect.That(span.Length)|}.IsPositive();
		    }

		    public IEnumerable<int> Iterator(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        yield return 1;
		    }
		}
		""",
		"""
		using System;
		using System.Collections.Generic;
		using aweXpect;

		public class MyClass
		{
		    public MyClass(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    ~MyClass()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }

		    public bool Property
		    {
		        get
		        {
		            {|aweXpect0001:Expect.That(true)|}.IsTrue();
		            return true;
		        }
		    }

		    public static MyClass operator +(MyClass left, MyClass right)
		    {
		        {|aweXpect0001:Expect.That(left)|}.IsNotNull();
		        return left;
		    }

		    public void WithRefParameter(ref bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public void WithSpanParameter(Span<int> span)
		    {
		        {|aweXpect0001:Expect.That(span.Length)|}.IsPositive();
		    }

		    public IEnumerable<int> Iterator(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        yield return 1;
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotOfferAFixWhenTheReturnTypeIsDictatedByABaseType() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using aweXpect;

		public abstract class Base
		{
		    public abstract void Verify(bool subject);
		}

		public class MyClass : Base, IDisposable
		{
		    public override void Verify(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public virtual void VerifyVirtual(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public void Dispose()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""",
		"""
		using System;
		using aweXpect;

		public abstract class Base
		{
		    public abstract void Verify(bool subject);
		}

		public class MyClass : Base, IDisposable
		{
		    public override void Verify(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public virtual void VerifyVirtual(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }

		    public void Dispose()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""");

	[Fact]
	public async Task ShouldNotWrapAQualifiedGenericTaskAgain() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public class MyClass
		{
		    public System.Threading.Tasks.Task<int> MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return System.Threading.Tasks.Task.FromResult(1);
		    }
		}
		""",
		"""
		using aweXpect;

		public class MyClass
		{
		    public async System.Threading.Tasks.Task<int> MyTest(bool subject)
		    {
		        await Expect.That(subject).IsTrue();
		        return 1;
		    }
		}
		""");

	[Fact]
	public async Task ShouldQualifyTaskWhenItsNamespaceIsNotImported() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""",
		"""
		using aweXpect;

		public class MyClass
		{
		    public async System.Threading.Tasks.Task MyTest(bool subject)
		    {
		        await Expect.That(subject).IsTrue();
		    }
		}
		""");

	[Fact]
	public async Task ShouldReplaceCompletedTaskReturnsWhenMakingTheMethodAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public ValueTask MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        if (subject)
		        {
		            return ValueTask.CompletedTask;
		        }

		        return ValueTask.CompletedTask;
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async ValueTask MyTest(bool subject)
		    {
		        await Expect.That(subject).IsTrue();
		        if (subject)
		        {
		            return;
		        }
		    }
		}
		""");
}
