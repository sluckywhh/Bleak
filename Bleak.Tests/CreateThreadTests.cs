using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace Bleak.Tests
{
    public sealed class CreateThreadTests : IDisposable
    {
        private readonly string _dllPath;

        private readonly Process _process;

        public CreateThreadTests()
        {
            _dllPath = Path.Combine(Path.GetFullPath(@"..\..\..\TestDll\"), "TestDll.dll");

            _process = new Process {StartInfo = {CreateNoWindow = true, FileName = "notepad.exe", UseShellExecute = true, WindowStyle = ProcessWindowStyle.Hidden}};

            _process.Start();

            _process.WaitForInputIdle();
        }

        public void Dispose()
        {
            _process.Kill();

            _process.Dispose();
        }

        [Fact]
        public void TestEject()
        {
            using var injector = new Injector(_process.Id, _dllPath, InjectionMethod.CreateThread);

            injector.InjectDll();

            injector.EjectDll();

            _process.Refresh();

            Assert.DoesNotContain(_process.Modules.Cast<ProcessModule>(), module => module.FileName == _dllPath);
        }

        [Fact]
        public void TestInject()
        {
            using var injector = new Injector(_process.Id, _dllPath, InjectionMethod.CreateThread);

            injector.InjectDll();

            _process.Refresh();

            Assert.Contains(_process.Modules.Cast<ProcessModule>(), module => module.FileName == _dllPath);
        }

        [Fact]
        public void TestWithHideDllFromPebFlag()
        {
            using var injector = new Injector(_process.Id, _dllPath, InjectionMethod.CreateThread, InjectionFlags.HideDllFromPeb);

            injector.InjectDll();

            _process.Refresh();

            Assert.DoesNotContain(_process.Modules.Cast<ProcessModule>(), module => module.FileName == _dllPath);
        }

        [Fact]
        public void TestWithRandomiseDllHeadersFlag()
        {
            using var injector = new Injector(_process.Id, _dllPath, InjectionMethod.CreateThread, InjectionFlags.RandomiseDllHeaders);

            injector.InjectDll();

            _process.Refresh();

            Assert.Contains(_process.Modules.Cast<ProcessModule>(), module => module.FileName == _dllPath);
        }

        [Fact]
        public void TestWithRandomiseDllNameFlag()
        {
            using var injector = new Injector(_process.Id, _dllPath, InjectionMethod.CreateThread, InjectionFlags.RandomiseDllName);

            injector.InjectDll();

            _process.Refresh();

            Assert.DoesNotContain(_process.Modules.Cast<ProcessModule>(), module => module.FileName == _dllPath);
        }
    }
}