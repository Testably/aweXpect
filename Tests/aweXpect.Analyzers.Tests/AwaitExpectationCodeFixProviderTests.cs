using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.AwaitExpectationAnalyzer,
	aweXpect.Analyzers.CodeFixers.AwaitExpectationCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class AwaitExpectationCodeFixProviderTests
{
	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task ShouldAwaitEachKindOfReturnedGenericTask() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    private readonly Task<int> _task = Task.FromResult(1);

		    public Task<int> MyTest(int value, Task<int> task)
		    {
		        {|aweXpect0001:Expect.That(value)|}.IsPositive();
		        if (value == 1)
		        {
		            return task;
		        }

		        if (value == 2)
		        {
		            return (task);
		        }

		        if (value == 3)
		        {
		            return this._task;
		        }

		        return GetValue();
		    }

		    private Task<int> GetValue() => Task.FromResult(1);
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    private readonly Task<int> _task = Task.FromResult(1);

		    public async Task<int> MyTest(int value, Task<int> task)
		    {
		        await Expect.That(value).IsPositive();
		        if (value == 1)
		        {
		            return await task;
		        }

		        if (value == 2)
		        {
		            return await (task);
		        }

		        if (value == 3)
		        {
		            return await this._task;
		        }

		        return await GetValue();
		    }

		    private Task<int> GetValue() => Task.FromResult(1);
		}
		""");

	[Test]
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

	[Test]
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

	[Test]
	public async Task ShouldAwaitTheExpectationReturnedFromAnAsyncLambda() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(bool subject)
		    {
		        await Task.Run(async () => {|aweXpect0001:Expect.That(subject)|}.IsTrue());
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
		        await Task.Run(async () => await Expect.That(subject).IsTrue());
		    }
		}
		""");

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task ShouldMakeAMethodAsyncThatImplementsNoInterfaceMember() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public interface IMarker
		{
		    static int Version => 1;
		}

		public class MyClass : IMarker
		{
		    public void MyTest()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public interface IMarker
		{
		    static int Version => 1;
		}

		public class MyClass : IMarker
		{
		    public async Task MyTest()
		    {
		        await Expect.That(true).IsTrue();
		    }
		}
		""");

	[Test]
	public async Task ShouldMakeAReferencedTaskMethodAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest()
		    {
		        Func<bool, Task> verify = Verify;
		        return verify(true);
		    }

		    private static Task Verify(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return Task.CompletedTask;
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task MyTest()
		    {
		        Func<bool, Task> verify = Verify;
		        return verify(true);
		    }

		    private static async Task Verify(bool subject)
		    {
		        await Expect.That(subject).IsTrue();
		    }
		}
		""");

	[Test]
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

	[Test]
	public async Task ShouldMakeExpressionBodiedLocalFunctionAsync() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        static void Verify(bool value) => {|aweXpect0001:Expect.That(value)|}.IsTrue();
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
		        static async Task Verify(bool value) => await Expect.That(value).IsTrue();
		    }
		}
		""");

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task ShouldNotOfferAFixForABareGetAwaiter() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue().GetAwaiter();
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
		        {|aweXpect0001:Expect.That(true)|}.IsTrue().GetAwaiter();
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForACalledLocalFunction() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        Verify(true);

		        static void Verify(bool value)
		        {
		            {|aweXpect0001:Expect.That(value)|}.IsTrue();
		        }
		    }
		}
		""",
		"""
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        Verify(true);

		        static void Verify(bool value)
		        {
		            {|aweXpect0001:Expect.That(value)|}.IsTrue();
		        }
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForAConditionalMethod() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Diagnostics;
		using aweXpect;

		public class MyClass
		{
		    [Conditional("DEBUG")]
		    public void MyTest()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""",
		"""
		using System.Diagnostics;
		using aweXpect;

		public class MyClass
		{
		    [Conditional("DEBUG")]
		    public void MyTest()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForAMethodCalledInAnotherFile()
	{
		const string caller = """
		                      public partial class MyClass
		                      {
		                          public void MyTest()
		                          {
		                              Verify(true);
		                          }
		                      }
		                      """;
		const string method = """
		                      using aweXpect;

		                      public partial class MyClass
		                      {
		                          private static void Verify(bool subject)
		                          {
		                              {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		                          }
		                      }
		                      """;
		Verifier.Test test = new()
		{
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				Sources =
				{
					caller,
					method,
				},
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
			},
			FixedState =
			{
				Sources =
				{
					caller,
					method,
				},
			},
		};

		await test.RunAsync(CancellationToken.None);
	}

	[Test]
	public async Task ShouldNotOfferAFixForAMethodCalledSynchronously() => await Verifier.VerifyCodeFixAsync(
		"""
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool subject)
		    {
		        Verify(subject);
		    }

		    private static void Verify(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""",
		"""
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool subject)
		    {
		        Verify(subject);
		    }

		    private static void Verify(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForAMethodThatImplementsAnInterfaceOfADerivedClass()
	{
		const string source = """
		                      using aweXpect;

		                      public interface IRunner
		                      {
		                          void Run();
		                      }

		                      public class Base
		                      {
		                          public void Run()
		                          {
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                          }
		                      }
		                      """;
		const string derived = """
		                       public class Derived : Base, IRunner
		                       {
		                       }
		                       """;
		Verifier.Test test = new()
		{
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				Sources =
				{
					source,
					derived,
				},
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
			},
			FixedState =
			{
				Sources =
				{
					source,
					derived,
				},
			},
		};

		await test.RunAsync(CancellationToken.None);
	}

	[Test]
	public async Task ShouldNotOfferAFixForAMethodUsedAsAMethodGroup() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using aweXpect;

		public class MyClass
		{
		    public event Action Changed;

		    public void MyTest()
		    {
		        Changed += Verify;
		    }

		    public void Verify()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""",
		"""
		using System;
		using aweXpect;

		public class MyClass
		{
		    public event Action Changed;

		    public void MyTest()
		    {
		        Changed += Verify;
		    }

		    public void Verify()
		    {
		        {|aweXpect0001:Expect.That(true)|}.IsTrue();
		    }
		}
		""");

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task ShouldNotOfferAFixForATaskMethodReturningNull() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task<int> MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return null;
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public Task<int> MyTest(bool subject)
		    {
		        {|aweXpect0001:Expect.That(subject)|}.IsTrue();
		        return null;
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForATernaryExpression() => await Verifier.VerifyCodeFixAsync(
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(bool condition)
		    {
		        _ = condition ? {|aweXpect0001:Expect.That(true)|}.IsTrue() : {|aweXpect0001:Expect.That(false)|}.IsTrue();
		    }
		}
		""",
		"""
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest(bool condition)
		    {
		        _ = condition ? {|aweXpect0001:Expect.That(true)|}.IsTrue() : {|aweXpect0001:Expect.That(false)|}.IsTrue();
		    }
		}
		""");

	[Test]
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

	[Test]
	public async Task ShouldNotOfferAFixInFunctionsWithRefLocals()
	{
		// A `ref` or `ref struct` local can't be preserved across an `await`.
		const string source = """
		                      using System;
		                      using System.Threading.Tasks;
		                      using aweXpect;

		                      public class MyClass
		                      {
		                          private int _field;

		                          public void SpanLocal()
		                          {
		                              Span<int> span = stackalloc int[1];
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              span[0] = 1;
		                          }

		                          public async Task SpanLocalInAsyncMethod()
		                          {
		                              Span<int> span = stackalloc int[1];
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              span[0] = 1;
		                          }

		                          public void RefLocal()
		                          {
		                              ref int alias = ref _field;
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              alias = 1;
		                          }

		                          public void ForEachOverStackalloc()
		                          {
		                              foreach (int value in stackalloc int[] { 1, 2, })
		                              {
		                                  {|aweXpect0001:Expect.That(value)|}.IsPositive();
		                              }
		                          }

		                          public void OutVariable()
		                          {
		                              Create(out Span<int> span);
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              span[0] = 1;
		                          }

		                          private static void Create(out Span<int> span) => span = default;
		                      }
		                      """;
		await VerifyNoCodeFixAsync(source, LanguageVersion.Preview);
	}

	[Test]
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

	[Test]
	public async Task ShouldNotOfferAFixInUnsafeCode()
	{
		const string source = """
		                      using System.Threading.Tasks;
		                      using aweXpect;

		                      public class MyClass
		                      {
		                          public unsafe void UnsafeMethod()
		                          {
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                          }

		                          public void UnsafeBlock()
		                          {
		                              unsafe
		                              {
		                                  {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              }
		                          }

		                          public async Task UnsafeBlockInAsyncMethod()
		                          {
		                              unsafe
		                              {
		                                  {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                              }
		                          }

		                          public static unsafe void PointerParameter(int* pointer)
		                          {
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                          }
		                      }

		                      public unsafe class UnsafeClass
		                      {
		                          public void Method()
		                          {
		                              {|aweXpect0001:Expect.That(true)|}.IsTrue();
		                          }
		                      }
		                      """;
		await VerifyNoCodeFixAsync(source, LanguageVersion.Preview);
	}

	[Test]
	public async Task ShouldNotOfferAFixWhenARefStructLocalIsDeclaredBeforeCSharp13()
		=> await VerifyNoCodeFixAsync(
			"""
			using System;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        Span<int> span = stackalloc int[1];
			        span[0] = 1;
			        {|aweXpect0001:Expect.That(true)|}.IsTrue();
			    }
			}
			""",
			LanguageVersion.CSharp12);

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	private static async Task VerifyNoCodeFixAsync(string source, LanguageVersion languageVersion)
	{
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
		test.SolutionTransforms.Add((solution, projectId) =>
		{
			Project project = solution.GetProject(projectId)!;
			return solution
				.WithProjectCompilationOptions(projectId,
					((CSharpCompilationOptions)project.CompilationOptions!).WithAllowUnsafe(true))
				.WithProjectParseOptions(projectId,
					((CSharpParseOptions)project.ParseOptions!).WithLanguageVersion(languageVersion));
		});

		await test.RunAsync(CancellationToken.None);
	}
}
