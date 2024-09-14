//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

//---------------------------------------------------------------------------
// HEADER FILES:
//---------------------------------------------------------------------------
#include "Snake.h"


//---------------------------------------------------------------------------
// TESTS:
//---------------------------------------------------------------------------
TEST( SnakeClass_0, Serialize_tests, true )
{
	// Create and load a Medusa with several snakes

		Medusa Gorgon;

		// check medusa
		CHECK( Gorgon.getHeadSnake() == 0 );

		// Create several snakes
		Snake *s0 = new Snake(0xBCCDBACD, 'r', 77.44, 55);
		Snake *s1 = new Snake(0x1C34FEC1, '#', 66.77, 10201110);
		Snake *s2 = new Snake(0xCCFE0220, '2', -23456.99, 0xcb);
		Snake *s3 = new Snake(0xFC7C4313, '?', 9234.87, 94);

		Gorgon.insertSnake( *s0 );
		Gorgon.insertSnake( *s1 );
		Gorgon.insertSnake( *s2 );
		Gorgon.insertSnake( *s3 );

	// validate the data in the medusa

		Egg tmpEgg;
		Snake *pSnake = (Snake *)Gorgon.getHeadSnake();
		CHECK( pSnake != 0);

		tmpEgg = pSnake->getEgg();
	
		CHECK( pSnake->getKey() == 0xFC7C4313);
		CHECK( tmpEgg.a == '?');
		CHECK( tmpEgg.b == 9234.87);
		CHECK( tmpEgg.x ==  94);

		pSnake = (Snake *)pSnake->next;
		CHECK( pSnake != 0);

		tmpEgg = pSnake->getEgg();

		CHECK( pSnake->getKey() == 0xCCFE0220);
		CHECK( tmpEgg.a == '2');
		CHECK( tmpEgg.b == -23456.99);
		CHECK( tmpEgg.x ==   0xcb);

		pSnake = (Snake *)pSnake->next;
		CHECK( pSnake != 0);

		tmpEgg = pSnake->getEgg();

		CHECK( pSnake->getKey() == 0x1C34FEC1);
		CHECK( tmpEgg.a == '#');
		CHECK( tmpEgg.b ==  66.77);
		CHECK( tmpEgg.x == 10201110);

		pSnake = (Snake *)pSnake->next;
		CHECK( pSnake != 0);

		tmpEgg = pSnake->getEgg();

		CHECK( pSnake->getKey() == 0xBCCDBACD);
		CHECK( tmpEgg.a == 'r');
		CHECK( tmpEgg.b == 77.44);
		CHECK( tmpEgg.x == 55);

		pSnake = (Snake *)pSnake->next;
		CHECK( pSnake == 0);
	
		// ensure that structure is small and efficient
		CHECK( sizeof(Egg) == 16 );

	// Serialize the medusa

		// create a local buffer
		char buff[ 1024 ];

		// serialize the data
		Gorgon.serialize( buff );
		Gorgon.clear();

	// Recreate a medusa from the serialized data

		// Create a new
		Medusa newGorgon;

		// deserialize the data
		newGorgon.deserialize( buff );

	// validate the data in the new_medusa


		// Testing head snake
		pSnake = (Snake*)newGorgon.getHeadSnake();
		CHECK(pSnake != 0);
		tmpEgg = pSnake->getEgg();
		CHECK(pSnake->getKey() == s3->getKey());
		CHECK(tmpEgg.a == s3->getEgg().a);
		CHECK(tmpEgg.b == s3->getEgg().b);
		CHECK(tmpEgg.x == s3->getEgg().x);

		// Testing 2nd snake
		pSnake = (Snake*)pSnake->next;
		CHECK(pSnake != 0);
		tmpEgg = pSnake->getEgg();
		CHECK(pSnake->getKey() == s2->getKey());
		CHECK(tmpEgg.a == s2->getEgg().a);
		CHECK(tmpEgg.b == s2->getEgg().b);
		CHECK(tmpEgg.x == s2->getEgg().x);

		// Testing 3rd snake
		pSnake = (Snake*)pSnake->next;
		CHECK(pSnake != 0);
		tmpEgg = pSnake->getEgg();
		CHECK(pSnake->getKey() == s1->getKey());
		CHECK(tmpEgg.a == s1->getEgg().a);
		CHECK(tmpEgg.b == s1->getEgg().b);
		CHECK(tmpEgg.x == s1->getEgg().x);

		// Testing last snake
		pSnake = (Snake*)pSnake->next;
		CHECK(pSnake != 0);
		tmpEgg = pSnake->getEgg();
		CHECK(pSnake->getKey() == s0->getKey());
		CHECK(tmpEgg.a == s0->getEgg().a);
		CHECK(tmpEgg.b == s0->getEgg().b);
		CHECK(tmpEgg.x == s0->getEgg().x);

		// Test that the list ends properly
		Snake* ptmpsnake = pSnake;
		pSnake = (Snake*)pSnake->next;
		CHECK(pSnake == 0);

		// Test the prev links
		CHECK(ptmpsnake->prev->prev->prev == (Snake*)newGorgon.getHeadSnake());

		delete s0;
		delete s1;
		delete s2;
		delete s3;
}
TEST_END