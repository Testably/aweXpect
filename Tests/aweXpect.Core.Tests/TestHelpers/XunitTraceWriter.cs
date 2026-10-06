using aweXpect.Customization;

namespace aweXpect.Core.Tests.TestHelpers;

internal class XunitTraceWriter : ITraceWriter, IDisposable
{
	private readonly CustomizationLifetime _disposable;
	private readonly ITestOutputHelper _testOutputHelper;

	public XunitTraceWriter(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
		_disposable = Customize.aweXpect.EnableTracing(this);
	}

	public void Dispose() => _disposable.Dispose();

	public void WriteMessage(string message)
		=> _testOutputHelper.WriteLine(message);

	public void WriteException(Exception exception)
		=> _testOutputHelper.WriteLine(exception.ToString());
}
