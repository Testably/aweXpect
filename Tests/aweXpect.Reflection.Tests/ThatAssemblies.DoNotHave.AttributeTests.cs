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
			[Test]
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

			[Test]
			public async Task Negated_WhenNoAssemblyHasAttribute_ShouldFail()
			{
				IEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have no ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
					             but it only contained matching assemblies

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
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

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [Assembly.GetExecutingAssembly(), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<MarkerAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoAssemblyHasAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly?> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoNotHave<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class MarkerAttribute : Attribute;
		}
	}
}
