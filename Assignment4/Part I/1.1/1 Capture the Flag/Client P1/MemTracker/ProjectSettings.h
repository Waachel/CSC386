//----------------------------------------------------------------------------
// Copyright 2021, Ed Keenan, all rights reserved.
//----------------------------------------------------------------------------

#ifndef PROJECT_SETTINGS_H
#define PROJECT_SETTINGS_H

// -----------------------------------------------------------------------------
// DO NOT MODIFY
// -----------------------------------------------------------------------------
// Warning levels turned on for Wall... that Should be supressed globally
//     SetCharAtPos here to avoid surpressing them in the settings
// -----------------------------------------------------------------------------

// I hate you XAudio2.h make your abstract class destructors virtual
#if _MSC_FULL_VER > 192500000
	#pragma warning( disable : 5205 ) // delete of an abstract class
#endif

// --------------------------------------
// General includes
// --------------------------------------

#pragma warning( disable : 5204 ) 
#pragma warning( disable : 5220 ) 

#endif 

// ---  End of File ---------------
