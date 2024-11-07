using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Inspection {

  internal static class InspectionExtensions {

    public static string GetTypeNameSave(this Type extendee) {
      return extendee.GetTypeNameSave(out var dummy1, out var dummy2, out var dummy3);
    }

    public static string GetTypeNameSave(this Type extendee, out bool isNullable,out Type t,out bool isArray) {

      isArray = extendee.IsArray;
      if (isArray) {
        isNullable = false;
        return extendee.GetElementType().GetTypeNameSave(out var dummy, out t, out var dummy2);
      }

     isNullable = (extendee.IsGenericType && extendee.GetGenericTypeDefinition() == typeof(Nullable<>));
      if (isNullable) {
        return extendee.GetGenericArguments()[0].GetTypeNameSave(out var dummy, out t, out var dummy2);
      }
      else {
        t = extendee;

        if (extendee.IsGenericType) {
          var typeNames = extendee.GetGenericArguments().Select((a)=>a.GetTypeNameSave(out var dummy1, out var dummy2, out var dummy3));
          return extendee.Name.Substring(0, extendee.Name .IndexOf("`")) + "_" + string.Join("_", typeNames);
        }

        return extendee.Name;
      }

    }

  }

}
