using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.OrResultValueAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class OrResultValueAnalyzerTests
{
	[Test]
	[Arguments("var result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	[Arguments("B result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	[Arguments("B result; result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	[Arguments("var result = await Expect.That(subject).IsExactly<A>().{|#0:Or|}.IsExactly<B>();")]
	[Arguments("var result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>().Because(\"foo\");")]
	[Arguments("var result = (await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>())!;")]
	[Arguments("var result = (object)await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	[Arguments("object result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	[Arguments("var result = (await (Expect.That(subject).Is<A>().{|#0:Or|}).Is<B>());")]
	[Arguments("var name = (await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>()).Name;")]
	[Arguments("var name = (await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>())?.Name;")]
	[Arguments("Consume(await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>());")]
	[Arguments("if (await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>() is null) { }")]
	[Arguments("B[] results = [await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>(),];")]
	[Arguments("Func<Task<B>> callback = async () => await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();")]
	public async Task WhenUsingTheValueAfterAnOr_ShouldBeFlagged(string statement) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using System;
			  using System.Threading.Tasks;
			  using aweXpect;

			  public class A;

			  public class B
			  {
			      public string Name => "";
			  }

			  public class MyClass
			  {
			      public async Task MyTest(object subject)
			      {
			          {{statement}}
			      }

			      private static void Consume(B value)
			      {
			      }
			  }
			  """,
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("B")
		);

	[Test]
	[Arguments("await Expect.That(subject).Is<A>().Or.Is<B>();")]
	[Arguments("_ = await Expect.That(subject).Is<A>().Or.Is<B>();")]
	[Arguments("await Expect.That(subject).Is<A>().Or.Is<B>().Because(\"foo\");")]
	[Arguments("var result = await Expect.That(subject).Is<A>().And.Is<B>();")]
	[Arguments("var result = await Expect.That(subject).Is<B>();")]
	[Arguments("var result = await Expect.That(subject).Is<A>().Or.IsNotNull();")]
	[Arguments("var result = await Expect.That(subject).Is<A>().Or.Is<B>().And.IsNotNull();")]
	[Arguments("Func<Task> callback = async () => await Expect.That(subject).Is<A>().Or.Is<B>();")]
	[Arguments("async Task Act() => await Expect.That(subject).Is<A>().Or.Is<B>();")]
	[Arguments("var result = await Expect.That(subject).Whose(x => x.ToString(), x => x.IsNull().Or.IsEmpty());")]
	public async Task WhenNotUsingAValueThatAnOrMakesUnreliable_ShouldNotBeFlagged(string statement)
		=> await Verifier
			.VerifyAnalyzerAsync(
				$$"""
				  using System;
				  using System.Threading.Tasks;
				  using aweXpect;

				  public class A;

				  public class B;

				  public class MyClass
				  {
				      public async Task MyTest(object subject)
				      {
				          {{statement}}
				      }
				  }
				  """
			);

	[Test]
	public async Task WhenAllAlternativesReturnTheSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			#nullable enable
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? text, Action callback, bool? nullableFlag)
			    {
			        bool notNullFlag = await Expect.That(nullableFlag).IsTrue().Or.IsFalse();
			        Exception exception = await Expect.That(callback).Throws<Exception>()
			            .WithMessage("foo").Or.WithMessage("bar");
			        int value = await Expect.That(5).IsGreaterThan(3).Or.IsLessThan(1);
			        string? result = await Expect.That(text).IsEqualTo("foo").Or.IsEqualTo("bar");
			        bool flag = await Expect.That(true).IsTrue().Or.IsFalse();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenAnAndFollowsTheOr_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class A;

			public class B;

			public class MyClass
			{
			    public async Task MyTest(object subject, IEnumerable<string> items)
			    {
			        B result = await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>().And.Is<B>();
			        string item = await Expect.That(items).IsEmpty().{|#1:Or|}.IsNotEmpty().And.HasSingle();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("string")
		);

	[Test]
	public async Task WhenCombiningMultipleOr_ShouldBeFlaggedOnceAtTheLastOr() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class A;

			public class B;

			public class C;

			public class MyClass
			{
			    public async Task MyTest(object subject)
			    {
			        C result = await Expect.That(subject).Is<A>().Or.Is<B>().{|#0:Or|}.Is<C>();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("C")
		);

	[Test]
	public async Task WhenReturningTheValue_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class A;

			public class B;

			public class MyClass
			{
			    public async Task<B> MyTest(object subject)
			    {
			        return await Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>();
			    }

			    public async Task<B> MyOtherTest(object subject)
			        => await Expect.That(subject).Is<A>().{|#1:Or|}.Is<B>();
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("B")
		);

	[Test]
	public async Task WhenTheLastExpectationReturnsAnItemOfTheCollection_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(IEnumerable<string> items, int[] values)
			    {
			        var item = await Expect.That(items).IsEmpty().{|#0:Or|}.HasSingle();
			        int value = await Expect.That(values).IsEmpty().{|#1:Or|}.HasSingle();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("string"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("int")
		);

	[Test]
	public async Task WhenTheOrFollowsAWhich_ShouldCompareWithTheNewSubject() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(IEnumerable<int> values)
			    {
			        int same = await Expect.That(values).HasSingle().Which.IsGreaterThan(3).Or.IsLessThan(1);
			        int other = await Expect.That(values).IsEmpty().{|#0:Or|}.HasSingle().Which.IsGreaterThan(3);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("int")
		);

	[Test]
	public async Task WhenTheOrIsOfATypeParameter_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public class MyClass
			{
			    public async Task<string> MyTest<TThat>(AndOrResult<int, TThat> result)
			        where TThat : IThat<string>
			    {
			        return await result.Or.IsEqualTo("foo");
			    }
			}
			"""
		);

	[Test]
	public async Task WhenTheValueIsABaseTypeOrAnInterfaceOfTheSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			#nullable enable
			using System;
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(ArgumentException exception, List<int> list, IEnumerable<int> other,
			        ArgumentException? nullableException)
			    {
			        Exception inner = await Expect.That(exception).HasParamName("x").Or.HasInner();
			        IEnumerable<int> items = await Expect.That(list).IsSameAs(other).Or.Contains(1);
			        Exception? nullable = await Expect.That(nullableException).HasParamName("x").Or.HasInner();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenTheValueIsADerivedTypeOfTheSubject_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(Exception exception, IEnumerable<int> items)
			    {
			        ArgumentException derived = await Expect.That(exception).Is<InvalidOperationException>()
			            .{|#0:Or|}.Is<ArgumentException>();
			        List<int> list = await Expect.That(items).Is<HashSet<int>>().{|#1:Or|}.Is<List<int>>();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("ArgumentException"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("List<int>")
		);

	[Test]
	public async Task WhenTheValueIsAValueType_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(object subject)
			    {
			        int value = await Expect.That(subject).Is<string>().{|#0:Or|}.Is<int>();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("int")
		);

	[Test]
	public async Task WhenTheValueIsNotTheOneOfTheExpectation_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Results;

			public class A;

			public class B;

			public static class Extensions
			{
			    public static Task<int> CountAsync<TType, TSelf>(this ExpectationResult<TType, TSelf> result)
			        where TSelf : ExpectationResult<TType, TSelf>
			        => Task.FromResult(1);
			}

			public class MyClass
			{
			    public async Task MyTest(object subject)
			    {
			        int count = await Expect.That(subject).Is<A>().Or.Is<B>().CountAsync();
			        int other = Expect.That(subject).Is<A>().Or.Is<B>().CountAsync().GetAwaiter().GetResult();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingAnOrOfAnotherType_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Runtime.CompilerServices;
			using System.Threading.Tasks;

			public class Awaitable<T>
			{
			    public Awaitable<T> Or => this;

			    public Awaitable<TOther> Is<TOther>() => new Awaitable<TOther>();

			    public Awaitable<T> OrElse() => this;

			    public TaskAwaiter<T> GetAwaiter() => Task.FromResult(default(T)!).GetAwaiter();
			}

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        string text = await new Awaitable<object>().Is<int>().Or.Is<string>();
			        string other = await new Awaitable<string>().OrElse();
			        string result = new Awaitable<object>().Is<int>().Or.Is<string>().GetAwaiter().GetResult();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingTheStaticThat_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System.Threading.Tasks;
			using static aweXpect.Expect;

			public class A;

			public class B;

			public class MyClass
			{
			    public async Task MyTest(object subject, Task<object> task)
			    {
			        B result = await That(subject).Is<A>().{|#0:Or|}.Is<B>();
			        B awaited = await That(task).Is<A>().{|#1:Or|}.Is<B>();
			        B qualified = await aweXpect.Expect.That(subject).Is<A>().{|#2:Or|}.Is<B>();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(2)
				.WithArguments("B")
		);

	[Test]
	public async Task WhenVerifyingSynchronously_ShouldBeFlaggedWhenTheValueIsUsed() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class A;

			public class B;

			public class MyClass
			{
			    public void MyTest(object subject)
			    {
			        B verified = Synchronously.Verify(Expect.That(subject).Is<A>().{|#0:Or|}.Is<B>());
			        B extension = Expect.That(subject).Is<A>().{|#1:Or|}.Is<B>().VerifySynchronously();
			        B awaiter = Expect.That(subject).Is<A>().{|#2:Or|}.Is<B>().GetAwaiter().GetResult();

			        Synchronously.Verify(Expect.That(subject).Is<A>().Or.Is<B>());
			        Expect.That(subject).Is<A>().Or.Is<B>().VerifySynchronously();
			        Expect.That(subject).Is<A>().Or.Is<B>().GetAwaiter().GetResult();
			        _ = Synchronously.Verify(Expect.That(subject).Is<A>().Or.Is<B>());
			        int value = Synchronously.Verify(Expect.That(5).IsGreaterThan(3).Or.IsLessThan(1));
			    }
			}
			""",
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(0)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(1)
				.WithArguments("B"),
			Verifier.Diagnostic(Rules.OrResultValueRule)
				.WithLocation(2)
				.WithArguments("B")
		);
}
