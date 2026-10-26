using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DoNotTarget
	{
#if NET8_0
		private const string CurrentTarget = "net8.0";
#elif NET10_0
		private const string CurrentTarget = "net10.0";
#else
		private const string CurrentTarget = "net48";
#endif

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesDoNotTargetTheFramework_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotTarget("net99.0");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesTargetTheFramework_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotTarget(CurrentTarget);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              none target equal to "{CurrentTarget}",
					              but 1 of 1 did

					              Matching items:
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
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotTarget(CurrentTarget);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingIgnoringCase_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotTarget(CurrentTarget.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              none target equal to "{CurrentTarget.ToUpperInvariant()}" ignoring case,
					              but 1 of 1 did

					              Matching items:
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
			public async Task WhenOneAssemblyIsNull_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [null,];

				async Task Act()
				{
					await That(subject).DoNotTarget("net99.0");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none target equal to "net99.0",
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<Assembly?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotTarget("net99.0");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none target equal to "net99.0",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesDoNotTargetTheFramework_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotTarget("net99.0"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             at least one targets equal to "net99.0",
					             but none of 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssembliesTargetTheFramework_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotTarget(CurrentTarget));
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAssembliesDoNotTargetTheFramework_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).DoNotTarget("net99.0");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesTargetTheFramework_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).DoNotTarget(CurrentTarget);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              none target equal to "{CurrentTarget}",
					              but 1 of 1 did

					              Matching items:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]

					              Collection:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]
					              """).AsWildcard();
			}
		}
#endif
	}
}
