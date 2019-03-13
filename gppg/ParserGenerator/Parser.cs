// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.0.0.0
// Machine:  dehaan
// DateTime: 2019-03-13T21:21:26Z
// UserName: jaap
// Input file <gppg.y - 2019-03-13T21:17:23Z>

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using QUT.Gplib;

namespace QUT.GPGen.Parser
{
internal enum Token {
    error=126,
    EOF=127,
    codeStart=128,
    codeEnd=129,
    ident=130,
    anchoredSymbol=131,
    number=132,
    filePath=133,
    litstring=134,
    verbatim=135,
    litchar=136,
    kwPCPC=137,
    kwLbrace=138,
    kwRbrace=139,
    kwToken=140,
    kwType=141,
    kwLeft=142,
    kwRight=143,
    kwNonassoc=144,
    kwPrec=145,
    kwStart=146,
    kwUnion=147,
    kwDefines=148,
    kwLocations=149,
    kwNamespace=150,
    kwPartial=151,
    kwOutput=152,
    kwParsertype=153,
    kwTokentype=154,
    kwScanbasetype=155,
    kwUsing=156,
    kwVisibility=157,
    kwYYSTYPE=158,
    kwYYLTYPE=159,
    maxParseToken=160,
    errTok=161
};

internal partial struct ValueType
{ public int iVal; 
         public List<string> stringList;
         public List<IToken> tokenList;
         public IToken tokenInfo; 
         public Production prod;
         public ActionProxy proxy;
       }
// Abstract base class for GPLEX scanners
internal abstract class ScanBase : AbstractScanner<ValueType,LexSpan> {
  private LexSpan __yylloc = new LexSpan();
  public override LexSpan yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

internal partial class Parser: ShiftReduceParser<ValueType, LexSpan>
{
  // Verbatim content from gppg.y - 2019-03-13T21:17:23Z
/*
 *  Parser spec for GPPG
 *  gppg.y: Author: John Gough, August 2008
 *  Process with > GPPG /gplex /no-lines gppg.y
 */
  // End verbatim content from gppg.y - 2019-03-13T21:17:23Z

#pragma warning disable 649
  private static Dictionary<int, string> aliasses;
#pragma warning restore 649
  private static Rule[] rules = new Rule[108];
  private static State[] states = new State[152];
  private static string[] nonTerms = new string[] {
      "TokenList", "TokenDecl", "NtSymbolList", "Symbols", "SymbolsOpt", "RightHandSide", 
      "RHStermList", "Action", "PrecOptAndAction", "Program", "$accept", "DefinitionSectionOpt", 
      "Divider", "RulesSection", "EpilogOpt", "CodeBlock", "Definitions", "Definition", 
      "Declaration", "KindOpt", "Kind", "NtSymbol", "TypeNameOpt", "UnionTypeConstructor", 
      "DottedName", "SemiOpt", "TypeConstructor", "CommaOpt", "BadSeparator", 
      "DeclList", "OneDecl", "TypeConstructorSeq", "ARule", "RuleProlog", "AlternativesOpt", 
      "SymOrLit", };

  static Parser() {
    states[0] = new State(new int[]{138,46,140,50,141,79,142,88,143,91,144,94,146,97,147,99,149,123,148,124,151,125,150,126,156,128,152,132,155,137,153,139,157,141,154,143,158,145,159,147,126,150,137,-11},new int[]{-10,1,-12,3,-17,44,-18,149,-19,49});
    states[1] = new State(new int[]{127,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{137,43},new int[]{-13,4});
    states[4] = new State(new int[]{131,39,126,42},new int[]{-14,5,-33,41,-34,13});
    states[5] = new State(new int[]{137,8,131,39,127,-6},new int[]{-15,6,-33,7,-34,13});
    states[6] = new State(-2);
    states[7] = new State(-84);
    states[8] = new State(new int[]{128,10,127,-8},new int[]{-16,9});
    states[9] = new State(-5);
    states[10] = new State(new int[]{129,11,126,12});
    states[11] = new State(-7);
    states[12] = new State(-9);
    states[13] = new State(new int[]{130,24,136,25,134,26,123,29,145,33,59,-91,124,-91},new int[]{-6,14,-7,19,-4,36,-36,27,-9,37,-8,28});
    states[14] = new State(-89,new int[]{-35,15});
    states[15] = new State(new int[]{59,16,124,17});
    states[16] = new State(-87);
    states[17] = new State(new int[]{130,24,136,25,134,26,123,29,145,33,59,-91,124,-91},new int[]{-6,18,-7,19,-4,36,-36,27,-9,37,-8,28});
    states[18] = new State(-90);
    states[19] = new State(new int[]{123,29,145,33,59,-92,124,-92},new int[]{-9,20,-8,28});
    states[20] = new State(new int[]{130,24,136,25,134,26,123,-96,145,-96,59,-96,124,-96},new int[]{-5,21,-4,22,-36,27});
    states[21] = new State(-95);
    states[22] = new State(new int[]{130,24,136,25,134,26,123,-97,145,-97,59,-97,124,-97},new int[]{-36,23});
    states[23] = new State(-99);
    states[24] = new State(-100);
    states[25] = new State(-101);
    states[26] = new State(-102);
    states[27] = new State(-98);
    states[28] = new State(-103);
    states[29] = new State(new int[]{128,10,125,-8,126,-8},new int[]{-16,30});
    states[30] = new State(new int[]{125,31,126,32});
    states[31] = new State(-106);
    states[32] = new State(-107);
    states[33] = new State(new int[]{130,34});
    states[34] = new State(new int[]{123,29,130,-105,136,-105,134,-105,145,-105,59,-105,124,-105},new int[]{-8,35});
    states[35] = new State(-104);
    states[36] = new State(new int[]{130,24,136,25,134,26,123,-93,145,-93,59,-93,124,-93},new int[]{-36,23});
    states[37] = new State(new int[]{130,24,136,25,134,26,123,-96,145,-96,59,-96,124,-96},new int[]{-5,38,-4,22,-36,27});
    states[38] = new State(-94);
    states[39] = new State(new int[]{58,40});
    states[40] = new State(-88);
    states[41] = new State(-85);
    states[42] = new State(-86);
    states[43] = new State(-4);
    states[44] = new State(new int[]{138,46,140,50,141,79,142,88,143,91,144,94,146,97,147,99,149,123,148,124,151,125,150,126,156,128,152,132,155,137,153,139,157,141,154,143,158,145,159,147,137,-10},new int[]{-18,45,-19,49});
    states[45] = new State(-13);
    states[46] = new State(new int[]{128,10,139,-8},new int[]{-16,47});
    states[47] = new State(new int[]{139,48});
    states[48] = new State(-16);
    states[49] = new State(-17);
    states[50] = new State(new int[]{60,74,130,-44,136,-44},new int[]{-20,51,-21,73});
    states[51] = new State(new int[]{130,55,136,59},new int[]{-1,52,-2,72});
    states[52] = new State(new int[]{44,61,59,62,58,63,60,64,62,65,40,66,41,67,91,68,93,69,123,70,125,71,138,-18,140,-18,141,-18,142,-18,143,-18,144,-18,146,-18,147,-18,149,-18,148,-18,151,-18,150,-18,156,-18,152,-18,155,-18,153,-18,157,-18,154,-18,158,-18,159,-18,137,-18,130,-71,136,-71},new int[]{-28,53,-29,60});
    states[53] = new State(new int[]{130,55,136,59},new int[]{-2,54});
    states[54] = new State(-48);
    states[55] = new State(new int[]{132,56,134,57,44,-52,59,-52,58,-52,60,-52,62,-52,40,-52,41,-52,91,-52,93,-52,123,-52,125,-52,130,-52,136,-52,138,-52,140,-52,141,-52,142,-52,143,-52,144,-52,146,-52,147,-52,149,-52,148,-52,151,-52,150,-52,156,-52,152,-52,155,-52,153,-52,157,-52,154,-52,158,-52,159,-52,137,-52});
    states[56] = new State(-50);
    states[57] = new State(new int[]{132,58,44,-53,59,-53,58,-53,60,-53,62,-53,40,-53,41,-53,91,-53,93,-53,123,-53,125,-53,130,-53,136,-53,138,-53,140,-53,141,-53,142,-53,143,-53,144,-53,146,-53,147,-53,149,-53,148,-53,151,-53,150,-53,156,-53,152,-53,155,-53,153,-53,157,-53,154,-53,158,-53,159,-53,137,-53});
    states[58] = new State(-51);
    states[59] = new State(-54);
    states[60] = new State(-49);
    states[61] = new State(-70);
    states[62] = new State(-74);
    states[63] = new State(-75);
    states[64] = new State(-76);
    states[65] = new State(-77);
    states[66] = new State(-78);
    states[67] = new State(-79);
    states[68] = new State(-80);
    states[69] = new State(-81);
    states[70] = new State(-82);
    states[71] = new State(-83);
    states[72] = new State(-47);
    states[73] = new State(-43);
    states[74] = new State(new int[]{130,75,126,77});
    states[75] = new State(new int[]{62,76});
    states[76] = new State(-45);
    states[77] = new State(new int[]{62,78});
    states[78] = new State(-46);
    states[79] = new State(new int[]{60,74},new int[]{-21,80});
    states[80] = new State(new int[]{130,84,131,85},new int[]{-3,81,-22,87});
    states[81] = new State(new int[]{44,61,59,62,58,63,60,64,62,65,40,66,41,67,91,68,93,69,123,70,125,71,138,-19,140,-19,141,-19,142,-19,143,-19,144,-19,146,-19,147,-19,149,-19,148,-19,151,-19,150,-19,156,-19,152,-19,155,-19,153,-19,157,-19,154,-19,158,-19,159,-19,137,-19,130,-71,131,-71},new int[]{-28,82,-29,86});
    states[82] = new State(new int[]{130,84,131,85},new int[]{-22,83});
    states[83] = new State(-56);
    states[84] = new State(-58);
    states[85] = new State(-59);
    states[86] = new State(-57);
    states[87] = new State(-55);
    states[88] = new State(new int[]{60,74,130,-44,136,-44},new int[]{-20,89,-21,73});
    states[89] = new State(new int[]{130,55,136,59},new int[]{-1,90,-2,72});
    states[90] = new State(new int[]{44,61,59,62,58,63,60,64,62,65,40,66,41,67,91,68,93,69,123,70,125,71,138,-20,140,-20,141,-20,142,-20,143,-20,144,-20,146,-20,147,-20,149,-20,148,-20,151,-20,150,-20,156,-20,152,-20,155,-20,153,-20,157,-20,154,-20,158,-20,159,-20,137,-20,130,-71,136,-71},new int[]{-28,53,-29,60});
    states[91] = new State(new int[]{60,74,130,-44,136,-44},new int[]{-20,92,-21,73});
    states[92] = new State(new int[]{130,55,136,59},new int[]{-1,93,-2,72});
    states[93] = new State(new int[]{44,61,59,62,58,63,60,64,62,65,40,66,41,67,91,68,93,69,123,70,125,71,138,-21,140,-21,141,-21,142,-21,143,-21,144,-21,146,-21,147,-21,149,-21,148,-21,151,-21,150,-21,156,-21,152,-21,155,-21,153,-21,157,-21,154,-21,158,-21,159,-21,137,-21,130,-71,136,-71},new int[]{-28,53,-29,60});
    states[94] = new State(new int[]{60,74,130,-44,136,-44},new int[]{-20,95,-21,73});
    states[95] = new State(new int[]{130,55,136,59},new int[]{-1,96,-2,72});
    states[96] = new State(new int[]{44,61,59,62,58,63,60,64,62,65,40,66,41,67,91,68,93,69,123,70,125,71,138,-22,140,-22,141,-22,142,-22,143,-22,144,-22,146,-22,147,-22,149,-22,148,-22,151,-22,150,-22,156,-22,152,-22,155,-22,153,-22,157,-22,154,-22,158,-22,159,-22,137,-22,130,-71,136,-71},new int[]{-28,53,-29,60});
    states[97] = new State(new int[]{130,84,131,85},new int[]{-22,98});
    states[98] = new State(-23);
    states[99] = new State(new int[]{130,122,123,-40},new int[]{-23,100});
    states[100] = new State(new int[]{123,102},new int[]{-24,101});
    states[101] = new State(-24);
    states[102] = new State(new int[]{130,118,126,121},new int[]{-30,103,-31,120,-32,106,-27,119,-25,110});
    states[103] = new State(new int[]{125,104,130,118},new int[]{-31,105,-32,106,-27,119,-25,110});
    states[104] = new State(-63);
    states[105] = new State(-65);
    states[106] = new State(new int[]{130,107},new int[]{-27,109,-25,110});
    states[107] = new State(new int[]{59,108,91,-42,46,-42,60,-42,130,-42});
    states[108] = new State(-67);
    states[109] = new State(-69);
    states[110] = new State(new int[]{91,111,46,113,60,115,130,-62,62,-62,138,-62,140,-62,141,-62,142,-62,143,-62,144,-62,146,-62,147,-62,149,-62,148,-62,151,-62,150,-62,156,-62,152,-62,155,-62,153,-62,157,-62,154,-62,158,-62,159,-62,137,-62});
    states[111] = new State(new int[]{93,112});
    states[112] = new State(-60);
    states[113] = new State(new int[]{130,114});
    states[114] = new State(-41);
    states[115] = new State(new int[]{130,118},new int[]{-27,116,-25,110});
    states[116] = new State(new int[]{62,117});
    states[117] = new State(-61);
    states[118] = new State(-42);
    states[119] = new State(-68);
    states[120] = new State(-64);
    states[121] = new State(-66);
    states[122] = new State(-39);
    states[123] = new State(-25);
    states[124] = new State(-26);
    states[125] = new State(-27);
    states[126] = new State(new int[]{130,118},new int[]{-25,127});
    states[127] = new State(new int[]{46,113,138,-28,140,-28,141,-28,142,-28,143,-28,144,-28,146,-28,147,-28,149,-28,148,-28,151,-28,150,-28,156,-28,152,-28,155,-28,153,-28,157,-28,154,-28,158,-28,159,-28,137,-28});
    states[128] = new State(new int[]{130,118},new int[]{-25,129});
    states[129] = new State(new int[]{46,113,59,131,138,-73,140,-73,141,-73,142,-73,143,-73,144,-73,146,-73,147,-73,149,-73,148,-73,151,-73,150,-73,156,-73,152,-73,155,-73,153,-73,157,-73,154,-73,158,-73,159,-73,137,-73},new int[]{-26,130});
    states[130] = new State(-29);
    states[131] = new State(-72);
    states[132] = new State(new int[]{61,133});
    states[133] = new State(new int[]{133,134,134,135,135,136});
    states[134] = new State(-30);
    states[135] = new State(-31);
    states[136] = new State(-32);
    states[137] = new State(new int[]{130,138});
    states[138] = new State(-33);
    states[139] = new State(new int[]{130,140});
    states[140] = new State(-34);
    states[141] = new State(new int[]{130,142});
    states[142] = new State(-35);
    states[143] = new State(new int[]{130,144});
    states[144] = new State(-36);
    states[145] = new State(new int[]{130,118},new int[]{-27,146,-25,110});
    states[146] = new State(-37);
    states[147] = new State(new int[]{130,118},new int[]{-27,148,-25,110});
    states[148] = new State(-38);
    states[149] = new State(-14);
    states[150] = new State(new int[]{138,46,140,50,141,79,142,88,143,91,144,94,146,97,147,99,149,123,148,124,151,125,150,126,156,128,152,132,155,137,153,139,157,141,154,143,158,145,159,147,137,-12,127,-3},new int[]{-18,151,-19,49});
    states[151] = new State(-15);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-11, new int[]{-10,127});
    rules[2] = new Rule(-10, new int[]{-12,-13,-14,-15});
    rules[3] = new Rule(-10, new int[]{126});
    rules[4] = new Rule(-13, new int[]{137});
    rules[5] = new Rule(-15, new int[]{137,-16});
    rules[6] = new Rule(-15, new int[]{});
    rules[7] = new Rule(-16, new int[]{128,129});
    rules[8] = new Rule(-16, new int[]{});
    rules[9] = new Rule(-16, new int[]{128,126});
    rules[10] = new Rule(-12, new int[]{-17});
    rules[11] = new Rule(-12, new int[]{});
    rules[12] = new Rule(-12, new int[]{126});
    rules[13] = new Rule(-17, new int[]{-17,-18});
    rules[14] = new Rule(-17, new int[]{-18});
    rules[15] = new Rule(-17, new int[]{126,-18});
    rules[16] = new Rule(-18, new int[]{138,-16,139});
    rules[17] = new Rule(-18, new int[]{-19});
    rules[18] = new Rule(-19, new int[]{140,-20,-1});
    rules[19] = new Rule(-19, new int[]{141,-21,-3});
    rules[20] = new Rule(-19, new int[]{142,-20,-1});
    rules[21] = new Rule(-19, new int[]{143,-20,-1});
    rules[22] = new Rule(-19, new int[]{144,-20,-1});
    rules[23] = new Rule(-19, new int[]{146,-22});
    rules[24] = new Rule(-19, new int[]{147,-23,-24});
    rules[25] = new Rule(-19, new int[]{149});
    rules[26] = new Rule(-19, new int[]{148});
    rules[27] = new Rule(-19, new int[]{151});
    rules[28] = new Rule(-19, new int[]{150,-25});
    rules[29] = new Rule(-19, new int[]{156,-25,-26});
    rules[30] = new Rule(-19, new int[]{152,61,133});
    rules[31] = new Rule(-19, new int[]{152,61,134});
    rules[32] = new Rule(-19, new int[]{152,61,135});
    rules[33] = new Rule(-19, new int[]{155,130});
    rules[34] = new Rule(-19, new int[]{153,130});
    rules[35] = new Rule(-19, new int[]{157,130});
    rules[36] = new Rule(-19, new int[]{154,130});
    rules[37] = new Rule(-19, new int[]{158,-27});
    rules[38] = new Rule(-19, new int[]{159,-27});
    rules[39] = new Rule(-23, new int[]{130});
    rules[40] = new Rule(-23, new int[]{});
    rules[41] = new Rule(-25, new int[]{-25,46,130});
    rules[42] = new Rule(-25, new int[]{130});
    rules[43] = new Rule(-20, new int[]{-21});
    rules[44] = new Rule(-20, new int[]{});
    rules[45] = new Rule(-21, new int[]{60,130,62});
    rules[46] = new Rule(-21, new int[]{60,126,62});
    rules[47] = new Rule(-1, new int[]{-2});
    rules[48] = new Rule(-1, new int[]{-1,-28,-2});
    rules[49] = new Rule(-1, new int[]{-1,-29});
    rules[50] = new Rule(-2, new int[]{130,132});
    rules[51] = new Rule(-2, new int[]{130,134,132});
    rules[52] = new Rule(-2, new int[]{130});
    rules[53] = new Rule(-2, new int[]{130,134});
    rules[54] = new Rule(-2, new int[]{136});
    rules[55] = new Rule(-3, new int[]{-22});
    rules[56] = new Rule(-3, new int[]{-3,-28,-22});
    rules[57] = new Rule(-3, new int[]{-3,-29});
    rules[58] = new Rule(-22, new int[]{130});
    rules[59] = new Rule(-22, new int[]{131});
    rules[60] = new Rule(-27, new int[]{-25,91,93});
    rules[61] = new Rule(-27, new int[]{-25,60,-27,62});
    rules[62] = new Rule(-27, new int[]{-25});
    rules[63] = new Rule(-24, new int[]{123,-30,125});
    rules[64] = new Rule(-30, new int[]{-31});
    rules[65] = new Rule(-30, new int[]{-30,-31});
    rules[66] = new Rule(-30, new int[]{126});
    rules[67] = new Rule(-31, new int[]{-32,130,59});
    rules[68] = new Rule(-32, new int[]{-27});
    rules[69] = new Rule(-32, new int[]{-32,-27});
    rules[70] = new Rule(-28, new int[]{44});
    rules[71] = new Rule(-28, new int[]{});
    rules[72] = new Rule(-26, new int[]{59});
    rules[73] = new Rule(-26, new int[]{});
    rules[74] = new Rule(-29, new int[]{59});
    rules[75] = new Rule(-29, new int[]{58});
    rules[76] = new Rule(-29, new int[]{60});
    rules[77] = new Rule(-29, new int[]{62});
    rules[78] = new Rule(-29, new int[]{40});
    rules[79] = new Rule(-29, new int[]{41});
    rules[80] = new Rule(-29, new int[]{91});
    rules[81] = new Rule(-29, new int[]{93});
    rules[82] = new Rule(-29, new int[]{123});
    rules[83] = new Rule(-29, new int[]{125});
    rules[84] = new Rule(-14, new int[]{-14,-33});
    rules[85] = new Rule(-14, new int[]{-33});
    rules[86] = new Rule(-14, new int[]{126});
    rules[87] = new Rule(-33, new int[]{-34,-6,-35,59});
    rules[88] = new Rule(-34, new int[]{131,58});
    rules[89] = new Rule(-35, new int[]{});
    rules[90] = new Rule(-35, new int[]{-35,124,-6});
    rules[91] = new Rule(-6, new int[]{});
    rules[92] = new Rule(-6, new int[]{-7});
    rules[93] = new Rule(-7, new int[]{-4});
    rules[94] = new Rule(-7, new int[]{-9,-5});
    rules[95] = new Rule(-7, new int[]{-7,-9,-5});
    rules[96] = new Rule(-5, new int[]{});
    rules[97] = new Rule(-5, new int[]{-4});
    rules[98] = new Rule(-4, new int[]{-36});
    rules[99] = new Rule(-4, new int[]{-4,-36});
    rules[100] = new Rule(-36, new int[]{130});
    rules[101] = new Rule(-36, new int[]{136});
    rules[102] = new Rule(-36, new int[]{134});
    rules[103] = new Rule(-9, new int[]{-8});
    rules[104] = new Rule(-9, new int[]{145,130,-8});
    rules[105] = new Rule(-9, new int[]{145,130});
    rules[106] = new Rule(-8, new int[]{123,-16,125});
    rules[107] = new Rule(-8, new int[]{123,-16,126});

    aliasses = new Dictionary<int, string>();
    aliasses.Add(137, "\"%%\"");
    aliasses.Add(138, "\"%{\"");
    aliasses.Add(139, "\"%}\"");
    aliasses.Add(140, "\"%token\"");
    aliasses.Add(141, "\"%type\"");
    aliasses.Add(142, "\"%left\"");
    aliasses.Add(143, "\"%right\"");
    aliasses.Add(144, "\"%nonassoc\"");
    aliasses.Add(145, "\"%prec\"");
    aliasses.Add(146, "\"%start\"");
    aliasses.Add(147, "\"%union\"");
    aliasses.Add(148, "\"%defines\"");
    aliasses.Add(149, "\"%locations\"");
    aliasses.Add(150, "\"%namespace\"");
    aliasses.Add(151, "\"%partial\"");
    aliasses.Add(152, "\"%output\"");
    aliasses.Add(153, "\"%parsertype\"");
    aliasses.Add(154, "\"%tokentype\"");
    aliasses.Add(155, "\"%scanbasetype\"");
    aliasses.Add(156, "\"%using\"");
    aliasses.Add(157, "\"%visibility\"");
    aliasses.Add(158, "\"%YYSTYPE\"");
    aliasses.Add(159, "\"%YYLTYPE\"");
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Token.error, (int)Token.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
    switch (action)
    {
      case 4: // Divider -> "%%"
{ TidyUpDefinitions(LocationStack[LocationStack.Depth-1]); }
        break;
      case 5: // EpilogOpt -> "%%", CodeBlock
{ grammar.epilogCode = LocationStack[LocationStack.Depth-1]; }
        break;
      case 7: // CodeBlock -> codeStart, codeEnd
{ /* default location action @$ = @1.Merge(@2); */ }
        break;
      case 9: // CodeBlock -> codeStart, error
{ handler.ListError(LocationStack[LocationStack.Depth-2], 77); }
        break;
      case 16: // Definition -> "%{", CodeBlock, "%}"
{ grammar.prologCode.Add(LocationStack[LocationStack.Depth-2]); }
        break;
      case 18: // Declaration -> "%token", KindOpt, TokenList
{ DeclareTokens(PrecedenceType.token, LocationStack[LocationStack.Depth-2].ToString(), ValueStack[ValueStack.Depth-1].tokenList); }
        break;
      case 19: // Declaration -> "%type", Kind, NtSymbolList
{
						  string kind = LocationStack[LocationStack.Depth-2].ToString();
						  DeclareNtKind(kind, ValueStack[ValueStack.Depth-1].stringList);
						}
        break;
      case 20: // Declaration -> "%left", KindOpt, TokenList
{ DeclareTokens(PrecedenceType.left, LocationStack[LocationStack.Depth-2].ToString(), ValueStack[ValueStack.Depth-1].tokenList); }
        break;
      case 21: // Declaration -> "%right", KindOpt, TokenList
{ DeclareTokens(PrecedenceType.right, LocationStack[LocationStack.Depth-2].ToString(), ValueStack[ValueStack.Depth-1].tokenList); }
        break;
      case 22: // Declaration -> "%nonassoc", KindOpt, TokenList
{ DeclareTokens(PrecedenceType.nonassoc, LocationStack[LocationStack.Depth-2].ToString(), ValueStack[ValueStack.Depth-1].tokenList); }
        break;
      case 23: // Declaration -> "%start", NtSymbol
{ grammar.startSymbol = grammar.LookupNonTerminal(LocationStack[LocationStack.Depth-1].ToString()); }
        break;
      case 24: // Declaration -> "%union", TypeNameOpt, UnionTypeConstructor
{ grammar.unionType = LocationStack[LocationStack.Depth-1]; }
        break;
      case 25: // Declaration -> "%locations"
{  handler.ListError(LocationStack[LocationStack.Depth-1], 101); }
        break;
      case 26: // Declaration -> "%defines"
{ GPCG.Defines = true; }
        break;
      case 27: // Declaration -> "%partial"
{ grammar.IsPartial = true; }
        break;
      case 28: // Declaration -> "%namespace", DottedName
{ grammar.Namespace = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 29: // Declaration -> "%using", DottedName, SemiOpt
{ grammar.usingList.Add(LocationStack[LocationStack.Depth-2].ToString()); }
        break;
      case 30: // Declaration -> "%output", '=', filePath
{ grammar.OutFileName = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 31: // Declaration -> "%output", '=', litstring
{ grammar.OutFileName = GetLitString(LocationStack[LocationStack.Depth-1]); }
        break;
      case 32: // Declaration -> "%output", '=', verbatim
{ grammar.OutFileName = GetVerbatimString(LocationStack[LocationStack.Depth-1]); }
        break;
      case 33: // Declaration -> "%scanbasetype", ident
{ grammar.ScanBaseName = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 34: // Declaration -> "%parsertype", ident
{ grammar.ParserName = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 35: // Declaration -> "%visibility", ident
{ grammar.Visibility = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 36: // Declaration -> "%tokentype", ident
{ grammar.TokenName = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 37: // Declaration -> "%YYSTYPE", TypeConstructor
{ SetSemanticType(LocationStack[LocationStack.Depth-1]); }
        break;
      case 38: // Declaration -> "%YYLTYPE", TypeConstructor
{ grammar.LocationTypeName = LocationStack[LocationStack.Depth-1].ToString(); }
        break;
      case 39: // TypeNameOpt -> ident
{ SetSemanticType(LocationStack[LocationStack.Depth-1]); }
        break;
      case 45: // Kind -> '<', ident, '>'
{ CurrentLocationSpan = LocationStack[LocationStack.Depth-2]; }
        break;
      case 47: // TokenList -> TokenDecl
{
                          CurrentSemanticValue.tokenList = new List<IToken>();
                          CurrentSemanticValue.tokenList.Add(ValueStack[ValueStack.Depth-1].tokenInfo);
                        }
        break;
      case 48: // TokenList -> TokenList, CommaOpt, TokenDecl
{ ValueStack[ValueStack.Depth-3].tokenList.Add(ValueStack[ValueStack.Depth-1].tokenInfo); CurrentSemanticValue.tokenList = ValueStack[ValueStack.Depth-3].tokenList;}
        break;
      case 49: // TokenList -> TokenList, BadSeparator
{ handler.ListError(LocationStack[LocationStack.Depth-1], 75); CurrentSemanticValue.tokenList = ValueStack[ValueStack.Depth-2].tokenList; }
        break;
      case 50: // TokenDecl -> ident, number
{ 
                          handler.ListError(LocationStack[LocationStack.Depth-1], 100); 
                          CurrentSemanticValue.tokenInfo = new TokenInfo(LocationStack[LocationStack.Depth-2], null);
                        }
        break;
      case 51: // TokenDecl -> ident, litstring, number
{ 
                          handler.ListError(LocationStack[LocationStack.Depth-2], 100); 
                          CurrentSemanticValue.tokenInfo = new TokenInfo(LocationStack[LocationStack.Depth-3], LocationStack[LocationStack.Depth-2]);
                        }
        break;
      case 52: // TokenDecl -> ident
{ CurrentSemanticValue.tokenInfo = new TokenInfo(LocationStack[LocationStack.Depth-1], null); }
        break;
      case 53: // TokenDecl -> ident, litstring
{ CurrentSemanticValue.tokenInfo = new TokenInfo(LocationStack[LocationStack.Depth-2], LocationStack[LocationStack.Depth-1]); }
        break;
      case 54: // TokenDecl -> litchar
{ CurrentSemanticValue.tokenInfo = new TokenInfo(LocationStack[LocationStack.Depth-1], null); }
        break;
      case 55: // NtSymbolList -> NtSymbol
{ 
                          CurrentSemanticValue.stringList = new List<string>();
						  CurrentSemanticValue.stringList.Add(LocationStack[LocationStack.Depth-1].ToString()); 
						}
        break;
      case 56: // NtSymbolList -> NtSymbolList, CommaOpt, NtSymbol
{ ValueStack[ValueStack.Depth-3].stringList.Add(LocationStack[LocationStack.Depth-1].ToString()); CurrentSemanticValue.stringList = ValueStack[ValueStack.Depth-3].stringList; }
        break;
      case 57: // NtSymbolList -> NtSymbolList, BadSeparator
{ handler.ListError(LocationStack[LocationStack.Depth-1], 75); CurrentSemanticValue.stringList = ValueStack[ValueStack.Depth-2].stringList; }
        break;
      case 87: // ARule -> RuleProlog, RightHandSide, AlternativesOpt, ';'
{ ClearCurrentLHS(); }
        break;
      case 88: // RuleProlog -> anchoredSymbol, ':'
{ SetCurrentLHS(LocationStack[LocationStack.Depth-2]); }
        break;
      case 91: // RightHandSide -> /* empty */
{ CurrentSemanticValue.prod = NewProduction(); FinalizeProduction(CurrentSemanticValue.prod); }
        break;
      case 92: // RightHandSide -> RHStermList
{ CurrentSemanticValue.prod = ValueStack[ValueStack.Depth-1].prod; FinalizeProduction(CurrentSemanticValue.prod); }
        break;
      case 93: // RHStermList -> Symbols
{ CurrentSemanticValue.prod = NewProduction(ValueStack[ValueStack.Depth-1].stringList, null); }
        break;
      case 94: // RHStermList -> PrecOptAndAction, SymbolsOpt
{
                          CurrentSemanticValue.prod = NewProduction(null, ValueStack[ValueStack.Depth-2].proxy);
                          AddSymbolsToProduction(CurrentSemanticValue.prod, ValueStack[ValueStack.Depth-1].stringList);
                        }
        break;
      case 95: // RHStermList -> RHStermList, PrecOptAndAction, SymbolsOpt
{
                          AddActionToProduction(ValueStack[ValueStack.Depth-3].prod, ValueStack[ValueStack.Depth-2].proxy);
                          AddSymbolsToProduction(ValueStack[ValueStack.Depth-3].prod, ValueStack[ValueStack.Depth-1].stringList);
                          CurrentSemanticValue.prod = ValueStack[ValueStack.Depth-3].prod;
                        }
        break;
      case 96: // SymbolsOpt -> /* empty */
{ CurrentSemanticValue.stringList = null; }
        break;
      case 98: // Symbols -> SymOrLit
{ CurrentSemanticValue.stringList = new List<string>(); CurrentSemanticValue.stringList.Add(LocationStack[LocationStack.Depth-1].ToString()); }
        break;
      case 99: // Symbols -> Symbols, SymOrLit
{ ValueStack[ValueStack.Depth-2].stringList.Add(LocationStack[LocationStack.Depth-1].ToString());  CurrentSemanticValue.stringList = ValueStack[ValueStack.Depth-2].stringList; }
        break;
      case 103: // PrecOptAndAction -> Action
{ CurrentSemanticValue.proxy = ValueStack[ValueStack.Depth-1].proxy; }
        break;
      case 104: // PrecOptAndAction -> "%prec", ident, Action
{ ValueStack[ValueStack.Depth-1].proxy.precedenceToken = LocationStack[LocationStack.Depth-2]; ValueStack[ValueStack.Depth-1].proxy.precedenceSpan = LocationStack[LocationStack.Depth-3]; CurrentSemanticValue.proxy = ValueStack[ValueStack.Depth-1].proxy; }
        break;
      case 105: // PrecOptAndAction -> "%prec", ident
{ CurrentSemanticValue.proxy = new ActionProxy(LocationStack[LocationStack.Depth-2], LocationStack[LocationStack.Depth-1], null); }
        break;
      case 106: // Action -> '{', CodeBlock, '}'
{ CurrentSemanticValue.proxy = new ActionProxy(null, null, CurrentLocationSpan); }
        break;
    }
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliasses != null && aliasses.ContainsKey(terminal))
        return aliasses[terminal];
    else if (((Token)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Token)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

}
}
