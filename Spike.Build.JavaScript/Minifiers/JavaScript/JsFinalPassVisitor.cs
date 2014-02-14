﻿// FinalPassVisitor.cs
//
// Copyright 2011 Microsoft Corporation
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
using System.Collections.Generic;
using System.Text;

namespace Spike.Build.Client
{
    internal class JsFinalPassVisitor : JsTreeVisitor
    {
        private JsParser m_parser;
        private JsStatementStartVisitor m_statementStart;

        private JsFinalPassVisitor(JsParser parser)
        {
            m_parser = parser;
            m_statementStart = new JsStatementStartVisitor();
        }

        public static void Apply(JsAstNode node, JsParser parser)
        {
            var visitor = new JsFinalPassVisitor(parser);
            node.Accept(visitor);
        }

        public override void Visit(JsBinaryOperator node)
        {
            if (node != null)
            {
                // if this isn't a comma-operator, just recurse normal.
                // if it is and this is the root block (parent is null) or a function block
                // or there's already more than one statement in the block, we will want to possibly break
                // this comma-operator expression statement into separate expression statements.
                JsBlock parentBlock;
                if (node.OperatorToken == JsToken.Comma
                    && m_parser.Settings.IsModificationAllowed(JsTreeModifications.UnfoldCommaExpressionStatements)
                    && ((parentBlock = node.Parent as JsBlock) != null)
                    && (parentBlock.Parent == null
                        || parentBlock.Parent is JsFunctionObject
                        || parentBlock.Parent is JsTryNode
                        || parentBlock.Parent is JsSwitchCase
                        || parentBlock.Count > 1))
                {
                    // possibly break this one comma statement into multiple statements and recurse
                    PossiblyBreakExpressionStatement(node, parentBlock);
                }
                else
                {
                    // just recurse it normally
                    base.Visit(node);
                }
            }
        }

        private void PossiblyBreakExpressionStatement(JsBinaryOperator node, JsBlock parentBlock)
        {
            var nodeList = node.Operand2 as JsAstNodeList;
            if (nodeList != null)
            {
                PossiblyBreakExpressionList(node, parentBlock, nodeList);
            }
            else
            {
                //  not a list
                if (CanBeBroken(node.Operand2))
                {
                    // flatten the operator. We have to explicitly recurse the left-hand side.
                    var temp = node.Operand1;
                    parentBlock.ReplaceChild(node, temp);
                    parentBlock.InsertAfter(temp, node.Operand2);
                    temp.Accept(this);
                }
                else
                {
                    // no change; just recurse normally
                    base.Visit(node);
                }
            }
        }

        private void PossiblyBreakExpressionList(JsBinaryOperator node, JsBlock parentBlock, JsAstNodeList nodeList)
        {
            // if the first item can be broken, then we an break it and be done.
            // otherwise we're going to have to walk until we find a breaking place
            if (CanBeBroken(nodeList[0]))
            {
                // break the first item. insert the left-hand side at our position and
                // recurse it. Then rotate the node.
                var index = parentBlock.IndexOf(node);
                var temp = node.Operand1;
                RotateOpeator(node, nodeList);
                parentBlock.Insert(index, temp);

                // assumes nothing will cause the node to be deleted, because then it
                // would cause us to miss the following item
                temp.Accept(this);
            }
            else
            {
                // the first one can't be broken, so find the first one that can (if any)
                for (var ndx = 1; ndx < nodeList.Count; ++ndx)
                {
                    if (CanBeBroken(nodeList[ndx]))
                    {
                        if (ndx == 1)
                        {
                            // the second item is where we are breaking it, so we're going to pull
                            // the first item, replace the list with that first item, then insert
                            // a new comma operator after the current node
                            var temp = nodeList[0];
                            nodeList.RemoveAt(0);
                            node.Operand2 = temp;

                            // if there's nothing left, then let it die. Otherwise split off
                            // the remainder and insert after the current item.
                            if (nodeList.Count > 0)
                            {
                                parentBlock.InsertAfter(node, CreateSplitNodeFromEnd(nodeList, 0));
                            }
                        }
                        else
                        {
                            // split off items from the index where we want to split, and insert
                            // it after the current node and leave the node list where it is.
                            parentBlock.InsertAfter(node, CreateSplitNodeFromEnd(nodeList, ndx));
                        }
                        
                        // and now that we've broken it, bail.
                        break;
                    }
                }

                // regardless if anything changed, recurse this node now
                base.Visit(node);
            }
        }

        private static JsAstNode CreateSplitNodeFromEnd(JsAstNodeList nodeList, int ndx)
        {
            JsAstNode newNode;
            if (ndx == nodeList.Count - 1)
            {
                // the LAST one can be broken. Pull it off the list and we will just
                // insert it after the current node.
                newNode = nodeList[ndx];
                nodeList.RemoveAt(ndx);
            }
            else if (ndx == nodeList.Count - 2)
            {
                // the PENULTIMATE item can be broken. So create a new comma operator
                // with the just the last two item and we'll insert it after the current node
                var left = nodeList[ndx];
                nodeList.RemoveAt(ndx);
                var right = nodeList[ndx];
                nodeList.RemoveAt(ndx);

                newNode = new JsCommaOperator(null, nodeList.Parser)
                    {
                        Operand1 = left,
                        Operand2 = right
                    };
            }
            else
            {
                // at least three items will be pulled off, which means there will
                // be at least two items on the right, so we'll create a new astlist to
                // insert those items into a new comma operator
                var left = nodeList[ndx];
                nodeList.RemoveAt(ndx);

                // if we were passed zero, then just reuse the node list.
                // otherwise we need to create a new one and move the items
                // from the index position over.
                JsAstNodeList right;
                if (ndx == 0)
                {
                    right = nodeList;
                }
                else
                {
                    right = new JsAstNodeList(null, nodeList.Parser);
                    while (ndx < nodeList.Count)
                    {
                        var temp = nodeList[ndx];
                        nodeList.RemoveAt(ndx);
                        right.Append(temp);
                    }
                }

                newNode = new JsCommaOperator(null, nodeList.Parser)
                    {
                        Operand1 = left,
                        Operand2 = right
                    };
            }

            return newNode;
        }

        private static void RotateOpeator(JsBinaryOperator node, JsAstNodeList rightSide)
        {
            if (rightSide.Count == 0)
            {
                // the list is empty -- remove the node altogether
                node.Parent.ReplaceChild(node, null);
            }
            else if (rightSide.Count == 1)
            {
                // the list has only one item -- replace the node with the one item
                node.Parent.ReplaceChild(node, rightSide[0]);
            }
            else if (rightSide.Count == 2)
            {
                // there are only two items -- rotate the first to the left-hand side
                // and replace the right-hand side with the second item
                node.Operand1 = rightSide[0];
                node.Operand2 = rightSide[1];
            }
            else
            {
                // there will still be more than one left in the list after we peel off the
                // first one. rotate the first item to the left-hand side
                var temp = rightSide[0];
                rightSide.RemoveAt(0);
                node.Operand1 = temp;
            }
        }

        private bool CanBeBroken(JsAstNode node)
        {
            JsAstNodeList nodeList;
            if (!m_statementStart.IsSafe(node))
            {
                // don't break if the next statement is a function or an object literal
                return false;
            }
            else if ((nodeList = node as JsAstNodeList) != null)
            {
                // if there aren't any operands in the list, we can break this,
                // otherwise check to see if the first item can be broken.
                return nodeList.Count == 0 || CanBeBroken(nodeList[0]);
            }

            // if we get here, it's okay to break this operation into separate statements
            return true;
        }

        public override void Visit(JsConstantWrapper node)
        {
            if (node != null)
            {
                // no children, so don't bother calling the base.
                if (node.PrimitiveType == JsPrimitiveType.Boolean
                    && m_parser.Settings.IsModificationAllowed(JsTreeModifications.BooleanLiteralsToNotOperators))
                {
                    node.Parent.ReplaceChild(node, new JsUnaryOperator(node.Context, m_parser)
                        {
                            Operand = new JsConstantWrapper(node.ToBoolean() ? 0 : 1, JsPrimitiveType.Number, node.Context, m_parser),
                            OperatorToken = JsToken.LogicalNot
                        });
                }
            }
        }
    }
}
