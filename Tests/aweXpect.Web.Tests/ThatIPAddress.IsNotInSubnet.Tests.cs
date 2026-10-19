using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsNotInSubnet
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Parse("10.0.0.1"),];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsNotInSubnet("10.0.0.0/8")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Addresses are not in subnet 10.0.0.0/8 for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubnetIsNull_ShouldThrowArgumentNullException()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotInSubnet(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("subnet").And
					.WithMessage("The 'subnet' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("10.0.0.0")]
			[Arguments("10.0.0.0/33")]
			[Arguments("10.0.0.1/8")]
			[Arguments("2001:db8::/129")]
			public async Task WhenSubnetIsNoValidCidrNotation_ShouldThrowArgumentException(string subnet)
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotInSubnet(subnet);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("subnet").And
					.WithMessage("The 'subnet' must be a valid CIDR notation, e.g. \"10.0.0.0/8\".").AsPrefix();
			}

			[Test]
			[Arguments("10.0.0.1", "10.0.0.0/8", "10.0.0.0/8")]
			[Arguments("10.0.15.255", "10.0.0.0/20", "10.0.0.0/20")]
			[Arguments("192.168.0.1", "0.0.0.0/0", "0.0.0.0/0")]
			[Arguments("2001:db8:7fff::1", "2001:db8::/33", "2001:db8::/33")]
			[Arguments("2001:db8::1", "2001:DB8::1/128", "2001:db8::1/128")]
			[Arguments("::ffff:10.0.0.1", "10.0.0.0/8", "10.0.0.0/8")]
			[Arguments("10.0.0.1", "::ffff:0:0/96", "::ffff:0:0/96")]
			public async Task WhenSubjectIsInSubnet_ShouldFail(string address, string subnet, string expectedSubnet)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsNotInSubnet(subnet);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not in subnet {expectedSubnet},
					              but it was {subject}
					              """);
			}

			[Test]
			[Arguments("11.0.0.0", "10.0.0.0/8")]
			[Arguments("10.0.16.0", "10.0.0.0/20")]
			[Arguments("192.168.0.2", "192.168.0.1/32")]
			[Arguments("2001:db8:8000::", "2001:db8::/33")]
			[Arguments("::ffff:11.0.0.1", "10.0.0.0/8")]
			[Arguments("2001:db8::1", "0.0.0.0/0")]
			public async Task WhenSubjectIsNotInSubnet_ShouldSucceed(string address, string subnet)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsNotInSubnet(subnet);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotInSubnet("10.0.0.0/8");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in subnet 10.0.0.0/8,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsInSubnet_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInSubnet("10.0.0.0/8"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNotInSubnet_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("192.168.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInSubnet("10.0.0.0/8"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in subnet 10.0.0.0/8,
					             but it was 192.168.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInSubnet("10.0.0.0/8"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in subnet 10.0.0.0/8,
					             but it was <null>
					             """);
			}
		}
	}
}
