
using System;
using NUnit.Framework;
using AppCrypto.IniFiles;
using StaticExtensions;
using System.IO;

namespace TestAppCrypto {
  public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Test1() {
      Assert.Pass();
    }
    [Test]
    public void TestIniFiles1() {
      string fileFolder = $"{DllExt.MMCommonsFolder()}";
      string filePath = $"{fileFolder}\\TestIni.Ini";
      if (!Directory.Exists(fileFolder + "\\")) {
        Directory.CreateDirectory(fileFolder + "\\");
      }
      Console.WriteLine( $"Filepath: {filePath}");

      IniFile iniFile = IniFile.FromFile(filePath);
      iniFile["TestSectionA"]["KeyA"] = "TestStringA";
      iniFile["TestSectionA"]["KeyB"] = "TestStringB";
      iniFile["TestSectionA"]["KeyC"] = "TestStringC";
      iniFile["TestSectionA"]["KeyD"] = "TestStringD";
      iniFile["TestSectionA"]["KeyE"] = "TestStringE";
      iniFile["TestSectionA"]["KeyF"] = "TestStringF";
      iniFile.Save(filePath);


      IniFile iniFile2 = IniFile.FromFile(filePath);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyA"]);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyB"]);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyC"]);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyD"]);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyE"]);
      Console.WriteLine(iniFile2["TestSectionA"]["KeyF"]);

      iniFile2["TestSectionA"].DeleteKey("KeyA");
      iniFile2["TestSectionA"].DeleteKey("KeyB");
      iniFile2["TestSectionA"].DeleteKey("KeyC");
      iniFile2["TestSectionA"].DeleteKey("KeyD");
      iniFile2["TestSectionA"].DeleteKey("KeyE");
      iniFile2["TestSectionA"].DeleteKey("KeyF");
      iniFile2.Save(filePath);

      IniFile iniFile3 = IniFile.FromFile(filePath);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyA"]);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyB"]);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyC"]);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyD"]);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyE"]);
      Console.WriteLine(iniFile3["TestSectionA"]["KeyF"]);

      Assert.Pass();
    }
    [Test]
    public void TestIniFiles2() {
      string fileFolder = $"{DllExt.MMCommonsFolder()}";
      string filePath = $"{fileFolder}\\TestIni.Ini";
      if (!Directory.Exists(fileFolder + "\\")) {
        Directory.CreateDirectory(fileFolder + "\\");
      }
      Assert.Pass();
    }
  }
}