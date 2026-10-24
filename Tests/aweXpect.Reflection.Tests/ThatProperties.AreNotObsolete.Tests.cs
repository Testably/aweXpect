using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatProperties
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllPropertiesAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainObsoleteProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!,
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllPropertiesAreNotObsolete_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainObsoleteProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!,
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllPropertiesAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllPropertiesAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainObsoleteProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!, typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
