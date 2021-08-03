using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Default {

  public class Cfg: RootCfg {

    public int countOfPrefixCharsToRemove = 0;
    public string[] namespaceWildcardsForModelImport = new string[] { };

  }

}
