
using System;
using NUnit.Framework;
using AppCrypto.IniFiles;
using AppCrypto.Models;
using AppCrypto.Services;
using StaticExtensions;
using System.IO;

namespace TestAppCrypto {
  public class TestIniFile {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestIniFiles() {
      string fileFolder = $"{DllExt.MMCommonsFolder()}";
      string filePath = $"{fileFolder}\\TestIni.Ini";
      if (!Directory.Exists(fileFolder + "\\")) {
        Directory.CreateDirectory(fileFolder + "\\");
      }
      Console.WriteLine( $"Filepath: {filePath}");

      IniFile iniFile = IniFile.FromFile(filePath);
      iniFile["TestSectionA"]["KeyA"] = "TestStringA";
      iniFile["TestSectionA"]["KeyB"] = "TestStringB";
      iniFile["TestSectionB"]["KeyC"] = "TestStringC";
      iniFile["TestSectionB"]["KeyD"] = "TestStringD";
      iniFile["TestSectionC"]["KeyE"] = "TestStringE";
      iniFile["TestSectionC"]["KeyF"] = "TestStringF";
      iniFile.Save(filePath);


      IniFile iniFile2 = IniFile.FromFile(filePath);

      foreach(string name in iniFile2.GetSectionNames()) {
        foreach(string key in iniFile2[name].GetKeys()) {
          Console.WriteLine("Sec: "+ name+" key: "+ key+" value: "+ iniFile2[name][key]);
        }        
      }

      Console.WriteLine("SecA:" + iniFile2["TestSectionA"]["KeyA"]);
      Console.WriteLine("SecA:" + iniFile2["TestSectionA"]["KeyB"]);
      Console.WriteLine("SecB:" + iniFile2["TestSectionB"]["KeyC"]);
      Console.WriteLine("SecB:" + iniFile2["TestSectionB"]["KeyD"]);
      Console.WriteLine("SecC:" + iniFile2["TestSectionC"]["KeyE"]);
      Console.WriteLine("SecC:" + iniFile2["TestSectionC"]["KeyF"]);

      

      iniFile2["TestSectionA"].DeleteKey("KeyA");
      iniFile2["TestSectionA"].DeleteKey("KeyB");
      iniFile2["TestSectionB"].DeleteKey("KeyC");
      iniFile2["TestSectionB"].DeleteKey("KeyD");
      iniFile2["TestSectionC"].DeleteKey("KeyE");
      iniFile2["TestSectionC"].DeleteKey("KeyF");
      iniFile2.Save(filePath);

      IniFile iniFile3 = IniFile.FromFile(filePath);
      Console.WriteLine("a "+ iniFile3["TestSectionA"]["KeyA"]);
      Console.WriteLine("a " + iniFile3["TestSectionA"]["KeyB"]);
      Console.WriteLine("a " + iniFile3["TestSectionB"]["KeyC"]);
      Console.WriteLine("a " + iniFile3["TestSectionB"]["KeyD"]);
      Console.WriteLine("a " + iniFile3["TestSectionC"]["KeyE"]);
      Console.WriteLine("a " + iniFile3["TestSectionC"]["KeyF"]);

    }
    [Test]
    public void TestFileVar() {
      string fileFolder = $"{DllExt.MMCommonsFolder()}";
      string filePath = $"{fileFolder}\\TestFileVar.Ini";
      if (!Directory.Exists(fileFolder + "\\")) {
        Directory.CreateDirectory(fileFolder + "\\");
      }
      Console.WriteLine($"FilePath {filePath}{Environment.NewLine}FileFolder: {fileFolder}");

      FileVar aFV = new(filePath);
      aFV["Location"] = filePath;
      aFV["Path"] = fileFolder;

      FileVar bFV = new(filePath);
      Console.WriteLine("From B Location:"+ bFV["Location"]);
      Console.WriteLine("From B Path:"+bFV["Path"]);

      FileVar cFV = new(filePath);
      var varNames = cFV.GetVarNames();
      foreach(var varName in varNames) {
        Console.WriteLine("From C "+ varName+": "+cFV[varName]);
        cFV.RemoveVar(varName);
      }
      
    }

    [Test]
    public void TestDBConnectionService() { 

      var TestKey = new AppKey("mConMgrBaseAlpha");

      DBConnectionService TestService = new DBConnectionService(TestKey, "TestSettings");
      DbConnectionInfo ConnInfo = TestService.GetConnectionInfo("TS");
      if (ConnInfo == null) { 
        DbConnectionInfo dbInfo = new DbConnectionInfo("TS", "data source=DESKTOP-DELICI0;initial catalog=ARC01;user id=website;password=qQ#Qw5S!25byAuY3;Connection Timeout=15");
        TestService.AddUpdate("TS", dbInfo);
      }
      ConnInfo = TestService.GetConnectionInfo("TS");

      Console.WriteLine(ConnInfo?.ConnectionName??" name was null");
    }
  }
}