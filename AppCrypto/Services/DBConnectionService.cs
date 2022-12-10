using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using StaticExtensions;
using AppCrypto.CObjects;
using AppCrypto.Models;

namespace AppCrypto.Services {
  public class DBConnectionService { 
    private readonly string sDefaultName = "ConnectGroupAlpha";
    public string FileName = "";
    public string sProvider = "System.Data.SqlClient";
    public CSecureVar ivFile;
    private readonly AppKey _appKey;
    public DBConnectionService(AppKey aAppKey, string? aFileName ) {
      _appKey = aAppKey;
      if ( aFileName != null ) { 
        sDefaultName = aFileName;
      }
      if ( ConfigurationManager.ConnectionStrings == null) { 
         throw new Exception("Project requires App.config file with ConnectionString node.");
      }
      FieldInfo? CfgMgrReadOnlyField = typeof(ConfigurationElementCollection).GetField("_readOnly", BindingFlags.Instance | BindingFlags.NonPublic);
      if (CfgMgrReadOnlyField is not null) {
        CfgMgrReadOnlyField.SetValue(ConfigurationManager.ConnectionStrings, false);
      }           
      FileName = DllExt.MMCommonsFolder() + "\\" + sDefaultName + ".con2";
      if (!Directory.Exists(DllExt.MMCommonsFolder() + "\\")) {
        Directory.CreateDirectory(DllExt.MMCommonsFolder() + "\\");
      }
      ivFile = new CSecureVar(_appKey, FileName);
      Load();
    }    
    public void Load() {
      ConfigurationManager.ConnectionStrings.Clear();
      foreach (string connectionName in ivFile.GetVarNames()) { 
        string sConConnection = ivFile[connectionName];
        Add(connectionName, sConConnection, sProvider);
      }      
    }
    public static void Add(string sConName, string sConStr, string sConPro) {
      ConnectionStringSettings cs = new(sConName, sConStr, sConPro);
      ConfigurationManager.ConnectionStrings.Add(cs);
    }
    public void Write() {
      foreach (ConnectionStringSettings sx in ConfigurationManager.ConnectionStrings) {        
        ivFile[sx.Name] = sx.ConnectionString;      
      }
    }

    public IReadOnlyCollection<string> GetConnectionNames() { 
      return ivFile.GetVarNames();
    }

    public int GetActiveConnectionCounts() { 
      return ConfigurationManager.ConnectionStrings.Count;
    }

    public int GetConnectionCountOnFile() { 
      return GetConnectionNames().Count;
    }

    public DbConnectionInfo? GetConnectionInfo(string connectionName) { 
      ConnectionStringSettings? cx = GetConnectionStringSetting(connectionName);
      return cx is null ? null : new DbConnectionInfo(connectionName, cx.ConnectionString);
    }
       
    public void AddUpdate(string sConName, DbConnectionInfo info) {
      ConnectionStringSettings? cx = GetConnectionStringSetting(sConName);      
      if (cx == null) {
        Add(sConName, info.ConnectionString, sProvider);
      } else {
        Int32 iIndex = ConfigurationManager.ConnectionStrings.IndexOf(cx);
        ConfigurationManager.ConnectionStrings[iIndex].Name = info.ConnectionName;
        ConfigurationManager.ConnectionStrings[iIndex].ConnectionString = info.ConnectionString;
      } 
      Write();
    }

    public void RemoveConnection(string connectionName) { 
      ConfigurationManager.ConnectionStrings.Remove(connectionName);
      ivFile.RemoveVar(connectionName);      
    }
    
    public static ConnectionStringSettings? GetConnectionStringSetting(string sConName) {
      ConnectionStringSettings? cs = null;
      foreach (ConnectionStringSettings sx in ConfigurationManager.ConnectionStrings) {
        if (sx.Name == sConName) {
          cs = sx;
          break;
        }
      }
      return cs;
    }

  }

}
