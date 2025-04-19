using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
					=> await That(subject).AreInternal();

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
					=> await That(subject).AreInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are internal,
					             but all were
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
