using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;

namespace clientbackup
{
    public class Serialization
    {
        public static void serialize(bool o)
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/autologon.txt", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, o);
            fichier.Close();
        }

        public static void serialize(ArrayList list)
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/files.txt", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, list);
            fichier.Close();
        }

        public static bool deserialize()
        {
            try
            {
                FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/autologon.txt", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                bool o = (bool)bf.Deserialize(fichier);
                fichier.Dispose();
                return o;
            }
            catch (FileNotFoundException)
            {
                serialize(false);
                return false;
            }

        }

        public static ArrayList deserializeList()
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/files.txt", FileMode.Open);

            BinaryFormatter bf = new BinaryFormatter();
            ArrayList list = (ArrayList)bf.Deserialize(fichier);
            fichier.Dispose();
            return list;
        }

        public static void serializeToXML(List<string> list, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/" + filename, FileMode.Create);
            serializer.Serialize(fichier, list);
            fichier.Close();
            fichier.Dispose();
        }

        public static List<string> deserializeXML(string filename)
        {
            System.Diagnostics.Debug.WriteLine("folder.xml chargé");
            var xmlFile = Environment.CurrentDirectory + @"/Data/" + filename;
            if (!File.Exists(xmlFile))
            {
                serializeToXML(new List<string>(), filename);
                return new List<string>();
            }
            FileStream fichier = new FileStream(xmlFile, FileMode.Open);
            try
            {
              
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                List<string> list = (List<string>)serializer.Deserialize(fichier);
                fichier.Close();
                fichier.Dispose();
                return list;
            } 
            catch(Exception exception)
            {
                Log.Write(exception.Message);
                return new List<string>();
            }
        }

        public static void serializeLastSaveDate(DateTime date)
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastSaveDate.aut", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, date);
            fichier.Close();
        }

        public static DateTime deserializeLastSaveDate(bool init)
        {
            try
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"/Data");
                FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastSaveDate.aut", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                DateTime dt = (DateTime)bf.Deserialize(fichier);
                fichier.Dispose();
                return dt;
            }
            catch (FileNotFoundException)
            {
                if (init)
                {
                    serializeLastSaveDate(DateTime.Now);
                }

                return new DateTime(2000, 1, 1);
            }
        }

        public static string deserializeEmpFichierConfig()
        {
            return ConfigurationManager.AppSettings["empFichierConfig"];
        }

        public static Configuration deserializeConfig()
        {
            FileStream fichier = new FileStream(deserializeEmpFichierConfig(), FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            Configuration c = (Configuration)bf.Deserialize(fichier);
            fichier.Dispose();
            return c;
        }

        public static void serializeLastVirtualSaveDate()
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastVirtualSaveDate.aut", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, DateTime.Now);
            fichier.Close();
        }

        public static DateTime deserializeLastVirtualSaveDate()
        {
            if (!File.Exists(Environment.CurrentDirectory + @"/Data/lastVirtualSaveDate.aut"))
            {
                serializeLastVirtualSaveDate();
            }
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastVirtualSaveDate.aut", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            DateTime dt = (DateTime)bf.Deserialize(fichier);
            fichier.Dispose();
            return dt;
        }

        public static void serializeMDPAdmin(Dictionary<String,String> passwordsHash)
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/MDP.bin", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, passwordsHash);
            fichier.Close();
        }

        public static Dictionary<String,String> deserializeMDPAdmin()
        {
            try
            {
                FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/MDP.bin", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                var dico = (Dictionary<String,String>)bf.Deserialize(fichier);
                fichier.Dispose();
                return dico;
            }
            catch
            {
                var dico = new Dictionary<string,string>();
                dico.Add("admin","");
                dico.Add("user","");
                return dico;
            } 
        }

        public static void serializeEtatDerniereSave(char c)
        {
            FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastSaveState.aut", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, c);
            fichier.Close();
        }

        public static char deserializeEtatDerniereSave()
        {
            try
            {
                FileStream fichier = new FileStream(Environment.CurrentDirectory + @"/Data/lastSaveState.aut", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                char c = (char)bf.Deserialize(fichier);
                fichier.Dispose();
                return c;
            }
            catch
            {
                serializeEtatDerniereSave(' ');
                return ' ';
            }
        }

    
    } 
}
