using System;
using System.Collections.Generic;
using System.Linq;

// Requirement: console app that finds the Missing Number using SOLID principles
//   S = Single Responsibility Principle: Each class has one reason to change
//   O = Open/Closed Principle: Open for extension, closed for modification
//   L = Liskov Substitution Principle: Any implementation can be substituted. 
//   I = Interface Segregation Principle: Clients depend only on interfaces they use
//   D = Dependency Inversion Principle: High-level modules don't depend on low-level modules
//
// Responsibilities separated into:
//  - Console application         => Orchestrates the application flow
//  - User interaction            => Handles user interaction
//  - Validation                  => Validates input
//  - Missing number calculation  => Calculates the missing number

// Interface for finding missing numbers - allows for different algorithms
public interface IMissingNumberFinder
{
    int FindMissingNumber(IEnumerable<int> numbers);
}

// Liskov Substitution Principle: Any implementation can be substituted. 
// There is only one implementation here, but we could add more.
// Solution concept: Input is already validated. Numbers are distinct and in range [0, n].
// Total Sum of 0 to n = n*(n+1)/2. Every number from 0 to n contributes to the total sum. 
// When one number is missing, the sum is reduced by exactly that amount. 
// By calculating what the sum should be and subtracting what it actually is, we get the missing number directly.
public class MathematicalMissingNumberFinder : IMissingNumberFinder
{
    public int FindMissingNumber(IEnumerable<int> numbers)
    {
        if (numbers == null)
            throw new ArgumentNullException(nameof(numbers));

        var numArray = numbers.ToArray();
        int n = numArray.Length; // Since one number is missing, n = actualLength
        
        // Expected sum of numbers from 0 to n
        int expectedSum = n * (n + 1) / 2;
        
        // Actual sum of numbers in array
        int actualSum = numArray.Sum();
        
        return expectedSum - actualSum;
    }
}

// Interface Segregation Principle: Clients depend only on interfaces they use
public interface IInputValidator
{
    bool IsValidInput(IEnumerable<int> numbers);
    string GetValidationError(IEnumerable<int> numbers);
}

// Dependency Inversion Principle: High-level modules don't depend on low-level modules
public class InputValidator : IInputValidator
{
    public bool IsValidInput(IEnumerable<int> numbers)
    {
        if (numbers == null) return false;
        
        var numArray = numbers.ToArray();
        if (numArray.Length == 0) return false;
        
        int n = numArray.Length;
        var distinctNumbers = numArray.Distinct().ToArray();
        
        // Check if all numbers are distinct
        if (distinctNumbers.Length != numArray.Length) return false;
        
        // Check if all numbers are within valid range [0, n]
        return distinctNumbers.All(num => num >= 0 && num <= n);
    }
    
    public string GetValidationError(IEnumerable<int> numbers)
    {
        if (numbers == null) return "Input array cannot be null";
        
        var numArray = numbers.ToArray();
        if (numArray.Length == 0) return "Input array cannot be empty";
        
        int n = numArray.Length;
        var distinctNumbers = numArray.Distinct().ToArray();
        
        if (distinctNumbers.Length != numArray.Length) 
            return "All numbers must be distinct";
        
        if (distinctNumbers.Any(num => num < 0 || num > n))
            return $"All numbers must be in range [0, {n}]";
            
        return string.Empty;
    }
}

// Single Responsibility: Handles user interaction
public interface IUserInterface
{
    int[] GetNumbersFromUser();
    void DisplayResult(int missingNumber);
    void DisplayError(string error);
    bool AskToContinue();
}

public class ConsoleUserInterface : IUserInterface
{
    public int[] GetNumbersFromUser()
    {
        Console.WriteLine("Enter numbers separated by spaces (e.g., 3 0 1):");
        string input = Console.ReadLine() + "";
        
        try
        {
            return input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                       .Select(int.Parse)
                       .ToArray();
        }
        catch (FormatException)
        {
            throw new FormatException("Invalid input format. Please enter integers separated by spaces.");
        }
    }
    
    public void DisplayResult(int missingNumber)
    {
        Console.WriteLine($"The missing number is: {missingNumber}");
    }
    
    public void DisplayError(string error)
    {
        Console.WriteLine($"Error: {error}");
    }
    
    public bool AskToContinue()
    {
        Console.WriteLine("\nDo you want to try another array? (y/n):");
        string response = (Console.ReadLine() ?? "").Trim().ToLower();
        return response == "y" || response == "yes";
    }
}

// Single Responsibility: Orchestrates the application flow
public class MissingNumberApplication
{
    private readonly IMissingNumberFinder _finder;
    private readonly IInputValidator _validator;
    private readonly IUserInterface _userInterface;
    
    // Dependency Inversion: Depends on abstractions, not concretions
    public MissingNumberApplication(
        IMissingNumberFinder finder, 
        IInputValidator validator,
        IUserInterface userInterface)
    {
        _finder = finder ?? throw new ArgumentNullException(nameof(finder));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
    }
    
    public void Run()
    {
        Console.WriteLine("Missing Number Finder");
        Console.WriteLine("====================");
        Console.WriteLine("Find the missing number in an array containing n distinct numbers from range [0, n]");
        Console.WriteLine();
        
        do
        {
            try
            {
                var numbers = _userInterface.GetNumbersFromUser();
                
                if (!_validator.IsValidInput(numbers))
                {
                    _userInterface.DisplayError(_validator.GetValidationError(numbers));
                    continue;
                }
                
                int missingNumber = _finder.FindMissingNumber(numbers);
                _userInterface.DisplayResult(missingNumber);
            }
            catch (Exception ex)
            {
                _userInterface.DisplayError(ex.Message);
            }
        } 
        while (_userInterface.AskToContinue());
        
        Console.WriteLine("Thank you for using Missing Number Finder!");
    }
}

// Composition Root - where dependencies are wired up
public class Program
{
    public static void Main(string[] args)
    {
        // Dependency injection - can easily swap implementations
        IMissingNumberFinder finder = new MathematicalMissingNumberFinder();
        // Alternative: IMissingNumberFinder finder = new XorMissingNumberFinder();
        
        IInputValidator validator = new InputValidator();
        IUserInterface userInterface = new ConsoleUserInterface();
        
        var application = new MissingNumberApplication(finder, validator, userInterface);
        application.Run();
    }
}