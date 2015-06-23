﻿// ContextError.cs
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
using System.Text;

namespace Spike.Build.Minifiers
{
    /// <summary>
    /// Represents a context error of a minifier.
    /// </summary>
    public class MinifierError
    {
        /// <summary>
        /// Gets whether this is an error.
        /// </summary>
        public bool IsError { get; protected set; }

        /// <summary>
        /// Gets the serverity value of the error.
        /// </summary>
        public int Severity { get; protected set; }

        /// <summary>
        /// Gets the subcategory of the error.
        /// </summary>
        public string Subcategory { get; protected set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; protected set; }

        /// <summary>
        /// Gets the help keyword for this error.
        /// </summary>
        public string HelpKeyword { get; protected set; }

        /// <summary>
        /// Gets the file where the error occured.
        /// </summary>
        public string File { get; protected set; }

        /// <summary>
        /// Gets the start line of the error.
        /// </summary>
        public int StartLine { get; protected set; }

        /// <summary>
        /// Gets the start column of the error.
        /// </summary>
        public int StartColumn { get; protected set; }

        /// <summary>
        /// Gets the end line of the error.
        /// </summary>
        public int EndLine { get; protected set; }

        /// <summary>
        /// Gets the end column of the error.
        /// </summary>
        public int EndColumn { get; protected set; }

        /// <summary>
        /// Gets the message for this error.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Constructs a new instance of a <see cref="MinifierError"/>.
        /// </summary>
        /// <param name="isError">Whether this is an error.</param>
        /// <param name="severity">The severity of this error.</param>
        /// <param name="subcategory">The subcategory of this error.</param>
        /// <param name="errorCode">The error code of this error.</param>
        /// <param name="helpKeyword">The help keyword of this error.</param>
        /// <param name="file">The file of this error.</param>
        /// <param name="startLine">The start line of this error.</param>
        /// <param name="startColumn">The start column of this error.</param>
        /// <param name="endLine">The end line of this error.</param>
        /// <param name="endColumn">The end column of this error.</param>
        /// <param name="message">The message of this error.</param>
        public MinifierError(bool isError, int severity, string subcategory, string errorCode, string helpKeyword, string file, int startLine, int startColumn, int endLine, int endColumn, string message)
        {
            // transfer the values as-is
            IsError = isError;
            Severity = severity;
            Subcategory = subcategory;
            ErrorCode = errorCode;
            HelpKeyword = helpKeyword;
            File = file;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
            Message = message;
        }

        /// <summary>
        /// Convert the exception to a VisualStudio format error message
        /// file(startline[-endline]?,startcol[-endcol]?):[ subcategory] category [errorcode]: message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(File))
            {
                sb.Append(File);
            }

            // if there is a startline, then there must be a location.
            // no start line, then no location
            if (StartLine > 0)
            {
                // we will always at least start with the start line
                sb.AppendFormat("({0}", StartLine);

                if (EndLine > StartLine)
                {
                    if (StartColumn > 0 && EndColumn > 0)
                    {
                        // all four values were specified
                        sb.AppendFormat(",{0},{1},{2}", StartColumn, EndLine, EndColumn);
                    }
                    else
                    {
                        // one or both of the columns wasn't specified, so ignore them both
                        sb.AppendFormat("-{0}", EndLine);
                    }
                }
                else if (StartColumn > 0)
                {
                    sb.AppendFormat(",{0}", StartColumn);
                    if (EndColumn > StartColumn)
                    {
                        sb.AppendFormat("-{0}", EndColumn);
                    }
                }

                sb.Append(')');
            }

            // seaprate the location from the error description
            sb.Append(':');

            // if there is a subcategory, add it prefaced with a space
            if (!string.IsNullOrEmpty(Subcategory))
            {
                sb.Append(' ');
                sb.Append(Subcategory);
            }

            // not localizable
            sb.Append(IsError ? " error " : " warning ");

            // if there is an error code
            if (!string.IsNullOrEmpty(ErrorCode))
            {
                sb.Append(ErrorCode);
            }

            // separate description from the message
            sb.Append(": ");

            if (!string.IsNullOrEmpty(Message))
            {
                sb.Append(Message);
            }

            return sb.ToString();
        }
    }
}
