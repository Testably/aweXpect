using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotHaveAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<AssemblyTitleAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute,
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Test]
			public async Task WithPredicate_WhenAssemblyDoesNotHaveAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<MarkerAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenAssemblyHasMatchingAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute matching attr => attr.Title.Contains("Reflection"),
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[Test]
			public async Task WithPredicate_WhenAssemblyHasNotMatchingAttribute_ShouldSucceed()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				async Task Act()
				{
					await That(subject).DoesNotHave<AssemblyTitleAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[Test]
			public async Task WithType_WhenAssemblyHasAttribute_ShouldFail()
			{
				Assembly subject = Assembly.GetExecutingAssembly();

				Type attributeType = typeof(AssemblyTitleAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no AssemblyTitleAttribute,
					             but it did in aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class MarkerAttribute : Attribute;
		}
	}
}
