using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Reflection.Results;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Infrastructure;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Jobs;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store;
using TUnit.Core.Exceptions;
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
						.SearchedIn(Types.InNamespace(UsageNamespace));
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
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
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
			public async Task WhenCanceledWhileReadingTheItems_ShouldNotBeVerified()
			{
				using CancellationTokenSource cts = new();
				int read = 0;

				IEnumerable<Type?> Subject()
				{
					yield return typeof(TrackStore);
					while (read < 1000)
					{
						read++;
						cts.Cancel();
						yield return typeof(UnusedStore);
					}
				}

				IEnumerable<Type?> subject = Subject();

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther()
						.WithCancellation(cts.Token);
				}

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or by each other, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it could not be verified, because the evaluation was already canceled

					              Collection:
					              [
					                TrackStore,
					                UnusedStore,
					                (… and maybe more)
					              ]
					              """);
				await That(read).IsLessThan(10)
					.Because("the cancellation is checked while the items are read");
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(UnusedStore), null,];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                UnusedStore,
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenEvaluatedAgain_ShouldSearchTheUsersAgain()
			{
				IEnumerable<Type?> subject = [typeof(UnusedStore),];
				ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> resolver =
					Customize.aweXpect.Reflection().DependencyResolver;
				Func<Type, IEnumerable<Type>> builtin = resolver.Get()!;
				OnlyUsedByItemsResult<IEnumerable<Type?>, IEnumerable<Type?>?> result = That(subject)
					.AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
					.SearchedIn(Types.InNamespace(UsageNamespace));
				await result;

				async Task Act()
				{
					using (resolver.Set(type => type == typeof(BodyOnlyJob)
						       ? builtin(type).Concat([typeof(UnusedStore),])
						       : builtin(type)))
					{
						await result;
					}
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but none of 1 did

					              Not matching items:
					              [
					                UnusedStore is used by ["BodyOnlyJob"]
					              ]

					              Collection:
					              [
					                UnusedStore is used by ["BodyOnlyJob"]
					              ]
					              """)
					.Because("the users of a previous evaluation must not be reused");
			}

			[Test]
			public async Task WhenFirstItemIsNullAndItemsMayUseEachOther_ShouldFail()
			{
				IEnumerable<Type?> subject = [null, typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies or by each other, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but for the item at index 0, it was <null>

					              Collection:
					              [
					                <null>,
					                TrackStore,
					                StoreAdapter
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
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsMayUseEachOther_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("StoreAdapter is an item of the collection");
			}

			[Test]
			public async Task WhenItemsMayUseEachOtherButAreUsedByDisallowedTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or by each other, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                TrackStore is used by ["SyncJob"]
					              ]

					              Collection:
					              [
					                TrackStore is used by ["SyncJob"],
					                StoreAdapter
					              ]
					              """);
			}

			[Test]
			public async Task WhenItemsMayUseEachOtherThroughAConstruction_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(CacheStore<int>), typeof(StoreCache<>),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(In.Types(typeof(StoreCache<int>)))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("the user StoreCache<int> is a construction of the item StoreCache<>");
			}

			[Test]
			public async Task WhenItemsMayUseEachOtherThroughANestedType_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(NestingStore.Entry), typeof(LinkTarget),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(In.Types(typeof(NestingStore.Entry.Detail)))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("the user Entry.Detail is nested in the item Entry");
			}

			[Test]
			public async Task WhenItemsMayUseEachOtherThroughTheirGenericTypeDefinition_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(LinkStore<int>), typeof(LinkTarget),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(In.Types(typeof(LinkStore<>)))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("the user LinkStore<> is the generic type definition of the item LinkStore<int>");
			}

			[Test]
			public async Task WhenSomeItemIsUsedByDisallowedTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
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

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenItemsMayUseEachOtherAndAreOnlyUsedByAllowedTypes_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              not all are only used by types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies or by each other, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but all 2 did

					              Collection:
					              [
					                TrackStore,
					                StoreAdapter
					              ]
					              """);
			}

			[Test]
			public async Task WhenItemsMayUseEachOtherButAreUsedByDisallowedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther());
				}

				await That(Act).DoesNotThrow()
					.Because("TrackStore is used by SyncJob");
			}
		}

		public sealed class QuantifierTests
		{
			[Test]
			public async Task All_WhenALaterItemUsesAnEarlierOne_ShouldSucceed()
			{
				int enumerations = 0;

				IEnumerable<Type?> Subject()
				{
					enumerations++;
					yield return typeof(TrackStore);
					yield return typeof(StoreAdapter);
				}

				async Task Act()
				{
					await That(Subject()).All()
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("StoreAdapter is read before TrackStore is checked");
				await That(enumerations).IsEqualTo(1)
					.Because("the items are read from the sequence that the evaluation shares");
			}

			[Test]
			public async Task Exactly_WhenOneItemIsOnlyUsedByTheAllowedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).Exactly(1).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace));
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
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies for no items,
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
			public async Task All_WhenItemsMayUseEachOther_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(TrackStore), typeof(StoreAdapter),];

				async Task Act()
				{
					await That(subject).All()
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("StoreAdapter is an item of the collection");
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsAreOnlyUsedByTheAllowedTypes_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(TrackStore), typeof(UnusedStore),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies for at least 2 items,
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
			public async Task All_WhenItemsMayUseEachOther_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(TrackStore), typeof(StoreAdapter),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).All()
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("StoreAdapter is an item of the collection");
			}

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
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsMayUseEachOther_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(TrackStore), typeof(StoreAdapter),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject)
						.AreOnlyUsedBy(Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.SearchedIn(Types.InNamespace(UsageNamespace))
						.OrByEachOther();
				}

				await That(Act).DoesNotThrow()
					.Because("StoreAdapter is an item of the collection");
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
						.SearchedIn(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all are only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
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
