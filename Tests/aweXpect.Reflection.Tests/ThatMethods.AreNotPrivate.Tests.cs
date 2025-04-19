using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotPrivate
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
					=> await That(subject).AreNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
					=> await That(subject).AreNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private,
					             but it contained private items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PrivateMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
