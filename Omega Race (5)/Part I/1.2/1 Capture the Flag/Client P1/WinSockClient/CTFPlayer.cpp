#include "CTFPlayer.h"

CTFPlayer::CTFPlayer(char pmark, char bmark)
	: PlayerMark(pmark), BaseMark(bmark)
{

}

void CTFPlayer::MoveBy(Position offset)
{
	pos = pos + offset;
	pos.x = abs(pos.x % limits.x);
	pos.y = abs(pos.y % limits.y);
}