using Messager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;

namespace MessagerLib
{
    [System.Serializable]
    public class scObject
    {
        #region variables declarations
        public string name = "";

        //[NonSerialized]
        private List<scDouble> doubleL = new List<scDouble>();
        //[NonSerialized]
        private List<scFloat> floatL = new List<scFloat>();
        //[NonSerialized]
        private List<scInt> intL = new List<scInt>();
        //[NonSerialized]
        private List<scLong> longL = new List<scLong>();
        //[NonSerialized]
        private List<scString> stringL = new List<scString>();
        //[NonSerialized]
        private List<scBool> boolL = new List<scBool>();
        //[NonSerialized]
        private List<scObject> objectL = new List<scObject>();
        //[NonSerialized]
        private scCred credential;
        //[NonSerialized]
        private List<scClient> clientL = new List<scClient>();
        //[NonSerialized]
        private List<scLocal> localL = new List<scLocal>();
        //[NonSerialized]
        private List<scMenuPanel> panelL = new List<scMenuPanel>();
        //[NonSerialized]
        private List<scCharacter> CharL = new List<scCharacter>();
        //[NonSerialized]
        private scMap Map = new scMap();
        //[NonSerialized]
        private scLocation Location;

        public scClient curClient;
        #endregion

        #region ScObject handler
        //add an object to the object list
        public void addSCObject(scObject x)
        {
            objectL.Add(x);
        }

        //return an ScObject given a string
        public scObject GetSCObject(string x)
        {
            for (int i = 0; i < objectL.Count; i++)
            {
                if (objectL[i].name == x)
                {
                    return objectL[i];
                }
            }
            return null;
        }

        //return an ScObject given an int
        public scObject GetSCObject(int x)
        {
            return objectL[x];
        }

        //return the current number of object
        public int GetSCObjectCount()
        {
            return objectL.Count;
        }

        //naming objects
        public scObject(string x)
        {
            name = x;
        }

        // Indexeur pour accéder à un sous-objet par nom
        public scObject this[string objectName]
        {
            get { return objectL.FirstOrDefault(o => o.name == objectName); }
        }

        // Méthode utilitaire pour vérifier l'existence d'un champ string
        public bool HasString(string key)
        {
            return stringL.Any(s => s.name == key);
        }

        #endregion

        #region ScDouble handler
        public void addScDouble(string x, double y)
        {
            doubleL.Add(new scDouble(x, y));
        }

        public double GetDouble(string x)
        {
            for (int i = 0; i < doubleL.Count; i++)
            {
                if (doubleL[i].name == x)
                {
                    return doubleL[i].value;
                }
            }
            return 0;
        }
        #endregion

        #region ScFloat handler
        public void addScFloat(string x, float y)
        {
            floatL.Add(new scFloat(x, y));
        }

        public float GetFloat(string x)
        {
            for (int i = 0; i < floatL.Count; i++)
            {
                if (floatL[i].name == x)
                {
                    return floatL[i].value;
                }
            }
            return 0f;
        }
        #endregion

        #region ScInt handler
        public void addScInt(string x, int y)
        {
            intL.Add(new scInt(x, y));
        }

        public int GetInt(string x)
        {
            for (int i = 0; i < intL.Count; i++)
            {
                if (intL[i].name == x)
                {
                    return intL[i].value;
                }
            }
            return 0;
        }
        #endregion

        #region ScLong handler
        public void addScLong(string x, long y)
        {
            longL.Add(new scLong(x, y));
        }

        public long GetLong(string x)
        {
            for (int i = 0; i < longL.Count; i++)
            {
                if (longL[i].name == x)
                {
                    return longL[i].value;
                }
            }
            return 0;
        }
        #endregion

        #region ScString handler
        public void addScString(string x, string y)
        {
            stringL.Add(new scString(x, y));
        }

        public string GetString(string x)
        {
            for (int i = 0; i < stringL.Count; i++)
            {
                if (stringL[i].name == x)
                {
                    return stringL[i].value;
                }
            }
            return null;
        }
        #endregion

        #region ScBool handler
        public void addScBool(string x, bool y)
        {
            boolL.Add(new scBool(x, y));
        }

        public bool GetBool(string x)
        {
            for (int i = 0; i < boolL.Count; i++)
            {
                if (boolL[i].name == x)
                {
                    return boolL[i].value;
                }
            }
            return false;
        }
        #endregion

        #region ScCred Handler

        public void addScCred(string user, string pass, string email)
        {
            credential = new scCred(user, pass, email);
        }

        public void addScCred(string user, string pass)
        {
            credential = new scCred(user, pass);
        }

        public scCred GetCred()
        {
            return credential;
        }
        #endregion

        #region ScClient
        public void addScClients(scClient client)
        {
            clientL.Add(client);
        }

        public void setScCurClient(scClient client)
        {
            curClient = client;
        }

        public scClient getCurClient()
        {
            return curClient;
        }

        public List<scClient> GetClients()
        {
            return clientL;
        }

        #endregion

        #region ScLocal

        public void addSLocal(string localname, string localcontent)
        {
            localL.Add(new scLocal(localname, localcontent));
        }

        public String getLocal(string localname)
        {
            for (int i = 0; i < localL.Count; i++)
            {
                if (localL[i].localname == localname)
                {
                    return localL[i].localcontent;
                }
            }
            return null;
        }

        public List<scLocal> scLocal()
        {
            return localL;
        }

        #endregion

        #region ScPanel


        public void addSPanel(int menuType, int StackObjectOrder, int StackSubObjectOrder,
                              string StackObjectType, string StackObjectContent1, string StackObjectContent2, string StackObjectProperties, string StackObjectGradient1, string StackObjectGradient2, string StackObjectTitleColor, string StackObjectContentColor)
        {
            panelL.Add(new scMenuPanel(menuType, StackObjectOrder, StackSubObjectOrder, StackObjectType, StackObjectContent1, StackObjectContent2, StackObjectProperties, StackObjectGradient1, StackObjectGradient2, StackObjectTitleColor, StackObjectContentColor));
        }

        public List<scMenuPanel> scPanel()
        {
            return panelL;
        }

        public void setscPanel(List<scMenuPanel> liste)
        {
            panelL.Clear();
            panelL = liste;
        }


        #endregion

        #region ScCharacter
        public void addScCharacter(scCharacter Character)
        {
            CharL.Add(Character);
        }

        public List<scCharacter> GetCharacter()
        {
            return CharL;
        }
        #endregion
        
        #region ScMap
        public void SetScMap(scMap map)
        {
            Map=map;
        }

        public scMap GetMap()
        {
            return Map;
        }

        public void SetScLocation(scLocation loc)
        {
            Location = loc;
        }

        public scLocation GetLocation()
        {
            return Location;
        }
        #endregion

    }

    [System.Serializable]
    public class scMesHandcheck : message
    {
        public scMesHandcheck(bool isBaned, string banReason) : base("handcheck")
        {
            //creation du scObjet qui contiendra la variable boolean 
            scObject obj = new scObject("accepted bool");
            if (isBaned)
            {
                obj.addScBool("Banned", true);
                obj.addScString("ban reason", banReason);
            }
            else
            {
                obj.addScBool("Banned", false);
            }
            addScObject(obj);
        }
    }



    #region Locals serializable message & response
    [System.Serializable]
    public class scMesReqLocal : message
    {
        public scMesReqLocal(string localLang) : base("request locals")
        {
            //creation du scObjet qui contiendra la variable boolean 
            scObject obj = new scObject("Langue");
            obj.addScString("Local", localLang);
            addScObject(obj);
        }
    }


    [System.Serializable]
    public class scMesResLocal : message
    {
        public scMesResLocal(List<scMenuPanel> localList) : base("response locals")
        {
            //creation du scObjet qui contiendra la variable boolean 
            scObject obj = new scObject("Response");
            obj.setscPanel(localList);
            addScObject(obj);
        }
    }
    #endregion

    #region Login serializable message & response
    [System.Serializable]
    public class scMesLogin : message
    {
        public scMesLogin(string login, string password) : base("request login")
        {
            //creation du scObjet qui contiendra la variable credentiels 
            scObject obj = new scObject("cred");
            obj.addScCred(login, password);
            addScObject(obj);
        }
    }

    [System.Serializable]
    public class scResLogin : message
    {
        public scResLogin(bool res) : base("response login")
        {
            //creation du scObjet qui contiendra la variable credentiels 
            scObject obj = new scObject("response");
            obj.addScBool("login result", res);
            addScObject(obj);
        }
    }

    [System.Serializable]
    public class scResRelList : message
    {
        public scResRelList(List<scCharacter> res) : base("reload charlist")
        {
            //creation du scObjet qui contiendra la variable credentiels 
            scObject obj = new scObject("response");
            foreach (scCharacter c in res)
            {
                obj.addScCharacter(c);
            }
            addScObject(obj);
        }
    }
    #endregion

    #region Register serializable message & response
    [System.Serializable]
    public class scMesRegister : message
    {
        public scMesRegister(string login, string password, string email) : base("request register")
        {
            //creation du scObjet qui contiendra la variable credentiels 
            scObject obj = new scObject("cred");
            obj.addScCred(login, password, email);
            addScObject(obj);
        }
    }

    [System.Serializable]
    public class scResRegister : message
    {
        public scResRegister(bool res) : base("response register")
        {
            //creation du scObjet qui contiendra la variable boolean 
            scObject obj = new scObject("response");
            obj.addScBool("registration result", res);
            addScObject(obj);
        }
    }
    #endregion

    #region creat/del char serializable message & response
    [System.Serializable]
    public class scMesCreatChar : message
    {
        public scMesCreatChar(scCharacter chara) : base("create character")
        {
            //creation du scObject qui continendra la variable scCharater
            scObject obj = new scObject("char");
            obj.addScCharacter(chara);
            addScObject(obj);
        }
    }

    [System.Serializable]
    public class scMesDeletChar : message
    {
        public scMesDeletChar(scCharacter chara) : base("delete character")
        {
            //creation du scObject qui continendra la variable scCharater
            scObject obj = new scObject("char");
            obj.addScCharacter(chara);
            addScObject(obj);
        }
    }
    #endregion

    #region play char serializable message & response
    [System.Serializable]
    public class scMesPlayChar : message
    {
        public scMesPlayChar(scCharacter chara) : base("play character")
        {
            //creation du scObject qui continendra la variable scCharater
            scObject obj = new scObject("char");
            obj.addScCharacter(chara);
            addScObject(obj);
        }
    }

    [System.Serializable]
    public class scResPlayChar : message
    {
        public scResPlayChar(scMap map, scLocation loc) : base("play character")
        {
            //creation du scObject qui continendra la variable scCharater
            scObject obj = new scObject("map");
            obj.SetScMap(map);
            obj.SetScLocation(loc);
            addScObject(obj);
        }
    }
    #endregion

    //[System.Serializable]
    //public class scReqCreaLocation : message
    //{
    //    public scReqCreaLocation(bool hasLoc) : base("req location")
    //    {
    //        scObject obj = new scObject("location");
    //        obj.addScBool("has location", hasLoc);
    //        addScObject(obj);
    //    }
    //}


    #region class declaration of serializable components
    [System.Serializable]
    public class scDouble
    {
        public string name;
        public double value;

        public scDouble(string s, double d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scFloat
    {
        public string name;
        public float value;

        public scFloat(string s, float d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scInt
    {
        public string name;
        public int value;

        public scInt(string s, int d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scLong
    {
        public string name;
        public long value;

        public scLong(string s, long d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scString
    {
        public string name;
        public string value;

        public scString(string s, string d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scBool
    {
        public string name;
        public bool value;

        public scBool(string s, bool d)
        {
            name = s;
            value = d;
        }

    }

    [System.Serializable]
    public class scCred
    {
        public string username;
        public string passworld;
        public string email;

        public scCred(string u, string p, string e)
        {
            username = u;
            passworld = p;
            email = e;
        }
        public scCred(string u, string p)
        {
            username = u;
            passworld = p;
        }
    }

    [System.Serializable]
    public class scClient
    {
        public string Ip;
        public bool isloged;
        public bool isplaying;
        public string charname;
        public int userID;

        public scClient(int uID, string ip, bool log, bool play, string char_name)
        {
            userID = uID;
            Ip = ip;
            isloged = log;
            isplaying = play;
            charname = char_name;
        }

        public scClient(string ip)
        {
            Ip = ip;
            isloged = false;
            isplaying = false;
        }

    }

    [System.Serializable]
    public class scSpaceStats
    {
        public string name;
    }

    [System.Serializable]
    public class scShip
    {
        public string nom;
        public double Mass;
        public double volume;
        public int trustForward;
        public int trustBackward;
        public int trustright;
        public int trustleft;

    }

    [System.Serializable]
    public class scSecteur
    {
        public double x;
        public double y;
        public double z;
    }

    [System.Serializable]
    public class scLocal
    {
        public string localname;
        public string localcontent;

        public scLocal(string s, string c)
        {
            localname = s;
            localcontent = c;
        }
    }

    [System.Serializable]
    public class scMenuPanel
    {
        public int Menu_Type;
        public int StackObjectOrder;
        public int StackSubObjectOrder;
        public string StackObjectType;
        public string StackObjectContent1;
        public string StackObjectContent2;
        public string StackObjectProperties;
        public string StackObjectGradient1;
        public string StackObjectGradient2;
        public string StackObjectTitleColor;
        public string StackObjectContentColor;

        public scMenuPanel(int menuType, int SOOrd, int SSoord, string SOTyp, string SOCo1, string SOCo2, string SOPro, string SOGr1, string SOGr2, string SOTCo, string SOCCo)
        {
            Menu_Type = menuType;
            StackObjectOrder = SOOrd;
            StackSubObjectOrder = SSoord;
            StackObjectType = SOTyp;
            StackObjectContent1 = SOCo1;
            StackObjectContent2 = SOCo2;
            StackObjectProperties = SOPro;
            StackObjectGradient1 = SOGr1;
            StackObjectGradient2 = SOGr2;
            StackObjectTitleColor = SOTCo;
            StackObjectContentColor = SOCCo;
        }
    }

    [System.Serializable]
    public class scCharacter
    {
        public int CharId;
        public int CharUserId;
        public int CharRaceId;
        public string CharName;
        public DateTime DateCreat;
        public string CharBio;
        public string FavColor;
        public int Eyes;
        public int Hair;
        public int Face;
        public int Body;
        public int Foot;
        public int Age;
        public int Strengh;   //affect max health, base damage, carry weigh, can alow destroying stuf to loot but low xp gain from it compare to using lockpikck, tech, magic, elemental
        public int Vitality;  //affect regen health/stamina and stamina regen, 
        public int Dexterity; //affect attack speed, chance to crit, tool usage, advanced weaponery efficiency 
        public int Knowledge; //affect manapool size, number spells and job cappability, disminish arcane fatigue 
        public int Wisdom;    //affect mana regen, spell base damage, magic resist, spells resplenish time
        public int Wittyness; //social skill, affect fleet cap, rescherch, 
        public int Perception;//beneffit everything to a point then give more informations about environment and boos sight
        public int Luck;//beneffit everything to a point then give more informations about environment and boos sight
        public int Genre; //obvious
        public int Level;
        public string SkinColor;
        public int experience;

        public scCharacter(int id, int uid, int rid, string nam, DateTime dat, string bio, string fco, int eye, int hai, int fac, int bod, int foo, int age, int str, int vit, int dex, int kno, int wis, int wit, int per, int luk, int sex, int lvl, string csc, int exp)
        {
            CharId = id;
            CharUserId = uid;
            CharRaceId = rid;
            CharName = nam;
            DateCreat = dat;
            CharBio = bio;
            FavColor = fco;
            Hair = hai;
            Face = fac;
            Body = bod;
            Foot = foo;
            Age = age;
            Strengh = str;   //affect max health, base damage, carry weigh, can alow destroying stuf to loot but low xp gain from it compare to using lockpikck, tech, magic, elemental
            Vitality = vit;  //affect regen health/stamina and stamina regen, 
            Dexterity = dex; //affect attack speed, chance to crit, tool usage, advanced weaponery efficiency 
            Knowledge = kno; //affect manapool size, number spells and job cappability, disminish arcane fatigue 
            Wisdom = wis;    //affect mana regen, spell base damage, magic resist, spells resplenish time
            Wittyness = wit; //social skill, affect fleet cap, rescherch, 
            Perception = per;//beneffit everything to a point then give more informations about environment and boos sight
            Luck = luk;//beneffit everything to a point then give more informations about environment and boos sight
            Genre = sex;
            Level = lvl;
            SkinColor = csc;
            experience = exp;
        }

        public scCharacter()
        {
            CharId = 0;
            CharRaceId = 1;
            CharName = "";
            DateCreat = DateTime.Now;
            CharBio = "";
            Age = 18;
            Strengh = 0;   //affect max health, base damage, carry weigh, can alow destroying stuf to loot but low xp gain from it compare to using lockpikck, tech, magic, elemental
            Vitality = 0;  //affect regen health/stamina and stamina regen, 
            Dexterity = 0; //affect attack speed, chance to crit, tool usage, advanced weaponery efficiency 
            Knowledge = 0; //affect manapool size, number spells and job cappability, disminish arcane fatigue 
            Wisdom = 0;    //affect mana regen, spell base damage, magic resist, spells resplenish time
            Wittyness = 0; //social skill, affect fleet cap, rescherch, 
            Perception = 0;//beneffit everything to a point then give more informations about environment and boos sight
            Luck = 0;//beneffit everything to a point then give more informations about environment and boos sight
            Genre = 0;
            Level = 0;
            experience = 0;
        }

        public scCharacter(CharDefinition _char)
        {
            CharId = 0;
            CharUserId = 0;
            CharRaceId = _char.CharRace;
            CharName = _char.CharName;
            DateCreat = DateTime.Now;
            CharBio = _char.CharBio;
            FavColor = _char.CharFavColor.ToString();
            Hair = _char.CharHair;
            Face = _char.CharFace;
            Body = _char.CharBody;
            Age = _char.CharAge;
            Strengh = _char.CharStrengh;   //affect max health, base damage, carry weigh, can alow destroying stuf to loot but low xp gain from it compare to using lockpikck, tech, magic, elemental
            Vitality = _char.CharVitality;  //affect regen health/stamina and stamina regen, 
            Dexterity = _char.CharDexterity; //affect attack speed, chance to crit, tool usage, advanced weaponery efficiency 
            Knowledge = _char.CharKnowledge; //affect manapool size, number spells and job cappability, disminish arcane fatigue 
            Wisdom = _char.CharWisdom;    //affect mana regen, spell base damage, magic resist, spells resplenish time
            Wittyness = _char.CharWittiness; //social skill, affect fleet cap, rescherch, 
            Perception = _char.CharPerception;//beneffit everything to a point then give more informations about environment and boos sight
            Luck = _char.CharLuck;//beneffit everything to a point then give more informations about environment and boos sight
            Genre = _char.CharSex;
            Level = _char.CharLevel;
            SkinColor = _char.CharSkinColor.ToString();
            experience = (int)_char.CharExp;
        }

        public scCharacter(scCharacter chara)
        {
            CharId = chara.CharId;
            CharUserId = chara.CharUserId;
            CharRaceId = chara.CharRaceId;
            CharName = chara.CharName;
            DateCreat = chara.DateCreat;
            CharBio = chara.CharBio;
            FavColor = chara.FavColor;
            Hair = chara.Hair;
            Face = chara.Face;
            Body = chara.Body;
            Foot = chara.Foot;
            Age = chara.Age;
            Strengh = chara.Strengh;   //affect max health, base damage, carry weigh, can alow destroying stuf to loot but low xp gain from it compare to using lockpikck, tech, magic, elemental
            Vitality = chara.Vitality;  //affect regen health/stamina and stamina regen, 
            Dexterity = chara.Dexterity; //affect attack speed, chance to crit, tool usage, advanced weaponery efficiency 
            Knowledge = chara.Knowledge; //affect manapool size, number spells and job cappability, disminish arcane fatigue 
            Wisdom = chara.Wisdom;    //affect mana regen, spell base damage, magic resist, spells resplenish time
            Wittyness = chara.Wittyness; //social skill, affect fleet cap, rescherch, 
            Perception = chara.Perception;//beneffit everything to a point then give more informations about environment and boos sight
            Luck = chara.Luck;//beneffit everything to a point then give more informations about environment and boos sight
            Genre = chara.Genre;
            Level = chara.Level;
            SkinColor = chara.SkinColor;
            experience = chara.experience;
        }

    }

    [System.Serializable]
    public class scLocation
    {
        public int locationIsOrbit;
        public int locationX;
        public int locationY;
        public int locationZ;
        public int locationSSID;
        
        public scLocation (int SSID, int x, int y, int z, int IIO)
        {
            locationSSID = SSID;
            locationX = x;
            locationY = y;
            locationZ = z;
            locationIsOrbit = IIO;
        }
    }

    [System.Serializable]
    public class scMap
    {
        public int mapSize;
        public int mapID;
        public int mapType;
        public int ATHHeight;
        public List<scTile> mapTiles;

        public List<scTile> getTiles()
        {
            return mapTiles;
        }

    }

    [System.Serializable]
    public class scTile
    {
        public int tilex;
        public int tiley;
        public int tilez;
        public int tiletype;

        public scTile(int tx, int ty, int tz, int tp) 
        {
            tilex = tx;
            tiley = ty;
            tilez = tz;
            tiletype = tp;
        }
        
    }

    #endregion
}
