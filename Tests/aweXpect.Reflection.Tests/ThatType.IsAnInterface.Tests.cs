using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnInterface
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnInterface_ShouldSucceed()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
				{
					await That(subject).IsAnInterface();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonInterfaceType))]
			public async Task WhenTypeIsNotAnInterface_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnInterface();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is an interface,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnInterface();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an interface,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonInterfaceType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAnInterface_ShouldFail()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnInterface());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was interface IPublicInterface
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonInterfaceTypeForNegated))]
			public async Task WhenTypeIsNotAnInterface_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnInterface());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonInterfaceTypeForNegated() => new List<Type>
			{
				typeof(PublicStruct),
				typeof(PublicClass),
				typeof(PublicEnum),
				typeof(PublicRecord),
				typeof(PublicRecordStruct),
			};

			public static IEnumerable<(Type?, string)> NonInterfaceType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}
	}
}
