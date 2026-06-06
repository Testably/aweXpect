using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotDependOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				// A null item's dependencies cannot be verified, so it fails the negative assertion just
				// like it fails DependOn and DependOnlyOn, instead of slipping through.
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on namespace "{Layer2Namespace}",
					              but it contained types with the dependency [
					                <null>
					              ]
					              """);
			}

			[Fact]
			public async Task WhenNoTypeDependsOnNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDependsOnNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on namespace "{Layer2Namespace}",
					              but it contained types with the dependency [
					                Layer1AndLayer2
					              ]
					              """);
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task WhenNoTypeDependsOnTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDependsOnTargetCollection_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it contained types with the dependency [
					                Layer1AndLayer2
					              ]
					              """);
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_ShouldSucceedTrivially()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(Layer1AndLayer2),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace))
						.OrOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>();
			}
		}
	}
}
