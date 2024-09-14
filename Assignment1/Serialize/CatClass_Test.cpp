//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

//---------------------------------------------------------------------------
// HEADER FILES:
//---------------------------------------------------------------------------
#include "Cat.h"


//---------------------------------------------------------------------------
// TESTS:
//---------------------------------------------------------------------------
TEST( CatClass, Serialize_tests, true )
{
	// initial data
	Cat  data(22,13,34,15,61,67,88);

	// Check the data before the save
	CHECK( data.getX() == 22 );
	CHECK( data.getA() == 13 );
	CHECK( data.getB() == 34 );
	CHECK( data.getY() == 15 );
	CHECK( data.getI() == 61 );
	CHECK( data.getC() == 67 );
	CHECK( data.getD() == 88 );

	// create a local buffer
	char buff[ 1024 ];

	// serialize the data
	data.serialize( buff );
	data.clear();

	// Create a new
	Cat newData;

	// deserialize the data
	newData.deserialize( buff );

	// validate that data is the same
	CHECK( newData.getX() == 22 );
	CHECK( newData.getA() == 13 );
	CHECK( newData.getB() == 34 );
	CHECK( newData.getY() == 15 );
	CHECK( newData.getI() == 61 );
	CHECK( newData.getC() == 67 );
	CHECK( newData.getD() == 88 );

	// check the size of the data structure
	CHECK( sizeof(Cat) == 24 );
}
TEST_END