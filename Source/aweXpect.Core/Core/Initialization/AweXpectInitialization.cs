using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using aweXpect.Core.Adapters;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Core.Initialization;

internal static class AweXpectInitialization
{
	public static Lazy<InitializationState> State { get; } = new(Initialize);

	internal static void EnsureInitialized()
	{
		if (State.IsValueCreated)
		{
			return;
		}

		_ = State.Value;
	}

	/// <summary>
	///     Detects a test framework adapter from the provided types.
	/// </summary>
	/// <returns>
	///     An instance of <see cref="ITestFrameworkAdapter" /> if a matching framework is found;
	///     otherwise <see langword="null" />.
	/// </returns>
	internal static ITestFrameworkAdapter? DetectFramework(IEnumerable<Type> types)
	{
		Type frameworkInterface = typeof(ITestFrameworkAdapter);
		foreach (Type frameworkType in types
			         .Where(x => x is { IsClass: true, IsAbstract: false, })
			         .Where(frameworkInterface.IsAssignableFrom))
		{
			try
			{
				ITestFrameworkAdapter? testFramework =
					(ITestFrameworkAdapter?)Activator.CreateInstance(frameworkType);
				if (testFramework?.IsAvailable == true)
				{
					return testFramework;
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
						$"Could not instantiate test framework '{Formatter.Format(frameworkType)}'!", ex)
					.LogTrace();
			}
		}

		return null;
	}

	private static InitializationState Initialize()
	{
		ExecuteCustomInitializers();

		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
			         .Where(IsAssemblyIncluded))
		{
			try
			{
				ITestFrameworkAdapter? testFrameworkAdapter = DetectFramework(
					assembly.GetTypes().Where(x => !x.IsNestedPrivate));
				if (testFrameworkAdapter is not null)
				{
					return new InitializationState(testFrameworkAdapter);
				}
			}
			catch (ReflectionTypeLoadException ex)
			{
				// Ignore any ReflectionTypeLoadException and continue with the next assembly.
				Debug.WriteLine($"ReflectionTypeLoadException caught: {ex.Message}");
				Debug.WriteLine(ex.StackTrace);
			}
		}

		return new InitializationState(new FallbackTestFramework());
	}

	private static bool IsAssemblyIncluded(Assembly assembly)
		=> IsAssemblyNameIncluded(assembly.GetName().Name);

	/// <summary>
	///     Checks whether an assembly with the given <paramref name="assemblyName" /> (its simple name) should be scanned,
	///     i.e. it is not excluded by any of the configured
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" />.
	/// </summary>
	/// <remarks>
	///     A prefix matches the assembly name only at a name-segment boundary, so that e.g. <c>System</c> excludes
	///     <c>System</c> and <c>System.Net.Http</c>, but not an unrelated assembly named <c>Systemics</c>.<br />
	///     Assemblies without a name are never scanned, as they cannot host a test framework adapter or initializer.
	/// </remarks>
	internal static bool IsAssemblyNameIncluded(string? assemblyName)
	{
		if (string.IsNullOrEmpty(assemblyName))
		{
			return false;
		}

		return Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get()
			.All(prefix => assemblyName != prefix &&
			               !assemblyName!.StartsWith(prefix + ".", StringComparison.Ordinal));
	}

	private static void ExecuteCustomInitializers()
	{
		Type initializerInterface = typeof(IAweXpectInitializer);
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
			         .Where(IsAssemblyIncluded))
		{
			try
			{
#pragma warning disable S2259
				foreach (Type initializerType in assembly.GetTypes()
					         .Where(type => type is { IsClass: true, IsAbstract: false, } &&
					                        initializerInterface.IsAssignableFrom(type)))
				{
					try
					{
						IAweXpectInitializer? initializer =
							(IAweXpectInitializer?)Activator.CreateInstance(initializerType);
						initializer?.Initialize();
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException(
								$"Could not instantiate initializer '{Formatter.Format(initializerType)}'!", ex)
							.LogTrace();
					}
				}
#pragma warning restore S2259
			}
			catch (ReflectionTypeLoadException)
			{
				// Ignore any ReflectionTypeLoadException and continue with the next assembly.
			}
		}
	}

	internal class InitializationState(ITestFrameworkAdapter testFramework)
	{
		public ValueFormatter Formatter { get; } = new();

		/// <summary>
		///     Throws a framework-specific exception to indicate a skipped unit test.
		/// </summary>
		[DoesNotReturn]
		[StackTraceHidden]
		public void Skip(string message)
		{
			try
			{
				testFramework.Skip(message);
			}
			catch (Exception ex)
			{
				ex.LogTrace();
				throw;
			}
		}

		/// <summary>
		///     Throws a framework-specific exception to indicate a failing unit test.
		/// </summary>
		[DoesNotReturn]
		[StackTraceHidden]
		public void Fail(string message)
		{
			try
			{
				testFramework.Fail(message);
			}
			catch (Exception ex)
			{
				ex.LogTrace();
				throw;
			}
		}

		/// <summary>
		///     Throws a framework-specific exception to indicate an inconclusive unit test.
		/// </summary>
		[DoesNotReturn]
		[StackTraceHidden]
		public void Inconclusive(string message)
		{
			try
			{
				testFramework.Inconclusive(message);
			}
			catch (Exception ex)
			{
				ex.LogTrace();
				throw;
			}
		}
	}

	private sealed class FallbackTestFramework : ITestFrameworkAdapter
	{
		#region ITestFrameworkAdapter Members

		public bool IsAvailable => false;

		[DoesNotReturn]
		[StackTraceHidden]
		public void Fail(string message)
			=> throw new FailException(message);

		[DoesNotReturn]
		[StackTraceHidden]
		public void Skip(string message)
			=> throw new SkipException(message);

		[DoesNotReturn]
		[StackTraceHidden]
		public void Inconclusive(string message)
			=> throw new InconclusiveException(message);

		#endregion
	}
}
