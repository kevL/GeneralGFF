﻿namespace generalgff
{
	struct Globals
	{
		internal const int Length_HEADER = 56; // 56-bytes in the header section

		internal const int Length_STRUCT = 12; // 12-bytes per Struct (3 uints)
		internal const int Length_FIELD  = 12; // 12-bytes per Field  (3 uints)
		internal const int Length_LABEL  = 16; // 16-bytes per Label

		internal const int Length_DWORD  =  4;
		internal const int Length_VER    =  8;


		internal const string SupportedVersion = " V3.2";
		internal const string TopLevelStruct   = "TopLevelStruct";
		internal const string SUF_F            = "[F]";

		internal const string About  = "About";

		internal const string Error   = "Error";
		internal const string Warning = "Warning";
		internal const string Quit    = "Quit";
		internal const string Close   = "Close";
		internal const string Reload  = "Reload";


		internal const uint BITS_STRREF = 0x00FFFFFF; // talktable bitflags ->
		internal const uint BITS_CUSTOM = 0x01000000;
		internal const uint BITS_UNUSED = 0xFE000000;
	}
}
