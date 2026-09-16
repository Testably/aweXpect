using System.Threading.Tasks;
using Xunit;
using Verifier =
	aweXpect.Analyzers.Tests.Verifiers.CSharpCodeFixVerifier<aweXpect.Analyzers.ThrownExceptionVocabularyAnalyzer,
		aweXpect.Analyzers.CodeFixers.ThrownExceptionVocabularyCodeFixProvider>;

namespace aweXpect.Analyzers.Tests;

public class ThrownExceptionVocabularyCodeFixProviderTests
{
	private const string ReplaceKey = nameof(Resources.aweXpect0003ReplaceCodeFixTitle);
	private const string InsertWhichKey = nameof(Resources.aweXpect0003InsertWhichCodeFixTitle);

	[Fact]
	public async Task ShouldReplaceHasMessageWithWithMessage() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().[|HasMessage|]("foo");
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
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().WithMessage("foo");
		    }
		}
		""",
		ReplaceKey);

	[Fact]
	public async Task ShouldReplaceGenericHasInnerWithWithInner() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act() => throw new Exception("foo", new ArgumentException("bar"));

		        await Expect.That(Act).Throws().[|HasInner<ArgumentException>|](inner => inner.HasMessage("bar"));
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
		        void Act() => throw new Exception("foo", new ArgumentException("bar"));

		        await Expect.That(Act).Throws().WithInner<ArgumentException>(inner => inner.HasMessage("bar"));
		    }
		}
		""",
		ReplaceKey);

	[Fact]
	public async Task ShouldInsertWhich() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().[|HasMessage|]("foo");
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
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().Which.HasMessage("foo");
		    }
		}
		""",
		InsertWhichKey);

	[Fact]
	public async Task ShouldInsertWhichBeforeLineBreak() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>()
		            .[|HasMessage|]("foo");
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
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().Which
		            .HasMessage("foo");
		    }
		}
		""",
		InsertWhichKey);

	[Fact]
	public async Task WhenNoTwinWithSameArityExists_ShouldOnlyOfferInsertWhich() => await Verifier.VerifyCodeFixAsync(
		"""
		using System;
		using System.Threading.Tasks;
		using aweXpect;

		public class MyClass
		{
		    public async Task MyTest()
		    {
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().[|HasHResult|]().EqualTo(42);
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
		        void Act() => throw new Exception("foo");

		        await Expect.That(Act).Throws<Exception>().Which.HasHResult().EqualTo(42);
		    }
		}
		""");
}
