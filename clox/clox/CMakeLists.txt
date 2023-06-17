# CMakeList.txt : CMake project for clox, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (clox "clox.cpp"  "headers/chunk.h" "headers/clox.h" "headers/common.h" "headers/compiler.h" "headers/debug.h" "headers/memory.h" "headers/object.h" "headers/scanner.h" "headers/value.h" "headers/vm.h" "chunk.cpp" "compiler.cpp" "debug.cpp" "memory.cpp" "object.cpp" "scanner.cpp" "value.cpp" "vm.cpp" "headers/table.h" "table.cpp")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET clox PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
