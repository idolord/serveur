-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';


-- -----------------------------------------------------
-- Table `ti`.`banlistip`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`banlistip` ;

CREATE TABLE IF NOT EXISTS `ti`.`banlistip` (
  `ipAdress` VARCHAR(45) NOT NULL,
  `reason` VARCHAR(45) NULL DEFAULT NULL,
  `date_issued` DATE NULL DEFAULT NULL,
  `lenght` TIME NULL DEFAULT NULL,
  PRIMARY KEY (`ipAdress`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ti`.`quadrant`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`quadrant` ;

CREATE TABLE IF NOT EXISTS `ti`.`quadrant` (
  `id_quadrant` INT NOT NULL AUTO_INCREMENT,
  `cardinal_quadrant` VARCHAR(3) NULL DEFAULT NULL,
  PRIMARY KEY (`id_quadrant`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE UNIQUE INDEX `id_quadrant_UNIQUE` ON `ti`.`quadrant` (`id_quadrant` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`secteur`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`secteur` ;

CREATE TABLE IF NOT EXISTS `ti`.`secteur` (
  `id_secteur` INT NOT NULL AUTO_INCREMENT,
  `nom_secteur` VARCHAR(45) NULL DEFAULT NULL,
  `x_loc_secteur` INT NULL DEFAULT NULL,
  `y_loc_secteur` INT NULL DEFAULT NULL,
  `z_loc_secteur` INT NULL DEFAULT NULL,
  `quadrant_secteur` INT NOT NULL,
  PRIMARY KEY (`id_secteur`),
  CONSTRAINT `fk_id_quadrant`
    FOREIGN KEY (`quadrant_secteur`)
    REFERENCES `ti`.`quadrant` (`id_quadrant`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `quad` ON `ti`.`secteur` (`quadrant_secteur` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`sub_secteur`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`sub_secteur` ;

CREATE TABLE IF NOT EXISTS `ti`.`sub_secteur` (
  `SSID` INT NOT NULL AUTO_INCREMENT,
  `SSName` VARCHAR(45) NULL DEFAULT NULL,
  `SSlocationX` INT NULL DEFAULT NULL,
  `SSlocationY` INT NULL DEFAULT NULL,
  `SSlocationZ` INT NULL DEFAULT NULL,
  `SSSID` INT NOT NULL,
  PRIMARY KEY (`SSID`),
  CONSTRAINT `fk_id_sector`
    FOREIGN KEY (`SSSID`)
    REFERENCES `ti`.`secteur` (`id_secteur`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_id_sector_idx` ON `ti`.`sub_secteur` (`SSSID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`location`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`location` ;

CREATE TABLE IF NOT EXISTS `ti`.`location` (
  `locationID` INT NOT NULL AUTO_INCREMENT,
  `locationIsOrbit` TINYINT NULL DEFAULT NULL,
  `locationX` INT NULL DEFAULT NULL,
  `locationY` INT NULL DEFAULT NULL,
  `locationZ` INT NULL DEFAULT NULL,
  `locationSSID` INT NULL DEFAULT NULL,
  `locationMapID` INT NULL DEFAULT NULL,
  PRIMARY KEY (`locationID`),
  CONSTRAINT `fk_location_sub_sector_ID`
    FOREIGN KEY (`locationSSID`)
    REFERENCES `ti`.`sub_secteur` (`SSID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
 CONSTRAINT `fk_location_map`
    FOREIGN KEY (`locationMapID`)
    REFERENCES `ti`.`map` (`mapID`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_sub_sector_id_idx` ON `ti`.`location` (`locationSSID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`celestial_type`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`celestial_type` ;

CREATE TABLE IF NOT EXISTS `ti`.`celestial_type` (
  `id_celestial_type` INT NOT NULL AUTO_INCREMENT,
  `lang_celestial_type` VARCHAR(45) NULL DEFAULT NULL,
  `name_celestial_type` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`id_celestial_type`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `lang` ON `ti`.`celestial_type` (`lang_celestial_type` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`celestial`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`celestial` ;

CREATE TABLE IF NOT EXISTS `ti`.`celestial` (
  `id_celestial` INT NOT NULL AUTO_INCREMENT,
  `name_celestial` VARCHAR(45) NULL DEFAULT NULL,
  `type_celestial` INT NOT NULL,
  `OwnerCorporationID` INT NULL,
  PRIMARY KEY (`id_celestial`),
  CONSTRAINT `fk_type_celestial`
    FOREIGN KEY (`type_celestial`)
    REFERENCES `ti`.`celestial_type` (`id_celestial_type`),
  CONSTRAINT `fk_celestial_owner_corporation`
    FOREIGN KEY (`OwnerCorporationID`)
    REFERENCES `ti`.`corporation` (`CorporationID`)
    ON DELETE SET NULL
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_type_celestial_idx` ON `ti`.`celestial` (`type_celestial` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`celestial_location_orbit`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`celestial_location_orbit` ;
-- Création de la table de liaison
CREATE TABLE IF NOT EXISTS `ti`.`celestial_location_orbit` (
  `celestial_id` INT NOT NULL,
  `location_id` INT NOT NULL,
  PRIMARY KEY (`celestial_id`, `location_id`),
  CONSTRAINT `fk_celestial`
    FOREIGN KEY (`celestial_id`) REFERENCES `ti`.`celestial` (`id_celestial`),
  CONSTRAINT `fk_location`
    FOREIGN KEY (`location_id`) REFERENCES `ti`.`location` (`locationID`)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `ti`.`race`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`race` ;

CREATE TABLE IF NOT EXISTS `ti`.`race` (
  `RaceID` INT NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`RaceID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

DROP TABLE IF EXISTS `ti`.`race_name`;

CREATE TABLE IF NOT EXISTS `ti`.`race_name` (
  `RaceID` INT NOT NULL,
  `Lang` VARCHAR(2) NOT NULL,
  `RaceName` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`RaceID`, `Lang`),
  CONSTRAINT `fk_race_name_race`
    FOREIGN KEY (`RaceID`) REFERENCES `ti`.`race` (`RaceID`)
    ON DELETE CASCADE
) ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `ti`.`users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`users` ;

CREATE TABLE IF NOT EXISTS `ti`.`users` (
  `UserID` INT NOT NULL AUTO_INCREMENT,
  `Username` VARCHAR(45) NOT NULL,
  `Email` VARCHAR(255) NOT NULL UNIQUE,
  `Password` VARCHAR(255) NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `last_login` DATETIME NULL,
  `failed_login_attempts` INT DEFAULT 0,
  `lockout_until` DATETIME NULL,
  PRIMARY KEY (`UserID`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8;

CREATE UNIQUE INDEX `id_UNIQUE` ON `ti`.`users` (`UserID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`chars`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`chars` ;

CREATE TABLE IF NOT EXISTS `ti`.`chars` (
  `CharID` INT NOT NULL AUTO_INCREMENT,
  `CharUserID` INT NOT NULL,
  `RaceID` INT NOT NULL,
  `CharName` VARCHAR(50) NULL DEFAULT ' ',
  `CharDateCreat` DATETIME NULL DEFAULT NULL,
  `CharBio` TEXT NULL DEFAULT NULL,
  `CharFavColor` VARCHAR(45) NULL DEFAULT ' ',
  `CharEyes` INT NULL DEFAULT '0',
  `CharHair` INT NULL DEFAULT '0',
  `CharFace` INT NULL DEFAULT '0',
  `CharBody` INT NULL DEFAULT '0',
  `CharFoot` INT NULL DEFAULT '0',
  `CharAge` INT NULL DEFAULT '0',
  `CharStrengh` INT NULL DEFAULT '5',
  `CharVitality` INT NULL DEFAULT '5',
  `CharDexterity` INT NULL DEFAULT '5',
  `CharKnowledge` INT NULL DEFAULT '5',
  `CharWisdom` INT NULL DEFAULT '5',
  `CharWittiness` INT NULL DEFAULT '5',
  `CharPerception` INT NULL DEFAULT '5',
  `CharLuck` INT NULL DEFAULT '5',
  `CharSex` TINYINT NULL DEFAULT '0',
  `CharLevel` INT NULL DEFAULT '0',
  `CharSkinCol` VARCHAR(45) NULL DEFAULT ' ',
  `CharExperience` INT NULL DEFAULT '0',
  `CharLocID` INT NULL DEFAULT NULL,
  `is_deleted` TINYINT(1) DEFAULT 0,
  `deleted_at` DATETIME NULL,
  `last_played_at` DATETIME NULL,
  `Status` ENUM('active', 'banned', 'pending', 'inactive') DEFAULT 'active',
  PRIMARY KEY (`CharID`),
  CONSTRAINT `fk_character_location`
    FOREIGN KEY (`CharLocID`)
    REFERENCES `ti`.`location` (`locationID`),
  CONSTRAINT `fk_character_Race`
    FOREIGN KEY (`RaceID`)
    REFERENCES `ti`.`race` (`RaceID`),
  CONSTRAINT `fk_character_user_account`
    FOREIGN KEY (`CharUserID`)
    REFERENCES `ti`.`users` (`UserID`))
ENGINE = InnoDB


DEFAULT CHARACTER SET = utf8;

CREATE UNIQUE INDEX `CharID_UNIQUE` ON `ti`.`chars` (`CharID` ASC) VISIBLE;

CREATE INDEX `fk_character_user_account_idx` ON `ti`.`chars` (`CharUserID` ASC) VISIBLE;

CREATE INDEX `fk_character_name_idx` ON `ti`.`chars` (`CharName` ASC) VISIBLE;

CREATE INDEX `fk_character_Race_idx` ON `ti`.`chars` (`RaceID` ASC) VISIBLE;

CREATE INDEX `fk_character_location_idx` ON `ti`.`chars` (`CharLocID` ASC) VISIBLE;


CREATE TABLE IF NOT EXISTS `ti`.`char_history` (
  `HistoryID` INT NOT NULL AUTO_INCREMENT,
  `CharID` INT NOT NULL,
  `Action` VARCHAR(32) NOT NULL, -- 'created', 'updated', 'deleted', etc.
  `ActionDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `DataSnapshot` TEXT, -- JSON ou XML de l’état du personnage
  PRIMARY KEY (`HistoryID`),
  FOREIGN KEY (`CharID`) REFERENCES `chars`(`CharID`)
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table `ti`.`map`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`map` ;

CREATE TABLE IF NOT EXISTS `ti`.`map` (
  `mapID` INT NOT NULL AUTO_INCREMENT,
  `mapSize` INT NULL DEFAULT '64',
  Type ENUM('terrestrial', 'orbital') NULL DEFAULT 'terrestrial',
  PRIMARY KEY (`mapID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE UNIQUE INDEX `mapID_UNIQUE` ON `ti`.`map` (`mapID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`location_map`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`location_map` ;

CREATE TABLE IF NOT EXISTS `ti`.`location_map` (
  `locationMapID` INT NOT NULL,
  `locationMapLocationID` INT NOT NULL,
  PRIMARY KEY (`locationMapID`, `locationMapLocationID`),
  CONSTRAINT `fk_location_location_ID`
    FOREIGN KEY (`locationMapLocationID`)
    REFERENCES `ti`.`location` (`locationID`),
  CONSTRAINT `fk_map_location_ID`
    FOREIGN KEY (`locationMapID`)
    REFERENCES `ti`.`map` (`mapID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_location_location_ID_idx` ON `ti`.`location_map` (`locationMapLocationID` ASC) INVISIBLE;

CREATE INDEX `fk_map_location_ID_idx` ON `ti`.`location_map` (`locationMapID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`map_ligne`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`map_ligne` ;

CREATE TABLE IF NOT EXISTS `ti`.`map_ligne` (
  `mapLigneID` INT NOT NULL,
  `mapLigneMapID` INT NOT NULL,
  PRIMARY KEY (`mapLigneID`, `mapLigneMapID`),
  CONSTRAINT `fk_map_ID`
    FOREIGN KEY (`mapLigneMapID`)
    REFERENCES `ti`.`map` (`mapID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_map_ID_idx` ON `ti`.`map_ligne` (`mapLigneMapID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`tile`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`tile` ;

CREATE TABLE IF NOT EXISTS `ti`.`tile` (
  `tileID` INT NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`tileID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ti`.`Ligne_tiles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`Ligne_tiles` ;

CREATE TABLE IF NOT EXISTS `ti`.`Ligne_tiles` (
  `LigneTilesIndexX` INT NOT NULL,
  `LigneTilesLigneID` INT NOT NULL,
  `LigneTilesTileID` INT NOT NULL,
  PRIMARY KEY (`LigneTilesIndexX`, `LigneTilesLigneID`),
  CONSTRAINT `fk_ligne_tile_tile_ID`
    FOREIGN KEY (`LigneTilesTileID`)
    REFERENCES `ti`.`tile` (`tileID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_map_ligne_ID`
    FOREIGN KEY (`LigneTilesLigneID`)
    REFERENCES `ti`.`map_ligne` (`mapLigneID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE INDEX `fk_ligne_tile_tile_ID_idx` ON `ti`.`Ligne_tiles` (`LigneTilesTileID` ASC) VISIBLE;

CREATE INDEX `fk_map_ligne_ID_idx` ON `ti`.`Ligne_tiles` (`LigneTilesLigneID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `ti`.`location_celestial`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`location_celestial` ;

CREATE TABLE IF NOT EXISTS `ti`.`location_celestial` (
  `locationCelestialID` INT NOT NULL,
  `locationCelestialLocation` INT NOT NULL,
  PRIMARY KEY (`locationcelestialLocation`, `locationcelestialID`),
  CONSTRAINT `fk_location_ID`
    FOREIGN KEY (`locationcelestialLocation`)
    REFERENCES `ti`.`location` (`locationID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_location_celestial_celestial`
    FOREIGN KEY (`locationCelestialID`)
    REFERENCES `ti`.`celestial` (`id_celestial`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE INDEX `fk_location_ID_idx` ON `ti`.`location_celestial` (`locationcelestialLocation` ASC) VISIBLE;


DROP TABLE IF EXISTS `ti`.`corporation` ;

CREATE TABLE IF NOT EXISTS `ti`.`corporation` (
  `CorporationID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `CreatorUserID` INT NOT NULL,
  PRIMARY KEY (`CorporationID`),
  FOREIGN KEY (`CreatorUserID`) REFERENCES `users`(`UserID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`char_corporation` ;
CREATE TABLE IF NOT EXISTS `ti`.`char_corporation` (
  `CharID` INT NOT NULL,
  `CorporationID` INT NOT NULL,
  `RoleID` INT NOT NULL,
  PRIMARY KEY (`CharID`, `CorporationID`),
  FOREIGN KEY (`CharID`) REFERENCES `chars`(`CharID`),
  FOREIGN KEY (`CorporationID`) REFERENCES `corporation`(`CorporationID`),
  FOREIGN KEY (`RoleID`) REFERENCES `corporation_role`(`RoleID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`corporation_inventory` ;
CREATE TABLE IF NOT EXISTS `ti`.`corporation_inventory` (
  `InventoryID` INT NOT NULL AUTO_INCREMENT,
  `CorporationID` INT NOT NULL,
  `Type` ENUM('item', 'worldobject') NOT NULL,
  `Name` VARCHAR(100),
  PRIMARY KEY (`InventoryID`),
  FOREIGN KEY (`CorporationID`) REFERENCES `corporation`(`CorporationID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`corporation_inventory_access` ;
CREATE TABLE IF NOT EXISTS `ti`.`corporation_inventory_access` (
  `InventoryID` INT NOT NULL,
  `RankMin` INT NOT NULL,
  PRIMARY KEY (`InventoryID`, `RankMin`),
  FOREIGN KEY (`InventoryID`) REFERENCES `corporation_inventory`(`InventoryID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`char_inventory` ;
CREATE TABLE IF NOT EXISTS `ti`.`char_inventory` (
  `InventoryID` INT NOT NULL AUTO_INCREMENT,
  `CharID` INT NOT NULL,
  `Type` ENUM('item', 'worldobject') NOT NULL,
  PRIMARY KEY (`InventoryID`),
  FOREIGN KEY (`CharID`) REFERENCES `chars`(`CharID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`item` ;
CREATE TABLE IF NOT EXISTS `ti`.`item` (
  `ItemID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `Type` ENUM('consumable', 'material', 'weapon', 'equipment', 'other') NOT NULL,
  PRIMARY KEY (`ItemID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`worldobject` ;
CREATE TABLE IF NOT EXISTS `ti`.`worldobject` (
  `WorldObjectID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `Type` ENUM('ship', 'station', 'drone', 'dyson', 'stargate', 'other') NOT NULL,
  `LocationID` INT,
  PRIMARY KEY (`WorldObjectID`),
  FOREIGN KEY (`LocationID`) REFERENCES `location`(`locationID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`inventory_item` ;
CREATE TABLE IF NOT EXISTS `ti`.`inventory_item` (
  `InventoryID` INT NOT NULL,
  `ItemID` INT NOT NULL,
  `Quantity` INT NOT NULL DEFAULT 1,
  PRIMARY KEY (`InventoryID`, `ItemID`),
  FOREIGN KEY (`InventoryID`) REFERENCES `char_inventory`(`InventoryID`),
  FOREIGN KEY (`ItemID`) REFERENCES `item`(`ItemID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`inventory_worldobject` ;
CREATE TABLE IF NOT EXISTS `ti`.`inventory_worldobject` (
  `InventoryID` INT NOT NULL,
  `WorldObjectID` INT NOT NULL,
  PRIMARY KEY (`InventoryID`, `WorldObjectID`),
  FOREIGN KEY (`InventoryID`) REFERENCES `char_inventory`(`InventoryID`),
  FOREIGN KEY (`WorldObjectID`) REFERENCES `worldobject`(`WorldObjectID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`worldobject_link` ;
CREATE TABLE IF NOT EXISTS `ti`.`worldobject_link` (
  `ParentWorldObjectID` INT NOT NULL,
  `ChildWorldObjectID` INT NOT NULL,
  PRIMARY KEY (`ParentWorldObjectID`, `ChildWorldObjectID`),
  FOREIGN KEY (`ParentWorldObjectID`) REFERENCES `worldobject`(`WorldObjectID`),
  FOREIGN KEY (`ChildWorldObjectID`) REFERENCES `worldobject`(`WorldObjectID`)
) ENGINE=InnoDB;

DROP TABLE IF EXISTS `ti`.`corporation_role` ;
CREATE TABLE IF NOT EXISTS `ti`.`corporation_role` (
  `RoleID` INT NOT NULL AUTO_INCREMENT,
  `CorporationID` INT NOT NULL,
  `RoleName` VARCHAR(50) NOT NULL,
  `RoleLevel` INT NOT NULL, -- 0 = fondateur, 1 = admin, 2 = officier, 3 = membre, etc.
  PRIMARY KEY (`RoleID`),
  FOREIGN KEY (`CorporationID`) REFERENCES `corporation`(`CorporationID`)
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table `ti`.`inventory`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`inventory` ;
CREATE TABLE IF NOT EXISTS `ti`.`inventory` (
  `InventoryID` INT NOT NULL AUTO_INCREMENT,
  `OwnerType` ENUM('char', 'corporation', 'worldobject') NOT NULL,
  `OwnerID` INT NOT NULL,
  `Name` VARCHAR(100),
  `Type` ENUM('item', 'worldobject') NOT NULL,
  PRIMARY KEY (`InventoryID`)
) ENGINE=InnoDB;

CREATE INDEX idx_inventory_owner ON `ti`.`inventory` (`OwnerID`);

-- -----------------------------------------------------
-- Table `ti`.`inventory_content`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ti`.`inventory_content` ;
CREATE TABLE IF NOT EXISTS `ti`.`inventory_content` (
  `InventoryID` INT NOT NULL,
  `ContentType` ENUM('item', 'worldobject') NOT NULL,
  `ContentID` INT NOT NULL,
  `Quantity` INT DEFAULT 1,
  PRIMARY KEY (`InventoryID`, `ContentType`, `ContentID`),
  FOREIGN KEY (`InventoryID`) REFERENCES `inventory`(`InventoryID`)
) ENGINE=InnoDB;

CREATE INDEX idx_inventory_content_type ON `ti`.`inventory_content` (`ContentType`, `ContentID`);

-- -----------------------------------------------------
-- Data for table `ti`.`race`
-- -----------------------------------------------------
INSERT INTO `ti`.`race` (`RaceID`) VALUES (1), (2), (3), (4);

INSERT INTO `ti`.`race_name` (`RaceID`, `Lang`, `RaceName`) VALUES
  (1, 'fr', 'Humain'),
  (1, 'en', 'Human'),
  (2, 'fr', 'shu'),
  (2, 'en', 'shu'),
  (3, 'fr', 'Valonser'),
  (3, 'en', 'Valonseer'),
  (4, 'fr', 'B-07'),
  (4, 'en', 'B-07');

COMMIT;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
