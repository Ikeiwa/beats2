using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

namespace Beats2.Data {

	public class IniFile {
		private const string TAG = "IniFile";

		private static string COMMENT_CHARS = ";#/";
		private static char[] VALUE_SEPARATORS = {':', ','};

		private Dictionary<string, Dictionary<string, List<string[]>>> _content;

		public IniFile(string url) {
			_content = new Dictionary<string, Dictionary<string, List<string[]>>>();
			Parse(url);
		}

		private void Parse(string url) {
			if (File.Exists(url)) {
				string[] lines = File.ReadAllLines(url);
				string sectionName = String.Empty;
				Dictionary<string, List<string[]>> sectionValues = new Dictionary<string, List<string[]>>();
				foreach (string lineRaw in lines) {
					string line = lineRaw.Trim();
					if (line.Length == 0) { // Empty line
						continue;
					} else if (COMMENT_CHARS.IndexOf(line[0]) != -1) { // Comment
						continue;
					} else if (line[0] == '[') { // Section start
						if (line.IndexOf(']') != -1) {
							sectionName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - 1);
							sectionValues = new Dictionary<string, List<string[]>>();
							_content.Add(sectionName, sectionValues);
						}
					} else { // Key-value pair
						if (line.IndexOf('=') != -1) {
							int indexEquals = line.IndexOf('=');
							string key = line.Substring(0, indexEquals);
							string[] values = line.Substring(indexEquals + 1).Split(VALUE_SEPARATORS);
							if (!_content[sectionName].ContainsKey(key)) {
								_content[sectionName].Add(key, new List<string[]>(1)); // Use initial capacity of 1 since usually no duplicates
							}
							_content[sectionName][key].Add(values);
						}
					}
				}
			}
		}

		public List<string[]> Get(string section, string key) {
			if (_content.ContainsKey(section) && _content[section].ContainsKey(key)) {
				return _content[section][key];
			} else {
				Logger.Error(TAG, String.Format("Unable to fetch key \"{0}\" from section \"{1}\"", key, section));
				return null;
			}
		}

		public bool Set(string section, string key, List<string[]> values) {
			if (_content.ContainsKey(section) && _content[section].ContainsKey(key)) {
				_content[section][key] = values;
				return true;
			} else {
				Logger.Error(TAG, String.Format("IniFile does not contain key \"{0}\" from section \"{1}\"", key, section));
				return false;
			}
		}

		public bool Write(string url) {
			StreamWriter writer;
			if (File.Exists(url)) {
				writer = new StreamWriter(url, false);
			} else {
				writer = File.CreateText(url);
			}
			string line;
			foreach (KeyValuePair<string, Dictionary<string, List<string[]>>> section in _content) {
				line = String.Format("[{0}]", section.Key);
				writer.WriteLine(line);
				foreach (KeyValuePair<string, List<string[]>> pair in section.Value) {
					foreach (string[] list in pair.Value) {
						StringBuilder sb = new StringBuilder();
						foreach (string s in list) {
							if (sb.Length != 0) {
								sb.Append(',');
							}
							sb.Append(s);
						}
						line = String.Format("{0}={1}", pair.Key, sb.ToString());
						writer.WriteLine(line);
					}
				}
			}
			writer.Flush();
			writer.Close();
			return true;
		}
	}
}
