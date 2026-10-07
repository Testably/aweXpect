using System.Threading;
using Microsoft.CodeAnalysis.Testing;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.ValueTaskDelegateAnalyzer,
	aweXpect.Analyzers.CodeFixers.ValueTaskDelegateCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class ValueTaskDelegateCodeFixProviderTests
{
	private const string AsTaskKey = nameof(Resources.aweXpect0007CodeFixTitle);

	[Test]
	public async Task ShouldAppendAsTaskToALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask<int> Act() => new(1);

		        Expect.That([|() => Act()|]);
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
		        ValueTask<int> Act() => new(1);

		        Expect.That(() => Act().AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldAppendAsTaskToALambdaWithCancellationToken() => await VerifyWithoutValueTaskOverloadsAsync(
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

		        Expect.That([|ct => Act(ct)|]);
		    }
		}
		""",
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

		        Expect.That(ct => Act(ct).AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldAppendAsTaskToEachReturnOfABlockLambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That([|() =>
		        {
		            Func<int> nested = () => { return 1; };
		            if (flag)
		            {
		                return Act();
		            }

		            return nested() > 0 ? Act() : default;
		        }|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That(() =>
		        {
		            Func<int> nested = () => { return 1; };
		            if (flag)
		            {
		                return Act().AsTask();
		            }

		            return (nested() > 0 ? Act() : default).AsTask();
		        });
		    }
		}
		""");

	[Test]
	public async Task ShouldAppendAsTaskToEachReturnOfAnAnonymousMethod() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask<int> Act() => new(1);

		        Expect.That([|delegate () { return Act(); }|]);
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
		        ValueTask<int> Act() => new(1);

		        Expect.That(delegate () { return Act().AsTask(); });
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForADelegateReturnedFromAMethod() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        Expect.That([|GetAct()|]);
		    }

		    private static Func<ValueTask> GetAct() => () => default;
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
		        Expect.That([|GetAct()|]);
		    }

		    private static Func<ValueTask> GetAct() => () => default;
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForAnAliasedReturnType() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;
		using Deferred = System.Threading.Tasks.ValueTask;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|Deferred () => Act()|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;
		using Deferred = System.Threading.Tasks.ValueTask;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|Deferred () => Act()|]);
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixForAnAsyncLambdaWithoutReturnType() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        ValueTask Act() => default;

		        await Expect.That<ValueTask>(([|async () => await Act()|])).DoesNotThrow();
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
		        ValueTask Act() => default;

		        await Expect.That<ValueTask>(([|async () => await Act()|])).DoesNotThrow();
		    }
		}
		""",
		AsTaskKey);

	[Test]
	public async Task ShouldNotOfferAFixForAnExplicitTaskTypeArgument() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        Task Act() => Task.CompletedTask;

		        await Expect.That<Task>([|Act|]).DoesNotThrow();
		        await Expect.That<Task>([|() => Act()|]).DoesNotThrow();
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
		        Task Act() => Task.CompletedTask;

		        await Expect.That<Task>([|Act|]).DoesNotThrow();
		        await Expect.That<Task>([|() => Act()|]).DoesNotThrow();
		    }
		}
		""",
		AsTaskKey);

	[Test]
	public async Task ShouldNotOfferAFixForAnExplicitTypeArgument() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        ValueTask Act() => default;

		        await Expect.That<ValueTask>([|Act|]).DoesNotThrow();
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
		        ValueTask Act() => default;

		        await Expect.That<ValueTask>([|Act|]).DoesNotThrow();
		    }
		}
		""",
		AsTaskKey);

	[Test]
	public async Task ShouldNotOfferAFixWhenAReturnedValueHasAnotherAsTask() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public readonly struct Deferred
		{
		    public Task AsTask() => Task.CompletedTask;

		    public static implicit operator ValueTask(Deferred deferred) => default;
		}

		public class MyClass
		{
		    public void MyTest()
		    {
		        Expect.That([|ValueTask () => new Deferred()|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public readonly struct Deferred
		{
		    public Task AsTask() => Task.CompletedTask;

		    public static implicit operator ValueTask(Deferred deferred) => default;
		}

		public class MyClass
		{
		    public void MyTest()
		    {
		        Expect.That([|ValueTask () => new Deferred()|]);
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixWhenAReturnedValueHasNoValueTaskType() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That([|() =>
		        {
		            if (flag)
		            {
		                return Act();
		            }

		            return default;
		        }|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That([|() =>
		        {
		            if (flag)
		            {
		                return Act();
		            }

		            return default;
		        }|]);
		    }
		}
		""");

	[Test]
	public async Task ShouldNotOfferAFixWhenTheParameterNameIsAlreadyUsed() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(CancellationToken token)
		    {
		        ValueTask Act(CancellationToken cancellationToken) => default;

		        Expect.That([|Act|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(CancellationToken token)
		    {
		        ValueTask Act(CancellationToken cancellationToken) => default;

		        Expect.That([|Act|]);
		    }
		}
		""");

	[Test]
	public async Task ShouldParenthesizeTheBodyOfALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That([|() => flag ? Act() : default|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest(bool flag)
		    {
		        ValueTask Act() => default;

		        Expect.That(() => (flag ? Act() : default).AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldReplaceAnAliasQualifiedReturnType() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;
		using Tasks = System.Threading.Tasks;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|Tasks::ValueTask () => Act()|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;
		using Tasks = System.Threading.Tasks;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That(Task () => Act().AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldReplaceOnlyTheReturnTypeOfAnAsyncLambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|async ValueTask () => await Act()|]);
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
		        ValueTask Act() => default;

		        Expect.That(async Task () => await Act());
		    }
		}
		""");

	[Test]
	public async Task ShouldReplaceTheExplicitGenericReturnTypeOfALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask<int> Act() => new(1);

		        Expect.That([|ValueTask<int> () => { return Act(); }|]);
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
		        ValueTask<int> Act() => new(1);

		        Expect.That(Task<int> () => { return Act().AsTask(); });
		    }
		}
		""");

	[Test]
	public async Task ShouldReplaceTheExplicitReturnTypeOfALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|System.Threading.Tasks.ValueTask () => Act()|]);
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
		        ValueTask Act() => default;

		        Expect.That(Task () => Act().AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldWrapAFuncVariableInALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        Func<ValueTask> act = () => default;

		        Expect.That([|act|]);
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
		        Func<ValueTask> act = () => default;

		        Expect.That(() => act().AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldWrapAMemberAccessInALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        Expect.That([|this.Act|]);
		    }

		    private ValueTask Act() => default;
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
		        Expect.That(() => this.Act().AsTask());
		    }

		    private ValueTask Act() => default;
		}
		""");

	[Test]
	public async Task ShouldWrapAMethodGroupInALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|Act|]);
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
		        ValueTask Act() => default;

		        Expect.That(() => Act().AsTask());
		    }
		}
		""");

	[Test]
	public async Task ShouldWrapAMethodGroupWithCancellationTokenInALambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask<int> Act(CancellationToken cancellationToken) => new(1);

		        Expect.That([|Act|]);
		    }
		}
		""",
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask<int> Act(CancellationToken cancellationToken) => new(1);

		        Expect.That(token => Act(token).AsTask());
		    }
		}
		""");

	private static async Task VerifyWithoutValueTaskOverloadsAsync(string source, string fixedSource)
	{
		Verifier.Test test = new()
		{
			TestCode = source,
			FixedCode = fixedSource,
			CodeActionEquivalenceKey = AsTaskKey,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				Sources =
				{
					NetStandard20Expect.Source,
				},
			},
			FixedState =
			{
				Sources =
				{
					NetStandard20Expect.Source,
				},
			},
		};

		await test.RunAsync(CancellationToken.None);
	}
}
