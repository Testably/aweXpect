using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class ArePrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPrivate_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateField");

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
