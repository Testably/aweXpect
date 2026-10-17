using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     Sets the environment variables that decide about accepting snapshots for a single test and restores them
///     afterwards.
/// </summary>
/// <remarks>
///     The environment is shared by the whole process, so tests that use it belong to the
///     <see cref="EnvironmentCollection" />, which does not run in parallel with other tests.
/// </remarks>
internal sealed class EnvironmentScope : IDisposable
{
	private static readonly string[] Variables =
		["AWEXPECT_ACCEPT_SNAPSHOTS", "CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];

	private readonly CustomizationLifetime _acceptChanges;
	private readonly Dictionary<string, string?> _originalValues = new();

	public EnvironmentScope(params (string Variable, string Value)[] values)
	{
		foreach (string variable in Variables)
		{
			_originalValues[variable] = Environment.GetEnvironmentVariable(variable);
			Environment.SetEnvironmentVariable(variable, null);
		}

		foreach ((string variable, string value) in values)
		{
			Environment.SetEnvironmentVariable(variable, value);
		}

		_acceptChanges = Customize.aweXpect.Snapshots().AcceptChanges.Set(null);
	}

	public void Dispose()
	{
		_acceptChanges.Dispose();
		foreach (KeyValuePair<string, string?> originalValue in _originalValues)
		{
			Environment.SetEnvironmentVariable(originalValue.Key, originalValue.Value);
		}
	}
}

[CollectionDefinition(nameof(EnvironmentCollection), DisableParallelization = true)]
public sealed class EnvironmentCollection;
