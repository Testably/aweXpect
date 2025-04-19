using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("InternalField");

				async Task Act()
					=> await That(subject).AreInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("InternalField");

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
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
