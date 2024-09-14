#include "CTFField.h"
#include <iostream>

const char CTFField::Title[] = "Capture The Flag";

CTFField::CTFField()
{
	for (int j = 0; j < FieldHeight; j++) 
	{
		for (int i = 0; i < FieldWidth; i++) 
		{
			Field[j][i] = NoOneMarker;
		}
	}
}

void CTFField::Display()
{
	// Title 
	std::cout << "\t" << std::string(1 + (FieldWidth + 2 - sizeof(Title)) / 2, ' ') << Title << std::endl;

	// Field
	std::cout << "\t" << std::string(FieldWidth + 2, BorderMarker) << std::endl;
	for (int j = 0; j < FieldHeight; j++) // each line
	{
		std::cout << "\t" << BorderMarker << std::string(Field[j], FieldWidth) << BorderMarker << std::endl;
	}
	std::cout << "\t" << std::string(FieldWidth + 2, BorderMarker) << std::endl;
}