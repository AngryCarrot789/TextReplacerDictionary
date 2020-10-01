# TextReplacerDictionary
A program for mass replacing multiple Keys with Values, in multiple files. (Currently still WIP)

## Example uses
- Can be used if you're decompiling/deobfuscating code. Sometimes you might see 100s of functions like "func37548_ad" in multiple files, 
but if you know what that function actually does, you could use the app to replace "func37548_ad" (or any other function like that) with an easy to read function name.
And this program can replace more than 1 function name at a time, in more than 1 file, hense "Mass Replacing".

# How to use

if you have a file containing Keys and Values (aka old words and new words to replace, and all the pairs are separated by new lines), at the bottom click the ... button, select the dictionary path, put in the character that separates the Keys from Values, which is usually a whitespace/space, and then click Load and it will add all the items in the list below. you can add items manually at the top. Add in the old text in Replace, and the new text in With and click add. it will add an item in the list below.
On top of that list are checkboxes; Case Sensitive and Match whole case. Case Sensitive will pay attention to lowercase and uppercase letters. Match whole words will only search if the word is on it's own (aka inbetween numbers or whitespaces). so searching in "hello there" for "ell" will return false.

then on the right at the top is your files. You can either add single/multiple files with the top button, or open an entire folder with the button below (also an option to recursively search through subfolders too by checking the checkbox on the right side).

then at the bottom click the green start. to the right tells you how many files are to be searched and how many searches are going to be done per file, and how many files are left to be searched. 

Lots of things are done asynchronously so the UI hopefully shouldn't freeze, however if loading 1000s of files/dictionary pairs, it defintely might take a few seconds (mainly due to the creation of UI elements and adding them to the collection).
