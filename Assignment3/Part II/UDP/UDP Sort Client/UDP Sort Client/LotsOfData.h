//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#ifndef LOTS_OF_DATA_H
#define LOTS_OF_DATA_H

class LotsOfData
{
public:
	// constructors
	LotsOfData();

	// destructor
	~LotsOfData();

	LotsOfData(int _x, short _y);

	// accessors
	int getX() const;
	short getY() const;
	//char* getS() const;

	void setX(int x);
	void setY(short y);

	void clear();

	// Note the ‘len’ component. It’ll be important to
	// know how big the serialization buffer actually needs to be
	void serialize(char* buffer, int& len);
	void deserialize(char* buffer, int len);

	

private:
	// data to serialize
	short	y;
	int     x;
	//char* s;

};



#endif