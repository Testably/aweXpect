using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsIPv4
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.IPv6Loopback,];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsIPv4()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Addresses are IPv4 addresses for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsIPv4_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsIPv4();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsIPv4MappedToIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsIPv4();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an IPv4 address,
					             but it was ::ffff:10.0.0.1
					             """)
					.Because("an IPv4-mapped address belongs to the IPv6 address family");
			}

			[Test]
			public async Task WhenSubjectIsIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).IsIPv4();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an IPv4 address,
					             but it was 2001:db8::1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsIPv4();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an IPv4 address,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsIPv4());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an IPv4 address,
					             but it was 10.0.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsIPv4());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsIPv4());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an IPv4 address,
					             but it was <null>
					             """);
			}
		}
	}
}
