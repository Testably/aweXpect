using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsInSubnet
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Parse("192.168.0.1"),];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsInSubnet("10.0.0.0/8")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Addresses are in subnet 10.0.0.0/8 for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenSubnetIsNull_ShouldThrowArgumentNullException()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsInSubnet(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("subnet").And
					.WithMessage("The 'subnet' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData("")]
			[InlineData("/")]
			[InlineData("10.0.0.0")]
			[InlineData("10.0.0.0/")]
			[InlineData("/8")]
			[InlineData("not an address/8")]
			[InlineData("10.0.0.0/eight")]
			[InlineData("10.0.0.0/-1")]
			[InlineData("10.0.0.0/+8")]
			[InlineData("10.0.0.0/08")]
			[InlineData("10.0.0.0/8/8")]
			[InlineData("10.0.0.0/33")]
			[InlineData("10.0.0.0/1000")]
			[InlineData("2001:db8::/129")]
			[InlineData("10.0.0.1/8")]
			[InlineData("10.0.0.0/0")]
			[InlineData("10.0.16.0/19")]
			[InlineData("2001:db8::1/64")]
			[InlineData("2001:db8:8000::/32")]
			[InlineData(" 10.0.0.0/8")]
			[InlineData("10.0.0.0/8 ")]
			[InlineData("10.0.0.0 /8")]
			[InlineData("10.0.0.0/ 8")]
			[InlineData("10.0/8")]
			[InlineData("010.0.0.0/8")]
			[InlineData("0x0A.0.0.0/8")]
			[InlineData("fe80::%1/64")]
			[InlineData("[2001:db8::]/32")]
			public async Task WhenSubnetIsNoValidCidrNotation_ShouldThrowArgumentException(string subnet)
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsInSubnet(subnet);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("subnet").And
					.WithMessage("The 'subnet' must be a valid CIDR notation, e.g. \"10.0.0.0/8\".").AsPrefix();
			}

			[Theory]
			[InlineData("10.0.0.0", "10.0.0.0/8")]
			[InlineData("10.255.255.255", "10.0.0.0/8")]
			[InlineData("172.16.0.1", "172.16.0.0/12")]
			[InlineData("172.31.255.255", "172.16.0.0/12")]
			[InlineData("10.0.15.255", "10.0.0.0/20")]
			[InlineData("192.168.0.1", "192.168.0.1/32")]
			[InlineData("192.168.0.1", "0.0.0.0/0")]
			[InlineData("2001:db8::1", "2001:db8::/32")]
			[InlineData("2001:db8:ffff:ffff::", "2001:db8::/32")]
			[InlineData("2001:db8:7fff::1", "2001:db8::/33")]
			[InlineData("2001:db8::1", "2001:db8::1/128")]
			[InlineData("2001:db8::1", "::/0")]
			[InlineData("fe80::1%4", "fe80::/64")]
			[InlineData("::ffff:10.0.0.1", "10.0.0.0/8")]
			[InlineData("::ffff:10.0.0.1", "0.0.0.0/0")]
			[InlineData("10.0.0.1", "::ffff:0:0/96")]
			[InlineData("10.0.0.1", "::ffff:10.0.0.0/104")]
			[InlineData("10.0.0.1", "::/0")]
			public async Task WhenSubjectIsInSubnet_ShouldSucceed(string address, string subnet)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsInSubnet(subnet);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("11.0.0.0", "10.0.0.0/8", "10.0.0.0/8")]
			[InlineData("9.255.255.255", "10.0.0.0/8", "10.0.0.0/8")]
			[InlineData("172.32.0.0", "172.16.0.0/12", "172.16.0.0/12")]
			[InlineData("10.0.16.0", "10.0.0.0/20", "10.0.0.0/20")]
			[InlineData("192.168.0.2", "192.168.0.1/32", "192.168.0.1/32")]
			[InlineData("2001:db9::", "2001:db8::/32", "2001:db8::/32")]
			[InlineData("2001:db8:8000::", "2001:db8::/33", "2001:db8::/33")]
			[InlineData("2001:db8::2", "2001:db8::1/128", "2001:db8::1/128")]
			[InlineData("2001:0DB8::2", "2001:0DB8::/128", "2001:db8::/128")]
			[InlineData("::ffff:11.0.0.1", "10.0.0.0/8", "10.0.0.0/8")]
			[InlineData("11.0.0.1", "::ffff:10.0.0.0/104", "::ffff:10.0.0.0/104")]
			[InlineData("::a00:1", "10.0.0.0/8", "10.0.0.0/8")]
			[InlineData("2001:db8::1", "0.0.0.0/0", "0.0.0.0/0")]
			public async Task WhenSubjectIsNotInSubnet_ShouldFail(string address, string subnet, string expectedSubnet)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsInSubnet(subnet);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is in subnet {expectedSubnet},
					              but it was {subject}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsInSubnet("10.0.0.0/8");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in subnet 10.0.0.0/8,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsInSubnet_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInSubnet("10.0.0.0/8"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in subnet 10.0.0.0/8,
					             but it was 10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotInSubnet_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("192.168.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInSubnet("10.0.0.0/8"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInSubnet("10.0.0.0/8"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in subnet 10.0.0.0/8,
					             but it was <null>
					             """);
			}
		}
	}
}
