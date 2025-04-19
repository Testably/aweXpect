using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("InternalField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PublicField");

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("InternalField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PublicField");

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
