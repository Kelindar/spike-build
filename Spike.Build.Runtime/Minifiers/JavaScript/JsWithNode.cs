// with.cs
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
using System.Collections.Generic;
using System.Text;

namespace Spike.Build.Minifiers
{
    internal sealed class JsWithNode : JsAstNode
    {
        private JsAstNode m_withObject;
        private JsBlock m_body;

        public JsAstNode WithObject
        {
            get { return m_withObject; }
            set
            {
                m_withObject.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_withObject = value;
                m_withObject.IfNotNull(n => n.Parent = this);
            }
        }

        public JsBlock Body
        {
            get { return m_body; }
            set
            {
                m_body.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_body = value;
                m_body.IfNotNull(n => n.Parent = this);
            }
        }

        public override JsContext TerminatingContext
        {
            get
            {
                // if we have one, return it. If not, return what the body has (if any)
                return base.TerminatingContext ?? Body.IfNotNull(b => b.TerminatingContext);
            }
        }

        public JsWithNode(JsContext context, JsParser parser)
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
                return EnumerateNonNullNodes(WithObject, Body);
            }
        }

        public override bool ReplaceChild(JsAstNode oldNode, JsAstNode newNode)
        {
            if (WithObject == oldNode)
            {
                WithObject = newNode;
                return true;
            }
            if (Body == oldNode)
            {
                Body = ForceToBlock(newNode);
                return true;
            }
            return false;
        }

        internal override bool RequiresSeparator
        {
            get
            {
                // requires a separator if the body does
                return Body == null || Body.Count == 0 ? false : Body.RequiresSeparator;
            }
        }

        internal override bool EncloseBlock(EncloseBlockType type)
        {
            // pass the query on to the body
            return Body == null ? false : Body.EncloseBlock(type);
        }
    }
}