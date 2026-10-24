using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are protected,
					             but none of * were

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
				{
					await That(subject).AreProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are protected,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
