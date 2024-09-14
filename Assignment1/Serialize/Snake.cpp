//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#include <stdio.h>
#include <string.h>
#include "Unused.h"
#include "Snake.h"

Medusa::Medusa()
: head(0)
{
}

Medusa::~Medusa()
{
	Snake *pSnake = (Snake *)this->head;

	// delete every snake associated with Medusa
	while( pSnake != 0 )
	{
		// squirrel away for delete
		Snake *pTmp = pSnake;

		// advanced to next snake
		pSnake = (Snake *)pSnake->next;

		// goodbye old snake
		delete(pTmp);
	}
}

void Medusa::clear()
{
	this->head = nullptr;
}

void Medusa::insertSnake( Snake &inSnake )
{
	// fix pointers
	if( this->head != 0 )
	{
		this->head->prev = &inSnake;
		inSnake.next = this->head;
	}
	// push to the front of head
	this->head = &inSnake;
}

Egg::Egg( char inA, double inB, int inX )
: a(inA),b(inB),x(inX)
{
}

SnakeLink::SnakeLink()
: next(0), prev(0)
{
}

Snake::Snake(unsigned int _key, char inA, double inB, int inX)
: key(_key)
{
	this->pEgg = new Egg( inA, inB, inX);
}

Snake::~Snake()
{
	delete pEgg;
}

// Accessors
const SnakeLink *Medusa::getHeadSnake( ) const
{
	return head;
}

const unsigned int Snake::getKey() const
{
	return this->key;
}

const Egg &Snake::getEgg() const
{
	return *this->pEgg;
}


// Read from a buffer
void Snake::deserialize( const char * const buffer )
{
	// do your magic here
	//deserialize egg -> a
	this->pEgg = new Egg;
	memcpy(&this->pEgg->a, buffer, sizeof(char));

	//deserialize egg -> b
	memcpy(&this->pEgg->b, buffer + sizeof(char), sizeof(double));

	//deserialize egg -> x
	memcpy(&this->pEgg->x, buffer + sizeof(char) + sizeof(double), sizeof(int));

	//deserialize Snake -> key
	memcpy(&this->key, buffer + sizeof(char) + sizeof(double) + sizeof(int), sizeof(unsigned int));
}

// Write object to a buffer
void Snake::serialize( char * const buffer ) const
{
	// do your magic here
	char* p = buffer;

	//serialize Egg -> a
	memcpy(p, &this->pEgg->a, sizeof(char));

	p += sizeof(char);

	//serialize Egg -> b
	memcpy(p, &this->pEgg->b, sizeof(double));

	p += sizeof(double);

	//serialize Egg -> x
	memcpy(p, &this->pEgg->x, sizeof(int));

	p += sizeof(int);

	//serialize Snake -> key
	memcpy(p, &this->key, sizeof(unsigned int));
}

// Read from a buffer
void Medusa::deserialize( const char * const buffer )
{
	const char * p = buffer;

	unsigned int tmpKey;
	double tmpB;
	int tmpX;
	char tmpA;

	//get Snake 1 key
	memcpy(&tmpKey, p, sizeof(unsigned int));
	p += sizeof(unsigned int);

	//get Snake 1 Egg -> b
	memcpy(&tmpB, p, sizeof(double));
	p += sizeof(double);

	//get Snake 1 Egg -> x
	memcpy(&tmpX, p, sizeof(int));
	p += sizeof(int);

	//get Snake 1 Egg -> a
	memcpy(&tmpA, p, sizeof(char));
	p += sizeof(char);

	//create new Snake 1
	this->head = new Snake(tmpKey, tmpA, tmpB, tmpX);

	//get Snake 2 key
	memcpy(&tmpKey, p, sizeof(unsigned int));
	p += sizeof(unsigned int);

	//get Snake 2 Egg -> b
	memcpy(&tmpB, p, sizeof(double));
	p += sizeof(double);

	//get Snake 2 Egg -> x
	memcpy(&tmpX, p, sizeof(int));
	p += sizeof(int);

	//get Snake 2 Egg -> a
	memcpy(&tmpA, p, sizeof(char));
	p += sizeof(char);

	//create new Snake 2
	this->head->next = new Snake(tmpKey, tmpA, tmpB, tmpX);

	//get Snake 3 key
	memcpy(&tmpKey, p, sizeof(unsigned int));
	p += sizeof(unsigned int);

	//get Snake 3 Egg -> b
	memcpy(&tmpB, p, sizeof(double));
	p += sizeof(double);

	//get Snake 3 Egg -> x
	memcpy(&tmpX, p, sizeof(int));
	p += sizeof(int);

	//get Snake 3 Egg -> a
	memcpy(&tmpA, p, sizeof(char));
	p += sizeof(char);

	//create new Snake 3
	this->head->next->next = new Snake(tmpKey, tmpA, tmpB, tmpX);

	//get Snake 4 key
	memcpy(&tmpKey, p, sizeof(unsigned int));
	p += sizeof(unsigned int);

	//get Snake 4 Egg -> b
	memcpy(&tmpB, p, sizeof(double));
	p += sizeof(double);

	//get Snake 4 Egg -> x
	memcpy(&tmpX, p, sizeof(int));
	p += sizeof(int);

	//get Snake 4 Egg -> a
	memcpy(&tmpA, p, sizeof(char));
	p += sizeof(char);

	//create new Snake 4
	this->head->next->next->next = new Snake(tmpKey, tmpA, tmpB, tmpX);

	//prev links
	this->head->prev = nullptr;
	this->head->next->prev = head;
	this->head->next->next->prev = head->next;
	this->head->next->next->next->prev = head->next->next;

}

// Write object to a buffer
void Medusa::serialize( char * const buffer ) const
{
	Snake* snakeHead = (Snake*)getHeadSnake();
	char* p = buffer;

	//egg 1
	unsigned int snakeKey = snakeHead->getKey();
	memcpy(p, &snakeKey, sizeof(unsigned int));
	p += sizeof(unsigned int);

	Egg snakeEgg = snakeHead->getEgg();
	Egg* eggPointer = &snakeEgg;
	
	memcpy(p, &eggPointer->b, sizeof(double));
	p += sizeof(double);

	memcpy(p, &eggPointer->x, sizeof(int));
	p += sizeof(int);

	memcpy(p, &eggPointer->a, sizeof(char));
	p += sizeof(char);

	//egg 2
	snakeHead = (Snake*)snakeHead->next;

	snakeKey = snakeHead->getKey();
	memcpy(p, &snakeKey, sizeof(unsigned int));
	p += sizeof(unsigned int);

	snakeEgg = snakeHead->getEgg();
	eggPointer = &snakeEgg;

	memcpy(p, &eggPointer->b, sizeof(double));
	p += sizeof(double);

	memcpy(p, &eggPointer->x, sizeof(int));
	p += sizeof(int);

	memcpy(p, &eggPointer->a, sizeof(char));
	p += sizeof(char);

	//egg 3
	snakeHead = (Snake*)snakeHead->next;

	snakeKey = snakeHead->getKey();
	memcpy(p, &snakeKey, sizeof(unsigned int));
	p += sizeof(unsigned int);

	snakeEgg = snakeHead->getEgg();
	eggPointer = &snakeEgg;

	memcpy(p, &eggPointer->b, sizeof(double));
	p += sizeof(double);

	memcpy(p, &eggPointer->x, sizeof(int));
	p += sizeof(int);

	memcpy(p, &eggPointer->a, sizeof(char));
	p += sizeof(char);

	//egg 4
	snakeHead = (Snake*)snakeHead->next;

	snakeKey = snakeHead->getKey();
	memcpy(p, &snakeKey, sizeof(unsigned int));
	p += sizeof(unsigned int);

	snakeEgg = snakeHead->getEgg();
	eggPointer = &snakeEgg;

	memcpy(p, &eggPointer->b, sizeof(double));
	p += sizeof(double);

	memcpy(p, &eggPointer->x, sizeof(int));
	p += sizeof(int);

	memcpy(p, &eggPointer->a, sizeof(char));
	p += sizeof(char);
}
