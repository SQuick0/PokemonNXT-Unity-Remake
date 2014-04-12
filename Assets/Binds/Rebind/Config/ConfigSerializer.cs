using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class ConfigSerializer
{
    public string Path;

    public ConfigSerializer(string path)
    {
        Path = path;
    }

    public void Save(Dictionary<string, List<InputCode>> dict)
    {
        using (StreamWriter sw = new StreamWriter(Path))
        {
            foreach (KeyValuePair<string, List<InputCode>> pair in dict)
            {
                string line = pair.Key.ToString() + "=";
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (i < pair.Value.Count - 1)
                    {
                        line += pair.Value[i].ToString() + ",";
                    }
                    else
                    {
                        line += pair.Value[i].ToString();
                    }
                }
                sw.WriteLine(line);
            }
        }
    }
    private int CountLinesInFile(string f)
    {
        int count = 0;
        using (StreamReader r = new StreamReader(f))
        {
            while ((r.ReadLine()) != null)
            {
                count++;
            }
        }
        return count;
    }

    public Dictionary<string, List<InputCode>> Read()
    {
        Dictionary<string, List<InputCode>> dictionary = new Dictionary<string, List<InputCode>>();
        if (File.Exists(Path))
        {
            string[] lines = new string[CountLinesInFile(Path)];
            using (StreamReader sr = new StreamReader(Path))
            {
                int count = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines[count] = line;
                    count++;
                }
            }

            foreach (string line in lines)
            {
                line.Trim();
                string[] keyandvalue = line.Split('=');
                string[] values = keyandvalue[1].Split(',');
                List<InputCode> working = new List<InputCode>();
                for (int i = 0; i < values.Length; i++)
                {
                    working.Add((InputCode)Enum.Parse(typeof(InputCode), values[i]));
                }

                if (dictionary.ContainsKey(keyandvalue[0]))
                    dictionary[keyandvalue[0]] = working;
                else
                    dictionary.Add(keyandvalue[0], working);
            }
        }
        return dictionary;
    }

}
