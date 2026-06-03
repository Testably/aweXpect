using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAttributes
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsAttributeType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAttribute),
				};

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all not attributes,
					             but it contained attributes [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoAttributeTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNoAttributeTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreNotAttributes();

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAttributes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which are attributes in assembly containing type ThatTypes.AreNotAttributes
					             are all not attributes,
					             but it contained attributes [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNoAttributeTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreNotAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAttributes());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are not attributes in assembly containing type ThatTypes.AreNotAttributes
					             also contain an attribute,
					             but it only contained not attributes [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAttributes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAttributes());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
