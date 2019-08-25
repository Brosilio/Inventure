/*
 * Copyright (C) 2019 FEAR Labs.
 * Inventure Parser for C#
 * MIT liscensed
 * Kept in a single file because single file things are good
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace InventureSharp
{
    public class IvtParser
    {
        private string input;
        private int pos;


        public void Parse(string input)
        {
            this.input = input;
        }
    }

    public class IvtLexer
    {
        private string input;
        private int pos;
        private int start;
        private int line = 1;
        private int character = 1;

        private List<Token> tokens = new List<Token>();

        public IvtLexer(string input)
        {
            this.input = input.Replace("\r", "");
        }

        public List<Token> Tokenize()
        {
            tokens.Clear();

            while(!AtEnd())
            {
                start = pos;
                ScanForToken();
            }

            tokens.Add(new Token(ETokenType.EOF, null, line, character));
            return tokens;
        }

        private void ScanForToken()
        {
            char c = Advance();
            switch (c)
            {
                case ';': tokens.Add(new Token(ETokenType.Semicolon, null, line, character)); break;
                case ':': tokens.Add(new Token(ETokenType.Colon, null, line, character)); break;
                case ',': tokens.Add(new Token(ETokenType.Comma, null, line, character)); break;
                case '.': tokens.Add(new Token(ETokenType.Period, null, line, character)); break;
                case '=': tokens.Add(new Token(ETokenType.Equals, null, line, character)); break;
                case '%': tokens.Add(new Token(ETokenType.Percent, null, line, character)); break;
                case '<': tokens.Add(new Token(ETokenType.LessThan, null, line, character)); break;
                case '>': tokens.Add(new Token(ETokenType.GreaterThan, null, line, character)); break;
                case '(': tokens.Add(new Token(ETokenType.LParen, null, line, character)); break;
                case ')': tokens.Add(new Token(ETokenType.RParen, null, line, character)); break;
                case '[': tokens.Add(new Token(ETokenType.LSquare, null, line, character)); break;
                case ']': tokens.Add(new Token(ETokenType.RSquare, null, line, character)); break;
                case '{': tokens.Add(new Token(ETokenType.LBrace, null, line, character)); break;
                case '}': tokens.Add(new Token(ETokenType.RBrace, null, line, character)); break;

                case '@':
                    GetDirective();
                    break;

                case '"':
                    GetSurroundedLiteral(ETokenType.StringLiteral, '"');
                    break;

                case '\n':
                    line++;
                    character = 1;
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '/':
                    if (IsMatchToNext('/'))
                        EatComment();
                    if (IsMatchToNext('*'))
                        EatCComment();
                    break;

                default:
                    if (char.IsDigit(c))
                        GetNumberLiteral();
                    else if (IsAlpha(c))
                        GetIdentifier();
                    else
                        tokens.Add(new Token(ETokenType.Unknown, c.ToString(), line, character));
                    break;
            }
        }

        private void GetSurroundedLiteral(ETokenType type, char endChar)
        {
            while (GetCurrentChar() != endChar && !AtEnd())
            {
                Advance();
            }
            Advance();

            tokens.Add(new Token(type, input.Substring(start + 1, pos - (start + 2)), line, character));
        }

        private void GetDirective()
        {
            while (IsAlphaNumeric(GetCurrentChar()))
                Advance();

            string s = input.Substring(start, pos - start);

            tokens.Add(new Token(ETokenType.Directive, s, line, character));
        }

        private void GetNumberLiteral()
        {
            bool isFloat = false;
            while (char.IsDigit(GetCurrentChar()))
                Advance();
            if (GetCurrentChar() == '.' && char.IsDigit(GetNextChar()))
            {
                isFloat = true;
                Advance();
                while (char.IsDigit(GetCurrentChar()))
                    Advance();
            }
            string s = input.Substring(start, pos - start);

            if (isFloat)
                tokens.Add(new Token(ETokenType.FloatLiteral, float.Parse(s), line, character));
            else
                tokens.Add(new Token(ETokenType.IntLiteral, int.Parse(s), line, character));
        }

        private void GetIdentifier()
        {
            while (IsAlphaNumeric(GetCurrentChar()))
                Advance();

            string s = input.Substring(start, pos - start);

            tokens.Add(new Token(ETokenType.Identifier, s, line, character));
        }

        private void EatCComment()
        {
            while(!AtEnd())
            {
                if (Advance() == '*' && IsMatchToNext('/'))
                    break;
            }
        }

        private void EatComment()
        {
            while (!AtEnd() && Advance() != '\n') ;
        }

        private char Advance()
        {
            pos++;
            character++;
            return input[pos - 1];
        }

        private char GetCurrentChar()
        {
            return !AtEnd() ? input[pos] : '\0';
        }

        private char GetNextChar()
        {
            if (!AtEnd())
                return input[pos + 1];
            else
                return '\0';
        }

        private bool IsMatchToNext(char c)
        {
            if (AtEnd())
                return false;
            if (GetCurrentChar() != c)
                return false;
            pos++;
            return true;
        }

        private bool AtEnd()
        {
            return pos >= input.Length;
        }

        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || char.IsDigit(c);
    }

    public class Token
    {
        public ETokenType type;
        public object value;
        public int line;
        public int character;

        public Token(ETokenType type, object value, int line, int character)
        {
            this.type = type;
            this.value = value;
            this.line = line;
            this.character = character;
        }

        public override string ToString()
        {
            return $"{type.ToString().PadRight(14)}|{(value ?? "").ToString().PadRight(33)}|{line.ToString().PadRight(4)}|{character.ToString().PadRight(4)}";
        }
    }

    public enum ETokenType
    {
        Unknown,
        Identifier,
        Directive,
        Plaintext,
        StringLiteral,
        FloatLiteral,
        IntLiteral,
        Semicolon,
        Colon,
        Comma,
        Period,
        Equals,
        Percent,
        LBrace,
        RBrace,
        LParen,
        RParen,
        LSquare,
        RSquare,
        GreaterThan,
        LessThan,
        Newline,
        EOF,
    }
}
