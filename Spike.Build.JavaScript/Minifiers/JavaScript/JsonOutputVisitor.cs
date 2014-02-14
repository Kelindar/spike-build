﻿// JsonOutputVisitor.cs
//
// Copyright 2012 Microsoft Corporation
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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Spike.Build.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// output JSON-compatible code
    /// </summary>
    internal class JsonOutputVisitor : IJsVisitor
    {
        // this is a regular expression that we'll use to minimize numeric values
        // that don't employ the e-notation
        private static Regex s_decimalFormat = new Regex(
            @"^\s*\+?(?<neg>\-)?0*(?<mag>(?<sig>\d*[1-9])(?<zer>0*))?(\.(?<man>\d*[1-9])?0*)?(?<exp>E\+?(?<eng>\-?)0*(?<pow>[1-9]\d*))?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private TextWriter m_writer;

        public bool IsValid
        {
            get;
            private set;
        }

        private JsonOutputVisitor(TextWriter writer)
        {
            m_writer = writer;
            IsValid = true;
        }

        public static bool Apply(TextWriter writer, JsAstNode node)
        {
            if (node != null)
            {
                var visitor = new JsonOutputVisitor(writer);
                node.Accept(visitor);
                return visitor.IsValid;
            }

            return false;
        }

        #region supported nodes

        public void Visit(JsArrayLiteral node)
        {
            if (node != null)
            {
                m_writer.Write('[');
                if (node.Elements != null)
                {
                    node.Elements.Accept(this);
                }

                m_writer.Write(']');
            }
        }

        public void Visit(JsAstNodeList node)
        {
            if (node != null)
            {
                for (var ndx = 0; ndx < node.Count; ++ndx)
                {
                    if (ndx > 0)
                    {
                        m_writer.Write(',');
                    }

                    if (node[ndx] != null)
                    {
                        node[ndx].Accept(this);
                    }
                }
            }
        }

        public void Visit(JsBlock node)
        {
            if (node != null && node.Count > 0)
            {
                // there should only be one "statement"
                node[0].Accept(this);
            }
        }

        public void Visit(JsConstantWrapper node)
        {
            if (node != null)
            {
                // allow string, number, true, false, and null.
                switch (node.PrimitiveType)
                {
                    case JsPrimitiveType.Boolean:
                        m_writer.Write((bool)node.Value ? "true" : "false");
                        break;

                    case JsPrimitiveType.Null:
                        m_writer.Write("null");
                        break;

                    case JsPrimitiveType.Number:
                        OutputNumber((double)node.Value, node.Context);
                        break;

                    case JsPrimitiveType.String:
                    case JsPrimitiveType.Other:
                        // string -- or treat it like a string
                        OutputString(node.Value.ToString());
                        break;
                }
            }
        }

        public void Visit(JsCustomNode node)
        {
            if (node != null)
            {
                // whatever people plug in. Hopefully it's valid JSON.
                OutputString(node.ToCode());
            }
        }

        public void Visit(JsUnaryOperator node)
        {
            if (node != null)
            {
                // only a negation is allowed -- and even then, I'm not sure
                // if it has already been integrated into the numeric value yet.
                if (node.OperatorToken == JsToken.Minus)
                {
                    m_writer.Write('-');
                    if (node.Operand != null)
                    {
                        node.Operand.Accept(this);
                    }
                }
                else
                {
                    // invalid! ignore
                    IsValid = false;
                }
            }
        }

        public void Visit(JsObjectLiteral node)
        {
            if (node != null)
            {
                m_writer.Write('{');
                if (node.Properties != null)
                {
                    node.Properties.Accept(this);
                }

                m_writer.Write('}');
            }
        }

        public void Visit(JsObjectLiteralField node)
        {
            if (node != null)
            {
                if (node.PrimitiveType == JsPrimitiveType.String)
                {
                    // must be double-quoted string with a limited number of escapes
                    OutputString(node.Value.ToString());
                }
                else
                {
                    // really the property names can only be strings, so this 
                    // branch means the input was invalid.
                    m_writer.Write('"');
                    Visit(node as JsConstantWrapper);
                    m_writer.Write('"');
                }
            }
        }

        public void Visit(JsObjectLiteralProperty node)
        {
            if (node != null)
            {
                if (node.Name != null)
                {
                    node.Name.Accept(this);
                }

                m_writer.Write(':');

                if (node.Value != null)
                {
                    node.Value.Accept(this);
                }
            }
        }

        #endregion 

        #region unsupported nodes

        public void Visit(JsAspNetBlockNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsBinaryOperator node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsBreak node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsCallNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationComment node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationElse node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationElseIf node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationEnd node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationIf node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationOn node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditionalCompilationSet node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConditional node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConstantWrapperPP node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsConstStatement node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsContinueNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsDebuggerNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsDirectivePrologue node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsDoWhile node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsEmptyStatement node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsForIn node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsForNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsFunctionObject node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsGetterSetter node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsGroupingOperator node)
        {
            // not TECHNICALLY valid! set the invalid flag, but
            // still recurse the operand, just in case
            IsValid = false;
            if (node != null && node.Operand != null)
            {
                node.Operand.Accept(this);
            }
        }

        public void Visit(JsIfNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsImportantComment node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsLabeledStatement node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsLexicalDeclaration node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsLookup node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsMember node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsParameterDeclaration node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsRegExpLiteral node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsReturnNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsSwitch node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsSwitchCase node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsThisLiteral node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsThrowNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsTryNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsVar node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsVariableDeclaration node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsWhileNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        public void Visit(JsWithNode node)
        {
            // invalid! ignore
            IsValid = false;
        }

        #endregion

        #region string formatting method

        private void OutputString(string text)
        {
            // must be double-quote delimited
            m_writer.Write('"');
            for (var ndx = 0; ndx < text.Length; ++ndx)
            {
                var ch = text[ndx];
                switch (ch)
                {
                    case '\"':
                        m_writer.Write("\\\"");
                        break;

                    case '\b':
                        m_writer.Write("\\b");
                        break;

                    case '\f':
                        m_writer.Write("\\f");
                        break;

                    case '\n':
                        m_writer.Write("\\n");
                        break;

                    case '\r':
                        m_writer.Write("\\r");
                        break;

                    case '\t':
                        m_writer.Write("\\t");
                        break;

                    default:
                        if (ch < ' ')
                        {
                            // other control characters must be escaped as \uXXXX
                            m_writer.Write("\\u{0:x4}", (int)ch);
                        }
                        else
                        {
                            // just append it. The output encoding will take care of the rest
                            m_writer.Write(ch);
                        }
                        break;
                }
            }

            m_writer.Write('"');
        }

        #endregion

        #region numeric formatting methods

        public void OutputNumber(double numericValue, JsContext originalContext)
        {
            // numerics are doubles in JavaScript, so force it now as a shortcut
            if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
            {
                // weird number -- just return the original source code as-is. 
                if (originalContext != null && !string.IsNullOrEmpty(originalContext.Code)
                    && !originalContext.Document.IsGenerated)
                {
                    m_writer.Write(originalContext.Code);
                    return;
                }

                // Hmmm... don't have an original source. 
                // Must be generated. Just generate the proper JS literal.
                //
                // DANGER! If we just output NaN and Infinity and -Infinity blindly, that assumes
                // that there aren't any local variables in this scope chain with that
                // name, and we're pulling the GLOBAL properties. Might want to use properties
                // on the Number object -- which, of course, assumes that Number doesn't
                // resolve to a local variable...
                string objectName = double.IsNaN(numericValue) ? "NaN" : "Infinity";

                // we're good to go -- just return the name because it will resolve to the
                // global properties (make a special case for negative infinity)
                m_writer.Write(double.IsNegativeInfinity(numericValue) ? "-Infinity" : objectName);
            }
            else if (numericValue == 0)
            {
                // special case zero because we don't need to go through all those
                // gyrations to get a "0" -- and because negative zero is different
                // than a positive zero
                m_writer.Write(1 / numericValue < 0 ? "-0" : "0");
            }
            else
            {
                // normal string representations
                m_writer.Write(GetSmallestRep(numericValue.ToString("R", CultureInfo.InvariantCulture)));
            }
        }

        private static string GetSmallestRep(string number)
        {
            var match = s_decimalFormat.Match(number);
            if (match.Success)
            {
                string mantissa = match.Result("${man}");
                if (string.IsNullOrEmpty(match.Result("${exp}")))
                {
                    if (string.IsNullOrEmpty(mantissa))
                    {
                        // no decimal portion
                        if (string.IsNullOrEmpty(match.Result("${sig}")))
                        {
                            // no non-zero digits in the magnitude either -- must be a zero
                            number = match.Result("${neg}") + "0";
                        }
                        else
                        {
                            // see if there are trailing zeros
                            // that we can use e-notation to make smaller
                            int numZeros = match.Result("${zer}").Length;
                            if (numZeros > 2)
                            {
                                number = match.Result("${neg}") + match.Result("${sig}")
                                    + 'e' + numZeros.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    else
                    {
                        // there is a decimal portion. Put it back together
                        // with the bare-minimum stuff -- no plus-sign, no leading magnitude zeros,
                        // no trailing mantissa zeros. A zero magnitude won't show up, either.
                        number = match.Result("${neg}") + match.Result("${mag}") + '.' + mantissa;
                    }
                }
                else if (string.IsNullOrEmpty(mantissa))
                {
                    // there is an exponent, but no significant mantissa
                    number = match.Result("${neg}") + match.Result("${mag}")
                        + "e" + match.Result("${eng}") + match.Result("${pow}");
                }
                else
                {
                    // there is an exponent and a significant mantissa
                    // we want to see if we can eliminate it and save some bytes

                    // get the integer value of the exponent
                    int exponent;
                    if (int.TryParse(match.Result("${eng}") + match.Result("${pow}"), NumberStyles.Integer, CultureInfo.InvariantCulture, out exponent))
                    {
                        // slap the mantissa directly to the magnitude without a decimal point.
                        // we'll subtract the number of characters we just added to the magnitude from
                        // the exponent
                        number = match.Result("${neg}") + match.Result("${mag}") + mantissa
                            + 'e' + (exponent - mantissa.Length).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // should n't get here, but it we do, go with what we have
                        number = match.Result("${neg}") + match.Result("${mag}") + '.' + mantissa
                            + 'e' + match.Result("${eng}") + match.Result("${pow}");
                    }
                }
            }

            return number;
        }

        #endregion
    }
}
