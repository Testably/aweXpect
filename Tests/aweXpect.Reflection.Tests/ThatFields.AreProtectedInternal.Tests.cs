using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

				async Task Act()
					=> await That(subject).AreProtectedInternal();

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
					=> await That(subject).AreProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected internal,
					             but all were
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
