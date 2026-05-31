using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARefStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARefStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonRefStructType))]
			public async Task WhenTypeIsNotARefStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a ref struct,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsARefStruct();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a ref struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonRefStructType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(PublicReadOnlyStruct), "struct PublicReadOnlyStruct"
				},
				{
					typeof(PublicClass), "class PublicClass"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicEnum), "enum PublicEnum"
				},
				{
					typeof(PublicRecordStruct), "record struct PublicRecordStruct"
				},
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsARefStruct_ShouldFail()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARefStruct());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was struct PublicRefStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRefStructTypeForNegated))]
			public async Task WhenTypeIsNotARefStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARefStruct());
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonRefStructTypeForNegated() => new()
			{
				typeof(PublicStruct),
				typeof(PublicReadOnlyStruct),
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
