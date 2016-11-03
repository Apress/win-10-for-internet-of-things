#include "myclass.h"

MyClass::MyClass() {
  num = 0;
}

int MyClass::get_num() {
  return num;
}

void MyClass::inc() {
  ++num;
}

void MyClass::dec() {
  --num;
}