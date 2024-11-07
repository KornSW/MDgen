using CodeGeneration.Inspection;
using CodeGeneration.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CodeGeneration.Default {

  public class Generator {

    public void Generate(CodeWriterBase writer, Cfg cfg) {

      if(writer.GetType() != typeof(WriterForMD)) {
        throw new Exception("For the selected template is currenty only language 'MD' supported!");
      }

      var inputFileFullPath = Path.GetFullPath(cfg.inputFile);
      Program.AddResolvePath(Path.GetDirectoryName(inputFileFullPath));
      Assembly ass = Assembly.LoadFile(inputFileFullPath);

      Type[] types;
      try {
        types = ass.GetTypes();
      }
      catch (ReflectionTypeLoadException ex) {
        types = ex.Types.Where((t) => t != null).ToArray();
      }

      //transform patterns to regex
      cfg.interfaceTypeNamePattern = "^(" + Regex.Escape(cfg.interfaceTypeNamePattern).Replace("\\*", ".*?") + ")$";
      types = types.Where((Type i) => Regex.IsMatch(i.FullName, cfg.interfaceTypeNamePattern)).ToArray();

      var modelTypes = new List<Type>();

      foreach (Type tp in types) {
        string typeName = tp.Name;
        if(cfg.countOfPrefixCharsToRemove > 0) {
          typeName = typeName.Substring(cfg.countOfPrefixCharsToRemove);
        }

        string svcIntDoc = XmlCommentAccessExtensions.GetDocumentation(tp, singleLine: false);
        writer.WriteLine($"# {typeName}");

        if(!String.IsNullOrWhiteSpace(svcIntDoc)) {
          writer.WriteLine(WriterForMD.TransformHyperlinksWithinSummary(svcIntDoc));
        }

        writer.WriteLine("");
        writer.WriteLine($"### Methods:");

        var methods = new List<MethodInfo>();
        this.CollectMethodsRecursive(methods, tp);

        foreach (MethodInfo svcMth in methods) {
          string svcMthDoc = XmlCommentAccessExtensions.GetDocumentation(svcMth, false);
          writer.WriteLine("");
          writer.WriteLine("");
          writer.WriteLine("");
          writer.WriteLine($"## .{svcMth.Name}");

          if (!String.IsNullOrWhiteSpace(svcMthDoc)) {
            writer.WriteLine(WriterForMD.TransformHyperlinksWithinSummary(svcMthDoc));
          }

          var svcMthPrms = svcMth.GetParameters();

          if (!svcMthPrms.Any()) {
            writer.WriteLine("no parameters");
          }
          else {

            writer.WriteLine($"#### Parameters:");

            writer.WriteLine($"|Name|Type|Description|");
            writer.WriteLine($"|----|----|-----------|");

            //if (!svcMthPrms.Any()) {
            //  writer.WriteLine($"|(none)|||");
            //}

            foreach (ParameterInfo svcMthPrm in svcMthPrms) {
              string svcMthPrmDoc = XmlCommentAccessExtensions.GetDocumentation(svcMthPrm);
              if (String.IsNullOrWhiteSpace(svcMthPrmDoc)) {
                svcMthPrmDoc = XmlCommentAccessExtensions.GetDocumentation(svcMthPrm.ParameterType);
              }
              //directlyUsedModelTypes.Add(svcMthPrm.ParameterType);

              string description;
              if (svcMthPrm.IsOut) {
                description = "**OUT**-Param ";
              }
              else if (svcMthPrm.ParameterType.IsByRef) {
                description = "**IN/OUT**-Param ";
              }
              else {
                description = "**IN**-Param ";
              }
              if (svcMthPrm.IsOptional) {
                description = description + "(optional)";
              }
              else if (!svcMthPrm.IsOut) {
                description = description + "(required)";
              }
              if (!String.IsNullOrWhiteSpace(svcMthPrmDoc)) {
                description = description + ": " + svcMthPrmDoc;
              }

              string pTypeName;
              bool nullable;
              bool array;
              Type realType;
              if (svcMthPrm.IsOut) {
                pTypeName = svcMthPrm.ParameterType.GetElementType().GetTypeNameSave(out nullable, out realType, out array);
              }
              else {
                pTypeName = svcMthPrm.ParameterType.GetTypeNameSave(out nullable, out realType, out array);
              }

              if (this.ProcessCandidate(realType, modelTypes, cfg.namespaceWildcardsForModelImport)) {
                pTypeName = $"[{pTypeName}](#{realType.GetTypeNameSave()})";
              }
              if (array) {
                pTypeName = pTypeName + "[] *(array)*";
              }
              if (nullable || (svcMthPrm.IsOptional && svcMthPrm.ParameterType.IsValueType)) {
                pTypeName = pTypeName + "? *(nullable)*";
                //initializer = " = null;";
              }

              writer.WriteLine($"|{svcMthPrm.Name}|{pTypeName}|{description}|");
            }

          }
          //writer.WriteLine("");
          //writer.WriteLine($"#### Return value:");
          if(svcMth.ReturnType != typeof(void)) {

            string pTypeName;
            bool nullable;
            bool array;
            Type realType;
            pTypeName = svcMth.ReturnType.GetTypeNameSave(out nullable, out realType, out array);

            if (this.ProcessCandidate(realType, modelTypes, cfg.namespaceWildcardsForModelImport)) {
              pTypeName = $"[{pTypeName}](#{realType.GetTypeNameSave()})";
            }
            if (array) {
              pTypeName = pTypeName + "[] *(array)*";
            }
            if (nullable) {
              pTypeName = pTypeName + "? *(nullable)*";
              //initializer = " = null;";
            }
            writer.WriteLine("**return value:** " + pTypeName);
          }
          else {
            writer.WriteLine("no return value (void)");
          }

        }//foreach Method

      }//foreach Interface

      writer.WriteLine("");
      writer.WriteLine("");
      writer.WriteLine("");

      if (modelTypes.Any()) {

        writer.WriteLine($"# Models:");

        foreach (var modelType in modelTypes.OrderBy((t)=> t.FullName)) {
          writer.WriteLine("");
          writer.WriteLine("");
          writer.WriteLine("");
          writer.WriteLine($"## {modelType.GetTypeNameSave()}");

          string mtDoc = XmlCommentAccessExtensions.GetDocumentation(modelType, singleLine: false);
          if (!String.IsNullOrWhiteSpace(mtDoc)) {
            writer.WriteLine(WriterForMD.TransformHyperlinksWithinSummary(mtDoc));
          }

          var props = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

          writer.WriteLine($"#### Fields:");

          writer.WriteLine($"|Name|Type|Description|");
          writer.WriteLine($"|----|----|-----------|");

          if (!props.Any()) {
            writer.WriteLine($"|(none)|||");
          }

          foreach (var prop in props) {

            string propDoc = XmlCommentAccessExtensions.GetDocumentation(prop);
            if (String.IsNullOrWhiteSpace(propDoc)) {
              propDoc = XmlCommentAccessExtensions.GetDocumentation(prop.PropertyType);
            }


            bool hasRequiredAttribute = prop.GetCustomAttributes().Where((a)=> a.GetType().FullName == typeof(RequiredAttribute).FullName).Any();

            string description;
            if (!hasRequiredAttribute) {
              description = "(optional)";
            }
            else {
              description = "(required)";
            }

            if (!String.IsNullOrWhiteSpace(propDoc)) {
              description = description + ": " + propDoc;
            }

            string pTypeName;
            bool nullable;
            bool array;
            Type realType;
            pTypeName = prop.PropertyType.GetTypeNameSave(out nullable, out realType,out array);
            
            if (this.ProcessCandidate(realType, modelTypes, cfg.namespaceWildcardsForModelImport)) {
              pTypeName = $"[{pTypeName}](#{realType.GetTypeNameSave()})";
            }
            if (array) {
              pTypeName = pTypeName + "[] *(array)*";
            }
            if (nullable || (!hasRequiredAttribute && prop.PropertyType.IsValueType)) {
              pTypeName = pTypeName + "? *(nullable)*";
              //initializer = " = null;";
            }

            writer.WriteLine($"|{prop.Name}|{pTypeName}|{description}|");

          }




        }

      }
    }

    private void CollectMethodsRecursive(List<MethodInfo> buffer, Type t) {
      if (t == typeof(object)) {
        return;
      }
      foreach (MethodInfo mi in t.GetMethods()) {
        if (!buffer.Contains(mi)) {
          buffer.Add(mi);
        }
      }
      foreach (Type subInterface in t.GetInterfaces()) {
        this.CollectMethodsRecursive(buffer, subInterface);
      }
      if (t.BaseType != null) {
        this.CollectMethodsRecursive(buffer, t.BaseType);
      }
    }

    private void CrawlReferences(Type currentType, List<Type> modelTypes, string[] wc) {

      this.ProcessCandidate(currentType.BaseType, modelTypes, wc);

      foreach (var prop in currentType.GetProperties()) {
        Type realType;
        bool nullable;
        bool array;
        prop.PropertyType.GetTypeNameSave(out nullable, out realType, out array);
        this.ProcessCandidate(realType, modelTypes, wc);
      }

    }

    private bool ProcessCandidate(Type candidate, List<Type> modelTypes, string[] wc) {
      if(candidate == null) {
        return false;
      }
      if (modelTypes.Contains(candidate)) {
        return true;
      }
      if (this.TypeMatchesModelWildCard(candidate, wc)) {
        modelTypes.Add(candidate);
        this.CrawlReferences(candidate, modelTypes, wc);
        return true;
      }
      return false;
    }

    private bool TypeMatchesModelWildCard(Type t, string[] namespaceWildcardsForModelImport) {
      foreach (var nswc in namespaceWildcardsForModelImport) {
        string pattern = "^(" + Regex.Escape(nswc).Replace("\\*", ".*?") + ")$";


        string sinitizedFullName = t.Namespace + "." + t.GetTypeNameSave(out var d1, out var d2, out var d3);
        if (Regex.IsMatch(sinitizedFullName, pattern)) {
          return true;
        }
      }
      return false;
    }

  }
}
