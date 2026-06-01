using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<TestAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyHasAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<AssemblyTitleAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute,
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class TestAttribute : Attribute;
		}
	}
}
