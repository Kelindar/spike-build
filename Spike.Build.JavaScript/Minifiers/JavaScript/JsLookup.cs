// lookup.cs
//
// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace Spike.Build.Client
{
    internal enum JsReferenceType
    {
        Variable,
        Function,
        Constructor
    }


    internal sealed class JsLookup : JsExpression, IJsNameReference
    {
        public JsVariableField VariableField { get; internal set; }

        public bool IsGenerated { get; set; }
        public JsReferenceType RefType { get; set; }
        public string Name { get; set; }

        public bool IsAssignment
        {
            get
            {
                var isAssign = false;

                // see if our parent is a binary operator.
                var binaryOp = Parent as JsBinaryOperator;
                if (binaryOp != null)
                {
                    // if we are, we are an assignment lookup if the binary operator parent is an assignment
                    // and we are the left-hand side.
                    isAssign = binaryOp.IsAssign && binaryOp.Operand1 == this;
                }
                else
                {
                    // not a binary op -- but we might still be an "assignment" if we are an increment or decrement operator.
                    var unaryOp = Parent as JsUnaryOperator;
                    isAssign = unaryOp != null
                        && (unaryOp.OperatorToken == JsToken.Increment || unaryOp.OperatorToken == JsToken.Decrement);

                    if (!isAssign)
                    {
                        // AND if we are the variable of a for-in statement, we are an "assignment".
                        // (if the forIn variable is a var, then it wouldn't be a lookup, so we don't have to worry about
                        // going up past a var-decl intermediate node)
                        var forIn = Parent as JsForIn;
                        isAssign = forIn != null && this == forIn.Variable;
                    }
                }

                return isAssign;
            }
        }

        public JsAstNode AssignmentValue
        {
            get
            {
                JsAstNode value = null;

                // see if our parent is a binary operator.
                var binaryOp = Parent as JsBinaryOperator;
                if (binaryOp != null)
                {
                    // the parent is a binary operator. If it is an assignment operator 
                    // (not including any of the op-assign which depend on an initial value)
                    // then the value we are assigning is the right-hand side of the = operator.
                    value = binaryOp.OperatorToken == JsToken.Assign && binaryOp.Operand1 == this ? binaryOp.Operand2 : null;
                }

                return value;
            }
        }

        public JsLookup(JsContext context, JsParser parser)
            : base(context, parser)
        {
            RefType = JsReferenceType.Variable;
        }

        public override void Accept(IJsVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }

        public override bool IsEquivalentTo(JsAstNode otherNode)
        {
            // this one is tricky. If we have a field assigned, then we are equivalent if the
            // field is the same as the other one. If there is no field, then just check the name
            var otherLookup = otherNode as JsLookup;
            if (otherLookup != null)
            {
                if (VariableField != null)
                {
                    // the variable fields should be the same
                    return VariableField.IsSameField(otherLookup.VariableField);
                }
                else
                {
                    // otherwise the names should be identical
                    return string.CompareOrdinal(Name, otherLookup.Name) == 0;
                }
            }

            // if we get here, we're not equivalent
            return false;
        }

        internal override string GetFunctionGuess(JsAstNode target)
        {
            // return the source name
            return Name;
        }

        private static bool MatchMemberName(JsAstNode node, string lookup, int startIndex, int endIndex)
        {
            // the node needs to be a Member node, and if it is, the appropriate portion of the lookup
            // string should match the name of the member.
            var member = node as JsMember;
            return member != null && string.CompareOrdinal(member.Name, 0, lookup, startIndex, endIndex - startIndex) == 0;
        }

        private static bool MatchesMemberChain(JsAstNode parent, string lookup, int startIndex)
        {
            // get the NEXT period
            var period = lookup.IndexOf('.', startIndex);

            // loop until we run out of periods
            while (period > 0)
            {
                // if the parent isn't a member, or if the name of the parent doesn't match
                // the current identifier in the chain, then we're no match and can bail
                if (!MatchMemberName(parent, lookup, startIndex, period))
                {
                    return false;
                }

                // next parent, next segment, and find the next period
                parent = parent.Parent;
                startIndex = period + 1;
                period = lookup.IndexOf('.', startIndex);
            }

            // now check the last segment, from start to the end of the string
            return MatchMemberName(parent, lookup, startIndex, lookup.Length);
        }

        internal override bool IsDebuggerStatement
        {
            get
            {
                // if we don't want to strip debug statements, then nothing is a debug statement
                if (Parser.Settings.StripDebugStatements)
                {
                    // we want to look through the parser's debug lookup list (if there is one)
                    // and see if we match any of the debug lookups specified therein.
                    foreach (var lookup in Parser.DebugLookups)
                    {
                        // see if there's a period in this lookup
                        var firstPeriod = lookup.IndexOf('.');
                        if (firstPeriod > 0)
                        {
                            // this lookup is a member chain, so check our name against that
                            // first part before the period; if it matches, we need to walk up the parent tree
                            if (string.CompareOrdinal(Name, 0, lookup, 0, firstPeriod) == 0)
                            {
                                // we matched the first one; test the rest of the chain
                                if (MatchesMemberChain(Parent, lookup, firstPeriod + 1))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            // just a straight comparison
                            if (string.CompareOrdinal(Name, lookup) == 0)
                            {
                                // we found a match
                                return true;
                            }
                        }
                    }
                }

                // if we get here, we didn't find a match
                return false;
            }
        }

        //code in parser relies on this.name being returned from here
        public override String ToString()
        {
            return Name;
        }

        #region INameReference Members

        public JsActivationObject VariableScope
        {
            get
            {
                // get the enclosing scope from the node, but that might be 
                // a block scope -- we only want variable scopes: functions or global.
                // so walk up until we find one.
                var enclosingScope = this.EnclosingScope;
                while (enclosingScope is JsBlockScope)
                {
                    enclosingScope = enclosingScope.Parent;
                }

                return enclosingScope;
            }
        }

        #endregion
    }
}
