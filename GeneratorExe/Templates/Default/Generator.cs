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

      //load available types
      Type[] allTypesOfAssembly;
      try {
        allTypesOfAssembly = ass.GetTypes();
      }
      catch (ReflectionTypeLoadException ex) {
        allTypesOfAssembly = ex.Types.Where((t) => t != null).ToArray();
      }

      //transform patterns to regex
      List<Regex> patterns = new List<Regex>();
      if (!string.IsNullOrWhiteSpace(cfg.interfaceTypeNamePattern)) {
        patterns.Add(new Regex("^(" + Regex.Escape(cfg.interfaceTypeNamePattern).Replace("\\*", ".*?") + ")$", RegexOptions.Compiled));
      }
      if (cfg.interfaceTypeNamePatterns != null) {
        foreach(string p in cfg.interfaceTypeNamePatterns) {
          patterns.Add(new Regex("^(" + Regex.Escape(p).Replace("\\*", ".*?") + ")$", RegexOptions.Compiled));
        }
      }

      //filter the types
      IEnumerable<Type> typesMatchingPattern = new Type[] { };
      foreach (Regex r in patterns) {
        Type[] matches = allTypesOfAssembly.Where((Type i) => r.IsMatch(i.FullName)).ToArray();
        typesMatchingPattern = typesMatchingPattern.Union(matches);
      }
      Type[] typesToInclude = typesMatchingPattern.Distinct().ToArray();

      //iterate the types
      var modelTypeBuffer = new List<Type>();
      foreach (Type tp in typesToInclude) {
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

              Type cleanParamType = svcMthPrm.ParameterType;
              if (svcMthPrm.IsOut) { //ist nochmal gewrapped!
                cleanParamType = cleanParamType.GetElementType();
              }

              string pTypeName;
              bool nullable;
              bool array;
              string collectionType;
              Type[] elementTypes;

              pTypeName = cleanParamType.GetTypeNameSave(out nullable, out elementTypes, out array, out collectionType);
              var elementTypeNamesOrLinks = new List<string>();
              foreach (Type et in elementTypes) {
                string argName = et.GetTypeNameSave();
                if (this.ProcessCandidate(et, modelTypeBuffer, cfg.namespaceWildcardsForModelImport)) {
                  elementTypeNamesOrLinks.Add($"[{argName}](#{argName})");
                }
                else {
                  elementTypeNamesOrLinks.Add(argName);
                }
              }

              if (!string.IsNullOrWhiteSpace(collectionType)) {
                pTypeName = $"*{collectionType}*<{string.Join(",", elementTypeNamesOrLinks)}>";
              }
              else if (array) {
                pTypeName = $"{elementTypeNamesOrLinks[0]}[] *(array)*";
              }
              else {
                pTypeName = elementTypeNamesOrLinks[0];
              }

              if (nullable || (svcMthPrm.IsOptional && svcMthPrm.ParameterType.IsValueType)) {
                pTypeName = pTypeName + "?";
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
            string collectionType;
            Type[] elementTypes;

            pTypeName = svcMth.ReturnType.GetTypeNameSave(out nullable, out elementTypes, out array, out collectionType);
            var elementTypeNamesOrLinks = new List<string>();
            foreach (Type et in elementTypes) {
              string argName = et.GetTypeNameSave();
              if (this.ProcessCandidate(et, modelTypeBuffer, cfg.namespaceWildcardsForModelImport)) {
                elementTypeNamesOrLinks.Add($"[{argName}](#{argName})");
              }
              else {
                elementTypeNamesOrLinks.Add(argName);
              }
            }

            if (!string.IsNullOrWhiteSpace(collectionType)) {
              pTypeName = $"*{collectionType}*<{string.Join(",", elementTypeNamesOrLinks)}>";
            }
            else if (array) {
              pTypeName = $"{elementTypeNamesOrLinks[0]}[] *(array)*";
            }
            else {
              pTypeName = elementTypeNamesOrLinks[0];
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

      if (modelTypeBuffer.Any()) {

        writer.WriteLine($"# Models:");

        foreach (var modelType in modelTypeBuffer.OrderBy((t)=> t.FullName)) {
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
            string collectionType;
            Type[] elementTypes;

            pTypeName = prop.PropertyType.GetTypeNameSave(out nullable, out elementTypes, out array, out collectionType);
            var elementTypeNamesOrLinks = new List<string>();
            foreach (Type et in elementTypes) {
              string argName = et.GetTypeNameSave();
              if (this.ProcessCandidate(et, modelTypeBuffer, cfg.namespaceWildcardsForModelImport)) {
                elementTypeNamesOrLinks.Add($"[{argName}](#{argName})");
              }
              else {
                elementTypeNamesOrLinks.Add(argName);
              }
            }

            if (!string.IsNullOrWhiteSpace(collectionType)) {
              pTypeName = $"*{collectionType}*<{string.Join(",", elementTypeNamesOrLinks)}>";
            }
            else if (array) {
              pTypeName = $"{elementTypeNamesOrLinks[0]}[] *(array)*";
            }
            else {
              pTypeName = elementTypeNamesOrLinks[0];
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
        Type[] elementTypes;
        bool nullable;
        bool array;
        string collectionType;
        prop.PropertyType.GetTypeNameSave(out nullable, out elementTypes, out array, out collectionType);
        foreach (Type elementType in elementTypes) {
        this.ProcessCandidate(elementType, modelTypes, wc);
        }
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


        string sinitizedFullName = t.Namespace + "." + t.GetTypeNameSave();
        if (Regex.IsMatch(sinitizedFullName, pattern)) {
          return true;
        }
      }
      return false;
    }

  }
}
