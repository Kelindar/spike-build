// forin.cs
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

using System.Collections.Generic;
using System.Text;

namespace Spike.Build.Client
{
    internal sealed class JsForIn : JsIterationStatement
    {
        private JsAstNode m_variable;
        private JsAstNode m_collection;

        public JsAstNode Variable
        {
            get { return m_variable; }
            set
            {
                m_variable.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_variable = value;
                m_variable.IfNotNull(n => n.Parent = this);
            }
        }

        public JsAstNode Collection
        {
            get { return m_collection; }
            set
            {
                m_collection.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_collection = value;
                m_collection.IfNotNull(n => n.Parent = this);
            }
        }

        public JsContext OperatorContext { get; set; }

        public JsBlockScope BlockScope { get; set; }

        public override JsContext TerminatingContext
        {
            get
            {
                // if we have one, return it. If not, return what the body has (if any)
                return base.TerminatingContext ?? Body.IfNotNull(b => b.TerminatingContext);
            }
        }

        public JsForIn(JsContext context, JsParser parser)
            : base(context, parser)
        {
        }

        public override void Accept(IJsVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }

        public override IEnumerable<JsAstNode> Children
        {
            get
            {
                return EnumerateNonNullNodes(Variable, Collection, Body);
            }
        }

        public override bool ReplaceChild(JsAstNode oldNode, JsAstNode newNode)
        {
            if (Variable == oldNode)
            {
                Variable = newNode;
                return true;
            }
            if (Collection == oldNode)
            {
                Collection = newNode;
                return true;
            }
            if (Body == oldNode)
            {
                Body = ForceToBlock(newNode);
                return true;
            }
            return false;
        }

        internal override bool EncloseBlock(EncloseBlockType type)
        {
            // pass the query on to the body
            return Body == null ? false : Body.EncloseBlock(type);
        }

        internal override bool RequiresSeparator
        {
            get
            {
                // requires a separator if the body does
                return Body == null || Body.Count == 0 ? false : Body.RequiresSeparator;
            }
        }
    }
}
