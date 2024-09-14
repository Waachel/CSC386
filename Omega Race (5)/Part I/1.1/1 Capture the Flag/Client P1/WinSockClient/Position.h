// Position
// AB 4/21

#ifndef _Position
#define _Position

struct Position
{
	int x;
	int y;

	Position()
		: x(0), y(0) {};
	Position(int a, int b)
		: x(a), y(b) {};

	Position operator+(const Position& B) { return Position(this->x + B.x, this->y + B.y); }
	Position operator-(const Position& B) { return Position(this->x - B.x, this->y - B.y); }
	Position operator/(const int& v) { return Position(this->x/v, this->y/v); }

	bool operator==(const Position& B) { return (this->x == B.x) && (this->y == B.y); }
};


#endif _Position