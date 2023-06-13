#include <stdio.h>

#include "headers/common.h"
#include "headers/compiler.h"
#include "headers/scanner.h"

void compile(const char* source) {
	initScanner(source);
	int line = -1;
	for (;;) {
		Token token = scanToken();
		if (token.line != line) {
			printf("4%d ", token.line);
			line = token.line;
		}
		else {
			printf("    | ");
		}
		printf("%2d '%.*s'\n", token.type, token.length, token.start);
		if (token.type == TOKEN_EOF) break;
	}
}