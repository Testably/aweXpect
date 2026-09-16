using System.Threading.Tasks;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.ThrownExceptionVocabularyAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class ThrownExceptionVocabularyAnalyzerTests
{
	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task WhenUsingHasMessageInsideWithInnerException_ShouldNotBeFlagged() => await Verifier
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
			            .WithInnerException(inner => inner.HasMessage("bar"));
			    }
			}
			"""
		);

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task WhenUsingHasHResultWithoutArgumentOnThrows_ShouldBeFlaggedWithoutTwin() => await Verifier
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
			Verifier.Diagnostic(Rules.ThrownExceptionVocabularyWithoutTwinRule)
				.WithLocation(0)
				.WithArguments("HasHResult")
		);

	[Fact]
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
}
