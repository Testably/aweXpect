using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             are all not generic,
					             but it contained generic methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("NonGenericMethod");

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("NonGenericMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that *
					             also contain a generic method,
					             but it only contained non-generic methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
