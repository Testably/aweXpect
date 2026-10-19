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
					typeof(DoNotHaveNameType), null,
				};

				async Task Act()
				{
					await That(subject).DoNotHaveName("SomeOtherClassName");
				}

				await That(Act).DoesNotThrow();
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
					             all have name not equal to "DoNotHaveNameType",
					             but it contained not matching items

					             Not matching items:
					             [
					               *DoNotHaveNameType*
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
					             all have name not ending with "HaveNameType",
					             but it contained not matching items

					             Not matching items:
					             [
					               *DoNotHaveNameType*
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
					typeof(DoNotHaveNameType), null,
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
					             all have name not equal to "DoNotHaveNameType",
					             but it contained not matching items

					             Not matching items:
					             [
					               *DoNotHaveNameType*
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
					             not all have name not equal to "SomeOtherClassName",
					             but it only contained matching items

					             Collection:
					             [
					               *DoNotHaveNameTypeNeg*
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
