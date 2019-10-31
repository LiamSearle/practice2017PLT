using Library;



using System;
using System.IO;
using System.Text;

namespace BoolCalc {

public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _variable = 2;
	// terminals
	public const int EOF_SYM = 0;
	public const int number_Sym = 1;
	public const int variable_Sym = 2;
	public const int equal_Sym = 3;
	public const int write_Sym = 4;
	public const int lparen_Sym = 5;
	public const int comma_Sym = 6;
	public const int rparen_Sym = 7;
	public const int read_Sym = 8;
	public const int semicolon_Sym = 9;
	public const int or_Sym = 10;
	public const int and_Sym = 11;
	public const int equalequal_Sym = 12;
	public const int bangequal_Sym = 13;
	public const int not_Sym = 14;
	public const int true_Sym = 15;
	public const int false_Sym = 16;
	public const int NOT_SYM = 17;
	// pragmas
	public const int CodeOn_Sym = 18;
	public const int CodeOff_Sym = 19;
	public const int DebugOn_Sym = 20;
	public const int DebugOff_Sym = 21;

	public const int maxT = 17;
	public const int _CodeOn = 18;
	public const int _CodeOff = 19;
	public const int _DebugOn = 20;
	public const int _DebugOff = 21;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;

	public static Token token;    // last recognized token   /* pdt */
	public static Token la;       // lookahead token
	static int errDist = minErrDist;

	static bool[] mem = new bool[26];




	static void SynErr (int n) {
		if (errDist >= minErrDist) Errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public static void SemErr (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void SemError (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void Warning (string msg) { /* pdt */
		if (errDist >= minErrDist) Errors.Warn(token.line, token.col, msg);
		errDist = 2; //++ 2009/11/04
	}

	public static bool Successful() { /* pdt */
		return Errors.count == 0;
	}

	public static string LexString() { /* pdt */
		return token.val;
	}

	public static string LookAheadString() { /* pdt */
		return la.val;
	}

	static void Get () {
		for (;;) {
			token = la; /* pdt */
			la = Scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }
				if (la.kind == CodeOn_Sym) {
				}
				if (la.kind == CodeOff_Sym) {
				}
				if (la.kind == DebugOn_Sym) {
				}
				if (la.kind == DebugOff_Sym) {
				}

			la = token; /* pdt */
		}
	}

	static void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}

	static bool StartOf (int s) {
		return set[s, la.kind];
	}

	static void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}

	static bool WeakSeparator (int n, int syFol, int repFol) {
		bool[] s = new bool[maxT+1];
		if (la.kind == n) { Get(); return true; }
		else if (StartOf(repFol)) return false;
		else {
			for (int i=0; i <= maxT; i++) {
				s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
			}
			SynErr(n);
			while (!s[la.kind]) Get();
			return StartOf(syFol);
		}
	}

	static void BoolCalc() {
		int index = 0; bool value = false;
		for (int i = 0; i < 26; i++)
		{
		
		    mem[i]=false;
		    Console.WriteLine(mem[i]);
		}
		
		while (la.kind == variable_Sym || la.kind == write_Sym || la.kind == read_Sym) {
			if (la.kind == variable_Sym) {
				Variable(out index);
				ExpectWeak(equal_Sym, 1);
				Expression(out value);
				mem[index] = value;
			} else if (la.kind == write_Sym) {
				Get();
				Expect(lparen_Sym);
				Expression(out value);
				IO.WriteLine(value);
				while (la.kind == comma_Sym) {
					Get();
					Expression(out value);
					IO.WriteLine(value);
				}
				Expect(rparen_Sym);
			} else {
				Get();
				Expect(lparen_Sym);
				Variable(out index);
				mem[index] = IO.ReadBool();
				Expect(rparen_Sym);
			}
			while (!(la.kind == EOF_SYM || la.kind == semicolon_Sym)) {SynErr(18); Get();}
			Expect(semicolon_Sym);
		}
		Expect(EOF_SYM);
	}

	static void Variable(out int index) {
		Expect(variable_Sym);
		index = token.val[0] - 'a';
	}

	static void Expression(out bool value) {
		bool value1;
		AndExp(out value);
		while (la.kind == or_Sym) {
			Get();
			AndExp(out value1);
			value = value || value1;
		}
	}

	static void AndExp(out bool value) {
		bool value1;
		EqlExp(out value);
		while (la.kind == and_Sym) {
			Get();
			EqlExp(out value1);
			value = value && value1;
		}
	}

	static void EqlExp(out bool value) {
		bool value1;
		NotExp(out value);
		while (la.kind == equalequal_Sym || la.kind == bangequal_Sym) {
			if (la.kind == equalequal_Sym) {
				Get();
				NotExp(out value1);
				value = value == value1;
			} else {
				Get();
				NotExp(out value1);
				value = value != value1;
			}
		}
	}

	static void NotExp(out bool value) {
		value = false;
		if (StartOf(2)) {
			Factor(out value);
		} else if (la.kind == not_Sym) {
			Get();
			NotExp(out value);
			value = ! value;
		} else SynErr(19);
	}

	static void Factor(out bool value) {
		int index;
		value = false;
		if (la.kind == true_Sym) {
			Get();
			value = true;
		} else if (la.kind == false_Sym) {
			Get();
			value = false;
		} else if (la.kind == variable_Sym) {
			Variable(out index);
			value = mem[index];
		} else if (la.kind == lparen_Sym) {
			Get();
			Expression(out value);
			Expect(rparen_Sym);
		} else SynErr(20);
	}



	public static void Parse() {
		la = new Token();
		la.val = "";
		Get();
		BoolCalc();
		Expect(EOF_SYM);

	}

	static bool[,] set = {
		{T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x},
		{T,x,T,x, x,T,x,x, x,T,x,x, x,x,T,T, T,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x}

	};

} // end Parser

/* pdt - considerable extension from here on */

public class ErrorRec {
	public int line, col, num;
	public string str;
	public ErrorRec next;

	public ErrorRec(int l, int c, string s) {
		line = l; col = c; str = s; next = null;
	}

} // end ErrorRec

public class Errors {

	public static int count = 0;                                     // number of errors detected
	public static int warns = 0;                                     // number of warnings detected
	public static string errMsgFormat = "file {0} : ({1}, {2}) {3}"; // 0=file 1=line, 2=column, 3=text
	static string fileName = "";
	static string listName = "";
	static bool mergeErrors = false;
	static StreamWriter mergedList;

	static ErrorRec first = null, last;
	static bool eof = false;

	static string GetLine() {
		char ch, CR = '\r', LF = '\n';
		int l = 0;
		StringBuilder s = new StringBuilder();
		ch = (char) Buffer.Read();
		while (ch != Buffer.EOF && ch != CR && ch != LF) {
			s.Append(ch); l++; ch = (char) Buffer.Read();
		}
		eof = (l == 0 && ch == Buffer.EOF);
		if (ch == CR) {  // check for MS-DOS
			ch = (char) Buffer.Read();
			if (ch != LF && ch != Buffer.EOF) Buffer.Pos--;
		}
		return s.ToString();
	}

	static void Display (string s, ErrorRec e) {
		mergedList.Write("**** ");
		for (int c = 1; c < e.col; c++)
			if (s[c-1] == '\t') mergedList.Write("\t"); else mergedList.Write(" ");
		mergedList.WriteLine("^ " + e.str);
	}

	public static void Init (string fn, string dir, bool merge) {
		fileName = fn;
		listName = dir + "listing.txt";
		listName = "listing.txt";
		mergeErrors = merge;
		if (mergeErrors)
			try {
				mergedList = new StreamWriter(new FileStream(listName, FileMode.Create));
			} catch (IOException) {
				Errors.Exception("-- could not open " + listName);
			}
	}

	public static void Summarize () {
		if (mergeErrors) {
			mergedList.WriteLine();
			ErrorRec cur = first;
			Buffer.Pos = 0;
			int lnr = 1;
			string s = GetLine();
			while (!eof) {
				mergedList.WriteLine("{0,4} {1}", lnr, s);
				while (cur != null && cur.line == lnr) {
					Display(s, cur); cur = cur.next;
				}
				lnr++; s = GetLine();
			}
			if (cur != null) {
				mergedList.WriteLine("{0,4}", lnr);
				while (cur != null) {
					Display(s, cur); cur = cur.next;
				}
			}
			mergedList.WriteLine();
			mergedList.WriteLine(count + " errors detected");
			if (warns > 0) mergedList.WriteLine(warns + " warnings detected");
			mergedList.Close();
		}
		switch (count) {
			case 0 : Console.WriteLine("Parsed correctly"); break;
			case 1 : Console.WriteLine("1 error detected"); break;
			default: Console.WriteLine(count + " errors detected"); break;
		}
		if (warns > 0) Console.WriteLine(warns + " warnings detected");
		if ((count > 0 || warns > 0) && mergeErrors) Console.WriteLine("see " + listName);
	}

	public static void StoreError (int line, int col, string s) {
		if (mergeErrors) {
			ErrorRec latest = new ErrorRec(line, col, s);
			if (first == null) first = latest; else last.next = latest;
			last = latest;
		} else Console.WriteLine(errMsgFormat, fileName, line, col, s);
	}

	public static void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "variable expected"; break;
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\"write\" expected"; break;
			case 5: s = "\"(\" expected"; break;
			case 6: s = "\",\" expected"; break;
			case 7: s = "\")\" expected"; break;
			case 8: s = "\"read\" expected"; break;
			case 9: s = "\";\" expected"; break;
			case 10: s = "\"or\" expected"; break;
			case 11: s = "\"and\" expected"; break;
			case 12: s = "\"==\" expected"; break;
			case 13: s = "\"!=\" expected"; break;
			case 14: s = "\"not\" expected"; break;
			case 15: s = "\"true\" expected"; break;
			case 16: s = "\"false\" expected"; break;
			case 17: s = "??? expected"; break;
			case 18: s = "this symbol not expected in BoolCalc"; break;
			case 19: s = "invalid NotExp"; break;
			case 20: s = "invalid Factor"; break;

			default: s = "error " + n; break;
		}
		StoreError(line, col, s);
		count++;
	}

	public static void SemErr (int line, int col, int n) {
		StoreError(line, col, ("error " + n));
		count++;
	}

	public static void Error (int line, int col, string s) {
		StoreError(line, col, s);
		count++;
	}

	public static void Error (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		count++;
	}

	public static void Warn (int line, int col, string s) {
		StoreError(line, col, s);
		warns++;
	}

	public static void Warn (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		warns++;
	}

	public static void Exception (string s) {
		Console.WriteLine(s);
		System.Environment.Exit(1);
	}

} // end Errors

} // end namespace
