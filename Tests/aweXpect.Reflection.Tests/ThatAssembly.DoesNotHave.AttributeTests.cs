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
