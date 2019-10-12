namespace GeneralGFF
{
	struct Globals
	{
		internal const int Length_HEADER = 56; // 56-bytes in the header section

		internal const int Length_STRUCT = 12; // 12-bytes per Struct (3 uints)
		internal const int Length_FIELD  = 12; // 12-bytes per Field  (3 uints)
		internal const int Length_LABEL  = 16; // 16-bytes per Label

		internal const int Length_DWORD  =  4;
	}
}
