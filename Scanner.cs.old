
using System;
using System.IO;
using System.Collections;
using System.Text;

namespace BoolCalc {

public class Token {
	public int kind;    // token kind
	public int pos;     // token position in the source text (starting at 0)
	public int col;     // token column (starting at 0)
	public int line;    // token line (starting at 1)
	public string val;  // token value
	public Token next;  // AW 2003-03-07 Tokens are kept in linked list
}

public class Buffer {
	public const char EOF = (char)256;
	static byte[] buf;
	static int bufLen;
	static int pos;

	public static void Fill (Stream s) {
		bufLen = (int) s.Length;
		buf = new byte[bufLen];
		s.Read(buf, 0, bufLen);
		pos = 0;
	}

	public static int Read () {
		if (pos < bufLen) return buf[pos++];
		else return EOF;                          /* pdt */
	}

	public static int Peek () {
		if (pos < bufLen) return buf[pos];
		else return EOF;                          /* pdt */
	}

	/* AW 2003-03-10 moved this from ParserGen.cs */
	public static string GetString (int beg, int end) {
		StringBuilder s = new StringBuilder(64);
		int oldPos = Buffer.Pos;
		Buffer.Pos = beg;
		while (beg < end) { s.Append((char)Buffer.Read()); beg++; }
		Buffer.Pos = oldPos;
		return s.ToString();
	}

	public static int Pos {
		get { return pos; }
		set {
			if (value < 0) pos = 0;
			else if (value >= bufLen) pos = bufLen;
			else pos = value;
		}
	}

} // end Buffer

public class Scanner {
	const char EOL = '\n';
	const int  eofSym = 0;
	const int charSetSize = 256;
	const int maxT = 17;
	const int noSym = 17;
	// terminals
	const int EOF_SYM = 0;
	const int number_Sym = 1;
	const int variable_Sym = 2;
	const int equal_Sym = 3;
	const int write_Sym = 4;
	const int lparen_Sym = 5;
	const int comma_Sym = 6;
	const int rparen_Sym = 7;
	const int read_Sym = 8;
	const int semicolon_Sym = 9;
	const int or_Sym = 10;
	const int and_Sym = 11;
	const int equalequal_Sym = 12;
	const int bangequal_Sym = 13;
	const int not_Sym = 14;
	const int true_Sym = 15;
	const int false_Sym = 16;
	const int NOT_SYM = 17;
	// pragmas
	const int CodeOn_Sym = 18;
	const int CodeOff_Sym = 19;
	const int DebugOn_Sym = 20;
	const int DebugOff_Sym = 21;

	static short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0, 25,  0,  0,  7,  0,  0,  0, 14, 16,  0,  0, 15,  0,  0,  0,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0, 20,  0, 36,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0, 40,  2,  2,  2,  2, 43,  2,  2,  2,  2,  2,  2,  2, 41, 39,
	  2,  2, 38,  2, 42,  2,  2, 37,  2,  2,  2,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};


	static Token t;          // current token
	static char ch;          // current input character
	static int pos;          // column number of current character
	static int line;         // line number of current character
	static int lineStart;    // start position of current line
	static int oldEols;      // EOLs that appeared in a comment;
	static BitArray ignore;  // set of characters to be ignored by the scanner

	static Token tokens;     // the complete input token stream
	static Token pt;         // current peek token

	public static void Init (string fileName) {
		FileStream s = null;
		try {
			s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			Init(s);
		} catch (IOException) {
			Console.WriteLine("--- Cannot open file {0}", fileName);
			System.Environment.Exit(1);
		} finally {
			if (s != null) s.Close();
		}
	}

	public static void Init (Stream s) {
		Buffer.Fill(s);
		pos = -1; line = 1; lineStart = 0;
		oldEols = 0;
		NextCh();
		ignore = new BitArray(charSetSize+1);
		ignore[' '] = true;  // blanks are always white space
		ignore[0] = true; ignore[1] = true; ignore[2] = true; ignore[3] = true; 
		ignore[4] = true; ignore[5] = true; ignore[6] = true; ignore[7] = true; 
		ignore[8] = true; ignore[9] = true; ignore[10] = true; ignore[11] = true; 
		ignore[12] = true; ignore[13] = true; ignore[14] = true; ignore[15] = true; 
		ignore[16] = true; ignore[17] = true; ignore[18] = true; ignore[19] = true; 
		ignore[20] = true; ignore[21] = true; ignore[22] = true; ignore[23] = true; 
		ignore[24] = true; ignore[25] = true; ignore[26] = true; ignore[27] = true; 
		ignore[28] = true; ignore[29] = true; ignore[30] = true; ignore[31] = true; 
		
		//--- AW: fill token list
		tokens = new Token();  // first token is a dummy
		Token node = tokens;
		do {
			node.next = NextToken();
			node = node.next;
		} while (node.kind != eofSym);
		node.next = node;
		node.val = "EOF";
		t = pt = tokens;
	}

	static void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; }
		else {
			ch = (char)Buffer.Read(); pos++;
			// replace isolated '\r' by '\n' in order to make
			// eol handling uniform across Windows, Unix and Mac
			if (ch == '\r' && Buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; lineStart = pos + 1; }
		}

	}


	static bool Comment0() {
		int level = 1, line0 = line, lineStart0 = lineStart;
		NextCh();
		if (ch == '*') {
			NextCh();
			for(;;) {
				if (ch == '*') {
					NextCh();
					if (ch == '/') {
						level--;
						if (level == 0) { oldEols = line - line0; NextCh(); return true; }
						NextCh();
					}
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			if (ch == EOL) { line--; lineStart = lineStart0; }
			pos = pos - 2; Buffer.Pos = pos+1; NextCh();
		}
		return false;
	}

	static bool Comment1() {
		int level = 1, line0 = line, lineStart0 = lineStart;
		NextCh();
		if (ch == '/') {
			NextCh();
			for(;;) {
				if (ch == 10) {
					level--;
					if (level == 0) { oldEols = line - line0; NextCh(); return true; }
					NextCh();
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			if (ch == EOL) { line--; lineStart = lineStart0; }
			pos = pos - 2; Buffer.Pos = pos+1; NextCh();
		}
		return false;
	}


	static void CheckLiteral() {
		switch (t.val) {
			default: break;
		}
	}

	/* AW Scan() renamed to NextToken() */
	static Token NextToken() {
		while (ignore[ch]) NextCh();
		if (ch == '/' && Comment0() ||ch == '/' && Comment1()) return NextToken();
		t = new Token();
		t.pos = pos; t.col = pos - lineStart + 1; t.line = line;
		int state = start[ch];
		StringBuilder buf = new StringBuilder(16);
		buf.Append(ch); NextCh();
		switch (state) {
			case -1: { t.kind = eofSym; goto done; } // NextCh already done /* pdt */
			case 0: { t.kind = noSym; goto done; }   // NextCh already done
			case 1:
				if ((ch >= '0' && ch <= '9')) { buf.Append(ch); NextCh(); goto case 1; }
				else { t.kind = number_Sym; goto done; }
			case 2:
				{ t.kind = variable_Sym; goto done; }
			case 3:
				{ t.kind = CodeOn_Sym; goto done; }
			case 4:
				{ t.kind = CodeOff_Sym; goto done; }
			case 5:
				{ t.kind = DebugOn_Sym; goto done; }
			case 6:
				{ t.kind = DebugOff_Sym; goto done; }
			case 7:
				if (ch == 'C') { buf.Append(ch); NextCh(); goto case 8; }
				else if (ch == 'D') { buf.Append(ch); NextCh(); goto case 9; }
				else { t.kind = noSym; goto done; }
			case 8:
				if (ch == '+') { buf.Append(ch); NextCh(); goto case 3; }
				else if (ch == '-') { buf.Append(ch); NextCh(); goto case 4; }
				else { t.kind = noSym; goto done; }
			case 9:
				if (ch == '+') { buf.Append(ch); NextCh(); goto case 5; }
				else if (ch == '-') { buf.Append(ch); NextCh(); goto case 6; }
				else { t.kind = noSym; goto done; }
			case 10:
				if (ch == 'i') { buf.Append(ch); NextCh(); goto case 11; }
				else { t.kind = noSym; goto done; }
			case 11:
				if (ch == 't') { buf.Append(ch); NextCh(); goto case 12; }
				else { t.kind = noSym; goto done; }
			case 12:
				if (ch == 'e') { buf.Append(ch); NextCh(); goto case 13; }
				else { t.kind = noSym; goto done; }
			case 13:
				{ t.kind = write_Sym; goto done; }
			case 14:
				{ t.kind = lparen_Sym; goto done; }
			case 15:
				{ t.kind = comma_Sym; goto done; }
			case 16:
				{ t.kind = rparen_Sym; goto done; }
			case 17:
				if (ch == 'a') { buf.Append(ch); NextCh(); goto case 18; }
				else { t.kind = noSym; goto done; }
			case 18:
				if (ch == 'd') { buf.Append(ch); NextCh(); goto case 19; }
				else { t.kind = noSym; goto done; }
			case 19:
				{ t.kind = read_Sym; goto done; }
			case 20:
				{ t.kind = semicolon_Sym; goto done; }
			case 21:
				{ t.kind = or_Sym; goto done; }
			case 22:
				if (ch == 'd') { buf.Append(ch); NextCh(); goto case 23; }
				else { t.kind = noSym; goto done; }
			case 23:
				{ t.kind = and_Sym; goto done; }
			case 24:
				{ t.kind = equalequal_Sym; goto done; }
			case 25:
				if (ch == '=') { buf.Append(ch); NextCh(); goto case 26; }
				else { t.kind = noSym; goto done; }
			case 26:
				{ t.kind = bangequal_Sym; goto done; }
			case 27:
				if (ch == 't') { buf.Append(ch); NextCh(); goto case 28; }
				else { t.kind = noSym; goto done; }
			case 28:
				{ t.kind = not_Sym; goto done; }
			case 29:
				if (ch == 'u') { buf.Append(ch); NextCh(); goto case 30; }
				else { t.kind = noSym; goto done; }
			case 30:
				if (ch == 'e') { buf.Append(ch); NextCh(); goto case 31; }
				else { t.kind = noSym; goto done; }
			case 31:
				{ t.kind = true_Sym; goto done; }
			case 32:
				if (ch == 'l') { buf.Append(ch); NextCh(); goto case 33; }
				else { t.kind = noSym; goto done; }
			case 33:
				if (ch == 's') { buf.Append(ch); NextCh(); goto case 34; }
				else { t.kind = noSym; goto done; }
			case 34:
				if (ch == 'e') { buf.Append(ch); NextCh(); goto case 35; }
				else { t.kind = noSym; goto done; }
			case 35:
				{ t.kind = false_Sym; goto done; }
			case 36:
				if (ch == '=') { buf.Append(ch); NextCh(); goto case 24; }
				else { t.kind = equal_Sym; goto done; }
			case 37:
				if (ch == 'r') { buf.Append(ch); NextCh(); goto case 10; }
				else { t.kind = variable_Sym; goto done; }
			case 38:
				if (ch == 'e') { buf.Append(ch); NextCh(); goto case 17; }
				else { t.kind = variable_Sym; goto done; }
			case 39:
				if (ch == 'r') { buf.Append(ch); NextCh(); goto case 21; }
				else { t.kind = variable_Sym; goto done; }
			case 40:
				if (ch == 'n') { buf.Append(ch); NextCh(); goto case 22; }
				else { t.kind = variable_Sym; goto done; }
			case 41:
				if (ch == 'o') { buf.Append(ch); NextCh(); goto case 27; }
				else { t.kind = variable_Sym; goto done; }
			case 42:
				if (ch == 'r') { buf.Append(ch); NextCh(); goto case 29; }
				else { t.kind = variable_Sym; goto done; }
			case 43:
				if (ch == 'a') { buf.Append(ch); NextCh(); goto case 32; }
				else { t.kind = variable_Sym; goto done; }

		}
		done:
		t.val = buf.ToString();
		return t;
	}

	/* AW 2003-03-07 get the next token, move on and synch peek token with current */
	public static Token Scan () {
		t = pt = t.next;
		return t;
	}

	/* AW 2003-03-07 get the next token, ignore pragmas */
	public static Token Peek () {
		do {                      // skip pragmas while peeking
			pt = pt.next;
		} while (pt.kind > maxT);
		return pt;
	}

	/* AW 2003-03-11 to make sure peek start at current scan position */
	public static void ResetPeek () { pt = t; }

} // end Scanner

} // end namespace
