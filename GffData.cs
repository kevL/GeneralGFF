using System;
using System.Collections.Generic;


namespace generalgff
{
	#region Enums (global)
	/// <summary>
	/// The types of fields available.
	/// </summary>
	enum FieldType : byte
	{
		BYTE,			//  0
		CHAR,			//  1
		WORD,			//  2
		SHORT,			//  3
		DWORD,			//  4
		INT,			//  5
		DWORD64,		//  6
		INT64,			//  7
		FLOAT,			//  8
		DOUBLE,			//  9
		CExoString,		// 10
		CResRef,		// 11
		CExoLocString,	// 12
		VOID,			// 13
		Struct,			// 14
		List,			// 15

		locale = Byte.MaxValue
	}

	/// <summary>
	/// The types of languages available.
	/// @note The values will be used as integers as well.
	/// </summary>
	enum Language : uint
	{
		English            =   0,
		French             =   1,
		German             =   2,
		Italian            =   3,
		Spanish            =   4,
		Polish             =   5,
		Russian            =   6, // <- nwn2 add (not in the nwn1 doc)
		Korean             = 128,
		ChineseTraditional = 129,
		ChineseSimplified  = 130,
		Japanese           = 131,

		// TlkEdit2 defines another Locale: "GffToken".
		// It can appear (output by the NwN2 executable itself) as either the
		//   "LocalizedName" (eg. "Battleaxe +2")
		// or
		//   "LastName" (eg. "DoneOnce7=1")
		// within an item's Struct under either
		//   "ItemList"
		// or
		//   "Equip_ItemList"
		// .
		GffToken = 4294967294 // 0xFFFFFFFE
	}

	/// <summary>
	/// The types of gff-files available.
	/// @note Maintain its congruity with FileDialogFilter etc.
	/// </summary>
	enum GffType : byte
	{
		generic,	//  0
		ARE,		//  1
		BIC,		//  2
		CAM,		//  3
		DLG,		//  4
		FAC,		//  5
		GIC,		//  6
		GIT,		//  7
		IFO,		//  8
		JRL,		//  9
		PFB,		// 10
		ROS,		// 11
		RST,		// 12
		ULT,		// 13
		UPE,		// 14
		UTC,		// 15
		UTD,		// 16
		UTE,		// 17
		UTI,		// 18
		UTM,		// 19
		UTP,		// 20
		UTR,		// 21
		UTS,		// 22
		UTT,		// 23
		UTW,		// 24
		WMP			// 25
	}
	#endregion Enums (global)


	/// <summary>
	/// Data-structure of a Struct.
	/// </summary>
	struct Struct
	{
		/// <summary>
		/// Is a bit arbitrary - basically a type or 'label' of sorts that could
		/// be used by the hardcode.
		/// </summary>
		internal uint typeid;

		/// <summary>
		/// This is irrelevant after the file loads (Fields will subsequently be
		/// scanned by child-nodes in the TreeList, NOT by fieldids).
		/// </summary>
		internal List<uint> fieldids;
	}



	/// <summary>
	/// Object that contains the loaded GFF data.
	/// </summary>
	sealed class GffData
	{
		#region Properties
		/// <summary>
		/// Path-file-extension of the currently loaded GFF. The value shall be
		/// "TopLevelStruct" if a path has not been set by Open or Save yet.
		/// </summary>
		internal string Pfe
		{ get; set; }

		/// <summary>
		/// The type+version string as found in the header - eg, "UTC V3.2"
		/// </summary>
		internal string TypeVer
		{ get; set; }

		/// <summary>
		/// The loaded data's GFF-type.
		/// </summary>
		internal GffType Type
		{ get; set; }

		/// <summary>
		/// Tracks if the data has been saved to file.
		/// </summary>
		internal bool Changed
		{ get; set; }

		/// <summary>
		/// Stores the file's latest DateTime.
		/// </summary>
		internal DateTime Latest
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file that loaded; the
		/// default is merely a placeholder for user-created data until the file
		/// gets written to disk</param>
		internal GffData(string pfe = Globals.TopLevelStruct)
		{
			Changed = (Pfe = pfe) == Globals.TopLevelStruct;
		}
		#endregion cTor


		/// <summary>
		/// Data-structure of a single Field.
		/// @note Is declared as a class instead of a struct because I don't
		/// want the added hassles of dealing with a struct in C#.
		/// @note Or the hassle of dealing with 15+ different types of
		/// field-objects.
		/// </summary>
		internal class Field
		{
			internal FieldType type;

			internal string label;			// ASCII (GeneralGFF policy: alphanumeric and underscore only)

			internal byte   BYTE;
			internal sbyte  CHAR;
			internal ushort WORD;
			internal short  SHORT;
			internal uint   DWORD;
			internal int    INT;

			internal ulong  DWORD64;
			internal long   INT64;

			internal float  FLOAT;
			internal double DOUBLE;

			internal string CResRef;		// ASCII (eg. saving a blueprint from the toolset changes non-Ascii Unicode chars to "?")
			internal string CExoString;		// UTF8

			internal uint CExoLocStrref;	// CExoLocString is actually an integer field.
			internal List<Locale> Locales;
			internal uint localeflags;		// bitflags stored by a CExoLocString that denote what Locales it currently has.

			internal uint localeid;			// for use by a locale pseudo-field (to find itself within its parent).

			internal byte[] VOID;

			internal List<uint> List;		// this is irrelevant after the file has loaded.

			internal Struct Struct;


			/// <summary>
			/// Duplicates a specified Field since Clone() is effed regardless.
			/// </summary>
			/// <param name="field"></param>
			/// <returns></returns>
			internal static Field Duplicate(Field field)
			{
				var field_ = new Field();

				field_.type          = field.type;
				field_.label         = field.label;
				field_.BYTE          = field.BYTE;
				field_.CHAR          = field.CHAR;
				field_.WORD          = field.WORD;
				field_.SHORT         = field.SHORT;
				field_.DWORD         = field.DWORD;
				field_.INT           = field.INT;
				field_.DWORD64       = field.DWORD64;
				field_.INT64         = field.INT64;
				field_.FLOAT         = field.FLOAT;
				field_.DOUBLE        = field.DOUBLE;
				field_.CResRef       = field.CResRef;
				field_.CExoString    = field.CExoString;
				field_.CExoLocStrref = field.CExoLocStrref;

				if (field.Locales != null)
				{
					field_.Locales = new List<Locale>();
					for (int i = 0; i != field.Locales.Count; ++i)
					{
						var locale = new Locale();
						locale.langid = field.Locales[i].langid;
						locale.F      = field.Locales[i].F;
						locale.local  = field.Locales[i].local;

						field_.Locales.Add(locale);
					}
				}
				field_.localeflags = field.localeflags;
				field_.localeid    = field.localeid;

				if (field.VOID != null)
				{
					field_.VOID = new byte[field.VOID.Length];
					for (int i = 0; i != field.VOID.Length; ++i)
					{
						field_.VOID[i] = field.VOID[i];
					}
				}

//				if (field.List != null)
//				{
//					field_.List = new List<uint>();
//					for (int i = 0; i != field.List.Count; ++i)
//					{
//						field_.List.Add(field.List[i]);
//					}
//				}

				field_.Struct.typeid   = field.Struct.typeid;
//				field_.Struct.fieldids = field.Struct.fieldids;

				return field_;
			}
		}


		/// <summary>
		/// Data-structure of a CExoLocString's localized string entries (if it
		/// has any).
		/// @note Is declared as a class instead of a struct because I don't
		/// want the added hassles of dealing with a struct in C#.
		/// </summary>
		internal class Locale
		{
			internal Language langid;
			internal bool F;

			internal string local; // UTF8

			/// <summary>
			/// Converts a GFF-LanguageId into its true LanguageId with
			/// masculine/neutral or feminine disposition.
			/// </summary>
			/// <param name="id">a GFF-style languageid</param>
			internal void SetLocaleLanguage(uint id)
			{
				if (id != (uint)Language.GffToken)
				{
					langid = (Language)(id / 2);
					F      = ((id & 1) != 0);
				}
				else
					langid = Language.GffToken;
			}

			/// <summary>
			/// Converts a real LanguageId into a readable string (GFF files
			/// store fake LanguageId(s) that need to be converted ...).
			/// @note A valid return should be lesser in length than
			/// GeneralGFF.LENGTH_LABEL (17-chars) to account for the limited
			/// space given to Labels in the treelist and to account for a
			/// possible suffix of GeneralGFF.SUF_F ("[F]"). As well as a
			/// trailing space-char.
			/// </summary>
			/// <param name="langid"></param>
			/// <param name="f">true if feline</param>
			/// <returns></returns>
			internal static string GetLanguageString(Language langid, bool f)
			{
				string l;

				switch (langid)
				{
					case Language.English:            l = "English";   break;
					case Language.French:             l = "French";    break;
					case Language.German:             l = "German";    break;
					case Language.Italian:            l = "Italian";   break;
					case Language.Spanish:            l = "Spanish";   break;
					case Language.Polish:             l = "Polish";    break;
					case Language.Russian:            l = "Russian";   break;
					case Language.Korean:             l = "Korean";    break;
					case Language.ChineseTraditional: l = "Chinese T"; break;
					case Language.ChineseSimplified:  l = "Chinese S"; break;
					case Language.Japanese:           l = "Japanese";  break;

					case Language.GffToken:
						return "GffToken";

					default:
						return "ErROr:lang na";
				}

				if (f) l += Globals.SUF_F;

				return l;
			}


			/// <summary>
			/// Duplicates a specified Locale since Clone() is effed regardless.
			/// </summary>
			/// <param name="locale"></param>
			/// <returns></returns>
			internal static Locale Duplicate(Locale locale)
			{
				var locale_ = new Locale();

				locale_.langid = locale.langid;
				locale_.F      = locale.F;
				locale_.local  = locale.local;

				return locale_;
			}
		}


		#region Methods (static)
		/// <summary>
		/// Converts a file's first 3-byte ASCII-string sequence to a recognized
		/// GffType.
		/// </summary>
		/// <param name="type">the first 3-chars of the file as a string</param>
		/// <returns></returns>
		internal static GffType GetGffType(string type)
		{
			switch (type)
			{
				case "ARE": return GffType.ARE; // area
				case "BIC": return GffType.BIC; // saved player character
				case "CAM": return GffType.CAM; // campaign info
				case "DLG": return GffType.DLG; // dialog file
				case "FAC": return GffType.FAC; // faction table
				case "GIC": return GffType.GIC; // area object-lists (counts only)
				case "GIT": return GffType.GIT; // area object-lists (incl/ area-properties and object-data)
				case "IFO": return GffType.IFO; // (1) module info (2) saved player character list and data
				case "JRL": return GffType.JRL; // journal
				case "PFB": return GffType.PFB; // prefab
				case "ROS": return GffType.ROS; // roster character
				case "RST": return GffType.RST; // rosterlist
				case "ULT": return GffType.ULT; // light effect
				case "UPE": return GffType.UPE; // placeable effect
				case "UTC": return GffType.UTC; // creature
				case "UTD": return GffType.UTD; // door
				case "UTE": return GffType.UTE; // encounter
				case "UTI": return GffType.UTI; // item
				case "UTM": return GffType.UTM; // merchant (store)
				case "UTP": return GffType.UTP; // placeable
				case "UTR": return GffType.UTR; // tree
				case "UTS": return GffType.UTS; // sound
				case "UTT": return GffType.UTT; // trigger
				case "UTW": return GffType.UTW; // waypoint
				case "WMP": return GffType.WMP; // world map info
			}
			return GffType.generic; // eg. "GFF"
		}

		/// <summary>
		/// Converts the loaded file's GffType to a string.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static string GetGffString(GffType type)
		{
			switch (type)
			{
				case GffType.ARE: return "ARE"; // area
				case GffType.BIC: return "BIC"; // saved player character
				case GffType.CAM: return "CAM"; // campaign info
				case GffType.DLG: return "DLG"; // dialog file
				case GffType.FAC: return "FAC"; // faction table
				case GffType.GIC: return "GIC"; // area object-lists (counts only)
				case GffType.GIT: return "GIT"; // area object-lists (incl/ area-properties and object-data)
				case GffType.IFO: return "IFO"; // (1) module info (2) saved player character list and data
				case GffType.JRL: return "JRL"; // journal
				case GffType.PFB: return "PFB"; // prefab
				case GffType.ROS: return "ROS"; // roster character
				case GffType.RST: return "RST"; // rosterlist
				case GffType.ULT: return "ULT"; // light effect
				case GffType.UPE: return "UPE"; // placeable effect
				case GffType.UTC: return "UTC"; // creature
				case GffType.UTD: return "UTD"; // door
				case GffType.UTE: return "UTE"; // encounter
				case GffType.UTI: return "UTI"; // item
				case GffType.UTM: return "UTM"; // merchant (store)
				case GffType.UTP: return "UTP"; // placeable
				case GffType.UTR: return "UTR"; // tree
				case GffType.UTS: return "UTS"; // sound
				case GffType.UTT: return "UTT"; // trigger
				case GffType.UTW: return "UTW"; // waypoint
				case GffType.WMP: return "WMP"; // world map info
			}
			return "GFF"; // GffType.generic;
		}

		/// <summary>
		/// The file-filter string for an OpenFile or SaveFile dialog.
		/// @note Maintain its congruity with GffTypes.
		/// </summary>
		internal const string FileDialogFilter =
				  "All files|*.*"
				+ "|" + GFF + "|*.GFF"
				+ "|" + ARE + "|*.ARE"
				+ "|" + BIC + "|*.BIC"
				+ "|" + CAM + "|*.CAM"
				+ "|" + DLG + "|*.DLG"
				+ "|" + FAC + "|*.FAC"
				+ "|" + GIC + "|*.GIC"
				+ "|" + GIT + "|*.GIT"
				+ "|" + IFO + "|*.IFO"
				+ "|" + JRL + "|*.JRL"
				+ "|" + PFB + "|*.PFB"
				+ "|" + ROS + "|*.ROS"
				+ "|" + RST + "|*.RST"
				+ "|" + ULT + "|*.ULT"
				+ "|" + UPE + "|*.UPE"
				+ "|" + UTC + "|*.UTC"
				+ "|" + UTD + "|*.UTD"
				+ "|" + UTE + "|*.UTE"
				+ "|" + UTI + "|*.UTI"
				+ "|" + UTM + "|*.UTM"
				+ "|" + UTP + "|*.UTP"
				+ "|" + UTR + "|*.UTR"
				+ "|" + UTS + "|*.UTS"
				+ "|" + UTT + "|*.UTT"
				+ "|" + UTW + "|*.UTW"
				+ "|" + WMP + "|*.WMP";

		#endregion Methods (static)


		#region tips
		internal const string GFF = "generic file";
		internal const string ARE = "area object";
		internal const string BIC = "player character";
		internal const string CAM = "campaign info";
		internal const string DLG = "dialog file";
		internal const string FAC = "faction table";
		internal const string GIC = "area object lists";
		internal const string GIT = "area properties and objects";
		internal const string IFO = "module info or playerlist info";
		internal const string JRL = "journal";
		internal const string PFB = "prefab";
		internal const string ROS = "roster character";
		internal const string RST = "rosterlist";
		internal const string ULT = "light effect";
		internal const string UPE = "placeable effect";
		internal const string UTC = "creature object";
		internal const string UTD = "door object";
		internal const string UTE = "encounter object";
		internal const string UTI = "item object";
		internal const string UTM = "merchant store";
		internal const string UTP = "placeable object";
		internal const string UTR = "tree object";
		internal const string UTS = "sound object";
		internal const string UTT = "trigger object";
		internal const string UTW = "waypoint object";
		internal const string WMP = "world map info";
		#endregion tips
	}
}
