using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected,
					             but it contained protected items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
