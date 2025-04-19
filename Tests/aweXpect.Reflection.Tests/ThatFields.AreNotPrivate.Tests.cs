using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPrivate
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
					=> await That(subject).AreNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPrivate_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateField");

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
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
