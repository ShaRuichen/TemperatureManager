using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using CefSharp;
using CefSharp.WinForms;

namespace Manager
{
    internal partial class Program : IProgram
    {
        private readonly ProgramHandler handler;
        private static readonly string appPath;
        private Control? control = null;

        static Program()
        {
            var domain = AppDomain.CurrentDomain;
            appPath = Path.Combine(domain.SetupInformation.ApplicationBase!,
                Environment.Is64BitProcess ? "x64" : "x86");
            domain.AssemblyResolve += AssemblyResolve;
        }

        public Program()
        {
            handler = new(this);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            var settings = new CefSettings
            {
                BrowserSubprocessPath = Path.Combine(appPath, "CefSharp.BrowserSubprocess.exe"),
            };
            var scheme = new CefCustomScheme
            {
                SchemeName = "resource",
                DomainName = "domain",
                SchemeHandlerFactory = new SchemeHandlerFactory(),
            };
            settings.RegisterScheme(scheme);
            _ = Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            var form = CreateForm();
            Application.Run(form);
            Cef.Shutdown();
        }

        private static Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var name = args.Name!;
            if (name.StartsWith("CefSharp"))
            {
                var assemblyName = name.Split(',', 2)[0] + ".dll";
                var archSpecificPath = Path.Combine(appPath, assemblyName);
                return File.Exists(archSpecificPath) ?
                    Assembly.LoadFile(archSpecificPath) : null;
            }
            return null;
        }
        private Form CreateForm()
        {
            var form = new Form
            {
                WindowState = FormWindowState.Maximized,
            };
            control = new ChromiumWebBrowser("resource://domain/index.html")
            {
                Dock = DockStyle.Fill,
                KeyboardHandler = new KeyBoardHander(),
                MenuHandler = new ContextMenuHandler(),
            };
            var browser = (ChromiumWebBrowser)control;
            browser.JavascriptObjectRepository.Register("csharp", new JSBinder(this), true);
            browser.LoadingStateChanged += (sender, e) =>
            {
                if (!e.IsLoading)
                {
                    browser.ExecuteScriptAsync("CefSharp.BindObjectAsync('csharp')");
                }
            };
            form.Controls.Add(browser);
            return form;
        }

        private class SchemeHandlerFactory : ISchemeHandlerFactory
        {
            public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
            {
                var url = request.Url;
                var key = url[18..];
                var path = Path.Combine(appPath[..^34] + "Frontend", key);
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ResourceHandler.FromStream(stream);
            }
        }
        private class KeyBoardHander : IKeyboardHandler
        {
            public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey) => false;
            public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
            {
#if DEBUG
                if (type == KeyType.RawKeyDown)
                {
                    const int VK_F5 = 0x74;
                    const int VK_F12 = 0x7b;
                    switch (windowsKeyCode)
                    {
                        case VK_F5: browser.Reload(); break;
                        case VK_F12: browser.ShowDevTools(); break;
                    }
                }
#endif
                return false;
            }
        }
        private class ContextMenuHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) => model.Clear();
            public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) => false;
            public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) { }
            public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) => false;
        }

        public void ExecuteJs(string jsText) => ((ChromiumWebBrowser)control!).ExecuteScriptAsync(jsText);
        public void ExecuteSql(string sqlText, params object[] args) => Sql.Execute(sqlText, args);
        public DataRowCollection ReadSql(string sqlText, params object[] args) => Sql.Read(sqlText, args);
    }
}
