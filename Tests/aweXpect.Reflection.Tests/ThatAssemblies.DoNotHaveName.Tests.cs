using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DoNotHaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task Enumerable_WhenNoAssemblyHasName_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly, null,
				};

				async Task Act()
				{
					await That(subject).DoNotHaveName("NonExistentAssembly");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasMatchingSuffix_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotHaveName(".Tests").AsSuffix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name not ending with ".Tests",
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyHasName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("aweXpect.Reflection.Tests");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name not equal to "aweXpect.Reflection.Tests",
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyMatchesIgnoringCase_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("AWExPECT.rEFLECTION.tESTS").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name not equal to "AWExPECT.rEFLECTION.tESTS" ignoring case,
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNoAssemblyHasName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("NonExistentAssembly");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoAssemblyHasName_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly, null,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("NonExistentAssembly");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenAssemblyHasName_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("aweXpect.Reflection.Tests");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name not equal to "aweXpect.Reflection.Tests",
					             but it contained not matching assemblies

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyHasName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName("aweXpect.Reflection.Tests"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoAssemblyHasName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName("NonExistentAssembly"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have name not equal to "NonExistentAssembly",
					             but it only contained matching assemblies*
					             """).AsWildcard();
			}
		}
	}
}
