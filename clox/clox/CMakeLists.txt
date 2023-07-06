# CMakeList.txt : CMake project for clox, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (clox "main.c"  "common.h" "chunk.h" "memory.h" "memory.c" "chunk.c" "debug.h" "debug.c" "value.h" "value.c" "vm.h" "vm.c" "compiler.h" "compiler.c" "scanner.h" "scanner.c")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET clox PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
