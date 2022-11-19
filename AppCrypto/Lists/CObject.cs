using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticExtensions;

namespace AppCrypto.Lists {
  public class CInt32 : ConcurrentDictionary<Int32, object> {
    public CInt32() : base() { }
    public Boolean Contains(Int32 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public new object this[Int32 aKey] { get { try { return base[aKey]; } catch { return null; } } set { base[aKey] = value; } }
    public void Remove(Int32 aKey) {
      if (Contains(aKey)) {
        base.TryRemove(aKey, out object outcast);
      }
    }
  }

  #region class CObject 
  /// <summary> ConcurrentDictionary with String as key is CObject</summary>
  /// <remarks> C is for Concurrent. Most common type is string lookup version so lets call them Objects. </remarks>
  public class CObject : ConcurrentDictionary<string, object> {
    public CObject() : base() { }
    public Boolean Contains(String aKey) { try { return (base[aKey] is Object); } catch { return false; } }
    public new object this[string aKey] { get { return (Contains(aKey) ? base[aKey] : null); } set { base[aKey] = value; } }
    public void Remove(string aKey) {
      if (Contains(aKey)) {
        object outcast;
        base.TryRemove(aKey, out outcast);
      }
    }
  }
  #endregion 

  /// <summary> ConcurrentDictionary with Decimal as key ordered is a Cbook. </summary>
  public class CBook : ConcurrentDictionary<decimal, object> {
    public CBook() : base() { }
    public Boolean Contains(decimal aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public new object this[decimal aKey] { get { return (Contains(aKey) ? base[aKey] : null); } set { base[aKey] = value; } }
    public void Remove(decimal aKey) { if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); } }
    public decimal ElementKeyAt(Int32 iIndex) {
      IEnumerable<decimal> lQS = base.Keys.OrderByDescending(x => (x));
      return lQS.ElementAt(iIndex);
    }
  }

  #region public class CCache variants 
  /// <summary>ConcurrentDictionary with Short Integer key ordered from Max to Min, add smallest, pop largest last is a CCache16</summary>  
  public class CCache16 : ConcurrentDictionary<Int16, object> {
    public Int16 Nonce = Int16.MaxValue;
    public Int16 Height => Convert.ToInt16(Int16.MaxValue.AsInt() - this.Nonce.AsInt());
    public Boolean Contains(Int16 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public CCache16() : base() { }
    public object Add(object aObj) { Nonce--; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).Last(), out aR); }
      return aR;
    }
    public void Remove(Int16 aKey) { if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); } }
  }

  /// <summary>ConcurrentDictionary with Int32 key ordered from Max to Min, add smallest, pop largest last is a CCache32 </summary>  
  public class CCache32 : ConcurrentDictionary<Int32, object> {
    public Int32 Nonce = Int32.MaxValue;
    public Int32 Height => Int32.MaxValue - Nonce;
    public Boolean Contains(Int32 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public CCache32() : base() { }
    public object Add(object aObj) { Nonce--; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).Last(), out aR); }
      return aR;
    }
    public void Remove(Int32 aKey) { if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); } }
  }

  /// <summary>ConcurrentDictionary with Int36 key ordered from Max to Min, add smallest, pop largest last is a CCache64</summary>  
  public class CCache64 : ConcurrentDictionary<Int64, object> {
    public Int64 Nonce = Int64.MaxValue;
    public Int64 Height { get { return Int64.MaxValue - Nonce; } }
    public Boolean Contains(Int64 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public CCache64() : base() { }
    public object Add(object aObj) { Nonce--; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).Last(), out aR); }
      return aR;
    }
    public void Remove(Int64 aKey) { if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); } }
  }

  #endregion

  #region public class CQueue variants 

  /// <summary>ConcurrentDictionary with Int16 key ordered from Min to Max, adds largest, pops smallest is a CQueue16 lifetime total not to exceed 32768 + 32767 = 65535 items. </summary>    
  public class CQueue16 : ConcurrentDictionary<Int16, object> {
    public Int16 Nonce = Int16.MinValue;
    public CQueue16() : base() { }
    public Boolean Contains(Int16 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public object Add(object aObj) { Nonce++; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).First(), out aR); }
      return aR;
    }
    public void Remove(Int16 aKey) { if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); } }
  }

  /// <summary>ConcurrentDictionary with Int32 key ordered from Min to Max, adds largest, pops smallest is a CQueue32 lifetime total not to exceed 4,294,967,295 items.</summary>  
  public class CQueue32 : ConcurrentDictionary<Int64, object> {
    public Int32 Nonce = Int32.MinValue;
    public CQueue32() : base() { }
    public Boolean Contains(Int32 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public object Add(object aObj) { Nonce++; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).First(), out aR); }
      return aR;
    }
    public void Remove(Int32 aKey) {
      if (Contains(aKey)) { base.TryRemove(aKey, out object outcast); }
    }
  }

  /// <summary>ConcurrentDictionary with Int64 key ordered from Min to Max, adds largest, pops smallest is a CQueue64 lifetime total not to exceed 18,446,744,073,709,551,616 items</summary>  
  public class CQueue64 : ConcurrentDictionary<Int64, object> {
    public Int64 Nonce = Int64.MinValue;
    public CQueue64() : base() { }
    public Boolean Contains(Int64 aKey) {
      try { return (base[aKey] is Object); } catch { return false; }
    }
    public object Add(object aObj) { Nonce++; base[Nonce] = aObj; return aObj; }
    public object Pop() {
      Object aR = null;
      if (Keys.Count > 0) { base.TryRemove(base.Keys.OrderBy(x => x).First(), out aR); }
      return aR;
    }
    public void Remove(Int64 aKey) {
      if (Contains(aKey)) {
        base.TryRemove(aKey, out object outcast);
      }
    }
  }

  #endregion
}
