using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ArePrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are private,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are private,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
