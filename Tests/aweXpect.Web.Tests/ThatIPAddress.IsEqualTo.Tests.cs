using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenComparerIsUsed_ShouldUseIt()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.2")).Using(new SameFamilyComparer());

				await That(Act).DoesNotThrow()
					.Because("the options of aweXpect for comparing objects are still available");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was 10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Parse("10.0.0.2"),];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsEqualTo(IPAddress.Parse("10.0.0.1"))));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Addresses are equal to 10.0.0.1 for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenScopeIdsDiffer_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("fe80::1%1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("fe80::1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to fe80::1,
					             but it was fe80::1%1
					             """)
					.Because("IPAddress.Equals decides equality and it considers the scope ID");
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferentIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.2");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1,
					             but it was 10.0.0.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferentIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::2");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("2001:db8::1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2001:db8::1,
					             but it was 2001:db8::2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsEqualIPv4_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEqualIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("2001:db8::1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("2001:0db8:0:0::1"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsIPv4MappedToIPv6AndExpectedIsIPv4_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1,
					             but it was ::ffff:10.0.0.1
					             """)
					.Because("an IPv4-mapped address is only equal to the IPv4 address when ignoring the mapping");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1,
					             but it was <null>
					             """);
			}
		}

		public sealed class IgnoringIPv4MappedToIPv6Tests
		{
			[Fact]
			public async Task WhenBothAreIPv4MappedToIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("::ffff:10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCombinedWithComparer_ShouldThrowInvalidOperationException()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1"))
						.Using(new SameFamilyComparer()).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<InvalidOperationException>()
					.Because("both options specify how the addresses are compared");
			}

			[Fact]
			public async Task WhenExpectedIsIPv4MappedToIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("::ffff:10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMappedAddressesDiffer_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.2");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was ::ffff:10.0.0.2
					             """)
					.Because("ignoring the mapping does not make different addresses equal");
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(null).IgnoringIPv4MappedToIPv6();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsIPv4MappedToIPv6_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsIPv6_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::1");

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("127.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 127.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was ::1
					             """)
					.Because("only IPv4-mapped IPv6 addresses are mapped to IPv4");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 10.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEqual_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 10.0.0.1,
					             but it was 10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsIPv4MappedToIPv6_ShouldFailWhenIgnoringMapping()
			{
				IPAddress subject = IPAddress.Parse("::ffff:10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsEqualTo(IPAddress.Parse("10.0.0.1")).IgnoringIPv4MappedToIPv6());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 10.0.0.1 ignoring IPv4-to-IPv6 mapping,
					             but it was ::ffff:10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(IPAddress.Parse("10.0.0.1")));

				await That(Act).DoesNotThrow();
			}
		}

		private sealed class SameFamilyComparer : System.Collections.Generic.IEqualityComparer<IPAddress?>
		{
			public bool Equals(IPAddress? x, IPAddress? y) => x?.AddressFamily == y?.AddressFamily;

			public int GetHashCode(IPAddress? obj) => 0;
		}
	}
}
