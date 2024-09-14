#include "CaptureTheFlag.h"
#include <iostream>
#include <assert.h>

CaptureTheFlag::CaptureTheFlag()
	: P1('1', 'A'), P2('2', 'B')
{
	Position baseOffset = Field.GetDimension() / 4;
	P1.SetBasePos(baseOffset);
	P2.SetBasePos(Field.GetDimension() - baseOffset - Position(1, 1));

	P1.SetLimits( Field.GetDimension() );
	P1.SetPos( P1.GetBasePos() + Position(1, 1));
	P1.SetTargetPos(P2.GetBasePos());

	P2.SetLimits(Field.GetDimension());
	P2.SetPos(P2.GetBasePos() - Position(1, 1) );
	P2.SetTargetPos(P1.GetBasePos() );
}

// Support method
void CaptureTheFlag::MovePlayer(CTFPlayer& P, Direction d, int num)
{
	Field.ClearCharAtPos(P.GetPos());	// clear old player pos

	Position offset(num, 0);	// Convert dir & num into 2D offset
	if (d == Direction::Vert) offset = Position(0, num);

	P.MoveBy(offset);
}

void CaptureTheFlag::Action(MoveData md)
{
	switch (md.player)
	{
	case Player::Player1:
		MoveP1(md.dir, md.val);
		break;
	case Player::Player2:
		MoveP2(md.dir, md.val);
		break;
	default:
		assert(false);
	}
}

// Method to use when moving P1
void CaptureTheFlag::MoveP1(Direction d, int num)
{
	MovePlayer(P1, d, num);
}

// Method to use when moving P2
void CaptureTheFlag::MoveP2(Direction d, int num)
{
	MovePlayer(P2, d, num);
}

void CaptureTheFlag::UpdateField()
{
	Field.SetCharAtPos( P1.GetPos(), P1.GetPlayerMark() );
	Field.SetCharAtPos( P2.GetPos(), P2.GetPlayerMark() );

	// Refresh bases (in case player movement erased them)
	Field.SetCharAtPos( P1.GetBasePos(), P1.GetBaseMark() );
	Field.SetCharAtPos( P2.GetBasePos(), P2.GetBaseMark() );
}

int CaptureTheFlag::coinFlip()
{
	int headsOrTails;

	headsOrTails = rand() % 2 + 1;

	if (headsOrTails == 1)
	{
		//heads
		return 1;
	}
	else
	{
		//tails
		return 0;
	}
}

char CaptureTheFlag::TestForWinner()
{
	char ans = CTFField::NoOneMarker;
	if (P1.IsWinning()) ans = P1.GetPlayerMark();
	else if (P2.IsWinning()) ans = P2.GetPlayerMark();

	return ans;
}

void CaptureTheFlag::Display()
{
	Field.Display();
	
	printf("\tP1: (%i,%i)\n", P1.GetPos().x, P1.GetPos().y);
	printf("\tP2: (%i,%i)\n", P2.GetPos().x, P2.GetPos().y);
	printf("\n");
}

void CaptureTheFlag::MoveData::serialize(char* const buffer)
{
	char* p = buffer;
	//send player
	memcpy(p, &player, sizeof(Player));

	p += sizeof(Player);

	//send direction
	memcpy(p, &dir, sizeof(Direction));

	p += sizeof(Direction);

	//send value
	memcpy(p, &val, sizeof(int));
}

void CaptureTheFlag::MoveData::deserialize(char* const buffer)
{
	char* p = buffer;

	//receive player
	memcpy(&player, p, sizeof(Player));

	p += sizeof(Player);

	//receive direction
	memcpy(&dir, p, sizeof(Direction));

	p += sizeof(Direction);

	//receive value
	memcpy(&val, p, sizeof(int));
}
