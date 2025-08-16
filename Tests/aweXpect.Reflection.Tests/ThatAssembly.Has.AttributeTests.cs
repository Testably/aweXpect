using System;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAssemblyHasAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).Has<AssemblyTitleAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyHasMatchingAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).Has<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatAssembly.Has.AttributeTests.TestAttribute,
					             but it did not in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyHasNotMatchingAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).Has<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute matching attr => attr.Title == "NonExistentTitle",
					             but it did not in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
					=> await That(subject).Has<AssemblyTitleAttribute>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute,
					             but it was <null>
					             """);
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class TestAttribute : Attribute;
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Has<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyHasAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<AssemblyTitleAttribute>().OrHas<TestAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute or ThatAssembly.Has.NegatedTests.TestAttribute,
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class TestAttribute : Attribute;
		}
	}
}
