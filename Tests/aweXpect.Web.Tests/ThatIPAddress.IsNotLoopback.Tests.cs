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
					.WithMessage("""*whose Addresses are not a loopback address for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			[Arguments("127.0.0.1")]
			[Arguments("127.0.0.2")]
			[Arguments("::1")]
			[Arguments("::ffff:127.0.0.1")]
			[Arguments("::ffff:127.0.0.2")]
			public async Task WhenSubjectIsLoopback_ShouldFail(string address)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not a loopback address,
					              but it was {address}
					              """);
			}

			[Test]
			[Arguments("2001:db8::1")]
			[Arguments("::ffff:10.0.0.1")]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed(string address)
			{
				IPAddress subject = IPAddress.Parse(address);

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
			[Arguments("127.0.0.1")]
			[Arguments("::ffff:127.0.0.2")]
			public async Task WhenSubjectIsLoopback_ShouldSucceed(string address)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("10.0.0.1")]
			[Arguments("::ffff:10.0.0.1")]
			public async Task WhenSubjectIsNotLoopback_ShouldFail(string address)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a loopback address,
					              but it was {address}
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
