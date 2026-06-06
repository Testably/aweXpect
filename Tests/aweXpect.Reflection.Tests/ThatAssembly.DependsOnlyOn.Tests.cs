using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DependsOnlyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllowedMatchesIgnoringCase_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("AWExPECT.cORE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it also had dependencies on [*]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenCustomizedPrefixEndsWithDot_ShouldExcludeMatchingAssemblies()
			{
				Assembly subject = typeof(In).Assembly;

				// A customized prefix in the natural trailing-dot form ("aweXpect.") must exclude the
				// aweXpect.Core reference at the (then explicit) name-segment boundary.
				using (Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes
					       .Set(["System", "netstandard", "mscorlib", "Microsoft", "aweXpect.",]))
				{
					async Task Act()
					{
						await That(subject).DependsOnlyOn();
					}

					await That(Act).DoesNotThrow();
				}
			}

			[Fact]
			public async Task WhenMultipleAllowedAssembliesAreGiven_ShouldDescribeAssemblies()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("First", "Second");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "First" or equal to "Second",
					             but it also had dependencies on *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoAllowedAssembliesAreGiven_ShouldDescribeNoAssemblies()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on no assemblies,
					             but it also had dependencies on *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldFail()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have dependencies only on assemblies equal to "aweXpect.Core",
					             but it only had dependencies on the allowed assemblies
					             """);
			}
		}
	}
}
