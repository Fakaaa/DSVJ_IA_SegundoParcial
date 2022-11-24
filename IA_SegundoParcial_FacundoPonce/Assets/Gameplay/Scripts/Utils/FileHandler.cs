using System.IO;

using UnityEngine;

namespace InteligenciaArtificial.SegundoParcial.Utils.Files
{
    public static class FileHandler<T> where T : class
    {
        #region EXPOSED_FIELDSS
        public static string directory = "/SavedGenerations/";
        public static string fileName = "Brain.txt";
        #endregion

        #region PUBLIC_METHODS
        public static void Save(T savedObject, string team, string generation, string fitness)
        {
            string dir = Application.persistentDataPath + directory;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string[] baseFileName = fitness.Split(".");
            string fullFileName = baseFileName[0] + "_" + team + "_" + generation + "_[Fitness " + fitness + "]" + baseFileName[1];
            string dataSerialized = JsonUtility.ToJson(savedObject);

            File.WriteAllText(dir + fullFileName, dataSerialized);
        }

        public static void Save(T savedObject)
        {
            string dir = Application.persistentDataPath + directory;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string dataSerialized = JsonUtility.ToJson(savedObject);
            File.WriteAllText(dir + fileName, dataSerialized);
        }

        public static T Load()
        {
            string fullPath = Application.persistentDataPath + directory + fileName;
            T newObj = null;

            if(File.Exists(fullPath))
            {
                string dataDeserialized = File.ReadAllText(fullPath);
                newObj = JsonUtility.FromJson<T>(dataDeserialized);
            }
            else
            {
                Debug.LogWarning("The file wasn't found");
            }

            return newObj;
        }
        #endregion
    }
}