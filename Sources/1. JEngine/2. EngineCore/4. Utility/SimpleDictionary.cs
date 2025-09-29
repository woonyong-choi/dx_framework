
using System;
using System.Collections;
using System.IO;


namespace J2y
{
    /*
        SimpleDictionary
        Author: Tonio Loewald
        Date: 4/27/2009

        Implements a simple key/value dictionary (of strings)
        Also allows loading and saving of dictionaries from text files
        "=" should not be used in key strings!

        Usage:
        var d = new SimpleDictionary();

        d.Set( "foo", "bar" );
        print( d.Get( "foo" ) ); // "bar"

        d.Set( "bar", "baz" );
        d.Set( "foo", "blah" );
        print( d.Get( "foo" ) ); // "blah"

        d.Remove( "foo" );
        print( d.Count() ); // 1
        d.Set( "foxtrot", "uniform" );
        d.Save( "test.ini" ); // file will be bar=baz\nfoxtrot=uniform\n
    */


    public class SimpleDictionary // : ScriptableObject 
    {
        public ArrayList keys = new ArrayList();
        public ArrayList values = new ArrayList();

        public String Get(String key)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                if ((string)keys[i] == key)
                {
                    return ((string)values[i]);
                }
            }

            return "";
        }

        public int GetInt(string key)
        {
            int.TryParse(Get(key), out int result);
            return result;
        }
        public float GetFloat(string key)
        {
            float.TryParse(Get(key), out float result);
            return result;
        }
        public int[] GetIntArray(string key)
        {
            var val = Get(key);
            int count = 0;
            int[] result = new int[val.Length / 2 + 1];
            for (int i = 0; i < val.Length; i++)
                if (i % 2 == 0)
                    int.TryParse(val[i].ToString(), out result[count++]);
            return result;
        }

        public float[] GetFloatArray(string name, char separator)
        {
            var line = Get(name);

            var values = line.Split(separator);
            var res = new float[values.Length];

            for (int i = 0; i < res.Length; ++i)
                res[i] = float.Parse(values[i]);

            return res;
        }

        public int[] GetIntArray(string name, string compare)
        {
            var value = Get(name);
            var count = 0;
            var prev = 0;
            var length = 0;

            char[] temp = new char[value.Length];
            temp = value.ToCharArray(0, value.Length);

            // , 갯수.
            for (int i = 0; i < value.Length; i++)
            {
                if (temp[i].ToString() == compare.ToString())
                    count++;
            }


            //, 갯수보다 한개더 많게
            string[] data = new string[count + 1];
            int[] result = new int[count + 1];

            for (int j = 0; j < count + 1; j++)
            {
                //인덱스...
                var index = value.IndexOf(compare, prev);

                //마지막 데이터가 아니면 index - 시작점 = length.(index는 무조건 0부터 시작함.)
                if (j != count)
                    length = index - prev;
                else
                {
                    //마지막 데이터...
                    length = (value.Length) - prev;
                }

                // // Debug.Log(length);

                //데이터 조합.
                for (int i = prev; i < length + prev; i++)
                {
                    data[j] += temp[i].ToString();
                    result[j] = int.Parse(data[j].ToString());
                }


                prev = index + 1;
            }

            return result;
        }


        public bool GetBool(string key)
        {
            bool.TryParse(Get(key), out bool result);
            return result;
        }

        public void Set(String key, String val)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                if ((string)keys[i] == key)
                {
                    values[i] = val;
                    return;
                }
            }

            keys.Add(key);
            values.Add(val);
        }

        public void Remove(String key)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                if ((string)keys[i] == key)
                {
                    keys.RemoveAt(i);
                    values.RemoveAt(i);
                    return;
                }
            }
            // Debug.Log( "SimpleDictionary.Remove failed, key not found: " + key );
        }

        public void Save(String fileName)
        {
            var dataPath = "."; // Application.dataPath 
            var sw = new StreamWriter(dataPath + "/" + fileName);
            for (var i = 0; i < keys.Count; i++)
            {
                sw.WriteLine(keys[i] + "=" + values[i]);
            }
            sw.Close();
            // Debug.Log("SimpleDictionary.Saved " + Application.dataPath + "/" + fileName);
        }

        public void Load(String fileName)
        {
            keys = new ArrayList();
            values = new ArrayList();

            var line = "-";
            int offset;
            try
            {
                var fullname = fileName;
                var sr = new StreamReader(fileName);
                line = sr.ReadLine();
                while (line != null)
                {
                    offset = line.IndexOf("=");
                    if (offset > 0)
                    {
                        Set(line.Substring(0, offset), line.Substring(offset + 1));
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                // Debug.Log("SimpleDictionary.Loaded " + fileName);
            }
            catch (Exception)
            {
                // Debug.Log("SimpleDictionary.Load failed: " + fileName);
            }
        }

        public int Count()
        {
            return keys.Count;
        }
    }



}
