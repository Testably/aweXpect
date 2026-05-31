using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSelectorMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly!.GetName().Name!.ToUpperInvariant())
						.IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSelectorReturnsDifferentName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(_ => "WrongName");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name matching _ => "WrongName",
					             but it contained not matching types [
					               aweXpect.Reflection.Tests, Version=* with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMultipleAssembliesDoNotMatchSelector_ShouldSeparateWithComma()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly, typeof(ThatAssemblies).Assembly);

				async Task Act()
				{
					await That(subject).HaveName(_ => "WrongName");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             all have name matching _ => "WrongName",
					             but it contained not matching types [
					               aweXpect.Reflection, Version=* with name "aweXpect.Reflection" instead of "WrongName",
					               aweXpect.Reflection.Tests, Version=* with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorReturnsEachAssemblysOwnName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly!.GetName().Name!);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesContainTypeWithDifferentName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("Reflection");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name equal to "Reflection",
					             but it contained not matching types [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypesHaveName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("AWExPECT.rEFLECTION.tESTS").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("aweXpect").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesDoNotHaveName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentAssembly"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesHaveName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have name equal to "aweXpect.Reflection.Tests",
					             but it only contained matching types *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorMatchesEveryName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(_ => "aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have name matching _ => "aweXpect.Reflection.Tests",
					             but it only contained matching types [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorMatchesNoName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(_ => "WrongName"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
