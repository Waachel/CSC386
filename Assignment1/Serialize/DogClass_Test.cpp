//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

//---------------------------------------------------------------------------
// HEADER FILES:
//---------------------------------------------------------------------------
#include "Dog.h"


//---------------------------------------------------------------------------
// TESTS:
//---------------------------------------------------------------------------
TEST( DogClass, Serialize_tests, true )
{
	// initial data
	Dog  data(18,13,43,53);

	// Check the data before the save
	CHECK( data.getI() == 18 );
	CHECK( data.getA() == 13 );
	CHECK( data.getY() == 43 );
	CHECK( data.getJ() == 53 );

	// create a local buffer
	char buff[ 1024 ];

	// serialize the data
	data.serialize( buff );
	data.clear();

	// Create a new
	Dog newData;

	// deserialize the data
	newData.deserialize( buff );

	// validate that data is the same
	CHECK( newData.getI() == 18 );
	CHECK( newData.getA() == 13 );
	CHECK( newData.getY() == 43 );
	CHECK( newData.getJ() == 53 );

	// check the size of the data structure
	CHECK( sizeof(Dog) == 16 );
}
TEST_END