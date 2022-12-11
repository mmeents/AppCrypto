
using System;
using NUnit.Framework;
using AppCrypto.IniFiles;
using AppCrypto.Models;
using AppCrypto.Services;
using AppCrypto.CObjects;
using StaticExtensions;
using System.IO;

namespace TestAppCrypto {


  public class TestJson {
    [Test] 
    public void UnitTestJson() {  
      string filename = DllExt.MMCommonsFolder() + "\\TestTable1.Ini";

      if (!File.Exists(filename)) {
        CFileTable testTbl = new(filename);
        testTbl.Columns.Add(new CFileTableColumn(testTbl.Columns, "ColA", "ColA", CColType.ctColKey));
        testTbl.Columns.Add(new CFileTableColumn(testTbl.Columns, "ColB", "ColB", CColType.ctString));
        testTbl.Columns.Add(new CFileTableColumn(testTbl.Columns, "ColC", "ColC", CColType.ctInt32));
        for (int i = 0; i < 100; i++) {
          CFileTableRow r = testTbl.Rows.Add(new CFileTableRow(testTbl.Rows));
          r["ColA"].Value = $"Val{i}A";
          r["ColB"].Value = $"Val{i}B";
          r["ColC"].Value = $"{i}";
        }
        testTbl.Save();
      }

      if (File.Exists(filename)) { 
        CFileTable testTbl = new(filename);
        var ordered = testTbl.Rows.OrderByIntType("ColC", false);
        if (ordered is not null) { 
          foreach(int r in ordered) { 
            CFileTableRow tr = testTbl.Rows[r];
            Console.WriteLine($" Key:{tr.Key} ColA:{tr["ColA"].Value} ColB:{tr["ColB"].Value} ColC:{tr["ColC"].Value} ");          
          }
        }
        
        int x = testTbl.Rows.GetSingleKeyWhere("ColA", "Val69A");
        Console.WriteLine($"Test Lookup ColA for Val69A print ColC:{testTbl.Rows[x]["ColC"].Value};");

        if (ordered is not null) { 
          testTbl.Rows.DeleteWhere("ColA", "Val");          
        }
      }


      if (File.Exists(filename)) { 
        CFileTable testEmpty = new(filename);
        Console.WriteLine($"After delete load coung {testEmpty.Rows.Count} ");
        File.Delete(filename);        
      }
    }


  }
}
