// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  DESKTOP-4T5HHDR
// DateTime: 2021-06-15 00:02:11
// UserName: Molom
// Input file <Parser.y - 2021-06-14 23:57:28>

// options: lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace MiniCompiler
{
public enum Tokens {error=2,EOF=3,Program=4,If=5,Else=6,
    While=7,Read=8,Write=9,Return=10,Int=11,Double=12,
    Bool=13,True=14,False=15,Hex=16,Assign=17,Or=18,
    And=19,BitOr=20,BitAnd=21,Equal=22,NotEqual=23,Less=24,
    LessEqual=25,Greater=26,GreaterEqual=27,Plus=28,Minus=29,Multiply=30,
    Divide=31,Negate=32,BitNegate=33,OpenPar=34,ClosePar=35,OpenBlock=36,
    CloseBlock=37,Comma=38,Endline=39,Eof=40,Ident=41,IntNumber=42,
    DoubleNumber=43,String=44,Error=45};

public struct ValueType
#line 6 "Parser.y"
       {
    public string val;
    public TypeEnum type;
    public SyntaxTree tree;
    public List<SyntaxTree> list;
    public LogicalExpression.Operation logicalOperation;
    public RelationalExpression.Operation relationalOperation;
    public ArithmeticExpression.Operation arithmeticOperation;
    public BinaryExpression.Operation binaryOperation;
}
#line default
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[71];
  private static State[] states = new State[108];
  private static string[] nonTerms = new string[] {
      "programContent", "declaration", "instruction", "output_instruction", "input_instruction", 
      "block_instruction", "conditional_instruction", "loop_instruction", "exp", 
      "number", "logical", "exp_rest", "relational", "additive", "multiplicative", 
      "binary", "unary", "type", "identifiers", "instructions", "declarations", 
      "logical_op", "relational_op", "additive_op", "multiplicative_op", "binary_op", 
      "start", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{4,3,2,106},new int[]{-27,1});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{36,4});
    states[4] = new State(new int[]{11,101,12,102,13,103,9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91,37,-7},new int[]{-1,5,-21,8,-2,104,-18,95,-20,105,-3,93,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[5] = new State(new int[]{37,6});
    states[6] = new State(new int[]{40,7});
    states[7] = new State(-2);
    states[8] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91,11,101,12,102,13,103,37,-5},new int[]{-20,9,-2,94,-3,93,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36,-18,95});
    states[9] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91,37,-4},new int[]{-3,10,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[10] = new State(-16);
    states[11] = new State(-18);
    states[12] = new State(new int[]{44,18,41,20,42,37,43,38,14,39,15,40},new int[]{-9,13,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[13] = new State(new int[]{39,14,38,15});
    states[14] = new State(-66);
    states[15] = new State(new int[]{16,16});
    states[16] = new State(new int[]{39,17});
    states[17] = new State(-67);
    states[18] = new State(new int[]{39,19});
    states[19] = new State(-68);
    states[20] = new State(new int[]{17,21,21,-60,20,-60,30,-60,31,-60,28,-60,29,-60,22,-60,23,-60,24,-60,25,-60,26,-60,27,-60,19,-60,18,-60,39,-60,38,-60,35,-60});
    states[21] = new State(new int[]{41,20,42,37,43,38,14,39,15,40},new int[]{-9,22,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[22] = new State(-33);
    states[23] = new State(new int[]{19,57,18,58,39,-34,38,-34,35,-34},new int[]{-22,24});
    states[24] = new State(new int[]{41,35,42,37,43,38,14,39,15,40},new int[]{-13,25,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[25] = new State(new int[]{22,50,23,51,24,52,25,53,26,54,27,55,19,-35,18,-35,39,-35,38,-35,35,-35},new int[]{-23,26});
    states[26] = new State(new int[]{41,35,42,37,43,38,14,39,15,40},new int[]{-14,27,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[27] = new State(new int[]{28,47,29,48,22,-39,23,-39,24,-39,25,-39,26,-39,27,-39,19,-39,18,-39,39,-39,38,-39,35,-39},new int[]{-24,28});
    states[28] = new State(new int[]{41,35,42,37,43,38,14,39,15,40},new int[]{-15,29,-16,46,-17,43,-12,34,-10,36});
    states[29] = new State(new int[]{30,44,31,45,28,-47,29,-47,22,-47,23,-47,24,-47,25,-47,26,-47,27,-47,19,-47,18,-47,39,-47,38,-47,35,-47},new int[]{-25,30});
    states[30] = new State(new int[]{41,35,42,37,43,38,14,39,15,40},new int[]{-16,31,-17,43,-12,34,-10,36});
    states[31] = new State(new int[]{21,41,20,42,30,-51,31,-51,28,-51,29,-51,22,-51,23,-51,24,-51,25,-51,26,-51,27,-51,19,-51,18,-51,39,-51,38,-51,35,-51},new int[]{-26,32});
    states[32] = new State(new int[]{41,35,42,37,43,38,14,39,15,40},new int[]{-17,33,-12,34,-10,36});
    states[33] = new State(-55);
    states[34] = new State(-59);
    states[35] = new State(-60);
    states[36] = new State(-61);
    states[37] = new State(-62);
    states[38] = new State(-63);
    states[39] = new State(-64);
    states[40] = new State(-65);
    states[41] = new State(-57);
    states[42] = new State(-58);
    states[43] = new State(-56);
    states[44] = new State(-53);
    states[45] = new State(-54);
    states[46] = new State(new int[]{21,41,20,42,30,-52,31,-52,28,-52,29,-52,22,-52,23,-52,24,-52,25,-52,26,-52,27,-52,19,-52,18,-52,39,-52,38,-52,35,-52},new int[]{-26,32});
    states[47] = new State(-49);
    states[48] = new State(-50);
    states[49] = new State(new int[]{30,44,31,45,28,-48,29,-48,22,-48,23,-48,24,-48,25,-48,26,-48,27,-48,19,-48,18,-48,39,-48,38,-48,35,-48},new int[]{-25,30});
    states[50] = new State(-41);
    states[51] = new State(-42);
    states[52] = new State(-43);
    states[53] = new State(-44);
    states[54] = new State(-45);
    states[55] = new State(-46);
    states[56] = new State(new int[]{28,47,29,48,22,-40,23,-40,24,-40,25,-40,26,-40,27,-40,19,-40,18,-40,39,-40,38,-40,35,-40},new int[]{-24,28});
    states[57] = new State(-37);
    states[58] = new State(-38);
    states[59] = new State(new int[]{22,50,23,51,24,52,25,53,26,54,27,55,19,-36,18,-36,39,-36,38,-36,35,-36},new int[]{-23,26});
    states[60] = new State(-19);
    states[61] = new State(new int[]{41,62});
    states[62] = new State(new int[]{39,63,38,64});
    states[63] = new State(-69);
    states[64] = new State(new int[]{16,65});
    states[65] = new State(new int[]{39,66});
    states[66] = new State(-70);
    states[67] = new State(-20);
    states[68] = new State(new int[]{37,92,9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91},new int[]{-20,69,-3,93,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[69] = new State(new int[]{37,70,9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91},new int[]{-3,10,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[70] = new State(-31);
    states[71] = new State(-21);
    states[72] = new State(new int[]{34,73});
    states[73] = new State(new int[]{41,20,42,37,43,38,14,39,15,40},new int[]{-9,74,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[74] = new State(new int[]{35,75});
    states[75] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91},new int[]{-3,76,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[76] = new State(new int[]{6,77,9,-28,8,-28,36,-28,5,-28,7,-28,41,-28,42,-28,43,-28,14,-28,15,-28,10,-28,2,-28,40,-28,37,-28});
    states[77] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91},new int[]{-3,78,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[78] = new State(-29);
    states[79] = new State(-22);
    states[80] = new State(new int[]{34,81});
    states[81] = new State(new int[]{41,20,42,37,43,38,14,39,15,40},new int[]{-9,82,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[82] = new State(new int[]{35,83});
    states[83] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91},new int[]{-3,84,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[84] = new State(-30);
    states[85] = new State(new int[]{39,86});
    states[86] = new State(-23);
    states[87] = new State(new int[]{39,88});
    states[88] = new State(-24);
    states[89] = new State(new int[]{39,90,9,-26,8,-26,36,-26,5,-26,7,-26,41,-26,42,-26,43,-26,14,-26,15,-26,10,-26,2,-26,40,-26,37,-26,6,-26});
    states[90] = new State(-25);
    states[91] = new State(-27);
    states[92] = new State(-32);
    states[93] = new State(-17);
    states[94] = new State(-8);
    states[95] = new State(new int[]{41,100},new int[]{-19,96});
    states[96] = new State(new int[]{39,97,38,98});
    states[97] = new State(-10);
    states[98] = new State(new int[]{41,99});
    states[99] = new State(-14);
    states[100] = new State(-15);
    states[101] = new State(-11);
    states[102] = new State(-12);
    states[103] = new State(-13);
    states[104] = new State(-9);
    states[105] = new State(new int[]{9,12,8,61,36,68,5,72,7,80,41,20,42,37,43,38,14,39,15,40,10,87,2,89,40,91,37,-6},new int[]{-3,10,-4,11,-5,60,-6,67,-7,71,-8,79,-9,85,-11,23,-13,59,-14,56,-15,49,-16,46,-17,43,-12,34,-10,36});
    states[106] = new State(new int[]{40,107});
    states[107] = new State(-3);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-28, new int[]{-27,3});
    rules[2] = new Rule(-27, new int[]{4,36,-1,37,40});
    rules[3] = new Rule(-27, new int[]{2,40});
    rules[4] = new Rule(-1, new int[]{-21,-20});
    rules[5] = new Rule(-1, new int[]{-21});
    rules[6] = new Rule(-1, new int[]{-20});
    rules[7] = new Rule(-1, new int[]{});
    rules[8] = new Rule(-21, new int[]{-21,-2});
    rules[9] = new Rule(-21, new int[]{-2});
    rules[10] = new Rule(-2, new int[]{-18,-19,39});
    rules[11] = new Rule(-18, new int[]{11});
    rules[12] = new Rule(-18, new int[]{12});
    rules[13] = new Rule(-18, new int[]{13});
    rules[14] = new Rule(-19, new int[]{-19,38,41});
    rules[15] = new Rule(-19, new int[]{41});
    rules[16] = new Rule(-20, new int[]{-20,-3});
    rules[17] = new Rule(-20, new int[]{-3});
    rules[18] = new Rule(-3, new int[]{-4});
    rules[19] = new Rule(-3, new int[]{-5});
    rules[20] = new Rule(-3, new int[]{-6});
    rules[21] = new Rule(-3, new int[]{-7});
    rules[22] = new Rule(-3, new int[]{-8});
    rules[23] = new Rule(-3, new int[]{-9,39});
    rules[24] = new Rule(-3, new int[]{10,39});
    rules[25] = new Rule(-3, new int[]{2,39});
    rules[26] = new Rule(-3, new int[]{2});
    rules[27] = new Rule(-3, new int[]{40});
    rules[28] = new Rule(-7, new int[]{5,34,-9,35,-3});
    rules[29] = new Rule(-7, new int[]{5,34,-9,35,-3,6,-3});
    rules[30] = new Rule(-8, new int[]{7,34,-9,35,-3});
    rules[31] = new Rule(-6, new int[]{36,-20,37});
    rules[32] = new Rule(-6, new int[]{36,37});
    rules[33] = new Rule(-9, new int[]{41,17,-9});
    rules[34] = new Rule(-9, new int[]{-11});
    rules[35] = new Rule(-11, new int[]{-11,-22,-13});
    rules[36] = new Rule(-11, new int[]{-13});
    rules[37] = new Rule(-22, new int[]{19});
    rules[38] = new Rule(-22, new int[]{18});
    rules[39] = new Rule(-13, new int[]{-13,-23,-14});
    rules[40] = new Rule(-13, new int[]{-14});
    rules[41] = new Rule(-23, new int[]{22});
    rules[42] = new Rule(-23, new int[]{23});
    rules[43] = new Rule(-23, new int[]{24});
    rules[44] = new Rule(-23, new int[]{25});
    rules[45] = new Rule(-23, new int[]{26});
    rules[46] = new Rule(-23, new int[]{27});
    rules[47] = new Rule(-14, new int[]{-14,-24,-15});
    rules[48] = new Rule(-14, new int[]{-15});
    rules[49] = new Rule(-24, new int[]{28});
    rules[50] = new Rule(-24, new int[]{29});
    rules[51] = new Rule(-15, new int[]{-15,-25,-16});
    rules[52] = new Rule(-15, new int[]{-16});
    rules[53] = new Rule(-25, new int[]{30});
    rules[54] = new Rule(-25, new int[]{31});
    rules[55] = new Rule(-16, new int[]{-16,-26,-17});
    rules[56] = new Rule(-16, new int[]{-17});
    rules[57] = new Rule(-26, new int[]{21});
    rules[58] = new Rule(-26, new int[]{20});
    rules[59] = new Rule(-17, new int[]{-12});
    rules[60] = new Rule(-12, new int[]{41});
    rules[61] = new Rule(-12, new int[]{-10});
    rules[62] = new Rule(-10, new int[]{42});
    rules[63] = new Rule(-10, new int[]{43});
    rules[64] = new Rule(-10, new int[]{14});
    rules[65] = new Rule(-10, new int[]{15});
    rules[66] = new Rule(-4, new int[]{9,-9,39});
    rules[67] = new Rule(-4, new int[]{9,-9,38,16,39});
    rules[68] = new Rule(-4, new int[]{9,44,39});
    rules[69] = new Rule(-5, new int[]{8,41,39});
    rules[70] = new Rule(-5, new int[]{8,41,38,16,39});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // start -> Program, OpenBlock, programContent, CloseBlock, Eof
#line 35 "Parser.y"
        {
            Compiler.Program = new Program(ValueStack[ValueStack.Depth-3].tree, CurrentLocationSpan);
            YYAccept();
        }
#line default
        break;
      case 3: // start -> error, Eof
#line 40 "Parser.y"
        {
            Compiler.Error(LocationStack[LocationStack.Depth-2]);
            yyerrok();
            YYAccept();
        }
#line default
        break;
      case 4: // programContent -> declarations, instructions
#line 49 "Parser.y"
        { CurrentSemanticValue.tree = new ProgramContent(ValueStack[ValueStack.Depth-2].list, ValueStack[ValueStack.Depth-1].list, CurrentLocationSpan); }
#line default
        break;
      case 5: // programContent -> declarations
#line 51 "Parser.y"
        { CurrentSemanticValue.tree = new ProgramContent(ValueStack[ValueStack.Depth-1].list, null, CurrentLocationSpan); }
#line default
        break;
      case 6: // programContent -> instructions
#line 53 "Parser.y"
        { CurrentSemanticValue.tree = new ProgramContent(null, ValueStack[ValueStack.Depth-1].list, CurrentLocationSpan); }
#line default
        break;
      case 7: // programContent -> /* empty */
#line 55 "Parser.y"
        { CurrentSemanticValue.tree = new ProgramContent(null, null, CurrentLocationSpan); }
#line default
        break;
      case 8: // declarations -> declarations, declaration
#line 60 "Parser.y"
        {
            ValueStack[ValueStack.Depth-2].list.Add(ValueStack[ValueStack.Depth-1].tree);
            CurrentSemanticValue.list = ValueStack[ValueStack.Depth-2].list;
        }
#line default
        break;
      case 9: // declarations -> declaration
#line 65 "Parser.y"
        {
            CurrentSemanticValue.list = new List<SyntaxTree>();
            CurrentSemanticValue.list.Add(ValueStack[ValueStack.Depth-1].tree);
        }
#line default
        break;
      case 10: // declaration -> type, identifiers, Endline
#line 73 "Parser.y"
        { CurrentSemanticValue.tree = new Declaration(ValueStack[ValueStack.Depth-3].type, ValueStack[ValueStack.Depth-2].list, CurrentLocationSpan); }
#line default
        break;
      case 11: // type -> Int
#line 78 "Parser.y"
        { CurrentSemanticValue.type = TypeEnum.Int; }
#line default
        break;
      case 12: // type -> Double
#line 80 "Parser.y"
        { CurrentSemanticValue.type = TypeEnum.Double; }
#line default
        break;
      case 13: // type -> Bool
#line 82 "Parser.y"
        { CurrentSemanticValue.type = TypeEnum.Bool; }
#line default
        break;
      case 14: // identifiers -> identifiers, Comma, Ident
#line 87 "Parser.y"
        {
            ValueStack[ValueStack.Depth-3].list.Add(new Identifier(ValueStack[ValueStack.Depth-1].val, LocationStack[LocationStack.Depth-1]));
            CurrentSemanticValue.list = ValueStack[ValueStack.Depth-3].list;
        }
#line default
        break;
      case 15: // identifiers -> Ident
#line 92 "Parser.y"
        {
            CurrentSemanticValue.list = new List<SyntaxTree>();
            CurrentSemanticValue.list.Add(new Identifier(ValueStack[ValueStack.Depth-1].val, LocationStack[LocationStack.Depth-1]));
        }
#line default
        break;
      case 16: // instructions -> instructions, instruction
#line 100 "Parser.y"
        {
            ValueStack[ValueStack.Depth-2].list.Add(ValueStack[ValueStack.Depth-1].tree);
            CurrentSemanticValue.list = ValueStack[ValueStack.Depth-2].list;
        }
#line default
        break;
      case 17: // instructions -> instruction
#line 105 "Parser.y"
        {
            CurrentSemanticValue.list = new List<SyntaxTree>();
            CurrentSemanticValue.list.Add(ValueStack[ValueStack.Depth-1].tree);
        }
#line default
        break;
      case 24: // instruction -> Return, Endline
#line 119 "Parser.y"
        { CurrentSemanticValue.tree = new ReturnInstruction(LocationStack[LocationStack.Depth-2]); }
#line default
        break;
      case 25: // instruction -> error, Endline
#line 121 "Parser.y"
        {
            Compiler.Error(LocationStack[LocationStack.Depth-2]);
            CurrentSemanticValue.tree = null;
            yyerrok();
        }
#line default
        break;
      case 26: // instruction -> error
#line 127 "Parser.y"
        {
            Compiler.Error(LocationStack[LocationStack.Depth-1]);
            CurrentSemanticValue.tree = null;
            yyerrok();
        }
#line default
        break;
      case 27: // instruction -> Eof
#line 133 "Parser.y"
        {
            Compiler.Error(LocationStack[LocationStack.Depth-1], "syntax error - unexpected end of file");
            YYAbort();
        }
#line default
        break;
      case 28: // conditional_instruction -> If, OpenPar, exp, ClosePar, instruction
#line 141 "Parser.y"
        { CurrentSemanticValue.tree = new ConditionalInstruction(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, CurrentLocationSpan); }
#line default
        break;
      case 29: // conditional_instruction -> If, OpenPar, exp, ClosePar, instruction, Else, 
               //                            instruction
#line 143 "Parser.y"
        { CurrentSemanticValue.tree = new ConditionalInstruction(ValueStack[ValueStack.Depth-5].tree, ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, CurrentLocationSpan); }
#line default
        break;
      case 30: // loop_instruction -> While, OpenPar, exp, ClosePar, instruction
#line 148 "Parser.y"
        { CurrentSemanticValue.tree = new LoopInstruction(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, CurrentLocationSpan); }
#line default
        break;
      case 31: // block_instruction -> OpenBlock, instructions, CloseBlock
#line 153 "Parser.y"
        { CurrentSemanticValue.tree = new BlockInstruction(ValueStack[ValueStack.Depth-2].list, CurrentLocationSpan); }
#line default
        break;
      case 32: // block_instruction -> OpenBlock, CloseBlock
#line 155 "Parser.y"
        { CurrentSemanticValue.tree = null; }
#line default
        break;
      case 33: // exp -> Ident, Assign, exp
#line 160 "Parser.y"
        { CurrentSemanticValue.tree = new AssignmentExpression(ValueStack[ValueStack.Depth-3].val, ValueStack[ValueStack.Depth-1].tree, CurrentLocationSpan); }
#line default
        break;
      case 35: // logical -> logical, logical_op, relational
#line 166 "Parser.y"
        { CurrentSemanticValue.tree = new LogicalExpression(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, ValueStack[ValueStack.Depth-2].logicalOperation, CurrentLocationSpan); }
#line default
        break;
      case 37: // logical_op -> And
#line 172 "Parser.y"
        { CurrentSemanticValue.logicalOperation = LogicalExpression.Operation.And; }
#line default
        break;
      case 38: // logical_op -> Or
#line 174 "Parser.y"
        { CurrentSemanticValue.logicalOperation = LogicalExpression.Operation.Or; }
#line default
        break;
      case 39: // relational -> relational, relational_op, additive
#line 179 "Parser.y"
        { CurrentSemanticValue.tree = new RelationalExpression(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, ValueStack[ValueStack.Depth-2].relationalOperation, CurrentLocationSpan); }
#line default
        break;
      case 41: // relational_op -> Equal
#line 185 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.Equal; }
#line default
        break;
      case 42: // relational_op -> NotEqual
#line 187 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.NotEqual; }
#line default
        break;
      case 43: // relational_op -> Less
#line 189 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.Less; }
#line default
        break;
      case 44: // relational_op -> LessEqual
#line 191 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.LessEqual; }
#line default
        break;
      case 45: // relational_op -> Greater
#line 193 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.Greater; }
#line default
        break;
      case 46: // relational_op -> GreaterEqual
#line 195 "Parser.y"
        { CurrentSemanticValue.relationalOperation = RelationalExpression.Operation.GreaterEqual; }
#line default
        break;
      case 47: // additive -> additive, additive_op, multiplicative
#line 200 "Parser.y"
        { CurrentSemanticValue.tree = new ArithmeticExpression(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, ValueStack[ValueStack.Depth-2].arithmeticOperation, CurrentLocationSpan); }
#line default
        break;
      case 49: // additive_op -> Plus
#line 206 "Parser.y"
        { CurrentSemanticValue.arithmeticOperation = ArithmeticExpression.Operation.Addition; }
#line default
        break;
      case 50: // additive_op -> Minus
#line 208 "Parser.y"
        { CurrentSemanticValue.arithmeticOperation = ArithmeticExpression.Operation.Subtraction; }
#line default
        break;
      case 51: // multiplicative -> multiplicative, multiplicative_op, binary
#line 213 "Parser.y"
        { CurrentSemanticValue.tree = new ArithmeticExpression(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, ValueStack[ValueStack.Depth-2].arithmeticOperation, CurrentLocationSpan); }
#line default
        break;
      case 53: // multiplicative_op -> Multiply
#line 219 "Parser.y"
        { CurrentSemanticValue.arithmeticOperation = ArithmeticExpression.Operation.Multiplication; }
#line default
        break;
      case 54: // multiplicative_op -> Divide
#line 221 "Parser.y"
        { CurrentSemanticValue.arithmeticOperation = ArithmeticExpression.Operation.Division; }
#line default
        break;
      case 55: // binary -> binary, binary_op, unary
#line 226 "Parser.y"
        { CurrentSemanticValue.tree = new BinaryExpression(ValueStack[ValueStack.Depth-3].tree, ValueStack[ValueStack.Depth-1].tree, ValueStack[ValueStack.Depth-2].binaryOperation, CurrentLocationSpan); }
#line default
        break;
      case 57: // binary_op -> BitAnd
#line 232 "Parser.y"
        { CurrentSemanticValue.binaryOperation = BinaryExpression.Operation.And; }
#line default
        break;
      case 58: // binary_op -> BitOr
#line 234 "Parser.y"
        { CurrentSemanticValue.binaryOperation = BinaryExpression.Operation.Or; }
#line default
        break;
      case 60: // exp_rest -> Ident
#line 243 "Parser.y"
        { CurrentSemanticValue.tree = new IdentifierExpression(ValueStack[ValueStack.Depth-1].val, CurrentLocationSpan); }
#line default
        break;
      case 62: // number -> IntNumber
#line 249 "Parser.y"
        { CurrentSemanticValue.tree = new NumberExpression(ValueStack[ValueStack.Depth-1].val, TypeEnum.Int, CurrentLocationSpan); }
#line default
        break;
      case 63: // number -> DoubleNumber
#line 251 "Parser.y"
        { CurrentSemanticValue.tree = new NumberExpression(ValueStack[ValueStack.Depth-1].val, TypeEnum.Double, CurrentLocationSpan); }
#line default
        break;
      case 64: // number -> True
#line 253 "Parser.y"
        { CurrentSemanticValue.tree = new NumberExpression("1", TypeEnum.Bool, CurrentLocationSpan); }
#line default
        break;
      case 65: // number -> False
#line 255 "Parser.y"
        { CurrentSemanticValue.tree = new NumberExpression("0", TypeEnum.Bool, CurrentLocationSpan); }
#line default
        break;
      case 66: // output_instruction -> Write, exp, Endline
#line 260 "Parser.y"
        { CurrentSemanticValue.tree = new OutputInstruction(ValueStack[ValueStack.Depth-2].tree, OutputInstruction.Flag.None, CurrentLocationSpan); }
#line default
        break;
      case 67: // output_instruction -> Write, exp, Comma, Hex, Endline
#line 262 "Parser.y"
        { CurrentSemanticValue.tree = new OutputInstruction(ValueStack[ValueStack.Depth-4].tree, OutputInstruction.Flag.Hex, CurrentLocationSpan); }
#line default
        break;
      case 68: // output_instruction -> Write, String, Endline
#line 264 "Parser.y"
        { CurrentSemanticValue.tree = new OutputInstruction(ValueStack[ValueStack.Depth-2].val, CurrentLocationSpan); }
#line default
        break;
      case 69: // input_instruction -> Read, Ident, Endline
#line 269 "Parser.y"
        { CurrentSemanticValue.tree = new InputInstruction(new Identifier(ValueStack[ValueStack.Depth-2].val, LocationStack[LocationStack.Depth-2]), false, CurrentLocationSpan); }
#line default
        break;
      case 70: // input_instruction -> Read, Ident, Comma, Hex, Endline
#line 271 "Parser.y"
        { CurrentSemanticValue.tree = new InputInstruction(new Identifier(ValueStack[ValueStack.Depth-4].val, LocationStack[LocationStack.Depth-4]), true, CurrentLocationSpan); }
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 275 "Parser.y"

public Parser(Scanner scanner) : base(scanner) { }
#line default
}
}
