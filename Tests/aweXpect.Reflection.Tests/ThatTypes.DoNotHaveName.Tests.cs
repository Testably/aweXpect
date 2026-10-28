using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task Enumerable_WhenNoTypeHasName_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DoNotHaveNameType),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveName("SomeOtherClassName");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(DoNotHaveNameType), null,];

				async Task Act()
				{
					await That(subject).DoNotHaveName("SomeOtherClassName");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName("SomeOtherClassName"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have name equal to "SomeOtherClassName",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has name equal to "SomeOtherClassName",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeHasName_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameType));

				async Task Act()
				{
					await That(subject).DoNotHaveName("SomeOtherClassName");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameType));

				async Task Act()
				{
					await That(subject).DoNotHaveName(nameof(DoNotHaveNameType));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameType" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             none have name equal to "DoNotHaveNameType",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypeMatchesSuffix_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameType));

				async Task Act()
				{
					await That(subject).DoNotHaveName("HaveNameType").AsSuffix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameType" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             none have name ending with "HaveNameType",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]
					             """).AsWildcard();
			}

			private class DoNotHaveNameType;

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoTypeHasName_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DoNotHaveNameType),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveName("SomeOtherClassName");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenTypeHasName_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DoNotHaveNameType),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveName(nameof(DoNotHaveNameType));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have name equal to "DoNotHaveNameType",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.Tests.DoNotHaveNameType
					             ]
					             """).AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeHasName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameTypeNeg));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName("SomeOtherClassName"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameTypeNeg" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             at least one has name equal to "SomeOtherClassName",
					             but none of 1 did

					             Collection:
					             [
					               ThatTypes.DoNotHaveName.NegatedTests.DoNotHaveNameTypeNeg
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypeHasName_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameTypeNeg));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName(nameof(DoNotHaveNameTypeNeg)));
				}

				await That(Act).DoesNotThrow();
			}

			private class DoNotHaveNameTypeNeg;
		}
	}
}
