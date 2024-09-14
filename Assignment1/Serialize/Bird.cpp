//-----------------------------------------------------------------------------
// Real-Time Networking
//----------------------------------------------------------------------------- 

#include <stdio.h>
#include <string.h>
#include "Unused.h"
#include "Bird.h"

// constructor
Bird::Bird()
: x(0), y(0), s(0)
{
}

// destructor
Bird::~Bird()
{
	//printf("Bird destructor\n");
	delete this->s;
}


// specialized constructor
Bird::Bird( int _x, short _y)
	: x(_x), y(_y)
{
	this->s = new char[55]; // do NOT assume fixed size value...
 	const char *refText = "This is a test to have a very long string to Serialize";
	strcpy_s( this->s, 55, refText ); // do NOT assume fixed size value...
}

// accessor
int Bird::getX() const
{
	return this->x;
};

short Bird::getY() const
{
	return this->y;
};

// return a const read pointer to the string
const char *Bird::getS() const
{
	return this->s;
};

void Bird::clear()
{
	delete this->s;
	this->s = nullptr;
	this->x = 0;
	this->y = 0;
}

// Read from a buffer
void Bird::deserialize( const char * const buffer ) 
{
	//deserialize y
	memcpy(&this->y, buffer, sizeof(this->y));

	//deserialize x
	memcpy(&this->x, buffer + sizeof(this->y), sizeof(this->x));

	//deserial p
	//first length
	int length = 0;
	memcpy(&length, buffer + sizeof(this->y) + sizeof(this->x), sizeof(length));

	//now string
	this->s = new char[length + 1];
	strcpy_s(this->s, length + 1, buffer + sizeof(this->y) + sizeof(this->x) + sizeof(length));
	//memcpy(&this->s, buffer + sizeof(this->y) + sizeof(this->x) + sizeof(length), length);
	
}

// Write object to a buffer
void Bird::serialize( char * const buffer ) const
{
	char* p = buffer;

	//serialize y
	memcpy(p, &this->y, sizeof(this->y));

	p += sizeof(this->y);

	//serialize x
	memcpy(p, &this->x, sizeof(this->x));

	p += sizeof(this->x);

	//serial p
	//first length
	int length = strlen(this->s);
	memcpy(p, &length, sizeof(length));

	p += sizeof(length);

	//now string
	std::string str(s, length);

	//int size = strlen(str.c_str());
	strcpy_s(p, length + 1, str.c_str());

	//memcpy(p, &str, length);

}

