using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARecord
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARecord_ShouldFail()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
				{
					await That(subject).IsNotARecord();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was record PublicRecord
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRecordTypes))]
			public async Task WhenTypeIsNotARecord_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotARecord();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotARecord();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonRecordTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicStruct),
					typeof(PublicRecordStruct),
				];
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsARecord_ShouldSucceed()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARecord());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonRecordTypes))]
			public async Task WhenTypeIsNotARecord_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARecord());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is a record*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonRecordTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicStruct),
					typeof(PublicRecordStruct),
				];
		}
	}
}
