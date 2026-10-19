using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsNotLoopback
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Loopback,];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsNotLoopback()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Addresses are not loopback addresses for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsLoopback_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("127.0.0.1");

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a loopback address,
					             but it was 127.0.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a loopback address,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsLoopback_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("127.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNotLoopback_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a loopback address,
					             but it was 10.0.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a loopback address,
					             but it was <null>
					             """);
			}
		}
	}
}
