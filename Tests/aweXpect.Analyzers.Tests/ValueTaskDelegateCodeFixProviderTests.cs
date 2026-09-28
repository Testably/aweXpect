using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.ValueTaskDelegateAnalyzer,
		aweXpect.Analyzers.CodeFixers.ValueTaskDelegateCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class ValueTaskDelegateCodeFixProviderTests
{
	private const string AsTaskKey = nameof(Resources.aweXpect0007CodeFixTitle);

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task ShouldNotOfferAFixForABlockLambda() => await VerifyWithoutValueTaskOverloadsAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public void MyTest()
		    {
		        ValueTask Act() => default;

		        Expect.That([|() => { return Act(); }|]);
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

		        Expect.That([|() => { return Act(); }|]);
		    }
		}
		""");

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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
