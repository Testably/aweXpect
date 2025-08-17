using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("InternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateProtectedField");

				async Task Act()
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private protected,
					             but it contained private protected items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("InternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateProtectedField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
