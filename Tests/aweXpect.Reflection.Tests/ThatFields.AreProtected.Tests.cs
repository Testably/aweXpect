using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreProtected
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
					=> await That(subject).AreProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

				async Task Act()
					=> await That(subject).AreProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
