// CTFField
// AB 4/21

#ifndef _CTFField
#define _CTFField

#include "Position.h"

class CTFField
{
private:
	static const int GridSize = 10;

public:
	static const char NoOneMarker = ' ';
	static const char Title[];
	static const int FieldWidth = 2 * GridSize;
	static const int FieldHeight = GridSize;

private:
	static const char BorderMarker = '#';
	char Field[FieldHeight][FieldWidth];

public:
	CTFField();
	~CTFField() = default;
	CTFField(const CTFField&) = delete;
	CTFField& operator=(const CTFField&) = delete;

	Position GetDimension() { return Position(FieldWidth,FieldHeight);}
	char GetCharAtPos(Position p) { return Field[p.y][p.x]; }
	void SetCharAtPos(Position p, char c) { Field[p.y][p.x] = c;	}
	void ClearCharAtPos(Position p) { SetCharAtPos(p, NoOneMarker); }

	void Display();

};


#endif _CTFField
