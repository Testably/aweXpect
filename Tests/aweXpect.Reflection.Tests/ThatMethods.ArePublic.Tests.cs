using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("InternalMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).ArePublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are public,
					             but it contained not matching items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodInfoIsPublic_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PublicMethod");

				async Task Act()
				{
					await That(subject).ArePublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("InternalMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsPublic_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PublicMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are public,
					             but it only contained public items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
