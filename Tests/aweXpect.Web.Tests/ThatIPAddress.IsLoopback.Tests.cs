using System.Net;

namespace aweXpect.Tests;

public sealed partial class ThatIPAddress
{
	public sealed class IsLoopback
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				IPAddress[] subjects = [IPAddress.Parse("10.0.0.1"),];

				async Task Act()
					=> await That(new { Addresses = subjects, })
						.Whose(x => x.Addresses, items => items.All()
							.ComplyWith(item => item.IsLoopback()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Addresses are loopback addresses for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Theory]
			[InlineData("127.0.0.1")]
			[InlineData("127.1.2.3")]
			[InlineData("::1")]
			public async Task WhenSubjectIsLoopback_ShouldSucceed(string address)
			{
				IPAddress subject = IPAddress.Parse(address);

				async Task Act()
					=> await That(subject).IsLoopback();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotLoopback_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).IsLoopback();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a loopback address,
					             but it was 10.0.0.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).IsLoopback();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a loopback address,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsLoopback_ShouldFail()
			{
				IPAddress subject = IPAddress.Parse("::1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLoopback());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a loopback address,
					             but it was ::1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed()
			{
				IPAddress subject = IPAddress.Parse("10.0.0.1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLoopback());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IPAddress? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLoopback());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a loopback address,
					             but it was <null>
					             """);
			}
		}
	}
}
