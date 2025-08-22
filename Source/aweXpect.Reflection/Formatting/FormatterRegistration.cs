using System;
using aweXpect.Core.Initialization;

namespace aweXpect.Reflection.Formatting;

internal sealed class FormatterRegistration : IAweXpectInitializer, IDisposable
{
	private static FormatterRegistration? _instance;
	private IDisposable[] _disposables = [];

	public FormatterRegistration()
	{
		if (_instance != null)
		{
			throw new InvalidOperationException(
				"A FormatterRegistration instance is already initialized. Dispose the existing instance before creating a new one.");
		}

#pragma warning disable S3010
		_instance = this;
#pragma warning restore S3010
	}

	internal static FormatterRegistration Instance
		=> _instance ??= new FormatterRegistration();

	public void Initialize() => _disposables =
	[
		ValueFormatter.Register(new ConstructorFormatter())
	];

	public void Dispose()
	{
		foreach (IDisposable? disposable in _disposables)
		{
			disposable.Dispose();
		}

#pragma warning disable S2696
		_instance = null;
#pragma warning restore S2696
	}
}
