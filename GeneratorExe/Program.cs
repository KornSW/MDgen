using CodeGeneration.Languages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CodeGeneration {

  public class Program {

    internal static RootCfg _RootCfg = null;

    static int Main(string[] args) {
      String cfgRawJson = null;

      AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;

      try {

        try {
          if (args.Length > 0) {
            args[0] = Path.GetFullPath(args[0]);
            cfgRawJson = File.ReadAllText(args[0], Encoding.UTF8);
            _RootCfg = JsonConvert.DeserializeObject<RootCfg>(cfgRawJson);
          }
        }
        catch (Exception ex) {
          Console.WriteLine("/* ERROR reading '" + args[0] + "': " + ex.Message);
          Console.WriteLine("   please specify a filename via commandline-arg which has the following content:");
          Console.WriteLine(JsonConvert.SerializeObject(new RootCfg(), Formatting.Indented));
          Console.WriteLine("*/");
          System.Threading.Thread.Sleep(200);
          throw new Exception("ERROR reading '" + args[0] + "': " + ex.Message);
        }

        if(_RootCfg == null) {
          Console.WriteLine("/* ERROR: wrong input!");
          Console.WriteLine("   please specify a filename via commandline-arg which has the following content:");
          Console.WriteLine(JsonConvert.SerializeObject(new RootCfg(), Formatting.Indented));
          Console.WriteLine("*/");
          System.Threading.Thread.Sleep(200);
          throw new Exception("ERROR: wrong input: invalid configuration content!");
        }

        if(_RootCfg.waitForDebuggerSec > 0) {
          var timeout = DateTime.Now.AddSeconds(_RootCfg.waitForDebuggerSec);
          while (!Debugger.IsAttached && DateTime.Now < timeout) {
            Thread.Sleep(250);
          }
          if (Debugger.IsAttached) {
            Debugger.Break();
          }
        }

        if (!string.IsNullOrWhiteSpace(_RootCfg.assemblyResolveDir)) {
          AddResolvePath(_RootCfg.assemblyResolveDir);
        }

        XmlCommentAccessExtensions.RequireXmlDocForNamespaces = _RootCfg.requireXmlDocForNamespaces;

        CodeWriterBase langSpecificWriter = new WriterForMD(Console.Out, _RootCfg);
       
        if (String.IsNullOrWhiteSpace(_RootCfg.template)) {
          throw new Exception($"No Template was selected!");
        }
        else if (String.Equals(_RootCfg.template, "Default", StringComparison.CurrentCultureIgnoreCase)) {
          var templateSpecificCfg  = JsonConvert.DeserializeObject<Default.Cfg>(cfgRawJson);
          var gen = new Default.Generator();
          gen.Generate(langSpecificWriter, templateSpecificCfg);
        }
        else {
          throw new Exception($"Unknown Template '{_RootCfg.template}'");
        }

      }
      catch (Exception ex) {
        Console.WriteLine("/* ERROR: " + ex.Message);
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine("*/");
        File.WriteAllText(args[0] + ".Error.txt", ex.Message + Environment.NewLine + ex.StackTrace, Encoding.Default);
        Thread.Sleep(200);
        return 100;
      }

      Thread.Sleep(1000);
      return 0;

    }

    private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs e) {
      var assN = new AssemblyName(e.Name);
      string assemblyFullFileName = null;
      if (TryFindAssemblyFileByName(assN.Name + ".dll", ref assemblyFullFileName)) {
        try {
          return Assembly.LoadFrom(assemblyFullFileName);
        }
        catch {
        }
      }

      if (TryFindAssemblyFileByName(assN.Name + ".exe", ref assemblyFullFileName)) {
        try {
          return Assembly.LoadFrom(assemblyFullFileName);
        }
        catch {
        }
      }

      return null;
    }

    public static void AddResolvePath(string p) {
      _ResolvePaths.Add(p);
    }

    private static List<string> _ResolvePaths = new List<string>();

    public static bool TryFindAssemblyFileByName(string assemblyName, ref string assemblyFullFileName) {
      string assemblyFilePath;
      FileInfo assemblyFileInfo;
      foreach (var resolvePath in _ResolvePaths) {
        assemblyFilePath = Path.Combine(resolvePath, assemblyName);
        assemblyFileInfo = new FileInfo(assemblyFilePath);
        if (assemblyFileInfo.Exists && assemblyFileInfo.Length > 0L) {
          assemblyFullFileName = assemblyFilePath;
          return true;
        }
      }

      var sc = StringComparison.CurrentCultureIgnoreCase;
      if (!assemblyName.EndsWith(".dll", sc) && !assemblyName.EndsWith(".exe", sc)) {
        string argassemblyFullFileName = assemblyFullFileName + ".dll";
        if (TryFindAssemblyFileByName(assemblyName, ref argassemblyFullFileName)) {
          return true;
        }

        string argassemblyFullFileName1 = assemblyFullFileName + ".exe";
        if (TryFindAssemblyFileByName(assemblyName, ref argassemblyFullFileName1)) {
          return true;
        }
      }

      return false;
    }

  }

}
