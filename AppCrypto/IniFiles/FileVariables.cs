using System;
using System.Collections.ObjectModel;
using StaticExtensions;
using AppCrypto.Lists;

namespace AppCrypto.IniFiles {
  public class FileVar {
    private readonly string FileName;
    private readonly CObject cache;
    public FileVar(string sFileName) {
      FileName = sFileName;
      cache = new CObject();
    }
    private void SetVarValue(string VarName, string VarValue) {
      try {
        IniFile f = IniFile.FromFile(FileName);
        f["Variables"][VarName] = VarValue.AsBase64Encoded();
        f.Save(FileName);
        cache[VarName] = VarValue;
      } catch {
        throw;
      }
    }
    private string GetVarValue(string VarName) {
      string result = "";
      try {
        if (cache.Contains(VarName)) {
          result = cache[VarName].AsString();
        } else {
          IniFile f = IniFile.FromFile(FileName);
          result = f["Variables"][VarName].AsBase64Decoded();
          cache[VarName] = result;
        }
      } catch { }
      return result;
    }
    public void RemoveVar(string VarName) {
      IniFile f = IniFile.FromFile(FileName);
      f["Variables"].DeleteKey(VarName);
      f.Save(FileName);
      if (cache.Contains(VarName)) {
        cache.Remove(VarName);
      }
    }

    public ReadOnlyCollection<string> GetVarNames() {
      IniFile f = IniFile.FromFile(FileName);
      return f["Variables"].GetKeys();
    }
    public string this[string VarName] { get { return GetVarValue(VarName); } set { SetVarValue(VarName, value); } }
  }
}
