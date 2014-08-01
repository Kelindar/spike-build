﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Spike.Build.CSharp5
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class TcpChannelTemplate : TcpChannelTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"
using Spike.Network.Packets;
using Spike.Network.CustomTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spike.Network
{
	public class TcpChannel : TcpChannelBase<TcpChannel>
	{
		//Events
");
            
            #line 22 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
		foreach(var receive in Model.Receives)
		{ 
            
            #line default
            #line hidden
            this.Write("\t\t\r\n\t\tpublic event Action<TcpChannel, ");
            
            #line 24 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Name));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 24 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Name));
            
            #line default
            #line hidden
            this.Write("; \r\n");
            
            #line 25 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
		} 
            
            #line default
            #line hidden
            this.Write("\t\t    \r\n\t\t//Sends        \r\n");
            
            #line 28 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
		foreach(var send in Model.Sends)
		{ 
            
            #line default
            #line hidden
            this.Write("\t\t\r\n\t\tpublic async void ");
            
            #line 30 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(send.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 30 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
	
			var first = true;
			foreach(var member in send.Members){
				if(!first)
					Write(", ");

				Write(CSharpBuilder.GetNativeType(member));
				Write(" ");
				Write(member.Name);
				first = false;
			}
		
            
            #line default
            #line hidden
            this.Write(")\r\n\t\t{\r\n\t\t\tBeginNewPacket(0x");
            
            #line 43 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(send.Id.ToString("X")));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 44 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			foreach(var member in send.Members)
			{ 
            
            #line default
            #line hidden
            this.Write("\t\t\tPacketWrite(");
            
            #line 46 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Name));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 47 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			} 
            
            #line default
            #line hidden
            this.Write("\t\t\tawait SendPacket(");
            
            #line 48 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(send.Compressed ? "true" : "false"));
            
            #line default
            #line hidden
            this.Write(");\r\n\t\t}\t\t \r\n");
            
            #line 50 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
		} 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t//Dispatcher\r\n\t\tprotected override void OnReceive(uint key)\r\n\t\t{\r\n\t\t\tswitch (" +
                    "key)\r\n\t\t\t{\r\n");
            
            #line 57 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
				foreach(var receive in Model.Receives)
				{ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\r\n\t\t\t\tcase 0x");
            
            #line 59 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Id.ToString("X")));
            
            #line default
            #line hidden
            this.Write("u:\r\n\t\t\t\t{\r\n\t\t\t\t\tvar packet = new ");
            
            #line 61 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Name));
            
            #line default
            #line hidden
            this.Write("();\r\n\t\t\t\t\tBeginReadPacket(");
            
            #line 62 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Compressed ? "true" : "false"));
            
            #line default
            #line hidden
            this.Write(");\r\n\t\t\t\t\t\r\n");
            
            #line 64 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
					foreach(var member in receive.Members){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\tpacket.");
            
            #line 65 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Name));
            
            #line default
            #line hidden
            this.Write(" = PacketRead");
            
            #line 65 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.IsList ? "ListOf" : string.Empty));
            
            #line default
            #line hidden
            
            #line 65 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Type));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 66 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
					} 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t\t\t\t//Now Call event\r\n\t\t\t\t\tif (");
            
            #line 69 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Name));
            
            #line default
            #line hidden
            this.Write(" != null)\r\n\t\t\t\t\t\t");
            
            #line 70 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(receive.Name));
            
            #line default
            #line hidden
            this.Write("(this, packet);\r\n\r\n\t\t\t\t\tbreak;\r\n\t\t\t\t}\r\n");
            
            #line 74 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
				} 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t\t\tdefault:\r\n\t\t\t\t\tDebug.WriteLine(\"Unknow packet : {0:X}\", key);\r\n\t\t\t\t\treturn;" +
                    "\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\t//Custom Type\r\n");
            
            #line 83 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
		foreach(var customType in Model.CustomTypes)
		{ 
            
            #line default
            #line hidden
            this.Write("\t\tprotected ");
            
            #line 85 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write(" PacketRead");
            
            #line 85 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("()\r\n        {\r\n            var value = new ");
            
            #line 87 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 88 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			foreach(var member in customType.Members)
			{ 
            
            #line default
            #line hidden
            this.Write("\t\t\tvalue.");
            
            #line 90 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Name));
            
            #line default
            #line hidden
            this.Write(" = PacketRead");
            
            #line 90 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.IsList ? "ListOf" : string.Empty));
            
            #line default
            #line hidden
            
            #line 90 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Type));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 91 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			} 
            
            #line default
            #line hidden
            this.Write("\t\t\treturn value;\r\n        }\r\n        protected void PacketWrite(");
            
            #line 94 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write(" value)\r\n        {\r\n            ");
            
            #line 96 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			foreach(var member in customType.Members)
			{ 
            
            #line default
            #line hidden
            this.Write("\t\t\tPacketWrite(value.");
            
            #line 98 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Name));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 99 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
			} 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n        protected ");
            
            #line 102 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("[] PacketReadListOf");
            
            #line 102 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("()\r\n        {\r\n            var value = new ");
            
            #line 104 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("[PacketReadInt32()];\r\n            for (int index = 0; index < value.Length; index" +
                    "++)\r\n                value[index] = PacketRead");
            
            #line 106 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("();\r\n            return value;\r\n        }\r\n        protected void PacketWrite(");
            
            #line 109 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(customType.Name));
            
            #line default
            #line hidden
            this.Write("[] value)\r\n        {\r\n            PacketWrite(value.Length);\r\n            foreach" +
                    " (var element in value)\r\n                PacketWrite(element);\r\n        }\r\n");
            
            #line 115 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"
				} 
            
            #line default
            #line hidden
            this.Write("\r\n\t}\r\n}");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\Users\Fabian\Desktop\Projects\GitHub\spike-build\Spike.Build.CSharp5\TcpChannelTemplate.tt"

private global::Spike.Build.Model _ModelField;

/// <summary>
/// Access the Model parameter of the template.
/// </summary>
private global::Spike.Build.Model Model
{
    get
    {
        return this._ModelField;
    }
}


/// <summary>
/// Initialize the template
/// </summary>
public virtual void Initialize()
{
    if ((this.Errors.HasErrors == false))
    {
bool ModelValueAcquired = false;
if (this.Session.ContainsKey("Model"))
{
    this._ModelField = ((global::Spike.Build.Model)(this.Session["Model"]));
    ModelValueAcquired = true;
}
if ((ModelValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Model");
    if ((data != null))
    {
        this._ModelField = ((global::Spike.Build.Model)(data));
    }
}


    }
}


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class TcpChannelTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
