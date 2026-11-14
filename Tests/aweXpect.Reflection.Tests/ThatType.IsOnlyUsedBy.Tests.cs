using System.Collections.Generic;
using System.Linq;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Jobs;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store;

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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it had disallowed users

					              Disallowed users:
					              [
					                "SyncJob"
					              ]
					              """)
					.Because("SyncJob uses the construction GenericStore<string>");
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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
					              but it was <null>
					              """);
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
					              is only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
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
					              is not only used by types within namespace "{InfrastructureNamespace}" in all loaded assemblies or types within namespace "{ApiNamespace}" in all loaded assemblies or types within namespace "{JobsNamespace}" in all loaded assemblies within types within namespace "{UsageNamespace}" in all loaded assemblies,
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
