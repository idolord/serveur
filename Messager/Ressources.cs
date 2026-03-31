using MessagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Messager
{
    public class MenuPanel
    {
        public string Langue { get; set; }
        public string Title { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }

        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }

        public int panelNum { get; set; }

        public int FontSize { get; set; }

        public Color FontColor { get; set; }

        public Thickness margin { get; set; }

        public Color gradient1Color1 { get; set; }
        public Color gradient1Color2 { get; set; }
        public Color title1FontColor { get; set; }
        public Color content1FontColor { get; set; }

        public Color gradient2Color1 { get; set; }
        public Color gradient2Color2 { get; set; }
        public Color title2FontColor { get; set; }
        public Color content2FontColor { get; set; }

        public Color gradient3Color1 { get; set; }
        public Color gradient3Color2 { get; set; }
        public Color title3FontColor { get; set; }
        public Color content3FontColor { get; set; }
    }

    public class CharDefinition
    {
        public bool isCreateMode { get; set; }
        public bool isLevelMode { get; set; }
        public int CharRace { get; set; }
        public string CharName { get; set; }
        public string CharDateCreation { get; set; }
        public string CharBio { get; set; }
        public Color CharFavColor { get; set; }
        public int CharEyes { get; set; }
        public int CharHair { get; set; }
        public int CharFace { get; set; }
        public int CharBody { get; set; }
        public int CharFoot { get; set; }
        public int CharAge { get; set; }
        public Color CharSkinColor { get; set; }
        public int CharStrengh { get; set; }
        public int CharVitality { get; set; }
        public int CharDexterity { get; set; }
        public int CharKnowledge { get;set; }
        public int CharWisdom { get; set;}
        public int CharWittiness { get; set; }
        public int CharPerception { get; set; }
        public int CharLuck { get; set;}
        public int CharSex { get; set;}
        public int CharLevel { get; set; }
        public long CharExp { get; set; }
        public long CharExpNextLevel { get; set; }
        
        public CharDefinition ()
        {
            isCreateMode = true;
            isLevelMode = true;
            CharRace = 1;
            CharName = "";
            CharDateCreation = "";
            CharBio = "";
            CharFavColor = Colors.Black;
            CharEyes = 0;
            CharHair = 0;
            CharFace = 0;
            CharBody = 0;
            CharFoot = 0;
            CharAge = 20;
            CharSkinColor = Colors.Black;
            CharStrengh = 0;
            CharVitality = 0;
            CharDexterity = 0;
            CharKnowledge = 0;
            CharWisdom = 0;
            CharWittiness = 0;
            CharPerception = 0;
            CharLuck = 0;
            CharSex = 0;
            CharLevel = 0;
            CharExp = 0;
            CharExpNextLevel = 100;
        }
    }

    

}
