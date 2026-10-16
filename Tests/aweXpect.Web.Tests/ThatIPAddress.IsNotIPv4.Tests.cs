using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsNotIPv4
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Loopback,];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsNotIPv4()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Addresses are not IPv4 addresses for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenSubjectIsIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsNotIPv4();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an IPv4 address,
					             but it was 10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).IsNotIPv4();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotIPv4();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an IPv4 address,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsIPv4_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotIPv4());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotIPv4());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an IPv4 address,
					             but it was 2001:db8::1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotIPv4());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an IPv4 address,
					             but it was <null>
					             """);
			}
		}
	}
}
