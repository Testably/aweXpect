using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfoIsInternal_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
				{
					await That(subject).AreInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).AreInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are internal,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodInfoIsInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("InternalMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are internal,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
