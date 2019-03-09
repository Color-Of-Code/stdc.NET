// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    internal sealed class LexCategory
    {
        string name;
        string verb;
        LexSpan vrbSpan;
        bool hasPred;
        internal RegExTree regX;

        internal LexCategory(string nam, string vrb, LexSpan spn)
        {
            vrbSpan = spn;
            verb = vrb;
            name = nam;
        }

        internal bool HasPredicate
        {
            get { return hasPred; }
            set { hasPred = value; }
        }

        internal string Name { get { return name; } }

        internal string PredDummyName { get { return "PRED_" + name + "_DUMMY"; } }

        internal void ParseRE(AAST aast) { regX = new AAST.ReParser(verb, vrbSpan, aast).Parse(); }
    }

}
