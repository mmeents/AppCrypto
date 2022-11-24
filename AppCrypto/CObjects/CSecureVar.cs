using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCrypto.Models;
using AppCrypto.IniFiles;

namespace AppCrypto.CObjects {
  public class CSecureVar : CVariables {       
    private readonly AppKey _appKey;    
    public CSecureVar(AppKey aAppKey, string aFileName) : base(aFileName){
      _appKey = aAppKey;      
      if (File.Exists(aFileName)) {
        this.LoadValues();
      }
    }
    public override void SetVarValue(string VarName, string VarValue) {
      string CipherValue = _appKey.AsCipherText(VarValue);
      base.SetVarValue(VarName, CipherValue);      
    }
    public override string GetVarValue(string VarName) {      
      string cipherResult = base.GetVarValue(VarName); 
      string result = _appKey.AsDecipheredText(cipherResult);
      return result;
    }
    public void LoadValues() {
      foreach (string varName in this.GetVarNames()) {
        _ = this[varName];
      }
    }

  }

}
