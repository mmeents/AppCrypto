using System;
using System.Data;
using System.Linq;
using System.IO;
using StaticExtensions;
using AppCrypto.IniFiles;

namespace AppCrypto.CObjects {

  #region CFileTable Enums

  public enum FTState { fsInactive = 0, fsBrowse = 1, fsEdit = 2, fsInsert = 3, fsSetKey = 4 }
  public enum CColType { ctColKey = 0, ctString = 1, ctInt32 = 2, ctDateTime = 3 }

  #endregion

  #region CFileTableColumns

  public class CFileTableColumn {
    public CFileTableColumns? Owner { get ; set ; }
    public short Key { get; set; }
    public string Caption { get; set; }
    public string Name { get; set;}
    public CColType ColType { get; set;}
    public CFileTableColumn(
      CFileTableColumns aOwner, 
      string aCaption, 
      string aName, 
      CColType aType
    ) {
      Owner = aOwner; 
      Caption = aCaption; 
      Name = aName; 
      ColType = aType;
    }    
    public string ToBase64Encoded() { 
      return $"{Key} {Caption.AsBase64Encoded()} {Name.AsBase64Encoded()} {ColType.AsInt()}".AsBase64Encoded();
    }
  }

  public class CFileTableColumns : CCache16 {
    public new CFileTableColumn? this[short aKey] {
      get { 
        return (CFileTableColumn?)(Contains(aKey) ? base[aKey] : null); }
      set { if (value is not null) base[aKey] = value; }
    }
    public CFileTableColumn Add(CFileTableColumn column) {
      column.Owner = this;
      Nonce--;
      base[Nonce] = column;
      column.Key = Nonce;      
      return (CFileTableColumn)column;
    }
    public CFileTableColumn? First() {
      CFileTableColumn? c = null;
      if (Keys.Count > 0) {
        short k = base.Keys.OrderBy(x => x).First();
        c = (CFileTableColumn?)this[k];
      }
      return c;
    }
  }

  #endregion

  #region CFileTableRows
  // Cell level Value
  public class CFileTableField {
    public CFileTableRow? Owner { get; set; }
    public CFileTableColumn? Column { get; set; }
    public string Value { get; set; }
    public CFileTableField(CFileTableRow aRow, CFileTableColumn aCol, string aValue) : base() {
      Owner = aRow;
      Column = aCol;
      Value = aValue;
    }    
  }

  // Row is a list of Fields governed via columns.
  public class CFileTableRow : CObject {
    public CFileTableRows Owner;
    public CFileTableColumns Columns { get; set; }
    public int Key;
    public CFileTableRow(CFileTableRows aOwner, string? encodedRow = null) : base() {
      Owner = aOwner ?? throw new ArgumentNullException(nameof(aOwner));
      Columns = Owner.Columns ?? throw new ArgumentNullException(nameof(aOwner)); ;      
      IOrderedEnumerable<short> ColKeysList = Columns.Keys.OrderBy(x => x);
      string decodedRows = "";
      if (encodedRow is not null) { 
        decodedRows = encodedRow.AsBase64Decoded();
      }
      if (decodedRows == ":null") { 
        decodedRows = "";
      }
      if (ColKeysList is not null) {        
        int parsePos = 0;
        foreach (short key in ColKeysList) {
          var encodedValue =  decodedRows.ParseString(" ", parsePos);           
          var sVal = encodedValue.AsBase64Decoded();
          if (sVal == ":null") { sVal = "";}
          CFileTableColumn? aCol = Owner.Columns[key];
          if (aCol is not null) {
            _ = Add(new CFileTableField(this, aCol, sVal));
          }
          parsePos++;
        }
      }
    }
    public CFileTableField? Add(CFileTableField field) {
      CFileTableColumn? col = field.Column;
      if (col is not null) {
        string? fieldName = col.Name;
        if (fieldName is not null) {
          base[fieldName] = field;
          return (CFileTableField)field;
        }
      } 
      return null;
    }
    public new CFileTableField this[string FieldName] {
      get {
        if (Contains(FieldName)) { 
          var aField = base[FieldName];
          if (aField is not null) {
            return (CFileTableField)aField;
          }
        }  // null or bust hack.
        throw new InvalidOperationException($"Invalid column:{FieldName}");
      }
      set { base[FieldName] = value; }
    }
    public string ToBase64Encoded() {
      string sOut = "";
      if (Owner is not null) { 
        IOrderedEnumerable<short>? ColKeysList = Columns.Keys.OrderBy(x => x);
        if (ColKeysList is not null) {
          string ColumnValues = "";
          foreach (short key in ColKeysList) {
            CFileTableColumn? aCol = Owner.Columns[key];
            if (aCol is not null) {
              string columnName = aCol.Name;
              CFileTableField? field = this[columnName];
              string columnValue = "";
              if (field is not null) {  
                columnValue = field.Value.AsBase64Encoded();                
              } else { 
                columnValue = ":null".AsBase64Encoded();
              }
              ColumnValues += $" {columnValue}";
            }
          }
          sOut = ColumnValues.AsBase64Encoded();
        }
      }
      return sOut;
    }
  }

  // Cache of Row objects is the Rows, add references to lets not forget who where doing this for...
  public class CFileTableRows : CCache32 {
    public CFileTable Owner;
    public CFileTableColumns Columns;
    public new CFileTableRow this[int aRK] {
      get { 
        if (Contains(aRK)) { 
          return (CFileTableRow)base[aRK];
        }
        throw new InvalidOperationException($"row not found row:{aRK}");
      }
      set { if (value is null) { base.Remove(aRK); } else { base[aRK] = value;} }
    }
    public CFileTableRows(CFileTable aOwner, CFileTableColumns columns) : base() {
      Owner = aOwner;
      Columns = columns;
    }
    public CFileTableRow Add(CFileTableRow row) {
      row.Owner = this;
      Nonce--;
      base[Nonce] = row;
      row.Key = Nonce;
      return (CFileTableRow)row;
    }
    public CFileTableRow Load(IniFile af, int iRowKey) {
      string rowKey = "R"+ iRowKey.AsString();
      string? encodedRow = af["Rows"][rowKey];
      CFileTableRow r = Add(new CFileTableRow(this, encodedRow));      
      return r;
    }
    public void Save(IniFile af, Int32 iRowKey) {
      string rowKey = "R"+iRowKey.AsString();
      CFileTableRow? row = this[iRowKey];
      if (row != null) {
        af["Rows"][rowKey] = row.ToBase64Encoded();
      } 
    }

    public int GetSingleKeyWhere(string ColumnName, string CompareTo) {       
      if (ColumnName != null) { 
        IEnumerable<int> w = Keys.Where(x => CompareTo == (((CFileTableRow)base[x])[ColumnName].Value));
        if (w.Count() > 0) { 
          return w.First();
        }
      }
      throw new Exception($"{ColumnName} failed to match on {CompareTo}");
    }
    public IOrderedEnumerable<int> OrderByStringType(string ColumnName, bool OrderAsc = true) {
      IOrderedEnumerable<int> orderedRows;
      if (OrderAsc) {
        orderedRows = Keys.OrderBy(x => ((CFileTableRow)base[x])[ColumnName].Value);
      } else {
        orderedRows = Keys.OrderByDescending(x => ((CFileTableRow)base[x])[ColumnName].Value);
      }        
      return orderedRows;
    }
    public IOrderedEnumerable<int> OrderByIntType(string ColumnName, bool OrderAsc = true) {
      IOrderedEnumerable<int> orderedRows;
      if (OrderAsc) {
        orderedRows = Keys.OrderBy(x => ((CFileTableRow)base[x])[ColumnName].Value.AsInt());
      } else {
        orderedRows = Keys.OrderByDescending(x => ((CFileTableRow)base[x])[ColumnName].Value.AsInt());
      }      
      return orderedRows;
    }

    public void DeleteWhere(string ColumnName, string strContains) {
      if (ColumnName != null) {
        IEnumerable<int> w = Keys.Where(x => (((CFileTableRow)base[x])[ColumnName].Value).Contains(strContains) );
        if (w.Count() > 0) {
          IniFile f = IniFile.FromFile(Owner.FileName); 
          foreach (int x in w) { 
            f["Rows"].DeleteKey("R"+x.AsInt());
            base.Remove(x);
          }
          f.Save(Owner.FileName);
        }
      }      
    }

  }

  #endregion

  #region CFileTable

  public class CFileTable {
    
    public string FileName;
    public CFileTableColumns Columns;
    public CFileTableRows Rows;

    public CFileTable(string sFileName) {      
      FileName = sFileName;
      Columns = new CFileTableColumns();
      Rows = new CFileTableRows(this, Columns);
      if (File.Exists(FileName)) {
        Load();
      }
    }

    public void Load() {

      IniFile f = IniFile.FromFile(FileName);
      Columns.Clear();
      var ColumnKeys = f["Columns"].GetKeys();
      if (ColumnKeys != null) { 
        foreach(var key in ColumnKeys) {          
          string? encodedCol = f["Columns"][key];
          if (encodedCol != null) {
            encodedCol = encodedCol.AsBase64Decoded();
            string caption = encodedCol.ParseString(" ", 1).AsBase64Decoded();
            string colName = encodedCol.ParseString(" ", 2).AsBase64Decoded();
            CColType aType = (CColType?)encodedCol.ParseString(" ", 3).AsInt()??CColType.ctString;
            Columns.Add(new  CFileTableColumn(Columns, caption, colName, aType));
          }          
        }

        Rows.Clear();
        var iniRowKeys = f["Rows"].GetKeys();
        foreach (var key in iniRowKeys) {
          if (int.TryParse(key.AsSpan(1), out int iVal)) {
            Rows.Load(f, iVal);            
          }
        }
      }     
    }

    public void Save() {

      IniFile f = IniFile.FromFile(FileName);

      // Column Index, list of keys by delimited by semicolon
      IOrderedEnumerable<short> k = Columns.Keys.OrderByDescending(x => x);      
      CFileTableColumn? c;
      foreach (short key in k) {
        c = (CFileTableColumn?)Columns[key];
        if (c is not null) {          
          string sI = "C"+c.Key.AsString();          
          f["Columns"][sI] = c.ToBase64Encoded();
        }
      }

      if (Rows.Count > 0) {
        IOrderedEnumerable<Int32> r = Rows.Keys.OrderByDescending(x => x);        
        foreach (Int32 RowKey in r) {          
          Rows.Save(f, RowKey);
        }
      }
      f.Save(FileName);
    }

    public void LoadRows() {
      IniFile f = IniFile.FromFile(FileName);
      var iniRowKeys = f["Rows"].GetKeys();
      foreach(var key in iniRowKeys) {                
        Rows.Add(new CFileTableRow(Rows, f["Rows"][key] ));                  
      }     
    }

    public void SaveRows() {
      if (Rows.Count > 0) {
        IniFile f = IniFile.FromFile(FileName);        
        IOrderedEnumerable<int> r = Rows.Keys.OrderBy(x => x);        
        foreach (int RowKey in r) {
          CFileTableRow? tr = Rows[RowKey];
          string RowKeyStr = "R" + RowKey.AsString();
          if (tr != null) {            
            f["Rows"][RowKeyStr] = tr.ToBase64Encoded();
          } else {
            f["Rows"][RowKeyStr] = ":null".AsBase64Encoded();
          }          
        }        
        f.Save(FileName);
      }
    }


  }




  #endregion


}