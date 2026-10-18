using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsNotEqualTo
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
							.ComplyWith(item => item.IsNotEqualTo(IPAddress.Parse("10.0.0.1"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Addresses are not equal to 10.0.0.1 for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenScopeIdsDiffer_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("fe80::1%1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("fe80::1"));

				await That(Act).DoesNotThrow()
					.Because("IPAddress.Equals decides equality and it considers the scope ID");
			}

			[Test]
			public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsDifferentIPv4_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.2");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferentIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::2");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("2001:db8::1"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsEqualIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 10.0.0.1,
					             but it was 10.0.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEqualIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("2001:db8::1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 2001:db8::1,
					             but it was 2001:db8::1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsIPv4MappedToIPv6AndUnexpectedIsIPv4_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).DoesNotThrow()
					.Because("an IPv4-mapped address is only equal to the IPv4 address when ignoring the mapping");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringIPv4MappedToIPv6Tests
		{
			[Test]
			public async Task WhenMappedAddressesDiffer_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.2");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("::ffff:10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to ::ffff:10.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was 10.0.0.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsIPv4MappedToIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsNotEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 10.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was ::ffff:10.0.0.1
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1,
					             but it was 10.0.0.2
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEqual_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1,
					             but it was <null>
					             """);
			}
		}
	}
}
