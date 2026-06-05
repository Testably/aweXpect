using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DependOnlyOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllDependenciesAreAllowed_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(FrameworkConsumer),
					typeof(ReferencesOwnNamespace),
				];

				async Task Act()
					=> await That(subject).DependOnlyOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDependsOnDisallowedNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
					=> await That(subject).DependOnlyOn(Layer1Namespace);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but it contained types with disallowed dependencies [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldListNullWithoutViolations()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					null,
				];

				async Task Act()
					=> await That(subject).DependOnlyOn(Layer1Namespace);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but it contained types with disallowed dependencies [
					                <null>
					              ]
					              """);
			}

			[Fact]
			public async Task WhenAllowingAllNamespaces_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
					=> await That(subject).DependOnlyOn(Layer1Namespace, Layer2Namespace);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
