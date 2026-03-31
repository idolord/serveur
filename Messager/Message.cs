using System;
using System.Collections.Generic;

namespace MessagerLib
{
    [System.Serializable]
    public class message
    {
        public string messageText ="";
        //[NonSerialized]
        private List<scObject> scObject = new List<scObject>();

        public message(string x)
        {
            messageText = x;
        }

        public void addScObject(scObject x)
        {
            scObject.Add(x);
        }

        public scObject GetScObject(string x)
        {
            for (int i = 0; i < scObject.Count; i++)
            {
                if (scObject[i].name == x)
                {
                    return scObject[i];
                }
            }
            return null;
        }

        public scObject GetScObject(int x)
        {
            return scObject[x];
        }

        public int GetScObjectCount()
        {
            return scObject.Count;
        }

    }

}
