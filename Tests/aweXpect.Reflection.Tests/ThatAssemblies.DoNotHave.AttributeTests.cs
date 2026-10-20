using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[TUnit.Core.Test]
			public async Task AsyncEnumerable_WhenAnAssemblyHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subjects).DoNotHave<AssemblyTitleAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have no AssemblyTitleAttribute,
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}
#endif

			[TUnit.Core.Test]
			public async Task Negated_WhenNoAssemblyHasAttribute_ShouldFail()
			{
				IEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<TestAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have no ThatAssemblies.DoNotHave.AttributeTests.TestAttribute,
					             but it only contained matching assemblies

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[TUnit.Core.Test]
			public async Task WhenAnAssemblyHasAttribute_ShouldFail()
			{
				IEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoNotHave<AssemblyTitleAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have no AssemblyTitleAttribute,
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[TUnit.Core.Test]
			public async Task WhenNoAssemblyHasAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(), null,
				};

				async Task Act()
				{
					await That(subjects).DoNotHave<TestAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class TestAttribute : Attribute;
		}
	}
}
