﻿namespace ScTools.ScriptLang.Ast.Errors
{
    using ScTools.ScriptLang.Ast.Declarations;
    using ScTools.ScriptLang.Ast.Types;

    public sealed class ErrorDeclaration : BaseError, IDeclaration, IValueDeclaration, ITypeDeclaration, ILabelDeclaration
    {
        public string Name { get; set; }
        public IType Type { get; set; }

        public ErrorDeclaration(SourceRange source, Diagnostic diagnostic) : base(source, diagnostic)
            => (Name, Type) = ("#ERROR#", new ErrorType(source, Diagnostic));

        public ErrorDeclaration(SourceRange source, DiagnosticsReport diagnostics, string message) : base(source, diagnostics, message)
            => (Name, Type) = ("#ERROR#", new ErrorType(source, Diagnostic));

        public IType CreateType(SourceRange source) => new ErrorType(source, Diagnostic);

        public override TReturn Accept<TReturn, TParam>(IVisitor<TReturn, TParam> visitor, TParam param)
            => visitor.Visit(this, param);
    }
}
