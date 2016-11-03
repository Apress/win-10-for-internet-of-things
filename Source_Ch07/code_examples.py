#
# Windows 10 for the IOT
#
# Example Python application.
#
# Created by Dr. Charles Bell
#
def print_dictionary(the_dictionary):
  for key, value in the_dictionary.items():
    print("'{0}': {1}".format(key, value))

zip = 35012                # Zip or postal code
address1 = "123 Main St."  # Store the street address

# Numbers
float_value = 9.75
integer_value = 5

# Strings
my_string = "He says, he's already got one."

print("Floating number: {0}".format(float_value))
print("Integer number: {0}".format(integer_value))
print(my_string)

# List
my_list = ["abacab", 575, "rex, the wonder dog", 24, 5, 6]
my_list.append("end")
my_list.insert(0,"begin")
for item in my_list:
  print("{0}".format(item))

# Tuple
my_tuple = (0,1,2,3,4,5,6,7,8,"nine")
for item in my_tuple:
  print("{0}".format(item))
if 7 in my_tuple:
  print("7 is in the list")

# Dictionary
my_dictionary = {
  'first_name': "Chuck",
  'last_name': "Bell",
  'age': 36,
  'my_ip': (192,168,1,225),
  42:"what is the meaning of life?",
}
# Access the keys:
print(my_dictionary.keys())
# Access the items (key, value) pairs
print(my_dictionary.items())
# Access the values
print(my_dictionary.values())
# Create a list of dictionaries
my_addresses = [my_dictionary]
print_dictionary(my_dictionary)
print("Goodbye!")
