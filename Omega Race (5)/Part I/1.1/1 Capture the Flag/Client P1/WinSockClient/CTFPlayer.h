// CTFPlayer
// AB 4/21

#ifndef _CTFPlayer
#define _CTFPlayer

#include "Position.h"

class CTFPlayer
{
private:
	Position pos;
	Position limits;
	Position target;
	char PlayerMark;
	char BaseMark;
	Position BasePos;

public:
	CTFPlayer(char pmark, char bmark);
	~CTFPlayer() = default;
	CTFPlayer(const CTFPlayer&) = delete;
	CTFPlayer& operator=(const CTFPlayer&) = delete;

	char GetPlayerMark() { return PlayerMark; }
	char GetBaseMark() { return BaseMark; }
	void SetLimits(Position lim) { limits = lim; };
	void SetTargetPos(Position t) { target = t; }
	void SetPos(Position p) { pos = p; }
	Position GetPos() { return pos; }
	void SetBasePos(Position p) { BasePos = p; }
	Position GetBasePos() { return BasePos; }
	bool IsWinning() { return pos == target; }
	void MoveBy(Position offset);
};

#endif _CTFPlayer