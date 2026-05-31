using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveNamespace
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsTypeWithDifferentNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ThatTypes),
				};

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but it contained not matching types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableTypesHaveNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicEnum),
				};

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypesContainTypeWithDifferentNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.HaveNamespace.Tests
					             all have namespace equal to "aweXpect.Reflection",
					             but it contained not matching types [
					               *SomeClassToVerifyTheNamespaceOfIt*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypesHaveNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject)
						.HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject)
						.HaveNamespace("AWExPECT.rEFLECTION.tESTS.tESThELPERS.tYPES.tOvERIFYINGtHEnAMESPACEoFiT")
						.IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypesContainTypeWithDifferentNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveNamespace("aweXpect.Reflection"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesHaveNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.HaveNamespace(
							"aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.HaveNamespace.Tests
					             not all have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but it only contained matching types [
					               *SomeClassToVerifyTheNamespaceOfIt*
					             ]
					             """).AsWildcard();
			}
		}
	}
}
