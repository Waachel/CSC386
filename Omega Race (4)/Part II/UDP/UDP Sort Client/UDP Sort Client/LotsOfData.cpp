#include "LotsOfData.h"

LotsOfData::LotsOfData()
	: x(0), y(0)//, s(0)
{

}

LotsOfData::~LotsOfData()
{
	//delete this->s;
}

// specialized constructor
LotsOfData::LotsOfData(int _x, short _y)
	: x(_x), y(_y)
{
	//this->s = new char[55]; // do NOT assume fixed size value...
	//const char* refText = "This is a test to have a very long string to Serialize";
	//strcpy_s(this->s, 55, refText); // do NOT assume fixed size value...
}

int LotsOfData::getX() const
{
	return this->x;
}

short LotsOfData::getY() const
{
	return this->y;
}

//char* LotsOfData::getS() const
//{
	//return this->s;
//}

void LotsOfData::setX(int _x)
{
	this->x = _x;
}
void LotsOfData::setY(short _y)
{
	this->y = _y;
}

void LotsOfData::clear()
{
	//delete this->s;
	//this->s = nullptr;
	this->x = 0;
	this->y = 0;
}

void LotsOfData::serialize(char* buffer, int& len)
{
	AZUL_UNUSED_VAR(len);
	char* p = buffer;

	//serialize y
	memcpy(p, &this->y, sizeof(this->y));

	p += sizeof(this->y);

	//serialize x
	memcpy(p, &this->x, sizeof(this->x));

	p += sizeof(this->x);

	//serial p
	//first length
	/*int length = strlen(s);
	memcpy(p, &length, sizeof(length));

	p += sizeof(length);

	//now string
	std::string str(s, length);

	//int size = strlen(str.c_str());
	strcpy_s(p, length + 1, str.c_str());

	//memcpy(p, &str, length);*/
}


void LotsOfData::deserialize(char* buffer, int len)
{
	AZUL_UNUSED_VAR(len);
	//deserialize y
	memcpy(&this->y, buffer, sizeof(this->y));

	//deserialize x
	memcpy(&this->x, buffer + sizeof(this->y), sizeof(this->x));

	//deserial p
	//first length
	int length = 0;
	memcpy(&length, buffer + sizeof(this->y) + sizeof(this->x), sizeof(length));

	/*//now string
	this->s = new char[length + 1];
	strcpy_s(this->s, length + 1, buffer + sizeof(this->y) + sizeof(this->x) + sizeof(length));
	//memcpy(&this->s, buffer + sizeof(this->y) + sizeof(this->x) + sizeof(length), length);*/

}