//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

//---------------------------------------------------------------------------
// HEADER FILES:
//---------------------------------------------------------------------------
#include "Fish.h"


//---------------------------------------------------------------------------
// TESTS:
//---------------------------------------------------------------------------
TEST( FishClass, Serialize_tests, true )
{

	// create an apple
	apple myApple;
	myApple.a = 44;
	myApple.b = 0x78463910;
	myApple.c = -567;

	// create an orange
	orange myOrange;
	myOrange.a = 78;
	myOrange.b = 'K';
	myOrange.x = 1345.849f;
	myOrange.d = 'e';

	// initial data
	Fish  data( myApple, myOrange, 99, 'q', 84.61f );

	// Check the data before the save
	CHECK( data.getX() == 99 );
	CHECK( data.getY() == 84.61f);
	CHECK( data.getA() == 'q' );

	CHECK( data.getApple().a == 44 );
	CHECK( data.getApple().b == 0x78463910);
	CHECK( data.getApple().c == -567);

	CHECK( data.getOrange().a == 78 );
	CHECK( data.getOrange().b == 'K' );
	CHECK( data.getOrange().x == 1345.849f);
	CHECK( data.getOrange().d == 'e' );

	// Check data structure sizes
	CHECK( sizeof(Fish) == 20);
	CHECK( sizeof(apple) == 12 );

	// create a local buffer
	char buff[ 1024 ];

	// serialize the data
	data.serialize( buff );
	data.clear();

	// Create a new
	Fish newData;

	// deserialize the data
	newData.deserialize( buff );

	// validate that data is the same
	CHECK( newData.getX() == 99 );
	CHECK( newData.getY() == 84.61f);
	CHECK( newData.getA() == 'q' );

	CHECK( newData.getApple().a == 44 );
	CHECK( newData.getApple().b == 0x78463910);
	CHECK( newData.getApple().c == -567);

	CHECK( newData.getOrange().a == 78 );
	CHECK( newData.getOrange().b == 'K' );
	CHECK( newData.getOrange().x == 1345.849f);
	CHECK( newData.getOrange().d == 'e' );


}
TEST_END