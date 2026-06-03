using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreAttributes
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAttributeTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types();

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreAttributes
					             are all attributes,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAttributes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsNonAttributeType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAttribute), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all attributes,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsOnlyAttributeTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAttribute),
				};

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAttributeTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAttributes());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAttributes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAttributes());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are attributes in assembly containing type ThatTypes.AreAttributes
					             are not all attributes,
					             but it only contained attributes [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
