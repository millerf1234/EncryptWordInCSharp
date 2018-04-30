using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptWord_CSharp.Code
{
    public class DriverProgram
    {
        //Constants
        public const int ZERO_SHIFT = 0;
        public const int ONE_SHIFT = 1;
        public const int NEGATIVE_SHIFT = -1;
        public const int SAMPLE_GUESS_VALUE = 13;
        public const int SAMPLE_GUESS_VALUE2 = 21;

        //Private member variables for use in the tests
        private EncryptWord test_WordUnderLengthRequirement; 
        private EncryptWord test_WordWithBothUpperAndLowerCaseLetters;
        private EncryptWord test_WordWithASymbolInIt;
        private EncryptWord test_LongerWord;


        public DriverProgram()
        {
            test_WordUnderLengthRequirement = new EncryptWord("Hi");
            test_WordWithBothUpperAndLowerCaseLetters = new EncryptWord("TeStWoRd");
            test_WordWithASymbolInIt = new EncryptWord("HI!!");
            test_LongerWord = new EncryptWord("Indubitably");
        }


        public void RunTests()
        {
            //Test 1 -- Word below minimum length
            Console.WriteLine("Test 1: Word below minimum length requirement.\n");
            Console.WriteLine("Expecting to get a warning message while trying to print out statistics: ");
            Console.WriteLine(test_WordUnderLengthRequirement.GetStatistics());
            Console.Write("Word's State should be false. This is correct: ");
            Console.WriteLine(test_WordUnderLengthRequirement.State == false);

            //Test 2 -- Word that has both lower and upper case letters
            Console.WriteLine("Test 2: Word With Both UpperCase and LowerCase letters: ");
            Console.WriteLine("Word before Encrypting: \"TeStWoRd\"");
            Console.Write("Word has been encrypted using randomly generated shift of ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.Shift);
            Console.Write("Result of Encryption: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.Word);
            Console.Write("Guessing that shift on word is 13. Result of guess: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.VerifyShift(SAMPLE_GUESS_VALUE));
            Console.WriteLine("Updated Guess Statistics: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.GetStatistics());
            Console.Write("Now Guessing that the shift on the word is 21. Result of guess: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.VerifyShift(SAMPLE_GUESS_VALUE2));
            Console.WriteLine("Updated Guess Statistics: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.GetStatistics());
            Console.Write("Guessing that shift on word is -1. Result of guess: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.VerifyShift(NEGATIVE_SHIFT));
            Console.WriteLine("Updated Guess Statistics: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.GetStatistics());
            Console.Write("Unencrypting Word using method getDecodedWord(). Result: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.GetDecodedWord());
            Console.WriteLine("Testing the toggle state function...");
            test_WordWithBothUpperAndLowerCaseLetters.ToggleState();
            Console.Write("Word's State should be false. This is correct: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.State == false);
            Console.Write("Word is now: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.Word);
            Console.WriteLine("Toggling Encryption again... Results:");
            test_WordWithBothUpperAndLowerCaseLetters.ToggleState();
            Console.Write("Word's State should be false. This is correct: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.State == false);
            Console.Write("Word is now: ");
            Console.WriteLine(test_WordWithBothUpperAndLowerCaseLetters.Word);


            Console.WriteLine("End of testing! Press Enter to quit!");

        }
    }
}
