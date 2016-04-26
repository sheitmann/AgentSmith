using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgentSmith.SpellCheck.NetSpell
{
    /// <summary>
    /// Summary description for <see cref="OpenOfficeDictionaryImporter"/>.
    /// </summary>
    public class OpenOfficeDictionaryImporter
    {
        private readonly IDictionary<string, string> _words = new Dictionary<string, string>();
        private string _tryChars = "";
        private string _prefix = "";
        private string _suffix = "";
        private string _replace = "";
        private Encoding _encoding = Encoding.UTF7;

        public static void Import(string affixFile, string wordFile, string outFile)
        {
            OpenOfficeDictionaryImporter importer = new OpenOfficeDictionaryImporter();
            importer.loadAffix(affixFile);
            importer.loadWords(wordFile);
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                importer.saveDictionary(sw);
                importer._words.Clear();
                ms.Seek(0, SeekOrigin.Begin);
                new WordDictionary(new StreamReader(ms));
                ms.Seek(0, SeekOrigin.Begin);
                File.WriteAllText(outFile, new StreamReader(ms).ReadToEnd());
            }
        }

        private void loadAffix(string fileName)
        {
            _encoding = Encoding.UTF7;
            using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
            {
                string tempLine = sr.ReadLine();
                while (tempLine != null)
                {
                    if (tempLine.StartsWith("SET "))
                    {
                        string encodingName = tempLine.Substring(4);
                        if (encodingName.ToLower().StartsWith("iso"))
                        {
                            encodingName = encodingName.Insert(3, "-");
                        }
                        _encoding = Encoding.GetEncoding(encodingName);
                        break;
                    }
                    tempLine = sr.ReadLine();
                }
            }

            using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), _encoding))
            {
                sr.ReadLine();

                StringBuilder prefix = new StringBuilder();
                StringBuilder suffix = new StringBuilder();
                StringBuilder replace = new StringBuilder();

                while (sr.Peek() >= 0)
                {
                    string tempLine = sr.ReadLine().Trim();
                    if (tempLine.Length > 3)
                    {
                        switch (tempLine.Substring(0, 3))
                        {
                            case "TRY":
                                _tryChars = tempLine.Substring(4);
                                break;
                            case "PFX":
                                prefix.AppendLine(tempLine.Substring(4));
                                break;
                            case "SFX":
                                suffix.AppendLine(tempLine.Substring(4));
                                break;
                            case "REP":
                                if (!char.IsNumber(tempLine.Substring(4)[0]))
                                {
                                    replace.AppendLine(tempLine.Substring(4));
                                }
                                break;
                        }
                    }
                }
                _prefix = prefix.ToString();
                _suffix = suffix.ToString();
                _replace = replace.ToString();
            }
        }

        private void loadWords(string fileName)
        {
            using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), _encoding))
            {
                sr.ReadLine();
                // read line by line
                while (sr.Peek() >= 0)
                {
                    string tempLine = sr.ReadLine().Trim();

                    if (tempLine.Length > 0 && !char.IsNumber(tempLine[0]))
                    {
                        string[] parts = tempLine.Split('/');
                        string word = parts[0];
                        string affixKeys = "";
                        if (parts.Length > 1)
                        {
                            affixKeys = parts[1];
                        }

                        // look for duplicate words
                        if (_words.ContainsKey(word))
                        {
                            // merge affix keys on duplicate words
                            string[] tempParts = _words[word].Split('/');
                            string oldKeys = "";
                            if (tempParts.Length > 1)
                            {
                                oldKeys = tempParts[1];
                            }

                            foreach (char key in oldKeys)
                            {
                                // if the new affix keys do not contain old key, add old key to new keys
                                if (affixKeys.IndexOf(key) == -1)
                                {
                                    affixKeys += key.ToString();
                                }
                            }
                            // only update if have keys
                            if (affixKeys.Length > 0)
                            {
                                _words[word] = string.Format("{0}/{1}", word, affixKeys);
                            }
                        }
                        else
                        {
                            _words.Add(word, tempLine);
                        }
                    }
                }
            }
        }

        private void saveDictionary(StreamWriter sw)
        {
            sw.NewLine = "\n";

            sw.WriteLine("[Try]");
            sw.WriteLine(_tryChars);
            sw.WriteLine();

            sw.WriteLine("[Replace]");
            sw.WriteLine(_replace);

            sw.WriteLine("[Prefix]");
            sw.WriteLine(_prefix);

            sw.WriteLine("[Suffix]");
            sw.WriteLine(_suffix);

            sw.WriteLine("[Words]");
            foreach (string tempWord in _words.Values)
            {
                sw.WriteLine(tempWord);
            }
        }
    }
}