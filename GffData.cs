using System;
using System.Collections.Generic;


namespace generalgff
{
	#region Enums (global)
	/// <summary>
	/// The types of fields available.
	/// </summary>
	enum FieldTypes : byte
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
	enum Languages : uint
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
		FAC,		//  4
		GIC,		//  5
		GIT,		//  6
		IFO,		//  7
		JRL,		//  8
		ROS,		//  9
		ULT,		// 10
		UPE,		// 11
		UTC,		// 12
		UTD,		// 13
		UTE,		// 14
		UTI,		// 15
		UTM,		// 16
		UTP,		// 17
		UTS,		// 18
		UTT,		// 19
		UTW,		// 20
		WMP			// 21
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
			internal FieldTypes type;

			internal string label; // treated as ASCII

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

			internal string CResRef;	// treated as ASCII
			internal string CExoString;	// treated as ASCII

			internal uint CExoLocStrref;
			internal List<Locale> Locales;
			internal uint localeflags; // bitflags stored by a CExoLocString that denote what Locales it currently has.

			internal uint localeid; // for use by a locale pseudo-field (to find itself within its parent).

			internal byte[] VOID;

			internal List<uint> List;

			internal Struct Struct;
		}


		/// <summary>
		/// Data-structure of a CExoLocString's localized string entries (if it
		/// has any).
		/// @note Is declared as a class instead of a struct because I don't
		/// want the added hassles of dealing with a struct in C#.
		/// </summary>
		internal class Locale
		{
			internal Languages langid;
			internal bool F;

			internal string local; // treated as UTF8

			/// <summary>
			/// Converts a GFF-LanguageId into its true LanguageId with
			/// masculine/neutral or feminine disposition.
			/// </summary>
			/// <param name="id">a GFF-style languageid</param>
			internal void SetLocaleLanguage(uint id)
			{
				if (id != (uint)Languages.GffToken)
				{
					langid = (Languages)(id / 2);
					F      = ((id & 1) != 0);
				}
				else
					langid = Languages.GffToken;
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
			internal static string GetLanguageString(Languages langid, bool f)
			{
				string l;

				switch (langid)
				{
					case Languages.English:            l = "English";   break;
					case Languages.French:             l = "French";    break;
					case Languages.German:             l = "German";    break;
					case Languages.Italian:            l = "Italian";   break;
					case Languages.Spanish:            l = "Spanish";   break;
					case Languages.Polish:             l = "Polish";    break;
					case Languages.Russian:            l = "Russian";   break;
					case Languages.Korean:             l = "Korean";    break;
					case Languages.ChineseTraditional: l = "Chinese T"; break;
					case Languages.ChineseSimplified:  l = "Chinese S"; break;
					case Languages.Japanese:           l = "Japanese";  break;

					case Languages.GffToken:
						return "GffToken";

					default:
						return "ErROr: language type unknown";
				}

				if (f) l += "[F]";

				return l;
			}
		}


		#region Methods (static)
		/// <summary>
		/// Converts a file's first 3-byte string sequence to a recognized
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
				case "FAC": return GffType.FAC; // faction table
				case "GIC": return GffType.GIC; // area object-lists (counts only)
				case "GIT": return GffType.GIT; // area object-lists (incl/ area-properties and object-data)
				case "IFO": return GffType.IFO; // (1) module info (2) saved player character list and data
				case "JRL": return GffType.JRL; // journal
				case "ROS": return GffType.ROS; // roster character
				case "ULT": return GffType.ULT; // light effect
				case "UPE": return GffType.UPE; // placeable effect
				case "UTC": return GffType.UTC; // creature
				case "UTD": return GffType.UTD; // door
				case "UTE": return GffType.UTE; // encounter
				case "UTI": return GffType.UTI; // item
				case "UTM": return GffType.UTM; // merchant (store)
				case "UTP": return GffType.UTP; // placeable
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
				case GffType.FAC: return "FAC"; // faction table
				case GffType.GIC: return "GIC"; // area object-lists (counts only)
				case GffType.GIT: return "GIT"; // area object-lists (incl/ area-properties and object-data)
				case GffType.IFO: return "IFO"; // (1) module info (2) saved player character list and data
				case GffType.JRL: return "JRL"; // journal
				case GffType.ROS: return "ROS"; // roster character
				case GffType.ULT: return "ULT"; // light effect
				case GffType.UPE: return "UPE"; // placeable effect
				case GffType.UTC: return "UTC"; // creature
				case GffType.UTD: return "UTD"; // door
				case GffType.UTE: return "UTE"; // encounter
				case GffType.UTI: return "UTI"; // item
				case GffType.UTM: return "UTM"; // merchant (store)
				case GffType.UTP: return "UTP"; // placeable
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
				+ "|generic file|*.GFF"
				+ "|area object|*.ARE"
				+ "|player character|*.BIC"
				+ "|campaign info|*.CAM"
				+ "|faction table|*.FAC"
				+ "|area object lists|*.GIC"
				+ "|area properties and objects|*.GIT"
				+ "|module info or player list info|*.IFO"
				+ "|journal|*.JRL"
				+ "|roster character|*.ROS"
				+ "|light effect|*.ULT"
				+ "|placeable effect|*.UPE"
				+ "|creature object|*.UTC"
				+ "|door object|*.UTD"
				+ "|encounter object|*.UTE"
				+ "|item object|*.UTI"
				+ "|merchant store|*.UTM"
				+ "|placeable object|*.UTP"
				+ "|sound object|*.UTS"
				+ "|trigger object|*.UTT"
				+ "|waypoint object|*.UTW"
				+ "|world map info|*.WMP";

		#endregion Methods (static)
	}
}
