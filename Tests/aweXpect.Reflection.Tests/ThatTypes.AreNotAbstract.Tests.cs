using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAbstract
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsAbstractType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAbstractClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoAbstractTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, } in assembly containing type ThatTypes.AreNotAbstract
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sealed types in assembly containing type ThatTypes.AreNotAbstract
					             also contain an abstract type,
					             but it only contained non-abstract types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
