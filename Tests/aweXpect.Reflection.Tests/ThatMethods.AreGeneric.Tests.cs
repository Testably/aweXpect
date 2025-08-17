using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
					=> await That(subject).AreGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods();

				async Task Act()
					=> await That(subject).AreGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             are all generic,
					             but it contained non-generic methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreGeneric());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that *
					             are not all generic,
					             but it only contained generic methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreGeneric());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
