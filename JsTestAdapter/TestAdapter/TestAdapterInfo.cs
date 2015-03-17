using JsTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;

namespace JsTestAdapter.TestAdapter
{
    public abstract class TestAdapterInfo
    {
        public abstract Uri ExecutorUri { get; }
        public abstract string Name { get; }
        public abstract bool IsTestContainer(string file);
        public abstract int GetContainerPriority(string file);
        public abstract string SettingsName { get; }
        public abstract string SettingsFileDirectory { get; }
        public abstract ITestLogger CreateLogger(IMessageLogger logger);
        public abstract ITestLogger CreateLogger(ILogger logger);
    }
}
