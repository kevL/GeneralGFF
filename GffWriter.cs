using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace GeneralGFF
{
	static class GffWriter
	{
		#region Fields (static)
		const int Length_HEADER = 56; // 56-bytes in the header section

		const int Length_STRUCT = 12; // 12-bytes per Struct  ( 3 uints)
		const int Length_FIELD  = 12; // 12-bytes per Field   ( 3 uints)
		const int Length_LABEL  = 16; // 16-bytes per Label   (16 'chars') <- TODO: just use ascii-bytes (7-bit byte: 0..127).


		static readonly List<string> LabelsList = new List<string>();

		static readonly List<byte> Structs   = new List<byte>();
		static readonly List<byte> Fields    = new List<byte>();
		static readonly List<byte> Labels    = new List<byte>();
		static readonly List<byte> DataBlock = new List<byte>();
		static readonly List<byte> FieldIds  = new List<byte>();
		static readonly List<byte> ListIds   = new List<byte>();
		#endregion Fields (static)


		#region Fields
		static bool _le;
		#endregion Fields


		#region Methods (static)
		/// <summary>
		/// Writes all data to a GFF file.
		/// </summary>
		/// <param name="pfe">path-file-extension to write to</param>
		/// <param name="tl">the TreeList</param>
		/// <param name="ver"></param>
		internal static void WriteGFFfile(string pfe, TreeView tl, string ver)
		{
			//logfile.Log("");
			//logfile.Log("");
			//logfile.Log("WriteUtcFile()");

			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + ".t";
			else
				pfeT = pfe;

			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				_le = BitConverter.IsLittleEndian;


				LabelsList.Clear();

				Structs  .Clear();
				Fields   .Clear();
				Labels   .Clear();
				DataBlock.Clear();
				FieldIds .Clear();
				ListIds  .Clear();

				//logfile.Log(". add TopLevelStruct");
				AddStruct(tl.Nodes[0], true); // add the TopLevelStruct - nearly all else follows.
				SwapTlsToStart();


				//logfile.Log("");

				byte[] buffer;

				buffer = Encoding.ASCII.GetBytes(ver); //GffReader.Ver
				fs.Write(buffer, 0, buffer.Length);

				// Header STRUCTS ->
				const uint start_Structs = Length_HEADER;
				//logfile.Log("start_Structs= " + start_Structs);
				buffer = BitConverter.GetBytes(start_Structs);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint records_Structs = (uint)(Structs.Count / Length_STRUCT); // count of entries in Structs
				//logfile.Log("records_Structs= " + records_Structs);
				buffer = BitConverter.GetBytes(records_Structs);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				// Header FIELDS ->
				uint start_Fields = start_Structs + (uint)Structs.Count;
				//logfile.Log("start_Fields= " + start_Fields);
				buffer = BitConverter.GetBytes(start_Fields);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint records_Fields = (uint)(Fields.Count / Length_FIELD); // count of entries in Fields
				//logfile.Log("records_Fields= " + records_Fields);
				buffer = BitConverter.GetBytes(records_Fields);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				// Header LABELS ->
				ConvertLabelsListToLabels();

				uint start_Labels = start_Fields + (uint)Fields.Count;
				//logfile.Log("start_Labels= " + start_Labels);
				buffer = BitConverter.GetBytes(start_Labels);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint records_Labels = (uint)(Labels.Count / Length_LABEL); // count of entries in Labels
				//logfile.Log("records_Labels= " + records_Labels);
				buffer = BitConverter.GetBytes(records_Labels);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				// Header DATABLOCK ->
				uint start_Data = start_Labels + (uint)Labels.Count;
				//logfile.Log("offset_Data= " + start_Data);
				buffer = BitConverter.GetBytes(start_Data);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint bytes_Data = (uint)(DataBlock.Count); // count of bytes in the DataBlock
				//logfile.Log("bytes_Data= " + bytes_Data);
				buffer = BitConverter.GetBytes(bytes_Data);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				// Header FIELDIDS ->
				uint start_FieldIds = start_Data + (uint)DataBlock.Count;
				//logfile.Log("start_FieldIds= " + start_FieldIds);
				buffer = BitConverter.GetBytes(start_FieldIds);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint bytes_FieldIds = (uint)(FieldIds.Count); // count of bytes in FieldIds
				//logfile.Log("bytes_FieldIds= " + bytes_FieldIds);
				buffer = BitConverter.GetBytes(bytes_FieldIds);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				// Header LISTIDS ->
				uint start_ListIds = start_FieldIds + (uint)FieldIds.Count;
				//logfile.Log("start_ListIds= " + start_ListIds);
				buffer = BitConverter.GetBytes(start_ListIds);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);

				uint bytes_ListIds = (uint)(ListIds.Count); // count of bytes in ListIds
				//logfile.Log("bytes_ListIds= " + bytes_ListIds);
				buffer = BitConverter.GetBytes(bytes_ListIds);
				if (!_le) Array.Reverse(buffer);

				fs.Write(buffer, 0, buffer.Length);


				//logfile.Log("");

				// Write STRUCTS ->
				buffer = Structs.ToArray();
				//logfile.Log("Structs length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);

				// Write FIELDS ->
				buffer = Fields.ToArray();
				//logfile.Log("Fields length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);

				// Write LABELS ->
				buffer = Labels.ToArray();
				//logfile.Log("Labels length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);

				// Write DATABLOCK ->
				buffer = DataBlock.ToArray();
				//logfile.Log("DataBlock length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);

				// Write FIELDIDS ->
				buffer = FieldIds.ToArray();
				//logfile.Log("FieldIds length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);

				// Write LISTIDS ->
				buffer = ListIds.ToArray();
				//logfile.Log("ListIds length= " + buffer.Length);
				fs.Write(buffer, 0, buffer.Length);
			}

			if (pfeT != pfe)
				FileService.ReplaceFile(pfe);
		}


		/// <summary>
		/// Converts an unsigned integer (4-bytes) into a little-endian array of
		/// 4-bytes.
		/// </summary>
		/// <param name="u"></param>
		/// <returns></returns>
		static byte[] GetBytes(uint u)
		{
			byte[] a = BitConverter.GetBytes(u);
			if (!_le) Array.Reverse(a);
			return a;
		}

		/// <summary>
		/// Swaps the TopLevelStruct bytes to the start of Structs.
		/// </summary>
		static void SwapTlsToStart()
		{
			var a = new byte[Length_STRUCT];

			for (int i = 0; i != Length_STRUCT; ++i)
				a[i] = Structs[Structs.Count - Length_STRUCT + i];

			for (int i = Structs.Count - Length_STRUCT - 1; i != -1; --i)
				Structs[i + Length_STRUCT] = Structs[i];

			for (int i = 0; i != Length_STRUCT; ++i)
				Structs[i] = a[i];
		}


		//static int _iSt = -1; // log.

		/// <summary>
		/// Adds a Struct to the Structs array.
		/// @note Structs that are in Lists (also the TopLevelStruct) do not
		/// have a Field associated. Only a Struct that is not in a List (also
		/// not the TopLevelStruct) has a Field associated with it.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="tls">true if TopLevelStruct; the TLS does not have a
		/// typeid or a label and its treenode does not have a (GffData.Field)Tag</param>
		/// <returns>the Struct's id for use by a Field</returns>
		static uint AddStruct(TreeNode node, bool tls = false)
		{
			//logfile.Log("");
			//int iSt = ++_iSt; // log.
			//logfile.Log("AddStruct" + iSt + " " + node.Text);

			// typeid     (DWORD)
			// idoroffset (DWORD) id into Fields or offset into FieldIds
			// fieldcount (DWORD)

			// kL_Policy #3876503: Blank Structs shall not be written.
			// (Although blank Lists shall be written.)
			//
			// overruled: Add blank Structs ...

			uint structid;

			int fields = node.Nodes.Count;
			//logfile.Log("AddStruct" + iSt + " . fieldcount= " + fields);

			var field = (GffData.Field)node.Tag;
			if (!tls && fields != field.Struct.fieldids.Count) // test.
			{
				logfile.Log("AddStruct" + /*iSt +*/ " ERROR: nodes.count vs fieldids.count Not Equal");
				structid = UInt32.MaxValue;
			}
			else
			{
				uint typeid;
				if (tls)
					typeid = (uint)0xFFFFFFFF;
				else
					typeid = field.Struct.typeid;

				//logfile.Log("AddStruct" + iSt + " . . typeid= " + typeid);

				if (fields == 0)
				{
					structid = (uint)(Structs.Count / Length_STRUCT);

					Structs.AddRange(GetBytes(typeid));						// -> write typeid
					Structs.AddRange(GetBytes((uint)0));					// -> write fieldid
				}
				else if (fields == 1)										// idoroffset is an id into Fields ->
				{
					//logfile.Log("AddStruct" + iSt + ". . . (fields == 1)");

					uint fieldid = AddField(node.Nodes[0]);					// write a Field and get its id
					//logfile.Log("AddStruct(" + iSt + ") . . fieldid= " + fieldid);

					structid = (uint)(Structs.Count / Length_STRUCT);

					Structs.AddRange(GetBytes(typeid));						// -> write typeid
					Structs.AddRange(GetBytes(fieldid));					// -> write fieldid - st.fieldids[0]
				}
				else														// idoroffset is an offset into FieldIds ->
				{
					//logfile.Log("AddStruct" + iSt + ". . . (fields > 1)");

					var fieldids = new List<byte>();
					for (int i = 0; i != fields; ++i)						// write the Fields and get their ids
					{
						uint fieldid = AddField(node.Nodes[i]);
						//logfile.Log("AddStruct" + iSt + " . . . [" + i + "] fieldid= " + fieldid);
						fieldids.AddRange(GetBytes(fieldid));				// -> write fieldid - st.fieldids[i]
					}

					uint offset = (uint)FieldIds.Count;						// write the fieldids to FieldIds ->
					//logfile.Log("AddStruct" + iSt + " . . offset= " + offset);

					FieldIds.AddRange(fieldids); // yep that's it. FieldIds is just a list of fieldids (accessed only by Structs).

					structid = (uint)(Structs.Count / Length_STRUCT);

					Structs.AddRange(GetBytes(typeid));						// -> write typeid
					Structs.AddRange(GetBytes(offset));						// -> write FieldIds offset
				}

				//logfile.Log("AddStruct" + iSt + " . . fields= " + fields);
				Structs.AddRange(GetBytes((uint)fields));					// -> write fieldcount
			}

			//logfile.Log("AddStruct" + iSt + " . structid= " + (tls ? 0 : structid + 1));
			return ++structid; // +1 to account for the TLS
		}

		/// <summary>
		/// Adds a Field and returns its id.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static uint AddField(TreeNode node)
		{
			//logfile.Log("");
			//logfile.Log("AddField() " + node.Text);

			uint fieldid = UInt32.MaxValue;

			switch (((GffData.Field)node.Tag).type)
			{
				// datatype     (DWORD)
				// labelid      (DWORD)
				// dataoroffset (DWORD)	- if the data type is smaller than a DWORD then the first bytes of the DataOrDataOffset
				//						  DWORD up to the size of the data type itself contain the data value.
				//						- if the Field data is complex then the DataOrDataOffset value is equal to a byte offset from the
				//						  beginning of the Field Data Block pointing to the raw bytes that represent the complex data.

				// Simple fields ->
				// return the value directly ->
				// 4-bytes ->

				// Note that if its size is less than 4-bytes it should be
				// stored in the first bytes of those 4-bytes.
				case FieldTypes.CHAR:
				case FieldTypes.BYTE:
				case FieldTypes.WORD:
				case FieldTypes.SHORT:
				case FieldTypes.DWORD:
				case FieldTypes.INT:
				case FieldTypes.FLOAT:
					fieldid = AddSimpleField(node);
					break;

				// Complex fields ->
				// return an offset into the DataBlock ->
				// 8-bytes ->
				case FieldTypes.DWORD64:
				case FieldTypes.INT64:
				case FieldTypes.DOUBLE:
				case FieldTypes.CResRef:
				case FieldTypes.CExoString:
				case FieldTypes.CExoLocString:
				case FieldTypes.VOID:
					fieldid = AddComplexDataField(node);
					break;

				// return an offset into ListIds (a list of structids) ->
				case FieldTypes.List:
					fieldid = AddComplexListField(node);
					break;

				// return an id into the Fields if count is 1
				// else an offset into the FieldIds if count is > 1
				case FieldTypes.Struct:
					// if a Struct adds a Struct a Field is created that points to that Struct
					// This is the only way that a Field points to a Struct.
					// Structs that are in Lists as well as the TopLevelStruct
					// ARE NOT REPRESENTED BY FIELDS.
					fieldid = AddComplexStructField(node);
					break;
			}

			//logfile.Log(". ret fieldid= " + fieldid);
			return fieldid;
		}

		/// <summary>
		/// Writes a Simple field and gets its id.
		/// @note 3.4 If the data type is smaller than a DWORD then the first
		/// bytes of the DataOrDataOffset DWORD up to the size of the data type
		/// itself contain the data value.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static uint AddSimpleField(TreeNode node)
		{
			//logfile.Log("AddSimpleField()");
			// datatype (DWORD)
			// labelid  (DWORD)
			// data     (DWORD)

			uint id = (uint)(Fields.Count / Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type));
			Fields.AddRange(GetBytes(GetLabelId(field.label)));

			var a = new byte[4];
			switch (field.type)
			{
				// TODO: I don't like ASCII chars being C# chars
				// (C# chars are 2-bytes but ASCII chars are 1-byte - actually 7-bit)
				// just use byte.
				case FieldTypes.CHAR: // 2.2 Single character byte
					a[0] = (byte)field.CHAR;
					a[1] = (byte)0;
					a[2] = (byte)0;
					a[3] = (byte)0;
					break;

				case FieldTypes.BYTE:
					a[0] = (byte)field.BYTE;
					a[1] = (byte)0;
					a[2] = (byte)0;
					a[3] = (byte)0;
					break;

				case FieldTypes.WORD:
				{
					var b = BitConverter.GetBytes(field.WORD);
					if (!_le) Array.Reverse(b);
					a[0] = b[0];
					a[1] = b[1];
					a[2] = (byte)0;
					a[3] = (byte)0;
					break;
				}

				case FieldTypes.SHORT:
				{
					var b = BitConverter.GetBytes(field.SHORT);
					if (!_le) Array.Reverse(b);
					a[0] = b[0];
					a[1] = b[1];
					a[2] = (byte)0;
					a[3] = (byte)0;
					break;
				}

				case FieldTypes.DWORD:
					a = GetBytes(field.DWORD);
					break;

				case FieldTypes.INT:
					a = BitConverter.GetBytes(field.INT);
					if (!_le) Array.Reverse(a);
					break;

				case FieldTypes.FLOAT:
					a = BitConverter.GetBytes(field.FLOAT);
					if (!_le) Array.Reverse(a);
					break;
			}

			Fields.AddRange(a);

			//logfile.Log(". id= " + id);
			return id;
		}

		/// <summary>
		/// Writes a Complex field (as well as its data to the data-block) and
		/// gets its id.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static uint AddComplexDataField(TreeNode node)
		{
			//logfile.Log("AddComplexDataField()");
			// datatype (DWORD)
			// labelid  (DWORD)
			// offset   (DWORD) - offset into the DataBlock

			// kL_Note: why didn't you guys go with either ids or offsets instead of both.

			uint id = (uint)(Fields.Count / Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type));
			Fields.AddRange(GetBytes(GetLabelId(field.label)));


			uint offset = (uint)DataBlock.Count;

			byte[] buffer = null; // NOTE: shall not be null (sic).
			switch (field.type)
			{
				// 8-bytes in the DataBlock ->
				case FieldTypes.DWORD64:
					buffer = BitConverter.GetBytes(field.DWORD64); // do not convert to uint
					goto case FieldTypes.locale;

				case FieldTypes.INT64:
					buffer = BitConverter.GetBytes(field.INT64); // do not convert to uint
					goto case FieldTypes.locale;

				case FieldTypes.FLOAT:
					buffer = BitConverter.GetBytes(field.FLOAT); // do not convert to uint
					goto case FieldTypes.locale;

				case FieldTypes.locale: // Is not a locale. Is just a goto-label.
					if (!_le) Array.Reverse(buffer);
					DataBlock.AddRange(buffer);
					break;


				// arbitrary length in the DataBlock ->
				case FieldTypes.CResRef:
				{
					// TODO: Ensure that user uses only ASCII characters and that
					// the length is < 256. (16-chars for NwN and 32-chars for NwN2)
					// [I believe that this is the only difference between NwN and NwN2 GFF-data.]
					//
					// NOTE: CResRef is stored in lowercase characters w/out extension.

					string str = field.CResRef.ToLower(CultureInfo.InvariantCulture);
					DataBlock.Add((byte)str.Length);
					DataBlock.AddRange(Encoding.ASCII.GetBytes(str));
					break;
				}

				case FieldTypes.CExoString:
				{
					string str = field.CExoString;
					DataBlock.AddRange(GetBytes((uint)str.Length));
					DataBlock.AddRange(Encoding.ASCII.GetBytes(str)); // TODO: Should probably be UTF8. But perhaps not.
					break;
				}

				case FieldTypes.CExoLocString:
				{
					// TODO: check that -1 has been converted to 0xFFFFFFFF
					byte[] bufferStrref = GetBytes((uint)field.CExoLocStrref); // (DWORD)

					// TODO: what if there are no strings ... does StringCount
					// still need to be set "0" or can it be forgotten ... I
					// think the safe thing to do is to set it regardless.
					int strings = node.Nodes.Count;
					byte[] bufferLocalCount = GetBytes((uint)strings); // (DWORD)

					// NOTE: 'total' does not include the size of the 'total' variable itself.
					int total = bufferStrref.Length + bufferLocalCount.Length;


					var bytesList = new List<byte[]>();

					GffData.Locale locale;
					int langid;
					string local;

					//logfile.Log(". . strings= " + strings);
					for (int i = 0; i != strings; ++i)
					{
//						int localeid = (int)((GffData.Field)node.Nodes[i].Tag).localeid;
//						locale = ((GffData.Field)node.Nodes[i].Tag).Locales[localeid];

						// NOTE: 'localeid' ought be identical to 'i'.
						//logfile.Log(". . . [" + i + "]");
						//logfile.Log(". . . localeid= " + ((GffData.Field)node.Nodes[i].Tag).localeid);

						locale = ((GffData.Field)node.Tag).Locales[i];

						langid = (int)locale.langid * 2;
						if (locale.F) ++langid;

						buffer = BitConverter.GetBytes(langid); // (INT)
						if (!_le) Array.Reverse(buffer);

						bytesList.Add(buffer);
						total += buffer.Length;


						local = locale.local;
						buffer = BitConverter.GetBytes(local.Length); // (INT)
						if (!_le) Array.Reverse(buffer);

						bytesList.Add(buffer);
						total += buffer.Length;


						buffer = Encoding.ASCII.GetBytes(local); // TODO: Should probably be UTF8.
						bytesList.Add(buffer);
						total += buffer.Length;
					}

					byte[] bufferTotal = GetBytes((uint)total); // (DWORD)

					DataBlock.AddRange(bufferTotal);
					DataBlock.AddRange(bufferStrref);
					DataBlock.AddRange(bufferLocalCount);

					foreach (var b in bytesList)
						DataBlock.AddRange(b);

					break;
				}


				case FieldTypes.VOID:
					DataBlock.AddRange(field.VOID);
					break;
			}

			Fields.AddRange(GetBytes(offset));

			//logfile.Log(". id= " + id);
			return id;
		}

		/// <summary>
		/// Writes a Complex list-field (as well as its ListIds and Structs) and
		/// gets its id.
		/// @note All records in a List are Structs and Structs that are records
		/// of a List do not have Labels; in fact Structs that are the records
		/// of a List ARE NOT ACCESSED BY FIELDS at all.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static uint AddComplexListField(TreeNode node)
		{
			//logfile.Log("AddComplexListField()");
			// datatype (DWORD)
			// labelid  (DWORD)
			// offset   (DWORD) - offset into ListIds -> ids into the Structs

			var listids = new List<byte>();	// need this because a Struct in the List could have Structs and/or Lists in it.
											// Hint: Each list of ids has to be kept contiguous (although the id-vals can jitter around).
			uint structs = 0;
			for (int i = 0; i != node.Nodes.Count; ++i)
			{
				// size             (DWORD)
				// ids into Structs (DWORDs)

				listids.AddRange(GetBytes(AddStruct(node.Nodes[i])));
				++structs;
			}

			uint id = (uint)(Fields.Count / Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;
			Fields.AddRange(GetBytes((uint)field.type)); // <- FieldTypes.List
			Fields.AddRange(GetBytes(GetLabelId(field.label)));
			Fields.AddRange(GetBytes((uint)ListIds.Count));

			ListIds.AddRange(GetBytes(structs));
			ListIds.AddRange(listids);

			//logfile.Log(". id= " + id);
			return id;
		}

		/// <summary>
		/// Writes a Complex struct-field (as well as its Struct) and gets its
		/// id.
		/// @note Only the TopLevelStruct is not a Field. The rest are Fields -
		/// except Structs that are in Lists ... they are not Fields either
		/// !!surprise!!.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static uint AddComplexStructField(TreeNode node)
		{
			//logfile.Log("AddComplexStructField()");

			uint structid = AddStruct(node);


			// datatype (DWORD)
			// labelid  (DWORD)
			// data     (DWORD) - id into Structs

			uint fieldid = (uint)(Fields.Count / Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type)); // <- FieldTypes.Struct
			Fields.AddRange(GetBytes(GetLabelId(field.label)));
			Fields.AddRange(GetBytes(structid));

			//logfile.Log(". id= " + id);
			return fieldid;
		}



		/// <summary>
		/// Gets the id for a given label or adds the label to Labels and
		/// returns its id.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		static uint GetLabelId(string label)
		{
			//logfile.Log("GetLabelId() label= " + label);

			int id = LabelsList.IndexOf(label);
			if (id == -1)
			{
				uint labelid = (uint)LabelsList.Count;
				LabelsList.Add(label); // TODO: do Labels in ASCII
				return labelid;
			}

			//logfile.Log(". id= " + id);
			return (uint)id;
		}

		/// <summary>
		/// Converts the list of Labels into a byte-array of labels.
		/// @note Each label takes 16-bytes. Any extra characters are padded w/
		/// '\0'.
		/// </summary>
		static void ConvertLabelsListToLabels()
		{
			//logfile.Log("ConvertLabelsListToLabels()");

			foreach (var label in LabelsList)
			{
				byte[] ascii = Encoding.ASCII.GetBytes(label);

				var buffer = new byte[Length_LABEL]; // inits all bytes to "0"
				for (int i = 0; i != ascii.Length; ++i)
					buffer[i] = ascii[i];

				Labels.AddRange(buffer);
			}
		}
		#endregion Methods (static)
	}
}
