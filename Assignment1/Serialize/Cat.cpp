//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#include <string.h>
#include "Unused.h"
#include "Cat.h"


Cat::Cat(float _x, char _a, char _b, float _y, int _i, char _c, double _d)
: x(_x), a(_a), b(_b), y(_y), i(_i), c(_c), d(_d)
{

}

void Cat::clear()
{
	d = 0;
	x = 0;
	y = 0;
	i = 0;
	c = 0;
	a = 0;
	b = 0;
}

// Read from a buffer
void Cat::deserialize( const char * const buffer ) 
{	
	// do your magic here
	memcpy(this, buffer, sizeof(Cat));
}

// Write object to a buffer
void Cat::serialize( char * const buffer ) const
{
	// do your magic here
	memcpy(buffer, this, sizeof(Cat));
}

float Cat::getX() const
{
	return this->x;
}

char Cat::getA() const
{
	return this->a;
}

char Cat::getB() const
{
	return this->b;
}

float Cat::getY() const
{
	return this->y;
}

int Cat::getI() const
{
	return this->i;
}

char Cat::getC() const
{
	return this->c;
}

double Cat::getD() const
{
	return this->d;
}
