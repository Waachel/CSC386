//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#include <stdio.h>
#include <string.h>
#include "Unused.h"
#include "Fish.h"

// constructor
Fish::Fish()
: x(0),a(0),y(0.0f),pApple(0),pOrange(0)
{

}

Fish::Fish( const apple &inApple, const orange &inOrange, int inX, char inA, float inY)
{
	this->pApple = new apple(inApple);
	this->pOrange = new orange(inOrange);
	this->x = inX;
	this->a = inA;
	this->y = inY;
}

void Fish::clear()
{
	delete this->pApple;
	delete this->pOrange;

	this->pApple = nullptr;
	this->pOrange = nullptr;
	this->x = 0;
	this->a = 0;
	this->y = 0;
}


// destructor
Fish::~Fish()
{
	//printf("fish destructor\n");
	delete this->pApple;
	delete this->pOrange;
}


float Fish::getY() const
{
	return this->y;
}

int Fish::getX() const
{
	return this->x;
}

char Fish::getA() const 
{
	return this->a;
}

const apple &Fish::getApple() const 
{
	return *(this->pApple);
}

const orange &Fish::getOrange() const
{
	return *(this->pOrange);
}

// Read from a buffer
void Fish::deserialize( const char * const buffer ) 
{
	//deserialize apple -> a
	this->pApple = new apple;
	memcpy(&this->pApple->a, buffer, sizeof(int));

	//serialize Apple -> b
	memcpy(&this->pApple->b, buffer + sizeof(int), sizeof(int));

	//serialize Apple -> c
	memcpy(&this->pApple->c, buffer + sizeof(int) + sizeof(int), sizeof(int));

	//serialize Orange -> a
	this->pOrange = new orange;
	memcpy(&this->pOrange->a, buffer + sizeof(int) + sizeof(int) + sizeof(int), sizeof(int));
	
	//serialize Orange -> b
	memcpy(&this->pOrange->b, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int), sizeof(char));
	
	//serialize Orange -> x
	memcpy(&this->pOrange->x, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char), sizeof(float));
	
	//serialize Orange -> d
	memcpy(&this->pOrange->d, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char) + sizeof(float), sizeof(char));
	
	//serialize Fish - > x
	memcpy(&this->x, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char) + sizeof(float) + sizeof(char), sizeof(int));
	
	//serialize Fish - > a
	memcpy(&this->a, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char) + sizeof(float) + sizeof(char) + sizeof(int), sizeof(char));
	
	//serialize Fish - > y
	memcpy(&this->y, buffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char) + sizeof(float) + sizeof(char) + sizeof(int) + sizeof(char), sizeof(float));
}

// Write object to a buffer
void Fish::serialize( char * const buffer ) const
{
	// do your magic here
	char* p = buffer;

	//serialize Apple -> a
	memcpy(p, &this->pApple->a, sizeof(int));

	p += sizeof(int);

	//serialize Apple -> b
	memcpy(p, &this->pApple->b, sizeof(int));

	p += sizeof(int);

	//serialize Apple -> c
	memcpy(p, &this->pApple->c, sizeof(int));

	p += sizeof(int);

	//serialize Orange -> a
	memcpy(p, &this->pOrange->a, sizeof(int));

	p += sizeof(int);

	//serialize Orange -> b
	memcpy(p, &this->pOrange->b, sizeof(char));

	p += sizeof(char);

	//serialize Orange -> x
	memcpy(p, &this->pOrange->x, sizeof(float));

	p += sizeof(float);

	//serialize Orange -> d
	memcpy(p, &this->pOrange->d, sizeof(char));

	p += sizeof(char);

	//serialize Fish - > x
	memcpy(p, &this->x, sizeof(int));

	p += sizeof(int);

	//serialize Fish - > a
	memcpy(p, &this->a, sizeof(char));

	p += sizeof(char);

	//serialize Fish - > y
	memcpy(p, &this->y, sizeof(float));
}

