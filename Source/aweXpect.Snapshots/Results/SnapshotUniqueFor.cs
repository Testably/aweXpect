using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The environments that a snapshot expectation can keep a snapshot of its own for, when the subject legitimately
///     differs between them, e.g. an exception message between .NET Framework and .NET.
/// </summary>
/// <remarks>
///     Each one adds its value to the snapshot, e.g.
///     <c>&lt;Class&gt;.&lt;Method&gt;@net8.0@windows.awexpect-snapshot.txt</c>.
///     Combined by repeating <c>.UniqueFor</c>, the values always keep the order target framework, operating system,
///     architecture, and adding one again changes nothing. A missing snapshot of the environment does not fall back to
///     the snapshot that every environment shares.
/// </remarks>
/// <typeparam name="TResult">The result of the snapshot expectation, which each environment returns.</typeparam>
public sealed class SnapshotUniqueFor<TResult>
{
	private readonly SnapshotOptions _options;
	private readonly TResult _result;

	internal SnapshotUniqueFor(TResult result, SnapshotOptions options)
	{
		_result = result;
		_options = options;
	}

	/// <summary>
	///     Keeps a snapshot for each architecture of the process, e.g. <c>x64</c>, <c>x86</c> or <c>arm64</c>.
	/// </summary>
	public TResult Architecture()
		=> Add(SnapshotEnvironment.Part.Architecture, SnapshotEnvironment.GetArchitecture());

	/// <summary>
	///     Keeps a snapshot for each operating system: <c>windows</c>, <c>linux</c>, <c>osx</c> or <c>freebsd</c>.
	/// </summary>
	/// <remarks>
	///     On any other operating system it throws a <see cref="System.PlatformNotSupportedException" />.
	/// </remarks>
	public TResult OperatingSystem()
		=> Add(SnapshotEnvironment.Part.OperatingSystem, SnapshotEnvironment.GetOperatingSystem());

	/// <summary>
	///     Keeps a snapshot for each target framework of the test project, as the build knows it, e.g. <c>net8.0</c> or
	///     <c>net48</c>.
	/// </summary>
	/// <remarks>
	///     The build targets of aweXpect.Snapshots record the target framework for the test project that contains the
	///     test file; without it, it throws an <see cref="System.InvalidOperationException" />.
	/// </remarks>
	public TResult TargetFramework()
		=> Add(SnapshotEnvironment.Part.TargetFramework, SnapshotEnvironment.GetTargetFramework(_options.Id.SourceFile));

	private TResult Add(SnapshotEnvironment.Part part, string value)
	{
		_options.UniqueFor(part, value);
		return _result;
	}
}
