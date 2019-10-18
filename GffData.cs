﻿using System;
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
	/// </summary>
	enum GffType : byte
	{
		generic,	//  0
		ARE,		//  1
		BIC,		//  2
		FAC,		//  3
		GIC,		//  4
		GIT,		//  5
		IFO,		//  6
		JRL,		//  7
		ROS,		//  8
		ULT,		//  9
		UPE,		// 10
		UTC,		// 11
		UTD,		// 12
		UTE,		// 13
		UTI,		// 14
		UTM,		// 15
		UTP,		// 16
		UTS,		// 17
		UTT,		// 18
		UTW			// 19
	}
	#endregion Enums (global)


	/// <summary>
	/// Data-structure of a Struct.
	/// </summary>
	struct Struct
	{
		internal uint typeid; // is a bit arbitrary - basically a type or 'label' of sorts used by the hardcode.
		internal List<uint> fieldids; // this is irrelevant after the file loads (Fields will be scanned by nodes not fieldids).
	}



	/// <summary>
	/// Object that contains the loaded GFF data.
	/// </summary>
	sealed class GffData
	{
		#region Fields
		/// <summary>
		/// Path-file-extension of the currently loaded GFF.
		/// </summary>
		internal readonly string _pfe;
		#endregion Fields


		#region Properties
		/// <summary>
		/// The version-string as found in the header - eg, "UTC V3.2"
		/// </summary>
		internal string Ver
		{ get; set; }

		internal GffType Type
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file that loaded; the
		/// default is merely a placeholder until user writes to file and it
		/// TODO needs to be set properly</param>
		internal GffData(string pfe = "TopLevelStruct")
		{
			_pfe = pfe;
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
			/// <returns></returns>
			internal static string GetLanguageString(Languages langid)
			{
				switch (langid)
				{
					case Languages.English:            return "English";
					case Languages.French:             return "French";
					case Languages.German:             return "German";
					case Languages.Italian:            return "Italian";
					case Languages.Spanish:            return "Spanish";
					case Languages.Polish:             return "Polish";
					case Languages.Russian:            return "Russian";
					case Languages.Korean:             return "Korean";
					case Languages.ChineseTraditional: return "Chinese T";
					case Languages.ChineseSimplified:  return "Chinese S";
					case Languages.Japanese:           return "Japanese";

					case Languages.GffToken:           return "GffToken";
				}
				return "ErROr: language type unknown";
			}
		}


		#region Methods (static)
		/// <summary>
		/// Gets the loaded file's GffType.
		/// </summary>
		/// <param name="type">the first 3-chars of the file as a string</param>
		/// <returns></returns>
		internal static GffType GetGffType(string type)
		{
			switch (type)
			{
				case "ARE": return GffType.ARE; // area
				case "BIC": return GffType.BIC; // saved player character
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
			}
			return GffType.generic; // eg. "GFF"
		}

		/// <summary>
		/// The file-filter string for an OpenFile or SaveFile dialog.
		/// @note Maintain its congruity with GffTypes.
		/// </summary>
		internal const string FileDialogFilter =
				  "All files (*.*)|*.*"
				+ "|generic file (*.GFF)|*.GFF"
				+ "|area object (*.ARE)|*.ARE"
				+ "|player character (*.BIC)|*.BIC"
				+ "|faction table (*.FAC)|*.FAC"
				+ "|area object lists(*.GIC)|*.GIC"
				+ "|area properties and objects(*.GIT)|*.GIT"
				+ "|module info or player list info (*.IFO)|*.IFO"
				+ "|journal (*.JRL)|*.JRL"
				+ "|roster character (*.ROS)|*.ROS"
				+ "|light effect (*.ULT)|*.ULT"
				+ "|placeable effect (*.UPE)|*.UPE"
				+ "|creature object (*.UTC)|*.UTC"
				+ "|door object (*.UTD)|*.UTD"
				+ "|encounter object (*.UTE)|*.UTE"
				+ "|item object (*.UTI)|*.UTI"
				+ "|merchant store (*.UTM)|*.UTM"
				+ "|placeable object (*.UTP)|*.UTP"
				+ "|sound object (*.UTS)|*.UTS"
				+ "|trigger object (*.UTT)|*.UTT"
				+ "|waypoint object (*.UTW)|*.UTW";

		#endregion Methods (static)
	}
}
