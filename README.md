# Requirement
In C# create a console app that finds the Missing Number using SOLID principles
Description: Given an array containing numbers taken from the range 0 to n, find the one number that is missing from the array.

Input:
An array of integers number where nums contains n distinct numbers from the range 0 to n.

Output:
Return the missing number.

Examples:
Input: [3, 0, 1]
Output: 2

Input: [9, 6, 4, 2, 3, 5, 7, 0, 1]
Output: 8

# SOLID Principles Applied:
1. Single Responsibility Principle (SRP):

MathematicalMissingNumberFinder: Only responsible for finding missing numbers using math
InputValidator: Only responsible for validating input
ConsoleUserInterface: Only responsible for user interaction
MissingNumberApplication: Only responsible for orchestrating the flow

2. Open/Closed Principle (OCP):

Easy to add new missing number algorithms by implementing IMissingNumberFinder
Can add new UI types by implementing IUserInterface
Existing code doesn't need modification when extending

3. Liskov Substitution Principle (LSP):

Any IMissingNumberFinder implementation can replace another without breaking functionality

4. Interface Segregation Principle (ISP):

IMissingNumberFinder: Focused only on finding missing numbers
IInputValidator: Focused only on validation
IUserInterface: Focused only on user interaction
No client is forced to depend on methods it doesn't use

5. Dependency Inversion Principle (DIP):

MissingNumberApplication depends on abstractions (IMissingNumberFinder, IInputValidator, IUserInterface)
Dependencies are injected through constructor
High-level modules don't depend on low-level implementation details


Features:
Algorithm: Mathematical (sum formula)
Input validation: Ensures array contains distinct numbers in valid range
Error handling: Graceful handling of invalid inputs and exceptions
Interactive console: User-friendly interface with option to try multiple arrays
Extensible design: Easy to add new algorithms or UI types