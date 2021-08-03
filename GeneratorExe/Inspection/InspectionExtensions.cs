using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Inspection {

  internal static class InspectionExtensions {

    public static string GetTypeNameSave(this Type extendee, out bool isNullable,out Type t,out bool isArray) {
      bool dummy;
      bool dummy2;

      isArray = extendee.IsArray;
      if (isArray) {
        isNullable = false;
        return extendee.GetElementType().GetTypeNameSave(out dummy, out t, out dummy);
      }

     isNullable = (extendee.IsGenericType && extendee.GetGenericTypeDefinition() == typeof(Nullable<>));
      if (isNullable) {
        return extendee.GetGenericArguments()[0].GetTypeNameSave(out dummy,out t, out dummy2);
      }
      else {
        t = extendee;
        return extendee.Name;
      }

    }

  }

}
