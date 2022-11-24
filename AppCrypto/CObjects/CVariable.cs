using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using AppCrypto.IniFiles;
using StaticExtensions;

namespace AppCrypto.CObjects {  
  public class CVariables : CObject {
    public readonly string FileName;    
    public CVariables(string sFileName) {
      FileName = sFileName;      
    }
    public virtual void SetVarValue(string VarName, string VarValue) {
      IniFile f = IniFile.FromFile(FileName);
      f["Variables"][VarName] = VarValue.AsBase64Encoded();
      f.Save(FileName);
      base[VarName] = VarValue;
    }
    public virtual string GetVarValue(string VarName) {
      string result = "";
      if (base.Contains(VarName)) {
        result = base[VarName].AsString();
      } else {
        IniFile f = IniFile.FromFile(FileName);
        var theItem = f["Variables"][VarName];
        if (theItem != null) {
          result = theItem.AsBase64Decoded();
          base[VarName] = result;
        }
      }
      return result;
    }
    public void RemoveVar(string VarName) {
      IniFile f = IniFile.FromFile(FileName);
      f["Variables"].DeleteKey(VarName);
      f.Save(FileName);
      if (base.Contains(VarName)) {
        base.Remove(VarName);
      }
    }

    public ReadOnlyCollection<string> GetVarNames() {
      IniFile f = IniFile.FromFile(FileName);
      return f["Variables"].GetKeys();
    }
    public new string this[string VarName] { 
      get { return GetVarValue(VarName); } 
      set { SetVarValue(VarName, value); } 
    }
  }

}
