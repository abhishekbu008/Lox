// clox.cpp : Defines the entry point for the application.
//

#include "headers/clox.h"
#include "headers/common.h"
#include "headers/chunk.h"
#include "headers/debug.h"

using namespace std;

int main(int argc, const char* argv[])
{
	Chunk chunk;
	initChunk(&chunk);

	int constant = addConstant(&chunk, 1.2);
	writeChunk(&chunk, OP_CONSTANT, 123);
	writeChunk(&chunk, constant, 123);

	writeChunk(&chunk, OP_RETURN, 123);

	disassembleChunk(&chunk, "test chunk");
	freeChunk(&chunk);
	return 0;
}
