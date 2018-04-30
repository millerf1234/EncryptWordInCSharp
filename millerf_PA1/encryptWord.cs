using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;  //Gives access to StringWriter

// FILENAME: encryptWord.cs
// AUTHOR: Forrest S Miller
// DATE: April 15, 2018
// REVISION HISTORY: Full implementation completed 4/29/2018
// References: None

// Class Overview:
//      This class defines an object that can be used to store words as strings
//      that have been encrypted using a Ceaser cipher-shift. This object stores
//      the shift that was used to encode it, and can have the shift toggled on
//      or off. This object also allows for guessing on what shift is used to
//      encode it's stored word, and provides the functionality of accepting
//      guesses for what the shift used to encode the word is.
//      Words to be encoded should be at least 4 characters in length, words
//      shorter than this will cause the object to be created into an invalid
//      state.
//
// Assumptions:
//      -Constructor will be called with words that are at least 4 characters
//          long. Calling constructor with a shorter-lengthed word will cause
//          the object to be constructed with state set to false and an empty
//          string.
//      -Strings will consist of only letters (either upper or lower case is
//          fine) with no punctuation or spaces. Punctuation may not be encoded
//          correctly within the cipher.
//      -Negative guess values are allowed and are treated as though the shift
//          was being performed in the opposite direction (i.e. a shift of -4 is
//          the same as a shift of +22).
//      -Calling reset with no parameters will deactivate the object and place
//          it into an invalid state. Calling reset with a string parameter will
//          then reactivate the object with that string, making it valid again.
//          Again, please make sure the string is at least four characters.
//      -Calling reset will reset the objects statistics
//      -Calling any of the accessors will not modify the objects state
//      -To change the word stored within the object, use reset.
//
//Functionality:
//      -This class defines an object that stores a word as a string of at least
//         4 characters in length and encrypts the word using a cipher shift.
//         The value of the shift is an integer, and can either be randomly
//         generated or can be specified. The object also allows for guessing
//         as to what the value of the shift is, and will store statistics
//         regarding guess attempts made. Object will only behave properly if
//         word it stores consists entirely of letters with no punctuation,
//         special characters or spaces.
//
//Legal input:
//         Word must be a string of at least four characters and the characters
//         must be either lower case or upper case letters. Shifts can be either
//         negative or positive. This means multiple shifts coudl be correct
//         (ie a shift of -4 is the same as a shift of 22)
//
//Illegal Input:
//         Strings less than 4 characters long will set the objects state to
//         invalid.
//         Including characters in the string that are not letters will cause
//         undefined/unexpected behavior when the shift is performed.
//
//Output:
//         Each of the getters will return the object member data they are
//         getting. Calling getStatistics will return the statistics as a
//         formated string with labels for each statistic.
//         Has member functions for printing out both the encrypted and
//         unencrypted words
//
//Anticipated use:
//         This object will be used to encompass a word from the English (or 
//         other roman-alphabet-based language) language which will be encrypted
//         using a simple letter-shift encryption. The same encryption will be 
//         used for each letter of the word. There is included functionality 
//         to support processing guesses from a user as to what the encrypted 
//         word is. Guesses will be tracked and statistics recorded.

//NOTE:Namespace name will be updated in later submissions/iterations
namespace EncryptWord_CSharp 
{
    public class EncryptWord
    {
        //---------------------------------------------------------------------
        // Constants
        //---------------------------------------------------------------------
        public const int MINIMUM_WORD_LENGTH = 4;
        public const int LETTERS_IN_ALPHABET = 26;
        public const int INDEX_SHIFT = 1;


        //---------------------------------------------------------------------
        //Private fields 
        //---------------------------------------------------------------------
        private bool constructedWithWordOfProperLength;

        //---------------------------------------------------------------------
        // Public Properties
        //---------------------------------------------------------------------
        public string Word  //The word to be encapsulated by this class in its current encryption state
        {
            get;   //C# will auto-implement these properties apparently
            private set;
        }

        public int Shift //Number of characters used in encryption shift, should be within interval [0,26)
        {
            get;
            private set;
        }

        public bool State //True if shift is on, false if shift is off
        {
            get;
            private set;
        }
        public int NumOfGuesses  
        {
            get;
            private set;
        }

        public int SumOfGuesses
        {
            get;
            private set;
        }

        public int HighestGuess
        {
            get;
            private set;
        }

        public int LowestGuess
        {
            get;
            private set;
        }


        //---------------------------------------------------------------------
        //Constructors 
        //---------------------------------------------------------------------
        private EncryptWord()
        {
        }  //Remove the default constructor by making it private 

        // Constructor that takes a word to be encrypted.
        // Description: Creates a EncryptWord Object from the parameter string. Will
        //              generate a random shift to encrypt the string with and will
        //              automatically encrypt the string with this shift. Will set
        //              the guess-tracking statistics to their default starting
        //              values. Will set the object into a valid state.
        //              Word must be at least 4 letters long or the object will become
        //              invalid.
        public EncryptWord(string word)
        {
            Initialize(word);
            if (!constructedWithWordOfProperLength)
            {
                return;
            }

            //Generate a random shift from between 1 and 25
            Random rand = new Random();
            Shift = rand.Next(INDEX_SHIFT, LETTERS_IN_ALPHABET - INDEX_SHIFT);

            Word = PerformCipherShift(Word);
            State = true;
        }

        //  Constructor that takes a word to be encapsulated and a predetermined shift
        //  value. Shifts greater than 26 will wrap back around, and negative shifts
        //  will shift letters in the reverse direction.
        // Description: Creates a EncryptWord Object from the parameter string. Will
        //              use the specified shift value as the encryption key and will
        //              automatically encrypt the string with this shift. Will set
        //              the guess-tracking statistics to their default starting
        //              values. Will set the object into a valid state.
        public EncryptWord(string word, int shift)
        {
            Initialize(word);
            if (!constructedWithWordOfProperLength)
            {
                return;
            }

            //Put shift in the [0,26) range
            PutValueInAlphabetRange(ref shift);
            Shift = shift;
           /* 
            //if (Shift < 0)
            //{
            //    do
            //    {
            //        Shift += LETTERS_IN_ALPHABET;
            //    } while (Shift < 0);
            //}
            //else if (Shift >= LETTERS_IN_ALPHABET)
            //{
            //    do
            //    {
            //        Shift -= LETTERS_IN_ALPHABET;
            //    } while (Shift >= LETTERS_IN_ALPHABET);
            //} 
            */

            Word = PerformCipherShift(Word);
            State = true;
        }


        

        //---------------------------------------------------------------------
        //Public member functions
        //---------------------------------------------------------------------
        // Description: Returns the encrypted word as a string to the user.
        // Preconditions: Object must be in a valid state. Calling this method on an
        //               object in an invalid state will cause bad things to happen.
        // Postconditions: This is constant, so the state of the object will not
        //                 change.
        public string GetEncryptedWord()
        {
            if (State)
            {
                return Word;
            }
            else
            {
                return PerformCipherShift(Word);
            }
        }


        // Description: Returns the decoded word stored within the object
        // Preconditions: Object must be in valid state.
        // Postconditions: Object will remain in same state as it was when this
        //                 getter was called.
        public string GetDecodedWord()
        {
            if (State)
            {
                return UndoCipherShift(Word);
            }
            else
            {
                return Word;
            }
        }

        // Description: Flips the state of the object, turning on/off the cipher
        // Preconditions: Object must be in a valid state
        // Postconditions: Object's cipher-shift will be the opposite of what is was
        //                  before this operation.
        public void ToggleState()
        {
            State = !State; //Toggle state
            if (!constructedWithWordOfProperLength)
            {
                return;
            }
            if (State)
            {
                //State was off before this method was called
                Word = PerformCipherShift(Word);//So turn the encryption on
            }
            else
            {
                //The State was set to on before this method was called
                Word = UndoCipherShift(Word); //So undo the shift to turn encryption off
            }
        }

        // Description: Returns the current guess statistics stored on the object
        // Preconditions: Object must be in a valid state
        // Postconditions: None, member function is constant
        public string GetStatistics() {
            if (!constructedWithWordOfProperLength)
            {
                return "Error! Word provided to this object was shorter than 4 letters!\n";
            }
            //see: https://msdn.microsoft.com/en-us/library/system.io.stringwriter.aspx
            StringWriter sw = new StringWriter();
            sw.WriteLine("Your Guess Statistics thus far: ");
            sw.Write("Number of guesses: ");
            sw.WriteLine(NumOfGuesses);
            sw.Write("Highest Guess: ");
            //What I would like to do:
            //(NumOfGuesses > 0) ? sw.WriteLine(HighestGuess) : sw.WriteLine("Not Applicable");
            //How I am being forced to write it:
            if (NumOfGuesses > 0)
            {
                sw.WriteLine(HighestGuess);
            }
            else
            {
                sw.WriteLine("Not Applicable");
            }
            sw.Write("Lowest Guess: ");
            //(NumOfGuesses > 0) ? sw.WriteLine(HighestGuess) : sw.WriteLine("Not Applicable");
            if (NumOfGuesses > 0)
            {
                sw.WriteLine(LowestGuess);
            }
            else
            {
                sw.WriteLine("Not Applicable");
            }

            sw.Write("Average Guess: ");

            if (NumOfGuesses > 0) //Check to make sure we don't divide by 0
            {
                //Will this do integer or floating-point division? 
                double averageGuess = SumOfGuesses / NumOfGuesses; //Hopefully not integer division...
                sw.WriteLine(averageGuess);
            }
            else
            {
                sw.WriteLine(0.0f);
            }

            return (sw.ToString());
        }

        // Description: Used to verify if the integer value parameter is equal to
        //              the shift used to encode the object. Will record guess to
        //              the objects guess statistics. Negative guesses are allowed
        //              and are treated as shifts in the reverse direction.
        // Preconditions: Object must be in the valid state
        // Postconditions: Objects guess statistics will be updated based off the
        //                 guess
        public bool VerifyShift(int guess)
        {
            if (!constructedWithWordOfProperLength)
            {
                return false;
            }
            //Add the guess to the statistics
            AddValueToGuesses(guess);

            //Put the guess in the same interval as Shift to do comparison
            PutValueInAlphabetRange(ref guess);

            if (Shift == guess)
            {
                return true;
            } 
            return false;
        }



        //---------------------------------------------------------------------
        //Private methods (helper functions)
        //---------------------------------------------------------------------

        //Helper function for constructors
        private void Initialize(string word)
        {
            //Initialize statistics
            NumOfGuesses = 0;
            HighestGuess = 0;
            LowestGuess = 0;
            SumOfGuesses = 0;

            if (word.Length < MINIMUM_WORD_LENGTH)
            {
                constructedWithWordOfProperLength = false;
                return;
            }
            else
            {
                constructedWithWordOfProperLength = true;
            }
            //Initialize the rest of the object
            Word = word;
            Shift = 0;
            State = false;
        }

        //Uses the value stored in Shift. 
        private string PerformCipherShift(string s)
        {
            //For each letter in s
            for (int i = 0; i < s.Length; i++ )
            {
                char c = s[i];
                if (char.IsLetter(c))
                {
                    if (char.IsLower(c))
                    {
                        c = (char)((int)c + Shift);
                        if (c > 'z')
                        {
                            c = (char) ((int)c - LETTERS_IN_ALPHABET);
                        }
                    }
                    else //c must be an upper case letter
                    {
                        c = (char)((int)c + Shift);
                        if (c > 'Z')
                        {
                            c = (char)((int)c - LETTERS_IN_ALPHABET);
                        }
                    }
                }
            }
            return s;
        }

        private string UndoCipherShift(string s) //Undoes encryption using the shift value stored in this object
        {
            //For each letter in s
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsLetter(c))
                {
                    if (char.IsLower(c))
                    {
                        c = (char)((int)c - Shift);
                        if (c < 'a')
                        {
                            c = (char)((int)c + LETTERS_IN_ALPHABET);
                        }
                    }
                    else //c must be an upper case letter
                    {
                        c = (char)((int)c - Shift);
                        if (c > 'Z')
                        {
                            c = (char)((int)c + LETTERS_IN_ALPHABET);
                        }
                    }
                }
            }
            return s;
        }

        private bool CheckLengthRequirement(string s)
        {
            if (s.Length < MINIMUM_WORD_LENGTH)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AddValueToGuesses(int guess)
        {
            //Add the guess to this objects guess-tracking statistics. 
            if (NumOfGuesses == 0)
            {
                HighestGuess = guess;
                LowestGuess = guess;
                SumOfGuesses = guess;
            }
            else //There was already a guess attempt on this word
            {
                if (HighestGuess < guess)
                {
                    HighestGuess = guess;
                }
                else if (LowestGuess > guess)
                {
                    LowestGuess = guess;
                }
                SumOfGuesses += guess;
                
            }
            ++NumOfGuesses; //Incremnent the number of guesses
        }

        private void PutValueInAlphabetRange(ref int value) //Puts that value in the range of [0,26)
        {
            if (value < 0)
            {
                do
                {
                    value += LETTERS_IN_ALPHABET;
                } while (value < 0);
            }
            else if (value >= LETTERS_IN_ALPHABET)
            {
                do
                {
                    value -= LETTERS_IN_ALPHABET;
                } while (value >= LETTERS_IN_ALPHABET);
            }
        }
    } //End class EncryptWord
}; //end namespace EncryptWord_CSharp


















/*  C++ HEADER FILE FOR ENCRYPT WORD
   
// Author: Forrest Miller
// Filename: EncryptWord.h
// Date: September 28, 2017
// Version: 1.0
//
//
// Class Overview:
//      This class defines an object that can be used to store words as strings
//      that have been encrypted using a Ceaser cipher-shift. This object stores
//      the shift that was used to encode it, and can have the shift toggled on
//      or off. This object also allows for guessing on what shift is used to
//      encode it's stored word, and provides the functionality of accepting
//      guesses for what the shift used to encode the word is.
//      Words to be encoded should be at least 4 characters in length, words
//      shorter than this will cause the object to be created into an invalid
//      state.
//
// Assumptions:
//      -Constructor will be called with words that are at least 4 characters
//          long. Calling constructor with a shorter-lengthed word will cause
//          the object to be constructed with state set to false and an empty
//          string.
//      -Strings will consist of only letters (either upper or lower case is
//          fine) with no punctuation or spaces. Punctuation may not be encoded
//          correctly within the cipher.
//      -Negative guess values are allowed and are treated as though the shift
//          was being performed in the opposite direction (i.e. a shift of -4 is
//          the same as a shift of +22).
//      -Calling reset with no parameters will deactivate the object and place
//          it into an invalid state. Calling reset with a string parameter will
//          then reactivate the object with that string, making it valid again.
//          Again, please make sure the string is at least four characters.
//      -Calling reset will reset the objects statistics
//      -Calling any of the accessors will not modify the objects state
//      -To change the word stored within the object, use reset.
//
//Functionality:
//      -This class defines an object that stores a word as a string of at least
//         4 characters in length and encrypts the word using a cipher shift.
//         The value of the shift is an integer, and can either be randomly
//         generated or can be specified. The object also allows for guessing
//         as to what the value of the shift is, and will store statistics
//         regarding guess attempts made. Object will only behave properly if
//         word it stores consists entirely of letters with no punctuation,
//         special characters or spaces.
//
//Legal input:
//         Word must be a string of at least four characters and the characters
//         must be either lower case or upper case letters. Shifts can be either
//         negative or positive. This means multiple shifts coudl be correct
//         (ie a shift of -4 is the same as a shift of 22)
//
//Illegal Input:
//         Strings less than 4 characters long will set the objects state to
//         invalid.
//         Including characters in the string that are not letters will cause
//         undefined/unexpected behavior when the shift is performed.
//
//Output:
//         Each of the getters will return the object member data they are
//         getting. Calling getStatistics will return the statistics as a
//         formated string with labels for each statistic.
//         Has member functions for printing out both the encrypted and
//         unencrypted words

#ifndef EncryptWord_h
#define EncryptWord_h

#include <iostream>

class EncryptWord {
public:
    EncryptWord(std::string);
    // Description: Creates a EncryptWord Object from the parameter string. Will
    //              generate a random shift to encrypt the string with and will
    //              automatically encrypt the string with this shift. Will set
    //              the guess-tracking statistics to their default starting
    //              values. Will set the object into a valid state.
    // Preconditions: string must be at least 4 characters long. Strings less
    //                than this length will place the object into an invalid
    //                state. String should only consist of upper and lower case
    //                letters with no special punctuation or spaces (i.e. A-Z or
    //                a-z).
    // Postconditions: Object will be in a valid state. Guess statistics will be
    //                 set to their default value. Object will contain word
    //                 stored in its encrypted format.
    //                 If Object was created with a word less than 4 characters
    //                 in length, this objects state will start out as false.
    //
    EncryptWord(std::string, int);
    // Description: Creates a EncryptWord Object from the parameter string. Will
    //              use the specified shift value as the encryption key and will
    //              automatically encrypt the string with this shift. Will set
    //              the guess-tracking statistics to their default starting
    //              values. Will set the object into a valid state.
    // Preconditions: string must be at least 4 characters long. Strings less
    //                than this length will place the object into an invalid
    //                state. String should only consist of upper and lower case
    //                letters with no special punctuation or spaces (i.e. A-Z or
    //                a-z).
    // Postconditions: Object will be in a valid state. Guess statistics will be
    //                 set to their default value. Object will contain word
    //                 stored in its encrypted format.
    //                 If Object was created with a word less than 4 characters
    //                 in length, this objects state will start out as false.

    
    ~EncryptWord(void);
    // Description: Destrutor
    // Preconditions: Object exists
    // Postconditions: Object will no longer exist
    
    bool getState(void) const;
    // Description: Returns 1 if object is in a valid state, returns 0 if object
    //              is in an invalid state.
    // Preconditions: The object must exist
    // Postconditions: None, this member function is constant
    
    int getShift(void) const;
    // Description: Returns the current shift used to encode this objects word.
    // Preconditions: The object must exist
    // Postconditions: None, this member function is constant
    
    std::string getWord(void) const;
    // Description: Returns the word stored within the object
    // Preconditions: The object must be in a valid state
    // Postconditions: None, this member function is constant
    
    void reset(void);
    // Description: Flips the state of the object, turning on/off the cipher
    // Preconditions: Object must be in a valid state
    // Postconditions: Object's cipher-shift will be the opposite of what is was
    //                  beofre this 
    
    std::string getStatistics(void) const;
    // Description: Returns the current guess statistics stored on the object
    // Preconditions: Object must be in a valid state
    // Postconditions: None, member function is constant
    
    bool verifyShift(int);
    // Description: Used to verify if the integer value parameter is equal to
    //              the shift used to encode the object. Will record guess to
    //              the objects guess statistics. Negative guesses are allowed
    //              and are treated as shifts in the reverse direction.
    // Preconditions: Object must be in the valid state
    // Postconditions: Objects guess statistics will be updated based off the
    //                 guess
    
private:
    //The word to be encapsulated by this class
    std::string word;
    //Number of characters to shift, should be within interval [0,26)
    int shift;
    bool state; //True if shift is on, false if shift is off
    
    //Statistics about querrys/guesses:
    //Number of guesses
    int numOfGuesses;
    //Highest Guess
    int highestGuess;
    //Lowest Guess
    int lowestGuess;
    //Sum of gueses (for computing avarage)
    int sumOfGuesses;
    //AvarageGuessValue
    float averageGuessValue;
    
    std::string getEncryptedWord(void) const;
    // Description: Returns the encrypted word as a string to the user.
    // Preconditions: Object must be in a valid state. Calling this method on an
    //               object in an invalid state will cause bad things to happen.
    // Postconditions: This is constant, so the state of the object will not
    //                 change.
    
    std::string getDecodedWord(void);
    // Description: Returns the decoded word stored within the object
    // Preconditions: Object must be in valid state.
    // Postconditions: Object will remain in same state as it was when this
    //                 getter was called.
    
    std::string& performCipherShift(std::string&);
    std::string& undoCipherShift(std::string&);
    bool checkLengthRequirement(std::string const&);
    void addValueToGuesses(int);
};

#endif */  /* EncryptWord_h */
