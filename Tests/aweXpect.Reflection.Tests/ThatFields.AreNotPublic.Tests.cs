using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPublic
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
					=> await That(subject).AreNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PublicField");

				async Task Act()
					=> await That(subject).AreNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not public,
					             but it contained public items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("InternalField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is public,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PublicField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
