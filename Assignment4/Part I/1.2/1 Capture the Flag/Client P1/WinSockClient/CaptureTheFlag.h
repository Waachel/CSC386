// CaptureTheFlag
// AB 4/21

#ifndef _CaptureTheFlag
#define _CaptureTheFlag

#include "Position.h"
#include "CTFPlayer.h"
#include "CTFField.h"

class CaptureTheFlag
{
public:
	enum class Direction { Hor, Vert, None };
	enum class Player { Player1, Player2, ACK };

	struct MoveData
	{
		Player player;
		Direction dir;
		int val;

		void serialize(char* const buffer);
		void deserialize(char* const buffer);
	};

	CaptureTheFlag();
	~CaptureTheFlag() = default;
	CaptureTheFlag(const CaptureTheFlag&) = delete;
	CaptureTheFlag& operator=(const CaptureTheFlag&) = delete;

	void Action(MoveData md);
	void MoveP1(Direction d, int num);
	void MoveP2(Direction d, int num);
	void UpdateField();
	int coinFlip();
	char TestForWinner();
	void Display();

private:
	CTFField Field;
	CTFPlayer P1;
	CTFPlayer P2;

	void MovePlayer(CTFPlayer& P, Direction d, int num);
};


#endif _CaptureTheFlag

