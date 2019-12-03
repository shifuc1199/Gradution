using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Ferr {
	public enum DataStringType {
		Ordered,
		Named
	}
	
	public class DataStringWriterUtil {
		DataStringType  _type;
		StringBuilder   _builder;
		HashSet<string> _names = new HashSet<string>();

		public DataStringWriterUtil(DataStringType aType) {
			_type      = aType;
			_builder   = new StringBuilder();
			_builder.Append('{');
		}

		public void Int(int aData) {
			Entry(aData.ToString());
		}
		public void Int(string aName, int aData) {
			Entry(aName, aData.ToString());
		}
		public void Long(long aData) {
			Entry(aData.ToString());
		}
		public void Long(string aName, long aData) {
			Entry(aName, aData.ToString());
		}
		public void Bool(bool aData) {
			Entry(aData.ToString());
		}
		public void Bool(string aName, bool aData) {
			Entry(aName, aData.ToString());
		}
		public void Float(float aData) {
			Entry(aData.ToString());
		}
		public void Float(string aName, float aData) {
			Entry(aName, aData.ToString());
		}
		public void Data(IToFromDataString aData) {
			if (aData == null)
				Entry("null");
			else
				Entry(aData.GetType().Name+"="+aData.ToDataString());
		}
		public void Data(string aName, IToFromDataString aData) {
			if (aData == null)
				Entry(aName, "null");
			else
				Entry(aName, aData.GetType().Name+"="+aData.ToDataString());
		}
		public void String(string aData) {
			char quotes = GetQuoteType(aData);
			if (quotes != ' ') {
				Entry(quotes+aData+quotes);
			} else {
				Entry(aData);
			}
		}
		public void String(string aName, string aData) {
			char quotes = GetQuoteType(aData);
			if (quotes != ' ') {
				Entry(aName, quotes+aData+quotes);
			} else {
				Entry(aName, aData);
			}
		}
		protected char GetQuoteType(string aData) {
			char result = ' ';
			if (!aData.StartsWith("{") && !aData.StartsWith("\"") && !aData.StartsWith("'")) {
				bool sq = aData.Contains("'");
				bool dq = aData.Contains("\"");
				if (sq && dq)
					throw new ArgumentException("String data contains -both- single and double quotes, what am I supposed to do with this?");

				result = sq ? '"' : '\'';
			}
			return result;
		}
		protected void Entry(string aData) {
			if (_type == DataStringType.Named)
				throw new Exception("Need a name for a named list!");

			if (_builder.Length > 1)
				_builder.Append(',');
			_builder.Append(aData);
		}
		protected void Entry(string aName, string aData) {
			if (_type == DataStringType.Ordered)
				throw new Exception("Name doesn't apply for ordered lists!");
			if (aName.Contains(":") || aName.Contains(","))
				throw new Exception("Name includes a reserved character! (: or ,) - " + aName);
			if (_names.Contains(aName))
				throw new Exception("Used the same name twice: " + aName);
			_names.Add(aName);

			if (_builder.Length > 1)
				_builder.Append(',');
			_builder.Append(aName);
			_builder.Append(":");
			_builder.Append(aData);
		}

		public override string ToString() {
			return _builder.ToString()+"}";
		}
	}

	public class DataStringReaderUtil {
		DataStringType _type;
		string[]       _words;
		string[]       _names;
		int            _curr = 0;

		public int  NameCount { get { return _names.Length; } }
		public bool HasNext   { get { return _curr < _words.Length; } }

		public DataStringReaderUtil(string aData, DataStringType aType) {
			List<string> words = DataStringUtil.SplitSmart(aData, ',');
			if (words == null)
				throw new ArgumentException("Poorly formed data string! Ensure sure quotes and brackets all match!");

			_type  = aType;
			_words = string.IsNullOrEmpty(aData) ? new string[] { } : words.ToArray();
			
			if (_type == DataStringType.Named) {
				_names = new string[_words.Length];

				for (int i = 0; i < _words.Length; i++) {
					int    sep  = _words[i].IndexOf(':');
					string name = _words[i].Substring(0, sep);
					string data = _words[i].Substring(sep+1);

					_words[i] = data;
					_names[i] = name;
				}
			}
		}

		public string GetName(int aIndex) {
			return _names[aIndex];
		}

		public int Int() {
			return int.Parse(Read());
		}
		public int Int(string aName) {
			return int.Parse(Read(aName));
		}
		public long Long() {
			return long.Parse(Read());
		}
		public long Long(string aName) {
			return long.Parse(Read(aName));
		}
		public bool Bool() {
			return bool.Parse(Read());
		}
		public bool Bool(string aName) {
			return bool.Parse(Read(aName));
		}
		public float Float() {
			return float.Parse(Read());
		}
		public float Float(string aName) {
			return float.Parse(Read(aName));
		}
		public string String() {
			return Read();
		}
		public string String(string aName) {
			return Read(aName);
		}
		public object Data() {
			return CreateObject(Read());
		}
		public object Data(string aName) {
			return CreateObject(Read(aName));
		}
		public void Data(ref IToFromDataString aBaseObject) {
			string dataString = Read();
			string data = dataString.Substring(dataString.IndexOf('=')+1);
			aBaseObject.FromDataString(data);
		}
		public void Data(string aName, ref IToFromDataString aBaseObject) {
			string dataString = Read(aName);
			string data = dataString.Substring(dataString.IndexOf('=')+1);
			aBaseObject.FromDataString(data);
		}

		private string Read(string aName) {
			if (_type == DataStringType.Ordered)
				throw new Exception("Can't do a named read from an ordered list!");
			int index = Array.IndexOf(_names, aName);
			if (index == -1)
				throw new Exception("Can't find data from given name: " + aName);

			return _words[index];
		}
		private string Read() {
			if (_type == DataStringType.Named)
				throw new Exception("Can't do an ordered read from a named list!");
			if (_curr >= _words.Length)
				throw new Exception("Reading past the end of an ordered data string!");

			string result = _words[_curr];
			_curr += 1;

			return result;
		}

		private object CreateObject(string aDataString) {
			if (string.IsNullOrEmpty(aDataString) || aDataString == "null")
				return null;

			int    sep      = aDataString.IndexOf('=');
			string typeName = aDataString.Substring(0, sep);
			string data     = aDataString.Substring(sep+1);
			Type t = Type.GetType(typeName);
			object result = null;
			if (typeof(IToFromDataString).IsAssignableFrom(t)) {
				if (typeof(ScriptableObject).IsAssignableFrom(t)) {
					result = ScriptableObject.CreateInstance(t);
				} else {
					result = Activator.CreateInstance(t);
				}
				((IToFromDataString)result).FromDataString(data);
			}
			return result;
		}
	}

	public static class DataStringUtil {
		static string _key = "FerrDataStringUtilDefaultKey";

		public static string Encrypt(string aData, string aKey = null) {
			if (string.IsNullOrEmpty(aKey))
				aKey = _key;

			byte[] clearBytes = Encoding.Unicode.GetBytes(aData);
			using (Aes encryptor = Aes.Create()) {
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(aKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV  = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream()) {
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) {
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					aData = Convert.ToBase64String(ms.ToArray());
				}
			}
			return aData;
		}
		public static string Decrypt(string aData, string aKey = null) {
			if (string.IsNullOrEmpty(aKey))
				aKey = _key;

			aData = aData.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(aData);
			using (Aes encryptor = Aes.Create()) {
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(aKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV  = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream()) {
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)) {
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					aData = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return aData;
		}

		public static List<string> SplitSmart(string aData, char aSeparator) {
			List<string> result = new List<string>();
			string curr       = "";
			char   waitingFor = ' ';
			int    waitCount  = 0;
			string data       = aData.Trim();
			if (data.StartsWith("{"))
				data = data.Substring(1, data.Length-2);

			for (int i = 0; i < data.Length; i++) {
				char c = data[i];

				if (waitingFor == ' ') {
					if (c == aSeparator) {
						result.Add(curr);
						curr = "";
						continue;
					}
					else if (c == '{' ) waitingFor = '}';
					else if (c == '"' ) waitingFor = '"';
					else if (c == '\'') waitingFor = '\'';
				} else if (c == waitingFor) {
					if (waitCount == 0)
						waitingFor = ' ';
					else
						waitCount -= 1;
				} else if (c == '{') {
					waitCount += 1;
				}

				curr += c;
			}
			if (curr.Length > 0)
				result.Add(curr);
			if (waitingFor != ' ')
				return null;
			return result;
		}
	}
}