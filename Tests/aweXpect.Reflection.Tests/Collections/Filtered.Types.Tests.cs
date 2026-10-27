using System.Threading;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed class Types
	{
		public sealed class Tests
		{
			[Test]
			public async Task Types_FromConstructors_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Constructors().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo(
						"declaring types of constructors in types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Types_FromEvents_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Events().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of events in types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Types_FromFields_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Fields().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of fields in types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Types_FromMethods_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Methods().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of methods in types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Types_FromProperties_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Properties().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo(
						"declaring types of properties in types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Types_InAssembly_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Types();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description).IsEqualTo("types in assembly containing type Filtered.Types.Tests");
			}

			[Test]
			public async Task Which_WhenAnAsyncFilterYieldsOnTheSynchronizationContextOfTheCaller_ShouldNotDeadlock()
			{
				Reflection.Collections.Filtered.Types types = In.Type<Tests>().Which(new YieldingFilter());

				await SingleThreadedSynchronizationContext.Run(async () => await That(types).IsNotEmpty());
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task Which_WhenEnumeratedWithACancellationToken_ShouldPassTheTokenToTheFilter()
			{
				using CancellationTokenSource cts = new();
				TokenRecordingFilter<Type> filter = new();
				Reflection.Collections.Filtered.Types types = In.Type<Tests>().Which(filter);

				List<Type> result = [];
				await foreach (Type type in types.WithCancellation(cts.Token))
				{
					result.Add(type);
				}

				await That(result).IsEqualTo([typeof(Tests),]);
				await That(filter.Tokens).IsEqualTo([cts.Token,]);
			}

			[Test]
			public async Task Which_WhenTheTokenIsCanceledDuringTheEnumeration_ShouldStopTheEnumeration()
			{
				using CancellationTokenSource cts = new();
				TokenRecordingFilter<Type> filter = new(cts.Cancel);
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Types().Which(filter);

				async Task Act()
				{
					await foreach (Type type in types.WithCancellation(cts.Token))
					{
						await That(type).IsNotNull();
					}
				}

				await That(Act).Throws<OperationCanceledException>().WithMessage("The operation was canceled.");
				await That(filter.Tokens).HasCount(1)
					.Because("no further type may reach the filter after the token was canceled");
			}
#endif

			private sealed class YieldingFilter : IFilter<Type>
			{
				public async ValueTask<bool> Applies(Type value, CancellationToken cancellationToken)
				{
					await Task.Yield();
					return true;
				}

				public string Describes(string text) => text;
			}
		}
	}
}
