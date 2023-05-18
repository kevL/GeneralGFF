using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace generalgff
{
	static class GffWriter
	{
		#region Fields (static)
		static readonly List<string> LabelsList = new List<string>();

		static readonly List<byte> Structs   = new List<byte>();
		static readonly List<byte> Fields    = new List<byte>();
		static readonly List<byte> Labels    = new List<byte>();
		static readonly List<byte> DataBlock = new List<byte>();
		static readonly List<byte> FieldIds  = new List<byte>();
		static readonly List<byte> ListIds   = new List<byte>();
		#endregion Fields (static)


		#region Fields (static)
		static bool _le;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Writes all data to a GFF file.
		/// </summary>
		/// <param name="pfe">path-file-extension to write to</param>
		/// <param name="tree">the TreeList</param>
		/// <param name="ver">GffReader.Ver</param>
		/// <returns>true if successful</returns>
		internal static bool WriteGFFfile(string pfe, TreeView tree, string ver)
		{
			//logfile.Log("WriteGFFfile()");
			//logfile.Log(". pfe= " + pfe);

			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + FileService.EXT_T;
			else
				pfeT = pfe;

			//logfile.Log(". pfeT= " + pfeT);

			var fs = FileService.CreateFile(pfeT);
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
				AddStruct(tree.Nodes[0], true); // add the TopLevelStruct - nearly all else follows.
				SwapTlsToStart();


				//logfile.Log("");

				byte[] buffer;

				buffer = Encoding.ASCII.GetBytes(ver);
				fs.Write(buffer, 0, buffer.Length);

				// Header STRUCTS ->
				const uint start_Structs = Globals.Length_HEADER;
				//logfile.Log("start_Structs= " + start_Structs);
				fs.Write(GetBytes(start_Structs), 0, Globals.Length_DWORD);

				uint records_Structs = (uint)(Structs.Count / Globals.Length_STRUCT); // count of entries in Structs
				//logfile.Log("records_Structs= " + records_Structs);
				fs.Write(GetBytes(records_Structs), 0, Globals.Length_DWORD);


				// Header FIELDS ->
				uint start_Fields = start_Structs + (uint)Structs.Count;
				//logfile.Log("start_Fields= " + start_Fields);
				fs.Write(GetBytes(start_Fields), 0, Globals.Length_DWORD);

				uint records_Fields = (uint)(Fields.Count / Globals.Length_FIELD); // count of entries in Fields
				//logfile.Log("records_Fields= " + records_Fields);
				fs.Write(GetBytes(records_Fields), 0, Globals.Length_DWORD);


				// Header LABELS ->
				ConvertLabelsListToLabels();

				uint start_Labels = start_Fields + (uint)Fields.Count;
				//logfile.Log("start_Labels= " + start_Labels);
				fs.Write(GetBytes(start_Labels), 0, Globals.Length_DWORD);

				uint records_Labels = (uint)(Labels.Count / Globals.Length_LABEL); // count of entries in Labels
				//logfile.Log("records_Labels= " + records_Labels);
				fs.Write(GetBytes(records_Labels), 0, Globals.Length_DWORD);


				// Header DATABLOCK ->
				uint start_Data = start_Labels + (uint)Labels.Count;
				//logfile.Log("offset_Data= " + start_Data);
				fs.Write(GetBytes(start_Data), 0, Globals.Length_DWORD);

				uint bytes_Data = (uint)(DataBlock.Count); // count of bytes in the DataBlock
				//logfile.Log("bytes_Data= " + bytes_Data);
				fs.Write(GetBytes(bytes_Data), 0, Globals.Length_DWORD);


				// Header FIELDIDS ->
				uint start_FieldIds = start_Data + (uint)DataBlock.Count;
				//logfile.Log("start_FieldIds= " + start_FieldIds);
				fs.Write(GetBytes(start_FieldIds), 0, Globals.Length_DWORD);

				uint bytes_FieldIds = (uint)(FieldIds.Count); // count of bytes in FieldIds
				//logfile.Log("bytes_FieldIds= " + bytes_FieldIds);
				fs.Write(GetBytes(bytes_FieldIds), 0, Globals.Length_DWORD);


				// Header LISTIDS ->
				uint start_ListIds = start_FieldIds + (uint)FieldIds.Count;
				//logfile.Log("start_ListIds= " + start_ListIds);
				fs.Write(GetBytes(start_ListIds), 0, Globals.Length_DWORD);

				uint bytes_ListIds = (uint)(ListIds.Count); // count of bytes in ListIds
				//logfile.Log("bytes_ListIds= " + bytes_ListIds);
				fs.Write(GetBytes(bytes_ListIds), 0, Globals.Length_DWORD);


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


				fs.Close();

				if (pfeT != pfe)
					return FileService.ReplaceFile(pfe);

				return true;
			}
			return false;
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
			var a = new byte[Globals.Length_STRUCT];

			for (int i = 0; i != Globals.Length_STRUCT; ++i)
				a[i] = Structs[Structs.Count - Globals.Length_STRUCT + i];

			for (int i = Structs.Count - Globals.Length_STRUCT - 1; i != -1; --i)
				Structs[i + Globals.Length_STRUCT] = Structs[i];

			for (int i = 0; i != Globals.Length_STRUCT; ++i)
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
		/// <param name="tls"><c>true</c> if TopLevelStruct; the TLS does not
		/// have a typeid or a label and its treenode does not have a
		/// (GffData.Field)Tag</param>
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
//			if (!tls && fields != field.Struct.fieldids.Count) // test.
//			{
//				logfile.Log("AddStruct" + /*iSt +*/ " ErROr: nodes.count vs fieldids.count Not Equal");
//				structid = UInt32.MaxValue;
//			}
//			else
//			{
			uint typeid;
			if (tls)
				typeid = UInt32.MaxValue;
			else
				typeid = field.Struct.typeid;

			//logfile.Log("AddStruct" + iSt + " . . typeid= " + typeid);

			if (fields == 0)
			{
				structid = (uint)(Structs.Count / Globals.Length_STRUCT);

				Structs.AddRange(GetBytes(typeid));						// -> write typeid
				Structs.AddRange(GetBytes((uint)0));					// -> write fieldid
			}
			else if (fields == 1)										// idoroffset is an id into Fields ->
			{
				//logfile.Log("AddStruct" + iSt + ". . . (fields == 1)");

				uint fieldid = AddField(node.Nodes[0]);					// write a Field and get its id
				//logfile.Log("AddStruct(" + iSt + ") . . fieldid= " + fieldid);

				structid = (uint)(Structs.Count / Globals.Length_STRUCT);

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

				structid = (uint)(Structs.Count / Globals.Length_STRUCT);

				Structs.AddRange(GetBytes(typeid));						// -> write typeid
				Structs.AddRange(GetBytes(offset));						// -> write FieldIds offset
			}

			//logfile.Log("AddStruct" + iSt + " . . fields= " + fields);
			Structs.AddRange(GetBytes((uint)fields));					// -> write fieldcount
//			}

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
				case FieldType.BYTE:
				case FieldType.CHAR:
				case FieldType.WORD:
				case FieldType.SHORT:
				case FieldType.DWORD:
				case FieldType.INT:
				case FieldType.FLOAT:
					fieldid = AddSimpleField(node);
					break;

				// Complex fields ->
				// return an offset into the DataBlock ->
				// 8-bytes ->
				case FieldType.DWORD64:
				case FieldType.INT64:
				case FieldType.DOUBLE:
				case FieldType.CResRef:
				case FieldType.CExoString:
				case FieldType.CExoLocString:
				case FieldType.VOID:
					fieldid = AddComplexDataField(node);
					break;

				// return an offset into ListIds (a list of structids) ->
				case FieldType.List:
					fieldid = AddComplexListField(node);
					break;

				// return an id into the Fields if count is 1
				// else an offset into the FieldIds if count is > 1
				case FieldType.Struct:
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

			uint id = (uint)(Fields.Count / Globals.Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type));
			Fields.AddRange(GetBytes(GetLabelId(field.label)));

			var a = new byte[4]; // inits to zeros
			switch (field.type)
			{
				case FieldType.BYTE:
					a[0] = (byte)field.BYTE;
					break;

				case FieldType.CHAR:
				{
					var b = (byte[])(object)new[]{ field.CHAR };
					a[0] = b[0];
					break;
				}

				case FieldType.WORD:
				{
					var b = BitConverter.GetBytes(field.WORD);
					if (!_le) Array.Reverse(b);
					a[0] = b[0];
					a[1] = b[1];
					break;
				}

				case FieldType.SHORT:
				{
					var b = BitConverter.GetBytes(field.SHORT);
					if (!_le) Array.Reverse(b);
					a[0] = b[0];
					a[1] = b[1];
					break;
				}

				case FieldType.DWORD:
					a = GetBytes(field.DWORD);
					break;

				case FieldType.INT:
					a = BitConverter.GetBytes(field.INT);
					if (!_le) Array.Reverse(a);
					break;

				case FieldType.FLOAT:
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

			uint id = (uint)(Fields.Count / Globals.Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type));
			Fields.AddRange(GetBytes(GetLabelId(field.label)));


			uint offset = (uint)DataBlock.Count;

			byte[] buffer = null; // NOTE: shall not be null (sic).
			switch (field.type)
			{
				// 8-bytes in the DataBlock ->
				case FieldType.DWORD64:
					buffer = BitConverter.GetBytes(field.DWORD64); // do not convert to uint
					goto case FieldType.locale;

				case FieldType.INT64:
					buffer = BitConverter.GetBytes(field.INT64); // do not convert to uint
					goto case FieldType.locale;

				case FieldType.DOUBLE:
					buffer = BitConverter.GetBytes(field.DOUBLE); // do not convert to uint
					goto case FieldType.locale;

				case FieldType.locale: // Is not a locale. Is just a goto-label.
					if (!_le) Array.Reverse(buffer);
					DataBlock.AddRange(buffer);
					break;


				// arbitrary length in the DataBlock ->
				case FieldType.CResRef:
					// Ensure that user uses only ASCII characters and that the
					// length is < 256. (16-chars for NwN and 32-chars for NwN2)
					// [I believe that this is the only difference between NwN and NwN2 GFF-data.]
					//
					// NOTE: CResRef is stored in lowercase characters w/out extension.

					buffer = Encoding.ASCII.GetBytes(field.CResRef.ToLower(CultureInfo.InvariantCulture)); // NOTE: That should already be lc.
					DataBlock.Add((byte)buffer.Length);
					DataBlock.AddRange(buffer);
					break;

				case FieldType.CExoString:
					buffer = Encoding.UTF8.GetBytes(field.CExoString);
					DataBlock.AddRange(GetBytes((uint)buffer.Length));
					DataBlock.AddRange(buffer);
					break;

				case FieldType.CExoLocString:
				{
					byte[] bufferStrref = GetBytes((uint)field.CExoLocStrref); // (DWORD)

					// NOTE: what if there are no strings ... does StringCount
					// still need to be set "0" or can it be forgotten ... I
					// think the safe thing to do is to set it regardless.
					int locals = node.Nodes.Count;
					byte[] bufferLocalCount = GetBytes((uint)locals); // (DWORD)

					// NOTE: 'total' does not include the size of the 'total' variable itself.
					int total = bufferStrref.Length + bufferLocalCount.Length;


					var bytesList = new List<byte[]>();

					GffData.Locale locale;
					int langid;
					byte[] local;

					//logfile.Log(". . locals= " + locals);
					for (int i = 0; i != locals; ++i)
					{
//						int localeid = (int)((GffData.Field)node.Nodes[i].Tag).localeid;
//						locale = ((GffData.Field)node.Nodes[i].Tag).Locales[localeid];

						// NOTE: 'localeid' shall be identical to 'i'.
						//logfile.Log(". . . localeid[" + i + "]= " + ((GffData.Field)node.Nodes[i].Tag).localeid);

						locale = ((GffData.Field)node.Tag).Locales[i];

						if (locale.langid != Language.GffToken)
						{
							langid = (int)locale.langid * 2;
							if (locale.F) ++langid;

							buffer = BitConverter.GetBytes(langid); // (INT)
						}
						else
							buffer = BitConverter.GetBytes(unchecked((int)locale.langid)); // converts gracefully tgit.

						if (!_le) Array.Reverse(buffer);

						bytesList.Add(buffer);
						total += buffer.Length;


						local = Encoding.UTF8.GetBytes(locale.local);

						buffer = BitConverter.GetBytes(local.Length); // (INT)
						if (!_le) Array.Reverse(buffer);

						bytesList.Add(buffer);
						total += buffer.Length;


						bytesList.Add(local);
						total += local.Length;
					}

					buffer = GetBytes((uint)total); // (DWORD)

					DataBlock.AddRange(buffer);
					DataBlock.AddRange(bufferStrref);
					DataBlock.AddRange(bufferLocalCount);

					foreach (var b in bytesList)
						DataBlock.AddRange(b);

					break;
				}


				case FieldType.VOID:
					DataBlock.AddRange(GetBytes((uint)field.VOID.Length)); // (DWORD)
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

			uint id = (uint)(Fields.Count / Globals.Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;
			Fields.AddRange(GetBytes((uint)field.type)); // <- FieldType.List
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

			uint fieldid = (uint)(Fields.Count / Globals.Length_FIELD); // id into Fields

			var field = (GffData.Field)node.Tag;

			Fields.AddRange(GetBytes((uint)field.type)); // <- FieldType.Struct
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
			int id = LabelsList.IndexOf(label);
			if (id == -1)
			{
				uint labelid = (uint)LabelsList.Count;
				LabelsList.Add(label);
				return labelid;
			}
			return (uint)id;
		}

		/// <summary>
		/// Converts the list of Labels into a byte-array of labels.
		/// @note Each label takes 16-bytes. Any extra characters are padded w/
		/// '\0'.
		/// </summary>
		static void ConvertLabelsListToLabels()
		{
			foreach (var label in LabelsList)
			{
				byte[] ascii = Encoding.ASCII.GetBytes(label);

				var buffer = new byte[Globals.Length_LABEL]; // inits all bytes to "0"
				for (int i = 0; i != ascii.Length; ++i)
					buffer[i] = ascii[i];

				Labels.AddRange(buffer);
			}
		}
		#endregion Methods (static)
	}
}
