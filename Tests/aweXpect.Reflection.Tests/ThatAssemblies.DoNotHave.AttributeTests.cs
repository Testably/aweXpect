using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class DoNotHave
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
					             none have AssemblyTitleAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
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
					             at least one has ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
					             but none of 1 did

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
					             none have AssemblyTitleAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
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
					             none have ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
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
					             at least one has ThatAssemblies.DoNotHave.AttributeTests.MarkerAttribute,
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
