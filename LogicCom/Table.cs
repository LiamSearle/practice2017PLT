// Handle variable and function tables for LogicCom level 1 compiler (C# version)
// P.D. Terry, Rhodes University, 2017

// Skeleton only - as supplied 24 hours before the November examination

// These are just suggestions; please yourself.  But the Types and Kinds
// classes are those used in the Parva compiler and should be left untouched.
//
// Entities are all declared public for ease of referring to them.

using Library;
using System.Collections.Generic;

namespace LogicCom {

  class Types {
  // Identifier (and expression) types.

    public const int
      noType   =  0,
      nullType =  2,
      intType  =  4,
      boolType =  6,
      voidType =  8;
  } // end Types


  class Kinds {
  // Identifier kinds

    public const int
      Con  = 0,
      Var  = 1,
      Fun  = 2;
  } // end Kinds


  class ConstRec {
  // Objects of this type are associated with literal constants

    public int value;            // value of a constant literal
    public int type;             // constant type (determined from the literal)
  } // end ConstRec

  class Entry {

    public const int
      global = 0,                    // declaration levels
      local  = 1;

    public char name;
    public int  offset;
    public int  level;
    // ....... other fields as you see fit, public for simplicity

    public Entry(char name, int level, int offset /* other parameters as you see fit */ ) {
      this.name   = name;
      this.offset = offset;
      this.level  = level;

      //  ....

    } // constructor

    public override string ToString() {

      //  ....

      return null; // Dummy

    } // ToString

  } // Entry


  class Table {
  // Symbol tables for single letter variables and parameters

  // It is suggested that you instantiate this class afresh for each function definition as a simple
  // way of handling its scope rules

    List<Entry> varList = new List<Entry>();

    public Entry Find(char name) {
    // Searches table for an entry matching name.
    // If found then returns the corresponding entry

      //  ....

      return null; // Dummy

    } // Table.Find

    public void Add( /*  suitable parameters */ ) {
    // Adds an entry, computing its offset in a stack frame

      //  ....

    } // Table.Add

    public void PrintTable(OutFile lst) {
    // Prints symbol table for diagnostic purposes

      //  ....

    } // Table.PrintTable

  } // end Table


  class FunEntry {
  // Symbol table entries for function definitions


    public string name;

    // ....... other fields as you see fit, public for simplicity

    public FunEntry(string name   /* other parameters as you see fit */    ) {
      this.name       = name;

      //  ....

    } // constructor

    public override string ToString() {

      //  ....

      return null; // Dummy

    } // FunEntry.ToString

  } // FunEntry;


  class FunTable {
  // Symbol table for functions

    static List<FunEntry> funList = new List<FunEntry>();

    public static void Init() {

      // ........

    } // FunTable.Init

    public static FunEntry FindEntry(string name) {
    // Searches table for an entry matching name.
    // If found then return the corresponding entry

      // ........

      return null; // Dummy

    } // FunEntry.FindEntry

    public static void AddFun(FunEntry entry) {
    // Adds an entry to the function table

       // .......

    } // FunTable.AddFun

    public static void PrintTable(OutFile lst) {
    // Prints symbol table for diagnostic purposes

       // .......

    } // FunTable.PrintTable

  } // end FunTable

} // namespace
