#include <iostream>
#include "myclass.h"

using namespace std;

int main(int argc, char **argv) {
  MyClass c = MyClass();
  c.inc();
  c.inc();
  cout << "contents of myclass: " << c.get_num() << "\n";
}

