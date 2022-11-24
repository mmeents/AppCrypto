using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using StaticExtensions;

namespace AppCrypto.Models {
  public class AppKey {
    private readonly string _a;
    private readonly string _b;
    public AppKey(string sPassword) {
      PasswordDeriveBytes aPDB = new(sPassword, null);
      _a = aPDB.GetBytes(32).AsHexStr();
      _b = aPDB.GetBytes(16).AsHexStr();
    }    
    public string AsCipherText(string aText) {
      string sResult = "";
      try {
        using Aes aes = Aes.Create();
        aes.Key = _a.AsByteArray();
        aes.IV = _b.AsByteArray();        
        MemoryStream ms = new();
        CryptoStream encStream = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        StreamWriter sw = new(encStream);
        sw.WriteLine(aText.AsBase64Encoded());
        sw.Close();
        encStream.Close();
        byte[] buffer = ms.ToArray();
        ms.Close();
        sResult = buffer.AsHexStr();
      } catch {
        throw;
      }
      return sResult;
    }
    public string AsDecipheredText(string aCipherText) {
      string val = "";
      try {
        using Aes aes = Aes.Create();
        aes.Key = _a.AsByteArray();
        aes.IV = _b.AsByteArray();       
        MemoryStream ms = new(aCipherText.AsByteArray());
        CryptoStream encStream = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        StreamReader sr = new(encStream);
        val = sr.ReadToEnd();
        val = val.AsBase64Decoded();
        sr.Close();
        encStream.Close();
        ms.Close();
      } catch {
        throw;
      }
      return val;
    }
  }
}
