﻿namespace ScTools.ScriptLang.SymbolTables
{
    using System.Collections.Generic;

    using ScTools.ScriptLang.Ast.Declarations;

    /// <summary>
    /// Table with all the labels available in a function or procedure.
    /// </summary>
    public sealed class LabelSymbolTable
    {
        private readonly Dictionary<string, ILabelDeclaration> labels = new(Parser.CaseInsensitiveComparer);

        public LabelSymbolTable()
        {
        }

        public bool AddLabel(ILabelDeclaration label) => labels.TryAdd(label.Name, label);

        public ILabelDeclaration? FindLabel(string name) => labels.TryGetValue(name, out var decl) ? decl : null;
    }
}
