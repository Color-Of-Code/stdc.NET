// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class LexCategory
    {
        private string _verb;
        private LexSpan _verbSpan;
        public RegExTree RegularExpressionTree {get; private set;}

        internal LexCategory(string name, string vrb, LexSpan spn)
        {
            _verbSpan = spn;
            _verb = vrb;
            Name = name;
        }

        internal bool HasPredicate { get; set; }

        internal string Name { get; private set; }

        internal string PredDummyName { get { return "PRED_" + Name + "_DUMMY"; } }

        internal void ParseRegularExpression(AAST aast)
        {
            RegularExpressionTree = new AAST.RegularExpressionParser(_verb, _verbSpan, aast).Parse();
        }
    }

}
