using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("InternalMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are public,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsPublic_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PublicMethod");

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("InternalMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsPublic_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PublicMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are public,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
