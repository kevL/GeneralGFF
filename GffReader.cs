using System;
using System.Collections.Generic;
using System.Text;


namespace generalgff
{
	/// <summary>
	/// Static object that reads GFF files.
	/// </summary>
	static class GffReader
	{
		#region Fields (static)
		const uint head_StructOffset       =  8; // 0x08 // offsets (in bytes) of data in the header section ->
		const uint head_StructCount        = 12; // 0x0C

		const uint head_FieldOffset        = 16; // 0x10
		const uint head_FieldCount         = 20; // 0x14

		const uint head_LabelOffset        = 24; // 0x18
		const uint head_LabelCount         = 28; // 0x1C

		const uint head_FieldDataOffset    = 32; // 0x20
		const uint head_FieldDataLength    = 36; // 0x24

		const uint head_FieldIndicesOffset = 40; // 0x28
		const uint head_FieldIndicesLength = 44; // 0x2C

		const uint head_ListIndicesOffset  = 48; // 0x30
		const uint head_ListIndicesLength  = 52; // 0x34
		#endregion Fields (static)


		#region Properties (static)
		static readonly List<Struct> _structs = new List<Struct>();
		/// <summary>
		/// All Structs in sequential order.
		/// </summary>
		internal static List<Struct> Structs
		{ get { return _structs; } }


		static readonly List<GffData.Field> _fields = new List<GffData.Field>();
		/// <summary>
		/// All Fields in sequential order.
		/// </summary>
		internal static List<GffData.Field> Fields
		{ get { return _fields; } }


/*		static readonly List<uint> _fieldids = new List<uint>();
		/// <summary>
		/// 
		/// </summary>
		internal static List<uint> FieldIds
		{ get { return _fieldids; } } */
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Reads a GFF-file and parses out its data to a GFFData object.
		/// @note Sections will be extracted in a non-arbitrary sequence because
		/// the data in a specific section could rely on the data in a different
		/// section. The order I have chosen is:
		/// 1. Header data (ofc)
		/// 2. FieldIndices
		/// 3. Structs - relies on FieldIndices
		/// 4. Labels
		/// 5. Fields - relies on Structs and Labels and extracts FieldData and ListIndices
		/// </summary>
		/// <param name="pfe">path-file-extension - ensure file exists before call</param>
		internal static GffData ReadGFFfile(string pfe)
		{
			Structs.Clear();
			Fields .Clear();

			byte[] bytes = FileService.ReadFile(pfe);
			if (bytes != null)
			{
				if (bytes.Length != 0)
				{
					uint pos = 0;
					uint b;

					var buffer = new byte[8];
					for (b = 0; b != 8; ++b)
						buffer[b] = bytes[pos++];

					string ver = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

					if (ver.Substring(3) != Globals.SupportedVersion)
					{
						FileService.ShowErrorBox("That is not a version 3.2 GFF file.");
						return null;
					}


					bool le = BitConverter.IsLittleEndian; // hardware architecture

					var data = new GffData(pfe);

					data.TypeVer = ver;
					data.Type = GffData.GetGffType(ver.Substring(0,3));


// HEADER METADATA ->
					pos = head_StructOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint StructOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("StructOffset= " + StructOffset);

					// The Struct-section will always start at 56-bytes (0x38)
					if (StructOffset != Globals.Length_HEADER)
					{
						FileService.ShowErrorBox("That does not appear to be a GFF file.");
						return null;
					}

					pos = head_StructCount;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint StructCount = BitConverter.ToUInt32(buffer, 0); // count of elements
					//logfile.Log("StructCount= " + StructCount);


					pos = head_FieldOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("FieldOffset= " + FieldOffset);


					pos = head_FieldCount;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldCount = BitConverter.ToUInt32(buffer, 0); // count of elements
					//logfile.Log("FieldCount= " + FieldCount);


					pos = head_LabelOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint LabelOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("LabelOffset= " + LabelOffset);


					pos = head_LabelCount;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint LabelCount = BitConverter.ToUInt32(buffer, 0); // count of elements
					//logfile.Log("LabelCount= " + LabelCount);


					pos = head_FieldDataOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldDataOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("FieldDataOffset= " + FieldDataOffset);


					pos = head_FieldDataLength;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldDataCount = BitConverter.ToUInt32(buffer, 0); // count of bytes (not used.)
					//logfile.Log("FieldDataCount= " + FieldDataCount);


					pos = head_FieldIndicesOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldIndicesOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("FieldIndicesOffset= " + FieldIndicesOffset);


					pos = head_FieldIndicesLength;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint FieldIndicesCount = BitConverter.ToUInt32(buffer, 0); // count of bytes
					//logfile.Log("FieldIndicesCount= " + FieldIndicesCount);


					pos = head_ListIndicesOffset;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint ListIndicesOffset = BitConverter.ToUInt32(buffer, 0);
					//logfile.Log("ListIndicesOffset= " + ListIndicesOffset);


					pos = head_ListIndicesLength;
					buffer = new byte[4];
					for (b = 0; b != 4; ++b)
						buffer[b] = bytes[pos++];

					if (!le) Array.Reverse(buffer);
					uint ListIndicesCount = BitConverter.ToUInt32(buffer, 0); // count of bytes (not used.)
					//logfile.Log("ListIndicesCount= " + ListIndicesCount);


					//logfile.Log("");

// FIELDID DATA -> contains the FieldIds of Structs that contain > 1 field (is req'd for parsing Structs)
// - a list of DWORD ids into the Fields section
					//logfile.Log("FIELDIDS");
					//int fid = 0; // log

					var fieldids = new List<uint>();

					pos = FieldIndicesOffset;
					while (pos != FieldIndicesOffset + FieldIndicesCount)
					{
						//logfile.Log(". start= " + FieldIndicesOffset + " stop= " + (FieldIndicesOffset + FieldIndicesCount));
						//logfile.Log(". fid #" + fid++);

						buffer = new byte[4]; // 4-byte DWORD - field id
						for (b = 0; b != 4; ++b)
						{
							//logfile.Log(". . pos= " + pos);
							buffer[b] = bytes[pos++];
						}

						if (!le) Array.Reverse(buffer);
						fieldids.Add(BitConverter.ToUInt32(buffer, 0)); // WARNING: There is no safety on the count below.
					}


// STRUCT DATA ->
// - DWORD type-id
// - DWORD dataordataoffset
// - DWORD fieldcount
					uint fields, idoroffset, i,j;

					pos = StructOffset;
					for (i = 0; i != StructCount; ++i)
					{
						//logfile.Log("Struct #" + i);
						var st = new Struct();

						buffer = new byte[4]; // type-id ->
						for (b = 0; b != 4; ++b)
							buffer[b] = bytes[pos++];

						if (!le) Array.Reverse(buffer);
						st.typeid = BitConverter.ToUInt32(buffer, 0);
						//logfile.Log(". typid= " + st.typeid);

						buffer = new byte[4]; // dataordataoffset ->
						for (b = 0; b != 4; ++b)
							buffer[b] = bytes[pos++];

						if (!le) Array.Reverse(buffer);
						idoroffset = BitConverter.ToUInt32(buffer, 0);	// if fields=1 -> id into the FieldArray
																		// if fields>1 -> offset into FieldIndices -> list of ids into the FieldArray

						buffer = new byte[4]; // fieldcount ->
						for (b = 0; b != 4; ++b)
							buffer[b] = bytes[pos++];

						if (!le) Array.Reverse(buffer);
						fields = BitConverter.ToUInt32(buffer, 0);
						//logfile.Log(". fields= " + fields);

						st.fieldids = new List<uint>();
						if (fields == 1)		// get the FieldId directly w/ the Struct's 'idoroffset' id ->
						{
							//logfile.Log(". . idoroffset/id= " + idoroffset);
							//logfile.Log(". . data._fields.Count= " + data._fields.Count);
							st.fieldids.Add(idoroffset);
						}
						else if (fields > 1)	// get the FieldIds out of the FieldIndices w/ the 'idoroffset' offset ->
						{
							uint fieldid;
							for (j = 0; j != fields; ++j)
							{
								//logfile.Log(". . [" + j + "] idoroffset/offset= " + idoroffset + " -> fieldids id= " + (idoroffset / 4 + j));
								//logfile.Log(". . data._fieldids Length= " + (data._fieldids.Count * 4));
								//logfile.Log(". . data._fields.Count= " + data._fields.Count);

								fieldid = fieldids[(int)(idoroffset / Globals.Length_DWORD + j)];	// 4 bytes in each DWORD (ie. convert offset to id id)
								//logfile.Log(". . fieldid= " + fieldid);
								st.fieldids.Add(fieldid);											// isn't the GFF format wonderful ... at least it works
							}																		// the Bioware documentation could be better.
						}																			// Ps. it contains inaccurate and unspecific info

						Structs.Add(st);
					}


// LABEL DATA -> contains Labels for the Fields (is req'd for parsing Fields)
// - each label shall be unique across the entire GFF data
// - 16-CHAR
					var labels = new List<string>();
					string label;

					pos = LabelOffset;
					for (i = 0; i != LabelCount; ++i)
					{
						buffer = new byte[Globals.Length_LABEL]; // 16-byte CHAR(s) - label length
						for (b = 0; b != Globals.Length_LABEL; ++b)
							buffer[b] = bytes[pos++];

						label = Encoding.ASCII.GetString(buffer, 0, buffer.Length).TrimEnd('\0');
						labels.Add(label);
					}


// FIELD DATA ->
// - the doc contradicts itself by saying that the TopLevelStruct is the 1st
//   entry in the Fields section but that the Fields section does not contain
//   the TopLevelStruct ... the latter appears to be correct: the first Field in
//   a toolset-written FieldsArray is "Description" eg.
// - DWORD datatype
// - DWORD label-id
// - DWORD dataordataoffset
//   - if BYTE,CHAR,WORD,SHORT,DWORD,INT,FLOAT -> dataordataoffset is a value.
//   - if DWORD64,INT64,DOUBLE,CExoString,CResRef,CExoLocString -> dataordataoffset
//     is an offset from the start of the FieldDataBlock section to complex data:
//     - DWORD64       8-bytes
//     - INT64         8-bytes
//     - DOUBLE        8-bytes
//     - CExoString    DWORD (length) + chars
//     - CExoLocString DWORD (total length) + DWORD (strref) + DWORD (stringcount) + [INT (id) + INT (length) + chars]
//     - CResRef       1-byte (length) + chars (lowercase)
//     - VOID          arbitrary
//   - if Struct -> dataordataoffset is an id into the Struct section.
//   - if List -> dataordataoffset is an offset from the start of the ListIndices
//     section to an array of DWORDs, the first of which is the count of DWORDS
//     that follow, which are ids into the Struct section.

					uint offset, length, count;

					pos = FieldOffset;
					for (i = 0; i != FieldCount; ++i)
					{
						var field = new GffData.Field();


						buffer = new byte[4]; // 4-byte DWORD - field type
						for (b = 0; b != 4; ++b)
							buffer[b] = bytes[pos++];

						if (!le) Array.Reverse(buffer);
						field.type = (FieldTypes)BitConverter.ToUInt32(buffer, 0);


						buffer = new byte[4]; // 4-byte DWORD - field label id
						for (b = 0; b != 4; ++b)
							buffer[b] = bytes[pos++];

						if (!le) Array.Reverse(buffer);
						field.label = labels[(int)BitConverter.ToUInt32(buffer, 0)];
						//logfile.Log("label= " + field.label);


						buffer = new byte[4];		// 4-byte DWORD - field data (val, is not a DWORD per se) or data
						for (b = 0; b != 4; ++b)	// offset into (a) DataBlock or (b) ListIds or id into (c) Structs
							buffer[b] = bytes[pos++];


						switch (field.type)
						{
							// WARNING: non-Complex types whose size is less than or
							// equal to 4-bytes are (according to the doc) contained
							// in the first byte(s) of the dataordataoffset 'DWORD'.

							case FieldTypes.BYTE:
								field.BYTE = buffer[0];
								break;

							case FieldTypes.CHAR:
							{
								var a = (sbyte[])(object)new[]{ buffer[0] };
								field.CHAR = a[0];
								break;
							}

							case FieldTypes.WORD:
							{
								var a = new byte[2];
								if (le)
								{
									a[0] = buffer[0];
									a[1] = buffer[1];
								}
								else
								{
									a[0] = buffer[1];
									a[1] = buffer[0];
								}

								field.WORD = BitConverter.ToUInt16(a, 0);
								break;
							}

							case FieldTypes.SHORT:
							{
								var a = new byte[2];
								if (le)
								{
									a[0] = buffer[0];
									a[1] = buffer[1];
								}
								else
								{
									a[0] = buffer[1];
									a[1] = buffer[0];
								}

								field.SHORT = BitConverter.ToInt16(a, 0);
								break;
							}

							case FieldTypes.DWORD:
								if (!le) Array.Reverse(buffer);
								field.DWORD = BitConverter.ToUInt32(buffer, 0);
								break;

							case FieldTypes.INT:
								if (!le) Array.Reverse(buffer);
								field.INT = BitConverter.ToInt32(buffer, 0);
								break;

							case FieldTypes.DWORD64:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);
								buffer = new byte[8];
								for (b = 0; b != 8; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								field.DWORD64 = BitConverter.ToUInt64(buffer, 0);
								break;

							case FieldTypes.INT64:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);
								buffer = new byte[8];
								for (b = 0; b != 8; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								field.INT64 = BitConverter.ToInt64(buffer, 0);
								break;

							case FieldTypes.FLOAT:
								if (!le) Array.Reverse(buffer);
								field.FLOAT = BitConverter.ToSingle(buffer, 0);
								break;

							case FieldTypes.DOUBLE:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);
								buffer = new byte[8];
								for (b = 0; b != 8; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								field.DOUBLE = BitConverter.ToDouble(buffer, 0);
								break;

							case FieldTypes.CResRef:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);
								length = bytes[offset]; // 1-byte size

								++offset;
								buffer = new byte[(int)length];
								for (b = 0; b != length; ++b)
									buffer[b] = bytes[offset++];

								field.CResRef = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
								break;

							case FieldTypes.CExoString:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);

								buffer = new byte[4];
								for (b = 0; b != 4; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								length = BitConverter.ToUInt32(buffer, 0); // 4-byte size

								buffer = new byte[(int)length];
								for (b = 0; b != length; ++b)
									buffer[b] = bytes[offset++];

								field.CExoString = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
								field.CExoString = field.CExoString.Replace("\r\n","\n").Replace("\r","\n").Replace("\n","\r\n");
								break;

							case FieldTypes.CExoLocString:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);

								buffer = new byte[4];								// total length (not incl/ these 4 bytes) ->
								for (b = 0; b != 4; ++b)							// aka Pointless. just advance the offset val
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								length = BitConverter.ToUInt32(buffer, 0); // 4-byte size

								buffer = new byte[4];								// strref (-1 no strref)
								for (b = 0; b != 4; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								field.CExoLocStrref = BitConverter.ToUInt32(buffer, 0); // 4-byte size

								buffer = new byte[4];								// substring count
								for (b = 0; b != 4; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								count = BitConverter.ToUInt32(buffer, 0); // 4-byte size

								if (count != 0)
								{
									field.Locales = new List<GffData.Locale>();

									//logfile.Log("label= " + field.label);
									for (j = 0; j != count; ++j)
									{
										var locale = new GffData.Locale();

										buffer = new byte[4]; // langid
										for (b = 0; b != 4; ++b)
											buffer[b] = bytes[offset++];

										if (!le) Array.Reverse(buffer);
										locale.SetLocaleLanguage(BitConverter.ToUInt32(buffer, 0));

										buffer = new byte[4]; // stringlength
										for (b = 0; b != 4; ++b)
											buffer[b] = bytes[offset++];

										if (!le) Array.Reverse(buffer);
										length = BitConverter.ToUInt32(buffer, 0);
										//logfile.Log("length= " + length);

										buffer = new byte[(int)length];
										for (b = 0; b != length; ++b)
											buffer[b] = bytes[offset++];

										locale.local = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
										locale.local = locale.local.Replace("\r\n","\n").Replace("\r","\n").Replace("\n","\r\n");
										//logfile.Log("local= " + locale.local);

										field.Locales.Add(locale);
									}
								}
								break;

							case FieldTypes.VOID:
								if (!le) Array.Reverse(buffer);
								offset = FieldDataOffset + BitConverter.ToUInt32(buffer, 0);
								buffer = new byte[4];
								for (b = 0; b != 4; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								length = BitConverter.ToUInt32(buffer, 0);

								field.VOID = new byte[(int)length];

								for (j = 0; j != length; ++j)
									field.VOID[j] = bytes[offset++];

								break;

							case FieldTypes.List: // a list-type Field is an offset into the FieldIndices; the later contains a list of StructIds.
								if (!le) Array.Reverse(buffer);
								offset = ListIndicesOffset + BitConverter.ToUInt32(buffer, 0); // offset into the (not)FieldIndices(not) -> try ListIndices

								buffer = new byte[4]; // 4-byte DWORD - count of structids
								for (b = 0; b != 4; ++b)
									buffer[b] = bytes[offset++];

								if (!le) Array.Reverse(buffer);
								count = BitConverter.ToUInt32(buffer, 0);

								var list = new List<uint>();
								for (j = 0; j != count; ++j)
								{
									buffer = new byte[4]; // 4-byte DWORD - structid
									for (b = 0; b != 4; ++b)
										buffer[b] = bytes[offset++];

									if (!le) Array.Reverse(buffer);
									list.Add(BitConverter.ToUInt32(buffer, 0));
								}
								field.List = list;
								break;

							case FieldTypes.Struct:
								if (!le) Array.Reverse(buffer);
								field.Struct = Structs[(int)BitConverter.ToUInt32(buffer, 0)]; // NOTE: That is an id into the Structs not an offset.
								break;
						}

						Fields.Add(field);
					}
					return data;
				}

				FileService.ShowErrorBox("That file has no data.");
			}
			return null;
		}
		#endregion Methods (static)
	}
}

/*
** data types **
_FieldType_		_Size(bytes)_	_Description_
BYTE			1				Unsigned single byte (0 to 255)
CHAR			1				Single character byte
WORD			2				Unsigned integer value (0 to 65535)
SHORT			2				Signed integer (-32768 to 32767)
DWORD			4				Unsigned integer (0 to 4294967296)
INT				4				Signed integer (-2147483648 to 2147483647)
DWORD64			8				Unsigned integer (0 to roughly 18E18)
INT64			8				Signed integer (roughly -9E18 to +9E18)
FLOAT			4				Floating point value
DOUBLE			8				Double-precision floating point value
CExoString		variable		Non-localized string
CResRef			16				Filename of a game resource. Max length is 16 characters. Unused characters are nulls.
CExoLocString	variable		Localized string. Contains a StringRef DWORD, and a number of CExoStrings, each having their own language ID.
VOID			variable		Variable-length arbitrary data
Struct			variable		A complex data type that can contain any number of any of the other data types, including other Structs.
List			variable		A list of Structs.


The GFF header contains a number of values, all of them DWORDs (32-bit unsigned
integers). The header contains offset information for all the other sections in
the GFF file. Values in the header are as follows, and arranged in the order
listed:

** header format **
_Value_				_Description_
FileType			4-char file type string
FileVersion			4-char GFF Version. At the time of writing, the version is "V3.2"
StructOffset		Offset of Struct array as bytes from the beginning of the file
StructCount			Number of elements in Struct array
FieldOffset			Offset of Field array as bytes from the beginning of the file
FieldCount			Number of elements in Field array
LabelOffset			Offset of Label array as bytes from the beginning of the file
LabelCount			Number of elements in Label array
FieldDataOffset		Offset of Field Data as bytes from the beginning of the file
FieldDataCount		Number of bytes in Field Data block
FieldIndicesOffset	Offset of Field Indices array as bytes from the beginning of the file
FieldIndicesCount	Number of bytes in Field Indices array
ListIndicesOffset	Offset of List Indices array as bytes from the beginning of the file
ListIndicesCount	Number of bytes in List Indices array

The FileVersion should always be "V3.2" for all GFF files that use the Generic
File Format as described in this document. If the FileVersion is different, then
the application should abort reading the GFF file.

The FileType is a programmer-defined 4-byte character string that identifies the
content-type of the GFF file. By convention, it is a 3-letter file extension in
all-caps, followed by a space. For example, "DLG ", "ITP ", etc. When opening a
GFF file, the application should check the FileType to make sure that the file
being opened is of the expected type.


_TypeID_	_Type_			_Complex_
0			BYTE
1			CHAR
2			WORD
3			SHORT
4			DWORD
5			INT
6			DWORD64			yes
7			INT64			yes
8			FLOAT
9			DOUBLE			yes
10			CExoString		yes
11			CResRef			yes
12			CExoLocString	yes
13			VOID			yes
14			Struct			yes*
15			List			yes**


_Language_			_ID_
English				0
French				1
German				2
Italian				3
Spanish				4
Polish				5
Korean				128
Chinese Traditional	129
Chinese Simplified	130
Japanese			131
*/
