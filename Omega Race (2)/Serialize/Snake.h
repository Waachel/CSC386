//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#ifndef SNAKE_H
#define SNAKE_H

struct Egg
{
	Egg( ) = default;
	Egg( char inA, double inB, int inX );
	Egg(const Egg &) = default;
	Egg & operator = (const Egg &) = default;
	~Egg() = default;

	// data
	double	b; //8
	int		x; //4
	char    a; //1
};


class SnakeLink
{
public:
	SnakeLink();
	SnakeLink(const SnakeLink &) = default;
	SnakeLink & operator = (const SnakeLink &) = default;
	~SnakeLink() = default;

	// data
	SnakeLink *next;
	SnakeLink *prev;
};


class Snake: public SnakeLink
{
public:
	
	// constructors
	Snake() = default;
	Snake( unsigned int key, char inA, double inB, int inX);
	Snake(const Snake &) = default;

	// destructor
	~Snake();

	// assignment
	Snake & operator = (const Snake &) = default;

	// accessors 
	const unsigned int		getKey() const;
	const Egg &		getEgg() const;

	// Read from a buffer
	void deserialize( const char * const buffer );

	// Write object to a buffer
	void serialize( char * const buffer ) const;

private:
	// data to serialize
	unsigned int	key;
	Egg 			*pEgg;
};


class Medusa
{
public:
	Medusa();
	Medusa(const Medusa &) = default;
	Medusa & operator = (const Medusa &) = default;
	~Medusa();
	
	// add a snake to the beginning of list
	void insertSnake( Snake &inSnake );
	
	// Accessors
	const SnakeLink *getHeadSnake( ) const;

	void clear();

	// Read from a buffer
	void deserialize( const char * const buffer );

	// Write object to a buffer
	void serialize( char * const buffer ) const;

private:
	SnakeLink *head;

};

#endif