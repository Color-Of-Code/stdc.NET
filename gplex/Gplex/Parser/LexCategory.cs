// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class LexCategory
    {
        private string _verb;
        private ISpan _verbSpan;
        public RegExTree RegularExpressionTree {get; private set;}

        internal LexCategory(string name, string vrb, ISpan spn)
        {
            _verbSpan = spn;
            _verb = vrb;
            Name = name;
        }

        internal bool HasPredicate { get; set; }

        internal string Name { get; private set; }

        internal string PredDummyName { get { return $"PRED_{Name}_DUMMY"; } }

        internal void ParseRegularExpression(AAST aast)
        {
            // TODO: remove cast
            RegularExpressionTree = new RegularExpressionParser(_verb, (LexSpan)_verbSpan, aast).Parse();
        }
    }

}
