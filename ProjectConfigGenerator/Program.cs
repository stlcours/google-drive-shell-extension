/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
﻿using System;
using System.IO;

namespace ProjectConfigGenerator
{
  // This program takes care of reading a ProjectConfig file and generating a
  // Config.h which can be compiled in the product. This is a way of centralizing
  // product name, version and other general infos.
  // Arguments:
  // arg1: <path to project config>
  // arg2: <path to output .h file>
  // arg3: optional suffix to the preproc defines.
  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("Generating Config for project.");
      string[] lines = File.ReadAllLines(args[0]);
      string prefix = "";
      if (args.Length == 3)
      {
        prefix = args[2] + "_";
      }


      string configFileContent = "";
      configFileContent += "// This file is generated by the build system.\n";
      configFileContent += String.Format("#ifndef {0}PROJECT_CONFIG_H\n", prefix);
      configFileContent += String.Format("#define {0}PROJECT_CONFIG_H\n", prefix);
      foreach (string line in lines)
      {
        if (line.StartsWith("Version "))
        {
          string versionNumber = line.Replace("Version ", "");
          configFileContent += String.Format("# define {0}VERSION_NUMBER {1}\n", prefix, versionNumber);
          configFileContent += String.Format("# define {0}VERSION_NUMBER_STR \"{1}\"\n", prefix, versionNumber);
          configFileContent += String.Format("# define {0}VERSION_NUMBER_WSTR L\"{1}\"\n", prefix, versionNumber);
          configFileContent += String.Format("# define {0}VERSION_NUMBER_COMMA {1}\n", prefix, versionNumber.Replace('.', ','));
        }
        else if (line.StartsWith("Title "))
        {
          string tittle = line.Replace("Title ", "");
          configFileContent += String.Format("# define {0}TITTLE_STR \"{1}\"\n", prefix, tittle);
          configFileContent += String.Format("# define {0}TITTLE_WSTR L\"{1}\"\n", prefix, tittle);
        }
        else if (line.StartsWith("Description "))
        {
          string description = line.Replace("Description ", "");
          configFileContent += String.Format("# define {0}DESCRIPTION_STR \"{1}\"\n", prefix, description);
          configFileContent += String.Format("# define {0}DESCRIPTION_WSTR L\"{1}\"\n", prefix, description);
        }
        else if (line.StartsWith("Company "))
        {
          string company = line.Replace("Company ", "");
          configFileContent += String.Format("# define {0}COMPANY_STR \"{1}\"\n", prefix, company);
          configFileContent += String.Format("# define {0}COMPANY_WSTR L\"{1}\"\n", prefix, company);
        }
        else if (line.StartsWith("Product "))
        {
          string product = line.Replace("Product ", "");
          configFileContent += String.Format("# define {0}PRODUCT_STR \"{1}\"\n", prefix, product);
          configFileContent += String.Format("# define {0}PRODUCT_WSTR L\"{1}\"\n", prefix, product);
        }
        else if (line.StartsWith("Copyright "))
        {
          string product = line.Replace("Copyright ", "");
          configFileContent += String.Format("# define {0}COPYRIGHT_STR \"{1}\"\n", prefix, product);
          configFileContent += String.Format("# define {0}COPYRIGHTY_WSTR L\"{1}\"\n", prefix, product);
        }
        else
        {
          int idEndPos = line.IndexOf(' ');
          string custom = line.Substring(0, idEndPos);
          string value = line.Substring(idEndPos + 1);
          configFileContent += String.Format("# define {0}" + custom + "_STR \"{1}\"\n", prefix, value);
          configFileContent += String.Format("# define {0}" + custom + "_WSTR L\"{1}\"\n", prefix, value);
        }
      }
      configFileContent += "#endif\n";

      string outFile = args[1];
      if (File.Exists(outFile))
      {
        string fileContent = File.ReadAllText(outFile);
        if (fileContent == configFileContent)
          return;
      }


      Directory.CreateDirectory(Path.GetDirectoryName(outFile));
      using (var file = new StreamWriter(outFile))
      {
        file.Write(configFileContent);
        file.Close();
      }
    }
  }
}
