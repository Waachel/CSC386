//-----------------------------------------------------------------------------
//
// Real-Time Networking
//----------------------------------------------------------------------------- 

#ifndef TEST_DATA_H
#define TEST_DATA_H

#include <list>

// Source data, its easier to define this way instead of push functions
// NOTE: try some different data values to shake out system
int initData[] =
{
	40,
	20,
	90,
	100,
	2221,
	403,
	202,
	920,
	150,
	2100,
	43,
	22,
	9,
	402,
	1,
	2,
	76,
	324,
	16,
	14,
	8993,
	401,
	37,
	26,
	28,
	27,
	453,
	9011,
	865,
	211
};

// Trick to automagically count number of elements
#define INIT_DATA_SIZE (sizeof(initData)/sizeof(initData[0]))

// test data: Remember the data for grading will be different, values and number of elements
//            No hard coding

class TestData
{
public:
	// initialize the STL list
	TestData()
	{
		this->NodeList.assign(initData, initData + INIT_DATA_SIZE);
	}

	// accessor
	std::list<int>& getNodeList()
	{
		return NodeList;
	}

private:
	std::list<int> NodeList;
};


#endif