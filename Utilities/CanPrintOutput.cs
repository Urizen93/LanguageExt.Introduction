using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace LanguageExt.Introduction.Utilities;

public abstract class CanPrintOutput : Controller
{
    private readonly ITestOutputHelper _output;

    public CanPrintOutput(ITestOutputHelper output) => _output = output;

    protected void WriteLine(object? value = null) => _output.WriteLine(value?.ToString() ?? string.Empty);
}