using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using MessagerLib;
using System.IO.Pipes;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Data.Entity.Core.Metadata.Edm;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace server
{


    //class qui gère la génération et le chargement des config
    static class serverConfigHandler
    {


        //récupération du chemin d'acces de l'application
        private static string path = AppDomain.CurrentDomain.BaseDirectory;

        //délégué de réglage des config
        delegate void ConfigsXml(string path);

        #region path getter

        //retourne le chemin d'acces du repertoir config
        static private string getConfigFolderPath(string s)
        {
            return path + "/config";
        }

        //retourne le chemin d'acces du fichier config
        static private string getConfigFilePath(string s)
        {
            string temp = getConfigFolderPath(s);
            return temp + "/serverConfig.xml";
        }

        static private string getBanFilePath(string s)
        {
            string temp = getConfigFolderPath(s);
            return temp + "/BanList.txt";
        }
        #endregion

        #region resolve config files
        //handle everything
        static public void resolveConfig()
        {
            ConfigsXml delegator = makeFileSys;
            delegator += loadConfigs;
            delegator(path);
        }

        //créer les fichiers si besoin est.
        static void makeFileSys(string s)
        {
            string configPath = getConfigFolderPath(s);
            string configFilePath = getConfigFilePath(s);
            //string banListFilePath = getBanFilePath(s);
            if (!Directory.Exists(configPath))
            {
                outputConsoleMain.ouToScreen("répértoir non trouvé...", true);
                Directory.CreateDirectory(configPath);
                outputConsoleMain.ouToScreen("création des fichiers de config.", true);
                var file = File.Create(configFilePath);
                file.Close();
            }
            else
            {
                outputConsoleMain.ouToScreen("répértoir trouvé ...", true);
                if (!File.Exists(configFilePath))
                {
                    outputConsoleMain.ouToScreen("ficher de config absent...", true);
                    var file = File.Create(configFilePath);
                    outputConsoleMain.ouToScreen("création du fichier de config.", true);
                    file.Close();
                }
                outputConsoleMain.ouToScreen("tout les fichier de config ont été trouvé.", true);
            }
        }

        //ecris le fichier de configuration de base
        static void writeBaseConfig(string s)
        {
            string configFilePath = getConfigFilePath(s);
            XDocument Writting = new XDocument(
                new XDeclaration("1.0", Encoding.UTF8.ToString(), "yes"),
                new XElement("configs",
                new XComment("configuration des port du serveur"),
                new XElement("serverPortConfigs",
                    new XElement("PFR_port", 2999),
                    new XElement("client_port", 3000)),
                new XComment("configuration database"),
                new XElement("DBSettings",
                    new XElement("IP", "127.0.0.1"),
                    new XElement("DBUser", ""),
                    new XElement("DBUserPass", ""),
                    new XElement("DBName", "ti"),
                    new XElement("DBPort", 3366))));
            Writting.Save(configFilePath);
            outputConsoleMain.ouToScreen("fichier de config créé avec succes!", true);
        }


        //charge le fichier de configuration
        static void loadConfigs(string s)
        {
            try
            {
                int PFRPort = 0, ClientPort = 0;
                string configFilePath = getConfigFilePath(s);
                XmlDocument configs = new XmlDocument();
                configs.Load(configFilePath);
                PFRPort = int.Parse(configs.SelectSingleNode("configs/serverPortConfigs/PFR_port").InnerText);
                ClientPort = int.Parse(configs.SelectSingleNode("configs/serverPortConfigs/client_port").InnerText);
                setter del = serverConf.getPortDelegate();
                del(PFRPort, ClientPort);
                string
                    i = configs.SelectSingleNode("configs/DBSettings/IP").InnerText,
                    u = configs.SelectSingleNode("configs/DBSettings/DBUser").InnerText,
                    p = configs.SelectSingleNode("configs/DBSettings/DBUserPass").InnerText,
                    d = configs.SelectSingleNode("configs/DBSettings/DBName").InnerText;
                int pr = int.Parse(configs.SelectSingleNode("configs/DBSettings/DBPort").InnerText);
                //outputConsoleMain.ouToScreen(i + " " + u + " " + p, false);

                DBHandler.setDB(i, u, p, d, pr);

                outputConsoleMain.ouToScreen("configurations chargées avec succes!", true);
                outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerBot(), true);
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "loadConfigs");
                writeBaseConfig(s);
                loadConfigs(s);
            }
        }

        #endregion

        #region banlist handler

        //TODO: 
        public static List<string[]> getBanedIp()
        {
            List<string[]> banedIp = new List<string[]>();
            return banedIp;
        }

        #endregion

    }

    #region console display handler
    //class qui permet d'afficher du text sur la console
    static class outputConsoleMain
    {
        private static ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
        private static StreamWriter logWriter;
        private static readonly object logLock = new object();
        private static readonly string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "config/server_error.log";

        static outputConsoleMain()
        {
            // S'assurer que le dossier existe
            string dir = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Ouvre le fichier en mode append
            logWriter = new StreamWriter(logFilePath, true, Encoding.UTF8) { AutoFlush = true };

            // Ferme le fichier à la fermeture de l'application
            AppDomain.CurrentDomain.ProcessExit += (s, e) => CloseLog();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => CloseLog();
        }

        public static void init()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.BufferHeight = 1030;
            Console.BufferWidth = 120;
            Console.SetWindowSize(120, 50);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void resetConsole()
        {
            Console.Clear();
            foreach (var msg in messages)
            {
                displayLineColored(msg);
            }
            Console.CursorTop = 47;
        }

        public static string normalizeline(string s)
        {
            string temp = s;
            if (temp.Length < 116)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("║ ");
                sb.Append(temp);

                for (int i = 0; i < 118; i++)
                {
                    if (i > temp.Length)
                    {
                        sb.Append(" ");
                    }
                }
                sb.Append("║");
                temp = sb.ToString();
                sb.Clear();
            }
            return temp;
        }

        public static string buildheader(string s)
        {
            int spacer = (int)Math.Floor((double)56 - (s.Length / 2));
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb.Append("╟");
            for (int i = 0; i < spacer; i++)
            {
                sb.Append("─");
            }
            sb2.Append(sb);
            sb2.Append("◄  " + s + "  ►");
            for (int i = 0; i < spacer; i++)
            {
                sb2.Append("─");
            }
            for (int i = sb2.Length; i <= 120; i++)
            {
                sb2.Append("─");
            }
            sb2.Append("╢");
            return sb2.ToString();
        }

        public static void consoleHeader(string s)
        {
            displayLineColored(consoleSpacerTop());
            messages.Enqueue(consoleSpacerTop());
            displayLineColored(buildheader(s));
            messages.Enqueue(buildheader(s));
            displayLineColored(consoleSpacerBot());
            messages.Enqueue(consoleSpacerBot());
        }

        public static void displayLineColored(string t)
        {
            string temp;
            if (t != "")
            {
                temp = normalizeline(t);
            }
            else
            {
                temp = t;
            }
            for (int i = 0; i < temp.Length; i++)
            {
                if (i < Math.Floor((float)(temp.Length) / 3))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(temp[i]);
                }
                else if (i < (Math.Floor((float)(temp.Length) / 3)) * 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(temp[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(temp[i]);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string center(string t)
        {
            string temp;
            if (t.Length < 99)
            {
                int spaceToAdd = (int)Math.Floor((double)(99 - t.Length) / 2);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < spaceToAdd; i++)
                {
                    sb.Append(" ");
                }
                sb.Append(t);
                temp = sb.ToString();
                sb.Clear();
                return temp;
            }
            else
            {
                return t;
            }
        }

        public static void ouToScreen(string t, bool r)
        {
            displayLineColored(t);
            if (r)
            {
                messages.Enqueue(t);
            }
        }

        public static void ouToScreenNormal(string t)
        {
            Console.WriteLine(t);
        }

        public static string consoleSpacer()
        {
            return "╟──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────╢";
        }
        public static string consoleSpacerTop()
        {
            return "╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗";
        }
        public static string consoleSpacerBot()
        {
            return "╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╝";
        }

        public static void consoleWSpacer()
        {
            messages.Enqueue("");
        }

        public static void LogError(Exception e, string context = "")
        {
            string message = $"[{DateTime.Now}] ERREUR{(string.IsNullOrEmpty(context) ? "" : $" ({context})")}: {e.Message}\n{e.StackTrace}";
            ouToScreen(message, true);

            lock (logLock)
            {
                logWriter.WriteLine(message);
                logWriter.WriteLine("--------------------------------------------------");
            }
        }

        private static void CloseLog()
        {
            lock (logLock)
            {
                if (logWriter != null)
                {
                    logWriter.Flush();
                    logWriter.Close();
                    logWriter = null;
                }
            }
        }
    }

    //class page d'inspection
    static class inspectPage
    {
        public static viewHandler clientview = new viewHandler("client_view");


        public static void startClientInfoWindow()
        {
            clientview.startview(infoscreen.checkinfo());
        }

        public static void addClientInfoWindow(scClient sc)
        {
            clientview.sendViewDataUpdate(sc);
        }

        public static void init()
        {

        }
    }
    #endregion


    #region db handler
    //class qui gère la database
    static class DBHandler
    {
        //variable sui contiens les infos crédential et de connection
        private static string
                ipadress = "",
                user = "",
                pass = "",
                db = "";


        private static int port = 0;

        //methode qui set la base de donée
        public static void setDB(string i, string u, string p, string d, int pr)
        {
            ipadress = i;
            user = u;
            pass = p;
            db = d;
            port = pr;
        }

        //methode qui retourne la connection a la base de donée
        public static MySqlConnection getConnection()
        {
            string mCon = "server=" + ipadress + ";database=" + db + ";user id=" + user + ";password=" + pass + ";";
            return new MySqlConnection(mCon);
        }

        //methode qui retourne "vrais" si la database est joinable
        public static bool isBDReachable()
        {
            MySqlConnection sqlCon = getConnection();
            try
            {
                sqlCon.Open();
                outputConsoleMain.ouToScreen("la connection avec la base de donnée est effective", true);
                sqlCon.Close();
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "isBDReachable");
                outputConsoleMain.ouToScreen("la connection avec la base de donnée est innefective, l'erreur retournée est: ", true);
                if (e.HResult == -2147467259)
                {
                    outputConsoleMain.ouToScreen("identifiants de base de donnée éronés, verifiez vos identifiants dans config/serverConfig", true);
                }
                else
                {
                    outputConsoleMain.ouToScreen(e.HResult.ToString(), true);
                    outputConsoleMain.ouToScreen(e.ToString(), true);
                    outputConsoleMain.ouToScreen(e.StackTrace, true);
                }
                sqlCon.Close();
                return false;
            }
            return true;
        }

        public static bool tryDeleteChar(scCharacter cha, int userid)
        {
            MySqlConnection sqlcon = getConnection();
            try
            {
                sqlcon.Open();
                // Supprimer l'inventaire du personnage
                string sqlInv = "DELETE FROM char_inventory WHERE CharID = @CharId";
                using (var cmdInv = new MySqlCommand(sqlInv, sqlcon))
                {
                    cmdInv.Parameters.AddWithValue("@CharId", cha.CharId);
                    cmdInv.ExecuteNonQuery();
                }
                // Supprimer le personnage
                string sql = "DELETE FROM chars WHERE CharUserID = @UserId AND CharID = @CharId";
                MySqlCommand cmd = new MySqlCommand(sql, sqlcon);
                cmd.Parameters.AddWithValue("@UserId", cha.CharUserId);
                cmd.Parameters.AddWithValue("@CharId", cha.CharId);
                int rowaffected = cmd.ExecuteNonQuery();
                return rowaffected > 0;
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "tryDeleteChar");
                throw;

            }
        }

        public static void AddItemToCharInventory(int charId, int itemId, int quantity)
        {
            using (var sqlcon = getConnection())
            {
                sqlcon.Open();
                // Récupérer l'ID d'inventaire du personnage
                string sqlGetInv = "SELECT InventoryID FROM char_inventory WHERE CharID = @CharId AND Type = 'item' LIMIT 1";
                int inventoryId = 0;
                using (var cmdGetInv = new MySqlCommand(sqlGetInv, sqlcon))
                {
                    cmdGetInv.Parameters.AddWithValue("@CharId", charId);
                    using (var reader = cmdGetInv.ExecuteReader())
                    {
                        if (reader.Read())
                            inventoryId = reader.GetInt32(0);
                    }
                }
                if (inventoryId == 0) throw new Exception("Inventaire non trouvé pour le personnage.");

                // Ajouter l'item
                string sqlAdd = @"INSERT INTO inventory_item (InventoryID, ItemID, Quantity)
                          VALUES (@invId, @itemId, @qty)
                          ON DUPLICATE KEY UPDATE Quantity = Quantity + @qty";
                using (var cmdAdd = new MySqlCommand(sqlAdd, sqlcon))
                {
                    cmdAdd.Parameters.AddWithValue("@invId", inventoryId);
                    cmdAdd.Parameters.AddWithValue("@itemId", itemId);
                    cmdAdd.Parameters.AddWithValue("@qty", quantity);
                    cmdAdd.ExecuteNonQuery();
                }
            }
        }

        public static List<(int ItemID, int Quantity)> GetCharInventoryItems(int charId)
        {
            var items = new List<(int, int)>();
            using (var sqlcon = getConnection())
            {
                sqlcon.Open();
                string sql = @"SELECT ii.ItemID, ii.Quantity
                       FROM char_inventory ci
                       JOIN inventory_item ii ON ci.InventoryID = ii.InventoryID
                       WHERE ci.CharID = @CharId AND ci.Type = 'item'";
                using (var cmd = new MySqlCommand(sql, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@CharId", charId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add((reader.GetInt32(0), reader.GetInt32(1)));
                        }
                    }
                }
            }
            return items;
        }

        public static scLocation tryPlayChar(scCharacter chara, int userId)
        {
            try
            {
                using (var sqlcon = getConnection())
                {
                    sqlcon.Open();
                    scLocation loc = GetCharacterLocation(chara.CharId, userId, sqlcon);

                    if (loc != null)
                    {
                        outputConsoleMain.ouToScreen("le personnage a déjà une location => loading", true);
                        setCharLocation(chara, loc);
                        return loc;
                    }
                    else
                    {
                        // Pas de location => set location spawn point
                        loc = new scLocation(0, 0, 0, 0, 0);
                        setCharLocation(chara, loc);
                        return loc;
                    }
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "tryPlayChar");
                throw;
            }
        }

        /// <summary>
        /// Récupère la location d'un personnage à partir de son ID et de l'ID utilisateur.
        /// Retourne null si aucune location n'est trouvée.
        /// </summary>
        private static scLocation GetCharacterLocation(int charId, int userId, MySqlConnection sqlcon)
        {
            string sql = @"SELECT l.locationID, l.locationSSID, l.locationX, l.locationY, l.locationMapID
                           FROM location l
                           INNER JOIN chars c ON l.locationID = c.CharLocID
                           WHERE c.CharUserID = @UserId AND c.CharID = @CharId
                           LIMIT 1";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CharId", charId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new scLocation(
                            reader.GetInt32(0), // locationID
                            reader.GetInt32(1), // locationSSID
                            reader.GetInt32(2), // locationX
                            reader.GetInt32(3), // locationY
                            reader.GetInt32(4)  // locationMapID
                        );
                    }
                }
            }
            return null;
        }

        public static bool tryCreateChar(scCharacter cha, int userid)
        {
            try
            {
                using (var sqlcon = getConnection())
                {
                    sqlcon.Open();

                    if (CharNameExists(cha.CharName, sqlcon))
                    {
                        outputConsoleMain.ouToScreen("le nom de personnage existe deja", true);
                        return false;
                    }

                    int newCharId = GetNextCharId(sqlcon);
                    InsertChar(cha, userid, newCharId, DateTime.Now, sqlcon);
                    return true;
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "tryCreateChar");
                throw;
            }
        }

        private static bool CharNameExists(string charName, MySqlConnection sqlcon)
        {
            string sql = "SELECT 1 FROM chars WHERE CharName = @CharName LIMIT 1";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                cmd.Parameters.AddWithValue("@CharName", charName);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        private static int GetNextCharId(MySqlConnection sqlcon)
        {
            string sql = "SELECT MAX(CharID) FROM chars";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                        return reader.GetInt32(0) + 1;
                    else
                        return 1;
                }
            }
        }

        private static void InsertChar(scCharacter cha, int userid, int id, DateTime creationDate, MySqlConnection sqlcon)
        {
            string sql = @"INSERT INTO chars (
                CharID, CharUserID, RaceID, CharName, CharDateCreat, CharBio, CharFavColor,
                CharEyes, CharHair, CharFace, CharBody, CharFoot, CharAge,
                CharStrengh, CharVitality, CharDexterity, CharKnowledge, CharWisdom, CharWittiness, CharPerception, CharLuck,
                CharSex, Charlevel, CharSkinCol, CharExperience
            ) VALUES (
                @id, @userid, @raceid, @charname, @datecreat, @bio, @favcolor,
                @eyes, @hair, @face, @body, @foot, @age,
                @strengh, @vitality, @dexterity, @knowledge, @wisdom, @wittyness, @perception, @luck,
                @genre, @level, @skincolor, @experience
            )";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@raceid", cha.CharRaceId);
                cmd.Parameters.AddWithValue("@charname", cha.CharName);
                cmd.Parameters.AddWithValue("@datecreat", creationDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                cmd.Parameters.AddWithValue("@bio", cha.CharBio);
                cmd.Parameters.AddWithValue("@favcolor", cha.FavColor);
                cmd.Parameters.AddWithValue("@eyes", cha.Eyes);
                cmd.Parameters.AddWithValue("@hair", cha.Hair);
                cmd.Parameters.AddWithValue("@face", cha.Face);
                cmd.Parameters.AddWithValue("@body", cha.Body);
                cmd.Parameters.AddWithValue("@foot", cha.Foot);
                cmd.Parameters.AddWithValue("@age", cha.Age);
                cmd.Parameters.AddWithValue("@strengh", cha.Strengh);
                cmd.Parameters.AddWithValue("@vitality", cha.Vitality);
                cmd.Parameters.AddWithValue("@dexterity", cha.Dexterity);
                cmd.Parameters.AddWithValue("@knowledge", cha.Knowledge);
                cmd.Parameters.AddWithValue("@wisdom", cha.Wisdom);
                cmd.Parameters.AddWithValue("@wittyness", cha.Wittyness);
                cmd.Parameters.AddWithValue("@perception", cha.Perception);
                cmd.Parameters.AddWithValue("@luck", cha.Luck);
                cmd.Parameters.AddWithValue("@genre", cha.Genre);
                cmd.Parameters.AddWithValue("@level", cha.Level);
                cmd.Parameters.AddWithValue("@skincolor", cha.SkinColor);
                cmd.Parameters.AddWithValue("@experience", cha.experience);
                cmd.ExecuteNonQuery();
            }
            // Création de l'inventaire du personnage
            string sqlInv = @"INSERT INTO char_inventory (CharID, Type) VALUES (@charid, 'item')";
            using (var cmdInv = new MySqlCommand(sqlInv, sqlcon))
            {
                cmdInv.Parameters.AddWithValue("@charid", id);
                cmdInv.ExecuteNonQuery();
            }
        }

        public static bool doIdCorrespond(scCred mes)
        {
            bool temp = false;
            scCred cred = mes;
            MySqlConnection sqlcon = getConnection();
            try
            {
                sqlcon.Open();
                string user, pass;
                string sql = "SELECT Username, Password FROM users WHERE Username = @Username";
                MySqlCommand cmd = new MySqlCommand(sql, sqlcon);
                cmd.Parameters.AddWithValue("@Username", cred.username);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user = reader["Username"].ToString();
                        pass = reader["Password"].ToString();
                        if ((user == cred.username) && (pass == serverTools.bytetostringexa(serverTools.stringtobyte(cred.passworld))))
                        {
                            temp = true;
                        }
                    }
                }
                sqlcon.Close();
                return temp;
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "doIdCorrespond");
                throw;
            }
        }

        public static List<scCharacter> GetUserInfo(message mes)
        {
            try
            {
                scCred cred = mes.GetScObject("cred").GetCred();
                using (var sqlcon = getConnection())
                {
                    sqlcon.Open();
                    int userId = GetUserIdByUsername(cred.username, sqlcon);
                    if (userId == 0)
                        return new List<scCharacter>();

                    return GetCharactersByUserId(userId, sqlcon);
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "GetUserInfo");
                throw;
            }
        }

        /// <summary>
        /// Récupère l'identifiant utilisateur à partir du nom d'utilisateur.
        /// </summary>
        private static int GetUserIdByUsername(string username, MySqlConnection sqlcon)
        {
            string sql = "SELECT UserID FROM users WHERE Username = @Username";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetInt32(0);
                }
            }
            return 0;
        }

        /// <summary>
        /// Récupère la liste des personnages pour un utilisateur donné.
        /// </summary>
        private static List<scCharacter> GetCharactersByUserId(int userId, MySqlConnection sqlcon)
        {
            var userChars = new List<scCharacter>();
            string sql = "SELECT * FROM chars WHERE CharUserID = @UserId";
            using (var cmd = new MySqlCommand(sql, sqlcon))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] strarray = new string[4];
                        int[] intarray = new int[20];
                        intarray[0] = (int)reader[0];
                        intarray[1] = (int)reader[1];
                        intarray[2] = (int)reader[2];
                        strarray[0] = reader.IsDBNull(3) ? "" : (string)reader[3];
                        strarray[1] = reader.IsDBNull(5) ? "" : (string)reader[5]; // Bio
                        strarray[2] = reader.IsDBNull(6) ? "" : (string)reader[6];
                        intarray[3] = (int)reader[7];
                        intarray[4] = (int)reader[8];
                        intarray[5] = (int)reader[9];
                        intarray[6] = (int)reader[10];
                        intarray[7] = (int)reader[11];
                        intarray[8] = (int)reader[12];
                        intarray[9] = (int)reader[13];
                        intarray[10] = (int)reader[14];
                        intarray[11] = (int)reader[15];
                        intarray[12] = (int)reader[16];
                        intarray[13] = (int)reader[17];
                        intarray[14] = (int)reader[18];
                        intarray[15] = (int)reader[19];
                        intarray[16] = (int)reader[20];
                        byte s = reader.GetByte(21);
                        intarray[17] = (int)s;
                        intarray[18] = (int)reader[22];
                        strarray[3] = reader.IsDBNull(23) ? "" : (string)reader[23];
                        intarray[19] = (int)reader[24];
                        scCharacter ch = new scCharacter(
                            intarray[0], intarray[1], intarray[2], strarray[0], DateTime.Now,
                            strarray[1], strarray[2], intarray[3], intarray[4], intarray[5], intarray[6], intarray[7],
                            intarray[8], intarray[9], intarray[10], intarray[11], intarray[12], intarray[13], intarray[14],
                            intarray[15], intarray[16], intarray[17], intarray[18], strarray[3], intarray[19]
                        );
                        userChars.Add(ch);
                    }
                }
            }
            return userChars;
        }

        public static List<scCharacter> GetUserInfoUid(int uid)
        {
            try
            {
                using (var sqlcon = getConnection())
                {
                    sqlcon.Open();
                    return GetCharactersByUserId(uid, sqlcon);
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "GetUserInfoUid");
                throw;
            }
        }

        public static bool isidfree(message mes)
        {
            bool temp = true;
            scCred cred = mes.GetScObject("cred").GetCred();
            MySqlConnection sqlcon = getConnection();
            try
            {
                sqlcon.Open();
                string user;
                string sql = "SELECT Username, Password FROM users WHERE Username = @Username";
                MySqlCommand cmd = new MySqlCommand(sql, sqlcon);
                cmd.Parameters.AddWithValue("@Username", cred.username);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user = reader["Username"].ToString();
                        if ((user == cred.username))
                        {
                            temp = false;
                        }
                    }
                }
                sqlcon.Close();
                return temp;
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "isidfree");
                throw;
            }
        }

        public static void register(message scmessage)
        {
            scCred cred = scmessage.GetScObject("cred").GetCred();
            MySqlConnection sqlcon = getConnection();
            try
            {
                sqlcon.Open();
                MySqlCommand cmd = sqlcon.CreateCommand();
                cmd.CommandText = "INSERT INTO users(Username,Password,Email) VALUES(?user,?pass,?Email)";
                cmd.Parameters.Add("?user", MySqlDbType.VarChar).Value = cred.username;
                cmd.Parameters.Add("?pass", MySqlDbType.VarChar).Value = serverTools.bytetostringexa(serverTools.stringtobyte(cred.passworld));
                cmd.Parameters.Add("?Email", MySqlDbType.VarChar).Value = cred.email;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "register");
                throw;
            }
        }

        public static List<string[]> retreiveBanData()
        {
            List<string[]> temp = new List<string[]>();
            MySqlConnection sqlCon = getConnection();
            try
            {
                var cmd = sqlCon.CreateCommand();
                cmd.CommandText = @"SELECT ipAdress, reason, date_issued,lenght FROM banlistip";
                sqlCon.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] arraytmp = new string[2];
                        arraytmp[0] = reader.GetString(0);
                        arraytmp[1] = reader.GetString(1);
                        temp.Add(arraytmp);
                        outputConsoleMain.ouToScreen("client : " + reader.GetString(0) + " bani pour cause : " + reader.GetString(1), false);
                    }
                }
                sqlCon.Close();
                outputConsoleMain.ouToScreen("données de ban récupérées", true);
                outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerBot(), true);
                return temp;
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "retreiveBanData");
                Console.ReadLine();
                return null;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        public static List<scMenuPanel> retreiveMenuInfo(string lang)
        {
            try
            {
                using (var sqlcon = getConnection())
                {
                    sqlcon.Open();
                    string sql = @"SELECT StackMenutype, StackObjectOrder, StackSubObjectOrder, StackObjectType, 
                                          StackObjectContent1, StackObjectContent2, StackObjectProperties, StackObjectImage, 
                                          StackObjectGradient1, StackObjectGradient2, StackObjectTitleColor, StackObjectContentColor 
                                   FROM launcherinfo 
                                   WHERE StackMenulang = @Lang 
                                   ORDER BY StackMenutype, StackObjectOrder, StackSubObjectOrder;";
                    using (var cmd = new MySqlCommand(sql, sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@Lang", lang);
                        using (var reader = cmd.ExecuteReader())
                        {
                            return ReadMenuPanels(reader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "retreiveMenuInfo");
                throw;
            }
        }

        /// <summary>
        /// Transforme un DataReader en liste de scMenuPanel.
        /// </summary>
        private static List<scMenuPanel> ReadMenuPanels(MySqlDataReader reader)
        {
            var panels = new List<scMenuPanel>();
            while (reader.Read())
            {
                int menuType = reader.GetInt32(0);
                int stackOrder = reader.GetInt32(1);
                int stackSubOrder = reader.GetInt32(2);
                string type = reader.IsDBNull(3) ? null : reader.GetString(3);
                string content1 = reader.IsDBNull(4) ? null : reader.GetString(4);
                string content2 = reader.IsDBNull(5) ? null : reader.GetString(5);
                string properties = reader.IsDBNull(6) ? null : reader.GetString(6);

                if (type != null && type.Contains("SCPanel"))
                {
                    string gradient1 = reader.IsDBNull(8) ? null : reader.GetString(8);
                    string gradient2 = reader.IsDBNull(9) ? null : reader.GetString(9);
                    string titleColor = reader.IsDBNull(10) ? null : reader.GetString(10);
                    string contentColor = reader.IsDBNull(11) ? null : reader.GetString(11);

                    panels.Add(new scMenuPanel(menuType, stackOrder, stackSubOrder, type, content1, content2, properties,
                                               gradient1, gradient2, titleColor, contentColor));
                }
                else
                {
                    panels.Add(new scMenuPanel(menuType, stackOrder, stackSubOrder, type, content1, content2, properties,
                                               "", "", "", ""));
                }
            }
            return panels;
        }

        public static void createBanIp(string[] ban)
        {
            using (MySqlConnection sqlCon = getConnection())
            {
                using (MySqlCommand querryCommand = new MySqlCommand())
                {
                    querryCommand.Connection = sqlCon;
                    querryCommand.CommandText = @"INSERT INTO banlistip (ipAdress,reason,date_issued,lenght) VALUES (@ipadress,@reason,@date_issued,@lenght)";
                    querryCommand.Parameters.Add("@ipAdress", MySqlDbType.String).Value = ban[0];
                    querryCommand.Parameters.Add("@reason", MySqlDbType.String).Value = ban[1];
                    querryCommand.Parameters.Add("@date_issued", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                    querryCommand.Parameters.Add("@lenght", MySqlDbType.Time).Value = new TimeSpan(20, 30, 00);
                    try
                    {
                        sqlCon.Open();
                        querryCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        outputConsoleMain.LogError(e, "createBanIp");
                    }
                    finally
                    {
                        sqlCon.Close();
                    }
                }
            }

        }


        public static void DelBan(string p)
        {
            using (MySqlConnection sqlCon = getConnection())
            {
                using (MySqlCommand querryCommand = new MySqlCommand())
                {
                    querryCommand.Connection = sqlCon;
                    querryCommand.CommandText = @"DELETE FROM banlistip WHERE ipAdress = @ipadress";
                    querryCommand.Parameters.Add("@ipAdress", MySqlDbType.String).Value = p;
                    try
                    {
                        sqlCon.Open();
                        querryCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        outputConsoleMain.LogError(e, "DelBan");
                    }
                    finally
                    {
                        sqlCon.Close();
                    }
                }
            }
        }

        private static void setCharLocation(scCharacter chara, scLocation loc)
        {
            MySqlConnection sqlcon = getConnection();
        }

        internal static scMap GetMapInfo(scLocation loc)
        {
            MySqlConnection sqlcon = getConnection();
            try
            {
            scMap map = new scMap();
            scTile tiletocreate;
            List<scTile> tiles = new List<scTile>();
                sqlcon.Open();
                MySqlDataReader reader;
                string sql = "SELECT mapSize, mapData" +
                             "FROM map, location_map " +
                             "WHERE locationMapLocationID = " + loc.locationSSID + " " +
                             "AND locationMapID = mapID ";
                MySqlCommand cmd = new MySqlCommand(sql, sqlcon);
                reader = cmd.ExecuteReader();
                //TODO: get map info
                if (reader.Read())
                {
                    map.mapSize = reader.GetInt32(0);
                    map.mapID = reader.GetInt32(1);
                    tiletocreate = new scTile(reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
                    tiles.Add(tiletocreate);
                    //outputConsoleMain.ouToScreen("Adding first tile found data : "+ tiletocreate.tilex + " " + tiletocreate.tiley + " " + tiletocreate.tiletype, true);
                    while (reader.Read())
                    {
                        tiletocreate = new scTile(reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
                        tiles.Add(tiletocreate);
                        //outputConsoleMain.ouToScreen("Adding more tiles data : " + tiletocreate.tilex + " " + tiletocreate.tiley + " " + tiletocreate.tiletype, true);
                    }
                    reader.Close();
                }
                else
                {
                    reader.Close();
                }
                map.mapTiles = tiles;

                return map;
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "GetMapInfo");
                throw;
            }
        }

        internal static int GetUserIdFromCredentials(scCred credentials)
        {
            //implementation pour récupérer l'ID utilisateur à partir des crédentials
            using (var sqlcon = getConnection())
            {
                sqlcon.Open();
                string sql = "SELECT UserID FROM users WHERE Username = @Username AND Password = @Password";
                using (var cmd = new MySqlCommand(sql, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@Username", credentials.username);
                    cmd.Parameters.AddWithValue("@Password", serverTools.bytetostringexa(serverTools.stringtobyte(credentials.passworld)));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0); // Retourne l'ID utilisateur
                        }
                        reader.Close();
                    }
                }
                return 0; // Aucun utilisateur trouvé
            }
        }
    }
    #endregion


}

