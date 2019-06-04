PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [Books] ( 
  [BookId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [Number] bigint NOT NULL 
, [Title] text NOT NULL 
, [Publisher] text NOT NULL 
, [Version] bigint NOT NULL 
, [Year] bigint NOT NULL 
, [Medium] text NOT NULL 
, [Place] text NOT NULL 
, [DayBought] text NOT NULL 
, [Pages] bigint NOT NULL 
, [Price] numeric(53,0) NOT NULL 
);
CREATE TABLE [Authors] ( 
  [AuthorId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [Name] text NOT NULL 
);
CREATE TABLE [Books_Signatures] ( 
  [Id] INTEGER NOT NULL 
, [BookId] INTEGER NOT NULL 
, [SignatureId] INTEGER NOT NULL 
, [Priority] INTEGER NOT NULL 
, CONSTRAINT [PK_Books_Signatures] PRIMARY KEY ([Id]) 
, CONSTRAINT [FK_Books_Signatures_0_0] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE ON UPDATE CASCADE 
, CONSTRAINT [FK_Books_Signatures_1_0] FOREIGN KEY ([SignatureId]) REFERENCES [Signatures] ([SignatureId]) ON DELETE CASCADE ON UPDATE CASCADE 
);
CREATE TABLE [Books_Authors] ( 
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [BookId] bigint NOT NULL 
, [AuthorId] bigint NOT NULL 
, [Priority] bigint NOT NULL 
, CONSTRAINT [FK_Books_Authors_0_0] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE ON UPDATE CASCADE 
, CONSTRAINT [FK_Books_Authors_1_0] FOREIGN KEY ([AuthorId]) REFERENCES [Authors] ([AuthorId]) ON DELETE CASCADE ON UPDATE CASCADE 
);
CREATE TABLE [Mediums] ( 
  [MediumId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [Name] text NOT NULL 
);
INSERT INTO Mediums VALUES(1,'Buch');
INSERT INTO Mediums VALUES(2,'CD');
INSERT INTO Mediums VALUES(3,'DVD');
INSERT INTO Mediums VALUES(4,'Lehrbuch');
INSERT INTO Mediums VALUES(5,'Lernkarten');
INSERT INTO Mediums VALUES(6,'Lexika');
INSERT INTO Mediums VALUES(7,'Roman');
INSERT INTO Mediums VALUES(8,'Sachbuch');
INSERT INTO Mediums VALUES(9,'Zeitschrift');
INSERT INTO Mediums VALUES(10,'N/A');
CREATE TABLE [Places] ( 
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [City] text NOT NULL 
, [State] text NULL 
, [Country] text NULL 
);
INSERT INTO Places VALUES(1,'Frankfurt',NULL,NULL);
INSERT INTO Places VALUES(2,'Bad_Kissingen1',NULL,NULL);
INSERT INTO Places VALUES(3,'Bad_Kissingen2',NULL,NULL);
INSERT INTO Places VALUES(4,'Bad_Kissingen3',NULL,NULL);
INSERT INTO Places VALUES(5,'Gießen',NULL,NULL);
INSERT INTO Places VALUES(6,'N/A',NULL,NULL);
CREATE TABLE [SubSignatures] ( 
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [ParentId] bigint NOT NULL 
, [Signature] text NOT NULL 
, [Info] text NOT NULL 
, [Sort] text NOT NULL 
);
CREATE TABLE [Signatures] ( 
  [SignatureId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [ParentId] bigint NULL 
, [Signature] text NOT NULL 
, [Info] text NOT NULL 
, [Sort] text NULL 
);
INSERT INTO Signatures VALUES(1,NULL,'AGRA','Agrarwissenschaften',NULL);
INSERT INTO Signatures VALUES(2,NULL,'ALLG','Allgemeines; Lexika; Nachschlagewerke;',NULL);
INSERT INTO Signatures VALUES(3,NULL,'ALTE','Altertumswissenschaft; Archäologie',NULL);
INSERT INTO Signatures VALUES(4,NULL,'ARCH','Architektur',NULL);
INSERT INTO Signatures VALUES(5,NULL,'ASTR','Astronomie',NULL);
INSERT INTO Signatures VALUES(6,NULL,'BETR','Betriebswirtschaft',NULL);
INSERT INTO Signatures VALUES(7,NULL,'BIOL','Biologie',NULL);
INSERT INTO Signatures VALUES(8,NULL,'CHEM','Chemie',NULL);
INSERT INTO Signatures VALUES(9,NULL,'ERNÄ','Ernährung',NULL);
INSERT INTO Signatures VALUES(10,NULL,'EROT','Erotika',NULL);
INSERT INTO Signatures VALUES(11,NULL,'ESOT','Esoterik',NULL);
INSERT INTO Signatures VALUES(12,NULL,'FRSP','Fremdsprachen',NULL);
INSERT INTO Signatures VALUES(13,NULL,'GEOL','Geologie',NULL);
INSERT INTO Signatures VALUES(14,NULL,'GEOG','Geografie',NULL);
INSERT INTO Signatures VALUES(15,NULL,'GESC','Geschichte',NULL);
INSERT INTO Signatures VALUES(16,NULL,'GESU','Gesundheitswesen',NULL);
INSERT INTO Signatures VALUES(17,NULL,'HOBB','Freizeit; Hobby',NULL);
INSERT INTO Signatures VALUES(18,NULL,'INFO','Informatik',NULL);
INSERT INTO Signatures VALUES(19,NULL,'INGE','Ingenieurwissenschaften',NULL);
INSERT INTO Signatures VALUES(20,NULL,'JAGD','Jagd',NULL);
INSERT INTO Signatures VALUES(21,NULL,'KIND','Kinderbücher',NULL);
INSERT INTO Signatures VALUES(22,NULL,'KUNS','Kunst',NULL);
INSERT INTO Signatures VALUES(23,NULL,'LITE','Literatur',NULL);
INSERT INTO Signatures VALUES(24,NULL,'LITF','Fremdsprachige Literatur',NULL);
INSERT INTO Signatures VALUES(25,NULL,'LITW','Literaturwissenschaft',NULL);
INSERT INTO Signatures VALUES(26,NULL,'LUFT','Luftfahrt',NULL);
INSERT INTO Signatures VALUES(27,NULL,'MATH','Mathematik',NULL);
INSERT INTO Signatures VALUES(28,NULL,'MEDI','Medizin',NULL);
INSERT INTO Signatures VALUES(29,NULL,'MILI','Militär',NULL);
INSERT INTO Signatures VALUES(30,NULL,'MUSI','Musik',NULL);
INSERT INTO Signatures VALUES(31,NULL,'PÄDA','Pädagogik',NULL);
INSERT INTO Signatures VALUES(32,NULL,'PHAR','Pharmazie',NULL);
INSERT INTO Signatures VALUES(33,NULL,'PHIL','Philosophie',NULL);
INSERT INTO Signatures VALUES(34,NULL,'PHYS','Physik',NULL);
INSERT INTO Signatures VALUES(35,NULL,'POLI','Politik',NULL);
INSERT INTO Signatures VALUES(36,NULL,'PSYC','Psychologie',NULL);
INSERT INTO Signatures VALUES(37,NULL,'RATG','Ratgeber',NULL);
INSERT INTO Signatures VALUES(38,NULL,'RECH','Rechtswissenschaft',NULL);
INSERT INTO Signatures VALUES(39,NULL,'REIS','Reisen; Reiseführer',NULL);
INSERT INTO Signatures VALUES(40,NULL,'RELI','Religionen',NULL);
INSERT INTO Signatures VALUES(41,NULL,'SOZI','Sozialwissenschaften',NULL);
INSERT INTO Signatures VALUES(42,NULL,'SPOR','Sport',NULL);
INSERT INTO Signatures VALUES(43,NULL,'SPRA','Sprachwissenschaft / Linguistik',NULL);
INSERT INTO Signatures VALUES(44,NULL,'STEU','Steuern',NULL);
INSERT INTO Signatures VALUES(45,NULL,'TECH','Technik',NULL);
INSERT INTO Signatures VALUES(46,NULL,'THEA','Theaterwissenschaften',NULL);
INSERT INTO Signatures VALUES(47,NULL,'TIER','Tiere',NULL);
INSERT INTO Signatures VALUES(48,NULL,'VETE','Tiermedizin',NULL);
INSERT INTO Signatures VALUES(49,NULL,'VOLK','Volkswirtschaft',NULL);
INSERT INTO Signatures VALUES(50,NULL,'WIRT','Wirtschaft (Allgemein; Ohne BWL und VWL)',NULL);
INSERT INTO Signatures VALUES(51,NULL,'CDXX','CD',NULL);
INSERT INTO Signatures VALUES(52,NULL,'DVDX','DVD',NULL);
INSERT INTO Signatures VALUES(53,1,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(54,1,'BIOL','Biologische Landwirtschaft','A');
INSERT INTO Signatures VALUES(55,1,'LAND','Landwirtschaft','A');
INSERT INTO Signatures VALUES(56,1,'OBSB','Obstbau/Obstbaumschnitt','A');
INSERT INTO Signatures VALUES(57,1,'VIEH','Viehwirtschaft','A');
INSERT INTO Signatures VALUES(58,2,'NACH','Nachschlagewerke','A');
INSERT INTO Signatures VALUES(59,2,'LEXI','Lexika','A');
INSERT INTO Signatures VALUES(60,3,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(61,3,'ACHÄ','Archäologie','A');
INSERT INTO Signatures VALUES(62,4,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(63,4,'ARCH','Architekten','A');
INSERT INTO Signatures VALUES(64,4,'ARCE','Architektur-Entwicklung','A');
INSERT INTO Signatures VALUES(65,4,'BILD','Bildbände','A');
INSERT INTO Signatures VALUES(66,4,'ENTW','Entwurfslehre','A');
INSERT INTO Signatures VALUES(67,4,'GART','Gartenarchitektur / Gartengestaltung','A');
INSERT INTO Signatures VALUES(68,4,'HAUS','Haustechnik','A');
INSERT INTO Signatures VALUES(69,4,'INDU','Industriebau','A');
INSERT INTO Signatures VALUES(70,4,'INNE','Innenarchitektur','A');
INSERT INTO Signatures VALUES(71,4,'SAKR','Sakralbau','A');
INSERT INTO Signatures VALUES(72,4,'SCHL','Schlösser','A');
INSERT INTO Signatures VALUES(73,4,'UMBA','Umbau; Renovierung','A');
INSERT INTO Signatures VALUES(74,4,'VERK','Verkehrsbau; Flughäfen; Bahnhöfe;','A');
INSERT INTO Signatures VALUES(75,4,'WOHN','Wohnbau','A');
INSERT INTO Signatures VALUES(76,5,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(77,6,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(78,6,'BANK','Bankbetriebslehre','A');
INSERT INTO Signatures VALUES(79,6,'BILA','Bilanzen','A');
INSERT INTO Signatures VALUES(80,6,'BUCH','Buchführung','A');
INSERT INTO Signatures VALUES(81,6,'CHAN','Change Management','A');
INSERT INTO Signatures VALUES(82,6,'ENTS','Entscheidungslehre','A');
INSERT INTO Signatures VALUES(83,6,'FALL','Fallstudien','A');
INSERT INTO Signatures VALUES(84,6,'FINA','Finanzwirtschaft','A');
INSERT INTO Signatures VALUES(85,6,'HAND','Handelsbetriebslehre','A');
INSERT INTO Signatures VALUES(86,6,'INDU','Industriebetriebslehre','A');
INSERT INTO Signatures VALUES(87,6,'INVE','Investition','A');
INSERT INTO Signatures VALUES(88,6,'INNO','Innovation','A');
INSERT INTO Signatures VALUES(89,6,'KORE','Kostenrechnung','A');
INSERT INTO Signatures VALUES(90,6,'LOGI','Logistik','A');
INSERT INTO Signatures VALUES(91,6,'MARK','Marketing','A');
INSERT INTO Signatures VALUES(92,6,'MATE','Materialwirtschaft','A');
INSERT INTO Signatures VALUES(93,6,'OPER','Operative Planung','A');
INSERT INTO Signatures VALUES(94,6,'PERS','Personalwirtschaft','A');
INSERT INTO Signatures VALUES(95,6,'PROD','Produktionswirtschaft','A');
INSERT INTO Signatures VALUES(96,6,'QUAL','Qualitätsmanagement','A');
INSERT INTO Signatures VALUES(97,6,'RECH','Rechnungslegung/Bilanzen','A');
INSERT INTO Signatures VALUES(98,6,'RISI','Risikomanagement','A');
INSERT INTO Signatures VALUES(99,6,'STRA','Strategische Führung','A');
INSERT INTO Signatures VALUES(100,6,'UNTE','Unternehmensführung','A');
INSERT INTO Signatures VALUES(101,7,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(102,7,'BOTA','Botanik','A');
INSERT INTO Signatures VALUES(103,7,'ZOOL','Zoologie','A');
INSERT INTO Signatures VALUES(104,8,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(105,8,'ANOR','Anorganische Chemie','A');
INSERT INTO Signatures VALUES(106,8,'BIOC','Biochemie','A');
INSERT INTO Signatures VALUES(107,8,'EXPE','Experimentelle Chemie','A');
INSERT INTO Signatures VALUES(108,8,'ORGA','Organische Chemie','A');
INSERT INTO Signatures VALUES(109,9,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(110,9,'BIER','Bier','A');
INSERT INTO Signatures VALUES(111,9,'DIÄT','Diäten','A');
INSERT INTO Signatures VALUES(112,9,'ERNÄ','Ernährungslehre','A');
INSERT INTO Signatures VALUES(113,9,'FETT','Fett','A');
INSERT INTO Signatures VALUES(114,9,'GEMÜ','Gemüse/Gemüsesorten','A');
INSERT INTO Signatures VALUES(115,9,'GETR','Getränke','A');
INSERT INTO Signatures VALUES(116,9,'GRILL','Grillen','A');
INSERT INTO Signatures VALUES(117,9,'KOCH','Kochbücher','A');
INSERT INTO Signatures VALUES(118,9,'OBST','Obst','A');
INSERT INTO Signatures VALUES(119,9,'PILZ','Pilze','A');
INSERT INTO Signatures VALUES(120,9,'TABE','Tabellen','A');
INSERT INTO Signatures VALUES(121,9,'WEIN','Wein','A');
INSERT INTO Signatures VALUES(122,9,'WHIS','Whisky','A');
INSERT INTO Signatures VALUES(123,10,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(124,10,'BILD','Bildbände','A');
INSERT INTO Signatures VALUES(125,10,'LITE','Erotische Literatur','A');
INSERT INTO Signatures VALUES(126,11,'ASTR','Astrologie','A');
INSERT INTO Signatures VALUES(127,11,'ESOT','Esoterik','A');
INSERT INTO Signatures VALUES(128,11,'HEXE','Hexen','A');
INSERT INTO Signatures VALUES(129,11,'HORO','Horoskope','A');
INSERT INTO Signatures VALUES(130,11,'MEDI','Meditation','A');
INSERT INTO Signatures VALUES(131,11,'MYTH','Mythologie','A');
INSERT INTO Signatures VALUES(132,11,'SCHA','Schamanimus','A');
INSERT INTO Signatures VALUES(133,11,'STER','Sternzeichen','A');
INSERT INTO Signatures VALUES(134,12,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(135,12,'AFRI','Afrikaans','A');
INSERT INTO Signatures VALUES(136,12,'AENG','Amerikanisches Englisch','A');
INSERT INTO Signatures VALUES(137,12,'BULG','Bulgarisch','A');
INSERT INTO Signatures VALUES(138,12,'CHIN','Chinesisch','A');
INSERT INTO Signatures VALUES(139,12,'DANI','Dänisch','A');
INSERT INTO Signatures VALUES(140,12,'ENGL','Englisch','A');
INSERT INTO Signatures VALUES(141,12,'FINN','Finnisch','A');
INSERT INTO Signatures VALUES(142,12,'FRAN','Französisch','A');
INSERT INTO Signatures VALUES(143,12,'GRIE','Griechisch','A');
INSERT INTO Signatures VALUES(144,12,'HÄBR','Hebräisch','A');
INSERT INTO Signatures VALUES(145,12,'INDI','Indisch','A');
INSERT INTO Signatures VALUES(146,12,'ITAL','Italienisch','A');
INSERT INTO Signatures VALUES(147,12,'JAPA','Japanisch','A');
INSERT INTO Signatures VALUES(148,12,'NORW','Norwegisch','A');
INSERT INTO Signatures VALUES(149,12,'POLN','Polnisch','A');
INSERT INTO Signatures VALUES(150,12,'PORT','Portugiesisch','A');
INSERT INTO Signatures VALUES(151,12,'RUSS','Russisch','A');
INSERT INTO Signatures VALUES(152,12,'SCHW','Schwedisch','A');
INSERT INTO Signatures VALUES(153,12,'SPAN','Spanisch','A');
INSERT INTO Signatures VALUES(154,12,'TÜRK','Türkisch','A');
INSERT INTO Signatures VALUES(155,12,'TSCH','Tschechisch','A');
INSERT INTO Signatures VALUES(156,12,'THAI','Thailändisch','A');
INSERT INTO Signatures VALUES(157,12,'UNGA','Ungarisch','A');
INSERT INTO Signatures VALUES(158,12,'VIET','Vietnamesisch','A');
INSERT INTO Signatures VALUES(159,13,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(160,13,'BILD','Bildbände','A');
INSERT INTO Signatures VALUES(161,14,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(162,14,'HÖHL','Höhlen','A');
INSERT INTO Signatures VALUES(163,14,'MINE','Mineralien','A');
INSERT INTO Signatures VALUES(164,15,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(165,15,'ANTI','Antike','A');
INSERT INTO Signatures VALUES(166,15,'AUFK','Aufklärung (Epoche)','A');
INSERT INTO Signatures VALUES(167,15,'BARO','Barock','A');
INSERT INTO Signatures VALUES(168,15,'ERST','Erster Weltkrieg','A');
INSERT INTO Signatures VALUES(169,15,'EURO','Europäische Geschichte','A');
INSERT INTO Signatures VALUES(170,15,'FRÜH','Ur- und Frühgeschichte','A');
INSERT INTO Signatures VALUES(171,15,'KAIS','Kaiserreich','A');
INSERT INTO Signatures VALUES(172,15,'KOLO','Kolonialgeschichte','A');
INSERT INTO Signatures VALUES(173,15,'KULT','Kulturgeschichte','A');
INSERT INTO Signatures VALUES(174,15,'KRIE','Kriege','A');
INSERT INTO Signatures VALUES(175,15,'LUFT','Luftfahrtgeschichte: s. LUFT','A');
INSERT INTO Signatures VALUES(176,15,'MILI','Militärgeschichte: s. MILI','A');
INSERT INTO Signatures VALUES(177,15,'MITT','Mittelalter','A');
INSERT INTO Signatures VALUES(178,15,'NATI','Nationalsozialismus','A');
INSERT INTO Signatures VALUES(179,15,'RENA','Renaissance','A');
INSERT INTO Signatures VALUES(180,15,'NEUE','Neue Geschichte ab 1945','A');
INSERT INTO Signatures VALUES(181,15,'STAD','Stadtgeschichte','A');
INSERT INTO Signatures VALUES(182,15,'SEEF','Geschichte Seefahrt','A');
INSERT INTO Signatures VALUES(183,15,'WELT','Weltgeschichte','A');
INSERT INTO Signatures VALUES(184,15,'WEIM','Weimarer Republik','A');
INSERT INTO Signatures VALUES(185,15,'ZWEI','Zweiter Weltkrieg','A');
INSERT INTO Signatures VALUES(186,16,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(187,16,'AKKU','Akupunktur','A');
INSERT INTO Signatures VALUES(188,16,'ARZT','Arztpraxis','A');
INSERT INTO Signatures VALUES(189,16,'GESU','Gesundheit, Allg.','A');
INSERT INTO Signatures VALUES(190,16,'JOGA','Joga','A');
INSERT INTO Signatures VALUES(191,16,'KRAN','Krankenhaus','A');
INSERT INTO Signatures VALUES(192,16,'KÖRP','Körper; Evolution','A');
INSERT INTO Signatures VALUES(193,16,'PHYS','Physiotherapie; Pilates','A');
INSERT INTO Signatures VALUES(194,16,'QUAL','Qualitätsmanagement','A');
INSERT INTO Signatures VALUES(195,16,'RÜCK','Rücken, Wirbelsäule','A');
INSERT INTO Signatures VALUES(196,16,'WIRT','Gesundheitswirtschaft','A');
INSERT INTO Signatures VALUES(197,17,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(198,17,'ANGE','Angeln','A');
INSERT INTO Signatures VALUES(199,17,'AQUA','Aquaristik','A');
INSERT INTO Signatures VALUES(200,17,'EISE','Eisenbahn','A');
INSERT INTO Signatures VALUES(201,17,'FOTO','Fotografieren','A');
INSERT INTO Signatures VALUES(202,17,'HAND','Handarbeiten; Basteln','A');
INSERT INTO Signatures VALUES(203,17,'MALE','Malen; Zeichnen','A');
INSERT INTO Signatures VALUES(204,17,'MODL','Modellbau','A');
INSERT INTO Signatures VALUES(205,17,'PHIL','Philatelie','A');
INSERT INTO Signatures VALUES(206,17,'RÄTS','Rätsel; Quizbücher','A');
INSERT INTO Signatures VALUES(207,17,'SAMM','Sammeln','A');
INSERT INTO Signatures VALUES(208,17,'TERR','Terrarien','A');
INSERT INTO Signatures VALUES(209,18,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(210,18,'PROG','Programmiersprachen','A');
INSERT INTO Signatures VALUES(211,19,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(212,19,'ELEK','Elektrotechnik','A');
INSERT INTO Signatures VALUES(213,19,'KRAF','Kraftfahrzeugtechnik','A');
INSERT INTO Signatures VALUES(214,19,'MASC','Maschinenbau','A');
INSERT INTO Signatures VALUES(215,19,'NACH','Nachrichtentechnik','A');
INSERT INTO Signatures VALUES(216,19,'SERV','Service-Engineering','A');
INSERT INTO Signatures VALUES(217,19,'VERF','Verfahrenstechnik','A');
INSERT INTO Signatures VALUES(218,19,'VERK','Verkehrstechnik','A');
INSERT INTO Signatures VALUES(219,20,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(220,20,'AUSB','Ausbildung/Jägerprüfung','A');
INSERT INTO Signatures VALUES(221,20,'JAGH','Jagdhunde','A');
INSERT INTO Signatures VALUES(222,20,'REVI','Revier','A');
INSERT INTO Signatures VALUES(223,20,'WAFF','Jagdwaffenkunde','A');
INSERT INTO Signatures VALUES(224,21,'BILD','Bilderbücher','A');
INSERT INTO Signatures VALUES(225,22,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(226,22,'AUSS','Ausstellungskataloge','A');
INSERT INTO Signatures VALUES(227,22,'DESI','Design','A');
INSERT INTO Signatures VALUES(228,22,'FOTO','Fotographen','A');
INSERT INTO Signatures VALUES(229,22,'KUEP','Epochen der Kunst','A');
INSERT INTO Signatures VALUES(230,22,'TEXD','Textildesign','A');
INSERT INTO Signatures VALUES(231,23,'BELL','Belletristik, Neue Literatur, Fiction','A');
INSERT INTO Signatures VALUES(232,23,'BIOG','Biographien','A');
INSERT INTO Signatures VALUES(233,23,'DRAM','Drama','A');
INSERT INTO Signatures VALUES(234,23,'DZWK','Die Zeit Reihe Wissenschaftskrimi','A');
INSERT INTO Signatures VALUES(235,23,'DZWR','Die Zeit Reihe Wissenschaftsromane','A');
INSERT INTO Signatures VALUES(236,23,'GRIE','Griechische Literatur','A');
INSERT INTO Signatures VALUES(237,23,'INTE','Interpretationen/Erläuterungen/Materialien','A');
INSERT INTO Signatures VALUES(238,23,'KMAY','Karl May','A');
INSERT INTO Signatures VALUES(239,23,'KLAS','Klassiker der Weltliteratur','A');
INSERT INTO Signatures VALUES(240,23,'KRIM','Kriminalromane/Thriller/Psychothriller','A');
INSERT INTO Signatures VALUES(241,23,'LYRI','Lyrik','A');
INSERT INTO Signatures VALUES(242,23,'MITT','Literatur Mittelalter','A');
INSERT INTO Signatures VALUES(243,23,'REISE','Reiseerzählungen','A');
INSERT INTO Signatures VALUES(244,23,'REIH','Reihen','A');
INSERT INTO Signatures VALUES(245,23,'RÖMI','Römische Literatur','A');
INSERT INTO Signatures VALUES(246,23,'SCIE','Science Fiction','A');
INSERT INTO Signatures VALUES(247,23,'SZBI','Süddeutsche Zeitung Bibliothek','A');
INSERT INTO Signatures VALUES(248,23,'SZME','Süddeutsche Zeitung Metropolen','A');
INSERT INTO Signatures VALUES(249,23,'THEA','Theaterstücke','A');
INSERT INTO Signatures VALUES(250,24,'ENGL','Englische Literatur','A');
INSERT INTO Signatures VALUES(251,24,'FRAN','Französische Literatur','A');
INSERT INTO Signatures VALUES(252,24,'ITAL','Italienische Literatur','A');
INSERT INTO Signatures VALUES(253,24,'SPAN','Spanische Literatur','A');
INSERT INTO Signatures VALUES(254,25,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(255,26,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(256,26,'GESC','Luftfahrtgeschichte','A');
INSERT INTO Signatures VALUES(257,27,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(258,27,'ALGE','Algebra','A');
INSERT INTO Signatures VALUES(259,27,'DIFF','Differentialrechnung','A');
INSERT INTO Signatures VALUES(260,27,'INTE','Integralrechnung','A');
INSERT INTO Signatures VALUES(261,27,'STAT','Statistik','A');
INSERT INTO Signatures VALUES(262,28,'ALLX','Allgemeines','A');
INSERT INTO Signatures VALUES(263,28,'GESC','Medizin Historisches/Geschichte','A');
INSERT INTO Signatures VALUES(264,28,'QUER','Querschnittbereiche','A');
INSERT INTO Signatures VALUES(265,28,'STUD','Studium Medizin','A');
INSERT INTO Signatures VALUES(266,28,'ANAT','Anatomie/Embryologie','A');
INSERT INTO Signatures VALUES(267,28,'BIOC','Biochemie für Mediziner','A');
INSERT INTO Signatures VALUES(268,28,'BIOL','Biologie für Mediziner','A');
INSERT INTO Signatures VALUES(269,28,'CHEM','Chemie für Mediziner','A');
INSERT INTO Signatures VALUES(270,28,'HIST','Histologie','A');
INSERT INTO Signatures VALUES(271,28,'MEDS','Medizinische Statistik','A');
INSERT INTO Signatures VALUES(272,28,'PHYK','Physik für Mediziner','A');
INSERT INTO Signatures VALUES(273,28,'PHYS','Physiologie','A');
INSERT INTO Signatures VALUES(274,28,'TERM','Medizinische Terminologie','A');
INSERT INTO Signatures VALUES(275,28,'ALLE','Allergologie','B');
INSERT INTO Signatures VALUES(276,28,'ALLG','Allgemeinmedizin','B');
INSERT INTO Signatures VALUES(277,28,'ANAM','Anamnese','B');
INSERT INTO Signatures VALUES(278,28,'ANÄS','Anästhesie','B');
INSERT INTO Signatures VALUES(279,28,'ANGI','Angiologie','B');
INSERT INTO Signatures VALUES(280,28,'ARBE','Arbeitsmedizin','B');
INSERT INTO Signatures VALUES(281,28,'AUGE','Augenheilkunde','B');
INSERT INTO Signatures VALUES(282,28,'CHIN','Chinesische Medizin','B');
INSERT INTO Signatures VALUES(283,28,'CHIR','Chirurgie','B');
INSERT INTO Signatures VALUES(284,28,'CTMR','CT/MRT','B');
INSERT INTO Signatures VALUES(285,28,'DERM','Dermatologie','B');
INSERT INTO Signatures VALUES(286,28,'EPID','Epidemiologie','B');
INSERT INTO Signatures VALUES(287,28,'FRAU','Frauenheilkunde','B');
INSERT INTO Signatures VALUES(288,28,'GAST','Gastroentereologie','B');
INSERT INTO Signatures VALUES(289,28,'GYNÄ','Gynäkologie','B');
INSERT INTO Signatures VALUES(290,28,'HÄMA','Hämatologie','B');
INSERT INTO Signatures VALUES(291,28,'HNOK','Hals-/Nasen-/Ohren','B');
INSERT INTO Signatures VALUES(292,28,'HOMÖ','Homöopathie','B');
INSERT INTO Signatures VALUES(293,28,'HUMA','Humangenetik','B');
INSERT INTO Signatures VALUES(294,28,'HYGI','Hygiene/Virologie','B');
INSERT INTO Signatures VALUES(295,28,'IMMU','Infektiologie / Immunologie','B');
INSERT INTO Signatures VALUES(296,28,'INNE','Innere Medizin','B');
INSERT INTO Signatures VALUES(297,28,'INTE','Intensivmedizin','B');
INSERT INTO Signatures VALUES(298,28,'KARD','Kardiologie','B');
INSERT INTO Signatures VALUES(299,28,'KIND','Kinderheilkunde','B');
INSERT INTO Signatures VALUES(300,28,'LABO','Labormedizin/Klinische Chemie','B');
INSERT INTO Signatures VALUES(301,28,'MIKR','Medizinische Mikrobiologie','B');
INSERT INTO Signatures VALUES(302,28,'MINF','Medizinische Informatik','B');
INSERT INTO Signatures VALUES(303,28,'NEPH','Nephrologie','B');
INSERT INTO Signatures VALUES(304,28,'NERV','Nervenheilkunde','B');
INSERT INTO Signatures VALUES(305,28,'NEUR','Neurologie','B');
INSERT INTO Signatures VALUES(306,28,'NOTF','Notfallmedizin','B');
INSERT INTO Signatures VALUES(307,28,'ONKO','Onkologie','B');
INSERT INTO Signatures VALUES(308,28,'ORTH','Orthopädie','B');
INSERT INTO Signatures VALUES(309,28,'PATH','Pathologie','B');
INSERT INTO Signatures VALUES(310,28,'PHAR','Pharmakologie/Arzneimittel','B');
INSERT INTO Signatures VALUES(311,28,'PHYT','Phytologie','B');
INSERT INTO Signatures VALUES(312,28,'PNEU','Pneumologie','B');
INSERT INTO Signatures VALUES(313,28,'PRÄV','Präventivmedizin','B');
INSERT INTO Signatures VALUES(314,28,'PSYC','Medizinische Psychologie/Soziologie/Psychosomatische Medizin','B');
INSERT INTO Signatures VALUES(315,28,'RADI','Radiologie, Nuklearmedizin','B');
INSERT INTO Signatures VALUES(316,28,'RECH','Rechtsmedizin','B');
INSERT INTO Signatures VALUES(317,28,'RHEU','Rheumatologie','B');
INSERT INTO Signatures VALUES(318,28,'SONO','Sonographie','B');
INSERT INTO Signatures VALUES(319,28,'SOZI','Sozialmedizin','B');
INSERT INTO Signatures VALUES(320,28,'SPOR','Sportmedin','B');
INSERT INTO Signatures VALUES(321,28,'THER','Medizinische Therapie','B');
INSERT INTO Signatures VALUES(322,28,'TOXI','Toxikologie','B');
INSERT INTO Signatures VALUES(323,28,'TROP','Tropenmedizin','B');
INSERT INTO Signatures VALUES(324,28,'UMWE','Umweltmedizin','B');
INSERT INTO Signatures VALUES(325,28,'UROL','Urologie','B');
INSERT INTO Signatures VALUES(326,29,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(327,29,'GESC','Militärgeschichte','A');
INSERT INTO Signatures VALUES(328,29,'HEER','Heer','A');
INSERT INTO Signatures VALUES(329,29,'LUFT','Luftwaffe','A');
INSERT INTO Signatures VALUES(330,29,'MARI','Marine','A');
INSERT INTO Signatures VALUES(331,29,'WAFF','Waffen; Waffensysteme','A');
INSERT INTO Signatures VALUES(332,30,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(333,30,'BIOG','Biographien (Leben und Werk)','A');
INSERT INTO Signatures VALUES(334,30,'INST','Instrumentenkunde','A');
INSERT INTO Signatures VALUES(335,31,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(336,31,'DIDA','Didaktik','A');
INSERT INTO Signatures VALUES(337,31,'ERWA','Erwachsenenbildung','A');
INSERT INTO Signatures VALUES(338,31,'ERZI','Erziehungswissenschaft','A');
INSERT INTO Signatures VALUES(339,31,'GRUND','Grundschulpädagogik','A');
INSERT INTO Signatures VALUES(340,31,'SCHU','Schulspädagogik','A');
INSERT INTO Signatures VALUES(341,32,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(342,33,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(343,33,'ANTI','Antike Philosophen','A');
INSERT INTO Signatures VALUES(344,33,'BIOG','Biographien (Leben und Werk)','A');
INSERT INTO Signatures VALUES(345,33,'GRUN','Grundlagen','A');
INSERT INTO Signatures VALUES(346,33,'GESC','Geschichtsphilosophie','A');
INSERT INTO Signatures VALUES(347,33,'POLI','Politische Philosophie','A');
INSERT INTO Signatures VALUES(348,33,'RECH','Rechtsphilosophie','A');
INSERT INTO Signatures VALUES(349,33,'SOZI','Sozilogische Philosophie','A');
INSERT INTO Signatures VALUES(350,33,'SPRA','Sprachphilosophie','A');
INSERT INTO Signatures VALUES(351,34,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(352,34,'ASTR','Astrophysik','A');
INSERT INTO Signatures VALUES(353,34,'ELEK','Elektrizität','A');
INSERT INTO Signatures VALUES(354,34,'MECH','Mechanik','A');
INSERT INTO Signatures VALUES(355,34,'QUAN','Quantenphysik','A');
INSERT INTO Signatures VALUES(356,34,'THER','Thermodynamik','A');
INSERT INTO Signatures VALUES(357,35,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(358,35,'AUSS','Außenpolitik','A');
INSERT INTO Signatures VALUES(359,35,'BILP','Bildungspolitik','A');
INSERT INTO Signatures VALUES(360,35,'ENER','Energiepolitik','A');
INSERT INTO Signatures VALUES(361,35,'ENTW','Entwicklungshilfe','A');
INSERT INTO Signatures VALUES(362,35,'ERNÄ','Welternährung','A');
INSERT INTO Signatures VALUES(363,35,'EUPO','Europäische Union','A');
INSERT INTO Signatures VALUES(364,35,'FÖDE','Föderalismus','A');
INSERT INTO Signatures VALUES(365,35,'FRAU','Frauenpolitik','A');
INSERT INTO Signatures VALUES(366,35,'FREI','Freimaurer','A');
INSERT INTO Signatures VALUES(367,35,'FRIE','Friedenspolitik/Friedensforschung','A');
INSERT INTO Signatures VALUES(368,35,'GEOP','Geopolitik/Weltpolitik','A');
INSERT INTO Signatures VALUES(369,35,'GESU','Gesundheitspolitik','A');
INSERT INTO Signatures VALUES(370,35,'GLOB','Globalisierung','A');
INSERT INTO Signatures VALUES(371,35,'INNE','Innenpolitik','A');
INSERT INTO Signatures VALUES(372,35,'MENS','Menschenrechte','A');
INSERT INTO Signatures VALUES(373,35,'MONA','Monarchien','A');
INSERT INTO Signatures VALUES(374,35,'NGOS','Nicht Regierungsorganisationen','A');
INSERT INTO Signatures VALUES(375,35,'PARL','Parlamentarismus','A');
INSERT INTO Signatures VALUES(376,35,'PART','Parteien','A');
INSERT INTO Signatures VALUES(377,35,'POLI','Politikwissenschaft','A');
INSERT INTO Signatures VALUES(378,35,'POLK','Politische Karikaturen','A');
INSERT INTO Signatures VALUES(379,35,'SOZI','Sozialpolitik','A');
INSERT INTO Signatures VALUES(380,35,'THEO','Politische Theorien (Anarchismus/Konservatismus/Liberalismus/Sozialismus/Kommunismus)','A');
INSERT INTO Signatures VALUES(381,35,'TERR','Terrorismus','A');
INSERT INTO Signatures VALUES(382,35,'UMWE','Umweltpolitik','A');
INSERT INTO Signatures VALUES(383,35,'VERT','Verteidigungspolitik/Krieg','A');
INSERT INTO Signatures VALUES(384,35,'WAHL','Wahlen','A');
INSERT INTO Signatures VALUES(385,35,'WIDE','Widerstand','A');
INSERT INTO Signatures VALUES(386,36,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(387,36,'TEST','Psychologische Testverfahren','A');
INSERT INTO Signatures VALUES(388,37,'ALTE','Alter','A');
INSERT INTO Signatures VALUES(389,37,'BABY','Baby; Kleinkind','A');
INSERT INTO Signatures VALUES(390,37,'BENE','Benehmen','A');
INSERT INTO Signatures VALUES(391,37,'BERU','Beruf','A');
INSERT INTO Signatures VALUES(392,37,'BEWE','Bewerbung','A');
INSERT INTO Signatures VALUES(393,37,'FAMI','Familie','A');
INSERT INTO Signatures VALUES(394,37,'KARR','Karriere','A');
INSERT INTO Signatures VALUES(395,37,'PART','Partnerschaft','A');
INSERT INTO Signatures VALUES(396,37,'PSYC','Psyche; Lebenshilfe','A');
INSERT INTO Signatures VALUES(397,37,'SCHR','Schreiben','A');
INSERT INTO Signatures VALUES(398,37,'SONS','Sonstige Ratgeber','A');
INSERT INTO Signatures VALUES(399,38,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(400,38,'STUD','Studium Rechtswissenschaft','A');
INSERT INTO Signatures VALUES(401,38,'ARBR','Arbeitsrecht','A');
INSERT INTO Signatures VALUES(402,38,'ERBR','Erbrecht','A');
INSERT INTO Signatures VALUES(403,38,'EURO','Europarecht','A');
INSERT INTO Signatures VALUES(404,38,'FAMI','Familienrecht','A');
INSERT INTO Signatures VALUES(405,38,'GESR','Gesellschaftsrecht','A');
INSERT INTO Signatures VALUES(406,38,'HAND','Handelsrecht','A');
INSERT INTO Signatures VALUES(407,38,'ÖFFR','Öffentliches Recht/Staatsrecht','A');
INSERT INTO Signatures VALUES(408,38,'SEER','Seerecht','A');
INSERT INTO Signatures VALUES(409,38,'SOZI','Sozialrecht','A');
INSERT INTO Signatures VALUES(410,38,'STRA','Strafrecht','A');
INSERT INTO Signatures VALUES(411,38,'VERK','Verkehrsrecht','A');
INSERT INTO Signatures VALUES(412,38,'VERW','Verwaltungsrecht','A');
INSERT INTO Signatures VALUES(413,38,'WERT','Wertpapierrecht','A');
INSERT INTO Signatures VALUES(414,38,'ZIVI','Zivilrecht; BGB','A');
INSERT INTO Signatures VALUES(415,39,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(416,39,'ATLA','Atlanten','A');
INSERT INTO Signatures VALUES(417,39,'HOTE','Hotelführer','A');
INSERT INTO Signatures VALUES(418,39,'KART','Landkarten','A');
INSERT INTO Signatures VALUES(419,39,'LAND','Land und Reisen','A');
INSERT INTO Signatures VALUES(420,39,'REIS','Reiseführer','A');
INSERT INTO Signatures VALUES(421,39,'REIZ','Reiseerzählungen','A');
INSERT INTO Signatures VALUES(422,39,'REST','Restaurantführer','A');
INSERT INTO Signatures VALUES(423,39,'STAD','Stadtführer','A');
INSERT INTO Signatures VALUES(424,40,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(425,40,'BIBL','Bibelausgaben/Bibelkommentare','A');
INSERT INTO Signatures VALUES(426,40,'BRAU','Brauchtum','A');
INSERT INTO Signatures VALUES(427,40,'BUDD','Buddhismus','A');
INSERT INTO Signatures VALUES(428,40,'CHRI','Christentum','A');
INSERT INTO Signatures VALUES(429,40,'GEBE','Gebetbücher','A');
INSERT INTO Signatures VALUES(430,40,'HEIL','Heilige','A');
INSERT INTO Signatures VALUES(431,40,'HIND','Hinduismus','A');
INSERT INTO Signatures VALUES(432,40,'ISLA','Islamismus','A');
INSERT INTO Signatures VALUES(433,40,'JESU','Jesus','A');
INSERT INTO Signatures VALUES(434,40,'JUDE','Judentum','A');
INSERT INTO Signatures VALUES(435,40,'KATH','Katholische Theologie','A');
INSERT INTO Signatures VALUES(436,40,'KIRC','Kirchengeschichte','A');
INSERT INTO Signatures VALUES(437,40,'KLOS','Klöster; Abteien;','A');
INSERT INTO Signatures VALUES(438,40,'KRIT','Religionskritik','A');
INSERT INTO Signatures VALUES(439,40,'LITU','Liturgie','A');
INSERT INTO Signatures VALUES(440,40,'MÖNC','Mönchtum','A');
INSERT INTO Signatures VALUES(441,40,'ORDE','Orden','A');
INSERT INTO Signatures VALUES(442,40,'PRED','Predigten','A');
INSERT INTO Signatures VALUES(443,40,'RECH','Kirchenrecht','A');
INSERT INTO Signatures VALUES(444,40,'SEKT','Sekten','A');
INSERT INTO Signatures VALUES(445,40,'TAOI','Taoismus','A');
INSERT INTO Signatures VALUES(446,41,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(447,41,'GESE','Gesellschaft','A');
INSERT INTO Signatures VALUES(448,41,'SOTH','Klassische Theorien','A');
INSERT INTO Signatures VALUES(449,41,'SOZW','Sozialer Wandel','A');
INSERT INTO Signatures VALUES(450,42,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(451,42,'BALL','Ballett','A');
INSERT INTO Signatures VALUES(452,42,'BASK','Basketball','A');
INSERT INTO Signatures VALUES(453,42,'FUSS','Fußball','A');
INSERT INTO Signatures VALUES(454,42,'GOLF','Golf','A');
INSERT INTO Signatures VALUES(455,42,'LAUF','Laufen/Walking/Marathon','A');
INSERT INTO Signatures VALUES(456,42,'REIT','Reiten','A');
INSERT INTO Signatures VALUES(457,42,'SCHI','Schießen','A');
INSERT INTO Signatures VALUES(458,42,'SCHW','Schwimmen','A');
INSERT INTO Signatures VALUES(459,42,'TANZ','Tanzsport','A');
INSERT INTO Signatures VALUES(460,42,'TENN','Tennis','A');
INSERT INTO Signatures VALUES(461,42,'WAND','Wandern','A');
INSERT INTO Signatures VALUES(462,43,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(463,43,'HISL','Historische Linguistik','A');
INSERT INTO Signatures VALUES(464,43,'PSYL','Psycholinguistik','A');
INSERT INTO Signatures VALUES(465,43,'VERL','Vergleichende Linguistik','A');
INSERT INTO Signatures VALUES(466,44,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(467,44,'HIST','Steuern Historisches','A');
INSERT INTO Signatures VALUES(468,44,'QUER','Steuern Querschnittbereiche','A');
INSERT INTO Signatures VALUES(469,44,'RATB','Steuern Berufsgruppen','A');
INSERT INTO Signatures VALUES(470,44,'RATG','Steuern Ratgeber','A');
INSERT INTO Signatures VALUES(471,44,'REGR','Reihen: Grüne Reihe','A');
INSERT INTO Signatures VALUES(472,44,'REBL','Reihen: Blaue Reihe','A');
INSERT INTO Signatures VALUES(473,44,'REST','Reihen: Steuer Seminar','A');
INSERT INTO Signatures VALUES(474,44,'AOXX','Abgabenordnung','A');
INSERT INTO Signatures VALUES(475,44,'GESE','Besteuerung von Gesellschaften','A');
INSERT INTO Signatures VALUES(476,44,'BEWE','Bewertungsrecht','A');
INSERT INTO Signatures VALUES(477,44,'BILA','Bilanzsteuerrecht','A');
INSERT INTO Signatures VALUES(478,44,'BUCH','Buchführung','A');
INSERT INTO Signatures VALUES(479,44,'EINK','Einkommensteuer','A');
INSERT INTO Signatures VALUES(480,44,'ERBS','Schenkungsteuer/Erbschaftsteuer','A');
INSERT INTO Signatures VALUES(481,44,'FGOR','Finanzgerichtsordnung','A');
INSERT INTO Signatures VALUES(482,44,'GEWE','Gewerbesteuer','A');
INSERT INTO Signatures VALUES(483,44,'GRUN','Grunderwerbsteuer','A');
INSERT INTO Signatures VALUES(484,44,'INTE','Internationales Steuerrecht','A');
INSERT INTO Signatures VALUES(485,44,'KÖRP','Körperschaftsteuer','A');
INSERT INTO Signatures VALUES(486,44,'LOHN','Lohnsteuer','A');
INSERT INTO Signatures VALUES(487,44,'LSTT','Lohnsteuertabellen','A');
INSERT INTO Signatures VALUES(488,44,'SSTR','Steuerstrafrecht','A');
INSERT INTO Signatures VALUES(489,44,'USTX','Umsatzsteuer','A');
INSERT INTO Signatures VALUES(490,44,'UMWA','Umwandlungsteuerrecht','A');
INSERT INTO Signatures VALUES(491,44,'VERK','Verkehrssteuern','A');
INSERT INTO Signatures VALUES(492,45,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(493,46,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(494,47,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(495,47,'FISC','Fische','A');
INSERT INTO Signatures VALUES(496,47,'HUND','Hunde','A');
INSERT INTO Signatures VALUES(497,47,'INSE','Insekten','A');
INSERT INTO Signatures VALUES(498,47,'KATZ','Katzen','A');
INSERT INTO Signatures VALUES(499,48,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(500,48,'ANAT','Tieranatomie','A');
INSERT INTO Signatures VALUES(501,49,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(502,49,'AUSS','Außenwirtschaft','A');
INSERT INTO Signatures VALUES(503,49,'FINA','Finanzwissenschaft','A');
INSERT INTO Signatures VALUES(504,49,'GELD','Geldtheorie','A');
INSERT INTO Signatures VALUES(505,49,'GLOB','Globalisierung','A');
INSERT INTO Signatures VALUES(506,49,'KONJ','Konjunkturtheorie','A');
INSERT INTO Signatures VALUES(507,49,'MAKR','Makroökonomie','A');
INSERT INTO Signatures VALUES(508,49,'MIKR','Mikroökonomie','A');
INSERT INTO Signatures VALUES(509,50,'ALLG','Allgemeines','A');
INSERT INTO Signatures VALUES(510,50,'FINA','Finanzanlagen','A');
INSERT INTO Signatures VALUES(511,50,'GESC','Wirtschaftsgeschichte','A');
INSERT INTO Signatures VALUES(512,50,'KRIM','Wirtschaftskriminalität','A');
INSERT INTO Signatures VALUES(513,51,'COWE','Country & Western','A');
INSERT INTO Signatures VALUES(514,51,'JAZZ','Jazz','A');
INSERT INTO Signatures VALUES(515,51,'KIRC','Kirchenmusik; Glockenklänge','A');
INSERT INTO Signatures VALUES(516,51,'KLAS','Klassik','A');
INSERT INTO Signatures VALUES(517,51,'LAND','Landestypische  Musik','A');
INSERT INTO Signatures VALUES(518,51,'POPX','Pop','A');
INSERT INTO Signatures VALUES(519,51,'WEIH','Weihnachtsklänge','A');
INSERT INTO Signatures VALUES(520,52,'DOKU','Dokumentarfilme','A');
INSERT INTO Signatures VALUES(521,52,'KOMÖ','Komödien','A');
INSERT INTO Signatures VALUES(522,NULL,'N/A','Not Available',NULL);
CREATE TABLE [Publishers] ( 
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [Name] text NOT NULL 
, [City] text NOT NULL 
, [Country] text NULL 
);
INSERT INTO Publishers VALUES(1,'Thieme Verlag','Stuttgart','unbekannt');
INSERT INTO Publishers VALUES(2,'Urban & Fischer','München/Jena',NULL);
INSERT INTO Publishers VALUES(3,'Elsevier','München/Jena',NULL);
INSERT INTO Publishers VALUES(4,'Lonely Planet','Victoria',NULL);
INSERT INTO Publishers VALUES(5,'R. Oldenbourg','München/Wien',NULL);
INSERT INTO Publishers VALUES(6,'Reise Know-How Verlag Peter Rump','Bielefeld',NULL);
INSERT INTO Publishers VALUES(7,'Arrow Books','London',NULL);
INSERT INTO Publishers VALUES(8,'Weltbild','Augsburg',NULL);
INSERT INTO Publishers VALUES(9,'Verlag Moderne Industrie','Landsberg am Lech',NULL);
INSERT INTO Publishers VALUES(10,'Verlag Neue Wirtschafts Briefe','Herne',NULL);
INSERT INTO Publishers VALUES(11,'Ballantine Books','New York',NULL);
INSERT INTO Publishers VALUES(12,'Heyne Verlag','München',NULL);
INSERT INTO Publishers VALUES(13,'Time Warner Book','New York, Boston',NULL);
INSERT INTO Publishers VALUES(14,'Philipp Reclam Verlag','Stuttgart',NULL);
INSERT INTO Publishers VALUES(15,'C. H. Beck Verlag','München',NULL);
INSERT INTO Publishers VALUES(16,'Hanser Verlag','München, Wien',NULL);
INSERT INTO Publishers VALUES(17,'Taschen Verlag','Köln',NULL);
INSERT INTO Publishers VALUES(18,'Penguin Books','New York',NULL);
INSERT INTO Publishers VALUES(19,'Knaur Verlag','München',NULL);
INSERT INTO Publishers VALUES(20,'Springer Verlag','Berlin/Heidelberg',NULL);
INSERT INTO Publishers VALUES(21,'Piper Verlag','München/Zürich',NULL);
INSERT INTO Publishers VALUES(22,'Verlag Süddeutsche Zeitung','München',NULL);
INSERT INTO Publishers VALUES(23,'DuMont Buchverlag','Köln',NULL);
INSERT INTO Publishers VALUES(24,'ADAC Verlag','München',NULL);
INSERT INTO Publishers VALUES(25,'Ravensburgrer Buchverlag','Ravensburg',NULL);
INSERT INTO Publishers VALUES(26,'Ernst Klett Verlag','Stuttgart',NULL);
INSERT INTO Publishers VALUES(27,'Blanvalet','München',NULL);
INSERT INTO Publishers VALUES(28,'Goldmann Verlag','München',NULL);
INSERT INTO Publishers VALUES(29,'Fachbuchverlag Leipzig (Hanser)','München/Wien',NULL);
INSERT INTO Publishers VALUES(30,'Erich Fleischer Verlag','Achim',NULL);
INSERT INTO Publishers VALUES(31,'Spektrum Akademischer Verlag','Heidelberg/Berlin',NULL);
INSERT INTO Publishers VALUES(32,'Lehmanns Media Verlag','Berlin',NULL);
INSERT INTO Publishers VALUES(33,'Verlag Wissenschaft und Praxis','Sternefels',NULL);
INSERT INTO Publishers VALUES(34,'Bantam Dell Books','New York',NULL);
INSERT INTO Publishers VALUES(35,'Rowohlt','Reinbek',NULL);
INSERT INTO Publishers VALUES(36,'Zweitausendeins','Frankfurt',NULL);
INSERT INTO Publishers VALUES(37,'Suhrkamp Verlag','Frankfurt',NULL);
INSERT INTO Publishers VALUES(38,'Schattauer','Stuttgart/New York',NULL);
INSERT INTO Publishers VALUES(39,'Diogenes Verlag','Zürich',NULL);
INSERT INTO Publishers VALUES(40,'Ewald von Kleist Verlag','Berlin',NULL);
INSERT INTO Publishers VALUES(41,'Lübbe Verlag','Bergisch Gladbach',NULL);
INSERT INTO Publishers VALUES(42,'List Verlag','Berlin',NULL);
INSERT INTO Publishers VALUES(43,'Erich Schmidt Verlag','Berlin',NULL);
INSERT INTO Publishers VALUES(44,'Kiehl Verlag','Ludwigshafen',NULL);
INSERT INTO Publishers VALUES(45,'Börm Bruckmeier Verlag','Grünwald',NULL);
INSERT INTO Publishers VALUES(46,'Wiley-VCH-Verlag','Weinheim',NULL);
INSERT INTO Publishers VALUES(47,'Vintage Books','New York',NULL);
INSERT INTO Publishers VALUES(48,'Compact Verlag','München',NULL);
INSERT INTO Publishers VALUES(49,'Nicol Verlag','Hamburg',NULL);
INSERT INTO Publishers VALUES(50,'Wissen Media Verlag','Gütersloh',NULL);
INSERT INTO Publishers VALUES(51,'Ecomed Verlag','Landsberg',NULL);
INSERT INTO Publishers VALUES(52,'Verlagsgesellschaft Stumpf+Kossendey','Edewecht',NULL);
INSERT INTO Publishers VALUES(53,'Medi-Learn Verlag','Marburg',NULL);
INSERT INTO Publishers VALUES(54,'Carl Heymanns Verlag','Köln',NULL);
INSERT INTO Publishers VALUES(55,'McGraw Hill','New York/Chicago',NULL);
INSERT INTO Publishers VALUES(56,'Krause und Pachernegg','Gablitz',NULL);
INSERT INTO Publishers VALUES(57,'Fischer Taschenbuch Verlag','Frankfurt',NULL);
INSERT INTO Publishers VALUES(58,'Motorbuch Verlag','Stuttgart',NULL);
INSERT INTO Publishers VALUES(59,'Callwey Verlag','München',NULL);
INSERT INTO Publishers VALUES(60,'Parragon Books','Bath','UK');
INSERT INTO Publishers VALUES(61,'Pantheon Verlag','München',NULL);
INSERT INTO Publishers VALUES(62,'UNI-MED-Verlag','Bremen',NULL);
INSERT INTO Publishers VALUES(63,'Grove Press','New York',NULL);
INSERT INTO Publishers VALUES(64,'Gerstenberg Verlag','Hildesheim',NULL);
INSERT INTO Publishers VALUES(65,'Campus Verlag','Frankfurt',NULL);
INSERT INTO Publishers VALUES(66,'Deutsche Verlags-Anstalt','München',NULL);
INSERT INTO Publishers VALUES(67,'Kiepenheuer & Witsch','Köln',NULL);
INSERT INTO Publishers VALUES(68,'Facultas Verlag','Wien',NULL);
INSERT INTO Publishers VALUES(69,'Hallwag Verlag','München',NULL);
INSERT INTO Publishers VALUES(70,'Editions Terrail','Paris',NULL);
INSERT INTO Publishers VALUES(71,'Diamed Verlag','Düsseldorf',NULL);
INSERT INTO Publishers VALUES(72,'Hippokrates Verlag','Stuttgart',NULL);
INSERT INTO Publishers VALUES(73,'Bund Verlag','Frankfurt',NULL);
INSERT INTO Publishers VALUES(74,'Alpmann&Schmidt','Münster',NULL);
INSERT INTO Publishers VALUES(75,'Carl Heymanns','Köln',NULL);
INSERT INTO Publishers VALUES(76,'Haus Publishing','London',NULL);
INSERT INTO Publishers VALUES(77,'BLV-Verlagsanstalt','München',NULL);
INSERT INTO Publishers VALUES(78,'Aufbau Verlag','Berlin',NULL);
INSERT INTO Publishers VALUES(79,'N/A','N/A','N/A');
CREATE TABLE [ListViewColVisible] ( 
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL 
, [ColName] text NOT NULL 
, [Boolean] bigint NOT NULL 
);
INSERT INTO ListViewColVisible VALUES(1,'BookId',1);
INSERT INTO ListViewColVisible VALUES(2,'Number',1);
INSERT INTO ListViewColVisible VALUES(3,'Signature',1);
INSERT INTO ListViewColVisible VALUES(4,'Title',1);
INSERT INTO ListViewColVisible VALUES(5,'Authors',1);
INSERT INTO ListViewColVisible VALUES(6,'Publisher',1);
INSERT INTO ListViewColVisible VALUES(7,'Year',1);
INSERT INTO ListViewColVisible VALUES(8,'Version',1);
INSERT INTO ListViewColVisible VALUES(9,'Medium',1);
INSERT INTO ListViewColVisible VALUES(10,'Place',1);
INSERT INTO ListViewColVisible VALUES(11,'Date',1);
INSERT INTO ListViewColVisible VALUES(12,'Pages',1);
INSERT INTO ListViewColVisible VALUES(13,'Price',1);
DELETE FROM sqlite_sequence;
INSERT INTO sqlite_sequence VALUES('Places',6);
INSERT INTO sqlite_sequence VALUES('Mediums',10);
INSERT INTO sqlite_sequence VALUES('SubSignatures',1);
INSERT INTO sqlite_sequence VALUES('Signatures',522);
INSERT INTO sqlite_sequence VALUES('Publishers',79);
INSERT INTO sqlite_sequence VALUES('ListViewColVisible',13);
INSERT INTO sqlite_sequence VALUES('Books',1);
INSERT INTO sqlite_sequence VALUES('Authors',1);
INSERT INTO sqlite_sequence VALUES('Books_Authors',1);
COMMIT;
