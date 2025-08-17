using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

				async Task Act()
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected internal,
					             but it contained protected internal items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none were
					             """).AsWildcard();
			}
		}
	}
}
