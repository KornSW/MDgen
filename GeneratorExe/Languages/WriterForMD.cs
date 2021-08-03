using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Languages {

  public class WriterForMD : CodeWriterBase {

    public WriterForMD(TextWriter targetWriter, CodeWritingSettings cfg) : base(targetWriter, cfg) {
    }
    public override void WriteImport(string @namespace) {
      this.WriteLine($"Imports: {@namespace};");
    }

    public override void WriteBeginNamespace(string name) {
      name = this.Escape(name);
      this.WriteLineAndPush($"Namespace: {name} {{");
    }

    public override void WriteEndNamespace() {
      this.PopAndWriteLine("}");
    }

    private string Escape(string input) {
      //TODO: implement escaping
      return input;
    }


    public static string TransformHyperlinksWithinSummary(string sum) {
      if (!sum.Contains("<see")) {
        return sum;
      }

      int beginIdx = sum.IndexOf("<see");
      while (beginIdx >= 0) {

        int endIdx = sum.IndexOf("</see>", beginIdx);
        int blockUntil;
        string left;

        if (endIdx < 0) {
          blockUntil = sum.Length;
          left = "";
        }
        else {
          blockUntil = endIdx;
          left = sum.Substring(endIdx + 6);
        }

        int blockStart = beginIdx + 5;

        string block = sum.Substring(blockStart, blockUntil - blockStart);
        int urlStart = block.IndexOf("href=\"") + 6;
        int urlEnd = block.IndexOf("\">");
        string url = block.Substring(urlStart, urlEnd - urlStart);
        string title = block.Substring(urlEnd + 2);

        sum = sum.Substring(0, beginIdx) + $"[{title}]({url})" + left;
        beginIdx = sum.IndexOf("<see");
      }

      return sum;
    }

  }

}
