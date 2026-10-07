using System.Threading;
using Microsoft.CodeAnalysis.Testing;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.ValueTaskDelegateAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class ValueTaskDelegateAnalyzerTests
{
	[Test]
	public async Task WhenCallingAThatMethodOfAnotherClass_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;

			public static class Other
			{
			    public static void That<TValue>(Func<TValue> @delegate)
			    {
			    }
			}

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask Act() => default;

			        Other.That<ValueTask>(Act);
			    }
			}
			"""
		);

	[Test]
	public async Task WhenExplicitlyTypingTheResultAsValueTask_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        ValueTask Act() => default;

			        await Expect.That<ValueTask>({|#0:Act|}).DoesNotThrow();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask")
		);

	[Test]
	public async Task WhenPassingAFuncOfValueTaskVariable_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        Func<ValueTask> act = () => default;

			        Expect.That({|#0:act|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask")
		);

	[Test]
	public async Task WhenPassingALambdaReturningAnotherTypeNamedValueTask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;

			namespace Tasks
			{
			    public class ValueTask { }
			}

			namespace Other.Tasks
			{
			    public class ValueTask<T1, T2> { }
			}

			namespace Other.Threading.Tasks
			{
			    public class ValueTask { }
			}

			namespace Other.System.Threading.Tasks
			{
			    public class ValueTask { }
			}

			public class ValueTask { }

			public class MyClass
			{
			    public void MyTest()
			    {
			        Expect.That(() => new ValueTask());
			        Expect.That(() => new Tasks.ValueTask());
			        Expect.That(() => new Other.Tasks.ValueTask<int, int>());
			        Expect.That(() => new Other.Threading.Tasks.ValueTask());
			        Expect.That(() => new Other.System.Threading.Tasks.ValueTask());
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingALambdaReturningAsTask_WithoutValueTaskOverloads_ShouldNotBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask Act() => default;

			        Expect.That(() => Act().AsTask());
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingALambdaReturningAValueTaskOfInt_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask<int> Act() => new(1);

			        Expect.That({|#0:() => Act()|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask<int>")
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAnInt_ShouldNotBeFlagged() => await Verifier
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

			        await Expect.That(Act).DoesNotThrow();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAnInt_WithoutValueTaskOverloads_ShouldNotBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        int Act() => 1;

			        Expect.That(Act);
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningATask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Task Act() => Task.CompletedTask;

			        await Expect.That(Act).DoesNotThrow();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningATask_WithoutValueTaskOverloads_ShouldNotBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        Task Act() => Task.CompletedTask;

			        Expect.That(Act);
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAValueTask_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        ValueTask Act() => default;

			        await Expect.That(Act).DoesNotThrow();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAValueTask_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask Act() => default;

			        Expect.That({|#0:Act|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask")
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAValueTaskOfInt_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        ValueTask<int> Act() => new(1);

			        await Expect.That(Act).DoesNotThrow();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenPassingAMethodGroupReturningAValueTaskOfInt_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask<int> Act() => new(1);

			        Expect.That({|#0:Act|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask<int>")
		);

	[Test]
	public async Task WhenPassingAMethodGroupWithCancellationTokenReturningAValueTask_ShouldNotBeFlagged()
		=> await Verifier
			.VerifyAnalyzerAsync(
				"""
				using System;
				using System.Threading;
				using System.Threading.Tasks;
				using aweXpect;

				public class MyClass
				{
				    public async Task MyTest()
				    {
				        ValueTask Act(CancellationToken token) => default;

				        await Expect.That(Act).DoesNotThrow();
				    }
				}
				"""
			);

	[Test]
	public async Task
		WhenPassingAMethodGroupWithCancellationTokenReturningAValueTask_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask Act(CancellationToken token) => default;

			        Expect.That({|#0:Act|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask")
		);

	[Test]
	public async Task
		WhenPassingAMethodGroupWithCancellationTokenReturningAValueTaskOfInt_WithoutValueTaskOverloads_ShouldBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask<int> Act(CancellationToken token) => new(1);

			        Expect.That({|#0:Act|});
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ValueTaskDelegateRule)
				.WithLocation(0)
				.WithArguments("ValueTask<int>")
		);

	[Test]
	public async Task WhenPassingAValueTask_WithoutValueTaskOverloads_ShouldNotBeFlagged()
		=> await VerifyWithoutValueTaskOverloadsAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public void MyTest()
			    {
			        ValueTask Act() => default;

			        Expect.That(Act());
			    }
			}
			"""
		);

	private static async Task VerifyWithoutValueTaskOverloadsAsync(string source, params DiagnosticResult[] expected)
	{
		Verifier.Test test = new()
		{
			TestCode = source,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				Sources =
				{
					NetStandard20Expect.Source,
				},
			},
		};

		test.ExpectedDiagnostics.AddRange(expected);
		await test.RunAsync(CancellationToken.None);
	}
}
