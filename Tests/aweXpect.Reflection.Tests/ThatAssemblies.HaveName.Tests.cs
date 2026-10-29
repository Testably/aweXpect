using System.Collections.Generic;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class HaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [Assembly.GetExecutingAssembly(), null,];

				async Task Act()
				{
					await That(subject).HaveName("aweXpect.Reflection.Tests");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name equal to "aweXpect.Reflection.Tests",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have name equal to "aweXpect.Reflection.Tests",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenItemIsNullWithASelector_ShouldNotInvokeTheSelector()
			{
				IEnumerable<Assembly?> subject = [Assembly.GetExecutingAssembly(), null,];

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly.GetName().Name!);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching assembly => assembly.GetName().Name!,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("the selector is only invoked for the items that can be inspected");
			}

			[Test]
			public async Task WhenMoreAssembliesDoNotMatchSelectorThanTheMaximumNumberOfCollectionItems_ShouldLimitTheList()
			{
				using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(1);
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly, typeof(ThatAssemblies).Assembly);

				async Task Act()
				{
					await That(subject).HaveName(_ => "WrongName");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             all have name matching _ => "WrongName",
					             but none of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=* with name "aweXpect.Reflection" instead of "WrongName",
					               (… and 1 more)
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=* with name "aweXpect.Reflection" instead of "WrongName",
					               (… and 1 more)
					             ]
					             """).AsWildcard()
					.Because("the not matching assemblies are limited like any other collection in a failure message");
			}

			[Test]
			public async Task WhenMultipleAssembliesDoNotMatchSelector_ShouldSeparateWithComma()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly, typeof(ThatAssemblies).Assembly);

				async Task Act()
				{
					await That(subject).HaveName(_ => "WrongName");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             all have name matching _ => "WrongName",
					             but none of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=* with name "aweXpect.Reflection" instead of "WrongName",
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=* with name "aweXpect.Reflection" instead of "WrongName",
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSelectorMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly.GetName().Name!.ToUpperInvariant())
						.IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorReturnsDifferentName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(_ => "WrongName");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name matching _ => "WrongName",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null with name "aweXpect.Reflection.Tests" instead of "WrongName"
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSelectorReturnsEachAssemblysOwnName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(assembly => assembly.GetName().Name!);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSelectorIsNull_ShouldThrowArgumentNullException()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName((Func<Assembly?, string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectedNameSelector").And
					.WithMessage("The 'expectedNameSelector' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSelectorThrows_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name matching _ => throw new InvalidOperationException("boom"),
					             but the name selector did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               (… and maybe more)
					             ]
					             """).AsWildcard()
					.Because("an exception of the name selector must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenTypesContainTypeWithDifferentName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have name equal to "Reflection",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypesHaveName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveName("AWExPECT.rEFLECTION.tESTS").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			public async Task WhenAssembliesDoNotHaveName_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentAssembly"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesHaveName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have name equal to "aweXpect.Reflection.Tests",
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSelectorMatchesEveryName_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(_ => "aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have name matching _ => "aweXpect.Reflection.Tests",
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
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
