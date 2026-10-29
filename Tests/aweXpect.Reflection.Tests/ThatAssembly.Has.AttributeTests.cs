using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatAssembly.Has.AttributeTests.MarkerAttribute,
					             but it did not in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyHasAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).Has<AssemblyTitleAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasMatchingAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).Has<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasNotMatchingAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).Has<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute matching attr => attr.Title == "NonExistentTitle",
					             but it did not in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).Has<AssemblyTitleAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has AssemblyTitleAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenCheckingTheResult_ShouldNotOfferDirectly()
			{
				async Task Act()
				{
					await That(typeof(Reflection.Results.HasAttributeWithoutInheritResult<,>))
						.ContainsMethods(methods => methods.WithName("Directly")).Never();
				}

				await That(Act).DoesNotThrow()
					.Because("the attributes of assemblies, constructors and fields cannot be inherited");
			}

			[Test]
			public async Task WithType_WhenAssemblyHasAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				Type attributeType = typeof(AssemblyTitleAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class MarkerAttribute : Attribute;
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.Has<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<AssemblyTitleAttribute>().OrHas<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute or ThatAssembly.Has.NegatedTests.MarkerAttribute,
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class MarkerAttribute : Attribute;
		}
	}
}
