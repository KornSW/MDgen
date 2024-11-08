﻿using CodeGeneration.Languages;
using System;

namespace CodeGeneration {

  public class RootCfg : CodeWritingSettings {

    //INPUT-BASICS

    public string inputFile = null;
    public string interfaceTypeNamePattern = null;
    public string[] interfaceTypeNamePatterns = null;

    public string assemblyResolveDir = null;

    public string[] requireXmlDocForNamespaces = new string[] { };

    //OUTPUT-BASICS

    public string template = null;

    //DEBUGGING
    public int waitForDebuggerSec = 0;

  }

}
