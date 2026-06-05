using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DependOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesDependOnNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(ViaProperty),
					typeof(OnlyLayer1),
				];

				async Task Act()
					=> await That(subject).DependOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDoesNotDependOnNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(FrameworkConsumer),
				];

				async Task Act()
					=> await That(subject).DependOn(Layer1Namespace);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all depend on namespace "{Layer1Namespace}",
					              but it contained types without the dependency [
					                FrameworkConsumer
					              ]
					              """);
			}

			[Fact]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

				async Task Act()
					=> await That(subject).DependOn();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("At least one namespace must be specified.");
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
					=> await That(subject).DependOn("Non.Existent.Namespace").OrOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
