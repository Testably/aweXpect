using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHaveName
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenTypeHasName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameType));

				async Task Act()
				{
					await That(subject).DoNotHaveName(nameof(DoNotHaveNameType));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameType" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             all have name not equal to "DoNotHaveNameType",
					             but it contained not matching items [
					               *DoNotHaveNameType*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypeMatchesSuffix_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameType));

				async Task Act()
				{
					await That(subject).DoNotHaveName("HaveNameType").AsSuffix();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameType" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             all have name not ending with "HaveNameType",
					             but it contained not matching items [
					               *DoNotHaveNameType*
					             ]
					             """).AsWildcard();
			}

			private class DoNotHaveNameType;

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have name not equal to "DoNotHaveNameType",
					             but it contained not matching items [
					               *DoNotHaveNameType*
					             ]
					             """).AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNoTypeHasName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(DoNotHaveNameTypeNeg));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveName("SomeOtherClassName"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with name equal to "DoNotHaveNameTypeNeg" in assembly containing type ThatTypes.DoNotHaveName.Tests
					             not all have name not equal to "SomeOtherClassName",
					             but it only contained matching items [
					               *DoNotHaveNameTypeNeg*
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
