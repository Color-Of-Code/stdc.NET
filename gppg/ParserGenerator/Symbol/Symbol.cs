// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)




namespace QUT.GPGen
{
    internal abstract class Symbol
    {
        private string name;
        internal string kind;

        internal abstract int num
        {
            get;
        }

        internal Symbol(string name)
        {
            this.name = name;
        }

        //protected void Rename(string newname)
        //{
        //    this.name = newname;
        //}


        public override string ToString()
        {
            return name;
        }


        internal abstract bool IsNullable();
    }
}