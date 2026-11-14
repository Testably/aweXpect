using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Infrastructure;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Jobs;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store;
using aweXpect.Results;
using TUnit.Core.Exceptions;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsOnlyUsedBy
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
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace),
							Types.InNamespace(ApiNamespace), Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAdditionalContainsNull_ShouldThrowArgumentException()
			{
				Type subject = typeof(TrackStore);
				Filtered.Types[] additional = [null!,];

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace), additional)
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("additional").And
					.WithMessage("The 'additional' collection cannot contain <null>.*").AsWildcard();
			}

			[Test]
			public async Task WhenAdditionalIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(TrackStore);
				Filtered.Types[] additional = null!;

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace), additional)
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("additional").And
					.WithMessage("The 'additional' cannot be null.*").AsWildcard();
			}

			[Test]
			public async Task WhenAllowedCollectionContainsTheGenericTypeDefinitionOfAUser_ShouldSucceed()
			{
				Type subject = typeof(CacheStore<int>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(In.Types(typeof(StoreCache<>)))
						.Within(In.Types(typeof(StoreCache<int>)));
				}

				await That(Act).DoesNotThrow()
					.Because("the generic type definition StoreCache<> allows its construction StoreCache<int>");
			}

			[Test]
			public async Task WhenCanceledWhileSearchingTheUsers_ShouldNotBeVerified()
			{
				using CancellationTokenSource cts = new();
				Type subject = typeof(TrackStore);
				Func<Type, bool> cancel = _ =>
				{
					cts.Cancel();
					return true;
				};

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace).Which(cancel))
						.WithCancellation(cts.Token);
				}

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" matching cancel in all loaded assemblies,
					              but it could not be verified, because the evaluation was already canceled
					              """)
					.Because("the cancellation is checked while the types within the scope are searched for users");
			}

			[Test]
			public async Task WhenDependencyResolverFindsADisallowedUser_ShouldFail()
			{
				Type subject = typeof(UnusedStore);
				ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> resolver =
					Customize.aweXpect.Reflection().DependencyResolver;
				Func<Type, IEnumerable<Type>> builtin = resolver.Get()!;

				async Task Act()
				{
					using (resolver.Set(type => type == typeof(BodyOnlyJob)
						       ? builtin(type).Concat([typeof(UnusedStore),])
						       : builtin(type)))
					{
						await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
							.Within(Types.InNamespace(UsageNamespace));
					}
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "BodyOnlyJob"
					              ]
					              """)
					.Because("the usages are read through the customized dependency resolver");
			}

			[Test]
			public async Task WhenDependencyResolverThrows_ShouldFail()
			{
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					using (Customize.aweXpect.Reflection().DependencyResolver
						       .Set(_ => throw new InvalidOperationException("boom")))
					{
						await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
							.Within(Types.InNamespace(UsageNamespace));
					}
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but the dependency resolver did throw an InvalidOperationException:
					                boom
					              """)
					.Because("an exception of the dependency resolver must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenGenericTypeDefinitionIsUsedThroughAConstruction_ShouldFail()
			{
				Type subject = typeof(GenericStore<>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "SyncJob"
					              ]
					              """)
					.Because("SyncJob uses the construction GenericStore<string>");
			}

			[Test]
			public async Task WhenGenericTypeDefinitionIsUsedThroughTwoConstructions_ShouldListTheUserOnce()
			{
				Type subject = typeof(CacheStore<>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "CacheJob"
					              ]
					              """)
					.Because("CacheJob uses both CacheStore<int> and CacheStore<string>");
			}

			[Test]
			public async Task WhenScopeDoesNotContainTheDisallowedUsers_ShouldSucceed()
			{
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(InfrastructureNamespace));
				}

				await That(Act).DoesNotThrow()
					.Because("only the types within the scope are searched for users");
			}

			[Test]
			public async Task WhenSubjectIsAConstructionThatOnlyAllowedTypesUse_ShouldSucceed()
			{
				Type subject = typeof(GenericStore<int>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow()
					.Because("SyncJob only uses another construction of the generic type");
			}

			[Test]
			public async Task WhenSubjectIsAnArrayType_ShouldCheckItsElementType()
			{
				Type subject = typeof(TrackStore[]);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace),
							Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "TrackController"
					              ]
					              """)
					.Because("dependencies are matched without their array wrappers");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenTypeUsesAnotherConstructionOfItself_ShouldSucceed()
			{
				Type subject = typeof(CacheStore<int>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow()
					.Because("the generic type definition CacheStore<> is the type itself");
			}

			[Test]
			public async Task WhenTypeUsesItself_ShouldSucceed()
			{
				Type subject = typeof(GenericStore<>);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace),
							Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).DoesNotThrow()
					.Because("a type may always use itself, also through a construction of itself");
			}

			[Test]
			public async Task WhenUsedByDisallowedTypes_ShouldFail()
			{
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "SyncJob",
					                "TrackController"
					              ]
					              """);
			}

			[Test]
			public async Task WhenUsersIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(TrackStore);
				Filtered.Types users = null!;

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(users).Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("users").And
					.WithMessage("The 'users' cannot be null.*").AsWildcard();
			}

			[Test]
			public async Task WhenWithinIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				Type subject = typeof(TrackStore);
				OnlyUsedByResult<AndOrResult<Type, IThat<Type?>>> result =
					That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace));
				_ = result.Within(Types.InNamespace(InfrastructureNamespace));

				void Act()
				{
					_ = result.Within(Types.InNamespace(UsageNamespace));
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("The scope cannot be set again: 'Within' can only be called once.");
			}

			[Test]
			public async Task WhenWithinIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(TrackStore);
				Filtered.Types within = null!;

				async Task Act()
				{
					await That(subject).IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace)).Within(within);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("types").And
					.WithMessage("The 'types' cannot be null.*").AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOnlyUsedByAllowedTypes_ShouldFail()
			{
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it
						.IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace), Types.InNamespace(ApiNamespace),
							Types.InNamespace(JobsNamespace))
						.Within(Types.InNamespace(UsageNamespace)));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies or types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies, searched in types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it was only used by the allowed types
					              """);
			}

			[Test]
			public async Task WhenUsedByDisallowedTypes_ShouldSucceed()
			{
				Type subject = typeof(TrackStore);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it
						.IsOnlyUsedBy(Types.InNamespace(InfrastructureNamespace))
						.Within(Types.InNamespace(UsageNamespace)));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
