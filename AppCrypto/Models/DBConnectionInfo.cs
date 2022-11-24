using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticExtensions;

namespace AppCrypto.Models {

	[Serializable]
	public class DbConnectionInfo {
		private string? _connectionName = null;
		private string _userName = String.Empty;
		private string _password = String.Empty;
		private string _serverName = String.Empty;
		private string _initialCatalog = String.Empty;
		private bool _useIntegratedSecurity = false;
		public DbConnectionInfo() {}
		public DbConnectionInfo(string connectionName, string connectionString) {
      if (connectionString == null) 
				throw new ArgumentNullException(nameof(connectionString));			
			_connectionName = connectionName ?? throw new ArgumentNullException(nameof(connectionName));
			SetConnectionString(connectionString);			
		}
		public string ConnectionName {
			get { return _connectionName ?? ""; }
			set {
        _connectionName = value ?? throw new ArgumentNullException("ConnectionName");
			}
		}

		public string ConnectionString {
			get { return GetConnectionString(); }
			set {
				if (value == null) { throw new ArgumentNullException("ConnectionString"); }
				SetConnectionString(value);
			}
		}

		public string UserName { get { return _userName; } set { _userName = value; } }

		public string Password { get { return _password; } set { _password = value; } }

		public string ServerName { get { return _serverName; } set { _serverName = value; } }

		public string InitialCatalog { get { return _initialCatalog; } set { _initialCatalog = value; } }

		public bool UseIntegratedSecurity { get { return _useIntegratedSecurity; } set { _useIntegratedSecurity = value; } }

		private string GetConnectionString() {   
			StringBuilder sb = new();
			sb.Append("Data Source=");
			sb.Append(_serverName);
			sb.Append(";Initial Catalog=");
			sb.Append(_initialCatalog);
			sb.Append(';');
			if (_useIntegratedSecurity == false) {
				sb.Append("User ID=");
				sb.Append(_userName);
				sb.Append(";Password=");
				sb.Append(_password);
				sb.Append(';');
			} else {
				sb.Append("Integrated Security=SSPI;");
			}
			sb.Append("TransparentNetworkIPResolution=False;");
			return sb.ToString();
		}

		private void SetConnectionString(string connstr) {
			Hashtable connStringKeys = new();
			string[] keysBySemicolon = connstr.Split(';');
			string[] keysByEquals;
			foreach (string keySemicolon in keysBySemicolon) {
				keysByEquals = keySemicolon.Split('=');

				if (keysByEquals.Length == 0) {
					// do nothing
				} else if (keysByEquals.Length == 1) {
					// assume key name but no value
					connStringKeys.Add(keysByEquals[0].ToUpper(), "");
				} else {
					connStringKeys.Add(keysByEquals[0].ToUpper(), keysByEquals[1]);
				}
			}

			if (connStringKeys.ContainsKey("SERVER") == true) {
				_serverName = connStringKeys["SERVER"].AsString();
			} else {
				if (connStringKeys.ContainsKey("DATA SOURCE") == true) {
					_serverName = connStringKeys["DATA SOURCE"].AsString();
				} else {
					_serverName = "";
				}
			}

			if (connStringKeys.ContainsKey("DATABASE") == true) {
				_initialCatalog = connStringKeys["DATABASE"].AsString();
			} else {
				if (connStringKeys.ContainsKey("INITIAL CATALOG") == true) {
					_initialCatalog = connStringKeys["INITIAL CATALOG"].AsString();
				} else {
					_initialCatalog = "";
				}
			}

			if (connStringKeys.ContainsKey("USER") == true) {
				_userName = connStringKeys["USER"].AsString();
			} else {
				if (connStringKeys.ContainsKey("USER ID") == true) {
					_userName = connStringKeys["USER ID"].AsString();
				} else {
					_userName = "";
				}
			}

			if (connStringKeys.ContainsKey("PASSWORD") == true) {
				_password = connStringKeys["PASSWORD"].AsString();
			} else {
				_password = "";
			}

			if (connStringKeys.ContainsKey("INTEGRATED SECURITY") == true) {
				_useIntegratedSecurity = true;
			} else {
				_useIntegratedSecurity = false;
			}
		}

		public override string ToString() {
			return $"{_connectionName}~{GetConnectionString()}";
		}

	} // end of class
}
