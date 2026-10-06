using System.Threading.Tasks;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.ThrownExceptionVocabularyAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class ThrownExceptionVocabularyAnalyzerTests
{
	[Test]
	public async Task WhenCalledAsStaticMethod_ShouldBeFlaggedOnTheWholeInvocation() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await {|#0:ThatException.HasMessage(Expect.That(Act).Throws<Exception>(), "foo")|};
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasMessage", "WithMessage")
		);

	[Test]
	public async Task WhenUsingDoesNotHaveInnerAfterWhich_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().Which.DoesNotHaveInner();
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingDoesNotHaveInnerOnThrows_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws().{|#0:DoesNotHaveInner|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("DoesNotHaveInner", "WithoutInner")
		);

	[Test]
	public async Task WhenUsingDoesNotHaveInnerOnThrows_ShouldBeFlaggedOnTheGenericName() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws().{|#0:DoesNotHaveInner<ArgumentException>|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("DoesNotHaveInner", "WithoutInner")
		);

	[Test]
	public async Task WhenUsingHasHResultWithArgumentOnThrows_ShouldBeFlaggedWithTwin() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().{|#0:HasHResult|}(42);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasHResult", "WithHResult")
		);

	[Test]
	public async Task WhenUsingHasHResultWithoutArgumentOnThrows_ShouldBeFlaggedWithTwin() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().{|#0:HasHResult|}().EqualTo(42);
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasHResult", "WithHResult")
		);

	[Test]
	public async Task WhenUsingHasInnerOnThrows_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo", new ArgumentException("bar"));

			        await Expect.That(Act).Throws().{|#0:HasInner|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasInner", "WithInner")
		);

	[Test]
	public async Task WhenUsingHasInnerOnThrows_ShouldBeFlaggedOnTheGenericName() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo", new ArgumentException("bar"));

			        await Expect.That(Act).Throws().{|#0:HasInner<ArgumentException>|}();
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasInner", "WithInner")
		);

	[Test]
	public async Task WhenUsingHasMessageAfterWhich_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().Which.HasMessage("foo");
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingHasMessageInsideWithInner_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo", new Exception("bar"));

			        await Expect.That(Act).Throws<Exception>()
			            .WithInner(inner => inner.HasMessage("bar"));
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingHasMessageOnExceptionSubject_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        Exception subject = new Exception("foo");

			        await Expect.That(subject).HasMessage("foo");
			    }
			}
			"""
		);

	[Test]
	public async Task WhenUsingHasMessageOnThrows_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().{|#0:HasMessage|}("foo");
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasMessage", "WithMessage")
		);

	[Test]
	public async Task WhenUsingHasMessageOnThrowsExactly_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).ThrowsExactly<Exception>().{|#0:HasMessage|}().Containing("oo");
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasMessage", "WithMessage")
		);

	[Test]
	public async Task WhenUsingHasParamNameAfterAnd_ShouldBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new ArgumentException("foo", "bar");

			        await Expect.That(Act).Throws<ArgumentException>()
			            .WithMessage("foo").And
			            .{|#0:HasParamName|}("bar");
			    }
			}
			""",
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyRule)
				.WithLocation(0)
				.WithArguments("HasParamName", "WithParamName")
		);

	[Test]
	public async Task WhenUsingWithMessageOnThrows_ShouldNotBeFlagged() => await Verifier
		.VerifyAnalyzerAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest()
			    {
			        void Act() => throw new Exception("foo");

			        await Expect.That(Act).Throws<Exception>().WithMessage("foo");
			    }
			}
			"""
		);
}
