
using System;

namespace JinxBot.Controls.CSS
{



    internal class Parser
    {
        const int _EOF = 0;
        const int _ident = 1;
        const int _newline = 2;
        const int _digit = 3;
        const int _whitespace = 4;
        const int maxT = 48;

        const bool T = true;
        const bool x = false;
        const int minErrDist = 2;

        public Scanner scanner;
        public Errors errors;

        public Token t;    // last recognized token
        public Token la;   // lookahead token
        int errDist = minErrDist;

        public CSSDocument CSSDoc;

        bool PartOfHex(string value)
        {
            if (value.Length == 7) { return false; }
            if (value.Length + la.val.Length > 7) { return false; }
            System.Collections.Generic.List<string> hexes = new System.Collections.Generic.List<string>(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "a", "b", "c", "d", "e", "f" });
            foreach (char c in la.val)
            {
                if (!hexes.Contains(c.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
        bool IsUnit()
        {
            if (la.kind != 1) { return false; }
            System.Collections.Generic.List<string> units = new System.Collections.Generic.List<string>(new string[] { "em", "ex", "px", "gd", "rem", "vw", "vh", "vm", "ch", "mm", "cm", "in", "pt", "pc", "deg", "grad", "rad", "turn", "ms", "s", "hz", "khz" });
            return units.Contains(la.val);
        }

        /*------------------------------------------------------------------------*
         *----- SCANNER DESCRIPTION ----------------------------------------------*
         *------------------------------------------------------------------------*/



        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            errors = new Errors();
        }

        void SynErr(int n)
        {
            if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
            errDist = 0;
        }

        public void SemErr(string msg)
        {
            if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
            errDist = 0;
        }

        void Get()
        {
            for (; ; )
            {
                t = la;
                la = scanner.Scan();
                if (la.kind <= maxT) { ++errDist; break; }

                la = t;
            }
        }

        void Expect(int n)
        {
            if (la.kind == n) Get(); else { SynErr(n); }
        }

        bool StartOf(int s)
        {
            return set[s, la.kind];
        }

        void ExpectWeak(int n, int follow)
        {
            if (la.kind == n) Get();
            else
            {
                SynErr(n);
                while (!StartOf(follow)) Get();
            }
        }


        bool WeakSeparator(int n, int syFol, int repFol)
        {
            int kind = la.kind;
            if (kind == n) { Get(); return true; }
            else if (StartOf(repFol)) { return false; }
            else
            {
                SynErr(n);
                while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
                {
                    Get();
                    kind = la.kind;
                }
                return StartOf(syFol);
            }
        }


        void CSS3()
        {
            CSSDoc = new CSSDocument();
            string cset = null;
            RuleSet rset = null;
            Directive dir = null;

            while (la.kind == 5 || la.kind == 6)
            {
                if (la.kind == 5)
                {
                    Get();
                }
                else
                {
                    Get();
                }
            }
            while (StartOf(1))
            {
                if (StartOf(2))
                {
                    ruleset(out rset);
                    CSSDoc.RuleSets.Add(rset);
                }
                else
                {
                    directive(out dir);
                    CSSDoc.Directives.Add(dir);
                }
                while (la.kind == 5 || la.kind == 6)
                {
                    if (la.kind == 5)
                    {
                        Get();
                    }
                    else
                    {
                        Get();
                    }
                }
            }
        }

        void ruleset(out RuleSet rset)
        {
            rset = new RuleSet();
            Selector sel = null;
            Declaration dec = null;

            selector(out sel);
            rset.Selectors.Add(sel);
            while (la.kind == 27)
            {
                Get();
                selector(out sel);
                rset.Selectors.Add(sel);
            }
            Expect(24);
            if (StartOf(3))
            {
                declaration(out dec);
                rset.Declarations.Add(dec);
                while (la.kind == 25)
                {
                    Get();
                    if (la.val.Equals("}")) { Get(); return; }

                    declaration(out dec);
                    rset.Declarations.Add(dec);
                }
                if (la.kind == 25)
                {
                    Get();
                }
            }
            Expect(26);
        }

        void directive(out Directive dir)
        {
            dir = new Directive();
            Declaration dec = null;
            RuleSet rset = null;
            Expression exp = null;
            Directive dr = null;
            string ident = null;
            Medium m;

            Expect(23);
            identity(out ident);
            dir.Name = "@" + ident;
            switch (dir.Name.ToLower())
            {
                case "@media": dir.Type = DirectiveType.Media; break;
                case "@import": dir.Type = DirectiveType.Import; break;
                case "@charset": dir.Type = DirectiveType.Charset; break;
                case "@page": dir.Type = DirectiveType.Page; break;
                case "@font-face": dir.Type = DirectiveType.FontFace; break;
                case "@namespace": dir.Type = DirectiveType.Namespace; break;
                default: dir.Type = DirectiveType.Other; break;
            }

            if (StartOf(4))
            {
                if (StartOf(4))
                {
                    expr(out exp);
                    dir.Expression = exp;
                }
                else
                {
                    medium(out m);
                    dir.Mediums.Add(m);
                }
            }
            if (la.kind == 24)
            {
                Get();
                if (StartOf(5))
                {
                    while (StartOf(1))
                    {
                        if (dir.Type == DirectiveType.Page || dir.Type == DirectiveType.FontFace)
                        {
                            declaration(out dec);
                            dir.Declarations.Add(dec);
                            while (la.kind == 25)
                            {
                                Get();
                                if (la.val.Equals("}")) { Get(); return; }
                                declaration(out dec);
                                dir.Declarations.Add(dec);
                            }
                            if (la.kind == 25)
                            {
                                Get();
                            }
                        }
                        else if (StartOf(2))
                        {
                            ruleset(out rset);
                            dir.RuleSets.Add(rset);
                        }
                        else
                        {
                            directive(out dr);
                            dir.Directives.Add(dr);
                        }
                    }
                }
                Expect(26);
            }
            else if (la.kind == 25)
            {
                Get();
            }
            else SynErr(49);
        }

        void QuotedString(out string qs)
        {
            qs = ""; char quote = '\n';
            if (la.kind == 7)
            {
                Get();
                quote = '\'';
                while (StartOf(6))
                {
                    Get();
                    qs += t.val;
                    if (la.val.Equals("'") && !t.val.Equals("\\")) { break; }
                }
                Expect(7);
            }
            else if (la.kind == 8)
            {
                Get();
                quote = '"';
                while (StartOf(7))
                {
                    Get();
                    qs += t.val;
                    if (la.val.Equals("\"") && !t.val.Equals("\\")) { break; }
                }
                Expect(8);
            }
            else SynErr(50);

        }

        void URI(out string url)
        {
            url = "";
            Expect(9);
            if (la.kind == 10)
            {
                Get();
            }
            if (la.kind == 7 || la.kind == 8)
            {
                QuotedString(out url);
            }
            else if (StartOf(8))
            {
                while (StartOf(9))
                {
                    Get();
                    url += t.val;
                    if (la.val.Equals(")")) { break; }
                }
            }
            else SynErr(51);
            if (la.kind == 11)
            {
                Get();
            }
        }

        void medium(out Medium m)
        {
            m = Medium.all;
            switch (la.kind)
            {
                case 12:
                    {
                        Get();
                        m = Medium.all;
                        break;
                    }
                case 13:
                    {
                        Get();
                        m = Medium.aural;
                        break;
                    }
                case 14:
                    {
                        Get();
                        m = Medium.braille;
                        break;
                    }
                case 15:
                    {
                        Get();
                        m = Medium.embossed;
                        break;
                    }
                case 16:
                    {
                        Get();
                        m = Medium.handheld;
                        break;
                    }
                case 17:
                    {
                        Get();
                        m = Medium.print;
                        break;
                    }
                case 18:
                    {
                        Get();
                        m = Medium.projection;
                        break;
                    }
                case 19:
                    {
                        Get();
                        m = Medium.screen;
                        break;
                    }
                case 20:
                    {
                        Get();
                        m = Medium.tty;
                        break;
                    }
                case 21:
                    {
                        Get();
                        m = Medium.tv;
                        break;
                    }
                default: SynErr(52); break;
            }
        }

        void identity(out string ident)
        {
            ident = null;
            switch (la.kind)
            {
                case 1:
                    {
                        Get();
                        break;
                    }
                case 22:
                    {
                        Get();
                        break;
                    }
                case 9:
                    {
                        Get();
                        break;
                    }
                case 12:
                    {
                        Get();
                        break;
                    }
                case 13:
                    {
                        Get();
                        break;
                    }
                case 14:
                    {
                        Get();
                        break;
                    }
                case 15:
                    {
                        Get();
                        break;
                    }
                case 16:
                    {
                        Get();
                        break;
                    }
                case 17:
                    {
                        Get();
                        break;
                    }
                case 18:
                    {
                        Get();
                        break;
                    }
                case 19:
                    {
                        Get();
                        break;
                    }
                case 20:
                    {
                        Get();
                        break;
                    }
                case 21:
                    {
                        Get();
                        break;
                    }
                default: SynErr(53); break;
            }
            ident = t.val;
        }

        void expr(out Expression exp)
        {
            exp = new Expression();
            char? sep = null;
            Term trm = null;

            term(out trm);
            exp.Terms.Add(trm);
            while (StartOf(10))
            {
                if (la.kind == 27 || la.kind == 44)
                {
                    if (la.kind == 44)
                    {
                        Get();
                        sep = '/';
                    }
                    else
                    {
                        Get();
                        sep = ',';
                    }
                }
                term(out trm);
                if (sep.HasValue) { trm.Seperator = sep.Value; }
                exp.Terms.Add(trm);
                sep = null;

            }
        }

        void declaration(out Declaration dec)
        {
            dec = new Declaration();
            Expression exp = null;
            string ident = null;

            identity(out ident);
            dec.Name = ident;
            Expect(42);
            expr(out exp);
            dec.Expression = exp;
            if (la.kind == 43)
            {
                Get();
                dec.Important = true;
            }
        }

        void selector(out Selector sel)
        {
            sel = new Selector();
            SimpleSelector ss = null;
            Combinator? cb = null;

            simpleselector(out ss);
            sel.SimpleSelectors.Add(ss);
            while (StartOf(11))
            {
                if (la.kind == 28 || la.kind == 29 || la.kind == 30)
                {
                    if (la.kind == 28)
                    {
                        Get();
                        cb = Combinator.PrecededImmediatelyBy;
                    }
                    else if (la.kind == 29)
                    {
                        Get();
                        cb = Combinator.ChildOf;
                    }
                    else
                    {
                        Get();
                        cb = Combinator.PrecededBy;
                    }
                }
                simpleselector(out ss);
                if (cb.HasValue) { ss.Combinator = cb.Value; }
                sel.SimpleSelectors.Add(ss);

                cb = null;
            }
        }

        void simpleselector(out SimpleSelector ss)
        {
            ss = new SimpleSelector();
            string psd = null;
            JinxBot.Controls.CSS.Attribute atb = null;
            SimpleSelector parent = ss;
            string ident = null;

            if (StartOf(3))
            {
                identity(out ident);
                ss.ElementName = ident;
            }
            else if (la.kind == 31)
            {
                Get();
                ss.ElementName = "*";
            }
            else if (StartOf(12))
            {
                if (la.kind == 32)
                {
                    Get();
                    identity(out ident);
                    ss.ID = ident;
                }
                else if (la.kind == 33)
                {
                    Get();
                    identity(out ident);
                    ss.Class = ident;
                }
                else if (la.kind == 34)
                {
                    attrib(out atb);
                    ss.Attribute = atb;
                }
                else
                {
                    pseudo(out psd);
                    ss.Pseudo = psd;
                }
            }
            else SynErr(54);
            while (StartOf(12))
            {
                SimpleSelector child = new SimpleSelector();
                if (la.kind == 32)
                {
                    Get();
                    identity(out ident);
                    child.ID = ident;
                }
                else if (la.kind == 33)
                {
                    Get();
                    identity(out ident);
                    child.Class = ident;
                }
                else if (la.kind == 34)
                {
                    attrib(out atb);
                    child.Attribute = atb;
                }
                else
                {
                    pseudo(out psd);
                    child.Pseudo = psd;
                }
                parent.Child = child;
                parent = child;

            }
        }

        void attrib(out JinxBot.Controls.CSS.Attribute atb)
        {
            atb = new JinxBot.Controls.CSS.Attribute();
            string quote = null;
            string ident = null;

            Expect(34);
            identity(out ident);
            atb.Operand = ident;
            if (StartOf(13))
            {
                switch (la.kind)
                {
                    case 35:
                        {
                            Get();
                            atb.Operator = AttributeOperator.Equals;
                            break;
                        }
                    case 36:
                        {
                            Get();
                            atb.Operator = AttributeOperator.InList;
                            break;
                        }
                    case 37:
                        {
                            Get();
                            atb.Operator = AttributeOperator.Hyphenated;
                            break;
                        }
                    case 38:
                        {
                            Get();
                            atb.Operator = AttributeOperator.EndsWith;
                            break;
                        }
                    case 39:
                        {
                            Get();
                            atb.Operator = AttributeOperator.BeginsWith;
                            break;
                        }
                    case 40:
                        {
                            Get();
                            atb.Operator = AttributeOperator.Contains;
                            break;
                        }
                }
                if (StartOf(3))
                {
                    identity(out ident);
                    atb.Value = ident;
                }
                else if (la.kind == 7 || la.kind == 8)
                {
                    QuotedString(out quote);
                    atb.Value = quote;
                }
                else SynErr(55);
            }
            Expect(41);
        }

        void pseudo(out string pseudo)
        {
            pseudo = "";
            Expression exp = null;
            string ident = null;

            Expect(42);
            if (la.kind == 42)
            {
                Get();
            }
            identity(out ident);
            pseudo = ident;
            if (la.kind == 10)
            {
                Get();
                pseudo += t.val;
                expr(out exp);
                pseudo += exp.ToString();
                Expect(11);
                pseudo += t.val;
            }
        }

        void term(out Term trm)
        {
            trm = new Term();
            string val = "";
            Expression exp = null;
            string ident = null;

            if (la.kind == 7 || la.kind == 8)
            {
                QuotedString(out val);
                trm.Value = val; trm.Type = TermType.String;
            }
            else if (la.kind == 9)
            {
                URI(out val);
                trm.Value = val; trm.Type = TermType.Url;
            }
            else if (la.kind == 45)
            {
                Get();
                identity(out ident);
                trm.Value = "U\\" + ident; trm.Type = TermType.Unicode;
            }
            else if (la.kind == 32)
            {
                HexValue(out val);
                trm.Value = val; trm.Type = TermType.Hex;
            }
            else if (StartOf(3))
            {
                identity(out ident);
                trm.Value = ident; trm.Type = TermType.String;
                if (StartOf(14))
                {
                    while (la.kind == 33 || la.kind == 35 || la.kind == 42)
                    {
                        if (la.kind == 42)
                        {
                            Get();
                            trm.Value += t.val;
                            if (la.kind == 42)
                            {
                                Get();
                                trm.Value += t.val;
                            }
                            identity(out ident);
                            trm.Value += ident;
                        }
                        else if (la.kind == 33)
                        {
                            Get();
                            trm.Value += t.val;
                            identity(out ident);
                            trm.Value += ident;
                        }
                        else
                        {
                            Get();
                            trm.Value += t.val;
                            if (StartOf(3))
                            {
                                identity(out ident);
                                trm.Value += ident;
                            }
                            else if (StartOf(15))
                            {
                                while (la.kind == 3)
                                {
                                    Get();
                                    trm.Value += t.val;
                                }
                            }
                            else SynErr(56);
                        }
                    }
                }
                if (la.kind == 10)
                {
                    Get();
                    expr(out exp);
                    Function func = new Function();
                    func.Name = trm.Value;
                    func.Expression = exp;
                    trm.Value = null;
                    trm.Function = func;
                    trm.Type = TermType.Function;

                    Expect(11);
                }
            }
            else if (StartOf(16))
            {
                if (la.kind == 28 || la.kind == 46)
                {
                    if (la.kind == 46)
                    {
                        Get();
                        trm.Sign = '-';
                    }
                    else
                    {
                        Get();
                        trm.Sign = '+';
                    }
                }
                while (la.kind == 3)
                {
                    Get();
                    val += t.val;
                }
                if (la.kind == 33)
                {
                    Get();
                    val += t.val;
                    while (la.kind == 3)
                    {
                        Get();
                        val += t.val;
                    }
                }
                if (StartOf(17))
                {
                    if (la.val.ToLower().Equals("n"))
                    {
                        Expect(22);
                        val += t.val;
                        if (la.kind == 28 || la.kind == 46)
                        {
                            if (la.kind == 28)
                            {
                                Get();
                                val += t.val;
                            }
                            else
                            {
                                Get();
                                val += t.val;
                            }
                            Expect(3);
                            val += t.val;
                            while (la.kind == 3)
                            {
                                Get();
                                val += t.val;
                            }
                        }
                    }
                    else if (la.kind == 47)
                    {
                        Get();
                        trm.Unit = Unit.Percent;
                    }
                    else
                    {
                        if (IsUnit())
                        {
                            identity(out ident);
                            try
                            {
                                trm.Unit = (Unit)Enum.Parse(typeof(Unit), ident, true);
                            }
                            catch
                            {
                                errors.SemErr(t.line, t.col, string.Format("Unrecognized unit '{0}'", ident));
                            }

                        }
                    }
                }
                trm.Value = val; trm.Type = TermType.Number;
            }
            else SynErr(57);
        }

        void HexValue(out string val)
        {
            val = "";
            bool found = false;

            Expect(32);
            val += t.val;
            if (StartOf(16))
            {
                while (la.kind == 3)
                {
                    Get();
                    val += t.val;
                }
            }
            else if (PartOfHex(val))
            {
                Expect(1);
                val += t.val; found = true;
            }
            else SynErr(58);
            if (!found && PartOfHex(val))
            {
                Expect(1);
                val += t.val;
            }
        }



        public void Parse()
        {
            la = new Token();
            la.val = "";
            Get();
            CSS3();

            Expect(0);
        }

        bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,T, x,x,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,x,T, x,x,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,T,x,T, x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x},
		{x,T,x,T, x,x,x,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x},
		{x,T,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x}

	};
    } // end Parser


    internal class Errors
    {
        public int count = 0;                                    // number of errors detected
        public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public void SynErr(int line, int col, int n)
        {
            string s;
            switch (n)
            {
                case 0: s = "EOF expected"; break;
                case 1: s = "ident expected"; break;
                case 2: s = "newline expected"; break;
                case 3: s = "digit expected"; break;
                case 4: s = "whitespace expected"; break;
                case 5: s = "\"<!--\" expected"; break;
                case 6: s = "\"-->\" expected"; break;
                case 7: s = "\"\'\" expected"; break;
                case 8: s = "\"\"\" expected"; break;
                case 9: s = "\"url\" expected"; break;
                case 10: s = "\"(\" expected"; break;
                case 11: s = "\")\" expected"; break;
                case 12: s = "\"all\" expected"; break;
                case 13: s = "\"aural\" expected"; break;
                case 14: s = "\"braille\" expected"; break;
                case 15: s = "\"embossed\" expected"; break;
                case 16: s = "\"handheld\" expected"; break;
                case 17: s = "\"print\" expected"; break;
                case 18: s = "\"projection\" expected"; break;
                case 19: s = "\"screen\" expected"; break;
                case 20: s = "\"tty\" expected"; break;
                case 21: s = "\"tv\" expected"; break;
                case 22: s = "\"n\" expected"; break;
                case 23: s = "\"@\" expected"; break;
                case 24: s = "\"{\" expected"; break;
                case 25: s = "\";\" expected"; break;
                case 26: s = "\"}\" expected"; break;
                case 27: s = "\",\" expected"; break;
                case 28: s = "\"+\" expected"; break;
                case 29: s = "\">\" expected"; break;
                case 30: s = "\"~\" expected"; break;
                case 31: s = "\"*\" expected"; break;
                case 32: s = "\"#\" expected"; break;
                case 33: s = "\".\" expected"; break;
                case 34: s = "\"[\" expected"; break;
                case 35: s = "\"=\" expected"; break;
                case 36: s = "\"~=\" expected"; break;
                case 37: s = "\"|=\" expected"; break;
                case 38: s = "\"$=\" expected"; break;
                case 39: s = "\"^=\" expected"; break;
                case 40: s = "\"*=\" expected"; break;
                case 41: s = "\"]\" expected"; break;
                case 42: s = "\":\" expected"; break;
                case 43: s = "\"!important\" expected"; break;
                case 44: s = "\"/\" expected"; break;
                case 45: s = "\"U\\\\\" expected"; break;
                case 46: s = "\"-\" expected"; break;
                case 47: s = "\"%\" expected"; break;
                case 48: s = "??? expected"; break;
                case 49: s = "invalid directive"; break;
                case 50: s = "invalid QuotedString"; break;
                case 51: s = "invalid URI"; break;
                case 52: s = "invalid medium"; break;
                case 53: s = "invalid identity"; break;
                case 54: s = "invalid simpleselector"; break;
                case 55: s = "invalid attrib"; break;
                case 56: s = "invalid term"; break;
                case 57: s = "invalid term"; break;
                case 58: s = "invalid HexValue"; break;

                default: s = "error " + n; break;
            }
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public void SemErr(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public void SemErr(string s)
        {
            errorStream.WriteLine(s);
            count++;
        }

        public void Warning(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public void Warning(string s)
        {
            errorStream.WriteLine(s);
        }
    } // Errors


    internal class FatalError : Exception
    {
        public FatalError(string m) : base(m) { }
    }

}