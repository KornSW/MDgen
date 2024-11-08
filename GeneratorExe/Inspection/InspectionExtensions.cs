using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Inspection {

  internal static class InspectionExtensions {

    public static string GetTypeNameSave(this Type extendee) {
      return extendee.GetTypeNameSave(out var dummy1, out var dummy2, out var dummy3, out var dummy4);
    }

    /// <summary>
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="isNullable"></param>
    /// <param name="elementTypes">nie leer!
    /// immer entweder der extendee oder bei (nullable/list/array) das item oder sonst (Dict/Generic) die GenArgs</param>
    /// <param name="isArray"></param>
    /// <param name="collectionType"></param>
    /// <returns></returns>
    public static string GetTypeNameSave(this Type extendee, out bool isNullable, out Type[] elementTypes, out bool isArray, out string collectionType) {
      //defaults
      elementTypes = new Type[] { extendee };
      isNullable = false;
      collectionType = null;

      isArray = extendee.IsArray;
      if (isArray) {
        isNullable = false;
        return extendee.GetElementType().GetTypeNameSave(out var dummy, out elementTypes, out var dummy2, out var dummy4);
      }

      if (extendee.IsGenericType) {
        Type genTypeBase = extendee.GetGenericTypeDefinition();
        Type[] genArgs = extendee.GetGenericArguments();

        var sanitizedGenArgs = new List<Type>();
        foreach (Type genArg in genArgs) {
          genArg.GetTypeNameSave(out var dummy, out var t, out var dummy2, out var dummy4);
          sanitizedGenArgs.Add(t[0]);
        }
        elementTypes = sanitizedGenArgs.ToArray();

        isNullable = (genTypeBase == typeof(Nullable<>));
        if (isNullable) {
          //RECURSION!
          var result = elementTypes[0].GetTypeNameSave(out var dummy, out elementTypes, out isArray, out collectionType);
          isNullable = true; //kommt also nur nochmal oben drauf
          return result;
        }
        if (genTypeBase == typeof(List<>)) {
          collectionType = "List";
          //durchreichen und nach kapsel verheimlichen (wird ja über isNullable rausgegeben)
          return elementTypes[0].GetTypeNameSave();
        }

        if (genTypeBase == typeof(Dictionary<,>)) {
          collectionType = "Dict";
          //durchreichen und nach kapsel verheimlichen (wird ja über isNullable rausgegeben)
          return elementTypes[0].GetTypeNameSave();
        }

        //any other generic type (make just a sanitized name)
        var typeNamesForAggregation = genArgs.Select((a) => a.GetTypeNameSave(out var dummy1, out var dummy2, out var dummy3, out var dummy4));
        return extendee.Name.Substring(0, extendee.Name.IndexOf("`")) + "<" + string.Join(",", typeNamesForAggregation) + ">";
      }
      else {
        return extendee.Name;
      }

    }

  }

}
