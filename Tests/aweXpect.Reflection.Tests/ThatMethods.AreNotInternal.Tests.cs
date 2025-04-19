using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not internal,
					             but it contained internal items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is internal,
					             but none were
					             """).AsWildcard();
			}
		}
	}
}
