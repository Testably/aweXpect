using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Infrastructure;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreOnlyUsedBy
	{
		private const string UsageNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage";
		private const string ApiNamespace = UsageNamespace + ".Api";
		private const string InfrastructureNamespace = UsageNamespace + ".Infrastructure";
		private const string JobsNamespace = UsageNamespace + ".Jobs";

		public sealed class Tests
		{
			[Test]
			public async Task WhenAdditionalCollectionsAllowTheUsers_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace),
							Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAnItemIsUsedByAnotherItem_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                TrackStore is used by ["StoreAdapter"]
					              ]

					              Collection:
					              [
					                TrackStore is used by ["StoreAdapter"],
					                StoreAdapter
					              ]
					              """)
					.Because("each item may only use itself");
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(UnusedStore), null,];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                UnusedStore,
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenItemsAreAllowedToUseEachOther_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace),
							In.Types<TrackStore, StoreAdapter>())
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeItemIsUsedByDisallowedTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"]
					              ]

					              Collection:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"],
					                UnusedStore
					              ]
					              """);
			}
		}

		public sealed class QuantifierTests
		{
			[Test]
			public async Task Exactly_WhenOneItemIsOnlyUsedByTheAllowedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).Exactly(1).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemIsOnlyUsedByTheAllowedTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).None().AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies for no items,
					              but 1 of 2 did

					              Matching items:
					              [
					                UnusedStore
					              ]

					              Collection:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"],
					                UnusedStore
					              ]
					              """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsAreOnlyUsedByTheAllowedTypes_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies for at least 2 items,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"]
					              ]

					              Collection:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"],
					                UnusedStore
					              ]
					              """);
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task Exactly_WhenOneItemIsOnlyUsedByTheAllowedTypes_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(TrackStore), typeof(UnusedStore),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).Exactly(1).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeItemIsUsedByDisallowedTypes_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(TrackStore), typeof(UnusedStore),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"]
					              ]

					              Collection:
					              [
					                TrackStore is used by ["SyncJob", "TrackController"],
					                UnusedStore
					              ]
					              """);
			}
		}
#endif
	}
}
